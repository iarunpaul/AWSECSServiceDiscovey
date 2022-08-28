using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace EchoApi.Controllers
{
   [ApiController]
   [Route( "[controller]" )]
   public class DashboardController : ControllerBase
   {
      private readonly ILogger<DashboardController> _logger;

      public DashboardController( ILogger<DashboardController> logger )
      {
         _logger = logger;
      }

      [HttpGet]
      public async Task<Dashboard> GetDashboard()
      {
            var httpClient = new HttpClient();

            // Create and run the tasks that would fetch the purchases & recommendations asynchronously
            //var purchasesTask = httpClient.GetStringAsync($"http://dotnet-test-purchase/purchases");
            var purchasesTask = httpClient.GetStringAsync($"http://purchase-svc.dotnetmicros/purchases");

            //var purchasesTask = Task.FromResult<object>(null);
            var recommendationsTask = httpClient.GetStringAsync($"http://dotnet-recomm-svc.dotnetmicros/recommendations");

            // Await (Not Wait) on both the purchases and recommendations fetch tasks
            await Task.WhenAll(purchasesTask, recommendationsTask);

            _logger.LogInformation($"Purchases JSON: {purchasesTask.Result}");
            _logger.LogInformation($"Recommendations JSON: {recommendationsTask.Result}");

            // Retrieve the results and covert to required objects
            var purchases = JsonConvert.DeserializeObject<List<Purchase>>(purchasesTask.Result);
            var recommendations = JsonConvert.DeserializeObject<List<Recommendation>>(recommendationsTask.Result);

            // Return the aggregated Dashboard object
            return new Dashboard
            {
                Purchases = purchases,
                Recommendations = recommendations
            };
            //return new List<Purchase> {
            //    new Purchase()
            //    {
            //       BookName = "The Star Wars",
            //       DateOfPurchase = new System.DateTime(2022,1,1),
            //       Price = 100.99
            //    },
            //    new Purchase()
            //    {
            //       BookName = "The Gladiator",
            //       DateOfPurchase = new System.DateTime(2022,1,1),
            //       Price = 49.99
            //    }
            // };
        }
    }
}
