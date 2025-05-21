
using System.CodeDom.Compiler;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.Serialization;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

public class AuthService(IConfiguration conf, NotesContext db) : IAuthService
{
    public async Task<User> Register(UserRegisterDTO userDTO)
    {
        // Check for if user existing already
        var user = await db.Users.FirstOrDefaultAsync(u => u.Email == userDTO.email);

        // User already registered with that email 
        if (user is not null)
            return null;

        // Email avaible

        var salt = GenerateSalt();

        User newUser = new User
        {
            Name = userDTO.name,
            Surname = userDTO.surname,
            Email = userDTO.email,
            // If user chose to set up that option later, the profile is private by default
            isPublic = userDTO.isPublic ?? false,
            HashedPassword = HashPassword(userDTO.password, salt),
            Salt = salt,
        };

        await db.Users.AddAsync(newUser);
        await db.SaveChangesAsync();
        return newUser;
    }
    public async Task<Token> Login(UserLoginDTO userDTO)
    {
        // Checking if the user is already registered
        var user = await db.Users.FirstOrDefaultAsync(u => u.Email == userDTO.email);

        // If user still not registered or the password is incorrect
        if (user is null || VerifyPassword(userDTO.password, user.HashedPassword, user.Salt) == false)
            return null;


        return new Token
        {
            AccessToken = GenerateToken(user),
            RefreshToken = await GenerateAndSaveRefreshToken(user)
        };
    }

    public async Task<User> GetUserById(int id)
    {
        var user = await db.Users
            .Include(u => u.Notes)
            .Include(u => u.Reviews)
            .Include(u => u.Favourites)
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user is null) return null;
        return user;
    }

    private string GenerateToken(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Surname, user.Surname),
            new Claim(ClaimTypes.Email, user.Email),
        };

        Console.WriteLine($"JWT Key: {conf["Jwt__Key"]}");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(conf["Jwt__Key"]));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: conf["Jwt:Issuer"],
            audience: conf["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(15),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    // Called when the user logs in for the first time, updating the expiry time
    private async Task<string> GenerateAndSaveRefreshToken(User user)
    {
        var refreshToken = await GetRefreshToken(user);
        var expiryTime = DateTime.UtcNow.AddDays(7);
        user.RefreshTokenExpiryTime = expiryTime;
        await db.SaveChangesAsync();
        return refreshToken;
    }

    // Called when the access token expires, so the user will also get a new refresh token but the expiry time will not change
    private async Task<string> GetRefreshToken(User user)
    {
        var rng = RandomNumberGenerator.Create();
        var randomNumber = new byte[32];
        rng.GetBytes(randomNumber);
        var refreshToken = Convert.ToBase64String(randomNumber);
        user.RefreshToken = refreshToken;
        await db.SaveChangesAsync();
        return refreshToken;
    }

    public async Task<Token> RefreshTokenAsync(TokenDTO tokenRequest)
    {
        var user = await ValidateRefreshTokenAsync(tokenRequest.userId, tokenRequest.refreshToken);
        if (user is null) return null;
        return new Token
        {
            AccessToken = GenerateToken(user),
            RefreshToken = await GetRefreshToken(user)
        };
    }

    public async Task<User> ChangeProfileVisibility(int id)
    {
        var user = await db.Users.FindAsync(id);
        if (user is null) return null;

        user.isPublic = !user.isPublic;
        await db.SaveChangesAsync();
        return user;
    }

    private async Task<User?> ValidateRefreshTokenAsync(int userId, string refreshToken)
    {
        var user = await db.Users.FindAsync(userId);
        if (user is null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
        {
            return null;
        }

        return user;
    }

    private byte[] GenerateSalt()
    {
        using var rng = RandomNumberGenerator.Create();
        var salt = new byte[16];
        rng.GetBytes(salt);
        return salt;
    }

    private string HashPassword(string plainText, byte[] salt)
    {
        // Hash of the password including the salt
        string pepperedPlainText = plainText + conf["Pw__Pepper"];
        Console.WriteLine($"Peppered password: {pepperedPlainText}");
        int.TryParse(conf["PWHash__Iterations"], out var iterations);
        Console.WriteLine($"Iterations: {iterations}");
        // Checking if the iterations are set correctly, if not it will set it to 10000
        if (iterations <= 0) iterations = 10000;
        var digest = new Rfc2898DeriveBytes(pepperedPlainText, salt, iterations, HashAlgorithmName.SHA256);
        var hash = digest.GetBytes(32);
        return Convert.ToBase64String(hash);
    }

    private bool VerifyPassword(string plainTextLogIn, string hashedPasswordStored, byte[] salt)
    {
        return HashPassword(plainTextLogIn, salt) == hashedPasswordStored;
    }
}