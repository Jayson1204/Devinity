using LearningApp.Api.Models;
using System.Net.Http.Json;

namespace LearningApp.Services
{
    public class AssessmentService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "NGROK_URL";

        public AssessmentService()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(BaseUrl),
                Timeout = TimeSpan.FromSeconds(30)
            };
        }

        public async Task<List<Assessment>> GetAssessmentsByCategoryAsync(string category)
        {
            try
            {
                var assessments = await _httpClient.GetFromJsonAsync<List<Assessment>>($"/api/assessments/category/{category}");
                return assessments ?? new List<Assessment>();
            }
            catch (Exception)
            {
                System.Diagnostics.Debug.WriteLine("Error fetching assessments:");
                return new List<Assessment>();
            }
        }

        public async Task<Assessment> GetAssessmentByIdAsync(int id)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<Assessment>($"/api/assessments/{id}");
            }
            catch (Exception)
            {
                System.Diagnostics.Debug.WriteLine("Error fetching assessment");
                return null;
            }
        }
    }
}