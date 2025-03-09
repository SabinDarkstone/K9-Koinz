using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using K9_Koinz.Models.Meta;
using K9_Koinz.Utils.Attributes;

namespace K9_Koinz.Models {
    public class Tag : BaseEntity, INameable {
        [Required]
        [RecycleBinProp("Name")]
        public string Name { get; set; }

        [Required]
        [StringLength(1)]
        [DisplayName("Short Form")]
        public string ShortForm { get; set; }
        [Required]
        [DisplayName("Badge Color")]
        public string HexColor { get; set; }
        [DisplayName("Retire Tag")]
        public bool IsRetired { get; set; }

        public ICollection<Transaction> Transactions { get; set; }
    }
}
