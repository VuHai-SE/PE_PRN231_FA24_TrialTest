using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dto.Responses
{
    public class GetPersonResponse
    {
        public int PersonId { get; set; }
        public string FullName { get; set; }
        public DateTime BirthDay { get; set; }
        public string Phone { get; set; }
        public List<VirusInfo> Viruses { get; set; }
    }

    public class VirusInfo
    {
        public string VirusName { get; set; }
        public float ResistanceRate { get; set; }
    }
}
