using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;


namespace HsgStarRezAPI
{
    [Serializable, XmlRoot]
    public class Entry
    {
        [XmlAttribute]
        public int EntryID { get; set; }
        [XmlIgnore]
        public int CategoryID { get; set; }
        [XmlIgnore]
        public int EventID { get; set; }
        [XmlIgnore]
        public int GroupID { get; set; }
        [XmlIgnore]
        public int ContactID { get; set; }
        [XmlIgnore]
        public string EntryStatusEnum { get; set; }
        [XmlIgnore]
        public int AddressTypeID { get; set; }
        [XmlIgnore]
        public int BookingID { get; set; }
        [XmlIgnore]
        public int EntryApplicationID { get; set; }
        [XmlIgnore]
        public int PinNumber { get; set; }
        [XmlIgnore]
        public string Password { get; set; }
        [XmlIgnore]
        public string PortalEmail { get; set; }
        [XmlIgnore]
        public string PortalAuthProviderUserID { get; set; }
        [XmlIgnore]
        public string NameLast { get; set; }
        [XmlIgnore]
        public string NameFirst { get; set; }
        [XmlIgnore]
        public string NameTitle { get; set; }
        [XmlIgnore]
        public string NamePreferred { get; set; }
        [XmlIgnore]
        public string NameWeb { get; set; }
        [XmlIgnore]
        public string NameOther { get; set; }
        [XmlIgnore]
        public string NameInitials { get; set; }
        [XmlIgnore]
        public string NameSharer { get; set; }
        [XmlIgnore]
        public string GenderEnum { get; set; }
        [XmlIgnore]
        public string Birth_GenderEnum { get; set; }
        [XmlIgnore]
        public bool DirectoryFlagPrivacy { get; set; }
        [XmlIgnore]
        public string DOB { get; set; }
        [XmlIgnore]
        public string Position { get; set; }
        [XmlIgnore]
        public string ID1 { get; set; }
        [XmlIgnore]
        public string ID2 { get; set; }
        //Yes
        public string ID3 { get; set; }
        [XmlIgnore]
        public int ID4 { get; set; }
        [XmlIgnore]
        public int ID5 { get; set; }
        [XmlIgnore]
        public bool PhoneProcessToAccount { get; set; }
        [XmlIgnore]
        public int PhoneChargeTypeID { get; set; }
        [XmlIgnore]
        public int PhoneDisableValue { get; set; }
        [XmlIgnore]
        public int PhoneRestrictValue { get; set; }
        [XmlIgnore]
        public string PhoneControlEnum { get; set; }
        [XmlIgnore]
        public string TaxExemptionEnum { get; set; }
        [XmlIgnore]
        public string LastCheckInOutDate { get; set; }
        [XmlIgnore]
        public string Previous_EntryStatusEnum { get; set; }
        [XmlIgnore]
        public bool Testing { get; set; }
        [XmlIgnore]
        public int User_SecurityUserID { get; set; }
        [XmlIgnore]
        public int SecurityUserID { get; set; }
        [XmlIgnore]
        public int CreatedBy_SecurityUserID { get; set; }
        [XmlIgnore]
        public string DateModified { get; set; }
        [XmlIgnore]
        public string DateCreated { get; set; }
        public EntryDetail EntryDetail { get; set; }
        [XmlArray("EntryCustomFields")]
        public List<EntryCustomField> EntryCustomField { get; set; }
        [XmlArray("EntryAddresses")]
        public List<EntryAddress> EntryAddress { get; set; }


        /// <summary>
        /// Gets the XML representation of the entry object to be sent to StarRez
        /// </summary>
        /// <returns>A string with the xml representation of the Entry object</returns>
        public string GetXML()
        {
            //Getting the XML to send to the API
            var xsSubmit = new XmlSerializer(this.GetType());
            var xml = string.Empty;
            using (var sww = new StringWriter())
            {
                using (var w = XmlWriter.Create(sww))
                {
                    xsSubmit.Serialize(w, this);
                    xml = sww.ToString();
                }
            }

            //removing array parent because StarRez needs the array elements to be in all in at the Entry Level
            xml = xml.Replace("</EntryCustomFields>", "");
            xml = xml.Replace("<EntryCustomFields>", "");
            xml = xml.Replace("</EntryAddresses>", "");
            xml = xml.Replace("<EntryAddresses>", "");

            return xml;
        }

    }
}
