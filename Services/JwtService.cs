namespace JobFitScoreAPI.Services
{
    public class JwtService
    {
        public string GenerateToken(string user)
        {
            // Aqui seria gerado o token JWT real
            return $"token_{user}_{Guid.NewGuid()}";
        }
    }
}
