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

        public async Task<IActionResult> CancelActionAsync(int airline, string pnr, List<string> passengerKeys, string cancellationType)
        {

            if (airline == 1) // AirAsia
            {
                if (cancellationType == "complete")
                {
                    return await CancelCompleteAirAsiaBooking(pnr);
                }
                else if (cancellationType == "partial")
                {
                    return await CancelPartialAirAsiaBooking(pnr, passengerKeys);
                }
            }
            else if (airline == 2) // Air India Express or other
            {
                if (cancellationType == "complete")
                {
                    return await CancelCompleteOtherAirline(pnr);
                }
                else if (cancellationType == "partial")
                {
                    return await CancelPartialOtherAirline(pnr, passengerKeys);
                }
            }

            ModelState.AddModelError("", "Unsupported airline or invalid cancellation type.");
            return View("Error");

        }

        private async Task<IActionResult> CancelCompleteAirAsiaBooking(string pnr)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(AppUrlConstant.BaseURL);

                // 1. Get AirAsia credentials
                HttpResponseMessage response = await client.GetAsync(AppUrlConstant.AirlineLogin);
                if (!response.IsSuccessStatusCode)
                {
                    ModelState.AddModelError(string.Empty, "Failed to retrieve airline credentials.");
                    return View("Error");
                }

                var results = await response.Content.ReadAsStringAsync();
                var jsonObject = JsonConvert.DeserializeObject<List<_credentials>>(results);
                var _credentialsAirasia = jsonObject.FirstOrDefault(cred => cred?.FlightCode == 1); // AirAsia

                // 2. Login and get token
                var login = new airlineLogin { credentials = _credentialsAirasia };
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
                string token = tokenJson.data.token;
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                // 3. Retrieve booking
                var retrieveUrl = $"{AppUrlConstant.AirasiaPNRBooking}/{pnr}";
                var retrieveResponse = await client.GetAsync(retrieveUrl);
                if (!retrieveResponse.IsSuccessStatusCode)
                {
                    var err = await retrieveResponse.Content.ReadAsStringAsync();
                    ModelState.AddModelError("", $"Booking retrieval failed: {err}");
                    return View("Error");
                }

                // 4. Delete journey
                var deleteResult = await HttpHelperService.SendDeleteAsync(client, $"{AppUrlConstant.DeleteBooking}");
                if (!deleteResult.Success)
                {
                    ModelState.AddModelError("", $"DELETE failed: {deleteResult.Error}");
                    return View("Error");
                }

                // 5. PUT commit
                var putResult = await HttpHelperService.SendPutAsync(client, $"{AppUrlConstant.AirasiaCommitBooking}");
                if (!putResult.Success)
                {
                    ModelState.AddModelError("", $"PUT failed: {putResult.Error}");
                    return View("Error");
                }

                // 6. Final GET booking
                var finalGet = await client.GetAsync($"{AppUrlConstant.AirasiaGetBoking}");
                if (!finalGet.IsSuccessStatusCode)
                {
                    var error = await finalGet.Content.ReadAsStringAsync();
                    ModelState.AddModelError("", $"Final booking status check failed. {error}");
                    return View("Error");
                }

                var _finalStatus = await finalGet.Content.ReadAsStringAsync();
                dynamic objcancelBooking = JsonConvert.DeserializeObject<dynamic>(_finalStatus);
                decimal balanceDue = objcancelBooking.data.breakdown.balanceDue;
                decimal totalAmount = objcancelBooking.data.breakdown.totalAmount;

                var email = User?.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

                Logs logs = new Logs();
                logs.WriteLogs(_finalStatus, "Cancel", "AirAsiaOneWay", "oneway");

                var cancelRequest = new
                {
                    RecordLocator = pnr,
                    Status = 3,
                    UserEmail = email,
                    BalanceDue = balanceDue,
                    TotalAmount = totalAmount
                };

                string jsonPayload = JsonConvert.SerializeObject(cancelRequest);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
                var responsecancel = await client.PostAsync(AppUrlConstant.CancleStatus, content);

                if (!responsecancel.IsSuccessStatusCode)
                {
                    string err = await responsecancel.Content.ReadAsStringAsync();
                    ModelState.AddModelError("", $"Cancellation status update failed: {err}");
                    return View("Error");
                }

                TempData["Success"] = "Booking cancellation session flow completed successfully.";
                TempData["FinalStatus"] = _finalStatus;
                return RedirectToAction("MyBooking", "Booking");
            }
        }

        private async Task<IActionResult> CancelPartialAirAsiaBooking(string pnr, List<string> passengerKeys)
        {
            // TODO: Call AirAsia SSR delete or passenger-specific cancel API if available
            ModelState.AddModelError("", "Partial cancellation for AirAsia is not yet implemented.");
            return View("Error");
        }

        private async Task<IActionResult> CancelCompleteOtherAirline(string pnr)
        {
            // TODO: Implement logic for airline 2 full cancellation
            ModelState.AddModelError("", "Complete cancellation for this airline is not yet implemented.");
            return View("Error");
        }

        private async Task<IActionResult> CancelPartialOtherAirline(string pnr, List<string> passengerKeys)
        {
            // TODO: Implement logic for airline 2 partial cancellation
            ModelState.AddModelError("", "Partial cancellation for this airline is not yet implemented.");
            return View("Error");
        }



        [HttpGet]
        public async Task<IActionResult> CancelRefundAsync(string flightid, string pnr)
        {
            RefundRequest RefundRequestList = null;
          
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    // Build the query string if needed
                    var apiUrl = $"{AppUrlConstant.GetRefund}?bookingId={flightid}&recordLocator={pnr}";

                    HttpResponseMessage response = await client.GetAsync(apiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        var result = await response.Content.ReadAsStringAsync();
                        RefundRequestList = JsonConvert.DeserializeObject<RefundRequest>(result);

                        return View(RefundRequestList); // Pass to Razor View
                    }
                    else
                    {
                        ViewBag.ErrorMessage = "Failed to load refund data.";
                        return View(RefundRequestList);
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An error occurred: " + ex.Message;
                return View(RefundRequestList);
            }



            //// Example response (replace with real refund logic)
            //var refundDetails = new
            //{
            //    RefundId = fid,
            //    PNR = p,
            //    Amount = 4500.00,
            //    Status = "Processed"
            //};

            //// Return as JSON
            //return Json(refundDetails);
        }

    }

}
