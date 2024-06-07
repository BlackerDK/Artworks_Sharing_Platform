using Application.Interfaces;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using AutoMapper;
using Domain.Entities;
using Domain.Model;
using Firebase.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Services
{
    public class PackageService : IPackageService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PackageService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public Task<ResponseDTO> AddPackage(PackageAddDTO package)
        {
            try
            {
                var newPackage = _mapper.Map<Package>(package);
                _unitOfWork.Repository<Package>().AddAsync(newPackage);
                _unitOfWork.Save();
                return Task.FromResult(new ResponseDTO { IsSuccess = true, Message = "Package added successfully", Data = newPackage });
            }
            catch (Exception ex)
            {
                return Task.FromResult(new ResponseDTO { IsSuccess = false, Message = ex.Message });
            }
        }

        public Task<ResponseDTO> DetelePackage(int id)
        {
            try
            {
                var checkId = _unitOfWork.Repository<Package>().GetQueryable().FirstOrDefault(c => c.Id == id);
                if (checkId == null)
                {
                    return Task.FromResult(new ResponseDTO { IsSuccess = false, Message = "Package not found" });
                }
                else
                {
                    var update = _mapper.Map<Package>(checkId);
                    update.Status = false;
                    _unitOfWork.Repository<Package>().UpdateAsync(update);
                    _unitOfWork.Save();
                    return Task.FromResult(new ResponseDTO { IsSuccess = true, Message = "Package delete successfully", Data = update });
                }
            }
            catch (Exception ex)
            {
                return Task.FromResult(new ResponseDTO { IsSuccess = false, Message = ex.Message });
            }
        }

        public async Task<IEnumerable<PackageDTO>> GetAllPackage()
        {
            var result = await _unitOfWork.Repository<Package>().GetAllAsync();
            return _mapper.Map<List<PackageDTO>>(result);
        }

        public async Task<PackageDTO> GetPackageById(int id)
        {
            var result = await _unitOfWork.Repository<Package>().GetByIdAsync(id);
            return _mapper.Map<PackageDTO>(result);
        }

        public Task<ResponseDTO> UpdatePackage(int id, PackageDTO package)
        {
            try
            {
                if (id != package.Id)
                {
                    return Task.FromResult(new ResponseDTO { IsSuccess = false, Message = "Package not found" });
                }
                else
                {
                    var update = _mapper.Map<Package>(package);
                    _unitOfWork.Repository<Package>().UpdateAsync(update);
                    _unitOfWork.Save();
                    return Task.FromResult(new ResponseDTO { IsSuccess = true, Message = "Package updated successfully", Data = update });
                }
            }
            catch (Exception ex)
            {
                return Task.FromResult(new ResponseDTO { IsSuccess = false, Message = ex.Message });
            }
        }
    }
}
