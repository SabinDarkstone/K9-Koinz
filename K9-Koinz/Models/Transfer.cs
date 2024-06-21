using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using K9_Koinz.Models.Meta;

namespace K9_Koinz.Models {
    public class Transfer : BaseEntity, IAmount {

        [DisplayName("From Account")]
        public Guid? FromAccountId { get; set; }
        public Account FromAccount { get; set; }

        [Required]
        [DisplayName("To Account")]
        public Guid ToAccountId { get; set; }
        public Account ToAccount { get; set; }

        [DataType(DataType.Date, ErrorMessage = "Date only")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }

        [Required]
        [DisplayName("Merchant")]
        public Guid MerchantId { get; set; }
        public Merchant Merchant { get; set; }

        [Required]
        [DisplayName("Category")]
        public Guid CategoryId { get; set; }
        public Category Category { get; set; }

        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        public double Amount { get; set; }

        [NotMapped]
        public string Notes { get; set; }

        [DisplayName("Tag")]
        public Guid? TagId { get; set; }
        public Tag Tag { get; set; }

        [DisplayName("Savings Goal")]
        public Guid? SavingsGoalId { get; set; }
        public SavingsGoal SavingsGoal { get; set; }

        [DisplayName("Recurring Schedule")]
        public Guid? RepeatConfigId { get; set; }
        public RepeatConfig RepeatConfig { get; set; }

        public Guid? RecurringTransferId { get; set; }
        public Transfer RecurringTransfer { get; set; }

        public bool IsSplit { get; set; }

        [DisplayName("Budget to Savings Transfer")]
        public bool IsTransferFromBudget { get; set; }

        public Transaction ToTransaction {
            get {
                if (Transactions == null || Transactions.Count == 0) {
                    return null;
                }

                // Hide transactions that are split (indicated by having a parent transaction
                // from the query results of the "To" transaction as there should only be one.
                return Transactions
                    .Where(trans => trans.AccountId == ToAccountId)
                    .Where(trans => trans.Amount > 0)
                    .Where(trans => trans.ParentTransactionId == null)
                    .SingleOrDefault();
            }
        }

        public Transaction FromTransaction {
            get {
                if (Transactions == null || Transactions.Count == 0) {
                    return null;
                }

                return Transactions
                    .Where(trans => trans.AccountId == FromAccountId)
                    .Where(trans => trans.Amount > 0)
                    .SingleOrDefault();
            }
        }

        public ICollection<Transaction> Transactions { get; set; }

        public ICollection<Transfer> InstantiatedFromRecurring { get; set; }
    }
}
