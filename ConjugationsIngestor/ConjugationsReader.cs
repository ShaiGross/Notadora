using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ConjugationsIngestor
{ 
    public static class ConjugationsReader
    {
        #region Consts

        private const string CONJUGATIONS_URL_APP_KEY = "CONJUGATIONS_URL";

        #endregion

        #region Static Methods

        public static string DownloadConjugationsHTML(string Infinative)
        {
            var baseUrl = ConfigurationManager.AppSettings[CONJUGATIONS_URL_APP_KEY];
            var verbUrl = $"{baseUrl}/{Infinative}";
            string html;

            if (verbUrl == null)
                return null;

            using (var client = new WebClient())
            {
                try
                {
                    html = client.DownloadString(verbUrl);
                }
                catch
                {
                    return null;
                }
            }

            return html;
        }

        #endregion
    }
}
