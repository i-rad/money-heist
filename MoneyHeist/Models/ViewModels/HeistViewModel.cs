using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MoneyHeist.Models.ViewModels
{
    public class HeistViewModel
    {

        [Required]
        public string Name { get; set; }

        [Required]
        public string Location { get; set; }

        [Required]
        public string StartTime { get; set; }

        [Required]
        public string EndTime { get; set; }

        public List<SkillViewModel> Skills { get; set; }

        public string Status { get; set; }
    }
}
