using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoneyHeist.Models
{
    public class MoneyHeistContext : DbContext
    {
        public MoneyHeistContext(DbContextOptions options)
            : base(options)
        {
        }
        public DbSet<Member> Members { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<MemberSkill> MemberSkills { get; set; }
        public DbSet<MemberStatus> MemberStatuses { get; set; }
        public DbSet<Heist> Heists { get; set; }
        public DbSet<HeistSkill> HeistSkills { get; set; }
        public DbSet<HeistStatus> HeistStatuses { get; set; }
        public DbSet<HeistMember> HeistMembers { get; set; }
        public DbSet<HeistOutcome> HeistOutcome { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MemberStatus>().HasData(
                new MemberStatus { StatusId = 1, Name = "AVAILABLE" },
                new MemberStatus { StatusId = 2, Name = "EXPIRED" },
                new MemberStatus { StatusId = 3, Name = "INCARCERATED" },
                new MemberStatus { StatusId = 4, Name = "RETIRED" }
            );

            modelBuilder.Entity<HeistStatus>().HasData(
                new HeistStatus { StatusId = 1, Name = "PLANNING" },
                new HeistStatus { StatusId = 2, Name = "READY" },
                new HeistStatus { StatusId = 3, Name = "IN_PROGRESS" },
                new HeistStatus { StatusId = 4, Name = "FINISHED" }
            );
        }
    }
}
