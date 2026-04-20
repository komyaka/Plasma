using System;
using System.Web;
using System.Web.Helpers;

namespace Plazma.Infrastructure
{
    public static class AntiForgeryHelper
    {
        private const string RequestVerificationTokenHeader = "RequestVerificationToken";
        private const string AntiForgeryFormField = "__RequestVerificationToken";

        public static void ValidateRequest(HttpRequestBase request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            string cookieToken = request.Cookies[AntiForgeryConfig.CookieName]?.Value;
            string formToken = request.Form[AntiForgeryFormField];

            if (string.IsNullOrWhiteSpace(formToken))
            {
                string headerValue = request.Headers[RequestVerificationTokenHeader];
                if (!string.IsNullOrWhiteSpace(headerValue))
                {
                    string[] tokens = headerValue.Split(':');
                    if (tokens.Length == 2)
                    {
                        if (string.IsNullOrWhiteSpace(cookieToken))
                        {
                            cookieToken = tokens[0];
                        }
                        formToken = tokens[1];
                    }
                    else
                    {
                        formToken = headerValue;
                    }
                }
            }

            AntiForgery.Validate(cookieToken, formToken);
        }
    }
}
