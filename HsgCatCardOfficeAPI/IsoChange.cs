using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HsgCatCardOfficeAPI
{
    public class IsoChange
    {
        /// <summary>
        /// The id of the change
        /// </summary>
        public int ChangeID { get; set; }
        /// <summary>
        /// The date and time of the change to this Cat Card
        /// </summary>
        public DateTime ChangeTimeStamp { get; set; }
        /// <summary>
        /// The Cat Card number
        /// </summary>
        public string ISO { get; set; }
        /// <summary>
        /// The EmplID/StudentID of the person
        /// </summary>
        public string UniversityID { get; set; }

    }
}
