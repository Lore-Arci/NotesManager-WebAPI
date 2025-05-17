using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

[ApiController]
[Route("/[controller]")]
public class AuthController(IAuthService service) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserRegisterDTO userDTO)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var user = await service.Register(userDTO);

        if (user is null)
            return BadRequest("User already registered with that email");

        return Created("User created successfully", user);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserLoginDTO userDTO)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var token = await service.Login(userDTO);

        if (token is null)
            return BadRequest("Invalid email or password");

        return Ok(token);
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken(TokenDTO tokenDTO)
    {
         if(!ModelState.IsValid) 
            return BadRequest(ModelState);

        var token = await service.RefreshTokenAsync(tokenDTO);
        if(tokenDTO is null) return Unauthorized();
        return Ok(tokenDTO);
    }
}