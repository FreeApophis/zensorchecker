/*
 * Erstellt mit SharpDevelop.
 * Benutzer: apophis
 * Datum: 03.05.2009
 * Zeit: 18:08
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */
using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.IO;
using System.Net;

namespace apophis.ZensorChecker
{
    /// <summary>
    /// Description of FindDNSServer.
    /// </summary>
    public class DNSHelper
    {
        private static IPAddress opendns1 = IPAddress.Parse("208.67.222.222");
        
        public static IPAddress OpenDNS1 {
            get {
                return opendns1;
            }
        }

        private static IPAddress opendns2 = IPAddress.Parse("208.67.220.220");

        public static IPAddress OpenDNS2 {
            get {
                return opendns2;
            }
        }
        
        public static IEnumerable<IPAddress> GetLocalDNS() {
            switch(Environment.OSVersion.Platform) {
                case PlatformID.Unix:
                    return GetLocalDNSUnix();
                default:
                    return GetLocalDNSdotNet2();
            }
        }
        
        private static IEnumerable<IPAddress> GetLocalDNSdotNet2() {
            List<IPAddress> dnsservers = new List<IPAddress>();
            
            foreach(NetworkInterface adapter in NetworkInterface.GetAllNetworkInterfaces()) {
                if ((adapter.OperationalStatus != OperationalStatus.Up) || (adapter.NetworkInterfaceType == NetworkInterfaceType.Loopback)) {
                    continue;
                }
                IPInterfaceProperties properties = adapter.GetIPProperties();
                foreach(IPAddress serverip in properties.DnsAddresses) {
                    dnsservers.Add(serverip);
                }
            }
            return dnsservers;
        }
        
        private static IEnumerable<IPAddress> GetLocalDNSUnix() {
            List<IPAddress> dnsservers = new List<IPAddress>();
            string line;
            TextReader resolve = File.OpenText("/etc/resolv.conf");
            while((line = resolve.ReadLine()) != null) {
                if(line.StartsWith("nameserver")) {
                    dnsservers.Add(IPAddress.Parse(line.Substring(11)));
                }
            }
            return dnsservers;
        }

        
        
        public static string ReverseDNS(string ip) {
            IPHostEntry IpEntry = Dns.GetHostEntry(ip);
            return IpEntry.HostName.ToString();
        }
        
    }
}
