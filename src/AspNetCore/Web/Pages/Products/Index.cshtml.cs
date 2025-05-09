using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using System.Threading.Tasks;
using Web.Models;

namespace Web.Pages.Products
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        public IndexModel(ILogger<IndexModel> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        public List<Product> Products { get; set; } = default!;

        [BindProperty(SupportsGet = true)]
        public string? Id { get; set; } = "5";

        public async Task OnGet()
        {
            ViewData["Message"] = $"êªïiÉJÉeÉSÉäî‘çÜ - {Id}";
            string jsonString = string.Empty;

            string actionName = $"api/catalog/category/{Id}/products";

            var httpClient = _httpClientFactory.CreateClient("BackendApi");
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, actionName);

            var response = await httpClient.SendAsync(httpRequestMessage);

            if (response.IsSuccessStatusCode)
            {
                jsonString = await response.Content.ReadAsStringAsync();
                Products = JsonSerializer.Deserialize<List<Product>>(jsonString, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                })!;
            }
        }
    }
}
