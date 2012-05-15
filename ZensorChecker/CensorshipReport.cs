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
using System.Reflection;
using System.Threading;
using System.Linq;

using Heijden.DNS;

namespace apophis.ZensorChecker
{
    /// <summary>
    /// Description of CensorshipReport.
    /// </summary>
    public class CensorshipReport
    {
        private string provider;

        public string Provider
        {
            get { return provider; }
        }

        private string country;

        public string Country
        {
            get { return country; }
        }

        private DateTime date;

        public DateTime Date
        {
            get { return date; }
        }

        private bool isCensoring = false;

        public bool IsCensoring
        {
            get { return isCensoring; }
        }

        private IPAddress censorRedirect = null;

        public IPAddress CensorRedirect
        {
            get { return censorRedirect; }
        }

        private IEnumerable<IPAddress> dnsServers;

        public IEnumerable<IPAddress> DnsServers
        {
            get { return dnsServers; }
        }

        private List<string> cenosredUrls = new List<string>();

        public List<string> CenosredUrls
        {
            get { return cenosredUrls; }
        }

        private List<string> urlsToTest = new List<string>();

        public List<string> UrlsToTest
        {
            get { return urlsToTest; }
        }

        private string reporter;

        public string Reporter
        {
            get { return reporter; }
        }

        private bool reportReady = false;

        private Resolver referenceResolver1;
        private Resolver referenceResolver2;
        private IEnumerable<Resolver> localResolvers;

        public CensorshipReport(string provider, string country, string reporter)
        {
            // Get the list from the resources

            string url;
            Random r = new Random();
            SortedList<int, string> rlist = new SortedList<int, string>();

            //foreach(string res in Assembly.GetExecutingAssembly().GetManifestResourceNames()) {
            //    Console.WriteLine("Resource: " + res);
            //}

            TextReader inputurls = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("baselist.txt"));
            while ((url = inputurls.ReadLine()) != null)
            {
                rlist.Add(r.Next(), url);
            }

            // the list in randomized Order
            this.urlsToTest.AddRange(rlist.Values);

            // Local DNS Server
            this.dnsServers = DNSHelper.GetLocalDNS();
            foreach (IPAddress dnsserver in this.dnsServers)
            {
                if ((dnsserver.ToString() == Resolver.DefaultDnsServers[0].ToString()) || (dnsserver.ToString() == Resolver.DefaultDnsServers[1].ToString()))
                {
                    Console.Write("Warning: one of your DNS Servers is an OpenDNS Server, which is not censored. Check might give invalid results.");
                    Thread.Sleep(5000);

                }
                if ((dnsserver.ToString().StartsWith("10.")) || (dnsserver.ToString().StartsWith("192.168.")))
                {
                    Console.Write("Warning: Your DNS seems to be a local adress, the gateway probably relies the request to your ISPs DNS, however its not transparent which DNS Server we actually use.");
                    Thread.Sleep(5000);
                }
            }

            // Resolvers
            referenceResolver1 = new Resolver(Resolver.DefaultDnsServers[0]);
            referenceResolver2 = new Resolver(Resolver.DefaultDnsServers[0]);
            localResolvers = dnsServers.Select(s => new Resolver(s, Resolver.DefaultPort));

            //Date
            this.date = DateTime.Now;

            //Account Information
            this.provider = provider;
            this.country = country;
            this.reporter = reporter;

        }

        /// <summary>
        /// You already know the IP adress of the censoring Server
        /// </summary>
        /// <param name="ip"></param>
        public void CensorServerHint(IPAddress ip)
        {
            this.censorRedirect = ip;
            this.isCensoring = true;
        }

        /// <summary>
        /// you want to use another DNS Server than the one in your local settings.
        /// </summary>
        /// <param name="ips"></param>
        public void DnsServerHint(IEnumerable<IPAddress> ips)
        {
            this.dnsServers = ips;
        }

