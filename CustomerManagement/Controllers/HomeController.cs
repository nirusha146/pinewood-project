using Microsoft.AspNetCore.Mvc;
using CustomerManagement.Models;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Text;
using System.Collections.Generic;
using System.Net;

namespace CustomerManagement.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger<HomeController> _logger;
       
        private readonly string apiUrl = "http://localhost:5263/api/customers"; 

        public HomeController(IHttpClientFactory clientFactory,ILogger<HomeController> logger)
        {
            _clientFactory = clientFactory;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);
            var client = _clientFactory.CreateClient();

            var response = await client.SendAsync(request);

            //_logger.LogInformation("request is : {request}",request);

            if (response.IsSuccessStatusCode)
            {
                var responseStream = await response.Content.ReadAsStreamAsync();
                //_logger.LogInformation("respone is : {response}",response);
                var customers = await JsonSerializer.DeserializeAsync<IEnumerable<Customer>>(responseStream,new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                
                return View(customers);
            }
            else
            {
                return View(new List<Customer>());
            }
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Customer customer)
        {
            var client = _clientFactory.CreateClient();
            var content = new StringContent(JsonSerializer.Serialize(customer), Encoding.UTF8, "application/json");
            var response = await client.PostAsync(apiUrl, content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }

            if (response.StatusCode == HttpStatusCode.Conflict)
            {
                ModelState.AddModelError("Email", "Email address already exists.");
            }

            return View(customer);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var client = _clientFactory.CreateClient();
            var response = await client.GetAsync($"{apiUrl}/{id}");

            if (response.IsSuccessStatusCode)
            {
                var responseStream = await response.Content.ReadAsStreamAsync();
                var customer = await JsonSerializer.DeserializeAsync<Customer>(responseStream,new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                return View(customer);
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Customer customer)
        {
            var client = _clientFactory.CreateClient();
            var content = new StringContent(JsonSerializer.Serialize(customer), Encoding.UTF8, "application/json");
            var response = await client.PutAsync($"{apiUrl}/{id}", content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }

            return View(customer);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var client = _clientFactory.CreateClient();
            var response = await client.GetAsync($"{apiUrl}/{id}");

            if (response.IsSuccessStatusCode)
            {
                var responseStream = await response.Content.ReadAsStreamAsync();
                var customer = await JsonSerializer.DeserializeAsync<Customer>(responseStream,new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                return View(customer);
            }

            return NotFound();
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var client = _clientFactory.CreateClient();
            var response = await client.DeleteAsync($"{apiUrl}/{id}");

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }

            return NotFound();
        }
    }
}
