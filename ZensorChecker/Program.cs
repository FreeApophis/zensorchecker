/**
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

using apophis.Tools;
using Heijden.DNS;
using System.Linq;

#if GUI
using System.Windows.Forms;
#endif

namespace apophis.ZensorChecker
{


    class Program
    {
        public static void Main(string[] args)
        {

            WebSpider webSpider = new WebSpider(Enumerable.Empty<Uri>(), new Resolver(Resolver.DefaultDnsServers[0]));

            webSpider.GetDocument(new Uri("http://thomasbruderer.ch/nothinghere"));

            webSpider.GetDocument(new Uri("http://projects.piratenpartei.ch/"));
            webSpider.GetDocument(new Uri("http://forum.piratenpartei.ch/"));
            webSpider.GetDocument(new Uri("http://piratenpartei.ch/"));
            webSpider.GetDocument(new Uri("http://download.apophis.ch/"), null, webSpider.PrintUrl);
            webSpider.GetDocument(new Uri("http://download.apophis.ch/audio/FerienQuizXIII.ogg"));

            return;


            var argsParsed = new Arguments(args);

            string country = "none";
            string provider = "ISP";
            string reporter = "Anonymous";

            if ((argsParsed["h"] != null) || (argsParsed["-help"] != null))
            {
                Help.PrintGeneralHelp();
                return;
            }

            if (argsParsed["-version"] != null)
            {
                Help.printVersionInfo();
                return;
            }

            if ((argsParsed["l"] != null) || (argsParsed["-list"] != null))
            {
                string url;
                TextReader urls = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("baselist.txt"));
                while ((url = urls.ReadLine()) != null)
                {
                    Console.WriteLine(url);
                }
                return;
            }

            if ((argsParsed["n"] == null) && (argsParsed["-noauto"] == null))
            {
                CountryISP autodetect = new CountryISP();
                country = autodetect.Country;
                provider = autodetect.Isp;
            }

            if ((argsParsed["c"] != null) || (argsParsed["-country"] != null))
            {
                if (argsParsed["c"] != null)
                {
                    country = (string)argsParsed["c"][0];
                }
                else
                {
                    country = (string)argsParsed["-country"][0];
                }
            }

            if ((argsParsed["p"] != null) || (argsParsed["-provider"] != null))
            {
                if (argsParsed["p"] != null)
                {
                    provider = (string)argsParsed["p"][0];
                }
                else
                {
                    provider = (string)argsParsed["-provider"][0];
                }
            }

            if ((argsParsed["r"] != null) || (argsParsed["-reporter"] != null))
            {
                if (argsParsed["r"] != null)
                {
                    reporter = (string)argsParsed["r"][0];
                }
                else
                {
                    reporter = (string)argsParsed["-reporter"][0];
                }
            }

            CensorshipReport cr = new CensorshipReport(provider, country, reporter);

            if (argsParsed["-censorhint"] != null)
            {
                cr.CensorServerHint(IPAddress.Parse((string)argsParsed["-censorhint"][0]));
            }

            if ((argsParsed["d"] != null) || (argsParsed["-dnshint"] != null))
            {
                List<IPAddress> dnshint = new List<IPAddress>();
                if (argsParsed["d"] != null)
                {
                    foreach (string ip in argsParsed["d"])
                    {
                        dnshint.Add(IPAddress.Parse(ip));
                    }
                }
                else
                {
                    foreach (string ip in argsParsed["-dnshint"])
                    {
                        dnshint.Add(IPAddress.Parse(ip));
                    }
                }
                cr.DnsServerHint(dnshint);
            }

#if GUI
            bool gui = true;
            if (argsParsed["c"] != null || argsParsed["-console"] != null)
            {
                gui = false;
            }
            
            if(gui) {
                Application.Run(new MainForm());
            }
#endif

#if DEBUG
            StopSpider spider = new StopSpider(IPAddress.Parse("192.168.1.1"), IPAddress.Parse("212.142.48.154"), provider, country, reporter);
            spider.CrawlSpiderList();
            return;
#endif

            cr.GetCensoringIP(); // returns without test if a hint was given

            Console.Clear();
            cr.RunCheck();

            Console.Clear();

            cr.PrintReport(Console.Out);
            TextWriter tw = new StreamWriter("report-" + DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day + "-" + DateTime.Now.Hour + "-" + DateTime.Now.Minute + ".txt", false);
            cr.PrintReport(tw);
            tw.Close();

            return;
        }


    }
}