        public void RunCheck()
        {
            if (reportReady)
            {
                return;
            }
            if (censorRedirect == null)
            {
                this.reportReady = true;
                this.isCensoring = false;
                return;
            }
            Console.WriteLine("We test now which adresses get censored. This will take very long.");
            Console.WriteLine();

            IPAddress providerDNS = null;
            foreach (IPAddress ip in this.dnsServers)
            {
                providerDNS = ip;
                break;
            }
            int i = 0;
            foreach (string url in this.urlsToTest)
            {
                i++;
                try
                {
                    if (false /*if censored*/)
                    {
                        Console.WriteLine("> " + i + "/" + urlsToTest.Count + " : " + url + " [Censored]");
                        //Console.Write("x");
                        Monitor.Enter(this.cenosredUrls);
                        this.cenosredUrls.Add(url);
                        Monitor.Exit(this.cenosredUrls);
                    }
                    else
                    {
                        Console.WriteLine("> " + i + "/" + urlsToTest.Count + " : " + url + " [Open]");
                        //Console.Write("o");
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine("> " + i + "/" + urlsToTest.Count + " : " + url + " [Skipped]");
                }
            }

            Console.WriteLine();
            Console.WriteLine();
            reportReady = true;
        }


        public void GetCensoringIP()
        {
            // If we already have a valid IP or a Report is ready, there is nothing to do anymore
            if ((this.reportReady) || (this.censorRedirect != null))
            {
                return;
            }

            Console.WriteLine("We try to find if you get censored, and to which IP you get redirected! This will take very long! If you know the redirct, use --censorhint to skip this test.");
            Console.WriteLine();

            Dictionary<string, int> censoringIPs = new Dictionary<string, int>();
            foreach (string url in this.urlsToTest)
            {
                try
                {
                    var ref1 = DNSHelper.ResolveUri(referenceResolver1, new Uri(url));
                    var ref2 = DNSHelper.ResolveUri(referenceResolver1, new Uri(url));


                    // TODO
                }
                catch (Exception)
                {
                    continue;
                }
            }
            Console.WriteLine();
            Console.WriteLine();
            int newmax = 0;

            RemoveFalsePositives(censoringIPs);

#if DEBUG
            TextWriter falsepositives = new StreamWriter("falsepositives" + DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day + "-" + DateTime.Now.Hour + "-" + DateTime.Now.Minute + ".txt", false);
#endif

            foreach (KeyValuePair<string, int> kvp in censoringIPs)
            {
                Console.WriteLine(kvp.Key + " has #" + kvp.Value);
#if DEBUG
                falsepositives.WriteLine(kvp.Key + " has #" + kvp.Value);
#endif
                if ((kvp.Value > 10) && (kvp.Value > newmax))
                {
                    newmax = kvp.Value;
                    this.censorRedirect = IPAddress.Parse(kvp.Key);
                    this.isCensoring = true;
                }
            }

#if DEBUG
            falsepositives.Close();
#endif

            Console.WriteLine();
            cenosredUrls.Sort();
        }

        private void RemoveFalsePositives(Dictionary<string, int> censoringIPs)
        {
            TextReader inputips = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("falsepositivs.txt"));
            List<string> falsepos = new List<string>();
            List<string> toremove = new List<string>();
            string ip;

            while ((ip = inputips.ReadLine()) != null)
            {
                falsepos.Add(ip);
            }

            foreach (string cip in censoringIPs.Keys)
            {
                if (falsepos.Contains(cip))
                {
                    toremove.Add(cip);
                }
            }

            foreach (string cip in toremove)
            {
                censoringIPs.Remove(cip);
            }
        }

        public void PrintReport(TextWriter sw)
        {
            if (!reportReady)
            {
                return;
            }

            sw.WriteLine();
            sw.WriteLine();
            sw.WriteLine("Automatic Censorship Report");
            sw.WriteLine("---------------------------");
            sw.WriteLine("ISP       : " + this.provider);
            sw.WriteLine("Country   : " + this.country);
            sw.WriteLine("Date      : " + this.date.ToShortDateString());
            sw.WriteLine("Reporter  : " + this.reporter);
            int ipidx = 0;
            foreach (IPAddress dnsip in dnsServers)
            {
                ipidx++;
                // Reverse DNS
                //Console.WriteLine("DNS#" + ipidx + "     : "+ dnsip + " (" + DNSHelper.ReverseDNS(dnsip.ToString()) + ")");
                sw.WriteLine("DNS#" + ipidx + "     : " + dnsip);
            }
            sw.WriteLine("Censored  : " + ((isCensoring) ? "Yes" : "No"));
            if (isCensoring)
            {
                sw.WriteLine("Censor IP : " + censorRedirect);
                sw.WriteLine("---------------------------");

                cenosredUrls.Sort();
                foreach (string url in cenosredUrls)
                {
                    sw.WriteLine("Censoring : " + url);
                }

                sw.WriteLine("---------------------------");
                sw.WriteLine("We found " + cenosredUrls.Count + " Domains which are banned on your ISP. ");
                sw.WriteLine("Please Post your result on apophis.ch.");
            }

            sw.WriteLine("-----------------");
            sw.WriteLine("apophis.ch - 2009");
        }

        private void PrintStatusLine(string line)
        {
            Console.CursorLeft = 4;
            Console.CursorTop = 4;
            Console.Write(line);
        }

    }

}
