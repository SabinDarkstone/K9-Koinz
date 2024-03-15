using K9_Koinz.Models;
using K9_Koinz.Services.Meta;
using K9_Koinz.Utils;
using Microsoft.EntityFrameworkCore;

namespace K9_Koinz.Services.BackgroundWorkers {
    public class RecurringTransferJob : AbstractWorker<RecurringTransferJob> {
        public RecurringTransferJob(IServiceScopeFactory scopeFactory)
            : base(scopeFactory, DateTime.Now, new CronData(Cron.Hourly, 1), true) { }

        protected override async void DoWork(object state) {
            _logger.LogInformation("Checking for recurring transfers that need to be created: " + DateTime.Now.ToShortTimeString());

            var nextMinute = DateTime.Now.AddMinutes(1);
            var transactionsCreated = await CreateTransfers(nextMinute);

            _logger.LogInformation("Created transfer transactions with the following IDs:");
            transactionsCreated.ForEach(trans => {
                _logger.LogInformation(trans.Id.ToString());
            });
        }

        private async Task<List<Transaction>> CreateTransfers(DateTime mark) {
            var repeatingTransfers = _context.Transfers
                .Where(fer => fer.RepeatConfigId.HasValue)
                .Include(fer => fer.RepeatConfig)
                .AsEnumerable()
                .Where(fer => fer.RepeatConfig.NextFiring.HasValue)
                .Where(fer => fer.RepeatConfig.NextFiring >= mark)
                .ToList();

            var transactions = new List<Transaction>();
            foreach (var transfer in repeatingTransfers) {
                var transferInstance = _context.GetInstanceOfRecurring(transfer);
                transactions.AddRange(await _context.CreateTransactionsFromTransfer(transferInstance));
            }

            _context.Transactions.AddRange(transactions);
            await _context.SaveChangesAsync();
            return transactions;
        }
    }
}