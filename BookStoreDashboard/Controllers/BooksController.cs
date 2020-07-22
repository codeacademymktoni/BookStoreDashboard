using BookStoreDashboard.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace BookStoreDashboard.Controllers
{
    public class BooksController : Controller
    {
        private readonly string bookStoreBaseUrl;
        private readonly string authSchema;
        private readonly string apiKey;

        public BooksController(IConfiguration configuration)
        {
            bookStoreBaseUrl = configuration["BookStoreApi:BaseUrl"];
            authSchema = configuration["BookStoreApi:AuthSchema"];
            apiKey = configuration["BookStoreApi:ApiKey"];
        }

        public async Task<IActionResult> Overview()
        {
            HttpClient httpClient = new HttpClient();

            var httpResponse = await httpClient.GetAsync($"{bookStoreBaseUrl}/api/books");

            if (httpResponse.IsSuccessStatusCode)
            {
                var response = await httpResponse.Content.ReadAsStringAsync();
                var books = JsonConvert.DeserializeObject<List<BookViewModel>>(response);
                return View(books);
            }

            return RedirectToAction("Error", "Home");
        }

        public async Task<IActionResult> Edit(int id)
        {
            HttpClient httpClient = new HttpClient();

            var httpResponse = await httpClient.GetAsync($"{bookStoreBaseUrl}/api/books/{id}");

            if (httpResponse.IsSuccessStatusCode)
            {
                var response = await httpResponse.Content.ReadAsStringAsync();
                var book = JsonConvert.DeserializeObject<BookViewModel>(response);
                return View(book);
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Edit(BookViewModel book)
        {
            HttpClient httpClient = new HttpClient();

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(authSchema, apiKey);

            var httpResponse = await httpClient.PutAsJsonAsync<BookViewModel>($"{bookStoreBaseUrl}/api/books", book);

            if (httpResponse.IsSuccessStatusCode)
            {
                ViewBag.SuccessMessage = "Updated successfully";
                return View();
            }
            else
            {
                ViewBag.ErrorMessage = "Update failed";
                return View();
            }

        }

        public async Task<IActionResult> Delete(int id)
        {
            HttpClient _client = new HttpClient();

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(authSchema, apiKey);

            var response = await _client.DeleteAsync($"{bookStoreBaseUrl}/api/Books/{id}");

            return RedirectToAction(nameof(Overview));
        }
    }
}