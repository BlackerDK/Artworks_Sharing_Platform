using Application.Interfaces;
using Application.Interfaces.Services;
using AutoMapper;
using Domain.Entities;
using Domain.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Services
{
    public class LikeService : ILikeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        public LikeService(IUnitOfWork unitOfWork, IMapper mapper, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userManager = userManager;
        }

        public async Task<ResponseDTO> CreateLike(LikeCreateDTO like)
        {           
            try
            {

				var CheckId = _unitOfWork.Repository<Like>().GetQueryable().Where(p => p.User.Id == like.UserId && p.Artwork.ArtworkId == like.ArtworkId).FirstOrDefault();
				if (CheckId != null)
				{
					await _unitOfWork.Repository<Like>().DeleteAsync(CheckId);
					_unitOfWork.Save();
					return new ResponseDTO { IsSuccess = true, Message = "Like delete successfully" };
                }
                else
                {
					var CheckArt = _unitOfWork.Repository<Artwork>().GetQueryable().Where(p => p.ArtworkId == like.ArtworkId).FirstOrDefault();
					var CheckUser = await _userManager.FindByIdAsync(like.UserId);
					if (CheckUser != null && CheckArt != null)
					{
						var newlike = _mapper.Map<Like>(like);
						newlike.Artwork = CheckArt;
						newlike.User = CheckUser;
						await _unitOfWork.Repository<Like>().AddAsync(newlike);
						_unitOfWork.Save();
						return new ResponseDTO { IsSuccess = true, Message = "Like added successfully", Data = like };
					}
					else
					{
						return new ResponseDTO { IsSuccess = false, Message = "Likes added fail" };
					}
				}
				
            }
            catch (Exception ex)
            {
                return new ResponseDTO { IsSuccess = false, Message = ex.Message };
            }

        }
        public async Task<IEnumerable<LikeDTO>> GetAllLikeByArtworkId(int ArtworkId)
        {
            //var LikeList = await _unitOfWork.Repository<Like>().GetAllAsync();
            //var LikeDTOList = _mapper.Map<List<LikeDTO>>(LikeList);
            //foreach (var like in LikeDTOList)
            //{
            //    like.UserId = await _unitOfWork.Repository<Like>().GetQueryable().Where(a => a.Id == like.Id).Select(a => a.User.Id).FirstOrDefaultAsync();
            //}
            //return LikeDTOList;
            var CheckArt = _unitOfWork.Repository<Like>().GetQueryable().Where(p => p.Artwork.ArtworkId == ArtworkId).ToList();
            var likeDTO = _mapper.Map<List<LikeDTO>>(CheckArt);
            if (likeDTO != null)
            {
                foreach (var like in likeDTO)
                {
                    like.ArtworkId = ArtworkId;
                    like.UserId = _unitOfWork.Repository<Like>().GetQueryable().Where(p => p.Artwork.ArtworkId == ArtworkId).Select(p => p.User.Id).FirstOrDefault();
                }
                return likeDTO;
            }
            else
            {
                return null;
            }
        }

        //public async Task<ResponseDTO> DeleteLike(LikeCreateDTO LikeId)
        //{
        //    //try
        //    //{
        //    //    var existingLike = _unitOfWork.Repository<Like>().GetQueryable().FirstOrDefault(a => a.Id == LikeId);
        //    //    if (existingLike == null)
        //    //    {
        //    //        return (new ResponseDTO { IsSuccess = false, Message = "Like not found" });
        //    //    }
        //    //    existingLike = submitCourse(existingLike, like);
        //    //    await _unitOfWork.Repository<Like>().DeleteAsync(existingLike);
        //    //    _unitOfWork.Save();
        //    //    return (new ResponseDTO { IsSuccess = true, Message = "Like deleted successfully", Data = like });
        //    //}
        //    //catch (Exception ex)
        //    //{
        //    //    return (new ResponseDTO { IsSuccess = false, Message = ex.Message });
        //    //}
        //    var CheckId = _unitOfWork.Repository<Like>().GetQueryable().Where(p=>p.User.Id == LikeId.UserId && p.Artwork.ArtworkId == LikeId.ArtworkId).FirstOrDefault();
        //    if (CheckId != null)
        //    {
        //        await _unitOfWork.Repository<Like>().DeleteAsync(CheckId);
        //        _unitOfWork.Save();
        //        return new ResponseDTO { IsSuccess = true, Message = "Like delete successfully" };
        //    }
        //    else
        //    {
        //        return new ResponseDTO { IsSuccess = false, Message = "Likes delete fail" };
        //    }
        //}

        private Like submitCourse(Like existingLike, LikeDeleteDTO like)
        {
            
            existingLike.Id = like.Id;

            return existingLike;
        }
    }
}
