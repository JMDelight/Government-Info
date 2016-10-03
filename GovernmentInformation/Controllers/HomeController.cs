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
using System.Text.RegularExpressions;

namespace GovernmentInformation.Controllers
{
    public class HomeController : Controller
    {
        public static Task<IRestResponse> GetResponseContentAsync(RestClient theClient, RestRequest theRequest)
        {
            var tcs = new TaskCompletionSource<IRestResponse>();
            theClient.ExecuteAsync(theRequest, response => {
                tcs.SetResult(response);
            });
            return tcs.Task;
        }

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
            var clientBills = new RestClient("http://congress.api.sunlightfoundation.com/bills/search");
            var requestBills = new RestRequest("?apikey=" + EnvironmentVariables.CongressApiKey + "&bill_id=" + billId);
            var responseBills = new RestResponse();
            Task.Run(async () =>
            {
                responseBills = await GetResponseContentAsync(clientBills, requestBills) as RestResponse;
            }).Wait();
            var jsonResponseBills = JsonConvert.DeserializeObject<JObject>(responseBills.Content);
            var resultBills = jsonResponseBills["results"];
            string billUrl = (string) resultBills[0]["last_version"]["urls"]["html"];
            ViewBag.SelectedBill = resultBills[0];

            var clientBillText = new RestClient(billUrl);
            var requestBillText = new RestRequest();
            var responseBillText = new RestResponse();
            Task.Run(async () =>
            {
                responseBillText = await GetResponseContentAsync(clientBillText, requestBillText) as RestResponse;
            }).Wait();
            var billText = responseBillText.Content.ToString();
            string processedBillText = Regex.Replace(billText, @"<\S*>", "");
            ViewBag.BillText = processedBillText;
            return View();
        }

        public IActionResult CommitteeDetail(string committeeId)
        {
            var clientCommittees = new RestClient("http://congress.api.sunlightfoundation.com/committees");
            var requestCommittees = new RestRequest("?apikey=" + EnvironmentVariables.CongressApiKey + "&committee_id=" + committeeId);
            var responseCommittees = new RestResponse();
            Task.Run(async () =>
            {
                responseCommittees = await GetResponseContentAsync(clientCommittees, requestCommittees) as RestResponse;
            }).Wait();
            var jsonResponseCommittees = JsonConvert.DeserializeObject<JObject>(responseCommittees.Content);
            var resultCommittee = jsonResponseCommittees["results"][0];
            ViewBag.Committee = resultCommittee;
            bool isSubCommittee = (bool)resultCommittee["subcommittee"];
            ViewBag.IsSubCommittee = isSubCommittee;
            if (!isSubCommittee)
            {
                var clientSubCommittees = new RestClient("http://congress.api.sunlightfoundation.com/committees");
                var requestSubCommittees = new RestRequest("?apikey=" + EnvironmentVariables.CongressApiKey + "&parent_committee_id=" + committeeId);
                var responseSubCommittees = new RestResponse();
                Task.Run(async () =>
                {
                    responseSubCommittees = await GetResponseContentAsync(clientSubCommittees, requestSubCommittees) as RestResponse;
                }).Wait();
                var jsonResponseSubCommittees = JsonConvert.DeserializeObject<JObject>(responseSubCommittees.Content);
                var resultCommittees = jsonResponseSubCommittees["results"];
                ViewBag.SubCommittees = resultCommittees;
            }
            else
            {
                var clientParentCommittee = new RestClient("http://congress.api.sunlightfoundation.com/committees");
                var requestParentCommittee = new RestRequest("?apikey=" + EnvironmentVariables.CongressApiKey + "&committee_id=" + resultCommittee["parent_committee_id"]);
                var responseParentCommittee = new RestResponse();
                Task.Run(async () =>
                {
                    responseParentCommittee = await GetResponseContentAsync(clientParentCommittee, requestParentCommittee) as RestResponse;
                }).Wait();
                var jsonResponseParentCommittee = JsonConvert.DeserializeObject<JObject>(responseParentCommittee.Content);
                var resultCommittees = jsonResponseParentCommittee["results"][0];
                ViewBag.ParentCommittee = resultCommittees;
            }

            var clientCommitteeMembers = new RestClient("http://congress.api.sunlightfoundation.com/committees");
            var requestCommitteeMembers = new RestRequest("?apikey=" + EnvironmentVariables.CongressApiKey + "&fields=members&committee_id=" + committeeId);
            var responseCommitteeMembers = new RestResponse();
            Task.Run(async () =>
            {
                responseCommitteeMembers = await GetResponseContentAsync(clientCommitteeMembers, requestCommitteeMembers) as RestResponse;
            }).Wait();
            var jsonResponseCommitteeMembers = JsonConvert.DeserializeObject<JObject>(responseCommitteeMembers.Content);
            var resultCommitteeMembers = jsonResponseCommitteeMembers["results"][0]["members"];
            ViewBag.CommitteeMembers = resultCommitteeMembers;

            return View();
        }
    }
}
