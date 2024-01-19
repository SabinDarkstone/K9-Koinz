using System.ComponentModel.DataAnnotations.Schema;

namespace K9_Koinz.Models {
    public class BudgetLinePeriod : BaseEntity {
        public Guid BudgetLineId { get; set; }
        public virtual BudgetLine BudgetLine { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public double StartingAmount { get; set; }
        public double SpentAmount { get; set; }
    }
}
