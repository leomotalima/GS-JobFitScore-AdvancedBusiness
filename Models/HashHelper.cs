using BCrypt.Net;
using System;

namespace JobFitScoreAPI
{
    public static class HashHelper
    {
        public static string GerarNovoHash(string senhaPlain)
        {
            return BCrypt.Net.BCrypt.HashPassword(senhaPlain, 12);
        }
        
        public static bool VerificarSenha(string senhaDigitada, string hashDoBanco)
        {
            return BCrypt.Net.BCrypt.Verify(senhaDigitada, hashDoBanco);
        }
    }
}