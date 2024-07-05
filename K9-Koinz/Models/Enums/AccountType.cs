using System.ComponentModel.DataAnnotations;

namespace K9_Koinz.Models.Enums {

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
}