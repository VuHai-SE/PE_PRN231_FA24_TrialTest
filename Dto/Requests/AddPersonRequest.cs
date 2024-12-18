﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dto.Requests
{
    public class AddPersonRequest
    {
        public int PersonId { get; set; }
        public string FullName { get; set; }
        public DateTime BirthDay { get; set; }
        public string Phone { get; set; }
        public List<VirusInfo> Viruses { get; set; }
    }
}
