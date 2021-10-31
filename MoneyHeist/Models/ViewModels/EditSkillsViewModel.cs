using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoneyHeist.Models.ViewModels
{
    public class EditSkillsViewModel
    {
        public string MainSkill { get; set; }
        public List<SkillViewModel> Skills { get; set; }
    }
}
