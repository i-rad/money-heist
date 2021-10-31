using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MoneyHeist.Models
{
    public class HeistMember
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int HeistMemberId { get; set; }

        [ForeignKey("Heist")]
        public int HeistId { get; set; }

        [ForeignKey("Member")]
        public int MemberId { get; set; }

        public Heist Heist { get; set; }
        public Member Member { get; set; }
    }
}
