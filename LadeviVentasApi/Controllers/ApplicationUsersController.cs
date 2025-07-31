namespace LadeviVentasApi.Controllers;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using KendoNET.DynamicLinq;
using LadeviVentasApi.Data;
using LadeviVentasApi.DTOs;
using LadeviVentasApi.Models;
using LadeviVentasApi.Models.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

[Route("api/[controller]")]
[ApiController]
public class ApplicationUsersController : RestController<ApplicationUser, ApplicationUserWritingDto>
{
    private SignInManager<IdentityUser> SignInManager { get; }
    private IEmailSender EmailSender { get; }
    private IConfiguration Configuration { get; }
    private UserManager<IdentityUser> UserManager { get; }
    private IOptions<Configuration> _options;

    public ApplicationUsersController(ApplicationDbContext context,
        IMapper mapper,
        SignInManager<IdentityUser> signInManager,
        IEmailSender emailSender,
        IConfiguration configuration,
        UserManager<IdentityUser> userManager,
        IOptions<Configuration> options) : base(context, mapper)
    {
        SignInManager = signInManager;
        EmailSender = emailSender;
        Configuration = configuration;
        UserManager = userManager;
        _options = options;
    }

    protected override IQueryable<ApplicationUser> GetQueryableWithIncludes()
    {
        EnsureUserRole(UsersRole.Superuser, UsersRole.Supervisor);
        var allUsers = base.GetQueryableWithIncludes()
            .Include(x => x.ApplicationRole)
            .Include(x => x.Country)
            .Include(x => x.CredentialsUser);

        return allUsers;
    }

    protected override DataSourceResult GetSearchDataSourceResult(KendoGridSearchRequestExtensions.KendoGridSearchRequest request)
    {
        var result = GetSearchQueryable()
            .Select(x => new
            {
                x.Id,
                x.ApplicationRoleId,
                x.ApplicationRole,
                x.CommisionCoeficient,
                x.Country,
                x.CountryId,
                x.CredentialsUser,
                x.CredentialsUserId,
                x.FullName,
                x.CredentialsUser.Email,
                x.Initials,
                x.ShouldDelete,
                CanDelete = !Context.Contracts.Any(c => c.SellerId == x.Id)
                    && !Context.Clients.Any(cl => cl.ApplicationUserSellerId == x.Id || cl.ApplicationUserDebtCollectorId == x.Id)
                    && !Context.PublishingOrders.Any(op => op.SellerId == x.Id)
            })
            .ToDataSourceResult(request);
        return result;
    }

    protected override void CleanFields(object x)
    {
        if (x is ApplicationUser u)
        {
            u.CredentialsUser.PasswordHash = null;
            u.CredentialsUser.SecurityStamp = null;
            u.CredentialsUser.ConcurrencyStamp = null;
        }
    }

    public override async Task<IActionResult> Post(ApplicationUserWritingDto x)
    {
        EnsureUserRole(UsersRole.Superuser);

        var user = new IdentityUser { UserName = x.Email, Email = x.Email };
        var identityResult = await UserManager.CreateAsync(user, x.Password);
        if (identityResult.Succeeded)
        {
            var applicationUser = new ApplicationUser
            {
                CountryId = x.CountryId,
                CredentialsUserId = user.Id,
                CommisionCoeficient = x.CommisionCoeficient,
                ApplicationRoleId = x.ApplicationRoleId,
                FullName = x.FullName,
                Initials = x.Initials,
                CredentialsUser = user
            };
            TryValidateModel(applicationUser);
            if (ModelState.IsValid)
            {
                Context.Add(applicationUser);
                var code = await UserManager.GenerateEmailConfirmationTokenAsync(user);
                Console.WriteLine("token {0} for user {1}", code, user.Id);
                var callbackUrl = Url.Action(nameof(Confirm), "ApplicationUsers", new { userId = user.Id, code },
                    Request.Scheme);
                await Context.SaveChangesAsync();
                // await EmailSender.SendEmailConfirmationAsync(x.Email, callbackUrl, code);
                x.Id = applicationUser.Id;

                #region Auditory
                try
                {
                    Auditory audit = new Auditory();
                    audit.Date = DateTime.Now;
                    audit.Entity = "Usuarios";
                    audit.UserId = CurrentAppUser.Value.Id;
                    audit.User = CurrentAppUser.Value.FullName;

                    audit.AuditMessage = "Creación de " + audit.Entity + ". Id=" + applicationUser.Id.ToString() + ". Nombre: " + x.FullName;

                    Context.Add(audit);
                    await Context.SaveChangesAsync();
                }
                //El catch vacio es simplemente para que un error aqui no interrumpa el proceso normal. Deberia ir algun tipo de log.
                catch (Exception ex)
                {
                }
                #endregion

                return Created(nameof(Post), x);

            }

            await UserManager.DeleteAsync(user);
            return ActionResultForModelStateValidation();
        }
        //identityResult.Errors.ToList().ForEach(e => ModelState.AddModelError("password", e.Description));
        foreach (var e in identityResult.Errors.ToList())
        {
            switch (e.Code)
            {
                case "InvalidUserName":
                    ModelState.AddModelError("email", e.Description);
                    break;
                case "PasswordTooShort":
                    ModelState.AddModelError("password", "El password debe contener al menos 6 caracteres. ");
                    break;
                case "PasswordRequiresNonAlphanumeric":
                    ModelState.AddModelError("password", "El password debe contener al menos un caracter no alfanúmerico. ");
                    break;
                case "PasswordRequiresLower":
                    ModelState.AddModelError("password", "El password debe contener al menos una minúscula. ");
                    break;
                case "PasswordRequiresUpper":
                    ModelState.AddModelError("password", "El password debe contener al menos una mayúscula. ");
                    break;
                case "PasswordRequiresUniqueChars":
                    ModelState.AddModelError("password", "El password debe usar al menos un caracter diferente. ");
                    break;
                case "PasswordRequiresDigit":
                    ModelState.AddModelError("password", "El password debe contener al menos un número. ");
                    break;
                default:
                    ModelState.AddModelError("password", e.Description);
                    break;
            }
        }
        return ActionResultForModelStateValidation();
    }

