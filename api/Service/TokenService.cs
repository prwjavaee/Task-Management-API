using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using api.Interfaces;
using api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace api.Service
{
    public class TokenService : ITokenService
    {
        //appsettings.json
        private readonly IConfiguration _config;
        private readonly SymmetricSecurityKey _key;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public TokenService(IConfiguration config, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _config = config;
            //產生密鑰
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:SigningKey"]!));
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public async Task<string> CreateToken(AppUser user)
        {
            //JWT Claim
            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Email, user.Email),
                new(JwtRegisteredClaimNames.GivenName, user.UserName)
            };

            var roles = await _userManager.GetRolesAsync(user);
            var roleClaimsList = new List<Claim>();
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
                var roleEntity = await _roleManager.FindByNameAsync(role);
                if (roleEntity != null){
                    var roleClaims = await _roleManager.GetClaimsAsync(roleEntity);
                    roleClaimsList.AddRange(roleClaims);
                }
            }
            claims.AddRange(roleClaimsList);

            //JWT 簽名資訊
            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);
            //JWT 結構物件
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(7),
                SigningCredentials = creds,
                Issuer = _config["JWT:Issuer"],
                Audience = _config["JWT:Audience"]
            };
            //負責生成 JWT Token
            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}