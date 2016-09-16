using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using RestSharp.Authenticators;

namespace ApiWork
{
    class Program
    {
        static void Main(string[] args)
        {
            //1
            var client = new RestClient("https://api.twilio.com/2010-04-01");
            //2
            var request = new RestRequest("Accounts/{{Account SID}}/Messages", Method.POST);
            //3
            request.AddParameter("To", "2087611179");
            request.AddParameter("From", "5037085611");
            request.AddParameter("Body", "Hello world!");
            //4
            client.Authenticator = new HttpBasicAuthenticator("AC359fa0b37cec3659ed3e19e593227f7d", "62f4c970e8ddf5100a43d475908ad8e3");
            //5
            client.Execute(request);
        }
    }
}
