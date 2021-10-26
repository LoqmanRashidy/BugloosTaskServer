using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataLayer.ViewModels
{
   public class RawQuery
    {
        //public string raw { get; set; }
        public string rawfilteronly { get; set; }
        public string rawfilterpaging { get; set; }
        public string sortlause { get; set; }
        //public RawSqlString maxcode { get; set; }
    }
}
