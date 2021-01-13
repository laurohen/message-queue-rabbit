using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace messageqServer.Models
{
    public class QueReceive
    {
        public string Timestamp { get; set; }

        public string Tag { get; set; }

        public string Valor { get; set; }
    }
}
