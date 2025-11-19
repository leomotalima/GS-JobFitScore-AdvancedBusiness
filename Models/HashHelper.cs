using BCrypt.Net;
using System;

namespace JobFitScoreAPI
{
    public static class HashHelper
    {
        // Método de utilidade para gerar hashes
        public static string GerarNovoHash(string senhaPlain)
        {
            // O '12' é o fator de custo (rounds), que define a força do hash.
            // Quanto maior, mais lento e mais seguro. 12 é um bom padrão.
            return BCrypt.Net.BCrypt.HashPassword(senhaPlain, 12);
        }
        
        // Mantenha o método de verificação de senha que você já tem:
        public static bool VerificarSenha(string senhaDigitada, string hashDoBanco)
        {
            return BCrypt.Net.BCrypt.Verify(senhaDigitada, hashDoBanco);
        }
    }
}