using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.DirectoryServices;
using System.DirectoryServices.Protocols;
using System.Net;

namespace HsgEdsLdap
{
    public class Eds
    {
        public string Server { get; set; }
        public int Port { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string UserPath { get; set; }

        public EdsEntryCollection Search(string SearchPath, string Query)
        {
            // Creating the server object
            LdapDirectoryIdentifier serverIdentifier = new LdapDirectoryIdentifier(Server, Port);
            
            // Creating the authentication object
            NetworkCredential serverCredential = new NetworkCredential($"uid={UserName},{UserPath}", Password);

            // Creating the actual LDAP connection (we're not connecting yet)
            LdapConnection ldapConnection = new LdapConnection(serverIdentifier)
            {
                AuthType = AuthType.Basic,
                Credential = serverCredential
            };

            // Creating the LDAP search request
            var scope = System.DirectoryServices.Protocols.SearchScope.OneLevel;
            var searchRequest = new SearchRequest(SearchPath, Query, scope);

            var result = (SearchResponse)ldapConnection.SendRequest(searchRequest); 

            return new EdsEntryCollection(result.Entries);
        }


    }
}
