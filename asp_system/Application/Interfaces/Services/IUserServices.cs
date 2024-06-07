using Domain.Entities;
using Domain.Model;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Services
{
    public interface IUserServices
    {
        public Task<IdentityResult> SignUpAsync(UserSignUpDTO model);
        //public Task<SignInResult> SignInAsync(UserSignInDTO model);
        public Task<ApplicationUser> SignInAsync(UserSignInDTO model);
        //public Task LogOutAsync();
        public Task<ProfileUserDTO> GetUserByIDlAsync(string id);
        public Task<IEnumerable<UserDTO>> GetAllUsers();

        public Task<ApplicationUser> ExternalLoginAsync(UserSignUpDTO model);
    }
}
