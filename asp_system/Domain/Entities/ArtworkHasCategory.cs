using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class ArtworkHasCategory
    {
        //Id	ArtworkId	CategoryId
        [Key]
        public int Id { get; set; }
        public int? ArtworkId { get; set; }
        public int? CategoryId { get; set; }

        public virtual Artwork Artwork { get; set; }
        public virtual Category Category { get; set; }

    }
}
