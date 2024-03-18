using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using K9_Koinz.Models.Meta;

namespace K9_Koinz.Models {

    public class Bill : BaseEntity, INameable, IAmount {
        public string Name { get; set; }
        [DisplayName("Account")]
        public Guid AccountId { get; set; }
        public Account Account { get; set; }
        public string AccountName { get; set; }
        [DisplayName("Merchant")]
        public Guid MerchantId { get; set; }
        public Merchant Merchant { get; set; }
        public string MerchantName { get; set; }
        [DisplayName("Category")]
        public Guid? CategoryId { get; set; }
        public Category Category { get; set; }
        public string CategoryName { get; set; }
        [DisplayName("First Due Date")]
        [DataType(DataType.Date, ErrorMessage = "Date only")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime FirstDueDate { get; set; }
        [DisplayName("Repeat Frequency")]
        public RepeatFrequency RepeatFrequency { get; set; }
        [DisplayName("Repeat Gap")]
        public int RepeatFrequencyCount { get; set; }
        [DisplayName("Bill End Date")]
        [DataType(DataType.Date, ErrorMessage = "Date only")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? EndDate { get; set; }
        [DisplayName("Repeat Settings")]
        public Guid? RepeatConfigId { get; set; }
        public RepeatConfig RepeatConfig { get; set; }
        public double Amount { get; set; }

        [DisplayName("Last Due Date")]
        [DataType(DataType.Date, ErrorMessage = "Date only")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? LastDueDate { get; set; }
        [DisplayName("Mark as Autopay")]
        public bool IsAutopay { get; set; }

        public ICollection<Transaction> Transactions { get; set; }
    }
}
