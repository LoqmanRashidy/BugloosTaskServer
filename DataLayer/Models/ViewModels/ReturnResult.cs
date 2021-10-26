using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Datalayer.ViewModels
{
   public class ReturnResult
    {
        public dynamic data { get; set; }
        public long? count { get; set; }
        //public long? maxcode { get; set; }
    }
}
