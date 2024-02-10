using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Services.Meta;
using K9_Koinz.Utils;

namespace K9_Koinz.Services {
    public interface IBillService : ICustomService {
        public abstract List<Transaction> CreateTransactionsForBills(DateTime? date);
    }

    public class BillService : AbstractService<BillService>, IBillService {
        public BillService(KoinzContext context, ILogger<BillService> logger) : base(context, logger) { }

        public List<Transaction> CreateTransactionsForBills(DateTime? date) {
            if (!date.HasValue) {
                date = DateTime.Now;
            }

            var bills = GetBillsFromMonthStartToNow(date.Value);
            
            List<Transaction> transactionsToCreate = new List<Transaction>();
            foreach (var bill in bills) {
                var newTransaction = new Transaction {
                    AccountId = bill.Id,
                    AccountName = bill.AccountName,
                    BillId = bill.Id,
                    MerchantId = bill.MerchantId,
                    MerchantName = bill.MerchantName,
                    CategoryId = bill.CategoryId.Value,
                    CategoryName = bill.CategoryName,
                    Amount = bill.BillAmount * -1,
                    Date = bill.NextDueDate ?? DateTime.Today
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

            return _context.Bills.AsEnumerable()
                .Where(bill => bill.NextDueDate >= startDate)
                .Where(bill => bill.NextDueDate <= refDate)
                .ToList();
        }
    }
}
