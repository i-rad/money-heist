using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MoneyHeist.Models
{
    public class Heist
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int HeistId { get; set; }

        public string Name { get; set; }

        public string Location { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        [ForeignKey("HeistStatus")]
        public int StatusId { get; set; }
        public HeistStatus HeistStatus { get; set; }


        public ICollection<HeistSkill> HeistSkills { get; set; }
    }
}
