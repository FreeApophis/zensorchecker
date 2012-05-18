using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mime;

namespace apophis.ZensorChecker
{
    class HttpHeader
    {
        public string Protocol { get; private set; }
        public int StatusCode { get; private set; }
        public string StatusMessage { get; private set; }
        public Dictionary<string, string> AllHeaders { get; private set; }

        public int ContentLength
        {
            get
            {
                var size = GetHeaderByKey("Content-Length");

                if (size == null)
                {
                    return -1;
                }
                else
                {
                    return int.Parse(size);
                }
            }
        }

        public ContentDisposition ContentDisposition
        {
            get
            {
                var contentDisposition = GetHeaderByKey("Content-Disposition");
                if (contentDisposition == null)
                {
                    return new ContentDisposition();
                }
                else
                {
                    return new ContentDisposition(contentDisposition);
                }
            }
        }



        public ContentType ContentType
        {
            get
            {
                var contentType = GetHeaderByKey("Content-Type");
                if (contentType == null)
                {
                    return new ContentType();
                }
                else
                {
                    return new ContentType(contentType);
                }
            }
        }

        public string Server
        {
            get
            {
                return GetHeaderByKey("Server");
            }
        }

        public DateTime Date
        {
            get
            {
                var date = GetHeaderByKey("Date");
                if (date == null)
                {
                    return DateTime.Now;
                }
                else
                {
                    return DateTime.Parse(date);
                }
            }
        }

        private string GetHeaderByKey(string key)
        {
            string value;
            if (AllHeaders.TryGetValue(key, out value))
            {
                return value;
            }
            else
            {
                return null;
            }
        }


        public HttpHeader(string status, Dictionary<string, string> headers)
        {
            var statusArray = status.Split(new char[] { ' ' }, 3);

            if (statusArray.Length == 3)
            {
                Protocol = statusArray[0];
                StatusCode = int.Parse(statusArray[1]);
                StatusMessage = statusArray[2];
            }
            else
            {
                throw new Exception("Bad Status:" + status);
            }

            AllHeaders = headers;

        }
    }
}
