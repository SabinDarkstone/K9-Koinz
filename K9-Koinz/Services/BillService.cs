using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Services.Meta;
using K9_Koinz.Utils;

namespace K9_Koinz.Services {
    public interface IBillService : ICustomService {
        public abstract List<Transaction> CreateTransactionsForBills(DateTime date);
    }

    public class BillService : AbstractService<BillService>, IBillService {
        public BillService(KoinzContext context, ILogger<BillService> logger) : base(context, logger) { }

        public List<Transaction> CreateTransactionsForBills(DateTime date) {
            var bills = GetBillsFromMonthStartToNow(date);

            List<Transaction> transactionsToCreate = new List<Transaction>();
            foreach (var bill in bills) {
                var newTransaction = new Transaction();
                newTransaction.AccountId = bill.AccountId;
                newTransaction.AccountName = bill.AccountName;
                newTransaction.BillId = bill.Id;
                newTransaction.MerchantId = bill.MerchantId;
                newTransaction.MerchantName = bill.MerchantName;
                newTransaction.CategoryId = bill.CategoryId.Value;
                newTransaction.CategoryName = bill.CategoryName;
                newTransaction.Amount = bill.BillAmount * -1;
                newTransaction.Date = bill.NextDueDate.Value;
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
