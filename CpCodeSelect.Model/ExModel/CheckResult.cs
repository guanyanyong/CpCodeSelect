using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CpCodeSelect.Model.ExModel
{
    public class CheckResult
    {
        public bool Result { get; set; } 
        public string Message { get; set; }
        public double KValue { get; set; }
        public Bolling Bolling { get; set; }
    }
}
