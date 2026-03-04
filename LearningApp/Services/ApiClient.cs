using LearningApp.Constants;

namespace LearningApp.Services
{
    public static class ApiClient
    {
        private static HttpClient _client;

        public static HttpClient Instance
        {
            get
            {
                if (_client == null)
                {
                    var handler = new HttpClientHandler
                    {
                        ServerCertificateCustomValidationCallback = (m, c, ch, e) => true
                    };
                    _client = new HttpClient(handler)
                    {
                        BaseAddress = new Uri(AppConfig.BaseUrl),
                        Timeout = TimeSpan.FromSeconds(30)
                    };
                    _client.DefaultRequestHeaders.Add("ngrok-skip-browser-warning", "true");
                }
                return _client;
            }
        }
    }
}