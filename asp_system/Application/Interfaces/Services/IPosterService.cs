using Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Services
{
    public interface IPosterService 
    {
        Task<IEnumerable<PosterDTO>> GetAllPoster();
        Task<ResponseDTO> AddPoster(PosterAddDTO post);
        Task<PosterDTO> GetPosterByUserId(string UserId);
        Task<ResponseDTO> DecreasePost(string userId);
        //Task<ResponseDTO> QuantityExtensionPost(PosterAddDTO post); 
        Task<ResponseDTO> IncreasePost(string userId);
    }
}
