using K9_Koinz.Data;
using K9_Koinz.Services.Meta;
using System.Transactions;

namespace K9_Koinz.Services {
    public interface IBillService : ICustomService {
        public abstract List<Transaction> CreateTransactionsForBills(DateTime date);
    }

    public class BillService : AbstractService<BillService>, IBillService {
        public BillService(KoinzContext context, ILogger<BillService> logger) : base(context, logger) { }

        public List<Transaction> CreateTransactionsForBills(DateTime date) {
            throw new NotImplementedException();
        }
    }
}
