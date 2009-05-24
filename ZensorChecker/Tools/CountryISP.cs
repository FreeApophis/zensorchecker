/*
 * Erstellt mit SharpDevelop.
 * Benutzer: apophis
 * Datum: 24.05.2009
 * Zeit: 15:39
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */
using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;

namespace apophis.ZensorChecker
{
    /// <summary>
    /// Description of CountryISP.
    /// </summary>
    public class CountryISP
    {
        private IPAddress externalIP;
        
        public IPAddress ExternalIP {
            get { return externalIP; }
        }
        
        private string country;
        
        public string Country {
            get { return country; }
        }
        
        private string isp;
        
        public string Isp {
            get { return isp; }
        }
        
        private string region;
        
        public string Region {
            get { return region; }
        }
        
        private string city;
        
        public string City {
            get { return city; }
        }
        
        private string timezone;
        
        public string Timezone {
            get { return timezone; }
        }
        
        private string networkspeed;
        
        public string Networkspeed {
            get { return networkspeed; }
        }
        
        private Regex strongMatch = new Regex(@"(?<=<strong>)[^<]*(?=</strong>)");
        public CountryISP()
        {
            WebClient web = new WebClient();
            
            // Webbrowser
            web.Headers.Add ("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");

            StreamReader reader = new StreamReader(web.OpenRead("http://www.ip2location.com/ib2/"));
            string line;
            while((line = reader.ReadLine())!= null) {
                MatchCollection mc = strongMatch.Matches(line);
                if(mc.Count > 6) {
                    externalIP = IPAddress.Parse(mc[0].Value);
                    isp = mc[1].Value;
                    country = mc[2].Value;
                    region = mc[3].Value;
                    city = mc[4].Value;
                    timezone = mc[5].Value;
                    networkspeed = mc[6].Value;
                }
            }
        }
    }
}
