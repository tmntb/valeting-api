using Newtonsoft.Json;

using DJValeting.Models;

namespace DJValeting.Data
{
    public class DJValetingContext
    {
        private readonly HttpClient _httpClient = new();
        private IConfiguration _configuration;
        public bool Refresh;
        public static bool Login;
        public static User User;

        public DJValetingContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private List<Booking> _bookings;
        public List<Booking> Bookings 
        {
            get
            {
                if(_bookings == null || Refresh)
                {
                    _httpClient.DefaultRequestHeaders.Accept.Clear();

                    string endpointListBookings = string.Format("{0}{1}", _configuration.GetSection("EndpointDJValeting").Value, "bookings");
                    var responseBookings = _httpClient.GetStringAsync(endpointListBookings).ConfigureAwait(false).GetAwaiter().GetResult();
                    _bookings = JsonConvert.DeserializeObject<List<Booking>>(responseBookings);

                    Refresh = false;
                }
                return _bookings;
            }
            set
            {
                _bookings = value;
            }
        }

        private List<Flexibility> _flexibilities;
        public List<Flexibility> Flexibilities
        {
            get
            {
                if(_flexibilities == null)
                {
                    string endpointFlexibility = string.Format("{0}{1}", _configuration.GetSection("EndpointDJValeting").Value, "flexibilities");
                    var responseFlexibility = _httpClient.GetStringAsync(endpointFlexibility).ConfigureAwait(false).GetAwaiter().GetResult();
                    _flexibilities = JsonConvert.DeserializeObject<List<Flexibility>>(responseFlexibility);
                }
                return _flexibilities;
            }
            set 
            {
                _flexibilities = value; 
            }
        }

        private List<VehicleSize> _vehicleSizes;
        public List<VehicleSize> VehicleSizes
        {
            get
            {
                if(_vehicleSizes == null)
                {
                    string endpointVehicleSize = string.Format("{0}{1}", _configuration.GetSection("EndpointDJValeting").Value, "vehicleSizes");
                    var responseVehicleSize = _httpClient.GetStringAsync(endpointVehicleSize).ConfigureAwait(false).GetAwaiter().GetResult();
                    _vehicleSizes = JsonConvert.DeserializeObject<List<VehicleSize>>(responseVehicleSize);

                }
                return _vehicleSizes;
            }
            set
            {
                _vehicleSizes = value;
            }
        }
    }
}
