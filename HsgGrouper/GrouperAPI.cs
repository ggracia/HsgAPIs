using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Newtonsoft.Json;

namespace HsgGrouper
{
    public class GrouperAPI
    {
        public string HostUrl { get; set; }
        public string User { get; set; }
        public string Password { get; set; }

        public string BasePath { get; set; }
        private WebClient Client { get; set; }
        public string LastServerResponse { get; }
        public string SentXml { get; }
        public int RetriesIfFailure { get; set; }
        public int WaitMillisecondsBeetweenRetries { get; set; }

        /// <summary>
        /// Generic constructor
        /// </summary>
        public GrouperAPI(string user, string pwd) {
            SetHeaders(user, pwd);
            RetriesIfFailure = 10;
            WaitMillisecondsBeetweenRetries = 1000;
        }

        /// <summary>
        /// Helps communicating with Grouper
        /// </summary>
        /// <param name="hostUrl">The base url to the API</param>
        /// <param name="basePath">The select portion of the URL after the base URL</param>
        /// <param name="user">The API User</param>
        /// <param name="pwd">The Password</param>
        public GrouperAPI(string hostUrl, string basePath, string user, string pwd, int retriesIfFailure = 10, int waitMilliSecondsBetweenRetries = 1000)
        {
            HostUrl = hostUrl;
            BasePath = basePath;

            User = user;
            Password = pwd;

            RetriesIfFailure = retriesIfFailure;
            WaitMillisecondsBeetweenRetries = waitMilliSecondsBetweenRetries;

            SetHeaders(user, pwd);

        }

        /// <summary>
        /// Sets the headers of the Net Client object
        /// </summary>
        /// <param name="user">The username to authorize the request</param>
        /// <param name="pwd">The password or token to authorize the request</param>
        /// <param name="AcceptHader">The accept header, the default value is application/json</param>
        /// <param name="KeepAlive">if the request needs a keepAlive header, the default value is false</param>
        public void SetHeaders(string user, string pwd, string AcceptHader = "application/json", bool KeepAlive = false)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            var auth = Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes(String.Format("{0}:{1}", user, pwd)));

            Client = new WebClient();
            Client.Headers[HttpRequestHeader.ContentType] = "text/xml";
            Client.Headers.Add("Authorization", string.Format("Basic {0}", auth));
            Client.Headers.Add(HttpRequestHeader.Accept, AcceptHader);
            Client.Headers.Add(HttpRequestHeader.KeepAlive, KeepAlive ? "true" : "false");
        }

        /// <summary>
        /// Adds a member to a group.
        /// </summary>
        /// <param name="fullStemAndGroup">The full Stem and Group in Grouper (e.g. arizona.edu:dept:HSG:technologyservices)</param>
        /// <param name="Id"></param>
        /// <returns>An AddMemberResponse object</returns>
        public AddMemberResponse AddMember(string fullStemAndGroup, string Id = null)
        {
            var response = SendRequest(fullStemAndGroup, "PUT", Id);
            return JsonConvert.DeserializeObject<AddMemberResponse>(response);
        }


        /// <summary>
        /// Deletes a member from a group.
        /// </summary>
        /// <param name="fullStemAndGroup">The full Stem and Group in Grouper (e.g. arizona.edu:dept:HSG:technologyservices)</param>
        /// <param name="Id">The ID of the person to delete</param>
        /// <returns>A DeleteMemberResponse object</returns>
        public DeleteMemberResponse DeleteMember(string fullStemAndGroup , string Id = null)
        {
            var response = SendRequest(fullStemAndGroup, "DELETE", Id);

            return JsonConvert.DeserializeObject<DeleteMemberResponse>(response);
        }

        private string SendRequest(string fullStemAndGroup, string HttpMethod, string memberID = "", string data = "")
        {
            var result = string.Empty;

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
                    var m = string.IsNullOrEmpty(memberID) ? "" : memberID;

                    var url = $"{HostUrl}/{BasePath}/{fullStemAndGroup}/members/{m}";

                    //To avoid double or triple slashes
                    url = url.Replace("///", "/").Replace("//", "/").Replace("https:/", "https://").Replace("http:/", "http://");


                    CheckAcceptHeader();
                    result = HttpMethod == "GET" ? Client.DownloadString(url) : Client.UploadString(url, HttpMethod, data);
                    resultOK = true;
                }
                catch (WebException e)
                {
                    resultOK = false;
                    var eResponse = (HttpWebResponse)e.Response;

                    if (eResponse.StatusCode ==  HttpStatusCode.NotFound)
                    {
                        GetMembersResponse r = new GetMembersResponse() {
                             WsGetMembersLiteResult = new WsGetMembersLiteResult() {
                                  ResultMetadata = new ResultMetadata() {
                                       ResultCode = "NotFound",
                                       ResultMessage = "Not Found",
                                       Success = "SUCCESS"
                                  }
                             }
                        };

                        var exceptionResult = JsonConvert.SerializeObject(r);
                        return exceptionResult;
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
        /// Gets a Grouper response with the members of the given group
        /// </summary>
        /// <param name="stem">The stem where the group is at. e.g. arizona.edu:dept:HSG</param>
        /// <param name="groupName">The name of the group as it shows in the info of the group in the grouper ui. e.g. technologyservices</param>
        /// <returns>A grouper response containing the status of the Api call and the members if any.</returns>
        public GetMembersResponse GetGroupMembers(string stem, string groupName)
        {
            return GetGroupMembers($"{stem}:{groupName}");
        }

        /// <summary>
        /// Gets a Grouper response with the members of the given group
        /// </summary>
        /// <param name="fullStemAndGroup">The stem and group. e.g. arizona.edu:dept:HSG:technologyservices</param>
        /// <returns>A grouper response containing the status of the Api call and the members if any.</returns>
        public GetMembersResponse GetGroupMembers(string fullStemAndGroup)
        {
            var response =  SendRequest(fullStemAndGroup, "GET");
            return JsonConvert.DeserializeObject<GetMembersResponse>(response);
        }

        /// <summary>
        /// Checks if the Accept header is present in the request, if not, it adds it to the request.
        /// </summary>
        private void CheckAcceptHeader()
        {
            if (Client.Headers[HttpRequestHeader.Accept] == null)
                Client.Headers.Add(HttpRequestHeader.Accept, "application/json");
        }
    }
}
