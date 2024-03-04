using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using K9_Koinz.Models.Meta;

namespace K9_Koinz.Models {
    public class Transfer : BaseEntity {
        [Required]
        [DisplayName("From Account")]
        public Guid FromAccountId { get; set; }
        public Account FromAccount { get; set; }
        [Required]
        [DisplayName("To Account")]
        public Guid ToAccountId { get; set; }
        public Account ToAccount { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Date only")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }
        [Required]
        [DisplayName("Merchant")]
        public Guid MerchantId { get; set; }
        public Merchant Merchant { get; set; }
        [Required]
        [DisplayName("Category")]
        public Guid CategoryId { get; set; }
        public Category Category { get; set; }
        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        public double Amount { get; set; }
        public string Notes { get; set; }
        [DisplayName("Tag")]
        public Guid? TagId { get; set; }
        public Tag Tag { get; set; }
    }
}
