using K9_Koinz.Models.Meta;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace K9_Koinz.Models {
    public class Merchant : BaseEntity, INameable {

        [Required]
        [DisplayName("Merchant Name")]
        public string Name { get; set; }

        [DisplayName("Description")]
        public string Description { get; set; }

        [DisplayName("Retire Merchant")]
        public bool IsRetired { get; set; }

        public ICollection<Transaction> Transactions { get; set; }
        public ICollection<Bill> Bills { get; set; }
    }
}
