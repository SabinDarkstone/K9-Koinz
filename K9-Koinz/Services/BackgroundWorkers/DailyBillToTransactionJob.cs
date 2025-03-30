
using K9_Koinz.Models;
using K9_Koinz.Services.Meta;
using K9_Koinz.Triggers;
using K9_Koinz.Utils;
using Microsoft.EntityFrameworkCore;

namespace K9_Koinz.Services.BackgroundWorkers {
    public class DailyBillToTransactionJob : AbstractWorker<DailyBillToTransactionJob> {

        private ITrigger<Transaction> _trigger;

        private List<Transaction> transactionsCreated = [];

        public DailyBillToTransactionJob(IServiceScopeFactory scopeFactory)
            : base(scopeFactory, DateTime.Now, new CronData(Cron.Hourly, 1), true) {
        }

        protected override void CreateScopeOnInit(IServiceScope scope) {
            base.CreateScopeOnInit(scope);
            _trigger = scope.ServiceProvider.GetRequiredService<ITrigger<Transaction>>();
        }

        protected override void DoWork(object state) {
            _logger.LogInformation("Checking for any transactions that need to be created for bills today: " + DateTime.Today.ToShortDateString());
            var nextMinute = DateTime.Now.AddMinutes(1);

            _logger.LogInformation("Checking for weekly bills...");
            transactionsCreated.AddRange(CreateTransactionsForBills(nextMinute, RepeatFrequency.WEEKLY));

            _logger.LogInformation("Checking for monthly bills...");
            transactionsCreated.AddRange(CreateTransactionsForBills(nextMinute, RepeatFrequency.MONTHLY));

            _logger.LogInformation("Checking for yearly bills...");
            transactionsCreated.AddRange(CreateTransactionsForBills(nextMinute, RepeatFrequency.YEARLY));

            if (transactionsCreated.Count > 0) {
                transactionsCreated.ForEach(trans => {
                    _logger.LogInformation("Created transaction: " + trans.Id.ToString() + " : " + trans.AccountName
                        + " : " + trans.MerchantName + " : " + trans.CategoryName
                        + " : " + trans.Amount.FormatCurrency(2));
                });

                _trigger.OnBeforeInsert(transactionsCreated);
            } else {
                _logger.LogInformation("No transactions need to be created today");
            }
        }

        private List<Transaction> CreateTransactionsForBills(DateTime date, RepeatFrequency frequency) {
            DateTime startDate, endDate;

            switch (frequency) {
                case RepeatFrequency.WEEKLY:
                    startDate = date.StartOfWeek();
                    break;
                case RepeatFrequency.MONTHLY:
                    startDate = date.StartOfMonth();
                    break;
                case RepeatFrequency.YEARLY:
                    startDate = date.StartOfYear();
                    break;
                default:
                    return null;
            }

            endDate = date;

            var bills = getBillsForTimePeriod(startDate, endDate);
            var transactionsCreated = new List<Transaction>();

            while (bills.Count > 0) {
                var transactionsToCreate = new List<Transaction>();
                foreach (var bill in bills) {
                    var newTransaction = new Transaction {
                        AccountId = bill.AccountId,
                        AccountName = bill.AccountName,
                        BillId = bill.Id,
                        MerchantId = bill.MerchantId,
                        MerchantName = bill.MerchantName,
                        CategoryId = bill.CategoryId.Value,
                        CategoryName = bill.CategoryName,
                        Amount = bill.Amount * -1,
                        Date = bill.RepeatConfig.CalculatedNextFiring.Value,
                        SavingsGoalId = bill.SavingsGoalId,
                        SavingsGoalName = bill.SavingsGoalName
                    };
                    transactionsToCreate.Add(newTransaction);
                }

                if (transactionsToCreate.Count > 0) {
                    _context.Transactions.AddRange(transactionsToCreate);

                    foreach (var bill in bills) {
                        bill.RepeatConfig.FireNow();
                    }

                    _context.Bills.UpdateRange(bills);
                    _context.SaveChanges();

                    transactionsCreated.AddRange(transactionsToCreate);
                }

                bills = getBillsForTimePeriod(startDate, endDate);
            }

            return transactionsCreated;
        }

        private List<Bill> getBillsForTimePeriod(DateTime startDate, DateTime endDate) {
            return _context.Bills
                .Include(bill => bill.RepeatConfig)
                .AsEnumerable()
                .Where(bill => bill.RepeatConfigId != null)
                .Where(bill => bill.RepeatConfig.CalculatedNextFiring.HasValue)
                .Where(bill => bill.RepeatConfig.CalculatedNextFiring >= startDate && bill.RepeatConfig.CalculatedNextFiring <= endDate)
                .Where(bill => bill.IsActive && bill.RepeatConfig.IsActive)
                .ToList();
        }

        protected override void AfterWork() {
            base.AfterWork();

            _trigger.OnAfterInsert(transactionsCreated);
        }
    }
}
