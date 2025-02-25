using Domain.Dtos.Auth;
using Domain.Entities;
using Infrastructure.Interfaces;
using Infrastructure.Responses;
using Infrastructure.Seed;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService)
{
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<Response<string>> Register(RegisterDto request)
    {
        return await authService.Register(request);
    }
    
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<Response<string>> Login(LoginDto request)
    {
        return await authService.Login(request);
    }
    
    [HttpPost("add-role-to-user")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<Response<string>> AddRoleToUser(RoleDto request)
    {
        return await authService.AddRoleToUser(request);
    }
    
    [HttpDelete("remove-role-from-user")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<Response<string>> RemoveRoleFromUser(RoleDto request)
    {
        return await authService.RemoveRoleFromUser(request);
    }
    
    [HttpGet("get-all-users")]
    [AllowAnonymous]
    public async Task<Response<List<User>>> GetUsers()
    {
        return await authService.GetUsers();
    }
}