    /// <summary>
    /// Este metodo acepta que se modifiquen todos los datos salvo el mail, id y password
    /// </summary>
    public override async Task<IActionResult> Put(long id, ApplicationUserWritingDto x)
    {
        EnsureUserRole(UsersRole.Supervisor);

        var user = await UserManager.FindByEmailAsync(x.Email);
        var applicationUser = await Context.FindAsync<ApplicationUser>(id);

        if (user == null || applicationUser == null || user.Id != applicationUser.CredentialsUserId)
        {
            return BadRequest();
        }

        applicationUser.CountryId = x.CountryId;
        applicationUser.CredentialsUserId = user.Id;
        applicationUser.CommisionCoeficient = x.CommisionCoeficient;
        applicationUser.FullName = x.FullName;
        applicationUser.Initials = x.Initials;
        applicationUser.ApplicationRoleId = x.ApplicationRoleId;
        var resultUpdate = await PerformUpdate(id, applicationUser);

        if (!String.IsNullOrEmpty(x.Password))
        {
            bool isLongEnought = x.Password.Length >= 6;
            bool containUppercase = x.Password.Any(char.IsUpper);
            bool containNonAlphanumeric = x.Password.Any(c => !char.IsLetterOrDigit(c));
            bool containDigit = x.Password.Any(char.IsDigit);
            bool containLowercase = x.Password.Any(char.IsLower);
            bool isError = false;

            if (!isLongEnought)
            {
                ModelState.AddModelError("password", "El password debe contener al menos 6 caracteres. ");
                isError = true;
            }

            if (!containUppercase)
            {
                ModelState.AddModelError("password", "El password debe contener al menos una mayúscula. ");
                isError = true;
            }

            if (!containNonAlphanumeric)
            {
                ModelState.AddModelError("password", "El password debe contener al menos un caracter no alfanúmerico. ");
                isError = true;
            }

            if (!containDigit)
            {
                ModelState.AddModelError("password", "El password debe contener al menos un número. ");
                isError = true;
            }

            if (!containLowercase)
            {
                ModelState.AddModelError("password", "El password debe contener al menos una minúscula. ");
                isError = true;
            }

            if (isError)
            {
                return ActionResultForModelStateValidation();
            }

            var code = await UserManager.GeneratePasswordResetTokenAsync(user);
            var changePassResult = UserManager.ResetPasswordAsync(user, code, x.Password).Result;
        }

        return resultUpdate;
    }

    [HttpGet("confirm")]
    [AllowAnonymous]
    public async Task<IActionResult> Confirm(string userId, string code)
    {
        var errorDto = GetSimpleWsErrorDto("Invalid confirm data");

        if (userId == null || code == null) return BadRequest(errorDto);

        var user = await UserManager.FindByIdAsync(userId);
        if (user == null) return BadRequest(errorDto);

        var result = await UserManager.ConfirmEmailAsync(user, code);
        if (result.Succeeded)
        {
            var applicationUser = Context.ApplicationUsers.Single(u => u.CredentialsUserId == userId);
            return Ok(await GetByIdCleaned(applicationUser.Id));
        }
        return BadRequest(errorDto);
    }

