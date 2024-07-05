using System.ComponentModel.DataAnnotations;

namespace K9_Koinz.Models.Enums {
    
    public enum TransactionType {
        [Display(Name = "Debit")]
        MINUS,
        [Display(Name = "Credit")]
        PLUS
    }
}