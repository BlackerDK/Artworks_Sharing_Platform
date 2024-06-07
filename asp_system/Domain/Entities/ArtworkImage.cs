using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class ArtworkImage
    {
        //Id	ArtworkId	Image
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int ArtworkId { get; set; }
        public string Image { get; set; }
        public virtual Artwork Artwork { get; set; }

    }
}
