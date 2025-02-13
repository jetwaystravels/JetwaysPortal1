using DomainLayer.Model;
using MongoDB.Driver;
using Nancy;
using OnionConsumeWebAPI.ApiService;
using System.Security.Cryptography;
using System.Xml.Serialization;
using OnionConsumeWebAPI.ErrorHandling;
using Microsoft.EntityFrameworkCore.Query;
using Newtonsoft.Json;

namespace OnionConsumeWebAPI.Models
{
    public class MongoDBHelper
    {
        private static IMongoClient mongoClient;
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        IMongoDatabase mDB;

        //  private readonly IOptionsSnapshot<AppSettings> _balSettings;
        //IOptionsSnapshot<AppSettings> serviceSettings

        //Logger logger = new Logger();

        public MongoDBHelper(IConfiguration configuration)
        {
            // mongoClient = new MongoClient(ConfigurationManager.AppSettings["MongoDBConn"].ToString());
            _configuration = configuration;

            // pick from iconfig
            //_connectionString = _configuration.GetSection("ConnectionStrings").GetValue<string>("DefaultConnection");
            //mongoClient = new MongoClient(_configuration.GetSection("MongoDbSettings").GetValue<string>("ConnectionString"));
            //mDB = mongoClient.GetDatabase(_configuration.GetSection("MongoDbSettings").GetValue<string>("DatabaseName"));

            _connectionString = _configuration["ConnectionStrings:DefaultConnection"];
            mongoClient = new MongoClient(_configuration["MongoDbSettings:ConnectionString"]);
            mDB = mongoClient.GetDatabase(_configuration["MongoDbSettings:DatabaseName"]);

        }

        //public MongoDBHelper(MongoDbService mongoDbService, IConfiguration configuration)
        //{
        //    // mongoClient = new MongoClient(ConfigurationManager.AppSettings["MongoDBConn"].ToString());
        //    _mongoDbService = mongoDbService;
        //    this.configuration = configuration;
        //    // mongoClient = new MongoClient(this.configuration.GetSection("MongoDbSettings").ToString());
        //    mongoClient = new MongoClient(this.configuration.GetValue<string>("MongoDbSettings"));
        //    mDB = mongoClient.GetDatabase(this.configuration.GetValue<string>("DatabaseName"));
        //}

        public async Task<string> GetFlightSearchByKeyRef(string keyref)
        {
            string guid = "";
            try
            {
                MongoResponces srchData = new MongoResponces();

                //  _mongoDbService = new MongoDbService();

                srchData = await mDB.GetCollection<MongoResponces>("KeyLog").Find(Builders<MongoResponces>.Filter.Eq("KeyRef", keyref)).Sort(Builders<MongoResponces>.Sort.Descending("CreatedDate")).FirstOrDefaultAsync().ConfigureAwait(false);

                if (srchData != null)
                {
                    if (srchData.CreatedDate < DateTime.UtcNow)
                    {
                        srchData.Guid = null;
                    }
                    else
                    {
                        guid = srchData.Guid;
                    }
                }
            }
            catch (Exception ex)
            {
                //logger.WriteLog(ex, "GetFlightSearchByKeyRef methhod", _connectionString);

            }
            return guid;
        }

        public async Task<MongoResponces> GetALLFlightResulByGUID(string guid)
        {
            MongoResponces srchDataALL = new MongoResponces();
            try
            {
                srchDataALL = await mDB.GetCollection<MongoResponces>("Result").Find(Builders<MongoResponces>.Filter.Eq("Guid", guid)).Sort(Builders<MongoResponces>.Sort.Descending("CreatedDate")).FirstOrDefaultAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                //logger.WriteLog(ex, "GetALLFlightResulByGUID methhod", _connectionString);
            }
            return srchDataALL;
        }

        public void SaveKeyRequest(string guid, string keyref)
        {
            try
            {
                MongoResponces srchData = new MongoResponces();
                srchData.CreatedDate = DateTime.UtcNow.AddMinutes(Convert.ToInt16(20));
                srchData.KeyRef = keyref;
                srchData.Guid = guid;

                mDB.GetCollection<MongoResponces>("KeyLog").InsertOneAsync(srchData);
            }
            catch (Exception ex)
            {
                //logger.WriteLog(ex, "SaveKeyRequest methhod", _connectionString);
            }
        }

