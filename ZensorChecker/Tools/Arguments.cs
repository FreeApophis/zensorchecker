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
using System.Collections;
using System.Collections.Specialized;
using System.Text.RegularExpressions;

namespace apophis.Tools
{
    /// <summary>
    /// CommandLine Argument Parser
    /// Author: raymond77 (codeproject)
    /// </summary>
    public class Arguments
    {
        // Variables
        private Hashtable parameters;

        public Hashtable Parameters
        {
            get
            {
                return parameters;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public Arguments(string[] Args)
        {
            parameters = new Hashtable();
            Regex splitter = new Regex(@"^-|^/|=|:", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            Regex remover = new Regex(@"^['""]?(.*?)['""]?$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

            string parameter = null;
            string[] parts;
            ArrayList values = new ArrayList();

            // Valid parameters:
            // {-,/}param{ ,=,:}((",')value(",'))
            // Examples:
            // -param1 value1 value2 -param2 /param3:"Test" /param4=happy
            foreach (string arg in Args)
            {
                parts = splitter.Split(arg, 3);

                switch (parts.Length)
                {
                    // Found value
                    case 1:
                        // if parameter still wait, add value to values-colection
                        if (parameter != null)
                        {
                            parts[0] = remover.Replace(parts[0], "$1");
                            values.Add(parts[0]);
                        }
                        break;

                    // found parameter
                    case 2:
                        // if paramater was still waiting,
                        // then add parameter and values-collection to _parameters
                        // clear values
                        if (parameter != null)
                        {
                            if (!parameters.ContainsKey(parameter))
                            {
                                parameters.Add(parameter, values);
                                values = new ArrayList();
                            }
                        }
                        parameter = parts[1];
                        break;

                    // found parameter with enclosed value
                    case 3:
                        // if paramater was still waiting,
                        // then add parameter and values-collection to _parameters
                        // clear values
                        if (parameter != null)
                        {
                            if (!parameters.ContainsKey(parameter))
                            {
                                parameters.Add(parameter, values);
                                values = new ArrayList();
                            }
                        }

                        // Parameter with enclosed value is allowed only one value.
                        // add parameter and value to _parameters
                        parameter = parts[1];
                        parts[2] = remover.Replace(parts[2], "$1");
                        values.Add(parts[2]);

                        if (!parameters.ContainsKey(parameter))
                            parameters.Add(parameter, values);

                        parameter = null;
                        values = new ArrayList();
                        break;
                }
            }
            // if  final parameter is still waiting,
            // add parameter and values to _parameters
            if (parameter != null)
            {
                if (!parameters.ContainsKey(parameter))
                    parameters.Add(parameter, values);
            }
        }

        // Retrieve a parameter value-collection if it exists
        // (overriding C# indexer property)
        public ArrayList this[string Param]
        {
            get
            {
                return (ArrayList)(parameters[Param]);
            }
        }
    }
}