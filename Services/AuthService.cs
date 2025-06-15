using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Blazored.LocalStorage;
using Frontend.Models;
using Microsoft.AspNetCore.Components.Authorization;
using System.Linq;

namespace Frontend.Services
{
    public interface IAuthService
    {
        Task<AuthResponse> Login(LoginRequest loginModel);
        Task<AuthResponse> Register(RegisterRequest registerModel);
        Task<AuthResponse> ForgotPassword(ForgotPasswordRequest forgotPasswordModel);
        Task<AuthResponse> ResetPassword(ResetPasswordRequest resetPasswordModel);
        Task<AuthResponse> GoogleLogin(string idToken);
        Task Logout();
        Task<string> GetToken();
        Task InitializeAuthState();
    }

    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly AuthenticationStateProvider _authStateProvider;
        private readonly ILocalStorageService _localStorage;

        public AuthService(HttpClient httpClient,
                         AuthenticationStateProvider authStateProvider,
                         ILocalStorageService localStorage)
        {
            _httpClient = httpClient;
            _authStateProvider = authStateProvider;
            _localStorage = localStorage;
        }

        public async Task InitializeAuthState()
        {
            var token = await GetToken();
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        public async Task<AuthResponse> Login(LoginRequest loginModel)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/account/login", loginModel);
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Login response content: {content}"); // Debug log

                if (!response.IsSuccessStatusCode)
                {
                    return new AuthResponse 
                    { 
                        Success = false, 
                        Message = content, 
                        Token = string.Empty 
                    };
                }

                var result = await response.Content.ReadFromJsonAsync<AuthResponse>(new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }) ?? new AuthResponse { Success = false, Message = "Failed to deserialize response", Token = string.Empty };

                if (!string.IsNullOrEmpty(result.Token))
                {
                    await _localStorage.SetItemAsync("authToken", result.Token);
                    await ((CustomAuthStateProvider)_authStateProvider).MarkUserAsAuthenticated(result.Token);
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", result.Token);
                    result.Success = true;
                }

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Login error: {ex}"); // Debug log
                return new AuthResponse { Success = false, Message = ex.Message, Token = string.Empty };
            }
        }

        public async Task<AuthResponse> Register(RegisterRequest registerModel)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/account/register", registerModel);
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Register response content: {content}"); // Debug log

                if (!response.IsSuccessStatusCode)
                {
                    // Try to parse the error response
                    try
                    {
                        var errorResponse = JsonSerializer.Deserialize<List<IdentityError>>(content, new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });

                        if (errorResponse != null && errorResponse.Any())
                        {
                            return new AuthResponse
                            {
                                Success = false,
                                Message = string.Join(", ", errorResponse.Select(e => e.Description)),
                                Token = string.Empty
                            };
                        }
                    }
                    catch
                    {
                        // If we can't parse the error response, return the raw content
                        return new AuthResponse
                        {
                            Success = false,
                            Message = content,
                            Token = string.Empty
                        };
                    }
                }

                var result = await response.Content.ReadFromJsonAsync<AuthResponse>(new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }) ?? new AuthResponse { Success = false, Message = "Failed to deserialize response", Token = string.Empty };

                result.Success = response.IsSuccessStatusCode;
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Register error: {ex}"); // Debug log
                return new AuthResponse { Success = false, Message = ex.Message, Token = string.Empty };
            }
        }

        public async Task<AuthResponse> ForgotPassword(ForgotPasswordRequest forgotPasswordModel)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/account/forgotpassword", forgotPasswordModel);
                var result = await response.Content.ReadFromJsonAsync<AuthResponse>() ?? 
                    new AuthResponse { Success = false, Message = "Failed to deserialize response", Token = string.Empty };
                result.Success = response.IsSuccessStatusCode;
                return result;
            }
            catch (Exception ex)
            {
                return new AuthResponse { Success = false, Message = ex.Message, Token = string.Empty };
            }
        }

        public async Task<AuthResponse> ResetPassword(ResetPasswordRequest resetPasswordModel)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/account/resetpassword", resetPasswordModel);
                var result = await response.Content.ReadFromJsonAsync<AuthResponse>() ?? 
                    new AuthResponse { Success = false, Message = "Failed to deserialize response", Token = string.Empty };
                result.Success = response.IsSuccessStatusCode;
                return result;
            }
            catch (Exception ex)
            {
                return new AuthResponse { Success = false, Message = ex.Message, Token = string.Empty };
            }
        }

        public async Task<AuthResponse> GoogleLogin(string idToken)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/account/googlelogin", new { IdToken = idToken });
                var result = await response.Content.ReadFromJsonAsync<AuthResponse>() ?? 
                    new AuthResponse { Success = false, Message = "Failed to deserialize response", Token = string.Empty };

                if (response.IsSuccessStatusCode && !string.IsNullOrEmpty(result.Token))
                {
                    await _localStorage.SetItemAsync("authToken", result.Token);
                    await ((CustomAuthStateProvider)_authStateProvider).MarkUserAsAuthenticated(result.Token);
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", result.Token);
                    result.Success = true;
                }
                else
                {
                    result.Success = false;
                    result.Message = result.Message ?? "Google login failed";
                }

                return result;
            }
            catch (Exception ex)
            {
                return new AuthResponse { Success = false, Message = ex.Message, Token = string.Empty };
            }
        }

        public async Task Logout()
        {
            await _localStorage.RemoveItemAsync("authToken");
            await ((CustomAuthStateProvider)_authStateProvider).MarkUserAsLoggedOut();
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }

        public async Task<string> GetToken()
        {
            return await _localStorage.GetItemAsync<string>("authToken") ?? string.Empty;
        }
    }
} 