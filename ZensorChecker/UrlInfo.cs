/*
 * Erstellt mit SharpDevelop.
 * Benutzer: apophis
 * Datum: 24.05.2009
 * Zeit: 22:00
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */
using System;
using System.Net;

namespace ZensorChecker
{
    /// <summary>
    /// Description of UrlInfo.
    /// </summary>
    public class UrlInfo
    {
        private string url;
        
        public string URL {
            get { 
                return url; 
            }
        }
        
        public bool queryAgain;
        
        public bool QueryAgain {
            get {
                return queryAgain;
            }
        }
        
        private IPAddress ipFromLocalDns;
        
        public IPAddress IPFromLocalDns {
            get { 
                return ipFromLocalDns; 
            }
        }
        
        public UrlInfo(string url)
        {
            this.url = url;
            this.queryAgain = true;
        }
        
        public void SetIP(IPAddress ip) {
            this.ipFromLocalDns = ip;
            this.queryAgain = false;
        }
    }
}
