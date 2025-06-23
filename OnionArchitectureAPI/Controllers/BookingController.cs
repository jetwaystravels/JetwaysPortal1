using DomainLayer.Model;
using Microsoft.AspNetCore.Mvc;
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
        public async Task<IActionResult> UpdateCancelStatus([FromQuery] string recordLocator, [FromQuery] int status)
        {
            var result = await _bookingDetail.UpdateCancelStatusAsync(recordLocator, status);
            if (result)
            {
                return Ok(new { message = "Booking status updated successfully." });
            }
            return BadRequest(new { message = "Failed to update booking status." });
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
    }
}
