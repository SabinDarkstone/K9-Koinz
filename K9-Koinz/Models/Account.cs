using K9_Koinz.Models.Enums;
using K9_Koinz.Models.Meta;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace K9_Koinz.Models {
    public class Account : BaseEntity, INameable {
        public static Account EmptyAccount() {
            return new Account();
        }

        [Required]
        [DisplayName("Account Name")]
        public string Name { get; set; }
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
