using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DbToExcel.MVC.Models
{
    //Dosya oluşturuluyor mu ne durumda
    public enum FileStatus
    {
        Creating,
        Completed
    }

    //Kimin dosyasını nereye kaydettik nasıl kaydettik vb...
    public class UserFile
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public FileStatus FileStatus { get; set; }
        public DateTime? CreatedDate { get; set; }

        //Db ye bu kolonu ekleme
        //Created date varsa onu dön yok ise "-" dön 
        [NotMapped]
        public string GetCreatedDate => CreatedDate.HasValue ? CreatedDate.Value.ToShortDateString() : "-";
    }
}
