using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MoneyHeist.Models
{
    public class Member
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MemberId { get; set; }
        public string Name { get; set; }
        public char Sex { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        [ForeignKey("MemberStatus")]
        public int StatusId { get; set; }
        public MemberStatus MemberStatus { get; set; }

        public ICollection<MemberSkill> MemberSkills { get; set; }
    }
}
