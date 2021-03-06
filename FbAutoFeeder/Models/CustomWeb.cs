﻿using log4net;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FbAutoFeeder.Models
{
    public class CustomWeb : System.Net.WebClient
    {
        private string userAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/53.0.2785.143 Safari/537.36";
        public CookieContainer CookieContainer { get; set; }
        private static readonly ILog logger = LogManager.GetLogger(typeof(CustomWeb));

        public CustomWeb(CookieContainer container)
        {
            CookieContainer = container;

        }

        public CustomWeb()
            : this(new CookieContainer())
        { }

        public CustomWeb(string userAgent)
           : this(new CookieContainer())
        {
            this.userAgent = userAgent;
            Console.WriteLine(userAgent);
        }

        protected override WebRequest GetWebRequest(Uri address)
        {
            var request = (HttpWebRequest)base.GetWebRequest(address);
            request.CookieContainer = CookieContainer;
            request.KeepAlive = false;
            return request;
        }

        public string SendRequest(string url, string method, string host, NameValueCollection nvc, ref string redirectUrl, bool isGetRedirectUrl, bool isCreateCookie = false, string contentType = "")
        {
            string result = string.Empty;
            try
            {
                CookieContainer container = new CookieContainer();
                string data = ConvertNVCToString(nvc);

                string finalUrl = (data != "") ? url + "?" + data : url;
                if (method.ToLower().Equals("post"))
                    finalUrl = url;

                var request = (HttpWebRequest)WebRequest.Create(finalUrl);
                request.Method = method;
                request.Host = host;
                if(isGetRedirectUrl)
                {
                    request.AllowAutoRedirect = false;
                }
                //request.AllowAutoRedirect = true;
                //request.Accept = "*/*";
                request.UserAgent = userAgent;
                request.ProtocolVersion = HttpVersion.Version10; // fix 1
                request.Timeout = 1000000000; // fix 3
                request.ReadWriteTimeout = 1000000000; // fix 4
                                                       // request.Timeout = 1000 * 60 * 5;

                if (!isCreateCookie)
                {
                    request.CookieContainer = CookieContainer;
                }
                else
                {
                    container = request.CookieContainer = new CookieContainer();
                }


                if (contentType != "")
                {
                    request.ContentType = contentType;
                }
                if (method.ToLower().Equals("post"))
                {
                    var byteData = Encoding.ASCII.GetBytes(data);
                    request.ContentLength = byteData.Length;
                    using (var stream = request.GetRequestStream())
                        stream.Write(byteData, 0, byteData.Length);
                }


                //using (var res = request.GetResponse())
                //{
                //    using (var sr = new StreamReader(res.GetResponseStream()))
                //    {
                //        result = sr.ReadToEnd();
                //    }
                //}

                using (var webResponse = (HttpWebResponse)request.GetResponse())
                {
                    // Now look to see if it's a redirect
                    if(isGetRedirectUrl)
                    {
                        if ((int)webResponse.StatusCode >= 300 && (int)webResponse.StatusCode <= 399)
                        {
                            string uriString = webResponse.Headers["Location"];
                            Console.WriteLine("Redirect to " + uriString ?? "NULL");
                            redirectUrl = uriString;
                        }
                    }
                  

                    using (var sr = new StreamReader(webResponse.GetResponseStream()))
                    {
                        result = sr.ReadToEnd();
                    }
                }

                if (isCreateCookie)
                {
                    CookieContainer = container;
                }

            }
            catch (Exception ex)
            {
                // System.Windows.Forms.MessageBox.Show(ex.Message);
                Console.WriteLine("error: " + ex.StackTrace);
                logger.DebugFormat("Error Message: {0}\nStacktrace: {1}", ex.Message, ex.StackTrace);
            }

            return result;
        }
        public string SendRequest(string url, string method, NameValueCollection nvc, bool isCreateCookie = false, string contentType = "")
        {
            string result = string.Empty;
            try
            {
                CookieContainer container = new CookieContainer();
                string data = ConvertNVCToString(nvc);

                string finalUrl = (data != "") ? url + "?" + data : url;
                if (method.ToLower().Equals("post"))
                    finalUrl = url;

                var request = (HttpWebRequest)WebRequest.Create(finalUrl);
                request.Method = method;
                //request.AllowAutoRedirect = true;
                //request.Accept = "*/*";
                request.UserAgent = userAgent;
                request.ProtocolVersion = HttpVersion.Version10; // fix 1
                request.Timeout = 1000000000; // fix 3
                request.ReadWriteTimeout = 1000000000; // fix 4
                                                       // request.Timeout = 1000 * 60 * 5;

                if (!isCreateCookie)
                {
                    request.CookieContainer = CookieContainer;
                }
                else
                {
                    container = request.CookieContainer = new CookieContainer();
                }


                if (contentType != "")
                {
                    request.ContentType = contentType;
                }
                if (method.ToLower().Equals("post"))
                {
                    var byteData = Encoding.ASCII.GetBytes(data);
                    request.ContentLength = byteData.Length;
                    using (var stream = request.GetRequestStream())
                        stream.Write(byteData, 0, byteData.Length);
                }


                using (var res = request.GetResponse())
                {
                    using (var sr = new StreamReader(res.GetResponseStream()))
                    {
                        result = sr.ReadToEnd();
                    }
                }

                if (isCreateCookie)
                {
                    CookieContainer = container;
                }

            }
            catch (Exception ex)
            {
                // System.Windows.Forms.MessageBox.Show(ex.Message);
                Console.WriteLine("error: " + ex.StackTrace);
                logger.DebugFormat("Error Message: {0}\nStacktrace: {1}", ex.Message, ex.StackTrace);
            }

            return result;
        }

        public string WebClientRequest(string url, NameValueCollection nvc, string method)
        {

            var responseData = this.UploadValues(url, method, nvc);
            WebHeaderCollection whc = this.ResponseHeaders;
            string result = string.Empty;

            using (var mr = new MemoryStream(responseData))
            {
                using (var sr = new StreamReader(mr))
                {
                    result = sr.ReadToEnd();
                }
            }
            return result;
        }

        public string HttpUploadFile(string url, string file, string paramName, string contentType, NameValueCollection nvc)
        {
            Console.WriteLine(string.Format("Uploading {0} to {1}", file, url));
            string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
            byte[] boundarybytes = System.Text.Encoding.ASCII.GetBytes("--" + boundary + "\r\n");


            HttpWebRequest wr = (HttpWebRequest)WebRequest.Create(url);
            wr.ServicePoint.Expect100Continue = false;
            wr.ContentType = "multipart/form-data; boundary=" + boundary;
            wr.Method = "POST";
            wr.KeepAlive = false;
            wr.Host = "manager.sunfrogshirts.com";
            wr.ProtocolVersion = HttpVersion.Version10; // fix 1
            wr.Timeout = 1000000000; // fix 3
            wr.ReadWriteTimeout = 1000000000; // fix 4
            //  wr.Referer = url;
            //wr.Headers[HttpRequestHeader.AcceptEncoding] = "gzip, deflate";
            //   wr.Headers[HttpRequestHeader.KeepAlive] = "true";
            //wr.Headers[HttpRequestHeader.CacheControl] = "max-age=0";
            //wr.AllowAutoRedirect = true;
            //wr.Accept = "*/*";
            // wr.Credentials = System.Net.CredentialCache.DefaultCredentials;
            //  wr.CachePolicy = new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.NoCacheNoStore); 


            wr.CookieContainer = CookieContainer;
            wr.UserAgent = userAgent;

            Stream rs = wr.GetRequestStream();
            rs.Flush();
            string formdataTemplate = "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}\r\n";
            foreach (string key in nvc.Keys)
            {
                rs.Write(boundarybytes, 0, boundarybytes.Length);
                string formitem = string.Format(formdataTemplate, key, nvc[key]);
                byte[] formitembytes = System.Text.Encoding.UTF8.GetBytes(formitem);
                rs.Write(formitembytes, 0, formitembytes.Length);
            }
            rs.Write(boundarybytes, 0, boundarybytes.Length);


            if (!string.IsNullOrEmpty(file))
            {
                string headerTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n";
                string header = string.Format(headerTemplate, paramName, Path.GetFileName(file), contentType);
                byte[] headerbytes = System.Text.Encoding.UTF8.GetBytes(header);
                rs.Write(headerbytes, 0, headerbytes.Length);

                FileStream fileStream = new FileStream(file, FileMode.Open, FileAccess.Read);
                byte[] buffer = new byte[4096];
                int bytesRead = 0;
                while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    rs.Write(buffer, 0, bytesRead);
                }
                fileStream.Close();
            }


            byte[] trailer = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
            rs.Write(trailer, 0, trailer.Length);
            rs.Close();

            WebResponse wresp = null;
            try
            {
                wresp = wr.GetResponse();
                Stream stream2 = wresp.GetResponseStream();
                StreamReader reader2 = new StreamReader(stream2);
                return reader2.ReadToEnd();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error uploading file", ex);
                if (wresp != null)
                {
                    wresp.Close();
                    wresp = null;
                }
            }
            finally
            {
                wr = null;
            }

            return null;
        }


        public string HttpUploadFileByJson(string url, string jsonModel)
        {
            Console.WriteLine(string.Format("Uploading {0}", url));

            HttpWebRequest wr = (HttpWebRequest)WebRequest.Create(url);
            // wr.ServicePoint.Expect100Continue = false;
            //  wr.ProtocolVersion = HttpVersion.Version11;
            wr.ContentType = "application/json; charset=utf-8";
            wr.Method = "POST";
            wr.Host = "manager.sunfrogshirts.com";
            //   wr.AllowAutoRedirect = false;
            //wr.Headers.Add("X-Requested-With", "XMLHttpRequest");
            //wr.Headers[HttpRequestHeader.KeepAlive] = "true";
            //wr.AllowAutoRedirect = true;
            wr.CookieContainer = CookieContainer;
            wr.UserAgent = userAgent;
            wr.Timeout = 1000 * 30;
            // wr.Timeout = Timeout.Infinite;
            //wr.KeepAlive = true;
            wr.ProtocolVersion = HttpVersion.Version10; // fix 1
                                                        //wr.Timeout = 1000000000; // fix 3
                                                        // wr.ReadWriteTimeout = 1000000000; // fix 4
            wr.Referer = "https://manager.sunfrogshirts.com/Designer/";
            //wr.Referer = "https://manager.sunfrogshirts.com/my-art-edit.cfm?editNewDesign";

            using (var streamWriter = new StreamWriter(wr.GetRequestStream()))
            {
                streamWriter.Write(jsonModel);
                streamWriter.Flush();
            }


            WebResponse wresp = null;
            //int tryCount = 0;
            //do
            //{
            //    try
            //    {
            //        Console.WriteLine("try count: "+(++tryCount));
            //        wresp = wr.GetResponse();
            //        Stream stream2 = wresp.GetResponseStream();
            //        StreamReader reader2 = new StreamReader(stream2);
            //        string result = reader2.ReadToEnd();
            //        return result;
            //    }
            //    catch (Exception ex)
            //    {
            //        Console.WriteLine("Error uploading file: ", ex.Message);
            //        if (wresp != null)
            //        {
            //            wresp.Close();
            //            wresp = null;
            //        }
            //    }
            //} while (true);
            try
            {
                wresp = wr.GetResponse();
                Stream stream2 = wresp.GetResponseStream();
                StreamReader reader2 = new StreamReader(stream2);
                string result = reader2.ReadToEnd();
                return result;
            }
            catch (Exception ex)
            {
                logger.DebugFormat("Error Message: {0}\nStacktrace: {1}", ex.Message, ex.StackTrace);
                Console.WriteLine("Error uploading file: " + ex.Message + ", stacktrace: " + ex.StackTrace);
                if (wresp != null)
                {
                    wresp.Close();
                    wresp = null;
                }
            }
            finally
            {
                wr = null;
            }

            return null;
        }

        public async Task<string> UpdateMockupByJson(string url, string jsonModel)
        {
            Console.WriteLine(string.Format("Updating {0}", url));

            HttpWebRequest wr = (HttpWebRequest)WebRequest.Create(url);
            wr.ServicePoint.Expect100Continue = false;
            wr.ProtocolVersion = HttpVersion.Version11;
            wr.ContentType = "application/json; charset=utf-8";
            wr.Method = "POST";
            wr.Host = "manager.sunfrogshirts.com";
            //wr.AllowAutoRedirect = false;
            //wr.Headers.Add("X-Requested-With", "XMLHttpRequest");
            //wr.Headers[HttpRequestHeader.KeepAlive] = "true";
            //wr.AllowAutoRedirect = true;
            wr.CookieContainer = CookieContainer;
            wr.UserAgent = userAgent;
            //wr.Timeout = 1000 * 60;
            //wr.Timeout = Timeout.Infinite;
            //wr.KeepAlive = true;
            //wr.ProtocolVersion = HttpVersion.Version10; // fix 1
            //wr.Timeout = 1000000000; // fix 3
            //wr.ReadWriteTimeout = 1000000000; // fix 4
            //wr.Referer = "https://manager.sunfrogshirts.com/Designer/";
            wr.Referer = "https://manager.sunfrogshirts.com/my-art-edit.cfm?editNewDesign";

            using (var streamWriter = new StreamWriter(wr.GetRequestStream()))
            {
                streamWriter.Write(jsonModel);
                streamWriter.Flush();
            }

            WebResponse wresp = null;
            try
            {
                using (var response = (HttpWebResponse)await wr.GetResponseAsync())
                {
                    Stream stream2 = wresp.GetResponseStream();
                    StreamReader reader2 = new StreamReader(stream2);
                    string result = reader2.ReadToEnd();
                    return result;
                }
                //wresp = wr.GetResponse();
                //Stream stream2 = wresp.GetResponseStream();
                //StreamReader reader2 = new StreamReader(stream2);
                //string result = reader2.ReadToEnd();
                //return result;
            }
            catch (Exception ex)
            {
                logger.DebugFormat("Error Message: {0}\nStacktrace: {1}", ex.Message, ex.StackTrace);
                Console.WriteLine("Error uploading file: " + ex.Message + ", stacktrace: " + ex.StackTrace);
                if (wresp != null)
                {
                    wresp.Close();
                    wresp = null;
                }
            }
            finally
            {
                wr = null;
            }

            return null;
        }

        public string ConvertNVCToString(NameValueCollection nvc)
        {
            if (nvc == null)
                return "";
            StringBuilder sb = new StringBuilder();
            sb.Append("");
            foreach (var item in nvc)
            {
                sb.AppendFormat("{0}={1}&", item.ToString(), nvc.Get(item.ToString()));
            }


            string result = sb.ToString();
            if (result == null || result == "")
                return "";
            result = result.Remove(result.LastIndexOf('&'), 1);
            return result;
        }

    }
}
