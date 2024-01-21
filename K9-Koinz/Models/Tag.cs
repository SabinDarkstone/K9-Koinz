using System.ComponentModel.DataAnnotations;

namespace K9_Koinz.Models {
    public class Tag : BaseEntity, INameable {
        [Required]
        public string Name { get; set; }

        [Required]
        [StringLength(1)]
        public string ShortForm { get; set; }
        public string HexColor { get; set; }
    }
}
