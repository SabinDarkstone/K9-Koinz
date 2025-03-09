using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using K9_Koinz.Models.Meta;
using K9_Koinz.Utils.Attributes;

namespace K9_Koinz.Models {

    public class Bill : BaseEntity, INameable, IAmount {
        [RecycleBinProp("Name")]
        public string Name { get; set; }
        [DisplayName("Account")]
        public Guid AccountId { get; set; }
        public Account Account { get; set; }
        public string AccountName { get; set; }
        [DisplayName("Merchant")]
        public Guid MerchantId { get; set; }
        public Merchant Merchant { get; set; }
        [RecycleBinProp("Merchant")]
        public string MerchantName { get; set; }
        [DisplayName("Category")]
        public Guid? CategoryId { get; set; }
        public Category Category { get; set; }
        public string CategoryName { get; set; }
        [DisplayName("Repeat Settings")]
        public Guid? RepeatConfigId { get; set; }
        public RepeatConfig RepeatConfig { get; set; }
        [RecycleBinProp("Amount")]
        public double Amount { get; set; }

        [DisplayName("Repeating Bill?")]
        public bool IsRepeatBill { get; set; }

        [DisplayName("Last Due Date")]
        [DataType(DataType.Date, ErrorMessage = "Date only")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? LastDueDate { get; set; }
        [DisplayName("Mark as Autopay")]
        public bool IsAutopay { get; set; }
        [DisplayName("Currently Active?")]
        public bool IsActive { get; set; }

        public ICollection<Transaction> Transactions { get; set; }
    }
}
