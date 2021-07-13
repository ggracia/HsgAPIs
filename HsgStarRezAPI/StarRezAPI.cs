using System;
using System.Threading;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

using Newtonsoft.Json;
using System.Linq;

namespace HsgStarRezAPI
{
    public class StarRezAPI
    {
        public string BaseURL { get; set; }
        public string User { get; set; }
        public string Token { get; set; }
        public string CreateURL { get; set; }
        public string UpdateURL { get; set; }
        public string SelectURL { get; set; }
        public WebClient Client { get; set; }
        public string LastServerResponse { get; set; }
        public string SentXml { get; set; }
        public int RetriesIfFailure { get; set; }
        public int WaitMillisecondsBeetweenRetries { get; set; }

        /// <summary>
        /// Helps send and receive information with StarRez
        /// </summary>
        /// <param name="baseURL">The base url to the API</param>
        /// <param name="createURL">The create portion of the URL after the base URL</param>
        /// <param name="updateURL">The update portion of the URL after the base URL</param>
        /// <param name="selectURL">The select portion of the URL after the base URL</param>
        /// <param name="user">The API User</param>
        /// <param name="token">The API Token</param>
        public StarRezAPI(string baseURL, string createURL, string updateURL, string selectURL, string user, string token, int retriesIfFailure, int waitMilliSecondsBetweenRetries)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            BaseURL = baseURL;
            CreateURL = createURL; //CREATE API URL
            UpdateURL = updateURL; //UPDATE API URL
            SelectURL = selectURL; //SELECT API URL

            CheckUpdateURL();

            User = user;
            Token = token;

            RetriesIfFailure = retriesIfFailure;
            WaitMillisecondsBeetweenRetries = waitMilliSecondsBetweenRetries;

            var auth = Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes(String.Format("{0}:{1}", User, Token)));

