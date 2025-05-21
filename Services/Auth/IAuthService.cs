public interface IAuthService
{
    Task<User> Register(UserRegisterDTO userDTO);
    Task<Token> Login(UserLoginDTO userDTO);
    Task<Token> RefreshTokenAsync(TokenDTO tokenRequest);
    Task<User> GetUserById(int id); 
    Task<User> ChangeProfileVisibility(int id);
}