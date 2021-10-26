using CommonLayer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Datalayer.Models
{
    [Table(nameof(Setting), Schema = DbSchemas.BaseSystem)]
    public class Setting : BaseEntity
    {
        [Key]
        public string Key { get; set; }
        public string Value { get; set; }
        public string FaTitle { get; set; }
        public DateTime DateChanged { get; set; } = DateTime.Now;
        private string _PersianDateChanged { get; set; }
        [NotMapped]
        public string PersianDateChanged { get; set; }
        //public bool IsDeleted { get; set; } = false;
    }
}
