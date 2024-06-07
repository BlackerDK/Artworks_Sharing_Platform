using Domain.Model;

namespace Application.Interfaces.Services
{
    public interface IArtworkImageService
    {
        //add list of artwork images
        Task<ResponseDTO> AddArtworkImages(ArtworkImageDTO artworkImages);
        //delete artwork image
        Task<ResponseDTO> DeleteArtworkImage(int artworkImageId);
    }
}
