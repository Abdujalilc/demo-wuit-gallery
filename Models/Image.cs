using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DemoWUITGallery.Models
{
    public class Image
    {
        public int Id { get; set; }        

        [StringLength(60, MinimumLength = 3)]
        [Required]
        public string Title { get; set; }


        public string? ImagePath { get; set; }


        public string? ThumbnailPath { get; set; }

        [BindNever]
        [DataType(DataType.Date)]
        public DateTime CreationDate { get; set; }

        [ForeignKey("PhotoGallery")]
        public int PhotoGalleryId { get; set; }

        [BindNever]
        public PhotoGallery? PhotoGallery { get; set; }

        public Image()
        {
            CreationDate = DateTime.Now;
        }
    }
}
