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
        public string APIKey { get; set; }
        /// <summary>
        /// (Optional) The id of the previous change ID
        /// </summary>
        public int? ChangeId { get; set; }
        /// <summary>
        /// (Optional) The date in the format MM/dd/yyyy HH:mm:ss
        /// </summary>
        public string ChangeTimeStamp { get; set; }

        public bool ShouldSerializeChangeId()
        {
            return ChangeId.HasValue;
        }
    }
}