    [HttpPost("ForgotPassword")]
    [AllowAnonymous]
    public async Task<IActionResult> ForgotPassword(string email)
    {
        if (string.IsNullOrWhiteSpace(email)) return BadRequest(GetSimpleWsErrorDto("Invalid data"));

        var user = await UserManager.FindByEmailAsync(email);
        if (user == null) return BadRequest(GetSimpleWsErrorDto("Invalid account or not confirmed"));

        var token = UserManager.GeneratePasswordResetTokenAsync(user).Result;

        //var callbackUrl = Url.Action(nameof(ResetPassword), "ApplicationUsers", new { token },
        //protocol: HttpContext.Request.Scheme);

        string callbackUrl = _options.Value.UrlFrontend + "recuperar-contrasena?token=" + token +
                             "&email=" + email;

        await EmailSender.SendEmailResetPasswordAsync(email, callbackUrl, token);
        return Ok(new { token });
    }

    [HttpPost("ResetPassword")]
    [AllowAnonymous]
    public async Task<IActionResult> ResetPassword(string token, [FromBody] ChangePass changePass)
    {
        if (
            string.IsNullOrWhiteSpace(changePass.Email)
            || string.IsNullOrWhiteSpace(token)
            || string.IsNullOrWhiteSpace(changePass.NewPassword)
            || changePass.NewPassword != changePass.NewPassword2
        ) return BadRequest(GetSimpleWsErrorDto("Invalid data"));

        var user = await UserManager.FindByEmailAsync(changePass.Email);
        if (user == null) return BadRequest(GetSimpleWsErrorDto("Invalid account or not confirmed"));

        var code = await UserManager.GeneratePasswordResetTokenAsync(user);

        // var identityResult = UserManager.ResetPasswordAsync(user, token, changePass.NewPassword).Result;
        var identityResult = UserManager.ResetPasswordAsync(user, code, changePass.NewPassword).Result;
        if (identityResult.Succeeded)
        {
            var applicationUser = Context.ApplicationUsers.Single(u => u.CredentialsUserId == user.Id);
            return Ok(await GetByIdCleaned(applicationUser.Id));
        }
        identityResult.Errors.ToList().ForEach(e => ModelState.AddModelError("password", e.Description));
        return ActionResultForModelStateValidation();
    }

