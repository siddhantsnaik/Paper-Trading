using Microsoft.AspNetCore.Mvc;
using System;
using OnPaper_Auth.Services;
using OnPaper_Auth.Abstractions;

using System.Net;
using Microsoft.AspNetCore.Identity.Data;
using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Sprache;


namespace OnPaper_Auth.Controllers;


[ApiController]
[Route("[controller]/[action]")]
public class AuthController : ControllerBase
{
    private readonly AuthenticationService _authService;

    public AuthController(AuthenticationService authService)
    {
        _authService = authService;
    }


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
    public async Task<IActionResult> Register([FromBody] UserRegisterRequest request)
    {
        //return _authService.RegisterAsync(request.Username, request.FullName, request.Email, request.Password);
        var result = await _authService.RegisterAsync(request.Username, request.FullName, request.Email, request.Password);
        if (result.StartsWith("{ \"error\":"))
        {
            return BadRequest(result);
        }
        return Ok(result);
    }

    [HttpPost(Name = "login")]
    public async Task<IActionResult> LoginWithEmailPassword([FromBody] UserLoginRequest request)
    {
        var result = await _authService.AuthenticateAsync(request.Email, request.Password, true);
        if (result.StartsWith("{ \"error\":"))
        {
            return BadRequest(result);
        }
        return Ok(result);
    }

    [HttpPost(Name = "refreshUserToken")]
    public async Task<IActionResult> RefreshUserToken([FromBody] string refreshToken)
    {
        var result = await _authService.RefreshToken(refreshToken);
        if (result.StartsWith("{ \"error\":"))
        {
            return BadRequest(result);
        }
        return Ok(result);
    }

    [HttpPost(Name = "revoketoken")]
    public async Task<IActionResult> RevokeToken([FromBody] string idToken)
    {
        try
        {
            await _authService.RevokeToken(idToken);
            return Ok();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

}
