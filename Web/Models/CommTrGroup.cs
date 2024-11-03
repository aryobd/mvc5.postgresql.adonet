using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Web.Models
{
    [Table("tr_group", Schema = "comm")]
    public class CommTrGroup
    {
        [Key]
        public int group_code { get; set; }
        public string group_desc { get; set; }
    }
}
