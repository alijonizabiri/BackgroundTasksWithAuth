using Domain.Dtos.Auth;
using Domain.Entities;
using Infrastructure.Responses;
using Infrastructure.Seed;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Interfaces;

public interface IAuthService
{
    Task<Response<string>> Register(RegisterDto model);
    Task<Response<string>> Login(LoginDto login);
    Task<Response<string>> RemoveRoleFromUser(RoleDto userRole);
    Task<Response<string>> AddRoleToUser(RoleDto userRole);
    Task<Response<List<User>>> GetUsers();
}