            Client = new WebClient();
            Client.Headers[HttpRequestHeader.ContentType] = "text/xml";
            Client.Headers.Add("Authorization", string.Format("Basic {0}", auth));
            Client.Headers.Add(HttpRequestHeader.Accept, "application/json");
            Client.Headers.Add(HttpRequestHeader.KeepAlive, "false");

        }

        /// <summary>
        /// Gets an Entry from StarRez
        /// </summary>
        /// <param name="EntryID">The Entry ID to get the entry for</param>
        /// <param name="relatedTables">if there are any related tables to pull for the object, here is the place to specify them (comma separated)</param>
        /// <returns>The entry object pulled from StarRez</returns>
        public Entry GetEntry(int EntryID, params string[] relatedTables)
        {
            var rt = string.Empty;
            if (relatedTables.Length > 0)
            {
                foreach (var t in relatedTables)
                {
                    rt = string.Format("{0}, {1}", rt, t);
                }
                rt.Remove(0, 2);
                rt = string.Format("&_relatedtables={0}", rt);
            }
            return JsonConvert.DeserializeObject<List<Entry>>(Client.DownloadString(string.Format("{0}{1}/{2}{3}", BaseURL, SelectURL, EntryID, rt))).FirstOrDefault();
        }

        /// <summary>
        /// Gets an entry from StarRez
        /// </summary>
        /// <param name="StudentID">The Student ID to get the entry for</param>
        /// <param name="relatedTables">if there are any related tables to pull for the object, here is the place to specify them (comma separated)</param>
        /// <returns>The entry object pulled from StarRez</returns>
        public StarRezResponse GetEntry(string StudentID, params string[] relatedTables)
        {
            var rt = string.Empty;
            StarRezResponse result = new StarRezResponse();

            if (relatedTables.Length > 1)
            {
                foreach (var t in relatedTables)
                {
                    rt = string.Format($"{rt}, {t}");
                }
                rt.Remove(0, 2);
                rt = string.Format($"&_relatedtables={rt}");
            }

            for (int i = 0; i <= RetriesIfFailure; i++)
            {
                //If it is retrying we sleep the thread
                if (i > 0)
                {
                    Thread.Sleep(WaitMillisecondsBeetweenRetries);
                }
                var ID1 = StudentID.Trim();

                var resultOK = false;
                try
                {
                    var response = Client.DownloadString(string.Format($"{BaseURL}{SelectURL}/?ID1={ID1}{rt}"));
                    result.Entry = (JsonConvert.DeserializeObject<List<Entry>>(response)).FirstOrDefault();
                    if (result.Entry.EntryID > 0)
                    {
                        result.StatusCode = "Sucess";
                        result.StatusDescription = "Sucess";
                        resultOK = true;
                    }
                }
                catch (WebException e)
                {
                    resultOK = false;
                    var eResponse = (HttpWebResponse)e.Response;
                    result.StatusCode = eResponse.StatusCode.ToString();
                    result.StatusDescription = eResponse.StatusDescription.ToString();

                    if (eResponse.StatusCode.ToString() == "NotFound")
                    {
                        break;
                    }

                    //if it's the last retry we throw the error
                    if (i == RetriesIfFailure)
                    {
                        throw;
                    }
                }

                if (resultOK)
                {
                    break; //If the request is successful we won't retry. We exit the "for" loop.
                }
            }

            return result;
        }

        /// <summary>
        /// Updates an Entry on StarRez
        /// </summary>
        /// <param name="e">The Entry object containing the parameters to be updated on StarRez</param>
        /// <returns>The updated Entry object pulled from StarRez</returns>
        public Entry UpdateEntry(Entry e)
        {

            LastServerResponse = string.Empty;
            SentXml = string.Empty;
            if (e.EntryID <= 0) throw new Exception("The EntryID cannot be 0 when updating an entry");
            var xml = e.GetXML();
            SentXml = xml;
            CheckAcceptHeader();
            Entry r = null;

            for (int i = 0; i <= RetriesIfFailure; i++)
            {
                //If it is retrying we sleep the thread
                if (i > 0)
                {
                    Thread.Sleep(WaitMillisecondsBeetweenRetries);
                }

                var resultOK = false;
                try
                {
                    var response = Client.UploadString(string.Format("{0}{1}{2}", BaseURL, UpdateURL, e.EntryID), "PUT", xml);
                    LastServerResponse = response;
                    r = JsonConvert.DeserializeObject<Entry>(response);
                    if (r.EntryID > 0)
                    {
                        resultOK = true;
                    }
                }
                catch
                {
                    resultOK = false;
                    //if it's the last retry we throw the error
                    if (i == RetriesIfFailure)
                    {
                        throw;
                    }
                }

                if (resultOK)
                {
                    break; //If the request is successful we won't retry. We exit the "for" loop.
                }
            }

            return r;
        }


        private void CheckUpdateURL()
        {
            if (UpdateURL.Length == 0) return;
            UpdateURL = UpdateURL.Substring(UpdateURL.Length - 1, 1) == "/" ? UpdateURL : UpdateURL + "/";
        }

        /// <summary>
        /// Creates an entry on StarRez
        /// </summary>
        /// <param name="e">The Entry object containing the parameters to be created on StarRez</param>
        /// <returns>An Entry object pulled from StarRez containing the generated EntryID</returns>
        public Entry CreateEntry(Entry e)
        {
            LastServerResponse = string.Empty;
            SentXml = string.Empty;
            var xml = SentXml = e.GetXML();
            CheckAcceptHeader();
            var response = LastServerResponse = Client.UploadString(string.Format("{0}{1}", BaseURL, CreateURL), "PUT", xml);
            //var response = LastServerResponse = "<Entry EntryID=1></Entry>";
            return JsonConvert.DeserializeObject<Entry>(response);
        }

        /// <summary>
        /// Checks if the Accept header is present in the request, if not, it adds it to the request.
        /// </summary>
        private void CheckAcceptHeader()
        {
            var h = Client.Headers[HttpRequestHeader.Accept] ?? null;
            if (h == null)
                Client.Headers.Add(HttpRequestHeader.Accept, "application/json");
        }


    }
}
