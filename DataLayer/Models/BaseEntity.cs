using CommonLayer.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Datalayer.Models
{
    public class BaseEntity
    {
        public long Id { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsDelete { get; set; } = false;
        public DateTime CreatedDate { get; set; } = PersianTools.LocalPersianDateTime(DateTime.Now);
     
        public long UserId { get; set; }
    }
}
