using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.IdentityModel.Tokens;
using MoviesWebApi.DTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MoviesWebApi.Controllers
{
  [ApiController]
  [Route("api/accounts")]
  public class AccountsController:CustomBaseController
  {
    private readonly UserManager<IdentityUser> userManager;
    private readonly SignInManager<IdentityUser> signInManager;
    private readonly IConfiguration configuration;
    private readonly ApplicationDbContext dbContext;

    public AccountsController(
        UserManager<IdentityUser> userManager,
        SignInManager<IdentityUser> signInManager,
        IConfiguration configuration,
        ApplicationDbContext dbContext,
        IMapper mapper
        ):base(dbContext: dbContext, mapper:mapper)
    {
      this.userManager = userManager;
      this.signInManager = signInManager;
      this.configuration = configuration;
      this.dbContext = dbContext;
    }

    /// <summary>
    /// Create a new user account.
    /// </summary>
    /// <param name="userCredentials">Credentials for the new account.</param>
    /// <returns></returns>
    [AllowAnonymous]
    [HttpPost("users", Name = "createUserV1")]
    public async Task<ActionResult<AccountCreationResponseDTO>> CreateUser(UserCredentialsDTO userCredentials)
    {
      IdentityUser user = new IdentityUser()
      {
        UserName = userCredentials.Email,
        Email = userCredentials.Email
      };

      var creationResult = await userManager.CreateAsync(user, userCredentials.Password);

      if (creationResult.Succeeded)
      {
        return await BuildToken(userCredentials);
      }
      else
      {
        return BadRequest(creationResult.Errors);
      }
    }

    [HttpGet("users")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
    public async Task<ActionResult<List<UserDTO>>> GetUsers([FromQuery] PaginationDTO paginationDTO)
    {
      IQueryable<IdentityUser> queryable = dbContext.Users.AsQueryable();
      queryable = queryable.OrderBy(x => x.Email);
      return await Get<IdentityUser, UserDTO>();
    }

    [HttpGet("roles")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
    public async Task<ActionResult<List<string>>> GetRoles()
    {
      return await dbContext.Roles.Select(x => x.Name).ToListAsync();
    }

    [HttpPost("roles")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
    public async Task<ActionResult> SetRole(EditRoleDTO editRoleDTO)
    {
      IdentityUser user = await userManager.FindByIdAsync(editRoleDTO.UserId);
      if(user == null)
      {
        return NotFound();
      }

      await userManager.AddClaimAsync(user, new Claim(ClaimTypes.Role,editRoleDTO.RoleName));
      return NoContent();
    }

    [HttpPost("roles/remove")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
    public async Task<ActionResult> RemoveRole(EditRoleDTO editRoleDTO)
    {
      IdentityUser user = await userManager.FindByIdAsync(editRoleDTO.UserId);
      if (user == null) {
        return NotFound();
      }

      await userManager.RemoveClaimAsync(user, new Claim(ClaimTypes.Role, editRoleDTO.RoleName));
      return NoContent();
    }

    /// <summary>
    /// Log in with your user account.
    /// </summary>
    /// <param name="userCredentialsDTO">Your user account credentials.</param>
    /// <returns></returns>
    [AllowAnonymous]
    [HttpPost("login", Name = "createLoginV1")]
    public async Task<ActionResult<AccountCreationResponseDTO>> Login(UserCredentialsDTO userCredentialsDTO)
    {
      var result = await signInManager.PasswordSignInAsync(
          userName: userCredentialsDTO.Email, password: userCredentialsDTO.Password,
          isPersistent: false, lockoutOnFailure: false
      );

      if (result.Succeeded)
      {
        return await BuildToken(userCredentialsDTO);
      }

      return BadRequest("Incorrect login.");
    }

    [AllowAnonymous]
    [HttpGet("refresh-token", Name = "getRefreshTokenV1")]
    public async Task<ActionResult<AccountCreationResponseDTO>> RefreshToken()
    {
      Claim emailClaim = HttpContext.User.Claims.Where(claim => claim.Type == "email").FirstOrDefault();

      if (emailClaim == null)
      {
        return NotFound("Please log in and try again.");
      }

      string email = emailClaim.Value;

      return await BuildToken(new UserCredentialsDTO
      {
        Email = email
      });

    }

    private async Task<AccountCreationResponseDTO> BuildToken(UserCredentialsDTO userCredentials)
    {
      List<Claim> claims = new List<Claim>()
            {
                new Claim("email",userCredentials.Email)
            };

      IdentityUser user = await userManager.FindByEmailAsync(userCredentials.Email);
      claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id));

      var claimsDb = await userManager.GetClaimsAsync(user);
      claims.AddRange(claimsDb);

      var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["jwt:key"]));
      var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
      DateTime expiration = DateTime.UtcNow.AddMinutes(30);

      var securityToken = new JwtSecurityToken(
          issuer: null, audience: null, claims: claims, expires: expiration, signingCredentials: creds
      );

      return new AccountCreationResponseDTO()
      {
        Token = new JwtSecurityTokenHandler().WriteToken(securityToken),
        Expiration = expiration,
      };

    }

    [HttpPost("set-admin", Name = "createAdminV1")]
    public async Task<ActionResult> SetAdmin(UserAdminEditDTO userAdminEditDTO)
    {
      IdentityUser user = await userManager.FindByEmailAsync(userAdminEditDTO.Email);

      if (user == null)
      {
        return NotFound($"Don't exist a user with email {userAdminEditDTO.Email}.");
      }

      await userManager.AddClaimAsync(user, new Claim("isAdmin", "1"));
      return NoContent();
    }

    [HttpPost("remove-admin", Name = "deleteAdminV1")]
    public async Task<ActionResult> DeleteAdmin(UserAdminEditDTO userAdminEditDTO)
    {
      IdentityUser user = await userManager.FindByEmailAsync(userAdminEditDTO.Email);

      if (user == null)
      {
        return NotFound($"Don't exist a user with email {userAdminEditDTO.Email}.");
      }

      await userManager.RemoveClaimAsync(user, new Claim("isAdmin", "1"));
      return NoContent();
    }

  }
}
