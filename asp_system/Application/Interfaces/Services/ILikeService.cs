using Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Services
{
    public interface ILikeService
    {
        //Task<LikeDTO> GetLike(int idLike);
        Task<ResponseDTO> CreateLike(LikeCreateDTO like);
        //Task<ResponseDTO> DeleteLike(LikeCreateDTO LikeId);
        Task<IEnumerable<LikeDTO>> GetAllLikeByArtworkId(int ArtworkId);
    }
}
