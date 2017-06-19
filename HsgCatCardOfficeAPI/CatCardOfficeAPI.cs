using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Newtonsoft.Json;
using System.Linq;

namespace HsgCatCardOfficeAPI
{
    public class CatCardOfficeAPI
    {
        public string BaseURL { get; set; }
        public string Token { get; set; }
        public string IsoChangesURL { get; set; }
        public WebClient Client{ get; set; }


        public CatCardOfficeAPI(string baseURL, string isoChangesURL, string token)
        {
            BaseURL = baseURL;
            IsoChangesURL = isoChangesURL;
            Token = token;

            Client = new WebClient();
            Client.Headers[HttpRequestHeader.ContentType] = "text/xml";
            Client.Headers.Add(HttpRequestHeader.Accept, "application/json");
            Client.Headers.Add(HttpRequestHeader.KeepAlive, "false");
        }

        /// <summary>
        /// Gets an Iso Change from Cat Card Office
        /// </summary>
        /// <param name="request">The Iso Change request object</param>
        /// <returns>A Iso Change Object containing the ISO changes from the request forward</returns>
        public List<IsoChange> GetIsoChanges(IsoChangeRequest request)
        {
            var isoChange = new IsoChange();
            var obj = JsonConvert.SerializeObject(request);
            var response = Client.UploadString(string.Format("{0}{1}",BaseURL, IsoChangesURL), "PUT", obj);
            return JsonConvert.DeserializeObject<List<IsoChange>>(response);
        }

    }
}
