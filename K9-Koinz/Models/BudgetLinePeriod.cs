using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using K9_Koinz.Models.Meta;

namespace K9_Koinz.Models {
    public class BudgetLinePeriod : BaseEntity {

        [DisplayName("Budget Line")]
        public Guid BudgetLineId { get; set; }
        public BudgetLine BudgetLine { get; set; }

        [DataType(DataType.Date, ErrorMessage = "Date only")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime StartDate { get; set; }

        [DataType(DataType.Date, ErrorMessage = "Date only")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime EndDate { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public double StartingAmount { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public double SpentAmount { get; set; }

        [NotMapped]
        public double BudgetedAmout { get; set; }
    }
}
