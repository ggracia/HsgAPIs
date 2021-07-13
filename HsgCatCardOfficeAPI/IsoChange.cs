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
        /// The date and time of the change to this Cat Card
        /// </summary>
        public DateTime ChangeDateTime { get; set; }

        /// <summary>
        /// The id of the change
        /// </summary>
        public int ChangeID { get; set; }
        /// <summary>
        /// Describes the type of change (Initial, Replacement)
        /// </summary>
        public string ChangeType { get; set; }
        /// <summary>
        /// The email of the person
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// The EmplID/StudentID of the person
        /// </summary>
        public string EmplID { get; set; }
        public string FirstName { get; set; }
        /// <summary>
        /// The Cat Card number
        /// </summary>
        public string IsoNumber { get; set; }
        public string LastName { get; set; }
        /// <summary>
        /// The Net ID of the person 
        /// </summary>
        public string NetID { get; set; }

    }
}
