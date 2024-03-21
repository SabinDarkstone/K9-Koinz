using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Services.Meta;
using Microsoft.EntityFrameworkCore;

namespace K9_Koinz.Services {
    public interface IDbCleanupService : ICustomService {
        public abstract Task DateMigrateBillSchedules();
    }

    public class DbCleanupService : AbstractService<DbCleanupService>, IDbCleanupService {
        public DbCleanupService(RepositoryWrapper data, ILogger<DbCleanupService> logger) : base(data, logger) { }

        public async Task DateMigrateBillSchedules() {
            _logger.LogInformation("Checking for bills to migration schedules");
            var bills = await _context.Bills
                .Where(bill => bill.RepeatConfigId == null)
                .ToListAsync();

            if (bills.Count() == 0) {
                _logger.LogInformation("No bills with out data model found");
                return;
            }

            var newRepeatConfigs = new List<RepeatConfig>();
            foreach (var bill in bills) {
                var config = new RepeatConfig {
                    FirstFiring = bill.FirstDueDate,
                    Frequency = bill.RepeatFrequency,
                    IntervalGap = bill.RepeatFrequencyCount,
                    LastFiring = bill.LastDueDate,
                    Mode = RepeatMode.INTERVAL,
                    TerminationDate = bill.EndDate
                };

                newRepeatConfigs.Add(config);
            }

            await _context.RepeatConfigs.AddRangeAsync(newRepeatConfigs);

            for (var i = 0; i < bills.Count; i++) {
                bills[i].RepeatConfigId = newRepeatConfigs[i].Id;
            }

            _context.UpdateRange(bills);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Bill schedules migrated successfully: " + bills.Count());
        }
    }
}
