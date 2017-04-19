using CommandLine;
using CommandLine.Text;
using System;

namespace rest_sender
{
    class Options
    {
        [Option('r', "requestUrl", Required = true,
            HelpText = "The full request URL")]
        public string RequestUrl { get; set; }
        [Option('m', "method", Required = false, DefaultValue = "GET",
            HelpText = "The HTTP method to use (GET, POST, PATCH, DELETE)")]
        public string Method { get; set; }
        [Option('s', "scopes", Required = false, DefaultValue = "Mail.Read",
            HelpText = "The OAuth scopes to request during authorization. Multiple scopes should be separated by a semicolon (Mail.Read;Calendar.Read)")]
        public string Scopes { get; set; }
        [Option('l', "logFile", Required = false,
            HelpText = "Path to a log file. If specified, results will be saved to the file.")]
        public string LogFile { get; set; }
        [Option('p', "pageResults", Required = false, DefaultValue = false,
            HelpText = "If this option is specified, repeated requests will be sent to follow @odata.nextLink values")]
        public bool PageResults { get; set; }
        [Option('o', "outputToken", Required = false, DefaultValue = false,
            HelpText = "If this option is specified, the access token will be output to the screen")]
        public bool OutputToken { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this,
                (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
        }

        public bool IsValid()
        {
            // Check if the requestUrl is actually a valid url
            try
            {
                Uri requestUri = new Uri(RequestUrl);

                // Is this one of our endpoints?
                string host = requestUri.Host.ToLower();
                if ((string.Compare(host, "graph.microsoft.com", StringComparison.OrdinalIgnoreCase) != 0) &&
                    (string.Compare(host, "outlook.office.com", StringComparison.OrdinalIgnoreCase) != 0) &&
                    (string.Compare(host, "outlook.office365.com", StringComparison.OrdinalIgnoreCase) != 0))
                {
                    return false;
                }
            }
            catch (UriFormatException)
            {
                return false;
            }

            return true;
        }

        public string[] GetQualifiedScopes()
        {
            // Determine our target
            Uri requestUri = new Uri(RequestUrl);
            string host = requestUri.Host.ToLower();

            string[] scopeArray = Scopes.Split(new char[] {';'} , StringSplitOptions.RemoveEmptyEntries);

            // If not Graph, need to append host to scopes
            if (string.Compare(host, "graph.microsoft.com", StringComparison.OrdinalIgnoreCase) != 0)
            {
                for (int i = 0; i < scopeArray.Length; i++)
                {
                    scopeArray[i] = string.Format("https://{0}/{1}", host, scopeArray[i]);
                }
            }

            return scopeArray;
        }
    }
}
