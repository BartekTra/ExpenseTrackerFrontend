using System.Net.Http.Json;
using Frontend.Models;
using Microsoft.AspNetCore.Components;
using System.Text.Json;

namespace Frontend.Services
{
    public interface IExpenseService
    {
        Task<List<Expense>> GetAllAsync();
        Task<Expense> GetByIdAsync(int id);
        Task<Expense> CreateAsync(CreateExpenseDto expense);
        Task UpdateAsync(Expense expense);
        Task DeleteAsync(int id);
        Task<MonthlyExpenseTotalDto> GetMonthlyTotalAsync(int year, int month);
        Task<List<CategoryExpenseSummaryDto>> GetCategorySummariesAsync(int year, int month);
        Task<List<CategoryExpenseSummaryDto>> GetCategoryExpenseSummary(DateTime startDate, DateTime endDate);
        Task<List<MonthlyExpenseTotalDto>> GetMonthlyExpenseTotals(DateTime startDate, DateTime endDate);
    }

    public class ExpenseService : IExpenseService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ExpenseService> _logger;

        public ExpenseService(HttpClient httpClient, ILogger<ExpenseService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<List<Expense>> GetAllAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<Expense>>("api/Expenses");
        }

        public async Task<Expense> GetByIdAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<Expense>($"api/Expenses/{id}");
        }

        public async Task<Expense> CreateAsync(CreateExpenseDto expense)
        {
            _logger.LogInformation("Creating expense: {Expense}", JsonSerializer.Serialize(expense));
            var response = await _httpClient.PostAsJsonAsync("api/Expenses", expense);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                _logger.LogError("Failed to create expense. Status: {Status}, Error: {Error}", 
                    response.StatusCode, error);
                throw new HttpRequestException($"Failed to create expense: {error}");
            }
            return await response.Content.ReadFromJsonAsync<Expense>();
        }

        public async Task UpdateAsync(Expense expense)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/Expenses/{expense.Id}", expense);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                _logger.LogError("Failed to update expense. Status: {Status}, Error: {Error}", 
                    response.StatusCode, error);
                throw new HttpRequestException($"Failed to update expense: {error}");
            }
        }

        public async Task DeleteAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/Expenses/{id}");
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                _logger.LogError("Failed to delete expense. Status: {Status}, Error: {Error}", 
                    response.StatusCode, error);
                throw new HttpRequestException($"Failed to delete expense: {error}");
            }
        }

        public async Task<MonthlyExpenseTotalDto> GetMonthlyTotalAsync(int year, int month)
        {
            return await _httpClient.GetFromJsonAsync<MonthlyExpenseTotalDto>($"api/Expenses/summary/monthly?year={year}&month={month}");
        }

        public async Task<List<CategoryExpenseSummaryDto>> GetCategorySummariesAsync(int year, int month)
        {
            return await _httpClient.GetFromJsonAsync<List<CategoryExpenseSummaryDto>>($"api/Expenses/summary/categories?year={year}&month={month}");
        }

        public async Task<List<CategoryExpenseSummaryDto>> GetCategoryExpenseSummary(DateTime startDate, DateTime endDate)
        {
            return await _httpClient.GetFromJsonAsync<List<CategoryExpenseSummaryDto>>(
                $"api/Expenses/summary/categories/range?startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}");
        }

        public async Task<List<MonthlyExpenseTotalDto>> GetMonthlyExpenseTotals(DateTime startDate, DateTime endDate)
        {
            return await _httpClient.GetFromJsonAsync<List<MonthlyExpenseTotalDto>>(
                $"api/Expenses/summary/monthly/range?startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}");
        }
    }
} 