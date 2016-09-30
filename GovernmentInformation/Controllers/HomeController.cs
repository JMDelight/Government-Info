using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RestSharp;
using RestSharp.Authenticators;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using GovernmentInformation.Models;

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
        public IActionResult Reroute(JObject response)
        {
            var abc = response["results"];
            var xyz = abc[0];
            ViewBag.Response = response["results"];
            ViewBag.Item = xyz;
            ViewBag.ABC = "Hello";
            return RedirectToAction("Index");
        }

        public static Task<IRestResponse> GetResponseContentAsync(RestClient theClient, RestRequest theRequest)
        {
            var tcs = new TaskCompletionSource<IRestResponse>();
            theClient.ExecuteAsync(theRequest, response => {
                tcs.SetResult(response);
            });
            return tcs.Task;
        }

        public IActionResult LocateLegislator(int zip)
        {
            var client = new RestClient("http://congress.api.sunlightfoundation.com/legislators/locate");
            var request = new RestRequest("?apikey=" + EnvironmentVariables.CongressApiKey + "&zip=" + zip);
            var response = new RestResponse();
            Task.Run(async () =>
            {
                response = await GetResponseContentAsync(client, request) as RestResponse;
            }).Wait();
            var jsonResponse = JsonConvert.DeserializeObject<JObject>(response.Content);
            ViewBag.Response = jsonResponse["results"];
            return View();
        }
        public IActionResult LegislatorDetail(string bioguide)
        {
            var clientLegislator = new RestClient("http://congress.api.sunlightfoundation.com/legislators");
            var requestLegislator = new RestRequest("?apikey=" + EnvironmentVariables.CongressApiKey + "&bioguide_id=" + bioguide);
            var responseLegislator = new RestResponse();
            Task.Run(async () =>
            {
                responseLegislator = await GetResponseContentAsync(clientLegislator, requestLegislator) as RestResponse;
            }).Wait();
            var jsonResponseLegislator = JsonConvert.DeserializeObject<JObject>(responseLegislator.Content);
            var resultLegislator = jsonResponseLegislator["results"];
            ViewBag.Response = resultLegislator;
            string image = "https://twitter.com/" + resultLegislator[0]["twitter_id"] + "/profile_image?size=original";
            ViewBag.Image = image;

            var clientCommittees = new RestClient("http://congress.api.sunlightfoundation.com/committees");
            var requestCommittees = new RestRequest("?apikey=" + EnvironmentVariables.CongressApiKey + "&per_page=all&member_ids=" + bioguide);
            var responseCommittees = new RestResponse();
            Task.Run(async () =>
            {
                responseCommittees = await GetResponseContentAsync(clientCommittees, requestCommittees) as RestResponse;
            }).Wait();
            var jsonResponseCommittees = JsonConvert.DeserializeObject<JObject>(responseCommittees.Content);
            var resultCommittees = jsonResponseCommittees["results"];
            ViewBag.Committees = resultCommittees;

            var clientBills = new RestClient("http://congress.api.sunlightfoundation.com/bills");
            var requestBills = new RestRequest("?apikey=" + EnvironmentVariables.CongressApiKey + "&per_page=all&sponsor_id=" + bioguide);
            var responseBills = new RestResponse();
            Task.Run(async () =>
            {
                responseBills = await GetResponseContentAsync(clientBills, requestBills) as RestResponse;
            }).Wait();
            var jsonResponseBills = JsonConvert.DeserializeObject<JObject>(responseBills.Content);
            var resultBills = jsonResponseBills["results"];
            ViewBag.Bills = resultBills;

            return View();
        }

        public IActionResult BillDetail(string billId)
        {

            //Rework to use data and return only needed api call


            //var clientLegislator = new RestClient("http://congress.api.sunlightfoundation.com/legislators");
            //var requestLegislator = new RestRequest("?apikey=" + EnvironmentVariables.CongressApiKey + "&bioguide_id=" + bioguide);
            //var responseLegislator = new RestResponse();
            //Task.Run(async () =>
            //{
            //    responseLegislator = await GetResponseContentAsync(clientLegislator, requestLegislator) as RestResponse;
            //}).Wait();
            //var jsonResponseLegislator = JsonConvert.DeserializeObject<JObject>(responseLegislator.Content);
            //var resultLegislator = jsonResponseLegislator["results"];
            //ViewBag.Response = resultLegislator;
            //string image = "https://twitter.com/" + resultLegislator[0]["twitter_id"] + "/profile_image?size=original";
            //ViewBag.Image = image;

            //var clientCommittees = new RestClient("http://congress.api.sunlightfoundation.com/committees");
            //var requestCommittees = new RestRequest("?apikey=" + EnvironmentVariables.CongressApiKey + "&per_page=all&member_ids=" + bioguide);
            //var responseCommittees = new RestResponse();
            //Task.Run(async () =>
            //{
            //    responseCommittees = await GetResponseContentAsync(clientCommittees, requestCommittees) as RestResponse;
            //}).Wait();
            //var jsonResponseCommittees = JsonConvert.DeserializeObject<JObject>(responseCommittees.Content);
            //var resultCommittees = jsonResponseCommittees["results"];
            //ViewBag.Committees = resultCommittees;

            var clientBills = new RestClient("http://congress.api.sunlightfoundation.com/bills/search");
            var requestBills = new RestRequest("?apikey=" + EnvironmentVariables.CongressApiKey + "&bill_id=" + billId);
            var responseBills = new RestResponse();
            Task.Run(async () =>
            {
                responseBills = await GetResponseContentAsync(clientBills, requestBills) as RestResponse;
            }).Wait();
            var jsonResponseBills = JsonConvert.DeserializeObject<JObject>(responseBills.Content);
            var resultBills = jsonResponseBills["results"];
            var abc = resultBills[0]["official_title"];
            ViewBag.SelectedBill = resultBills[0];

            return View();
        }

        public IActionResult CommitteeDetail(string bioguide)
        {
            //Rework to use data and return only needed api call


            //var clientLegislator = new RestClient("http://congress.api.sunlightfoundation.com/legislators");
            //var requestLegislator = new RestRequest("?apikey=" + EnvironmentVariables.CongressApiKey + "&bioguide_id=" + bioguide);
            //var responseLegislator = new RestResponse();
            //Task.Run(async () =>
            //{
            //    responseLegislator = await GetResponseContentAsync(clientLegislator, requestLegislator) as RestResponse;
            //}).Wait();
            //var jsonResponseLegislator = JsonConvert.DeserializeObject<JObject>(responseLegislator.Content);
            //var resultLegislator = jsonResponseLegislator["results"];
            //ViewBag.Response = resultLegislator;
            //string image = "https://twitter.com/" + resultLegislator[0]["twitter_id"] + "/profile_image?size=original";
            //ViewBag.Image = image;

            //var clientCommittees = new RestClient("http://congress.api.sunlightfoundation.com/committees");
            //var requestCommittees = new RestRequest("?apikey=" + EnvironmentVariables.CongressApiKey + "&per_page=all&member_ids=" + bioguide);
            //var responseCommittees = new RestResponse();
            //Task.Run(async () =>
            //{
            //    responseCommittees = await GetResponseContentAsync(clientCommittees, requestCommittees) as RestResponse;
            //}).Wait();
            //var jsonResponseCommittees = JsonConvert.DeserializeObject<JObject>(responseCommittees.Content);
            //var resultCommittees = jsonResponseCommittees["results"];
            //ViewBag.Committees = resultCommittees;

            //var clientBills = new RestClient("http://congress.api.sunlightfoundation.com/bills/search");
            
            //var responseBills = new RestResponse();
            //Task.Run(async () =>
            //{
            //    responseBills = await GetResponseContentAsync(clientBills, requestBills) as RestResponse;
            //}).Wait();
            //var jsonResponseBills = JsonConvert.DeserializeObject<JObject>(responseBills.Content);
            //var resultBills = jsonResponseBills["results"];
            //ViewBag.Bills = resultBills;

            return View();
        }
    }
}
