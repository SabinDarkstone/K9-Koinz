
using Humanizer;
using K9_Koinz.Models;
using K9_Koinz.Services.Meta;
using K9_Koinz.Utils;

namespace K9_Koinz.Services.BackgroundWorkers {
    public class ScheduledTransactionCreation : AbstractWorker<ScheduledTransactionCreation> {

        public ScheduledTransactionCreation(IServiceScopeFactory scopeFactory)
            : base(scopeFactory, DateTime.Today.AtMidnight(), new CronData(Cron.Daily, 1), true) {
        }

        protected override void DoWork(object state) {
            base.DoWork(state);
            _logger.LogInformation("Checking for any transactions that need to be created for bills today: " + DateTime.Today.ToShortDateString());
            var transactionsCreated = new List<Transaction>();
            var nextMinute = DateTime.Now.AddMinutes(1);

            _logger.LogInformation("Checking for weekly bills...");
            transactionsCreated.AddRange(CreateTransactionsForBills(nextMinute, Frequency.WEEKLY));

            _logger.LogInformation("Checking for monthly bills...");
            transactionsCreated.AddRange(CreateTransactionsForBills(nextMinute, Frequency.MONTHLY));

            _logger.LogInformation("Checking for yearly bills...");
            transactionsCreated.AddRange(CreateTransactionsForBills(nextMinute, Frequency.YEARLY));
            
            if (transactionsCreated.Count > 0) {
                transactionsCreated.ForEach(trans => {
                    _logger.LogInformation("Created transaction: " + trans.Id.ToString() + " : " + trans.AccountName
                        + " : " + trans.MerchantName + " : " + trans.CategoryName
                        + " : " + trans.Amount.FormatCurrency(2));
                });
            } else {
                _logger.LogInformation("No transactions need to be created today");
            }
        }

        private List<Transaction> CreateTransactionsForBills(DateTime date, Frequency frequency) {
            DateTime startDate, endDate;

            switch (frequency) {
                case Frequency.WEEKLY:
                    startDate = date.StartOfWeek();
                    break;
                case Frequency.MONTHLY:
                    startDate = date.StartOfMonth();
                    break;
                case Frequency.YEARLY:
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
                        Amount = bill.BillAmount * -1,
                        Date = bill.NextDueDate.Value
                    };
                    transactionsToCreate.Add(newTransaction);
                }

                if (transactionsToCreate.Count > 0) {
                    _context.Transactions.AddRange(transactionsToCreate);

                    foreach (var bill in bills) {
                        bill.LastDueDate = bill.NextDueDate;
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
            return _context.Bills.AsEnumerable()
                .Where(bill => bill.NextDueDate >= startDate && bill.NextDueDate <= endDate)
                .Where(bill => !bill.IsExpired)
                .ToList();
        }
    }
}
