using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JobFitScoreAPI.Migrations
{
    /// <inheritdoc />
    public partial class SeedInitialData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // USUARIOS
            migrationBuilder.Sql(@"
                INSERT INTO ""USUARIOS"" (""id_usuario"", ""nome"", ""email"", ""senha"")
                VALUES
                (1, 'Admin', 'admin@jobfitscore.com', '$2a$12$3uKG7GyamTjjg2FLKKkbgOOYtw7.Ky2r9VfH.LSXEC9oE01/cEMy6'),
                (2, 'Maria Souza', 'maria@email.com', '$2a$12$PZ8DNWydk98kLXei361IAuPK9dLkV50lhS2r.AvLoCUCjG7VLwLMW'),
                (3, 'Carlos Lima', 'carlos@email.com', '$2a$12$uxhQky4PasnB2CQmR44Yee1yI0MbXlJUEwK88uz1.paXcbz1ew.k.');
            ");

            // EMPRESAS
            migrationBuilder.Sql(@"
                INSERT INTO ""EMPRESAS"" (""id_empresa"", ""nome"", ""cnpj"", ""email"", ""senha"")
                VALUES
                (1, 'Empresa Alpha', '12345678000100', 'contato@alpha.com', '$2a$12$iGbpaLY9ozT0UY0siGYewOjcl8rvigWNS4qLKp8S3g5hSR0XIZ7qi'),
                (2, 'Empresa Beta', '23456789000111', 'contato@beta.com', '$2a$12$SJ.afe1EDlzCKEaSjnHmHO1QHZXtcZGiwDroFTqF8hYWkDgOXSw1.'),
                (3, 'Empresa Gamma', '34567890000122', 'contato@gamma.com', '$2a$12$FLy31xZVIUG2xV4luEKr7efUN0KeVUTH1gx30d3Osq4.mun5D20lS');
            ");

            // VAGAS
            migrationBuilder.Sql(@"
                INSERT INTO ""VAGAS"" (""id_vaga"", ""titulo"", ""empresa_id"")
                VALUES
                (1, 'Desenvolvedor .NET', 1),
                (2, 'Analista de Marketing', 2),
                (3, 'Engenheiro de Dados', 3);
            ");

            // HABILIDADES
            migrationBuilder.Sql(@"
                INSERT INTO ""HABILIDADES"" (""id_habilidade"", ""nome"")
                VALUES
                (1, 'C#'),
                (2, 'SQL'),
                (3, 'Marketing Digital');
            ");

            // USUARIO_HABILIDADE
            migrationBuilder.Sql(@"
                INSERT INTO ""USUARIO_HABILIDADE"" (""id_usuario_habilidade"", ""usuario_id"", ""habilidade_id"")
                VALUES
                (1, 1, 1),
                (2, 1, 2),
                (3, 2, 3);
            ");

            // CURSOS
            migrationBuilder.Sql(@"
                INSERT INTO ""CURSOS"" (""id_curso"", ""nome"", ""instituicao"", ""carga_horaria"", ""usuario_id"")
                VALUES
                (1, 'Curso C# Avançado', 'Udemy', 40, 1),
                (2, 'Excel para Negócios', 'Coursera', 30, 2),
                (3, 'Marketing Digital', 'Alura', 25, 3);
            ");

            // CANDIDATURAS
            migrationBuilder.Sql(@"
                INSERT INTO ""CANDIDATURAS"" (""id_candidatura"", ""usuario_id"", ""vaga_id"", ""data_candidatura"", ""status"")
                VALUES
                (1, 1, 1, now(), 'Pendente'),
                (2, 2, 2, now(), 'Aprovado'),
                (3, 3, 3, now(), 'Reprovado');
            ");

            // VAGA_HABILIDADE
            migrationBuilder.Sql(@"
                INSERT INTO ""VAGA_HABILIDADE"" (""id_vaga_habilidade"", ""vaga_id"", ""habilidade_id"")
                VALUES
                (1, 1, 1),
                (2, 1, 2),
                (3, 2, 3);
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("TRUNCATE TABLE \"CANDIDATURAS\" CASCADE;");
            migrationBuilder.Sql("TRUNCATE TABLE \"USUARIO_HABILIDADE\" CASCADE;");
            migrationBuilder.Sql("TRUNCATE TABLE \"VAGA_HABILIDADE\" CASCADE;");
            migrationBuilder.Sql("TRUNCATE TABLE \"CURSOS\" CASCADE;");
            migrationBuilder.Sql("TRUNCATE TABLE \"HABILIDADES\" CASCADE;");
            migrationBuilder.Sql("TRUNCATE TABLE \"VAGAS\" CASCADE;");
            migrationBuilder.Sql("TRUNCATE TABLE \"EMPRESAS\" CASCADE;");
            migrationBuilder.Sql("TRUNCATE TABLE \"USUARIOS\" CASCADE;");
        }
    }
}
