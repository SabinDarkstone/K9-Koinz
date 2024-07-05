using System.ComponentModel.DataAnnotations;

namespace K9_Koinz.Models.Enums {
    public enum CategoryType {
        [Display(Name = "Income")]
        INCOME,
        [Display(Name = "Expense")]
        EXPENSE,
        [Display(Name = "Transfer")]
        TRANSFER,
        [Display(Name = "Other")]
        OTHER,
        UNASSIGNED
    }

}