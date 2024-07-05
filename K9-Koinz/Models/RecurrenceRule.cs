using K9_Koinz.Models.Meta;
using K9_Koinz.Models.Enums;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace K9_Koinz.Models {
    public class RecurrenceRule : BaseEntity {
        
        [Required]
        [DisplayName("Recurrence Type")]
        public RecurrenceType Type { get; set; }

        [DisplayName("Interval")]
        public int Interval { get; set; }

        [DisplayName("Day of the Month")]
        public int? DayOfMonth { get; set; }

        [DisplayName("Month")]
        public int? MonthOfYear { get; set; }

        [DisplayName("Day of the Week")]
        public int? DayOfWeek { get; set; }

        [Required]
        [DisplayName("Start Date")]
        [DataType(DataType.Date, ErrorMessage = "Date only")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime StartDate { get; set; }

        [DisplayName("End Date")]
        [DataType(DataType.Date, ErrorMessage = "Date only")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? EndDate { get; set; }

        [DisplayName("Previous Date")]
        [DataType(DataType.Date, ErrorMessage = "Date only")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? PreviousDate { get; set; }

        [DisplayName("Next Date")]
        [DataType(DataType.Date, ErrorMessage = "Date only")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? NextDate { get; set; }

        public bool IsActive { get; set; }

        [DisplayName("Bill")]
        public Guid? BillId { get; set; }
        public Bill Bill { get; set; }

        [DisplayName("Repeating Transfer")]
        public Guid? TransferId { get; set; }
        public Transfer Transfer { get; set; }
    }
}