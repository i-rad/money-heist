using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MoneyHeist.Models
{
    public class MemberSkill
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MemberSkillId { get; set; }

        [Range(1, 10)]
        public int SkillLevel { get; set; }
        public int MemberId { get; set; }
        public bool IsMain { get; set; }

        [ForeignKey("Skill")]
        public int SkillId { get; set; }
        public Skill Skill { get; set; }


    }
}
