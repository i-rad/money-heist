using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoneyHeist.Models.ViewModels
{
    public class HeistMemberViewModel
    {
        public string Name { get; set; }
        public List<MemberSkillViewModel> Skills { get; set; }
    }
}
