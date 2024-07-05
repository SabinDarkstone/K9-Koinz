using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using K9_Koinz.Models.Meta;

namespace K9_Koinz.Models {

    public class Bill : BaseEntity, INameable, IAmount {
        public string Name { get; set; }

        [Required]
        [DisplayName("Account")]
        public Guid AccountId { get; set; }
        public Account Account { get; set; }
        public string AccountName { get; set; }

        [Required]
        [DisplayName("Merchant")]
        public Guid MerchantId { get; set; }
        public Merchant Merchant { get; set; }
        public string MerchantName { get; set; }

        [DisplayName("Category")]
        public Guid? CategoryId { get; set; }
        public Category Category { get; set; }
        public string CategoryName { get; set; }

        [Obsolete]
        [DisplayName("Repeat Settings")]
        public Guid? RepeatConfigId { get; set; }
        [Obsolete]
        public RepeatConfig RepeatConfig { get; set; }

        public Guid? RecurrenceRuleId { get; set; }
        public RecurrenceRule RecurrenceRule { get; set; }

        [Required]
        public double Amount { get; set; }

        [DisplayName("Repeating Bill?")]
        public bool IsRepeatBill { get; set; }

        [Obsolete]
        [DisplayName("Last Due Date")]
        [DataType(DataType.Date, ErrorMessage = "Date only")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? NextDate { get; set; }

        [DisplayName("Mark as Autopay")]
        public bool IsAutopay { get; set; }

        [DisplayName("Currently Active?")]
        public bool IsActive { get; set; }

        public ICollection<Transaction> Transactions { get; set; }
    }
}
