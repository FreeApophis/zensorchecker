/**
 * Project: Zensorchecker: A dns checker to identify potentiel zensoring from you 
 * File name: WebSpider.cs
 * Description: Generic Webspider, with customizable DNS Server
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
 c * You should have received a copy of the GNU General Public License along 
 * with this program; if not, write to the Free Software Foundation, Inc., 
 * 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
 **/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Heijden.DNS;
using System.IO;
using System.Net.Sockets;
using System.Net;
using ZensorChecker;
using System.Text.RegularExpressions;

namespace apophis.ZensorChecker
{
    class WebSpider
    {
        public Resolver DNSResolver { get; private set; }
        public IEnumerable<Uri> RootSet { get; private set; }
        public string UserAgent { get; set; }
        private const string ProtocolString = "HTTP/1.1";

        public delegate void HandleLine(string line, Uri referer);

        public WebSpider(IEnumerable<Uri> rootSet, Resolver dnsResolver)
        {
            DNSResolver = dnsResolver;
            RootSet = rootSet;
            UserAgent = "Mozilla/5.0 (compatible; zensorchecker/" + this.GetType().Assembly.GetName().Version.ToString() + ";  https://github.com/FreeApophis/zensorchecker/)";
        }

        public HttpHeader GetHeader(Uri uri, Uri referrer = null)
        {
            return Request(uri, referrer, "HEAD", null);
        }

        public HttpHeader GetDocument(Uri uri, Uri referrer = null, HandleLine lineHandler = null)
        {
            return Request(uri, referrer, "GET", lineHandler);
        }

        public void PrintUrl(string line, Uri referer)
        {
            MatchCollection links = Regex.Matches(line, @"(<a.*?>.*?</a>)", RegexOptions.Singleline);

            foreach (Match link in links)
            {
                string value = link.Groups[1].Value;

                Match href = Regex.Match(value, @"href=\""(.*?)\""", RegexOptions.Singleline);
                if (href.Success)
                {

                    Uri temp;
                    Uri.TryCreate(href.Groups[1].Value, UriKind.RelativeOrAbsolute, out temp);
                    if (temp.IsAbsoluteUri)
                    {
                        Console.WriteLine(temp.AbsoluteUri);
                    }
                    else
                    {                        
                        Uri.TryCreate(referer, temp, out temp);
                        Console.WriteLine(temp.AbsoluteUri);
                    }

                }
            }
        }


        private HttpHeader Request(Uri uri, Uri referrer, string method, HandleLine lineHandler)
        {
            if (uri == null)
                return null;

            TcpClient client = new TcpClient();
            IPAddress ip = DNSHelper.ResolveUri(DNSResolver, uri).First();

            try
            {
                client.Connect(ip, uri.Port);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }

            //Send Request
            TextWriter tw = new StreamWriter(client.GetStream());
            tw.WriteLine(method + " " + uri.AbsolutePath + " " + ProtocolString);
            tw.WriteLine("Host: " + uri.Host);
            tw.WriteLine("User-Agent: " + UserAgent);
            tw.WriteLine("Connection: close");
            if (referrer != null)
            {
                tw.WriteLine("Referer: " + referrer.AbsoluteUri);
            }
            tw.WriteLine();
            tw.Flush();


            TextReader document = new StreamReader(client.GetStream());
            var header = ParseHeader(document);

            if (lineHandler != null)
            {
                string line;
                try
                {
                    while ((line = document.ReadLine()) != null)
                    {
                        lineHandler.Invoke(line, uri);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            client.Close();
            return header;
        }

        private HttpHeader ParseHeader(TextReader document)
        {
            string status = document.ReadLine();

            string line;
            var headers = new Dictionary<string, string>();

            while ((line = document.ReadLine()) != "")
            {
                if (line == null) { throw new Exception("Unexpected End of Request"); }
                var head = line.Split(new char[] { ':' }, 2);

                if (head.Length == 2)
                {
                    if (headers.ContainsKey(head[0]))
                    {
                        headers[head[0]] = headers[head[0]] + ", " + head[1];
                    }
                    else
                    {
                        headers.Add(head[0], head[1].Trim());
                    }
                }
                else
                {
                    throw new Exception("Bad Header:" + line);
                }
            }

            return new HttpHeader(status, headers);
        }
    }
}
