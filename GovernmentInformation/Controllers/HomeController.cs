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
using GovernmentInformation.ViewModels;

namespace GovernmentInformation.Controllers
{
    public class HomeController : Controller
    {
        public static List<ApiCall> apiCalls = new List<ApiCall> {};
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
            ViewBag.Calls = apiCalls;
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
        public IActionResult LegislatorDetail(int columnId, string bioguide)
        {
            JToken resultLegislator;
            JToken resultCommittees;
            JToken resultBills;
            int index = Legislator.retrievedLegislators.FindIndex(legislator => legislator.BioguideId == bioguide);
            if (index >= 0)
            {
                resultLegislator = Legislator.retrievedLegislators[index].JsonResponse;
                resultCommittees = Legislator.retrievedLegislators[index].JsonResponseCommittees;
                resultBills = Legislator.retrievedLegislators[index].JsonResponseSponsoredBills;
            }
            else
            {
                var clientLegislator = new RestClient("http://congress.api.sunlightfoundation.com/legislators");
                var requestLegislator = new RestRequest("?apikey=" + EnvironmentVariables.CongressApiKey + "&bioguide_id=" + bioguide);
                var responseLegislator = new RestResponse();
                Task.Run(async () =>
                {
                    responseLegislator = await GetResponseContentAsync(clientLegislator, requestLegislator) as RestResponse;
                }).Wait();
                var jsonResponseLegislator = JsonConvert.DeserializeObject<JObject>(responseLegislator.Content);
                resultLegislator = jsonResponseLegislator["results"][0];

                var clientCommittees = new RestClient("http://congress.api.sunlightfoundation.com/committees");
                var requestCommittees = new RestRequest("?apikey=" + EnvironmentVariables.CongressApiKey + "&per_page=all&member_ids=" + bioguide);
                var responseCommittees = new RestResponse();
                Task.Run(async () =>
                {
                    responseCommittees = await GetResponseContentAsync(clientCommittees, requestCommittees) as RestResponse;
                }).Wait();
                var jsonResponseCommittees = JsonConvert.DeserializeObject<JObject>(responseCommittees.Content);
                resultCommittees = jsonResponseCommittees["results"];

                var clientBills = new RestClient("http://congress.api.sunlightfoundation.com/bills");
                var requestBills = new RestRequest("?apikey=" + EnvironmentVariables.CongressApiKey + "&per_page=all&sponsor_id=" + bioguide);
                var responseBills = new RestResponse();
                Task.Run(async () =>
                {
                    responseBills = await GetResponseContentAsync(clientBills, requestBills) as RestResponse;
                }).Wait();
                var jsonResponseBills = JsonConvert.DeserializeObject<JObject>(responseBills.Content);
                resultBills = jsonResponseBills["results"];
                Legislator foundLegislator = new Legislator(bioguide, resultLegislator, resultCommittees, resultBills);
                Legislator.retrievedLegislators.Add(foundLegislator);
            }
            string legislatorName = (string) resultLegislator["first_name"] + " " + (string) resultLegislator["last_name"];
            ApiCall newCall = new ApiCall(columnId, "Legislator", legislatorName, bioguideId: bioguide);
            apiCalls.Add(newCall);

            string image = "https://twitter.com/" + resultLegislator["twitter_id"] + "/profile_image?size=original";

            ViewBag.Legislator = resultLegislator;
            ViewBag.Image = image;
            ViewBag.Calls = apiCalls;
            ViewBag.Committees = resultCommittees;
            ViewBag.Bills = resultBills;

            return View();
        }

        public IActionResult BillDetail(int columnId, string billId)
        {

            int index = Bill.retrievedBills.FindIndex(bill => bill.BillId == billId);
            JToken resultBill;
            string finishedBillText;
            string billTitle;

            if (index >= 0)
            {
                resultBill = Bill.retrievedBills[index].JsonResponse;
                finishedBillText = Bill.retrievedBills[index].BillText;
            }
            else
            {
                var clientBills = new RestClient("http://congress.api.sunlightfoundation.com/bills/search");
                var requestBills = new RestRequest("?apikey=" + EnvironmentVariables.CongressApiKey + "&bill_id=" + billId);
                var responseBills = new RestResponse();
                Task.Run(async () =>
                {
                    responseBills = await GetResponseContentAsync(clientBills, requestBills) as RestResponse;
                }).Wait();
                var jsonResponseBills = JsonConvert.DeserializeObject<JObject>(responseBills.Content);
                resultBill = jsonResponseBills["results"][0];

                string billUrl = (string) resultBill["last_version"]["urls"]["html"];
                var clientBillText = new RestClient(billUrl);
                var requestBillText = new RestRequest();
                var responseBillText = new RestResponse();
                Task.Run(async () =>
                {
                    responseBillText = await GetResponseContentAsync(clientBillText, requestBillText) as RestResponse;
                }).Wait();
                var billText = responseBillText.Content.ToString();
                string processedBillText = Regex.Replace(billText, @"<\S*>", "");
                string formattedBillText = Regex.Replace(processedBillText, @"_{71}", "</p><hr /><p>");
                finishedBillText = formattedBillText.Replace("&lt;DELETED&gt;", "<span class='deleted'>").Replace("&lt;/DELETED&gt;", "</span>");

                Bill foundBill = new Bill(billId, resultBill, finishedBillText);
                Bill.retrievedBills.Add(foundBill);
            }



            string shortTitle = (string)resultBill["short_title"];
            if(shortTitle != null)
            {
                billTitle = (string) resultBill["short_title"];
            } 
            else
            {
                billTitle = (string)resultBill["official_title"];
            }

            if (index < 0)
            {
                ApiCall newCall = new ApiCall(columnId, "Bill", billTitle, bioguideId: billId);
                apiCalls.Add(newCall);
            }

            ViewBag.Calls = apiCalls;
            ViewBag.SelectedBill = resultBill;
            ViewBag.BillText = finishedBillText;
            ViewBag.ThisColumn = columnId;
            return View();
        }


        public IActionResult CommitteeDetail(int columnId, string committeeId)
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

            string committeeName = (string) resultCommittee["name"];
            ApiCall newCall = new ApiCall(columnId, "Committee", committeeName, bioguideId: committeeId);
            apiCalls.Add(newCall);
            ViewBag.Calls = apiCalls;

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

        public IActionResult ViewHistory()
        {
            ViewBag.Calls = apiCalls;
            return View();
        }
        public IActionResult ViewHistoryLookup(int apiID)
        {
            return View();
        }
    }
}
