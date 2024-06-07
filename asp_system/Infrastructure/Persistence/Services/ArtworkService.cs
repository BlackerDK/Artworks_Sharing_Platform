using Application.Interfaces;
using Application.Interfaces.Services;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Domain.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Services
{
    public class ArtworkService : IArtworkService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        public ArtworkService(IUnitOfWork unitOfWork, IMapper mapper, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userManager = userManager;
        }

        public Task<ResponseDTO> AddArtwork(ArtworkAddDTO artwork)
        {
            try
            {
                var user = _userManager.FindByIdAsync(artwork.UserId).Result;
                var newArtwork = new Artwork
                {
                    Title = artwork.Title,
                    Description = artwork.Description,
                    Price = artwork.Price,
                    ReOrderQuantity = artwork.ReOrderQuantity,
                    Status = ArtWorkStatus.PendingConfirmation,
                    CreateOn = DateTime.Now,
                    UpdateOn = DateTime.Now,
                    User = user
                };
                _unitOfWork.Repository<Artwork>().AddAsync(newArtwork);
                _unitOfWork.Save();

                ArtworkDTO artworkDTO = new ArtworkDTO
                {
                    ArtworkId = newArtwork.ArtworkId,
                    Title = newArtwork.Title,
                    Description = newArtwork.Description,
                    Price = newArtwork.Price,
                    ReOrderQuantity = newArtwork.ReOrderQuantity
                };

                //Add Artwork Images
                if (artwork.ImagesUrl != null && artwork.ImagesUrl.Any())
                {
                    foreach (var image in artwork.ImagesUrl)
                    {
                        var newImage = new ArtworkImage
                        {
                            ArtworkId = newArtwork.ArtworkId,
                            Image = image
                        };
                        _unitOfWork.Repository<ArtworkImage>().AddAsync(newImage);
                        _unitOfWork.Save();
                    }
                }
                //Add Artwork Categories
                if (artwork.CategoryIds != null && artwork.CategoryIds.Any())
                {
                    foreach (var category in artwork.CategoryIds)
                    {
                        var newArtworkCategory = new ArtworkHasCategory
                        {
                            ArtworkId = newArtwork.ArtworkId,
                            CategoryId = category
                        };
                        _unitOfWork.Repository<ArtworkHasCategory>().AddAsync(newArtworkCategory);
                        _unitOfWork.Save();
                    }
                }


                return Task.FromResult(new ResponseDTO { IsSuccess = true, Message = "Artwork added successfully", Data = artworkDTO });
            }
            catch (Exception ex)
            {
                return Task.FromResult(new ResponseDTO { IsSuccess = false, Message = ex.Message });
            }

        }

        public async Task<IEnumerable<ArtworkDTO>> GetAllArtworks()
        {
            var ArtworkList = await _unitOfWork.Repository<Artwork>().GetAllAsync();
            var ArtworkDTOList = _mapper.Map<List<ArtworkDTO>>(ArtworkList);
            foreach (var artwork in ArtworkDTOList)
            {
                artwork.ImageUrl = await _unitOfWork.Repository<ArtworkImage>().GetQueryable().Where(a => a.ArtworkId == artwork.ArtworkId).Select(a => a.Image).ToListAsync();
                artwork.UserId= await _unitOfWork.Repository<Artwork>().GetQueryable().Where(a => a.ArtworkId == artwork.ArtworkId).Select(a => a.User.Id).FirstOrDefaultAsync();
                artwork.Categories = await _unitOfWork.Repository<ArtworkHasCategory>().GetQueryable().Where(a => a.ArtworkId == artwork.ArtworkId).Select(a => a.CategoryId).ToListAsync();
            }

            return ArtworkDTOList;
        }

        public async Task<IEnumerable<ArtworkDTO>> GetArtworkByFilter(ArtworkFilterParameterDTO filter)
        {
            var filterExpression = BuildFilterExpression(filter);

            
            var artworks = await _unitOfWork.Repository<Artwork>().GetByConditionAsync(
                filter: filterExpression,
                orderBy: q => q.OrderBy(a => a.ArtworkId), // Example ordering by ID
                pageIndex: filter.PageNumber - 1,   // Adjusting to zero-based index
                pageSize: filter.PageSize
            );

            //return _mapper.Map<List<ArtworkDTO>>(artworks);
            var ArtworkDTOList = _mapper.Map<List<ArtworkDTO>>(artworks);
            foreach (var artwork in ArtworkDTOList)
            {
                artwork.ImageUrl = await _unitOfWork.Repository<ArtworkImage>().GetQueryable().Where(a => a.ArtworkId == artwork.ArtworkId).Select(a => a.Image).ToListAsync();
                artwork.UserId = await _unitOfWork.Repository<Artwork>().GetQueryable().Where(a => a.ArtworkId == artwork.ArtworkId).Select(a => a.User.Id).FirstOrDefaultAsync();
            }
            return ArtworkDTOList;
        }

        private Expression<Func<Artwork, bool>> BuildFilterExpression(ArtworkFilterParameterDTO filter)
        {
            Expression<Func<Artwork, bool>> filterExpression = artwork => true;

            // Add conditions based on the filter parameters
            if (!string.IsNullOrEmpty(filter.txtSearch))
            {
                filterExpression = filterExpression.And(artwork => artwork.Title.Contains(filter.txtSearch) 
                ||artwork.Description.Contains(filter.txtSearch));
            }

            if (filter.MinPrice.HasValue)
            {
                filterExpression = filterExpression.And(artwork => artwork.Price >= filter.MinPrice.Value);
            }

            if (filter.MaxPrice.HasValue)
            {
                filterExpression = filterExpression.And(artwork => artwork.Price <= filter.MaxPrice.Value);
            }

            return filterExpression;
        }

        public async Task<ArtworkDTO> GetArtworkById(int id)
        {
            var Artwork = await _unitOfWork.Repository<Artwork>().GetByIdAsync(id);
            ArtworkDTO artworkDTO = _mapper.Map<ArtworkDTO>(Artwork);
            artworkDTO.ImageUrl = await _unitOfWork.Repository<ArtworkImage>().GetQueryable().Where(a => a.ArtworkId == artworkDTO.ArtworkId).Select(a => a.Image).ToListAsync();
            artworkDTO.UserId = await _unitOfWork.Repository<Artwork>().GetQueryable().Where(a => a.ArtworkId == id).Select(a => a.User.Id).FirstOrDefaultAsync();
            artworkDTO.Categories = await _unitOfWork.Repository<ArtworkHasCategory>().GetQueryable().Where(a => a.ArtworkId == artworkDTO.ArtworkId).Select(a => a.CategoryId).ToListAsync();

            return artworkDTO;
        }

        public   async Task<ResponseDTO> UpdateArtwork(ArtworkUpdateDTO artwork)
        {
            try
            {
                var existingArtwork = _unitOfWork.Repository<Artwork>().GetQueryable().FirstOrDefault(a => a.ArtworkId == artwork.ArtworkId);
                if (existingArtwork == null)
                {
                    return (new ResponseDTO { IsSuccess = false, Message = "Artwork not found" });
                }
                existingArtwork = submitCourseChange(existingArtwork, artwork);
                await _unitOfWork.Repository<Artwork>().UpdateAsync(existingArtwork);
                _unitOfWork.Save();
                return (new ResponseDTO { IsSuccess = true, Message = "Artwork updated successfully", Data = artwork });
            }
            catch (Exception ex)
            {
                return (new ResponseDTO { IsSuccess = false, Message = ex.Message });
            }
        }

        private Artwork submitCourseChange(Artwork existingArtwork, ArtworkUpdateDTO artwork)
        {
            //existingArtwork.Title = artwork.Title;
            //existingArtwork.Description = artwork.Description;
            //existingArtwork.Price = artwork.Price;
            //existingArtwork.ReOrderQuantity = artwork.ReOrderQuantity;
            existingArtwork.Status = artwork.Status;
            //existingArtwork.UpdateOn = DateTime.Now;

            return existingArtwork;
        }

        public async Task<string> GetUserIdByArtworkId(int id)
        {
            var user = await _unitOfWork.Repository<Artwork>().GetQueryable().Where(a => a.ArtworkId == id).Select(a => a.User.Id).FirstOrDefaultAsync();
            return user;
        }

        public async Task<IEnumerable<ArtworkDTO>> GetByCategory(int categoryId)
        {
            var artworkList = _unitOfWork.Repository<ArtworkHasCategory>().GetQueryable().Where(a => a.CategoryId == categoryId).Select(a => a.Artwork).ToList();
            var ArtworkDTOList = _mapper.Map<List<ArtworkDTO>>(artworkList);
            foreach (var artwork in ArtworkDTOList)
            {
                artwork.ImageUrl = await _unitOfWork.Repository<ArtworkImage>().GetQueryable().Where(a => a.ArtworkId == artwork.ArtworkId).Select(a => a.Image).ToListAsync();
                artwork.UserId = await _unitOfWork.Repository<Artwork>().GetQueryable().Where(a => a.ArtworkId == artwork.ArtworkId).Select(a => a.User.Id).FirstOrDefaultAsync();
            }

            return ArtworkDTOList;
        }

		public async Task<IEnumerable<ArtworkDTO>> GetAllArtworkByUserID(string userId)
		{
			var artworkList = _unitOfWork.Repository<Artwork>().GetQueryable().Where(a => a.User.Id == userId).ToList();
            var ArtworkDTOList = _mapper.Map<List<ArtworkDTO>>(artworkList);
            foreach (var artwork in ArtworkDTOList)
            {
				artwork.ImageUrl = await _unitOfWork.Repository<ArtworkImage>().GetQueryable().Where(a => a.ArtworkId == artwork.ArtworkId).Select(a => a.Image).ToListAsync();
			}
            return ArtworkDTOList;
		}

        public Task<IEnumerable<ArtworkDTO>> GetAllArtworkByStatus(ArtWorkStatus status)
        {
            var artworkList = _unitOfWork.Repository<Artwork>().GetQueryable().Where(a => a.Status == status).ToList();
            var ArtworkDTOList = _mapper.Map<List<ArtworkDTO>>(artworkList);
            foreach (var artwork in ArtworkDTOList)
            {
                artwork.ImageUrl = _unitOfWork.Repository<ArtworkImage>().GetQueryable().Where(a => a.ArtworkId == artwork.ArtworkId).Select(a => a.Image).ToList();
                artwork.UserId = _unitOfWork.Repository<Artwork>().GetQueryable().Where(a => a.ArtworkId == artwork.ArtworkId).Select(a => a.User.Id).FirstOrDefault();
                artwork.Categories = _unitOfWork.Repository<ArtworkHasCategory>().GetQueryable().Where(a => a.ArtworkId == artwork.ArtworkId).Select(a => a.CategoryId).ToList();
            }
            return Task.FromResult((IEnumerable<ArtworkDTO>)ArtworkDTOList);
        }
    }
    public static class ExpressionExtensions
    {
        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> left, Expression<Func<T, bool>> right)
        {
            var parameter = Expression.Parameter(typeof(T));
            var combined = Expression.AndAlso(
                Expression.Invoke(left, parameter),
                Expression.Invoke(right, parameter)
            );
            return Expression.Lambda<Func<T, bool>>(combined, parameter);
        }
    }
}
