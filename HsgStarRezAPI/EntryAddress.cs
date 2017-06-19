using System;
using System.Xml.Serialization;

namespace HsgStarRezAPI
{
    [Serializable]
    public class EntryAddress
    {
        //[XmlIgnore]
        [XmlAttribute]
        public string EntryAddressID { get; set; }
        [XmlIgnore]
        public int EntryID { get; set; }
        [XmlAttribute]
        public int AddressTypeID { get; set; }
        public string Salutation { get; set; }
        public string ContactName { get; set; }
        public string Street { get; set; }
        public string Street2 { get; set; }
        public string City { get; set; }
        public int CountryID { get; set; }
        public string StateProvince { get; set; }
        public string ZipPostcode { get; set; }
        public string Phone { get; set; }
        public string PhoneMobileCell { get; set; }
        public string PhoneOther { get; set; }
        public string PhoneOther2 { get; set; }
        public string Email { get; set; }
        public string Relationship { get; set; }
        public string ActiveDateStart { get; set; }
        public string ActiveDateEnd { get; set; }
        public string Reference { get; set; }
        public string Comments { get; set; }
    }
}
