using Application.Interfaces;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using AutoMapper;
using AutoMapper.Configuration.Annotations;
using Domain.Entities;
using Domain.Model;
using Firebase.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Services
{
    public class PosterService : IPosterService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;

        public PosterService(IUnitOfWork unitOfWork, IMapper mapper, UserManager<ApplicationUser> user)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userManager = user;
        }

        public async Task<ResponseDTO> AddPoster(PosterAddDTO post)
        {
            try
            {               
                var checkId = _unitOfWork.Repository<Package>().GetQueryable().FirstOrDefault(c => c.Id == post.PackageId);
                var user = await _userManager.FindByIdAsync(post.UserId);
                
                if (checkId != null && user !=null)
                {
                    var CheckPostId = _unitOfWork.Repository<Poster>().GetQueryable().FirstOrDefault(p => p.User.Id == post.UserId);
                    if (CheckPostId != null)
                    {
                        var update = _mapper.Map<Poster>(CheckPostId);
                        update.QuantityPost = update.QuantityPost + checkId.Quantity;
                        update.PackageId = post.PackageId;
                        _unitOfWork.Repository<Poster>().UpdateAsync(update);
                        _unitOfWork.Save();
                    }
                    else
                    {
                        var newPost = _mapper.Map<Poster>(post);
                        newPost.User = user;
                        newPost.QuantityPost = checkId.Quantity;
                         _unitOfWork.Repository<Poster>().AddAsync(newPost);
                        _unitOfWork.Save();
                    }
                    return new ResponseDTO { IsSuccess = true, Message = "Poster added successfully", Data = post };

                }
                else
                {
                  return  new ResponseDTO { IsSuccess = false, Message = "Poster added fail Package" };
                }
            }
            catch (Exception ex)
            {
                 return new ResponseDTO { IsSuccess = false, Message = ex.Message };
            }
        }

        public Task<ResponseDTO> DecreasePost(string userId)
        {
            try
            {
                var CheckQuantityPost = _unitOfWork.Repository<Poster>().GetQueryable().FirstOrDefault(p => p.User.Id == userId);
                if (CheckQuantityPost != null)
                {
                    var update = _mapper.Map<Poster>(CheckQuantityPost);
                    update.QuantityPost = update.QuantityPost - 1;
                    _unitOfWork.Repository<Poster>().UpdateAsync(update);
                    _unitOfWork.Save();
                    return Task.FromResult(new ResponseDTO { IsSuccess = true, Message = "Poster updated successfully", Data = CheckQuantityPost });
                }
                return Task.FromResult(new ResponseDTO { IsSuccess = false, Message = "Package not found" });
            }
            catch (Exception ex)
            {
                return Task.FromResult(new ResponseDTO { IsSuccess = false, Message = ex.Message });
            }
        }

        public async Task<IEnumerable<PosterDTO>> GetAllPoster()
        {
            var PosterList = await _unitOfWork.Repository<Poster>().GetAllAsync();
            var PosterDTOList = _mapper.Map<List<PosterDTO>>(PosterList);
            foreach (var post in PosterDTOList)
            {
                post.UserId = await _unitOfWork.Repository<Poster>().GetQueryable().Where(a => a.Id == post.Id).Select(a => a.User.Id).FirstOrDefaultAsync();
            }
            return PosterDTOList;
        }

        public async Task<PosterDTO> GetPosterByUserId(string UserId)
        {
           var result = _unitOfWork.Repository<Poster>().GetQueryable().FirstOrDefault(p=>p.User.Id == UserId);
           if(result != null && result.QuantityPost >= 1)
            {
                PosterDTO post = _mapper.Map<PosterDTO>(result);
                post.UserId=UserId;
                return post;
            }
            else
            {
                return null;
            }          
        }

		public Task<ResponseDTO> IncreasePost(string userId)
		{
			try
			{
				var CheckQuantityPost = _unitOfWork.Repository<Poster>().GetQueryable().FirstOrDefault(p => p.User.Id == userId);
				if (CheckQuantityPost != null)
				{
					var update = _mapper.Map<Poster>(CheckQuantityPost);
					update.QuantityPost = update.QuantityPost + 1;
					_unitOfWork.Repository<Poster>().UpdateAsync(update);
					_unitOfWork.Save();
					return Task.FromResult(new ResponseDTO { IsSuccess = true, Message = "Poster updated successfully", Data = CheckQuantityPost });
				}
				return Task.FromResult(new ResponseDTO { IsSuccess = false, Message = "Package not found" });
			}
			catch (Exception ex)
			{
				return Task.FromResult(new ResponseDTO { IsSuccess = false, Message = ex.Message });
			}
		}
	}
}
