using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace HsgStarRezAPI
{
    [Serializable]
    public class EntryDetail : IXmlSerializable
    {
        [XmlAnyAttribute]
        public string EntryDetailID { get; set; }
        [XmlIgnore]
        public int EntryID { get; set; }
        [XmlIgnore]
        public string PhotoPath { get; set; }
        [XmlIgnore]
        public int StaffID { get; set; }
        [XmlIgnore]
        public int ClassificationID { get; set; }
        [XmlIgnore]
        public string AttendeeStatusEnum { get; set; }
        [XmlIgnore]
        public int EventRegistrationFeeID { get; set; }
        [XmlIgnore]
        public int CountryOfBirth_CountryID { get; set; }
        [XmlIgnore]
        public int CountryOfResidence_CountryID { get; set; }
        [XmlIgnore]
        public int RegionOfBirthID { get; set; }
        [XmlIgnore]
        public int NationalityID { get; set; }
        [XmlElement]
        public int Citizenship_CountryID { get; set; }
        [XmlIgnore]
        public bool International { get; set; }
        [XmlIgnore]
        public string InternationalDetails { get; set; }
        [XmlIgnore]
        public bool Visa { get; set; }
        [XmlIgnore]
        public string VisaDetails { get; set; }
        [XmlIgnore]
        public string Religion { get; set; }
        [XmlElement]
        public string Ethnicity { get; set; }
        [XmlIgnore]
        public string Medical { get; set; }
        [XmlIgnore]
        public string Disability { get; set; }
        [XmlIgnore]
        public string Dietary { get; set; }
        [XmlIgnore]
        public string SpecialNeeds { get; set; }
        [XmlIgnore]
        public bool Married { get; set; }
        [XmlIgnore]
        public bool Deceased { get; set; }
        [XmlIgnore]
        public bool LivingWithDependents { get; set; }
        [XmlIgnore]
        public string DateEntry { get; set; }
        [XmlIgnore]
        public string DateExit { get; set; }
        [XmlIgnore]
        public int ResidentYear { get; set; }
        [XmlIgnore]
        public string ResidentStatus { get; set; }
        [XmlIgnore]
        public string Occupation { get; set; }
        [XmlIgnore]
        public string HearAboutUs { get; set; }
        [XmlIgnore]
        public string VehicleRegistration { get; set; }
        [XmlIgnore]
        public string VehicleDetails { get; set; }
        [XmlIgnore]
        public string VehiclePermit { get; set; }
        [XmlIgnore]
        public string PreviousMembership { get; set; }
        [XmlIgnore]
        public string PreviousMembershipYears { get; set; }
        [XmlIgnore]
        public string PreviousMemberName { get; set; }
        [XmlIgnore]
        public string PreviousMemberYears { get; set; }
        [XmlIgnore]
        public string PreviousMemberRelationship { get; set; }
        [XmlIgnore]
        public bool AccountHold { get; set; }
        [XmlIgnore]
        public string AccountCode { get; set; }
        [XmlIgnore]
        public string AccountComments { get; set; }
        [XmlIgnore]
        public string AccountDueDate { get; set; }
        [XmlIgnore]
        public int Account_PaymentTypeID { get; set; }
        [XmlIgnore]
        public string AccountBankName { get; set; }
        [XmlIgnore]
        public string AccountBankNumber { get; set; }
        [XmlIgnore]
        public string AccountDetail1 { get; set; }
        [XmlIgnore]
        public string AccountDetail2 { get; set; }
        [XmlIgnore]
        public string AccountDetail3 { get; set; }
        [XmlIgnore]
        public string AccountDetail4 { get; set; }
        [XmlIgnore]
        public int FinancialSupportID { get; set; }
        [XmlIgnore]
        public string FinancialComments { get; set; }
        [XmlIgnore]
        public string EnrollmentClass { get; set; }
        [XmlIgnore]
        public string EnrollmentTerm { get; set; }
        [XmlIgnore]
        public string EnrollmentLevel { get; set; }
        [XmlIgnore]
        public string EnrollmentStatus { get; set; }
        [XmlIgnore]
        public int EnrollmentYear { get; set; }
        [XmlIgnore]
        public string ProfileInterests { get; set; }
        [XmlIgnore]
        public string Career { get; set; }
        [XmlIgnore]
        public string CareerComments { get; set; }
        [XmlIgnore]
        public string EmploymentDetails { get; set; }
        [XmlIgnore]
        public bool IncidentHold { get; set; }
        [XmlIgnore]
        public string IncidentHoldComments { get; set; }
        [XmlIgnore]
        public string Comments { get; set; }
        [XmlIgnore]
        public string DeceasedDate { get; set; }
        [XmlIgnore]
        public bool VisitorHold { get; set; }
        [XmlIgnore]
        public bool UsesScreenReader { get; set; }


        public XmlSchema GetSchema() { return (null); }

        public void ReadXml(XmlReader reader)
        {
        }

        public void WriteXml(XmlWriter writer)
        {
            // Attributes
            if (EntryDetailID != null && EntryDetailID != "0") writer.WriteAttributeString("EntryDetailID", EntryDetailID);
            writer.WriteElementString("Ethnicity", Ethnicity);
            writer.WriteElementString("Citizenship_CountryID", Citizenship_CountryID.ToString());
        }

    }
}
