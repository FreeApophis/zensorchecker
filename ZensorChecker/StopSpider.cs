﻿/**
 * Project: Zensorchecker: A dns checker to identify potentiel zensoring from you 
 * File name: Program.cs
 * Description:  
 *   
 * @author Thomas Bruderer, www.apophis.ch, apophis@apophis.ch, Copyright (C) 2009-2011
 * @version 0.6
 *   
 * @see The GNU Public License (GPL)
 *
 * This program is free software; you can redistribute it and/or modify 
 * it under the terms of the GNU General Public License as published by 
 * the Free Software Foundation; either version 2 of the License, or 
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful, but 
 * WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY 
 * or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License 
 * for more details.
 * 
 * You should have received a copy of the GNU General Public License along 
 * with this program; if not, write to the Free Software Foundation, Inc., 
 * 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
 **/

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Linq;

using Heijden.DNS;

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
        private Resolver openDnsResolver;

        public StopSpider(IPAddress providerDNS, IPAddress censorRedirect, string provider, string country, string reporter)
        {
            // initiate spiderlist

            this.providerDNS = providerDNS;
            this.censorRedirect = censorRedirect;
            this.provider = provider;
            this.country = country;
            this.reporter = reporter;
            this.openDnsResolver = new Resolver(Resolver.DefaultDnsServers[0]);

            // add an initial list in randomized order, this will also prevent adding already known urls!
            SortedList<int, string> rlist = new SortedList<int, string>();
            TextReader inputurls = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("shortlist.txt"));
            string url; Random r = new Random();
            while ((url = inputurls.ReadLine()) != null)
            {
                rlist.Add(r.Next(), url);
            }


            foreach (string uri in rlist.Values)
            {
                spiderlist.Add(new SpiderInfo(uri, 0));
                spidercheck.Add(uri, true);
            }
        }

        private bool crawl = true;
        private int pooled = 0;
        private int running = 0;
        private SpiderInfo lastfinshed;

        public void CrawlSpiderList()
        {
            int index = 0; lastfinshed = spiderlist[0];
            while (crawl)
            {

                if (index >= spiderlist.Count || pooled >= 100)
                {
                    Thread.Sleep(500);
                    Console.WriteLine("status: i" + (index - 1) + "|" + lastfinshed.URL + "|" + lastfinshed.Depth + "|c" + spiderlist.Count + "|" + running + "/" + pooled);
                    continue;
                }

                pooled++;

                ThreadPool.QueueUserWorkItem(FindNewUrls, (object)spiderlist[index]);

                index++;

            }
        }

        private Regex hrefMatch = new Regex("(?<=href=\"http://)[^\"]*(?=\")");

        private void FindNewUrls(object o)
        {
            var spiderInfo = (SpiderInfo)o;
            running++;
            // We cannot use WebClient or similar, since we cannot rely on the DNS resolution!
            TcpClient client = new TcpClient();
            IPAddress ip = DNSHelper.ResolveUri(openDnsResolver, spiderInfo.URL).First();
            //check for censorship

            CheckIfCensored(spiderInfo);

            if (ip == null)
            {
                // Invalid Response
                pooled--; running--;
                return;
            }

            try
            {
                client.Connect(ip, 80);
            }
            catch (Exception)
            {
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
            try
            {
                while ((line = document.ReadLine()) != null)
                {
                    MatchCollection mc = hrefMatch.Matches(line);

                    foreach (Match m in mc)
                    {
                        string href = m.Value + "/";
                        string url = href.Substring(0, href.IndexOf('/'));
                        if (!spidercheck.ContainsKey(url))
                        {
                            spiderlist.Add(new SpiderInfo(url, ((SpiderInfo)spiderInfo).Depth + 1));
                            spidercheck.Add(url, true);

                        }
                    }
                }
            }
            catch (Exception)
            {
                ((SpiderInfo)spiderInfo).ReadError = true;
            }
            lastfinshed = (SpiderInfo)spiderInfo;
            pooled--; running--;
        }

        void CheckIfCensored(SpiderInfo spiderInfo)
        {
            if (spiderInfo.Depth == 0)
            {
                // no test needed
                return;
            }
            try
            {
                // TODO: censored
            }
            catch (Exception) { }
        }

        public enum ReturnState
        {
            OK, NotNew, Failed
        }
    }
}
