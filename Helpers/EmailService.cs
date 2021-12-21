using MeetingNow.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;


namespace MeetingNow.Helpers
{
    public class EmailService : IHostedService, IDisposable
    {
        private Timer timer;

        public ApplicationContext applicationContext;
        private  IServiceScopeFactory serviceScopeFactory;

        public EmailService(IServiceScopeFactory serviceScopeFactory)
        { 
            this.serviceScopeFactory = serviceScopeFactory;
        }
        public void SendEmail(Object? state)
        {
            using (var scope = serviceScopeFactory.CreateScope())
            {
                applicationContext = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
                List<Event> events = applicationContext.Events.Include(e => e.Users).Where(e => e.Date.Date.CompareTo(DateTime.Now.Date) == 0).ToList();
                foreach (Event e in events)
                {
                    foreach (Profile p in e.Users)
                    {
                        User user = applicationContext.Users.FirstOrDefault(u => u.UserId == p.UserId);
                        MailAddress from = new MailAddress("kyrylo.kremenenko@nure.ua", "Admin");

                        MailAddress to = new MailAddress(user.Login);

                        MailMessage m = new MailMessage(from, to);

                        m.Subject = e.Name;

                        m.Body = $"Ваш ивент {e.Name} начинается в {e.Date.ToShortTimeString()}";

                        SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);

                        smtp.Credentials = new NetworkCredential("kyrylo.kremenenko@nure.ua", "panzar12345");
                        smtp.EnableSsl = true;
                        smtp.Send(m);
                    }
                }
            }
        }
        public void Dispose()
        {
            timer?.Dispose();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            timer = new Timer(SendEmail, null, TimeSpan.Zero, TimeSpan.FromDays(1));
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }
    }
}
