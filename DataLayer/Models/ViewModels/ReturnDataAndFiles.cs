
using System;
using System.Collections.Generic;
using System.Text;

namespace DataLayer.ViewModels
{
   public  class ReturnDataAndFiles<T1,T2> 
    {
        public T1 Data { get; set; }
        public List<T2> Files { get; set; }

    }
}