        public void SaveFlightSearch(MongoResponces srchData, List<SimpleAvailibilityaAddResponce> resp)
        {
            try
            {
                MongoHelper mongoHelper = new MongoHelper();
                srchData.CreatedDate = DateTime.UtcNow.AddMinutes(Convert.ToInt16(20));
                using (StringWriter stringWriter = new StringWriter())
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(List<SimpleAvailibilityaAddResponce>));
                    serializer.Serialize(stringWriter, resp);
                    //  srchData.Response = mongoHelper.Zip(stringWriter.ToString());
                    srchData.Response = mongoHelper.Zip(JsonConvert.SerializeObject(resp));


                }
                mDB.GetCollection<MongoResponces>("Result").InsertOneAsync(srchData);
            }
            catch (Exception ex)
            {
                //logger.WriteLog(ex, "SaveFlightSearch methhod", _connectionString);
            }
        }

        public void SaveRequest(SimpleAvailabilityRequestModel sCriteria, string Guid)
        {
            MongoRequest srchData = new MongoRequest();
            MongoHelper mongoHelper = new MongoHelper();
            try
            {
                srchData.Guid = Guid;
                srchData.CreatedDate = DateTime.UtcNow.AddMinutes(Convert.ToInt16(20));

                using (StringWriter stringWriter = new StringWriter())
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(SimpleAvailabilityRequestModel));
                    serializer.Serialize(stringWriter, sCriteria);
                    srchData.Request = mongoHelper.Zip(stringWriter.ToString());
                }

                mDB.GetCollection<MongoRequest>("Requests").InsertOneAsync(srchData);

            }
            catch (Exception ex)
            {
                //logger.WriteLog(ex, "SaveRequest methhod", _connectionString);
            }
        }

        public async Task<SimpleAvailabilityRequestModel> GetRequests(string guid)
        {
            SimpleAvailabilityRequestModel sCriteria = new SimpleAvailabilityRequestModel();
            MongoRequest srchData = new MongoRequest();
            MongoHelper mongoHelper = new MongoHelper();
            //service1 src = new service1();
            try
            {
                await mDB.GetCollection<MongoRequest>("Requests").Find(Builders<MongoRequest>.Filter.Eq("Guid", guid)).FirstOrDefaultAsync().ConfigureAwait(false);

                if (srchData != null && srchData.Request != null)
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(SimpleAvailabilityRequestModel));

                    StringReader textReader = new StringReader(mongoHelper.UnZip(srchData.Request));

                    sCriteria = (SimpleAvailabilityRequestModel)serializer.Deserialize(textReader);

                    // sCriteria.LogDateTime = srchData.CreatedDate;
                }
            }
            catch (Exception ex)
            {
                //logger.WriteLog(ex, "GetRequests methhod", _connectionString);
            }

            return sCriteria;
        }


        public void SaveSearchLog(SimpleAvailabilityRequestModel requestModel, string Guid)
        {
            MongoHelper mongoHelper = new MongoHelper();

            SearchLog searchLog = new SearchLog();

            try
            {
                searchLog.TripType = requestModel.trip;
                searchLog.Log_WSGUID = Guid;
                searchLog.Log_SearchTypeID = 1;
                searchLog.Origin = requestModel.origin.Split("-")[1];
                searchLog.Destination = requestModel.destination.Split("-")[1];
                searchLog.Log_RefNumber = mongoHelper.Get8Digits();
                searchLog.DepartDateTime = requestModel.beginDate;
                searchLog.ArrivalDateTime = requestModel.endDate;

                if (requestModel.passengercount != null)
                {
                    searchLog.Adults = requestModel.passengercount.adultcount;
                    searchLog.Children = requestModel.passengercount.childcount;
                    searchLog.Infants = requestModel.passengercount.infantcount;
                }
                else
                {
                    searchLog.Adults = requestModel.adultcount;
                    searchLog.Children = requestModel.childcount;
                    searchLog.Infants = requestModel.infantcount;
                }
                searchLog.Log_DateTime = DateTime.Now;
                searchLog.IP = mongoHelper.GetIp();

                // _mongoDbService.GetCollection<SearchLog>("LogSearchData").InsertOneAsync(searchLog);
                mDB.GetCollection<SearchLog>("LogSearchData").InsertOneAsync(searchLog);

            }
            catch (Exception ex)
            {
                //logger.WriteLog(ex, "SaveSearchLog methhod", _connectionString);
            }
        }

        public async Task<SearchLog> GetFlightSearchLog(string Guid)
        {
            SearchLog srchData = null;
            try
            {
                // srchData = new SearchLog();

                //  _mongoDbService = new MongoDbService();l

                //  srchData = await _mongoDbService.GetCollection<SearchLog>("LogSearchData").Find(Builders<SearchLog>.Filter.Eq("Log_WSGUID", Guid)).Sort(Builders<SearchLog>.Sort.Descending("Log_DateTime")).FirstOrDefaultAsync().ConfigureAwait(false);
                srchData = await mDB.GetCollection<SearchLog>("LogSearchData").Find(Builders<SearchLog>.Filter.Eq("Log_WSGUID", Guid)).Sort(Builders<SearchLog>.Sort.Descending("Log_DateTime")).FirstOrDefaultAsync().ConfigureAwait(false);


            }
            catch (Exception ex)
            {
                //logger.WriteLog(ex, "GetFlightSearchLog methhod", _connectionString);

            }

            return srchData;

        }
    }
}
