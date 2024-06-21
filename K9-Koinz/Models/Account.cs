using K9_Koinz.Models.Meta;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace K9_Koinz.Models {
    public enum AccountType {
        [Display(Name = "Credit Card")]
        CREDIT_CARD,
        [Display(Name = "Loan")]
        LOAN,
        [Display(Name = "Checking Account")]
        CHECKING,
        [Display(Name = "Savings Account")]
        SAVINGS,
        [Display(Name = "Investments")]
        INVESTMENT,
        [Display(Name = "Property")]
        PROPERTY
    }

    public class Account : BaseEntity, INameable {

        [Required]
        [DisplayName("Account Name")]
        public string Name { get; set; }

        [DisplayName("Description")]
        public string Description { get; set; }

        [Required]
        [DisplayName("Account Type")]
        public AccountType Type { get; set; }

        [DisplayName("Initial Balance")]
        [Column(TypeName = "decimal(10, 2)")]

        public double InitialBalance { get; set; }
        [DisplayName("Initial Balance Date")]
        public DateTime InitialBalanceDate { get; set; }

        [DisplayName("Minimum Balance")]
        [Column(TypeName = "decimal(10, 2)")]
        public double? MinimumBalance { get; set; }

        [DisplayName("Exclude from Budget and Trends")]
        public bool HideAccountTransactions { get; set; }

        [DisplayName("Retire Account")]
        public bool IsRetired { get; set; }

        public ICollection<Transaction> Transactions { get; set; }
        public ICollection<Bill> Bills { get; set; }

        [NotMapped]
        [DisplayName("Current Balance")]
        public double CurrentBalance { get; set; }
    }
}
