using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HsgGrouper
{
    public class WsAddMemberLiteResult
    {
        public ResponseMetadata ResponseMetadata { get; set; }
        public ResultMetadata ResultMetadata { get; set; }
        public WsGroupAssigned WsGroupAssigned { get; set; }
        public WsSubject WsSubject { get; set; }
    }
}
