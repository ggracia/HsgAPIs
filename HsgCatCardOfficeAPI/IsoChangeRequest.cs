using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HsgCatCardOfficeAPI
{
    public class IsoChangeRequest
    {
        /// <summary>
        /// The authorization key to access the API
        /// </summary>
        public string ApiKey { get; set; }

        /// <summary>
        /// The date and time to pull changes since.
        /// </summary>
        public string FromDateTime { get; set; }

        /// <summary>
        /// The date and time to pull changes to.
        /// </summary>
        public string ToDateTime { get; set; }

        

    }
}
