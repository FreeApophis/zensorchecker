/*
 * Erstellt mit SharpDevelop.
 * Benutzer: apophis
 * Datum: 30.04.2009
 * Zeit: 21:25
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */
using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading;

using Bdev.Net.Dns;
using apophis.Tools;

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

            if ((argsParsed["l"] != null) || (argsParsed["-list"] != null))
            {
                string url;
                TextReader urls = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("baselist.txt"));
                while((url = urls.ReadLine()) != null) {
                    Console.WriteLine(url);
                }
                return;
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
            
            //);
            
            
            cr.GetCensoringIP(); // hint censor IP to speed up
            cr.RunCheck();
            
            
            cr.PrintReport(Console.Out);
            cr.PrintReport(new StreamWriter("report.txt", false));
            
            Thread.Sleep(20000);
            return;
        }
        
        
    }
}