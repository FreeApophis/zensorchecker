/*
 * Erstellt mit SharpDevelop.
 * Benutzer: apophis
 * Datum: 30.04.2009
 * Zeit: 21:25
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading;

using apophis.Tools;
using Bdev.Net.Dns;

#if GUI
using System.Windows.Forms;
#endif

namespace apophis.ZensorChecker
{

    
    class Program
    {
        public static void Main(string[] args)
        {
            
            Arguments argsParsed = new Arguments(args);
            
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
                while((url = urls.ReadLine()) != null) {
                    Console.WriteLine(url);
                }
                return;
            }
            
            if ((argsParsed["n"] == null) && (argsParsed["-noauto"] == null)) {
                CountryISP autodetect = new CountryISP();
                country = autodetect.Country;
                provider = autodetect.Isp;
            }
            
            if ((argsParsed["c"] != null) || (argsParsed["-country"] != null))
            {
                if (argsParsed["c"] != null) {
                    country = (string)argsParsed["c"][0];
                } else {
                    country = (string)argsParsed["-country"][0];
                }
            }
            
            if ((argsParsed["p"] != null) || (argsParsed["-provider"] != null))
            {
                if (argsParsed["p"] != null) {
                    provider = (string)argsParsed["p"][0];
                } else {
                    provider = (string)argsParsed["-provider"][0];
                }
            }

            if ((argsParsed["r"] != null) || (argsParsed["-reporter"] != null))
            {
                if (argsParsed["r"] != null) {
                    reporter = (string)argsParsed["r"][0];
                } else {
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
                if (argsParsed["d"] != null) {
                    foreach(string ip in argsParsed["d"]) {
                        dnshint.Add(IPAddress.Parse(ip));
                    }
                } else {
                    foreach(string ip in argsParsed["-dnshint"]) {
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
            TextWriter tw = new StreamWriter("report-" + DateTime.Now.Year + "-"+ DateTime.Now.Month + "-"+ DateTime.Now.Day + "-"+ DateTime.Now.Hour + "-"+ DateTime.Now.Minute + ".txt", false);
            cr.PrintReport(tw);
            tw.Close();
            
            return;
        }
        
        
    }
}