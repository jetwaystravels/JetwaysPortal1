using DomainLayer.Model;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using RepositoryLayer.DbContextLayer;
using ServiceLayer.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Service.Implementation
{
    public class BookingService : IBooking<Booking>
    {
        private readonly AppDbContext _dbContext;
        public BookingService(AppDbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        //public async Task<IEnumerable<Booking>> GetAllAsync()
        //{
        //    return await _dbContext.GetBookingDetails.FromSqlRaw("EXEC sp_GetFlightBookingDetails")
        //.ToListAsync();
        //}

        public async Task<IEnumerable<Booking>> GetAllAsync(string? flightId = null, string? recordLocator = null)
        {
            var flightIdParam = new SqlParameter("@FlightID", (object?)flightId ?? DBNull.Value);
            var recordLocatorParam = new SqlParameter("@RecordLocator", (object?)recordLocator ?? DBNull.Value);

            return await _dbContext.GetBookingDetails
                .FromSqlRaw("EXEC sp_GetFlightBookingDetails @FlightID, @RecordLocator", flightIdParam, recordLocatorParam)
                .ToListAsync();
        }
        public async Task<bool> UpdateCancelStatusAsync(string recordLocator, int status)
        {
            var recordLocatorParam = new SqlParameter("@RecordLocator", recordLocator);
            var statusParam = new SqlParameter("@CancelStatus", status);

            int rowsAffected = await _dbContext.Database.ExecuteSqlRawAsync(
                "EXEC sp_UpdateBookingCancelStatus @RecordLocator, @CancelStatus",
                recordLocatorParam, statusParam);

            return rowsAffected != 0;
        }

    }
}
