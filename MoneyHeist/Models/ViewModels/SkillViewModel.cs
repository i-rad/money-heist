using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MoneyHeist.Models.ViewModels
{
    public class SkillViewModel
    {
        public string Name { get; set; }

        [StringLength(10)]
        public string Level { get; set; }

        public int Members { get; set; }
    }
}
