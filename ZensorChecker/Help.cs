/*
 * Erstellt mit SharpDevelop.
 * Benutzer: apophis
 * Datum: 04.05.2009
 * Zeit: 08:43
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */
using System;

namespace apophis.ZensorChecker
{
    /// <summary>
    /// Description of Helpp.
    /// </summary>
    public class Help
    {
        public static void PrintGeneralHelp()
        {

            char[] sep = { System.IO.Path.DirectorySeparatorChar };
            string[] alp = Environment.GetCommandLineArgs()[0].Split(sep);
            string application = alp[alp.Length - 1];

            System.Console.WriteLine("Usage:");
            System.Console.WriteLine(application + "");
            System.Console.WriteLine();
            System.Console.WriteLine("Options:");
            System.Console.WriteLine("  -c  --country          Specify your country");
            System.Console.WriteLine("      --censorhint       Non-pure Server stats");
            System.Console.WriteLine("  -h  --help             This help");
            System.Console.WriteLine("  -l  --list             List all urls in the baselist");
            System.Console.WriteLine("  -p, --provider         Specify your provider ");
            System.Console.WriteLine("  -r, --reporter         Specify your name");
            System.Console.WriteLine("  -v, --verbose          Debug information ");
            System.Console.WriteLine("      --version          Version information");
            System.Console.WriteLine();
            System.Console.WriteLine("Examples:");
            System.Console.WriteLine(application + " -c Switzerland -p Cablecom --censorhint 212.142.48.154");
            System.Console.WriteLine("The working Cablecom example skipping the Censor IP Detection");
            System.Console.WriteLine();
            return;
        }
    }
}
