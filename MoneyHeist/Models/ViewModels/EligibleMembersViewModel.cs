using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoneyHeist.Models.ViewModels
{
    public class EligibleMembersViewModel
    {
        public List<SkillViewModel> Skills { get; set; }
        public List<MemberViewModel> Members { get; set; }
    }
}
