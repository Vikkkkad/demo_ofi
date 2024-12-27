using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace demo_ofi
{
    public class Task
    {
        public int TaskId { get; set; }
        public DateTime CreationDate { get; set; }
        public string ProjectName { get; set; }
        public string Description { get; set; }
        public string Priority { get; set; }
        public string AssignedTo { get; set; }
        public string Status { get; set; }
    }

}

