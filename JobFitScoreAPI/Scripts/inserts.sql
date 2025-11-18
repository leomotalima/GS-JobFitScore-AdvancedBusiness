BEGIN
    -- USUÁRIOS
    EXECUTE IMMEDIATE 'INSERT INTO "' || USER || '"."USUARIOS" ("id_usuario", "nome", "email", "senha")
                       VALUES (1, ''Admin'', ''admin@jobfitscore.com'', ''admin123'')';
    EXECUTE IMMEDIATE 'INSERT INTO "' || USER || '"."USUARIOS" ("id_usuario", "nome", "email", "senha")
                       VALUES (2, ''João Silva'', ''joao.silva@gmail.com'', ''senha123'')';

    -- EMPRESAS
    EXECUTE IMMEDIATE 'INSERT INTO "' || USER || '"."EMPRESAS" ("id_empresa", "nome", "cnpj", "email", "senha")
                       VALUES (1, ''TechFlow Ltda'', ''12345678000190'', ''contato@techflow.com'', ''senha123'')';
    EXECUTE IMMEDIATE 'INSERT INTO "' || USER || '"."EMPRESAS" ("id_empresa", "nome", "cnpj", "email", "senha")
                       VALUES (2, ''DataMind Solutions'', ''98765432000110'', ''contato@datamind.com'', ''senha123'')';

    -- HABILIDADES
    EXECUTE IMMEDIATE 'INSERT INTO "' || USER || '"."HABILIDADES" ("id_habilidade", "nome") VALUES (1, ''C#'')';
    EXECUTE IMMEDIATE 'INSERT INTO "' || USER || '"."HABILIDADES" ("id_habilidade", "nome") VALUES (2, ''SQL'')';
    EXECUTE IMMEDIATE 'INSERT INTO "' || USER || '"."HABILIDADES" ("id_habilidade", "nome") VALUES (3, ''Python'')';
    EXECUTE IMMEDIATE 'INSERT INTO "' || USER || '"."HABILIDADES" ("id_habilidade", "nome") VALUES (4, ''Power BI'')';

    -- VAGAS
    EXECUTE IMMEDIATE 'INSERT INTO "' || USER || '"."VAGAS" ("id_vaga", "titulo", "empresa_id") VALUES (1, ''Desenvolvedor Backend'', 1)';
    EXECUTE IMMEDIATE 'INSERT INTO "' || USER || '"."VAGAS" ("id_vaga", "titulo", "empresa_id") VALUES (2, ''Analista de Dados'', 2)';

    -- CURSOS
    EXECUTE IMMEDIATE 'INSERT INTO "' || USER || '"."CURSOS" ("id_curso", "nome", "usuario_id", "instituicao", "carga_horaria")
                       VALUES (1, ''Introdução a C#'', 1, ''FIAP'', 40)';
    EXECUTE IMMEDIATE 'INSERT INTO "' || USER || '"."CURSOS" ("id_curso", "nome", "usuario_id", "instituicao", "carga_horaria")
                       VALUES (2, ''SQL Avançado para Análise de Dados'', 1, ''FIAP'', 30)';

    -- CANDIDATURAS
    EXECUTE IMMEDIATE 'INSERT INTO "' || USER || '"."CANDIDATURAS" ("id_candidatura", "usuario_id", "vaga_id", "data_candidatura", "status")
                       VALUES (1, 1, 1, SYSDATE, ''Aprovado'')';
    EXECUTE IMMEDIATE 'INSERT INTO "' || USER || '"."CANDIDATURAS" ("id_candidatura", "usuario_id", "vaga_id", "data_candidatura", "status")
                       VALUES (2, 2, 2, SYSDATE, ''Aprovado'')';

    -- USUARIO_HABILIDADE
    EXECUTE IMMEDIATE 'INSERT INTO "' || USER || '"."USUARIO_HABILIDADE" ("id_usuario_habilidade", "usuario_id", "habilidade_id") VALUES (1, 1, 1)';
    EXECUTE IMMEDIATE 'INSERT INTO "' || USER || '"."USUARIO_HABILIDADE" ("id_usuario_habilidade", "usuario_id", "habilidade_id") VALUES (2, 1, 2)';
    EXECUTE IMMEDIATE 'INSERT INTO "' || USER || '"."USUARIO_HABILIDADE" ("id_usuario_habilidade", "usuario_id", "habilidade_id") VALUES (3, 2, 3)';
    EXECUTE IMMEDIATE 'INSERT INTO "' || USER || '"."USUARIO_HABILIDADE" ("id_usuario_habilidade", "usuario_id", "habilidade_id") VALUES (4, 2, 4)';

    -- VAGA_HABILIDADE
    EXECUTE IMMEDIATE 'INSERT INTO "' || USER || '"."VAGA_HABILIDADE" ("id_vaga_habilidade", "vaga_id", "habilidade_id") VALUES (1, 1, 1)';
    EXECUTE IMMEDIATE 'INSERT INTO "' || USER || '"."VAGA_HABILIDADE" ("id_vaga_habilidade", "vaga_id", "habilidade_id") VALUES (2, 1, 2)';
    EXECUTE IMMEDIATE 'INSERT INTO "' || USER || '"."VAGA_HABILIDADE" ("id_vaga_habilidade", "vaga_id", "habilidade_id") VALUES (3, 2, 3)';
    EXECUTE IMMEDIATE 'INSERT INTO "' || USER || '"."VAGA_HABILIDADE" ("id_vaga_habilidade", "vaga_id", "habilidade_id") VALUES (4, 2, 4)';

    COMMIT;
END;
/
