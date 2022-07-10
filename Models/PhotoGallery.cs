using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DemoWUITGallery.Models
{
    public class PhotoGallery
    {
        public int Id { get; set; }

        [StringLength(60, MinimumLength = 3)]
        [Required]
        public string Title { get; set; }

        [BindNever]
        [DataType(DataType.Date)]
        public DateTime CreationDate { get; set; }

        public IList<Image>? Image { get; set; }

        [NotMapped]
        public virtual PagedList<Image> Images { get; set; } = new PagedList<Image>();
    }
}
