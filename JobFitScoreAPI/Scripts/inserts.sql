BEGIN
    EXECUTE IMMEDIATE 'INSERT INTO "' || USER || '"."Usuarios" ("nome", "email", "senha", "habilidades")
                       VALUES (''Admin'', ''admin@jobfitscore.com'', ''admin123'', ''C#, SQL, .NET'')';

    EXECUTE IMMEDIATE 'INSERT INTO "' || USER || '"."Vagas" ("titulo", "requisitos", "empresa")
                       VALUES (''Desenvolvedor Backend'', ''C#, API REST, SQL Server'', ''TechFlow Ltda'')';

    EXECUTE IMMEDIATE 'INSERT INTO "' || USER || '"."Vagas" ("titulo", "requisitos", "empresa")
                       VALUES (''Analista de Dados'', ''Python, Power BI, SQL'', ''DataMind Solutions'')';

    EXECUTE IMMEDIATE 'INSERT INTO "' || USER || '"."Cursos" ("nome", "habilidade_relacionada")
                       VALUES (''Introdução a C#'', ''C#'')';

    EXECUTE IMMEDIATE 'INSERT INTO "' || USER || '"."Cursos" ("nome", "habilidade_relacionada")
                       VALUES (''SQL Avançado para Análise de Dados'', ''SQL'')';

    EXECUTE IMMEDIATE 'INSERT INTO "' || USER || '"."Candidaturas" ("id_usuario", "id_vaga", "score", "data_candidatura")
                       VALUES (1, 1, 85, SYSDATE)';

    COMMIT;
END;
/
