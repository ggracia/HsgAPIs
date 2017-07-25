using System;
using System.Collections.ObjectModel;
using System.DirectoryServices.Protocols;

namespace HsgEdsLdap
{
    /// <summary>
    /// A Collection of EdsEntry
    /// </summary>
    [Serializable]
    public class EdsEntryCollection : Collection<EdsEntry>
    {
        // Constructors
        #region Constructors

        /// <summary>
        /// Default Constructor
        /// </summary>
        public EdsEntryCollection() : base() { }

        public EdsEntryCollection(SearchResultEntryCollection entries)
        {
            foreach (SearchResultEntry entry in entries)
            {
                this.Add(new EdsEntry(entry));
            }
        }

        #endregion
    }
}
