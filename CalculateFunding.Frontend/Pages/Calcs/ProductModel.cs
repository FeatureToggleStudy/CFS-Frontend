using System.Threading.Tasks;
using CalculateFunding.Frontend.ApiClient;
using CalculateFunding.Frontend.ApiClient.Models;
using CalculateFunding.Frontend.Interfaces.APiClient;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CalculateFunding.Frontend.Pages.Calcs
{
    public class ProductModel : PageModel
    {
        private readonly IAllocationsApiClient _apiClient;
        private readonly IBudgetApiClient _budgetApiClient;

        public ProductModel(IAllocationsApiClient apiClient, IBudgetApiClient budgetApiClient)
        {
            _apiClient = apiClient;
            _budgetApiClient = budgetApiClient;
        }
        public async Task OnGet(string id, string productId)
        {
            Budget = (await _budgetApiClient.GetBudgetAsync(id))?.Content;


            var response = await _apiClient.PostPreview(new PreviewRequest
            {
                BudgetId = Budget.Id,
                ProductId = Product.Id,
                Calculation = Product.Calculation?.SourceCode
            });

            Preview = response.Content;
        }

        public async Task OnPost(string id, string calculation)
        {
            Budget = (await _apiClient.GetBudget(id))?.Content;


            var response = await _apiClient.PostPreview(new PreviewRequest
            {
                BudgetId = Budget.Id,
                ProductId = Product.Id,
                Calculation = calculation ?? Product.Calculation?.SourceCode
            });

            if (!string.IsNullOrEmpty(calculation))
            {
                Product.Calculation = new ProductCalculation { SourceCode = calculation };
            }

            Preview = response.Content;
        }

        public PreviewResponse Preview { get; set; }

        public Specification Budget { get; set; }

        public Product Product { get; set; }
    }
}