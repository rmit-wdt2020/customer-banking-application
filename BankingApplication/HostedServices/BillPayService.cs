﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BankingApplication.Data;
using BankingApplication.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RepositoryWrapper;

namespace BankingApplication.HostedServices {
    //IHostedService example referenced from Microsoft Documentation
    //https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/hosted-services?view=aspnetcore-3.1&tabs=visual-studio
    public class BillPayService : IHostedService, IDisposable {
        private int executionCount = 0;
        private readonly ILogger<BillPayService> _logger;
        private readonly IServiceScopeFactory _scopedFactory;
        private Wrapper repo;
        private List<BillPay> bills = new List<BillPay>();
        private Timer _timer;
        private Task task;

        public BillPayService (ILogger<BillPayService> logger, IServiceScopeFactory scopedFactory) {
            _logger = logger;
            _scopedFactory = scopedFactory;
        }

        public Task StartAsync (CancellationToken stoppingToken) {
            _logger.LogInformation ("Timed Hosted Service running.");

            _timer = new Timer (DoWork, null, TimeSpan.Zero,
                TimeSpan.FromSeconds (15));

            return Task.CompletedTask;
        }
        //DbContext scoping learnt from StackOverflow user alsami:
        //https://stackoverflow.com/questions/51572637/access-dbcontext-service-from-background-task
        private void DoWork (object state) {

           task = CheckBills();
           
            var count = Interlocked.Increment (ref executionCount);
            _logger.LogInformation (
                "CheckBills completed. Count: {Count}", count);
        }

        //Timed method for checking if bills need to be paid.
        //Looks up bills that have a scheduled date before the current time
        //and pays them in the referenced account.
        private async Task CheckBills()
        {
            using (var scope = _scopedFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<BankAppContext>();
                repo = new Wrapper(dbContext);
                bills = await repo.BillPay.GetByID(x => x.ScheduleDate < DateTime.UtcNow).ToListAsync();
                foreach (var bill in bills)
                {
                    var account = await repo.Account.GetByID(x => x.AccountNumber == bill.AccountNumber).Include(x => x.Transactions).FirstOrDefaultAsync();
                    account.PayBill(bill);
                    if(bill.Period == BillPay.Periods.OnceOff)
                    {
                        repo.BillPay.Delete(bill);
                    }
                    else
                    {
                        repo.BillPay.Update(bill);
                    }
                }
                await repo.SaveChanges();


            }
        }

        public Task StopAsync (CancellationToken stoppingToken) {
            _logger.LogInformation ("Timed Hosted Service is stopping.");

            _timer?.Change (Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose () {
            _timer?.Dispose ();
        }
    }
}