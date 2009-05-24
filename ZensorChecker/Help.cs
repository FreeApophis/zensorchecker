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
            System.Console.WriteLine("      --censorhint       If you know the redirect IP, hint it");
            System.Console.WriteLine("  -d  --dnshint          Check a certain DNS Server for Censorship");
            System.Console.WriteLine("  -h  --help             This help");
            System.Console.WriteLine("  -l  --list             List all urls in the baselist");
            System.Console.WriteLine("  -n  --noauto           Don't detect ISP/Country automatically");
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
        
        public static void printVersionInfo()
        {
            System.Console.WriteLine();
            System.Console.WriteLine(System.Reflection.Assembly.GetEntryAssembly().FullName);
            System.Console.WriteLine();
            System.Console.WriteLine("This is free software.  You may redistribute copies of it under the terms of the GNU General Public License <http://www.gnu.org/licenses/gpl.html>.There is NO WARRANTY, to the extent permitted by law.");
            System.Console.WriteLine();
            System.Console.WriteLine("Written by Thomas Bruderer");
            return;
        }
        
    }
}
