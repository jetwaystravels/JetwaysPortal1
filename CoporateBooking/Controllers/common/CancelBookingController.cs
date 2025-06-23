using DomainLayer.Model;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OnionConsumeWebAPI.Extensions;
using System.Net.Http.Headers;
using System.Text;
using Utility;

namespace CoporateBooking.Controllers.common
{
    public class CancelBookingController : Controller
    {
        private _credentials _credentialsAirasia = null;

        public async Task<IActionResult> CancelActionAsync(int airline, string pnr)
        {
            //string baseUrl = "https://dotrezapi.test.I5.navitaire.com";
            //string recordLocator = pnr;
            string recordLocator = "D8G1KH";

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(AppUrlConstant.BaseURL);

                // Step 1: Fetch Airline Credentials
                HttpResponseMessage response = await client.GetAsync(AppUrlConstant.AirlineLogin);
                if (!response.IsSuccessStatusCode)
                {
                    ModelState.AddModelError(string.Empty, "Failed to retrieve airline credentials.");
                    return View("Error");
                }

                var results = await response.Content.ReadAsStringAsync();
                var jsonObject = JsonConvert.DeserializeObject<List<_credentials>>(results);
                _credentialsAirasia = jsonObject.FirstOrDefault(cred => cred?.FlightCode == 1); // AirAsia

                // Step 2: Login to get AirAsia token
                var login = new airlineLogin { credentials = _credentialsAirasia };
                var AirasiaTokan = new AirasiaTokan();

                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage tokenResponse = await client.PostAsJsonAsync(AppUrlConstant.AirasiaTokan, login);

                if (!tokenResponse.IsSuccessStatusCode)
                {
                    ModelState.AddModelError("", "AirAsia token request failed.");
                    return View("Error");
                }

                var tokenResult = await tokenResponse.Content.ReadAsStringAsync();
                dynamic tokenJson = JsonConvert.DeserializeObject<dynamic>(tokenResult);
                AirasiaTokan.token = tokenJson.data.token;

                // Set Auth Header
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AirasiaTokan.token);

                // Step 3: GET /booking/retrieve/byRecordLocator/{pnr}
                string retrieveUrl = $"{AppUrlConstant.AirasiaPNRBooking}/{recordLocator}";
                HttpResponseMessage retrieveResponse = await client.GetAsync(retrieveUrl);
                if (!retrieveResponse.IsSuccessStatusCode)
                {
                    string err = await retrieveResponse.Content.ReadAsStringAsync();
                    ModelState.AddModelError("", $"Booking retrieval failed: {err}");
                    return View("Error");
                }

                string bookingResult = await retrieveResponse.Content.ReadAsStringAsync();
                TempData["BookingDetails"] = bookingResult;

                // Step 4: DELETE /v1/booking/journeys
                if (!await SendDeleteAsync(client, $"{AppUrlConstant.DeleteBooking}"))
                    return View("Error");

                // Step 5: PUT /v3/booking
                if (!await SendPutAsync(client, $"{AppUrlConstant.AirasiaCommitBooking}"))
                    return View("Error");

                // Step 6: GET /v1/booking (final confirmation)
                HttpResponseMessage finalGet = await client.GetAsync($"{AppUrlConstant.AirasiaGetBoking}");
                if (finalGet.IsSuccessStatusCode)
                {
                    string finalStatus = await finalGet.Content.ReadAsStringAsync();
                    //HttpResponseMessage response = await client.GetAsync(AppUrlConstant.CancleStatus);

                   
                    TempData["Success"] = "Booking cancellation session flow completed successfully.";
                    TempData["FinalStatus"] = finalStatus;
                    return RedirectToAction("MyBooking", "Booking");
                }
                else
                {
                    string error = await finalGet.Content.ReadAsStringAsync();
                    ModelState.AddModelError("", $"Final booking status check failed. {error}");
                    return View("Error");
                }
            }
        }

        // Helper: Generic DELETE (no body)
        private async Task<bool> SendDeleteAsync(HttpClient client, string url)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Delete, url);
            HttpResponseMessage response = await client.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                string error = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError("", $"DELETE failed at {url}: {error}");
                return false;
            }
            return true;
        }

        // Helper: Generic PUT (no body)
        private async Task<bool> SendPutAsync(HttpClient client, string url)
        {
            var body = new
            {
                notifyContacts = true,
                contactTypesToNotify = new[] { "P" }
            };

            var content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Put, url)
            {
                Content = content
            };

            HttpResponseMessage response = await client.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                string error = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError("", $"PUT failed at {url}: {error}");
                return false;
            }

            return true;
        }

    }

}