    public class ChangePass { public string Email, CurrentPassword, NewPassword, NewPassword2; }
    [HttpPost("ChangePassword")]
    [AllowAnonymous]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePass changePass)
    {
        var errorDto = GetSimpleWsErrorDto("Invalid confirm data");

        if (
                string.IsNullOrWhiteSpace(changePass.Email)
                || string.IsNullOrWhiteSpace(changePass.CurrentPassword)
                || string.IsNullOrWhiteSpace(changePass.NewPassword)
                || changePass.NewPassword != changePass.NewPassword2
            ) return BadRequest(errorDto);

        var user = await UserManager.FindByEmailAsync(changePass.Email);
        if (user == null) return BadRequest(errorDto);

        var result = await UserManager.ChangePasswordAsync(user, changePass.CurrentPassword, changePass.NewPassword);
        if (result.Succeeded)
        {
            var applicationUser = Context.ApplicationUsers.Single(u => u.CredentialsUserId == user.Id);
            return Ok(await GetByIdCleaned(applicationUser.Id));
        }
        return BadRequest(errorDto);
    }

    public override async Task<ActionResult<ApplicationUser>> Delete(long id)
    {
        EnsureUserRole(UsersRole.Superuser);
        try
        {
            var deleteBase = await base.Delete(id);
            await UserManager.DeleteAsync(await UserManager.FindByIdAsync(Context.FindAsync<ApplicationUser>(id).Result.CredentialsUserId));
            return deleteBase;
        }
        catch (Exception ex)
        {
            return BadRequest();
        }

    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(string email, string password)
    {
        try
        {
            var resultCheck = await SignInManager.PasswordSignInAsync(email, password, false, false);
            if (resultCheck.Succeeded /*&& UserManager.FindByEmailAsync(email).Result.EmailConfirmed*/)
            {
                var applicationUser = Context.ApplicationUsers
                                            .Single(u => u.CredentialsUser.Email.ToLower() == email.ToLower());

                var userclaim = new[] { new Claim(ClaimTypes.Name, email) };
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["SecretKey"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: Configuration["Issuer"],
                    audience: Configuration["Issuer"],
                    claims: userclaim,
                    // expires: DateTime.Now.AddMinutes(1),
                    expires: DateTime.Now.AddDays(1),
                    signingCredentials: creds);

                var refreshTokenObj = new RefreshToken
                {
                    Username = email,
                    Refreshtoken = Guid.NewGuid().ToString()
                };
                Context.RefreshTokens.Add(refreshTokenObj);
                Context.SaveChanges();

                #region Auditory
                try
                {
                    Auditory audit = new Auditory();
                    audit.Date = DateTime.Now;
                    audit.Entity = "Usuarios";
                    audit.UserId = applicationUser.Id;
                    audit.User = applicationUser.FullName;
                    audit.AuditMessage = "Login usuario: " + applicationUser.FullName;
                    Context.Add(audit);
                    await Context.SaveChangesAsync();
                }
                //El catch vacio es simplemente para que un error aqui no interrumpa el proceso normal. Deberia ir algun tipo de log.
                catch (Exception ex)
                {
                }
                #endregion

                var user = await GetByIdCleaned(applicationUser.Id);
                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    refreshToken = refreshTokenObj.Refreshtoken,
                    user = new
                    {
                        user.Id,
                        user.FullName,
                        user.CountryId,
                        user.ApplicationRoleId,
                        ApplicationRoleName = user.ApplicationRole != null ? user.ApplicationRole.Name : string.Empty
                    }
                });
            }
        }
        catch (Exception ex)
        {
            string filePath = @"C:\Logs\ErrorLadevi.txt";
            using (StreamWriter writer = new StreamWriter(filePath, true))
            {
                writer.WriteLine("-----------------------------------------------------------------------------");
                writer.WriteLine("Date : " + DateTime.Now.ToString());
                writer.WriteLine();

                while (ex != null)
                {
                    writer.WriteLine(ex.GetType().FullName);
                    writer.WriteLine("Message : " + ex.Message);
                    writer.WriteLine("StackTrace : " + ex.StackTrace);

                    ex = ex.InnerException;
                }
            }
        }
        return BadRequest();
    }

    [ApiExplorerSettings(IgnoreApi = true)]
    public string GetTokenString(string email)
    {
        var userclaim = new[] { new Claim(ClaimTypes.Name, email) };
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["SecretKey"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: Configuration["Issuer"],
            audience: Configuration["Issuer"],
            claims: userclaim,
            expires: DateTime.Now.AddDays(1),
            signingCredentials: creds
        );
        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
        return tokenString;
    }

    [HttpPost("refreshToken")]
    public async Task<IActionResult> RefreshToken(string refreshToken)
    {
        var refreshTokenEntity = Context.RefreshTokens.SingleOrDefault(m => m.Refreshtoken == refreshToken);
        if (refreshTokenEntity == null) return NotFound(GetSimpleWsErrorDto("Refresh token not found"));
        var userclaim = new[] { new Claim(ClaimTypes.Name, refreshTokenEntity.Username) };
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["SecretKey"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: Configuration["Issuer"],
            audience: Configuration["Issuer"],
            claims: userclaim,
            expires: DateTime.Now.AddMinutes(10),
            signingCredentials: creds);

        refreshTokenEntity.Refreshtoken = Guid.NewGuid().ToString();
        Context.RefreshTokens.Update(refreshTokenEntity);
        Context.SaveChanges();

        var email = refreshTokenEntity.Username.ToLower();
        var applicationUser = Context.ApplicationUsers
            .Single(u => u.CredentialsUser.Email.ToLower() == email);

        return Ok(new
        {
            token = new JwtSecurityTokenHandler().WriteToken(token),
            refreshToken = refreshTokenEntity.Refreshtoken,
            user = await GetByIdCleaned(applicationUser.Id)
        });
    }

    [HttpGet("Options")]
    public async Task<IActionResult> Options()
    {
        EnsureUserRole(UsersRole.Superuser, UsersRole.Supervisor);

        var products = Context.ApplicationUsers
                            .Include(u => u.ApplicationRole)
                            .AsNoTracking()
                            .Where(u => !u.Deleted.HasValue || !u.Deleted.Value)
                            .Select(u => new
                            {
                                u.Id,
                                u.FullName,
                                ApplicationRoleName = u.ApplicationRole != null ? u.ApplicationRole.Name : string.Empty
                            })
                            .ToList();

        return Ok(products);
    }
}
