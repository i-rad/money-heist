using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MoneyHeist.Models.ViewModels
{
    public class MemberViewModel
    {
        public int MemberId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public char Sex { get; set; }

        [Required]
        public string Email { get; set; }
        public string MainSkill { get; set; }

        [Required]
        public string Status { get; set; }
        public List<SkillViewModel> Skills { get; set; }
    }
}
