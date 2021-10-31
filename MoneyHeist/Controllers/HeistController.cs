using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MoneyHeist.Models;
using System.Globalization;
using MoneyHeist.MailHelperClasses;

namespace MoneyHeist.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class HeistController : Controller
    {
        private readonly MoneyHeistContext _context;
        private readonly MailLogic _mailLogic;

        public HeistController(MoneyHeistContext context, Services.IMailService mailService)
        {
            _context = context;
            _mailLogic = new MailLogic(mailService);
        }

        [HttpGet]
        public List<Heist> Get()
        {
            var heists = _context.Heists.Include(p => p.HeistStatus).OrderBy(x => x.StartTime).ToList();
            return heists;
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var heist = _context.Heists.Include(p => p.HeistSkills).ThenInclude(p => p.Skill).Include(p => p.HeistStatus).FirstOrDefault(x => x.HeistId == id);
            if (heist == null)
            {
                return NotFound();
            }
            var heistViewModel = new Models.ViewModels.HeistViewModel
            {
                Name = heist.Name,
                Location = heist.Location,
                StartTime = heist.StartTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                EndTime = heist.EndTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                Skills = heist.HeistSkills.Select(x => new Models.ViewModels.SkillViewModel
                {
                    Name = x.Skill.Name,
                    Level = string.Concat(Enumerable.Repeat("*", x.SkillLevel)),
                    Members = x.HeistMembers
                }).ToList(),
                Status = heist.HeistStatus.Name
            };

            return Ok(heistViewModel);
        }

        [HttpGet("{id}/members")]
        public IActionResult GetMembers(int id)
        {
            var heist = _context.Heists.FirstOrDefault(x => x.HeistId == id);
            if (heist == null)
            {
                return NotFound();
            }
            if(heist.StatusId == 1)
            {
                return StatusCode(405);
            }

            var heistMembers = _context.HeistMembers.Where(x => x.HeistId == id).Select(x => x.MemberId);
            var heistMembersViewModel = _context.Members.Where(x => heistMembers.Contains(x.MemberId)).Include(p => p.MemberSkills).ThenInclude(p => p.Skill).Select(y => new Models.ViewModels.HeistMemberViewModel
            {
                Name = y.Name,
                Skills = y.MemberSkills.Select(x => new Models.ViewModels.MemberSkillViewModel
                {
                    Name = x.Skill.Name,
                    Level = string.Concat(Enumerable.Repeat("*", x.SkillLevel))
                }).ToList()
            }).ToList();

            return Ok(heistMembersViewModel);
        }

        [HttpGet("{id}/skills")]
        public IActionResult GetSkills(int id)
        {
            var heist = _context.Heists.Include(p => p.HeistSkills).ThenInclude(p => p.Skill).FirstOrDefault(x => x.HeistId == id);
            if (heist == null)
            {
                return NotFound();
            }
            var heistSkillsViewModel = heist.HeistSkills.Select(x => new Models.ViewModels.SkillViewModel
            {
                Name = x.Skill.Name,
                Level = string.Concat(Enumerable.Repeat("*", x.SkillLevel)),
                Members = x.HeistMembers
            }).ToList();

            return Ok(heistSkillsViewModel);
        }

        [HttpGet("{id}/status")]
        public IActionResult GetStatus(int id)
        {
            var heist = _context.Heists.Include(p => p.HeistStatus).FirstOrDefault(x => x.HeistId == id);
            if (heist == null)
            {
                return NotFound();
            }
            var heistStatusViewModel = new { status = heist.HeistStatus.Name };

            return Ok(heistStatusViewModel);
        }

        [HttpGet("{id}/outcome")]
        public IActionResult GetOutcome(int id)
        {
            var heist = _context.Heists.Include(p => p.HeistStatus).FirstOrDefault(x => x.HeistId == id);
            if (heist == null)
            {
                return NotFound();
            }
            if(heist.StatusId < 4)
            {
                return StatusCode(405);
            }

            var outcome = _context.HeistOutcome.FirstOrDefault(x => x.HeistId == heist.HeistId).Outcome;
            var heistOutcomeViewModel = new { outcome = outcome };

            return Ok(heistOutcomeViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Models.ViewModels.HeistViewModel heist)
        {
            // validation - check for duplicates in the skills list
            var duplicateExists = heist.Skills.GroupBy(x => new { x.Name, x.Level })
                   .Where(x => x.Skip(1).Any()).Any();
            // validation - check whether this name already exists
            var nameExists = _context.Heists.Any(x => x.Name == heist.Name);

            DateTime startDate = new DateTime(); //If Parsing succeed, it will store date in result variable.
            DateTime.TryParseExact(heist.StartTime, "yyyy-MM-ddTHH:mm:ss.fffZ", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out startDate);

            DateTime endDate = new DateTime();
            DateTime.TryParseExact(heist.EndTime, "yyyy-MM-ddTHH:mm:ss.fffZ", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out endDate);

            if (duplicateExists || nameExists || startDate == new DateTime() || endDate == new DateTime() || endDate < DateTime.Now || startDate > endDate)
            {
                return BadRequest();
            }

            var newHeist = new Heist
            {
                Name = heist.Name,
                Location = heist.Location,
                StartTime = startDate,
                EndTime = endDate,
                StatusId = 1
            };

            _context.Heists.Add(newHeist);
            // save to get memberId for next step
            await _context.SaveChangesAsync();

            foreach (var item in heist.Skills)
            {
                // if skills are not already in the database add them
                // ToUpper() - case insensitive
                if (!_context.Skills.Any(x => x.Name.ToUpper() == item.Name.ToUpper()))
                {
                    _context.Skills.Add(new Skill
                    {
                        Name = item.Name
                    });

                    await _context.SaveChangesAsync();
                }

                _context.HeistSkills.Add(new HeistSkill
                {
                    HeistId = newHeist.HeistId,
                    SkillId = _context.Skills.FirstOrDefault(x => x.Name == item.Name).SkillId,
                    SkillLevel = item.Level.Length,
                    HeistMembers = item.Members
                });
            }

            await _context.SaveChangesAsync();

            return Created("heist/" + newHeist.HeistId, null);

        }

        [HttpPatch("{id}/skills")]
        public async Task<IActionResult> Patch(int id, [FromBody] Models.ViewModels.EditHeistSkillsViewModel skills)
        {
            // validation - check for duplicates in the skills list
            var duplicateExists = skills.Skills.GroupBy(x => new { x.Name, x.Level })
                   .Where(x => x.Skip(1).Any()).Any();
            if (duplicateExists || skills.Skills.Count < 1)
            {
                return BadRequest();
            }
            // return 404 not found if member doesn't exist
            var heist = _context.Heists.FirstOrDefault(x => x.HeistId == id);
            if (heist == null)
            {
                return NotFound();
            }
            if (heist.StartTime < DateTime.Now)
            {
                return StatusCode(405);
            }
            foreach (var item in skills.Skills)
            {
                // if skills are not already in the database add them
                // ToUpper() - case insensitive
                if (!_context.Skills.Any(x => x.Name.ToUpper() == item.Name.ToUpper()))
                {
                    _context.Skills.Add(new Skill
                    {
                        Name = item.Name
                    });

                    await _context.SaveChangesAsync();
                }

                var skillId = _context.Skills.FirstOrDefault(x => x.Name.ToUpper() == item.Name.ToUpper()).SkillId;
                // check if this skill is already given to the member and if it is update it
                var prevSkill = _context.HeistSkills.FirstOrDefault(x => x.HeistId == id && x.SkillId == skillId);
                if (prevSkill != null)
                {
                    prevSkill.SkillLevel = item.Level.Length;
                    prevSkill.HeistMembers = item.Members;
                }
                else
                {
                    _context.HeistSkills.Add(new HeistSkill
                    {
                        HeistId = id,
                        SkillId = skillId,
                        SkillLevel = item.Level.Length,
                        HeistMembers = item.Members
                    });
                }

            }

            await _context.SaveChangesAsync();
            Response.Headers.Add("Content-Location", "heist/" + id + "/skills");
            return NoContent();

        }

        [HttpGet("{id}/eligible_members")]
        public IActionResult GetEligibleMembers(int id)
        {
            var heist = _context.Heists.Include(p => p.HeistSkills).ThenInclude(p => p.Skill).FirstOrDefault(x => x.HeistId == id);
            if (heist == null)
            {
                return NotFound();
            }
            if(heist.StatusId > 1)
            {
                return StatusCode(405);
            }
            var alreadyTaken = _context.HeistMembers.Where(x => heist.StartTime >= x.Heist.StartTime && heist.StartTime <= x.Heist.EndTime).Select(y => y.MemberId);
            var neededSkills = heist.HeistSkills;
            var membersWithSkills = new List<int>();
            foreach (var item in neededSkills)
            {
                membersWithSkills.AddRange(_context.MemberSkills.Where(x => x.SkillId == item.SkillId && x.SkillLevel >= item.SkillLevel).Select(y => y.SkillId));
            }
            var eligibleMembers = _context.Members.Include(p => p.MemberSkills).ThenInclude(p => p.Skill).Where(x => !alreadyTaken.Contains(x.MemberId) && (x.StatusId == 1 || x.StatusId == 4) && membersWithSkills.Contains(x.MemberId)).ToList();
            var jsonToReturn = new Models.ViewModels.EligibleMembersViewModel
            {
                Skills = heist.HeistSkills.Select(x => new Models.ViewModels.SkillViewModel
                {
                    Name = x.Skill.Name,
                    Level = string.Concat(Enumerable.Repeat("*", x.SkillLevel)),
                    Members = x.HeistMembers
                }).ToList(),
                Members = eligibleMembers.Select(x => new Models.ViewModels.MemberViewModel
                {
                    Name = x.Name,
                    Skills = x.MemberSkills.Select(y => new Models.ViewModels.SkillViewModel
                    {
                        Name = y.Skill.Name,
                        Level = string.Concat(Enumerable.Repeat("*", y.SkillLevel))
                    }).ToList()
                }).ToList()

            };
            return Ok(jsonToReturn);
        }

        [HttpPut("{id}/members")]
        public async Task<IActionResult> Put(int id, Models.ViewModels.ConfirmMembersViewModel members)
        {
            var heist = _context.Heists.Include(p => p.HeistSkills).FirstOrDefault(x => x.HeistId == id);
            if (heist == null)
            {
                return NotFound();
            }
            if (heist.StatusId > 1)
            {
                return StatusCode(405);
            }

            var alreadyTaken = _context.HeistMembers.Where(x => heist.StartTime >= x.Heist.StartTime && heist.StartTime <= x.Heist.EndTime).Select(y => y.MemberId);
            var allMembers = _context.Members.Include(p => p.MemberSkills).Where(x => members.Members.Contains(x.Name));
            var taken = alreadyTaken.Intersect(allMembers.Select(x => x.MemberId)).Any();
            var skillsNeeded = heist.HeistSkills.Select(x => x.SkillId);
            var skillsMatch = allMembers.All(x => x.MemberSkills.Select(y => y.SkillId).Any(yy => skillsNeeded.Contains(yy)));
            if (allMembers.Count() < members.Members.Count() || allMembers.Any(x => x.StatusId == 2 || x.StatusId == 3) || taken || !skillsMatch)
            {
                return BadRequest();
            }

            //SEND EMAIL NOTIFICATION - MEMBER CREW HAS BEEN CONFIRMED
            foreach (var item in allMembers)
            {
                _context.HeistMembers.Add(new HeistMember
                {
                    HeistId = id,
                    MemberId = item.MemberId
                });

                // for simplification added notification type
                // 1 - Added as member
                // 2 - Confirmed for heist
                // 3 - Heist started
                // 4 - Heist ended
                _mailLogic.SendMail(new MailRequest { MemberName = item.Name, HeistName = heist.Name, ToEmail = item.Email}, 2);
            }

            heist.StatusId = 2;
            await _context.SaveChangesAsync();

            Response.Headers.Add("Content-Location", "heist/" + id + "/members");
            return NoContent();
        }

        [HttpPut("{id}/start")]
        public async Task<IActionResult> Put(int id)
        {
            var heist = _context.Heists.FirstOrDefault(x => x.HeistId == id);
            if (heist == null)
            {
                return NotFound();
            }
            if (heist.StatusId > 2)
            {
                return StatusCode(405);
            }

            heist.StatusId = 3;
            await _context.SaveChangesAsync();


            //SEND EMAIL NOTIFICATION - HEIST HAS STARTED
            var members = _context.HeistMembers.Include(p => p.Member).Where(x => x.HeistId == heist.HeistId);
            foreach(var item in members)
            {
                // for simplification added notification type
                // 1 - Added as member
                // 2 - Confirmed for heist
                // 3 - Heist started
                // 4 - Heist ended
                _mailLogic.SendMail(new MailRequest { MemberName = item.Member.Name, HeistName = heist.Name, ToEmail = item.Member.Email }, 3);
            }

            Response.Headers.Add("Location", "heist/" + id + "/status");
            return Ok(null);
        }
    }
}
