namespace CoporateBooking.Models
{
    public class CancelBookingResponse
    {
        public BookingData data { get; set; }

        public class BookingData
        {
            public string recordLocator { get; set; }
            public Breakdown breakdown { get; set; }
            public Dictionary<string, Passenger> passengers { get; set; }
            public List<Journey> journeys { get; set; }
        }

        public class Breakdown
        {
            public decimal totalAmount { get; set; }
            public decimal totalToCollect { get; set; }
        }

        public class Passenger
        {
            public string passengerKey { get; set; }
            public PassengerName name { get; set; }
        }

        public class PassengerName
        {
            public string first { get; set; }
            public string last { get; set; }
        }

        public class Journey
        {
            public JourneyDesignator designator { get; set; }
        }

        public class JourneyDesignator
        {
            public string origin { get; set; }
            public string destination { get; set; }
            public DateTime departure { get; set; }
            public DateTime arrival { get; set; }
        }

    }
}
