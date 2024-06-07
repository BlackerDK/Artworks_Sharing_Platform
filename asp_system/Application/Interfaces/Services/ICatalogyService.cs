using Domain.Entities;
using Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Services
{
    public interface ICatalogyService
    {
        Task<IEnumerable<CatalogyDTO>> GetAllCatalogy();
        Task<ResponseDTO> AddCatalogy(CatalogyAddDTO catalogy);
        Task<CatalogyDTO> GetCatalogyById(int id);
        Task<ResponseDTO> DeteleCatalogy(int id);
        Task <ResponseDTO> UpdateCatalogy(int id, CatalogyDTO catalogy);
    }
}
