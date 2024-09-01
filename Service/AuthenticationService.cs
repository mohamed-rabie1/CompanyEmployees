using AutoMapper;
using Entities.Exceptions;
using Entities.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Service.Contracts;
using Shared.DTOs;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly IOptions<JwtConfiguration> _configuration;
        private readonly JwtConfiguration _jwtConfiguration;
        private User? _user;
        public AuthenticationService(IMapper mapper,UserManager<User> userManager,IOptions<JwtConfiguration> configuration)
        {
            _mapper = mapper;
            _userManager = userManager;
            _configuration = configuration;
            _jwtConfiguration = _configuration.Value;//extract the JwtConfiguration object with all the populated properties
            
        }

        public async Task<IdentityResult> RegisterUser(UserForRegistrationDTO userForRegistrationDTO)
        {
            var user = _mapper.Map<User>(userForRegistrationDTO);
            //save the user to the database
            var result =await _userManager.CreateAsync(user, userForRegistrationDTO.Password);
            if (result.Succeeded)
            {
                //add that user to the named roles that sent from the client side
                await _userManager.AddToRolesAsync(user, userForRegistrationDTO.Roles);
            }
            return result;
        }

        public async Task<bool> ValidateUser(UserForAuthenticationDTO userForAuthenticationDTO)
        {
            //get user from database
            _user = await _userManager.FindByNameAsync(userForAuthenticationDTO.UserName);
            //verify the user’s password against the hashed password from the database.
            var result =(_user!=null && await _userManager.CheckPasswordAsync(_user,userForAuthenticationDTO.Password));
            return result;
        }

        public async Task<TokenDTO> CreateToken(bool populateExp)
        {
            //returns our secret key as a byte array with the security algorithm.
            var signingCredentials = GetSigningCredentials();
            //creates a list of claims with the user name inside and all the roles the user belongs to
            var claims = await GetClaims();
            // create token with options
            var tokenOptions = GenerateTokenOptions(signingCredentials, claims);

            var refreshToken = GenerateRefreshToken();
            _user.RefreshToken = refreshToken;

            if (populateExp)
            {
                _user.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);
            }
            await _userManager.UpdateAsync(_user);

            var accessToken= new JwtSecurityTokenHandler().WriteToken(tokenOptions);

            return new TokenDTO(accessToken, refreshToken);
        }

        private JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials, List<Claim> claims)
        {
            

            var tokenOptions = new JwtSecurityToken(
                issuer: _jwtConfiguration.ValidIssuer, 
                audience: _jwtConfiguration.ValidAudience, 
                claims: claims, 
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(_jwtConfiguration.Expires)),
                signingCredentials: signingCredentials); 
            return tokenOptions;
        }

        private async Task<List<Claim>> GetClaims()
        {
            var claims = new List<Claim>() 
            { 
                new Claim(ClaimTypes.Name, _user.UserName) 
            };
            var roles = await _userManager.GetRolesAsync(_user);
            foreach (var role in roles) 
            { 
                claims.Add(new Claim(ClaimTypes.Role, role)); 
            }
            return claims;
        }

        private SigningCredentials GetSigningCredentials()
        {
            var key = Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("SECRETKEY"));
            var secret = new SymmetricSecurityKey(key);

            return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
        }
        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            //generate a cryptographic random number
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }
        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            

            var tokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("SECRETKEY"))),
                ValidateLifetime = true,
                ValidIssuer = _jwtConfiguration.ValidIssuer,
                ValidAudience = _jwtConfiguration.ValidAudience
            };
            var tokenHandler=new JwtSecurityTokenHandler();
            SecurityToken securityToken;

            var principal=tokenHandler.ValidateToken(token,tokenValidationParameters,out securityToken);

            var jwtSecurityToken=securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null
                || ! jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid Token");
            }
            return principal;
        }

        public async Task<TokenDTO> RefreshToken(TokenDTO tokenDTO)
        {
            //extract the principal from the expired token
            var principal = GetPrincipalFromExpiredToken(tokenDTO.AccessToken);
            //use the Identity.Name which is the username of the user, to fetch that user from the database
            var user =await _userManager.FindByNameAsync(principal.Identity.Name);
            
            if (user == null || user.RefreshToken!=tokenDTO.RefreshToken || user.RefreshTokenExpiryTime<=DateTime.Now)
            {
                throw new RefreshTokenBadRequest();
            }
            _user = user;
            return await CreateToken(populateExp: false);
        }
    }
}
