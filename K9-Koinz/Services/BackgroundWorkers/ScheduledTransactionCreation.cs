
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
            var transactionsCreated = CreateTransactionsForBills(DateTime.Now.AddMinutes(1));
            
            if (transactionsCreated.Any()) {
                transactionsCreated.ForEach(trans => {
                    _logger.LogInformation("Created transaction: " + trans.Id.ToString() + " : " + trans.AccountName
                        + " : " + trans.MerchantName + " : " + trans.CategoryName
                        + " : " + trans.Amount.FormatCurrency(2));
                });
            } else {
                _logger.LogInformation("No transactions need to be created today");
            }
        }

        private List<Transaction> CreateTransactionsForBills(DateTime date) {
            var bills = GetBillsFromMonthStartToNow(date);

            List<Transaction> transactionsToCreate = new List<Transaction>();
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

            if (transactionsToCreate.Any()) {
                _context.Transactions.AddRange(transactionsToCreate);
                _context.SaveChanges();

                foreach (var bill in bills) {
                    bill.LastDueDate = bill.NextDueDate;
                }

                _context.Bills.UpdateRange(bills);
                _context.SaveChanges();
            }

            return transactionsToCreate;
        }

        private List<Bill> GetBillsFromMonthStartToNow(DateTime refDate) {
            var startDate = refDate.StartOfMonth();
            return (_context.Bills.ToList())
                .Where(bill => bill.NextDueDate >= startDate)
                .Where(bill => bill.NextDueDate <= refDate)
                .ToList();
        }
    }
}
