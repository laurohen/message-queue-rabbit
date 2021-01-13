using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace messageqServer.Models
{
    public class CreateEventRequest
    {
        public int Id { get; set; }
        public string Pais { get; set; }
        public string Regiao { get; set; }
        public string Sensor { get; set; }
        public string Valor { get; set; }
        public string Status { get; set; }
        public string Timestamp { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
    }
}
