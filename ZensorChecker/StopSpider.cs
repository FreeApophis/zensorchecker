/*
 * Erstellt mit SharpDevelop.
 * Benutzer: apophis
 * Datum: 24.05.2009
 * Zeit: 16:45
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;

using Bdev.Net.Dns;

namespace apophis.ZensorChecker
{

    /// <summary>
    /// Description of Spider.
    /// </summary>
    public class StopSpider
    {
        List<SpiderInfo> spiderlist = new List<SpiderInfo>();
        
        // for fast existence lookup only!
        Dictionary<string, bool> spidercheck = new Dictionary<string, bool>();
        
        private IPAddress providerDNS;

        private IPAddress censorRedirect;
        private string provider;
        private string country;
        private string reporter;
        
        public StopSpider(IPAddress providerDNS, IPAddress censorRedirect, string provider, string country, string reporter)
        {
            // initiate spiderlist
            
            this.providerDNS = providerDNS;
            this.censorRedirect = censorRedirect;
            this.provider = provider;
            this.country = country;
            this.reporter = reporter;
            
             // add an initial list in randomized order, this will also prevent adding already known urls!
            SortedList<int, string> rlist = new SortedList<int, string>();
            TextReader inputurls = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("shortlist.txt"));
            string url; Random r = new Random();
            while((url = inputurls.ReadLine()) != null) {
                rlist.Add(r.Next(), url);
            }
            
           
            foreach(string uri in rlist.Values) {
                spiderlist.Add(new SpiderInfo(uri, 0));
                spidercheck.Add(uri, true);
            }

        }
        
        public void PublishNewUrls() {
            
        }
        
        private bool crawl = true;
        private int pooled = 0;
        private int running = 0;        
        private SpiderInfo lastfinshed;
        
        public void CrawlSpiderList() {
            int index = 0; lastfinshed = spiderlist[0];
            while(crawl) {

                if(index >= spiderlist.Count || pooled >= 100) {
                    Thread.Sleep(500);
                    Console.WriteLine("status: i" + (index-1) + "|" + lastfinshed.URL + "|" + lastfinshed.Depth + "|c" + spiderlist.Count+ "|" + running + "/" + pooled);
                    continue;
                }

                pooled++;

                ThreadPool.QueueUserWorkItem(FindNewUrls, (object) spiderlist[index]);
                
                index++;

            }
        }
        
        private Regex hrefMatch = new Regex("(?<=href=\"http://)[^\"]*(?=\")");
        
        private void FindNewUrls(object spiderInfo) {
            running++;
            // We cannot use WebClient or similar, since we cannot rely on the DNS resolution!
            TcpClient client = new TcpClient();
            IPAddress ip = GetRealIPFromUri(((SpiderInfo)spiderInfo).URL);
            //check for censorship

            CheckIfCensored((SpiderInfo) spiderInfo);
            
            if(ip == null) {
                // Invalid Response
                pooled--; running--;
                return;
            }
            
            try {
                client.Connect(ip, 80);
            } catch (Exception) {
                pooled--; running--;
                return;
            }
            
            //Send Request
            TextWriter tw = new StreamWriter(client.GetStream());
            tw.WriteLine("GET / HTTP/1.1");
            tw.WriteLine("Host: " + ((SpiderInfo)spiderInfo).URL);
            tw.WriteLine("User-Agent: Mozilla/5.0 (compatible; zensorchecker/" + this.GetType().Assembly.GetName().Version.ToString() + ";  http://zensorchecker.origo.ethz.ch/)");
            tw.WriteLine();
            tw.Flush();
            
            
            TextReader document = new StreamReader(client.GetStream());
            string line;
            try {
                while((line = document.ReadLine()) != null) {
                    MatchCollection mc = hrefMatch.Matches(line);
                    
                    foreach(Match m in mc) {
                        string href = m.Value + "/";
                        string url = href.Substring(0, href.IndexOf('/'));
                        if (!spidercheck.ContainsKey(url)) {
                            spiderlist.Add( new SpiderInfo(url, ((SpiderInfo)spiderInfo).Depth + 1));
                            spidercheck.Add(url, true);
                            
                        }
                    }
                }
            } catch (Exception) {
                ((SpiderInfo)spiderInfo).ReadError = true;
            }
            lastfinshed = (SpiderInfo)spiderInfo;
            pooled--; running--;
        }

        void CheckIfCensored(SpiderInfo spiderInfo)
        {
            if (spiderInfo.Depth==0) {
                // no test needed
                return;
            }
            try {
                Request request = new Request();
                request.AddQuestion(new Question(spiderInfo.URL, DnsType.ANAME, DnsClass.IN));
                Response response = Resolver.Lookup(request, providerDNS);
                if (((ANameRecord)response.Answers[0].Record).IPAddress.ToString() == this.censorRedirect.ToString()) {
                    switch(PostNewFoundUrl(spiderInfo.URL)) {
                        case ReturnState.OK:
                            break;
                        case ReturnState.Failed:
                            break;
                        case ReturnState.NotNew:
                            break;
                    }
                    
                    
                    Console.WriteLine("> " + spiderInfo.URL + " (" + spiderInfo.Depth + ") [NEW Censored]");
                    spiderInfo.Censored = true;
                    
                    TextWriter tw = new StreamWriter("spider.txt", true);
                    tw.WriteLine(spiderInfo.URL);
                    tw.Close();
                    
                }
            } catch (Exception) {}
        }

        
        private IPAddress GetRealIPFromUri(string uri) {
            try {
                Request request = new Request();
                request.AddQuestion(new Question(uri, DnsType.ANAME, DnsClass.IN));
                Response response = Resolver.Lookup(request, DNSHelper.OpenDNS1);
                
                if (response.Answers[0].Record is ANameRecord) {
                    return ((ANameRecord)response.Answers[0].Record).IPAddress;
                }
                
                // CNAME redirect (infinite loop?)
                if (response.Answers[0].Record is NSRecord) {
                    return GetRealIPFromUri(((NSRecord)response.Answers[0].Record).DomainName);
                }
            } catch(ArgumentException) {
                //Invalid Domain name ignored
            } catch(NoResponseException) {
                //happens
            } catch(OverflowException) {
                //BUG in DNSResolver, should update to another one!
            }
            return null;
        }

        public enum ReturnState {
            OK, NotNew, Failed
        }
        
        public ReturnState PostNewFoundUrl(string url) {
            WebClient web = new WebClient();
            
            web.QueryString.Add("url", url); // new URL
            web.QueryString.Add("rip", this.censorRedirect.ToString()); // Redirected to
            web.QueryString.Add("cnt", this.country); // Country
            web.QueryString.Add("isp", this.provider); // ISP
            web.QueryString.Add("rep", this.reporter); // Reporter
            
            string s = web.DownloadString("http://apophis.ch/zensorchecker.php");
            if (s.EndsWith("[OK]")) {
                return ReturnState.OK;
            } else if (s.EndsWith("[NOTNEW]")) {
                return ReturnState.NotNew;
            } else {
                return ReturnState.Failed;
            }
            
        }
    }
}
