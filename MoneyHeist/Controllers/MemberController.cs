using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MoneyHeist.MailHelperClasses;
using MoneyHeist.Models;

namespace MoneyHeist.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class MemberController : Controller
    {
        private readonly MoneyHeistContext _context;
        private readonly MailLogic _mailLogic;

        public MemberController(MoneyHeistContext context, Services.IMailService mailService)
        {
            _context = context;
            _mailLogic = new MailLogic(mailService);
        }

        [HttpGet]
        public List<Member> Get()
        {
            var members = _context.Members.Include(p => p.MemberStatus).ToList();
            return members;
        }

        // GET: Member
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var member = _context.Members.Include(p => p.MemberSkills).ThenInclude(p => p.Skill).Include(p => p.MemberStatus).FirstOrDefault(x => x.MemberId == id);
            if(member == null)
            {
                return NotFound();
            }
            var memberViewModel = new Models.ViewModels.MemberViewModel
            {
                MemberId = member.MemberId,
                Name = member.Name,
                Sex = member.Sex,
                Email = member.Email,
                Skills = member.MemberSkills.Select(x => new Models.ViewModels.SkillViewModel
                {
                    Name = x.Skill.Name,
                    Level = string.Concat(Enumerable.Repeat("*", x.SkillLevel))
                }).ToList(),
                MainSkill = member.MemberSkills.First(x => x.IsMain).Skill.Name,
                Status = member.MemberStatus.Name
            };
            return Ok(memberViewModel);
        }

        [HttpGet("{id}/skills")]
        public IActionResult GetSkills(int id)
        {
            var member = _context.Members.Include(p => p.MemberSkills).ThenInclude(p => p.Skill).Include(p => p.MemberStatus).FirstOrDefault(x => x.MemberId == id);
            if (member == null)
            {
                return NotFound();
            }
            var memberViewModel = new Models.ViewModels.EditSkillsViewModel
            {
                Skills = member.MemberSkills.Select(x => new Models.ViewModels.SkillViewModel
                {
                    Name = x.Skill.Name,
                    Level = string.Concat(Enumerable.Repeat("*", x.SkillLevel))
                }).ToList(),
                MainSkill = member.MemberSkills.First(x => x.IsMain).Skill.Name
            };
            return Ok(memberViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Models.ViewModels.MemberViewModel member)
        {
            // validation - check for duplicates in the skills list
            var duplicateExists = member.Skills.GroupBy(n => n.Name).Any(c => c.Count() > 1);
            // validation - check whether this email already exists
            var emailExists = _context.Members.Any(x => x.Email == member.Email);
            // validation - check if status is valid
            var status = member.Status == "AVAILABLE" ? 1 : member.Status == "EXPIRED" ? 2 : member.Status == "INCARCERATED" ? 3 : member.Status == "RETIRED" ? 4 : 99;
            if (duplicateExists || emailExists || status > 3 || (member.Sex != 'F' && member.Sex != 'M'))
            {
                return BadRequest();
            }
            
            var newMember = new Member
            {
                Name = member.Name,
                Sex = member.Sex,
                Email = member.Email,
                StatusId = status
            };

            _context.Members.Add(newMember);
            // save to get memberId for next step
            await _context.SaveChangesAsync();

            foreach (var item in member.Skills)
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

                _context.MemberSkills.Add(new MemberSkill
                {
                    MemberId = newMember.MemberId,
                    SkillId = _context.Skills.FirstOrDefault(x => x.Name == item.Name).SkillId,
                    SkillLevel = item.Level.Length,
                    IsMain = item.Name.ToUpper() == member.MainSkill.ToUpper()
                });
            }

            await _context.SaveChangesAsync();

            //SEND EMAIL NOTIFICATION - ADDED AS MEMBER
            // for simplification added notification type
            // 1 - Added as member
            // 2 - Confirmed for heist
            // 3 - Heist started
            // 4 - Heist ended
            _mailLogic.SendMail(new MailRequest { MemberName = newMember.Name, ToEmail = newMember.Email }, 1);

            return Created("member/" + newMember.MemberId, null);

        }

        [HttpPut("{id}/skills")]
        public async Task<IActionResult> Put(int id, [FromBody] Models.ViewModels.EditSkillsViewModel skills)
        {
            // validation - check for duplicates in the skills list
            var duplicateExists = skills.Skills.GroupBy(n => n.Name).Any(c => c.Count() > 1);
            // get current skills to check if new mainSkill exists in previous or new skills
            var currentSkills = _context.MemberSkills.Where(x => x.MemberId == id).Include(p => p.Skill);
            var mainSkillExists = currentSkills.Any(x => x.Skill.Name.ToUpper() == skills.MainSkill.ToUpper()) || skills.Skills.Any(x => x.Name.ToUpper() == skills.MainSkill.ToUpper()) || string.IsNullOrWhiteSpace(skills.MainSkill);
            if (!mainSkillExists || duplicateExists || (skills.Skills.Count < 1 && string.IsNullOrWhiteSpace(skills.MainSkill)))
            {
                return BadRequest();
            }
            // return 404 not found if member doesn't exist
            if(_context.Members.FirstOrDefault(x => x.MemberId == id) == null)
            {
                return NotFound();
            }

            if (skills.Skills.Count < 1)
            {
                var skillId = _context.Skills.FirstOrDefault(x => x.Name.ToUpper() == skills.MainSkill.ToUpper()).SkillId;
                // check if this skill is already given to the member and if it is update it
                var prevSkill = _context.MemberSkills.FirstOrDefault(x => x.MemberId == id && x.SkillId == skillId);
                if (prevSkill != null)
                {
                    prevSkill.SkillLevel = 1;
                    prevSkill.IsMain = true;
                }
                else
                {
                    _context.MemberSkills.Add(new MemberSkill
                    {
                        SkillId = skillId,
                        SkillLevel = 1,
                        MemberId = id,
                        IsMain = true
                    });
                }
            }
            else
            {
                foreach(var item in skills.Skills)
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

                    //if main skill has been left empty first skill in skill list will be main
                    if (string.IsNullOrWhiteSpace(skills.MainSkill))
                    {
                        skills.MainSkill = item.Name;
                    }

                    var skillId = _context.Skills.FirstOrDefault(x => x.Name.ToUpper() == item.Name.ToUpper()).SkillId;
                    // check if this skill is already given to the member and if it is update it
                    var prevSkill = _context.MemberSkills.FirstOrDefault(x => x.MemberId == id && x.SkillId == skillId);
                    if (prevSkill != null)
                    {
                        prevSkill.SkillLevel = item.Level.Length;
                        prevSkill.IsMain = item.Name.ToUpper() == skills.MainSkill.ToUpper();
                    }
                    else
                    {
                        _context.MemberSkills.Add(new MemberSkill
                        {
                            SkillId = skillId,
                            SkillLevel = item.Level.Length,
                            MemberId = id,
                            IsMain = item.Name.ToUpper() == skills.MainSkill.ToUpper()
                        });
                    }

                }
            }
            await _context.SaveChangesAsync();
            Response.Headers.Add("Content-Location", "member/" + id + "/skills");
            return NoContent();

        }

        [HttpDelete("{id}/skills/{skillName}")]
        public async Task<IActionResult> Delete(int id, string skillName)
        {
            var member = _context.Members.FirstOrDefault(x => x.MemberId == id);
            if(member != null)
            {
                var allSkills = _context.MemberSkills.Where(x => x.MemberId == id);
                if(allSkills.Count() > 1)
                {
                    var memberSkill = allSkills.FirstOrDefault(x => x.Skill.Name.ToUpper() == skillName.ToUpper());
                    if (memberSkill != null)
                    {
                        allSkills.FirstOrDefault(x => x.Skill.Name.ToUpper() != skillName.ToUpper()).IsMain = true;
                        _context.MemberSkills.Remove(memberSkill);
                        await _context.SaveChangesAsync();
                        return NoContent();
                    }
                }
            }

            return NotFound();
        }

    }
}
