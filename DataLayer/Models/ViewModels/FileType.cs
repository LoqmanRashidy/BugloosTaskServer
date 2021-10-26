using System;
using System.Collections.Generic;
using System.Text;

namespace DataLayer.ViewModels
{
    public class FileType
    {
        public string Name { get; set; }

        public int Type { get; set; }
        public int Order { get; set; }
        
        public bool IsRead { get; set; } = false;

    }

    public class ListFileType
    {
        public List<FileType> TypeList { get; set; }


    }
}
