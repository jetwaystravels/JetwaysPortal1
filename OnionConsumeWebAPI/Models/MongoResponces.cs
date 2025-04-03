namespace OnionConsumeWebAPI.Models
{
    public class MongoResponces
    {
        public MongoDB.Bson.ObjectId _id;
        public string Guid;
        public DateTime CreatedDate;
        public string KeyRef;
        // public List<ResultList> ResList;
        public string Response;
        public string RightResponse;
        //public string SupplierCode;
    }

    public class MongoRequest
    {
        public MongoDB.Bson.ObjectId _id;
        public string Guid;
        public string Request;
        public DateTime CreatedDate;

    }

    public class MongoSuppFlightToken
    {
        public MongoDB.Bson.ObjectId _id;
        public string Guid;
        public string PassRequest;
		public string ContactRequest;
        public string Request;
        public string Token;
        public string RToken;
        public string JourneyKey;
        public string Supp;
		public string PassengerRequest;
		public DateTime CreatedDate;

    }

	public class MongoSeatMealdetail
	{
		public MongoDB.Bson.ObjectId _id;
		public string Guid;
		public string ResultRequest;
		public string SeatMap;
		public string Meals;
		public string Baggage;
		public string Infant;
		public string Supp;
    	public DateTime CreatedDate;

	}

}
