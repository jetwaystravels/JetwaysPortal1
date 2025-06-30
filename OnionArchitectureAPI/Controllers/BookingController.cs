using DomainLayer.Model;
using Microsoft.AspNetCore.Mvc;
using ServiceLayer.Service.Implementation;
using ServiceLayer.Service.Interface;

namespace OnionArchitectureAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : Controller
    {
        private readonly IBooking<Booking> _bookingDetail;

        public BookingController(IBooking<Booking> bookingDetail)
        {
            this._bookingDetail = bookingDetail;
        }
        //[HttpGet("GetbookingDetail")]
        //public async Task<IActionResult> GetbookingDetail()
        //{
        //    var data = await _bookingDetail.GetAllAsync();
        //    return Ok(data);

        //    //return View();

        //}

        [HttpGet("GetbookingDetail")]
        public async Task<IActionResult> GetbookingDetail([FromQuery] string? flightId = null, [FromQuery] string? recordLocator = null)
        {
            var data = await _bookingDetail.GetAllAsync(flightId, recordLocator);
            return Ok(data);
        }


        [HttpPost("UpdateCancelStatus")]
        //public async Task<IActionResult> UpdateCancelStatus([FromQuery] string recordLocator, [FromQuery] int status)
        //{
        //    var result = await _bookingDetail.UpdateCancelStatusAsync(recordLocator, status);
        //    if (result)
        //    {
        //        return Ok(new { message = "Booking status updated successfully." });
        //    }
        //    return BadRequest(new { message = "Failed to update booking status." });
        //}
        public async Task<IActionResult> UpdateCancelStatus([FromBody] CancelStatusRequest req)
        {
            bool ok = await _bookingDetail.UpdateCancelStatusAsync(
                          req.RecordLocator,
                          req.Status,
                          req.UserEmail,
                          req.BalanceDue,
                          req.TotalAmount);

            return ok ? Ok() : StatusCode(500, "DB update failed");
        }

        [HttpGet("GetbookingPNR")]
        public async Task<IActionResult> GetDetails(string recordLocator)
        {
            var result = await _bookingDetail.GetBookingDetailsFromSPAsync(recordLocator);
                      if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
        public sealed record CancelStatusRequest(
          string RecordLocator,
          int Status,
          string UserEmail,
          decimal BalanceDue,
         decimal TotalAmount);
    }
}
