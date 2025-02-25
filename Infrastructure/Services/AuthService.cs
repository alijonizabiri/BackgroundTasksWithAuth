using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using Domain.Dtos.Auth;
using Domain.Entities;
using Infrastructure.Interfaces;
using Infrastructure.Responses;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Services;

public class AuthService(
    UserManager<User> userManager,
    RoleManager<IdentityRole> roleManager,
    IConfiguration configuration) : IAuthService
{
    public async Task<Response<string>> Register(RegisterDto model)
    {
        var existingUser = await userManager.FindByNameAsync(model.UserName);
        if (existingUser != null) return new Response<string>(HttpStatusCode.BadRequest, "Username already taken");

        var newUser = new User()
        {
            UserName = model.UserName,
            Email = model.Email,
        };

        var result = await userManager.CreateAsync(newUser, model.Password);
        return !result.Succeeded 
            ? new Response<string>(HttpStatusCode.BadRequest, "Some thing went wrong") 
            : new Response<string>("Successfully created");
    }

    public async Task<Response<string>> Login(LoginDto login)
    {
        var existingUser = await userManager.FindByNameAsync(login.Username);
        if (existingUser == null)
        {
            return new Response<string>(HttpStatusCode.BadRequest, "Username or password is incorrect");
        }

        var result = await userManager.CheckPasswordAsync(existingUser, login.Password);
        if (!result)
        {
            return new Response<string>(HttpStatusCode.BadRequest, "Username or password is incorrect");
        }

        var token = await GenerateJwtToken(existingUser);
        return new Response<string>(token);
    }
    
    private async Task<string> GenerateJwtToken(User user)
    {
        var key = Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!);
        var securityKey = new SymmetricSecurityKey(key);
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var claims = new List<Claim>()
        {
            new Claim(JwtRegisteredClaimNames.Name, user.UserName),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.NameId, user.Id),
        };

        //add roles
        var roles = await userManager.GetRolesAsync(user);
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var token = new JwtSecurityToken(
            issuer: configuration["Jwt:Issuer"],
            audience: configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials
        );
        
        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
        return tokenString;
    }

    public async Task<Response<string>> RemoveRoleFromUser(RoleDto userRole)
    {
        var role = await roleManager.FindByIdAsync(userRole.RoleId);
        if (role == null)
        {
            return new Response<string>(HttpStatusCode.NotFound, "Role not found");
        }
        var user = await userManager.FindByIdAsync(userRole.UserId);
        if (user == null)
        {
            return new Response<string>(HttpStatusCode.NotFound, "User not found");
        }

        var result = await userManager.RemoveFromRoleAsync(user, role.Name!);
        return !result.Succeeded 
            ? new Response<string>(HttpStatusCode.BadRequest, "Some thing went wrong") 
            : new Response<string>("Role successfully removed from user");
    }

    public async Task<Response<string>> AddRoleToUser(RoleDto userRole)
    {
        var role = await roleManager.FindByIdAsync(userRole.RoleId);
        if (role == null)
        {
            return new Response<string>(HttpStatusCode.NotFound, "Role not found");
        }
        var user = await userManager.FindByIdAsync(userRole.UserId);
        if (user == null)
        {
            return new Response<string>(HttpStatusCode.NotFound, "User not found");
        }

        var result = await userManager.AddToRoleAsync(user, role.Name!);
        return !result.Succeeded 
            ? new Response<string>(HttpStatusCode.BadRequest, "Some thing went wrong") 
            : new Response<string>("Role successfully assigned to user");
    }

    public async Task<Response<List<User>>> GetUsers()
    {
        var users = await userManager.Users.ToListAsync();
        return new Response<List<User>>(users);
    }
}
