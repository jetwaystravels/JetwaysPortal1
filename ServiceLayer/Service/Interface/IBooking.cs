using DomainLayer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Service.Interface
{
    public interface IBooking<T> where T : class
    {
         Task<IEnumerable<Booking>> GetAllAsync(string? flightId = null, string? recordLocator = null);
         Task<bool> UpdateCancelStatusAsync(string recordLocator, int status);

    }
}
