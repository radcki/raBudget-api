using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;
using RestSharp;
using RestSharp.Authenticators;

namespace WebApi.Services
{
    public class EmailService : IEmailSender
    {
        #region Constructors

        public EmailService()
        {
            var apiKey = Environment.GetEnvironmentVariable("MGAPIKEY");

            ApiUrl = Environment.GetEnvironmentVariable("MGAPIURL");
            Client = new RestClient();
            Client.BaseUrl = new Uri("https://api.mailgun.net/v3");
            Client.Authenticator = new HttpBasicAuthenticator("api", apiKey);
        }

        #endregion

        #region Properties

        private string ApiUrl { get; set; }
        private RestClient Client { get; set; }

        #endregion

        #region Methods

        public IRestResponse SendEmailConfirmEmail(string email, string verificationCode)
        {
            var subject = "raBudget - weryfikacja adresu email";
            var htmlMessage = "Twój kod weryfikacyjny do podania po zalogowaniu: " + verificationCode;

            return SendEmail(email, subject, htmlMessage);
        }

        public IRestResponse SendPasswordRecoveryEmail(string email, string verificationCode)
        {
            var subject = "raBudget - odzyskiwanie hasła";
            var htmlMessage = "Twój kod weryfikacyjny do podania przy odzyskiwaniu hasła: " + verificationCode;

            return SendEmail(email, subject, htmlMessage);
        }


        /// <inheritdoc />
        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var request = new RestRequest();
            request.AddParameter("domain", ApiUrl, ParameterType.UrlSegment);
            request.Resource = "{domain}/messages";
            request.AddParameter("from", "raBudget <no-reply@rabt.pl>");
            request.Method = Method.POST;
            request.AddParameter("to", email);
            request.AddParameter("subject", subject);
            request.AddParameter("text", htmlMessage);

            var cancellationTokenSource = new CancellationTokenSource();
            await Client.ExecuteTaskAsync(request, cancellationTokenSource.Token);
        }

        public IRestResponse SendEmail(string email, string subject, string htmlMessage)
        {
            var request = new RestRequest();
            request.AddParameter("domain", ApiUrl, ParameterType.UrlSegment);
            request.Resource = "{domain}/messages";
            request.AddParameter("from", "raBudget <no-reply@rabt.pl>");
            request.Method = Method.POST;
            request.AddParameter("to", email);
            request.AddParameter("subject", subject);
            request.AddParameter("text", htmlMessage);

            var cancellationTokenSource = new CancellationTokenSource();
            return Client.Execute(request);
        }

        #endregion
    }
}