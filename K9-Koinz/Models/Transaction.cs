using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using K9_Koinz.Models.Meta;

namespace K9_Koinz.Models {
    public enum TransactionType {
        [Display(Name = "Debit")]
        MINUS,
        [Display(Name = "Credit")]
        PLUS
    }

    public class Transaction : DateTrackedEntity {
        [Required]
        [DisplayName("Account")]
        public Guid AccountId { get; set; } = Guid.Empty;
        public Account Account { get; set; }
        public string AccountName { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Date only")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }
        [Required]
        [DisplayName("Merchant")]
        public Guid MerchantId { get; set; }
        public Merchant Merchant { get; set; }
        public string MerchantName { get; set; }
        [Required]
        [DisplayName("Category")]
        public Guid CategoryId { get; set; } = Guid.Empty;
        public Category Category { get; set; }
        public string CategoryName { get; set; }
        [Column(TypeName = "decimal(10, 2)")]
        public double Amount { get; set; }
        public string Notes { get; set; }
        [DisplayName("Does this transaction occur AFTER the initial balance was set?")]
        public bool DoNotSkip { get; set; }
        [DisplayName("Tag")]
        public Guid? TagId { get; set; }
        public Tag Tag { get; set; }
        [DisplayName("Bill")]
        public Guid? BillId { get; set; }
        public Bill Bill { get; set; }
        [DisplayName("Savings Goal")]
        public Guid? SavingsGoalId {  get; set; }
        public SavingsGoal SavingsGoal { get; set; }
        public string SavingsGoalName { get; set; }

        [NotMapped]
        public bool IsUnCategorized {
            get {
                return CategoryId == Guid.Empty;
            }
        }

        [NotMapped]
        public TransactionType TransactionType {
            get {
                if (Amount > 0) {
                    return TransactionType.PLUS;
                } else {
                    return TransactionType.MINUS;
                }
            }
        }
    }

    public struct TinyTransaction {
        public Guid Id { get; init; }
        public double Amount { get; init; }

        public TinyTransaction(Transaction transaction) {
            this.Id = transaction.Id;
            this.Amount = transaction.Amount;
        }
    }
}
