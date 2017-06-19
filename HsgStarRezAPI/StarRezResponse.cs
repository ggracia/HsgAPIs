using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HsgStarRezAPI
{
    public class StarRezResponse
    {
        public string StatusCode { get; set; }
        public string StatusDescription { get; set; }
        public Entry Entry { get; set; }
    }
}
