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
