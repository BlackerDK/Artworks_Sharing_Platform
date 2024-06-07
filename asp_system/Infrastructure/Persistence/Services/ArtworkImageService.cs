using Application.Interfaces;
using Application.Interfaces.Services;
using AutoMapper;
using Domain.Entities;
using Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Services
{
    public class ArtworkImageService : IArtworkImageService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public ArtworkImageService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public Task<ResponseDTO> AddArtworkImages(ArtworkImageDTO artworkImages)
        {
            try
            {
                var ArtworkImages = new ArtworkImage
                {
                    ArtworkId = artworkImages.ArtworkId,
                    Image = artworkImages.ImageUrl
                };
                _unitOfWork.Repository<ArtworkImage>().AddAsync(ArtworkImages);
                _unitOfWork.Save();
                return Task.FromResult(new ResponseDTO { Message = "Artwork Images Added Successfully", IsSuccess = true, Data = artworkImages });
            }
            catch(Exception ex)
            {
                return Task.FromResult(new ResponseDTO { Message = ex.Message, IsSuccess = false });
            }
        }

        public async Task<ResponseDTO> DeleteArtworkImage(int artworkImageId)
        {
            try
            {
                var artworkImage = await _unitOfWork.Repository<ArtworkImage>().GetByIdAsync(artworkImageId);
                if(artworkImage != null)
                {
                     await _unitOfWork.Repository<ArtworkImage>().DeleteAsync(artworkImage);
                    _unitOfWork.Save();
                    return (new ResponseDTO { Message = "Artwork Image Deleted Successfully", IsSuccess = true , Data = artworkImage});
                }
                return (new ResponseDTO { Message = "Artwork Image Not Found", IsSuccess = false });
            }
            catch (Exception ex)
            {
                return (new ResponseDTO { Message = ex.Message, IsSuccess = false });
            }
        }
    }
}
