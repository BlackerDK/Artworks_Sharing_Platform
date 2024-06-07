using Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Services
{
    public interface IPackageService
    {
        Task<IEnumerable<PackageDTO>> GetAllPackage();
        Task<ResponseDTO> AddPackage(PackageAddDTO package);
        Task<PackageDTO> GetPackageById(int id);
        Task<ResponseDTO> DetelePackage(int id);
        Task<ResponseDTO> UpdatePackage(int id, PackageDTO package);
    }
}
