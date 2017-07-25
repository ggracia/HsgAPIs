using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HsgGrouper
{
    public class WsGetMembersLiteResult
    {
        public ResponseMetadata ResponseMetadata { get; set; }
        public ResultMetadata ResultMetadata { get; set; }
        public WsGroup WsGroup { get; set; }
        public List<WsSubject> WsSubjects { get; set; }
    }
}
