using Microsoft.AspNetCore.Mvc;
using OnPaper_Auth.Services;
namespace OnPaper_Auth.Controllers;
using OnPaper_Auth.Abstractions;

[ApiController]
[Route("[controller]/[action]")]
public class UserController : Controller
{
    private readonly UserDataServices _userDataServices;

    public UserController(UserDataServices userDataServices)
    {
        _userDataServices = userDataServices;
    }

    [HttpPost(Name = "update")]
    public async Task<IActionResult> UpdateUserProfile([FromBody] UserUpdateRequest request)
    {
        var result = await _userDataServices.UpdateUserProfile(request.IdToken, request);
        if (result.StartsWith("{ \"error\":"))
        {
            return BadRequest(result);
        }
        return Ok(result);
    }
}
