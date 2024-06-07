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
    public class CatalogyService : ICatalogyService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CatalogyService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public Task<ResponseDTO> AddCatalogy(CatalogyAddDTO catalogy)
        {
            try
            {
                var newCatalogy = _mapper.Map<Category>(catalogy);
                _unitOfWork.Repository<Category>().AddAsync(newCatalogy);
                _unitOfWork.Save();
                //get id of data after add
                return Task.FromResult(new ResponseDTO { IsSuccess = true, Message = "Catalogy added successfully", Data = newCatalogy });
            }catch (Exception ex)
            {
                return Task.FromResult(new ResponseDTO { IsSuccess = false, Message = ex.Message });
            }
        }

        public Task<ResponseDTO> DeteleCatalogy(int id)
        {
            try
            {   
                var checkId = _unitOfWork.Repository<Category>().GetQueryable().FirstOrDefault(c => c.Id == id);
                if (checkId == null)
                {
                    return Task.FromResult(new ResponseDTO { IsSuccess = false, Message = "Catalogy not found" });
                }
                else
                {
                    var update = _mapper.Map<Category>(checkId);
                    update.Status = false;
                    _unitOfWork.Repository<Category>().UpdateAsync(update);
                    _unitOfWork.Save();
                    return Task.FromResult(new ResponseDTO { IsSuccess = true, Message = "Catalogy delete successfully", Data = checkId });
                }
            }
            catch (Exception ex)
            {
                return Task.FromResult(new ResponseDTO { IsSuccess = false, Message = ex.Message });
            }
        }

        public async Task<IEnumerable<CatalogyDTO>> GetAllCatalogy()
        {
            var result = await _unitOfWork.Repository<Category>().GetAllAsync();
            return _mapper.Map<List<CatalogyDTO>>(result); // Take to list with Mapper
        }

        public async Task<CatalogyDTO> GetCatalogyById(int id)
        {
            var result = await _unitOfWork.Repository<Category>().GetByIdAsync(id);
            return _mapper.Map<CatalogyDTO>(result);
        }

        public  Task<ResponseDTO> UpdateCatalogy(int id,CatalogyDTO catalogy)
        {
            try
            {
                if (id != catalogy.Id)
                {
                    return  Task.FromResult(new ResponseDTO { IsSuccess = false, Message = "Catalogy not found" });
                }
                else
                {
                    var update = _mapper.Map<Category>(catalogy);
                    _unitOfWork.Repository<Category>().UpdateAsync(update);
                    _unitOfWork.Save();
                    return  Task.FromResult(new ResponseDTO { IsSuccess = true, Message = "Catalogy updated successfully", Data = catalogy});
                }
            }
            catch (Exception ex)
            {
                return  Task.FromResult(new ResponseDTO { IsSuccess = false, Message = ex.Message });
            }
        }
    }
}
