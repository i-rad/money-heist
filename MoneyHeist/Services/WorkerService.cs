using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MoneyHeist.MailHelperClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MoneyHeist
{
    public class WorkerService : BackgroundService
    {

        public WorkerService(IServiceProvider services)
        {
            Services = services;
        }

        public IServiceProvider Services { get; }

        private const int generalDelay = 60 * 1000; // 60 seconds

        private DateTime? isTime;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(generalDelay, stoppingToken);
                await DoDBChangesAsync();
            }
        }

        private async Task DoDBChangesAsync()
        {
            SetIsTime();
            if (isTime.HasValue && isTime <= DateTime.Now)
            {
                Console.WriteLine("Executing background task");
                await ScheduledItemChangeState();
            }

        }

        private async Task ScheduledItemChangeState()
        {
            using (var scope = Services.CreateScope())
            {
                var context =
                 scope.ServiceProvider
                     .GetRequiredService<Models.MoneyHeistContext>();

                var mailLogic =
                 scope.ServiceProvider
                     .GetRequiredService<MailHelperClasses.MailLogic>();

                var heist = context.Heists.Where(x => x.StatusId == 2 || x.StatusId == 3).OrderBy(a => a.StartTime <= DateTime.Now ? a.EndTime : a.StartTime).FirstOrDefault();
                if (heist != null)
                {
                    if (heist.StatusId == 2)
                    {
                        heist.StatusId = 3;

                        //SEND EMAIL NOTIFICATION - HEIST HAS STARTED
                        var members = context.HeistMembers.Where(x => x.HeistId == heist.HeistId);
                        foreach (var item in members)
                        {
                            // for simplification added notification type
                            // 1 - Added as member
                            // 2 - Confirmed for heist
                            // 3 - Heist started
                            // 4 - Heist ended
                            mailLogic.SendMail(new MailRequest { MemberName = item.Member.Name, HeistName = heist.Name, ToEmail = item.Member.Email }, 3);
                        }
                    }
                    else
                    {
                        heist.StatusId = 4;
                        var requiredMembers = context.HeistSkills.Where(x => x.HeistId == heist.HeistId).Sum(y => y.HeistMembers);
                        var actualMembers = context.HeistMembers.Where(x => x.HeistId == heist.HeistId);
                        var outcomePercentage = (int)Math.Round((double)(100 * actualMembers.Count()) / requiredMembers);
                        var experienceBoost = (int)(Math.Floor(heist.EndTime.Subtract(heist.StartTime).TotalSeconds)/86400);
                        Random r = new Random();

                        //INCREASE MEMBERS EXPERIENCE
                        //SEND EMAIL NOTIFICATION - HEIST HAS ENDED                        
                        foreach (var item in actualMembers)
                        {
                            //increase experience
                            foreach(var memberSkill in item.Member.MemberSkills)
                            {
                                if (heist.HeistSkills.Select(x => x.SkillId).Contains(memberSkill.SkillId))
                                {
                                    memberSkill.SkillLevel += experienceBoost <= 10 ? memberSkill.SkillLevel += experienceBoost : memberSkill.SkillLevel = 10;
                                }
                            }
                            // random status
                            //2 - EXPIRED, 3 - INCARCERATED
                            item.Member.StatusId = r.Next(2, 4);

                            // for simplification added notification type
                            // 1 - Added as member
                            // 2 - Confirmed for heist
                            // 3 - Heist started
                            // 4 - Heist ended
                            mailLogic.SendMail(new MailRequest { MemberName = item.Member.Name, HeistName = heist.Name, ToEmail = item.Member.Email }, 4);
                        }

                        // CHECK THE OUTCOME
                        if (outcomePercentage < 75)
                        {
                            if (outcomePercentage < 50)
                            {
                                context.HeistOutcome.Add(new Models.HeistOutcome
                                {
                                    HeistId = heist.HeistId,
                                    Outcome = "FAILED"
                                });
                            }
                            else if (outcomePercentage >= 50 && outcomePercentage < 75)
                            {
                                //check if this percentage range has failed or succeeded
                                // again randomly
                                if (r.Next(1, 3) == 1)
                                {
                                    actualMembers.OrderBy(x => r.Next()).Take((int)Math.Round(actualMembers.Count() * 0.67));
                                    context.HeistOutcome.Add(new Models.HeistOutcome
                                    {
                                        HeistId = heist.HeistId,
                                        Outcome = "FAILED"
                                    });
                                }
                                else
                                {
                                    actualMembers.OrderBy(x => r.Next()).Take((int)Math.Round(actualMembers.Count() * 0.33));
                                    context.HeistOutcome.Add(new Models.HeistOutcome
                                    {
                                        HeistId = heist.HeistId,
                                        Outcome = "SUCCEEDED"
                                    });
                                }
                            }
                            foreach (var item in actualMembers)
                            {
                                // random status
                                //2 - EXPIRED, 3 - INCARCERATED
                                item.Member.StatusId = r.Next(2, 4);
                            }
                        }
                        else
                        {
                            if (outcomePercentage < 100)
                            {
                                actualMembers.OrderBy(x => r.Next()).Take((int)Math.Round(actualMembers.Count() * 0.33));

                                foreach (var item in actualMembers)
                                {
                                    item.Member.StatusId = 3;
                                }
                            }

                            context.HeistOutcome.Add(new Models.HeistOutcome
                            {
                                HeistId = heist.HeistId,
                                Outcome = "SUCCEEDED"
                            });
                        }
                    }

                    await context.SaveChangesAsync();
                }
            }
        }

        private void SetIsTime()
        {
            using (var scope = Services.CreateScope())
            {
                var context =
                 scope.ServiceProvider
                     .GetRequiredService<Models.MoneyHeistContext>();

                var newHeist = context.Heists.Where(x => x.StatusId == 2 || x.StatusId == 3).OrderBy(a => a.StartTime <= DateTime.Now ? a.EndTime : a.StartTime).FirstOrDefault();
                if (newHeist != null)
                {
                    isTime = newHeist.StartTime <= DateTime.Now ? newHeist.EndTime : newHeist.StartTime;
                }
                else
                {
                    isTime = null;
                }

            }
        }
    }
}
