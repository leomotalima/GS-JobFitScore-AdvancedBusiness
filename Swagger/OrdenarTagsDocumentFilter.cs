using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.OpenApi.Models;

namespace JobFitScoreAPI.Swagger
{
    public class OrdenarTagsDocumentFilter : IDocumentFilter
    {
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            string[] ordemTags =
            {
                "Autenticacao",
                "Usuários",
                "Empresas",
                "Habilidades",
                "UsuariosHabilidades",
                "Cursos",
                "Vagas",
                "VagaHabilidade",
                "Candidaturas",
                "JobFitScoreAPI"
            };

            swaggerDoc.Tags = swaggerDoc.Tags?
                .OrderBy(t =>
                {
                    int index = Array.IndexOf(ordemTags, t.Name);
                    return index >= 0 ? index : int.MaxValue;
                })
                .ToList();

            var orderedPaths = new OpenApiPaths();

            foreach (var tag in ordemTags)
            {
                foreach (var path in swaggerDoc.Paths
                    .Where(p => p.Value.Operations
                        .Any(op => op.Value.Tags.Any(t => t.Name == tag))))
                {
                    if (!orderedPaths.ContainsKey(path.Key))
                        orderedPaths.Add(path.Key, path.Value);
                }
            }

            foreach (var path in swaggerDoc.Paths)
            {
                if (!orderedPaths.ContainsKey(path.Key))
                    orderedPaths.Add(path.Key, path.Value);
            }

            swaggerDoc.Paths = orderedPaths;
        }
    }
}
