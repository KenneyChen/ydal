using NetCore.Dal.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NetCore_Dal.Models
{
    [Table("demo")]
    public class Demo : BaseEntity
    {
        [Key]
        public int Id { get; set; }

        public string UserName { get; set; }
    }
}
