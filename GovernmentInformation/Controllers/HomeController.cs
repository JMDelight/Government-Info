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
                string legislatorName = (string) resultLegislator["first_name"] + " " + (string) resultLegislator["last_name"];
                ApiCall newCall = new ApiCall(columnId, "Legislator", legislatorName, bioguideId: bioguide);
                apiCalls.Add(newCall);
            }
            string image = "";
            if (resultLegislator["twitter_id"] != null)
            {
                image = "https://twitter.com/" + resultLegislator["twitter_id"] + "/profile_image?size=original";
            }
            else if (resultLegislator["facebook_id"] != null)
            {
                image = "graph.facebook.com/v2.8/ " + resultLegislator["facebook_id"] + "/picture?height=200";
            }

            ViewBag.Legislator = resultLegislator;
            ViewBag.Image = image;
            ViewBag.Calls = apiCalls;
            ViewBag.Committees = resultCommittees;
            ViewBag.Bills = resultBills;
            ViewBag.ThisColumn = columnId;
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
                string formattedBillText = Regex.Replace(processedBillText, @"_{71}", "</p><hr /><p style=\"display: none; \">");
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
                ApiCall newCall = new ApiCall(columnId, "Bill", billTitle, billId: billId);
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
            int index = Committee.retrievedCommittees.FindIndex(committee => committee.CommitteeId == committeeId);
            JToken resultCommittee;
            bool isSubCommittee;
            JToken subCommittees;
            JToken parentCommittee;
            //JToken committeeMembers;
            JToken resultCommitteeMembers;
            JToken resultBills;

            if (index >= 0)
            {
                Committee foundCommittee = Committee.retrievedCommittees[index];
                resultCommittee = foundCommittee.JsonResponse;
                isSubCommittee = foundCommittee.IsSubCommittee;
                resultCommitteeMembers = foundCommittee.CommitteeMembers;
                resultBills = foundCommittee.CommitteesBills;
                if (!isSubCommittee)
                {
                    subCommittees = foundCommittee.SubCommittees;
                    ViewBag.SubCommittees = subCommittees;
                }
                else
                {
                    int parentIndex = Committee.retrievedCommittees.FindIndex(committee => committee.CommitteeId == foundCommittee.ParentCommitteeId);
                    //parentCommittee = Committee.retrievedCommittees[parentIndex].JsonResponse;
                    //
                    // Retrieve Parent Committee Re retrieve due to issues with linked committees currently
                    var clientParentCommittee = new RestClient("http://congress.api.sunlightfoundation.com/committees");
                    var requestParentCommittee = new RestRequest("?apikey=" + EnvironmentVariables.CongressApiKey + "&committee_id=" + resultCommittee["parent_committee_id"]);
                    var responseParentCommittee = new RestResponse();
                    Task.Run(async () =>
                    {
                        responseParentCommittee = await GetResponseContentAsync(clientParentCommittee, requestParentCommittee) as RestResponse;
                    }).Wait();
                    var jsonResponseParentCommittee = JsonConvert.DeserializeObject<JObject>(responseParentCommittee.Content);
                    var jTokenParentCommittee = jsonResponseParentCommittee["results"][0];
                    ViewBag.ParentCommittee = jTokenParentCommittee;

                    //ViewBag.ParentCommittee = parentCommittee;
                }
            }
            else
            {
                // Retrieve Main Committee
                var clientCommittees = new RestClient("http://congress.api.sunlightfoundation.com/committees");
                var requestCommittees = new RestRequest("?apikey=" + EnvironmentVariables.CongressApiKey + "&committee_id=" + committeeId);
                var responseCommittees = new RestResponse();
                Task.Run(async () =>
                {
                    responseCommittees = await GetResponseContentAsync(clientCommittees, requestCommittees) as RestResponse;
                }).Wait();
                var jsonResponseCommittees = JsonConvert.DeserializeObject<JObject>(responseCommittees.Content);
                resultCommittee = jsonResponseCommittees["results"][0];

                //Retrieve Committee Members for Main Committee
                var clientCommitteeMembers = new RestClient("http://congress.api.sunlightfoundation.com/committees");
                var requestCommitteeMembers = new RestRequest("?apikey=" + EnvironmentVariables.CongressApiKey + "&fields=members&committee_id=" + committeeId);
                var responseCommitteeMembers = new RestResponse();
                Task.Run(async () =>
                {
                    responseCommitteeMembers = await GetResponseContentAsync(clientCommitteeMembers, requestCommitteeMembers) as RestResponse;
                }).Wait();
                var jsonResponseCommitteeMembers = JsonConvert.DeserializeObject<JObject>(responseCommitteeMembers.Content);
                resultCommitteeMembers = jsonResponseCommitteeMembers["results"][0]["members"];

                //Retrieve Bills before a Committee

                var clientBills = new RestClient("http://congress.api.sunlightfoundation.com/bills");
                var requestBills = new RestRequest("?apikey=" + EnvironmentVariables.CongressApiKey + "&history.enacted=false&history.active=true&per_page=all&committee_ids=" + committeeId);
                var responseBills = new RestResponse();
                Task.Run(async () =>
                {
                    responseBills = await GetResponseContentAsync(clientBills, requestBills) as RestResponse;
                }).Wait();
                var jsonResponseBills = JsonConvert.DeserializeObject<JObject>(responseBills.Content);
                resultBills = jsonResponseBills["results"];
                

                // Add this to stored calls
                string committeeName = (string) resultCommittee["name"];
                ApiCall newCall = new ApiCall(columnId, "Committee", committeeName, committeeId: committeeId);
                apiCalls.Add(newCall);


                isSubCommittee = (bool)resultCommittee["subcommittee"];
                if (!isSubCommittee)
                {
                    // Retrive Subcommittees for Main Committee
                    var clientSubCommittees = new RestClient("http://congress.api.sunlightfoundation.com/committees");
                    var requestSubCommittees = new RestRequest("?apikey=" + EnvironmentVariables.CongressApiKey + "&parent_committee_id=" + committeeId);
                    var responseSubCommittees = new RestResponse();
                    Task.Run(async () =>
                    {
                        responseSubCommittees = await GetResponseContentAsync(clientSubCommittees, requestSubCommittees) as RestResponse;
                    }).Wait();
                    var jsonResponseSubCommittees = JsonConvert.DeserializeObject<JObject>(responseSubCommittees.Content);
                    var resultCommittees = jsonResponseSubCommittees["results"];

                    Committee madeCommittee = new Committee(committeeId, resultCommittee, resultCommitteeMembers, false, resultCommittees, committeesBills: resultBills);
                    Committee.retrievedCommittees.Add(madeCommittee);
                    ViewBag.SubCommittees = resultCommittees;
                }
                else
                {
                    // Retrieve Parent Committee
                    var clientParentCommittee = new RestClient("http://congress.api.sunlightfoundation.com/committees");
                    var requestParentCommittee = new RestRequest("?apikey=" + EnvironmentVariables.CongressApiKey + "&committee_id=" + resultCommittee["parent_committee_id"]);
                    var responseParentCommittee = new RestResponse();
                    Task.Run(async () =>
                    {
                        responseParentCommittee = await GetResponseContentAsync(clientParentCommittee, requestParentCommittee) as RestResponse;
                    }).Wait();
                    var jsonResponseParentCommittee = JsonConvert.DeserializeObject<JObject>(responseParentCommittee.Content);
                    var jTokenParentCommittee = jsonResponseParentCommittee["results"][0];
                    ViewBag.ParentCommittee = jTokenParentCommittee;

                    ////Retrive Committee Members for Parent Committee- No longer used due to issues with the retrieval function
                    //var clientParentCommitteeMembers = new RestClient("http://congress.api.sunlightfoundation.com/committees");
                    //var requestParentCommitteeMembers = new RestRequest("?apikey=" + EnvironmentVariables.CongressApiKey + "&fields=members&committee_id=" + resultCommittee["parent_committee_id"]);
                    //var responseParentCommitteeMembers = new RestResponse();
                    //Task.Run(async () =>
                    //{
                    //    responseParentCommitteeMembers = await GetResponseContentAsync(clientParentCommitteeMembers, requestParentCommitteeMembers) as RestResponse;
                    //}).Wait();
                    //var jsonResponseParentCommitteeMembers = JsonConvert.DeserializeObject<JObject>(responseParentCommitteeMembers.Content);
                    //var resultParentCommitteeMembers = jsonResponseParentCommitteeMembers["results"][0]["members"];

                    //// Retrieve Parent Comittee's subcommittees
                    //var clientSubCommittees = new RestClient("http://congress.api.sunlightfoundation.com/committees");
                    //var requestSubCommittees = new RestRequest("?apikey=" + EnvironmentVariables.CongressApiKey + "&parent_committee_id=" + committeeId);
                    //var responseSubCommittees = new RestResponse();
                    //Task.Run(async () =>
                    //{
                    //    responseSubCommittees = await GetResponseContentAsync(clientSubCommittees, requestSubCommittees) as RestResponse;
                    //}).Wait();
                    //var jsonResponseSubCommittees = JsonConvert.DeserializeObject<JObject>(responseSubCommittees.Content);
                    //var parentSubCommittees = jsonResponseSubCommittees["results"];

                    ////Retrieve Bills before the Parent Committee

                    //var clientParentBills = new RestClient("http://congress.api.sunlightfoundation.com/bills");
                    //var requestParentBills = new RestRequest("?apikey=" + EnvironmentVariables.CongressApiKey + "&per_page=all&committee_ids=" + committeeId);
                    //var responseParentBills = new RestResponse();
                    //Task.Run(async () =>
                    //{
                    //    responseParentBills = await GetResponseContentAsync(clientParentBills, requestParentBills) as RestResponse;
                    //}).Wait();
                    //var jsonResponseParentBills = JsonConvert.DeserializeObject<JObject>(responseParentBills.Content);
                    //var resultParentBills = jsonResponseParentBills["results"];

                    //Committee parentCommitteeObject = new Committee((string) resultCommittee["parent_committee_id"], jTokenParentCommittee, resultParentCommitteeMembers, false, parentSubCommittees, committeesBills: resultParentBills);
                    //Committee.retrievedCommittees.Add(parentCommitteeObject);

                    Committee madeCommittee = new Committee(committeeId, resultCommittee, resultCommitteeMembers, true, parentCommitteeId: (string)jTokenParentCommittee["committee_id"], committeesBills: resultBills);
                    Committee.retrievedCommittees.Add(madeCommittee);
                }
            }

            ViewBag.CommitteesBills = resultBills;
            ViewBag.CommitteeMembers = resultCommitteeMembers;
            ViewBag.Committee = resultCommittee;
            ViewBag.Calls = apiCalls;
            ViewBag.IsSubCommittee = isSubCommittee;
            ViewBag.ThisColumn = columnId;
            return View();
        }

        public IActionResult ViewHistory()
        {
            ViewBag.Calls = apiCalls;
            return View();
        }
        public IActionResult ViewHistoryLookup(int apiID)
        {
            ApiCall foundApiCall = new ApiCall(-1, null, null);
            foreach( ApiCall call in apiCalls)
            {
                if (call.ColumnId == apiID)
                {
                    foundApiCall = call;
                }
            }
            if (foundApiCall.BioguideId != null)
            {
                return RedirectToAction("LegislatorDetail", new { columnId = -1, bioguide = foundApiCall.BioguideId });
                //foreach( Legislator foundLegislator in Legislator.retrievedLegislators)
                //{
                //    if (foundLegislator.BioguideId == foundApiCall.BioguideId)
                //    {
                //        var resultLegislator = foundLegislator.JsonResponse;
                //        var resultCommittees = foundLegislator.JsonResponseCommittees;
                //        var resultBills = foundLegislator.JsonResponseSponsoredBills;
                //        string image = "https://twitter.com/" + resultLegislator["twitter_id"] + "/profile_image?size=original";
                //        ViewBag.Legislator = resultLegislator;
                //        ViewBag.Image = image;
                //        ViewBag.Calls = apiCalls;
                //        ViewBag.Committees = resultCommittees;
                //        ViewBag.Bills = resultBills;

                //        return View("LegislatorDetail");
                //    }
                //}
            }
            else if(foundApiCall.BillID != null)
            {
                return RedirectToAction("BillDetail", new { columnId = -1, billId = foundApiCall.BillID });
            }
            else if (foundApiCall.CommitteeId != null)
            {
                return RedirectToAction("CommitteeDetail", new { columnId = -1, committeeId = foundApiCall.CommitteeId });
            }
            return View();
        }
    }
}
