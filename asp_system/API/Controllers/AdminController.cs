using API.Helper;
using API.Service;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Domain.Model;
using Firebase.Auth;
using Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace API.Controllers
{
    [Authorize(Roles = AppRole.Admin)]
    [Route("api/admin/")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly ILogger<AdminController> _logger;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        public AdminController(ILogger<AdminController> logger, ApplicationDbContext context,
            RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager, IMapper mapper)
        {
            _userManager = userManager;
            _context = context;
            _logger = logger;
            _roleManager = roleManager;
            _mapper = mapper;

        }
        #region RoleManagement

        #region CRUDRole
        [HttpGet("getRole")]
        public async Task<IActionResult> GetRole()
        {
            var roles = await _roleManager.Roles.OrderBy(_ => _.Name).ToListAsync();
            List<string> roleName = new List<string>();
            if (roles.Count > 0)
            {
                var result = roles.Select(_ => new
                {
                    _.Id,
                    _.Name
                });
                return Ok(result);
            }
            return Ok(new { ProcessStatus.RelateEntity });
        }
        //[HttpGet("getRoleName")]
        //public async Task<IActionResult> GetRoleName()
        //{
        //    var roles = await _roleManager.Roles.OrderBy(_ => _.Name).ToListAsync();
        //    List<string> roleName = new List<string>();
        //    if (roles.Count > 0)
        //    {
               
        //        foreach (var itemRole in roles)
        //        {
        //            roleName.Add(itemRole.ToString());
        //        }
               
        //        return Ok(roleName);
        //    }
        //    return Ok(new { ProcessStatus.RelateEntity });
        //}

        [HttpGet("getRoleBy/{id}")]
        public async Task<IActionResult> GetRoleById(String id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role != null)
            {
                return Ok(role);
            }
            else
                return Ok(new { ProcessStatus.NotFound });
        }

        [HttpPost("createRole")]
        public async Task<IActionResult> CreateRole(String roleName)
        {
            IdentityRole _roleName = new IdentityRole(roleName);
            var result = await _roleManager.CreateAsync(_roleName);
            if (result.Succeeded)
            {
                return Ok(new { ProcessStatus.Success });
            }
            else
                return Ok(new { ProcessStatus.Fail });
        }

        [HttpPut("updateRole/{id}")]
        public async Task<IActionResult> UpdateRole(String roleName, String id)
        {
            try
            {
                var role = await _roleManager.FindByIdAsync(id);
                if (role != null)
                {
                    role.Name = roleName;
                    _context.Roles.Update(role);
                    var result = await _context.SaveChangesAsync();
                    return Ok(new { ProcessStatus.Success });
                }
                else
                    return Ok(new { ProcessStatus.NotFound });
            }
            catch (Exception)
            {
                return Ok(new { ProcessStatus.Fail });
            }
        }

        [HttpDelete("deleteRole")]
        public async Task<IActionResult> DeleteRole(String roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role != null)
            {
                var result = await _roleManager.DeleteAsync(role);
                if (result.Succeeded)
                {
                    return Ok(new { ProcessStatus.Success });
                }
                else
                {
                    return Ok(new { ProcessStatus.Fail });
                }
            }
            else
                return Ok(new { ProcessStatus.NotFound });
        }
        #endregion

        #region UserRoles

        [HttpGet("getUserRole")]
        public async Task<IActionResult> GetListUsers([FromQuery] DefaultSearch defaultSearch)
        {
            var userTotal = await _context.Users.Select(_ => new UserRoles { Id = _.Id }).ToListAsync();
            var users = await _context.Users.Select(_ => new UserRoles
            {
                Id = _.Id,
                UserName = _.UserName,
                Birthday = _.Birthday,
                Email = _.Email,
                IsActive = _.IsActive,
                FirstName = _.FirstName,
                LastName = _.LastName,
            }).Sort(string.IsNullOrEmpty(defaultSearch.sortBy) ? "UserName" : defaultSearch.sortBy
                      , defaultSearch.isAscending)
                      .ToPageList(defaultSearch.currentPage, defaultSearch.perPage).AsNoTracking().ToListAsync();
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                user.RolesName = roles.ToList<string>();
            }
            var result = users.Select(_ => _mapper.Map<UserRoles, UserRolesVM>(_));
            return Ok(new { total = userTotal.Count(), data = result, page = defaultSearch.currentPage });
        }
        [HttpGet("getUserRole/{userId}")]
        public async Task<IActionResult> GetUserRole(String userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                var userRoles = (await _userManager.GetRolesAsync(user)).ToArray<string>();
                return Ok(new { userRoles });
            }
            return Ok(new { ProcessStatus.NotFound });
        }


        [HttpPost("addUserRole")]
        public async Task<IActionResult> AddRoleUser(List<string> roleNames, String userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                var userRoles = (await _userManager.GetRolesAsync(user)).ToArray<string>();
                var deleteRoles = userRoles.Where(r => !roleNames.Contains(r));
                var addRoles = roleNames.Where(r => !userRoles.Contains(r));
                var result = await _userManager.RemoveFromRolesAsync(user, deleteRoles);
                if (!result.Succeeded)
                {
                    return Ok(ProcessStatus.Fail);
                }
                result = await _userManager.AddToRolesAsync(user, addRoles);
                if (result.Succeeded)
                {
                    return Ok(ProcessStatus.Success);
                }
            }
            return Ok(new { ProcessStatus.NotFound });
        }

        [HttpPost("changeUserStatus")]
        public async Task<IActionResult> EnalbleUser(String userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                user.IsActive = !(user.IsActive);
                _context.Update(user);
                int result = _context.SaveChanges();
                if (result > 0)
                {
                    return Ok(ProcessStatus.Success);
                }
            }
            return Ok(new { ProcessStatus.NotFound });
        }

        #endregion


        #endregion
    }
}

