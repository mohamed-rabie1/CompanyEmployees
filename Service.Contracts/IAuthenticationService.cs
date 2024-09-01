using Microsoft.AspNetCore.Identity;
using Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Contracts
{
    public interface IAuthenticationService
    {
        Task<IdentityResult> RegisterUser(UserForRegistrationDTO userForRegistrationDTO);
        Task<bool> ValidateUser(UserForAuthenticationDTO userForAuthenticationDTO);
        Task<TokenDTO> CreateToken(bool populateExp);
        Task<TokenDTO> RefreshToken(TokenDTO tokenDTO);
    }
}
