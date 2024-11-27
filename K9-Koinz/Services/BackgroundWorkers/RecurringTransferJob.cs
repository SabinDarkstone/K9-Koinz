using K9_Koinz.Models;
using K9_Koinz.Services.Meta;
using K9_Koinz.Triggers;
using K9_Koinz.Utils;
using Microsoft.EntityFrameworkCore;

namespace K9_Koinz.Services.BackgroundWorkers {
    public class RecurringTransferJob : AbstractWorker<RecurringTransferJob> {
        public RecurringTransferJob(IServiceScopeFactory scopeFactory)
            : base(scopeFactory, DateTime.Now, new CronData(Cron.Hourly, 6), true) { }

        protected override async void DoWork(object state) {
            _logger.LogInformation("Checking for recurring transfers that need to be created: " + DateTime.Now.ToShortTimeString());

            var nextMinute = DateTime.Now.AddMinutes(1);
            var transactionsCreated = await CreateTransfers(nextMinute);

            _logger.LogInformation("Created transfer transactions with the following IDs:");
            transactionsCreated
                .Where(trans => trans != null)
                .ToList()
                .ForEach(trans => {
                _logger.LogInformation(trans.Id.ToString());
            });
        }

        private async Task<List<Transaction>> CreateTransfers(DateTime mark) {
            var repeatingTransfers = _context.Transfers
                .Where(fer => fer.RepeatConfigId.HasValue)
                .Include(fer => fer.RepeatConfig)
                .AsEnumerable()
                .Where(fer => fer.RepeatConfig.CalculatedNextFiring.HasValue)
                .Where(fer => fer.RepeatConfig.CalculatedNextFiring.Value.Date <= mark.Date)
                .ToList();

            var transactions = new List<Transaction>();
            foreach (var transfer in repeatingTransfers) {
                var transferInstance = _context.GetInstanceOfRecurring(transfer);
                _context.Transfers.Add(transferInstance);
                transactions.AddRange(await _context.CreateTransactionsFromTransfer(transferInstance));
                transfer.RepeatConfig.FireNow();
            }

            var transactionsToInsert = transactions.Where(x => x != null).ToList();
            var transTrigger = new TransactionTrigger(_context, _logger);
            transTrigger.SetState(new Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary());
            transTrigger.OnBeforeInsert(transactionsToInsert);

            _context.Transactions.AddRange(transactionsToInsert);
            await _context.SaveChangesAsync();
            transTrigger.OnAfterInsert(transactionsToInsert);

            return transactions;
        }
    }
}