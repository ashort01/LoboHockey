using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;

namespace UNMHockeySite.Services
{
    public class CaptchaService
    {
        //Secret given to me when I signed up for the google captcha
        //the secret is stored in the web config file for the project
        private static readonly string secret =  WebConfigurationManager.AppSettings["captcha"];
        //create a client for querying the google captcha service
        private static readonly HttpClient client = new HttpClient();

        public static async Task<bool> VerifyUser(string captchaString)
        {
            //get a url string with the google recaptcha service verification, secret and response as parameters
            var url = String.Format("https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}",secret, captchaString);

            //see what the service says
            var response = await client.GetStringAsync(url);

            //parse the response from the server into JSON
            var obj = JsonConvert.DeserializeObject<RecaptchaResponse>(response);

            //return whether or not the captcha was successful
            return obj.Success;
        }

        private class RecaptchaResponse
        {
            //this class is a JSON representation of the response we would get from the service
            //we create this so we have something to parse are response into
            public bool Success { get; set; }
            [JsonProperty("error-codes")]
            public ICollection<string> ErrorCodes { get; set; }
            public RecaptchaResponse()
            {
                ErrorCodes = new HashSet<string>();
            }
        }
    }
}