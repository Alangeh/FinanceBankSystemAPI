using BaseLibrary.DTOs;
using BaseLibrary.Entities;
using BaseLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ServerLibrary.Data;
using ServerLibrary.Helpers;
using ServerLibrary.Repositories.Contracts;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ServerLibrary.Repositories.Implementations
{
    public class UserAuthenticationRepository(IOptions<JwtSection> config, AppDbContext appDbContext) : IUserAccount
    {
        public async Task<GeneralResponse> CreateAsync(RegisterDto user)
        {
            if(user is null)
            {
                return new GeneralResponse(false, "User does not exist");
            }
            // verify user availability
            var checkUser = await FindUserByEmail(user.Email!);

            if (checkUser != null)
            {
                return new GeneralResponse(false, "User is registered already");
            }

            // add user
            var applicationUser = await AddToDatabase(new ApplicationUser()
            {
                Fullname = user.Fullname,
                Email = user.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(user.Password)
            });

            // check, create and assign user role
            var checkAdminRole = await appDbContext.SystemRoles.FirstOrDefaultAsync(a => a.Name!.Equals(Constants.Admin));

            if (checkAdminRole == null)
            {
                var createAdminRole = await AddToDatabase(new SystemRole()
                {
                    Name = Helpers.Constants.Admin
                });
                await AddToDatabase(new UserRole()
                {
                    RoleId = createAdminRole.Id,
                    UserId = applicationUser.Id,
                });
                return new GeneralResponse(true, "Account created!");
            }

            var checkUserRole = await appDbContext.SystemRoles.FirstOrDefaultAsync(u => u.Name!.Equals(Constants.User));
            SystemRole response = new();
            if (checkUserRole == null)
            {
                response = await AddToDatabase(new SystemRole()
                {
                    Name = Constants.User
                });
                await AddToDatabase(new UserRole() 
                { 
                    RoleId = response.Id,
                    UserId = applicationUser.Id, 
                });
            } else
            {
                await AddToDatabase(new UserRole()
                {
                    RoleId = checkUserRole.Id,
                    UserId = applicationUser.Id
                });
            }
            return new GeneralResponse(true, "Account Created!");
        }
        public async Task<LoginResponse> SignInAsync(LoginDto user)
        {
            if (user is null)
            {
                return new LoginResponse(false, "Login Error");
            }

            var applicationUser = await FindUserByEmail(user.Email!);
            if (applicationUser is null)
            {
                return new LoginResponse(false, "User not found");
            }

            // Verify password
            if (!BCrypt.Net.BCrypt.Verify(user.Password, applicationUser.Password))
            {
                return new LoginResponse(false, "Email or Password is not correct");
            }

            // Verify roles
            //var getUserRole = await FindUserRole(applicationUser.Id);
            var getUserRole = await appDbContext.UserRoles.FirstOrDefaultAsync(r => r.UserId == applicationUser.Id);
            if (getUserRole is null)
            {
                return new LoginResponse(false, "user role not found");
            }

            var getRoleName = await appDbContext.SystemRoles.FirstOrDefaultAsync(r => r.Id == getUserRole.RoleId);
            if (getRoleName is null)
            {
                return new LoginResponse(false, "system role not found");
            }

            // get JWT Token
            string jwtToken = GenerateToken(applicationUser, getRoleName!.Name!);
            string refreshToken = GenerateRefreshToken();

            // save the refresh token to database
            var findUser = await appDbContext.RefreshTokenInfos.FirstOrDefaultAsync(t => t.UserId == applicationUser.Id);
            if (findUser is not null)
            {
                findUser!.Token = refreshToken;
                await appDbContext.SaveChangesAsync();
            } else
            {
                await AddToDatabase(new RefreshTokenInfo()
                {
                    Token = refreshToken,
                    UserId = applicationUser.Id,
                });
            }
            return new LoginResponse(true,"Login Successful", jwtToken, refreshToken);
        }
        public async Task<LoginResponse> RefreshTokenAsync(RefreshTokenDto token)
        {
            if (token == null)
                return new LoginResponse(false, "Token data is emptyp");

            // get refresh token
            var findToken = await appDbContext.RefreshTokenInfos.FirstOrDefaultAsync(t => t.Token!.Equals(token.Token));
            if (findToken is null)
                return new LoginResponse(false, "Refresh token in required");

            // get user details
            var user = await appDbContext.ApplicationUsers.FirstOrDefaultAsync(u => u.Id == findToken.UserId);
            if (user is null)
                return new LoginResponse(false, "Refresh token could not be generated because does not exist");

            var userRole = await FindUserRole(user.Id);
            var roleName = await FindRoleName(userRole.RoleId);
            string jwtToken = GenerateToken(user, roleName.Name!);
            string refreshToken = GenerateRefreshToken();

            var updateRefreshToken = await appDbContext.RefreshTokenInfos.FirstOrDefaultAsync(t => t.UserId == user.Id);
            if (updateRefreshToken is null)
                return new LoginResponse(false, "Refresh token not generated because user has not signed in");

            updateRefreshToken.Token = refreshToken;
            await appDbContext.SaveChangesAsync();
            return new LoginResponse(true, "Token refreshed successfully", jwtToken, refreshToken);
        }

        #region private methods
        private async Task<UserRole> FindUserRole(int userId) => await appDbContext.UserRoles.FirstOrDefaultAsync(r => r.UserId == userId);
        private async Task<SystemRole> FindRoleName(int roleId) => await appDbContext.SystemRoles.FirstOrDefaultAsync(s => s.Id == roleId);
        private async Task<ApplicationUser> FindUserByEmail(string email)
        {
            ApplicationUser applicationUser = await appDbContext.ApplicationUsers.FirstOrDefaultAsync(e => e.Email!.ToLower()!.Equals(email!.ToLower()));
            return applicationUser;
        }
        private async Task<T> AddToDatabase<T>(T model)
        {
            var result = appDbContext.Add(model!);
            await appDbContext.SaveChangesAsync();
            return (T)result.Entity;
        }
        private string GenerateToken(ApplicationUser user, string role) 
        { 
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.Value.Key!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var userClaims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Fullname!),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(ClaimTypes.Role, role!),
            };
            var token = new JwtSecurityToken(
                issuer: config.Value.Issuer,
                audience: config.Value.Audience,
                claims: userClaims,
                expires: DateTime.Now.AddSeconds(30),
                signingCredentials: credentials
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }       
        private static string GenerateRefreshToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        }
        #endregion
    }
}
