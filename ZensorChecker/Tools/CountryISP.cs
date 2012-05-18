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
using System.IO;
using System.Net;
using System.Text.RegularExpressions;

namespace apophis.ZensorChecker
{
    /// <summary>
    /// Description of CountryISP.
    /// </summary>
    public class CountryISP
    {
        public IPAddress ExternalIP { get; private set; }
        public string Country { get; private set; }
        public string Isp { get; private set; }
        public string Region { get; private set; }
        public string City { get; private set; }
        public string Timezone { get; private set; }
        public string Networkspeed { get; private set; }

        private Regex strongMatch = new Regex(@"(?<=<strong>)[^<]*(?=</strong>)");
        private Regex boldMatch = new Regex(@"(?<=<b>)[^<]*(?=</b>)");

        public CountryISP()
        {
            WebClient web = new WebClient();

            // Webbrowser
            web.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");

            StreamReader reader = new StreamReader(web.OpenRead("http://www.ip2location.com/ib2/"));
            string line;
            int index = 0;
            while ((line = reader.ReadLine()) != null)
            {
                MatchCollection mc = boldMatch.Matches(line);
                if (mc.Count == 0)
                {
                    mc = strongMatch.Matches(line);
                }
                if (mc.Count == 1)
                {
                    switch (index)
                    {
                        case 0:
                            ExternalIP = IPAddress.Parse(mc[0].Value);
                            break;
                        case 1:
                            Isp = mc[0].Value;
                            break;
                        case 2:
                            Country = mc[0].Value;
                            break;
                        case 3:
                            Region = mc[0].Value;
                            break;
                        case 4:
                            City = mc[0].Value;
                            break;
                        case 5:
                            Timezone = mc[0].Value;
                            break;
                        case 6:
                            Networkspeed = mc[0].Value;
                            break;
                    }
                    index++;
                }
                if (mc.Count > 6)
                {
                    ExternalIP = IPAddress.Parse(mc[0].Value);
                    Isp = mc[1].Value;
                    Country = mc[2].Value;
                    Region = mc[3].Value;
                    City = mc[4].Value;
                    Timezone = mc[5].Value;
                    Networkspeed = mc[6].Value;
                }
            }
        }
    }
}
