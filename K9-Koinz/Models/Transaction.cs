using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        public virtual Account Account { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Date only")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }
        [Required]
        [DisplayName("Merchant")]
        public Guid MerchantId { get; set; }
        public virtual Merchant Merchant { get; set; }
        [Required]
        [DisplayName("Category")]
        public Guid CategoryId { get; set; } = Guid.Empty;
        public virtual Category Category { get; set; }
        public double Amount { get; set; }
        public string Notes { get; set; }
        [DisplayName("Does this transaction occur AFTER the initial balance was set?")]
        public bool DoNotSkip { get; set; }

		public override int GetHashCode() {
			return Id.GetHashCode();
		}

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
}
