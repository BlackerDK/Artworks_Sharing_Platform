using Application.Interfaces;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using AutoMapper;
using Domain.Entities;
using Domain.Model;
using Infrastructure.Persistence.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Services
{
	public class CommentService : ICommentService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;
		private readonly UserManager<ApplicationUser> _userManager;

		public CommentService(IUnitOfWork unitOfWork, IMapper mapper, UserManager<ApplicationUser> user)
		{
			_unitOfWork = unitOfWork;
			_mapper = mapper;
			_userManager = user;
		}
		public async Task<IEnumerable<CommentDTO>> GetAllCommentByArtwork(int ArtworkId)
		{
			var CheckArt = _unitOfWork.Repository<Comment>().GetQueryable().Where(p => p.Artwork.ArtworkId == ArtworkId).ToList();
			var CmtDTO = _mapper.Map<List<CommentDTO>>(CheckArt);
			if (CmtDTO != null)
			{
				foreach (var cmt in CmtDTO)
				{
					cmt.ArtworkId = ArtworkId;
					cmt.UserId = _unitOfWork.Repository<Comment>().GetQueryable().Where(p => p.Artwork.ArtworkId == ArtworkId).Select(p => p.User.Id).FirstOrDefault();
				}
				return CmtDTO;
			}
			else
			{
				return null;
			}
		}
		public async Task<ResponseDTO> CreateComment(CommentAddDTO cmt)
		{
			try
			{
				var CheckArt = _unitOfWork.Repository<Artwork>().GetQueryable().Where(p => p.ArtworkId == cmt.ArtworkId).FirstOrDefault();
				var CheckUser = await _userManager.FindByIdAsync(cmt.UserId);
				if (CheckUser != null && CheckArt != null)
				{
					var newCmt = _mapper.Map<Comment>(cmt);
					newCmt.Artwork = CheckArt;
					newCmt.User = CheckUser;
					await _unitOfWork.Repository<Comment>().AddAsync(newCmt);
					_unitOfWork.Save();
					return new ResponseDTO { IsSuccess = true, Message = "Comment added successfully", Data = cmt };
				}
				else
				{
					return new ResponseDTO { IsSuccess = false, Message = "Comments added fail" };
				}
			}
			catch (Exception ex)
			{
				return new ResponseDTO { IsSuccess = false, Message = ex.Message };
			}
		}

		public async Task<ResponseDTO> DeleteComent(CommentAddDTO CommentId)
		{
			var CheckId = _unitOfWork.Repository<Comment>().GetQueryable().Where(p=>p.User.Id == CommentId.UserId && p.Artwork.ArtworkId == CommentId.ArtworkId).FirstOrDefault();
			if (CheckId != null)
			{
				await _unitOfWork.Repository<Comment>().DeleteAsync(CheckId);
				_unitOfWork.Save();
				return new ResponseDTO { IsSuccess = true, Message = "Comment delete successfully" };
			}
			else
			{
				return new ResponseDTO { IsSuccess = false, Message = "Comments delete fail" };
			}
		}

		public async Task<ResponseDTO> UpdateComment(CommentUpdateDTO cmt)
		{
			var ckeckId = _unitOfWork.Repository<Comment>().GetQueryable().Where(p=>p.Id == cmt.Id).FirstOrDefault();
			if (ckeckId != null)
			{
				var result = _mapper.Map<Comment>(ckeckId);
				result.Content = cmt.Content;
				_unitOfWork.Repository<Comment>().UpdateAsync(result);
				_unitOfWork.Save();
				return  new ResponseDTO { IsSuccess = true, Message = "Comment update successfully" };
			}
			else
			{
				return new ResponseDTO { IsSuccess = false, Message = "Comments added fail" };
			}
		}
	}
}
