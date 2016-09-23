using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RestSharp;
using RestSharp.Authenticators;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace GovernmentInformation.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }

        public void ApiTest()
        {
            var client = new RestClient("http://congress.api.sunlightfoundation.com/legislators/locate");
            var request = new RestRequest("?zip=97229&apikey=d5977756ff384a7da59c8e860379b4bd");
            client.ExecuteAsync(request, response =>
            {
                JObject jsonResponse = JsonConvert.DeserializeObject<JObject>(response.Content);
            });
            //Task.Run(async () =>
            //{
            //    response = await GetResponseContentAsync(client, request) as RestResponse;
            //}).Wait();
            //JObject jsonResponse = JsonConvert.DeserializeObject<JObject>(response.Content);
            
        }
    }
}
