using Microsoft.EntityFrameworkCore;
using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Pages.Meta;

namespace K9_Koinz.Pages.Transactions {
    public class DetailsModel : DetailsPageModel<Transaction> {
        public DetailsModel(TransactionRepository repository) : base(repository) { }

        protected override Task<Transaction> QueryRecord(Guid id) {
            return (_repository as TransactionRepository).GetTransactionWithDetailsById(id);
        }
    }
}
