using System;
using System.Xml.Serialization;

namespace HsgStarRezAPI
{
    [Serializable]
    public class EntryCustomField
    {
        [XmlIgnore]
        public int EntryCustomFieldID { get; set; }
        [XmlIgnore]
        public int EntryID { get; set; }
        [XmlAttribute]
        public int CustomFieldDefinitionID { get; set; }
        public string FieldDataTypeEnum { get; set; }
        public string ValueString { get; set; }
        public string ValueDate { get; set; }
        public bool ValueBoolean { get; set; }
        public int ValueInteger { get; set; }
        public double ValueMoney { get; set; }
    }
}
