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
using System.Net.NetworkInformation;
using System.IO;
using System.Net;
using System.Linq;

using Heijden.DNS;

namespace apophis.ZensorChecker
{
    /// <summary>
    /// Description of FindDNSServer.
    /// </summary>
    public class DNSHelper
    {
        public static IEnumerable<IPAddress> GetLocalDNS()
        {
            switch (Environment.OSVersion.Platform)
            {
                case PlatformID.Unix:
                    return GetLocalDNSUnix();
                default:
                    return GetLocalDNSdotNet2();
            }
        }

        private static IEnumerable<IPAddress> GetLocalDNSdotNet2()
        {
            List<IPAddress> dnsservers = new List<IPAddress>();

            foreach (NetworkInterface adapter in NetworkInterface.GetAllNetworkInterfaces())
            {
                if ((adapter.OperationalStatus != OperationalStatus.Up) || (adapter.NetworkInterfaceType == NetworkInterfaceType.Loopback))
                {
                    continue;
                }
                IPInterfaceProperties properties = adapter.GetIPProperties();
                foreach (IPAddress serverip in properties.DnsAddresses)
                {
                    dnsservers.Add(serverip);
                }
            }
            return dnsservers;
        }

        private static IEnumerable<IPAddress> GetLocalDNSUnix()
        {
            List<IPAddress> dnsservers = new List<IPAddress>();
            string line;
            TextReader resolve = File.OpenText("/etc/resolv.conf");
            while ((line = resolve.ReadLine()) != null)
            {
                if (line.StartsWith("nameserver"))
                {
                    dnsservers.Add(IPAddress.Parse(line.Substring(11)));
                }
            }
            return dnsservers;
        }

        public static IEnumerable<IPAddress> ResolveUri(Resolver resolver, Uri uri)
        {
            var response = resolver.Query(uri.Host, QType.A, QClass.IN);

            return response.Answers.Where(a => a.Type == Heijden.DNS.Type.A).Select(a => IPAddress.Parse(a.RECORD.ToString()));
        }

        public static string ReverseDNS(string ip)
        {
            IPHostEntry IpEntry = Dns.GetHostEntry(ip);
            return IpEntry.HostName.ToString();
        }

    }
}
