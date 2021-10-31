using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MoneyHeist.Models
{
    public class HeistOutcome
    {
        public int HeistOutcomeId { get; set; }
        public int HeistId { get; set; }
        public string Outcome { get; set; }

    }
}
