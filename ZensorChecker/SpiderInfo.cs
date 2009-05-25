/*
 *  <project description>
 * 
 *  Copyright (c) 2008-2009 Thomas Bruderer <apophis@apophis.ch> 
 *  File created by ${USER} at ${DATE} ${TIME}
 *
 *  This program is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with this program.  If not, see <http://www.gnu.org/licenses/>. 
 */
 
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;

using Bdev.Net.Dns;

namespace apophis.ZensorChecker
{
	public class SpiderInfo {
	    
	    private string url;
	    
        public string URL {
            get { 
	            return url; 
	        }
        }
	    
	    private bool successfullyRetrived;
	    
        public bool SuccessfullyRetrived {
            get { 
	            return successfullyRetrived; 
	        }
            set { 
	            successfullyRetrived = value; 
	        }
        }
	    
	    private bool readError; 
	    
        public bool ReadError {
            get { 
	            return readError; 
	        }
            set { 
	            readError = value; 
	        }
        }
	    
	    private bool censored;
	    
        public bool Censored {
            get { 
	            return censored; 
	        }
            set { 
	            censored = value; 
	        }
        }
	    
	    private int depth;
	    
        public int Depth {
            get { 
	            return depth; 
	        }
        }
	    
	    
	    
	    public SpiderInfo(string url, int depth) {
	        this.url = url;
	        this.depth = depth;
	    }
	    
	}
}
