using Microsoft.AspNetCore.Mvc;
using System;
using OnPaper_Auth.Services;
using OnPaper_Auth.Abstractions;

using System.Net;
using Microsoft.AspNetCore.Identity.Data;


namespace OnPaper_Auth.Controllers;


[ApiController]
[Route("[controller]/[action]")]
public class AuthController : ControllerBase
{
    private readonly AuthenticationService _authService= new AuthenticationService();


    [HttpGet(Name = "Hello")]
    public string Hello()
    {
        return "Hello, World!";
    }

    [HttpGet("{name}", Name = "HelloButDiff")]
    public string HelloName(string name)
    {
        return "Hello " + name;
    }

    [HttpPost(Name = "register")]
    public Task<string> Register([FromBody] UserRegisterRequest request)
    {
        return _authService.RegisterAsync(request.Username, request.FullName, request.Email, request.Password);
    }

    [HttpPost(Name = "login")]
    public Task<string> LoginWithEmailPassword([FromBody] UserLoginRequest request)
    {
        return _authService.AuthenticateAsync(request.Email, request.Password, true);
    }

}
