using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace K9_Koinz.Models.Meta
{
    public abstract class DateTrackedEntity : BaseEntity
    {
        [DisplayName("Created Date")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy hh:mm tt}", ApplyFormatInEditMode = true)]
        public DateTime CreatedDate { get; set; }
        [DisplayName("Last Modified Date")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy hh:mm tt}", ApplyFormatInEditMode = true)]
        public DateTime LastModifiedDate { get; set; }
    }
}
