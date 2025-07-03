using Microsoft.AspNetCore.Mvc;
using DomainLayer.Model;
using OnionConsumeWebAPI.Extensions;
using CoporateBooking.Comman;
using static DomainLayer.Model.ReturnTicketBooking;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using Utility;
using Newtonsoft.Json;
using CoporateBooking.Models;
namespace CoporateBooking.Controllers.common
{
    public class BookingController : Controller
    {
        private _credentials _credentialsAirasia = null;
        public async Task<IActionResult> MyBookingAsync()
        {
            List<Booking> bookingList = null;
            var identity = (ClaimsIdentity)User.Identity;
            IEnumerable<Claim> claims = identity.Claims;
            var userEmail = claims.Where(c => c.Type == ClaimTypes.Email).ToList()[0].Value;

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    // Make GET call
                    //HttpResponseMessage response = await client.GetAsync(AppUrlConstant.Getflightbooking);
                    var requestUrl = $"{AppUrlConstant.Getflightbooking}?userEmail={Uri.EscapeDataString(userEmail)}";
                    HttpResponseMessage response = await client.GetAsync(requestUrl);
                    if (response.IsSuccessStatusCode)
                    {
                        var result = await response.Content.ReadAsStringAsync();
                        bookingList = JsonConvert.DeserializeObject<List<Booking>>(result);

                        return View(bookingList); // Pass to Razor View
                    }
                    else
                    {
                        ViewBag.ErrorMessage = "Failed to load booking data.";
                        return View(bookingList);
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An error occurred: " + ex.Message;
                return View(bookingList);
            }
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> CancelbookingAsync(int airline, string pnr)
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

                switch (airline)
                {
                    case 1: // AirAsia
                        var _credentialsAirasia = jsonObject.FirstOrDefault(cred => cred?.FlightCode == 1);
                        if (_credentialsAirasia == null)
                        {
                            ModelState.AddModelError("", "AirAsia credentials not found.");
                            return View("Error");
                        }

                        var login = new airlineLogin { credentials = _credentialsAirasia };
                        var airasiaToken = new AirasiaTokan();

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
                        airasiaToken.token = tokenJson.data.token;

                        // Set Auth Header
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", airasiaToken.token);

                        // Step 3: GET /booking/retrieve/byRecordLocator/{pnr}
                        string retrieveUrl = $"{AppUrlConstant.AirasiaPNRBooking}/{pnr}";
                        HttpResponseMessage retrieveResponse = await client.GetAsync(retrieveUrl);
                        if (!retrieveResponse.IsSuccessStatusCode)
                        {
                            string err = await retrieveResponse.Content.ReadAsStringAsync();
                            ModelState.AddModelError("", $"Booking retrieval failed: {err}");
                            return View("Error");
                        }

                        string bookingResult = await retrieveResponse.Content.ReadAsStringAsync();
                        var booking = JsonConvert.DeserializeObject<CancelBookingResponse>(bookingResult);

                        ViewBag.AirlineId = airline;
                        return View(booking.data);

                    // TODO: Add other airlines here
                    case 2: // Example: Akasa
                        ModelState.AddModelError("", "IndiGo cancellation is not yet implemented.");
                        return View("Error");

                    case 3: // Example: SpiceJet
                        ModelState.AddModelError("", "SpiceJet cancellation is not yet implemented.");
                        return View("Error");
                    case 4: // Example: IndiGo
                        ModelState.AddModelError("", "SpiceJet cancellation is not yet implemented.");
                        return View("Error");
                    case 5: // Example: GDS
                        ModelState.AddModelError("", "SpiceJet cancellation is not yet implemented.");
                        return View("Error");

                    default:
                        ModelState.AddModelError("", "Unsupported airline.");
                        return View("Error");
                }
            }
        }

    }
}
