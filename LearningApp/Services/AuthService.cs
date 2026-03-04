using LearningApp.Models;
using System.Net.Http.Json;
//using static KotlinX.Serialization.Descriptors.PolymorphicKind;

namespace LearningApp.Services
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;

        public AuthService()
        {
            _httpClient = ApiClient.Instance;

        }

        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("/api/auth/login", request);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<LoginResponse>();
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                return new LoginResponse
                {
                    Success = false,
                    Message = $"Login failed Invalid Username OR Password"
                };
            }
            catch (Exception)
            {
                return new LoginResponse
                {
                    Success = false,
                    Message = "Error"
                };
            }
        }

        public async Task<RegisterResponse> RegisterAsync(RegisterRequest request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("/api/auth/register", request);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<RegisterResponse>();
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                return new RegisterResponse
                {
                    Success = false,
                    Message = "Registration failed"
                };
            }
            catch (Exception)
            {
                return new RegisterResponse
                {
                    Success = false,
                    Message = "Error: "
                };
            }
        }
    }
}
