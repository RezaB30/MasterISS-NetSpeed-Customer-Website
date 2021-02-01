using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.ViewModels.Auth
{
    public class RecaptchaValidationModel
    {
        [JsonProperty("success")]
        public bool Success { get; set; }
        [JsonProperty("error-codes")]
        public List<string> ErrorCodes { get; set; }

        public static bool Validate(string encodedResponse)
        {
            //if (string.IsNullOrEmpty(encodedResponse)) return false;

            var client = new System.Net.WebClient();
            var secret = "6LemlQwaAAAAAMAzZ6gkJsOT6eFTMOdu5xWO8r5X";

            if (string.IsNullOrEmpty(secret)) return false;

            var googleReply = client.DownloadString(string.Format("https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}", secret, encodedResponse));

            var reCaptcha = JsonConvert.DeserializeObject<RecaptchaValidationModel>(googleReply);

            return reCaptcha.Success;
        }
    }
}
