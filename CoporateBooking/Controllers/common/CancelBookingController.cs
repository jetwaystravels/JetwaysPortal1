using CoporateBooking.Comman;
using DomainLayer.Model;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OnionConsumeWebAPI.Extensions;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using Utility;
using static DomainLayer.Model.ReturnTicketBooking;

namespace CoporateBooking.Controllers.common
{
    public class CancelBookingController : Controller
    {
        private _credentials _credentialsAirasia = null;

        public async Task<IActionResult> CancelActionAsync(int airline, string pnr)
        {
           

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
                string retrieveUrl = $"{AppUrlConstant.AirasiaPNRBooking}/{pnr}";
                HttpResponseMessage retrieveResponse = await client.GetAsync(retrieveUrl);
                if (!retrieveResponse.IsSuccessStatusCode)
                {
                    string err = await retrieveResponse.Content.ReadAsStringAsync();
                    ModelState.AddModelError("", $"Booking retrieval failed: {err}");
                    return View("Error");
                }

                string _bookingResult = await retrieveResponse.Content.ReadAsStringAsync();
                //var objpnrBooking = JsonConvert.DeserializeObject<dynamic>(_bookingResult);
               
                TempData["BookingDetails"] = _bookingResult;

                // Step 4: DELETE /v1/booking/journeys  
                var deleteResult = await HttpHelperService.SendDeleteAsync(client, $"{AppUrlConstant.DeleteBooking}");
                if (deleteResult.Success)
                {
                    // Step 5: PUT /v3/booking (commit)  
                    var putResult = await HttpHelperService.SendPutAsync(client, $"{AppUrlConstant.AirasiaCommitBooking}");
                    if (!putResult.Success)
                    {
                        ModelState.AddModelError("", $"PUT failed: {putResult.Error}");
                        return View("Error");
                    }

                    // Step 6: GET /v1/booking (final confirmation)  
                    HttpResponseMessage finalGet = await client.GetAsync($"{AppUrlConstant.AirasiaGetBoking}");
                    if (finalGet.IsSuccessStatusCode)
                    {
                        string _finalStatus = await finalGet.Content.ReadAsStringAsync();
                        var objcancelBooking = JsonConvert.DeserializeObject<dynamic>(_finalStatus);

                        Breakdown breakdown = new Breakdown();
                        breakdown.balanceDue = objcancelBooking.data.breakdown.balanceDue;
                        breakdown.totalAmount = objcancelBooking.data.breakdown.totalAmount;

                        var identity = (ClaimsIdentity)User.Identity;
                        IEnumerable<Claim> claims = identity.Claims;
                        var userEmail = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

                        Logs logs = new Logs();
                        logs.WriteLogs(_finalStatus, "Cancel", "AirAsiaOneWay", "oneway");

                        // Create request object for the API
                        var cancelRequest = new
                        {
                            RecordLocator = pnr,
                            Status = 3,
                            UserEmail = userEmail,
                            BalanceDue = breakdown.balanceDue,
                            TotalAmount = breakdown.totalAmount
                        };

                        string jsonPayload = JsonConvert.SerializeObject(cancelRequest);
                        var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                        string url = $"{AppUrlConstant.CancleStatus}";
                        HttpResponseMessage responsecancel = await client.PostAsync(url, content);

                        if (!responsecancel.IsSuccessStatusCode)
                        {
                            string error = await responsecancel.Content.ReadAsStringAsync();
                            ModelState.AddModelError("", $"Cancellation status update failed: {error}");
                            return View("Error");
                        }

                        TempData["Success"] = "Booking cancellation session flow completed successfully.";
                        TempData["FinalStatus"] = _finalStatus;
                        return RedirectToAction("MyBooking", "Booking");
                    }

                    else
                    {
                        string error = await finalGet.Content.ReadAsStringAsync();
                        ModelState.AddModelError("", $"Final booking status check failed. {error}");
                        return View("Error");
                    }
                }
                else
                {
                    ModelState.AddModelError("", $"DELETE failed: {deleteResult.Error}");
                }

                return View("Error");
            }
        }

        [HttpGet]
        public IActionResult CancelRefund(string fid, string p)
        {
            // Example response (replace with real refund logic)
            var refundDetails = new
            {
                RefundId = fid,
                PNR = p,
                Amount = 4500.00,
                Status = "Processed"
            };

            // Return as JSON
            return Json(refundDetails);
        }

    }

}
