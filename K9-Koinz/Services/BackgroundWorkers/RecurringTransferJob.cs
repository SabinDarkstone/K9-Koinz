using K9_Koinz.Models;
using K9_Koinz.Services.Meta;
using K9_Koinz.Utils;

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
            var repeatingTransfers = _data.TransferRepository.GetRecurringBeforeDate(mark);

            var transactions = new List<Transaction>();
            foreach (var transfer in repeatingTransfers) {
                var transferInstance = transfer.GetInstanceOfRecurring();
                transactions.AddRange(await _data.CreateTransactionsFromTransfer(transferInstance));
                transfer.RepeatConfig.FireNow();
            }

            _data.TransactionRepository.Add(transactions);
            await _data.SaveAsync();
            return transactions;
        }
    }
}