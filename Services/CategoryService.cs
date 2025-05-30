using System.Net.Http.Json;
using Frontend.Models;
using System.Text.Json;

namespace Frontend.Services
{
    public interface ICategoryService
    {
        Task<List<Category>> GetAllAsync();
        Task<Category> GetByIdAsync(int id);
        Task<Category> CreateAsync(Category category);
        Task UpdateAsync(Category category);
        Task DeleteAsync(int id);
    }

    public class CategoryService : ICategoryService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<CategoryService> _logger;

        public CategoryService(HttpClient httpClient, ILogger<CategoryService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<List<Category>> GetAllAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<Category>>("api/Categories");
        }

        public async Task<Category> GetByIdAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<Category>($"api/Categories/{id}");
        }

        public async Task<Category> CreateAsync(Category category)
        {
            _logger.LogInformation("Creating category: {Category}", JsonSerializer.Serialize(category));
            var response = await _httpClient.PostAsJsonAsync("api/Categories", category);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                _logger.LogError("Failed to create category. Status: {Status}, Error: {Error}", 
                    response.StatusCode, error);
                throw new HttpRequestException($"Failed to create category: {error}");
            }
            return await response.Content.ReadFromJsonAsync<Category>();
        }

        public async Task UpdateAsync(Category category)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/Categories/{category.Id}", category);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/Categories/{id}");
            response.EnsureSuccessStatusCode();
        }
    }
} 