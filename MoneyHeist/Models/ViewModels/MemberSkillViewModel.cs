using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MoneyHeist.Models.ViewModels
{
    public class MemberSkillViewModel
    {
        public string Name { get; set; }

        [StringLength(10)]
        public string Level { get; set; }
    }
}
