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
using System.Net;

namespace ZensorChecker
{
    /// <summary>
    /// Description of UrlInfo.
    /// </summary>
    public class UrlInfo
    {
        private string url;

        public string URL
        {
            get
            {
                return url;
            }
        }

        public bool queryAgain;

        public bool QueryAgain
        {
            get
            {
                return queryAgain;
            }
        }

        private IPAddress ipFromLocalDns;

        public IPAddress IPFromLocalDns
        {
            get
            {
                return ipFromLocalDns;
            }
        }

        public UrlInfo(string url)
        {
            this.url = url;
            this.queryAgain = true;
        }

        public void SetIP(IPAddress ip)
        {
            this.ipFromLocalDns = ip;
            this.queryAgain = false;
        }
    }
}
