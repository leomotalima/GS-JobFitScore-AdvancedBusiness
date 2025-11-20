BEGIN
    -- =======================================
    -- 1. LIMPEZA: TRUNCATE TABLES (Para garantir reexecutabilidade)
    -- Ordem: Tabelas Filhas -> Tabelas Pais (devido às Foreign Keys)
    -- =======================================
    EXECUTE IMMEDIATE 'TRUNCATE TABLE "' || USER || '"."AUDITORIA_LOG"';
    EXECUTE IMMEDIATE 'TRUNCATE TABLE "' || USER || '"."CANDIDATURAS"';
    EXECUTE IMMEDIATE 'TRUNCATE TABLE "' || USER || '"."USUARIO_HABILIDADE"';
    EXECUTE IMMEDIATE 'TRUNCATE TABLE "' || USER || '"."VAGA_HABILIDADE"';
    EXECUTE IMMEDIATE 'TRUNCATE TABLE "' || USER || '"."CURSOS"';
    EXECUTE IMMEDIATE 'TRUNCATE TABLE "' || USER || '"."HABILIDADES"';
    EXECUTE IMMEDIATE 'TRUNCATE TABLE "' || USER || '"."VAGAS"';
    EXECUTE IMMEDIATE 'TRUNCATE TABLE "' || USER || '"."EMPRESAS"';
    EXECUTE IMMEDIATE 'TRUNCATE TABLE "' || USER || '"."USUARIOS"';

    -- =======================
    -- 2. USUARIOS
    -- =======================
    EXECUTE IMMEDIATE 'INSERT INTO "' || USER || '"."USUARIOS" ("ID_USUARIO", "NOME", "EMAIL", "SENHA", "REFRESH_TOKEN", "EXPIRA_REFRESH_TOKEN") VALUES (1, ''Admin'', ''admin@jobfitscore.com'', ''$2a$12$5qhNzR4ihwdEV1.xQCgGeeZIkYl5eeJ9WTH6R993q3StGiD5vQ7HO'', NULL, NULL)';
    EXECUTE IMMEDIATE 'INSERT INTO "' || USER || '"."USUARIOS" ("ID_USUARIO", "NOME", "EMAIL", "SENHA", "REFRESH_TOKEN", "EXPIRA_REFRESH_TOKEN") VALUES (2, ''Maria Souza'', ''maria@email.com'', ''$2a$12$ExemploHash2'', NULL, NULL)';
    EXECUTE IMMEDIATE 'INSERT INTO "' || USER || '"."USUARIOS" ("ID_USUARIO", "NOME", "EMAIL", "SENHA", "REFRESH_TOKEN", "EXPIRA_REFRESH_TOKEN") VALUES (3, ''Carlos Lima'', ''carlos@email.com'', ''$2a$12$ExemploHash3'', NULL, NULL)';

    -- =======================
    -- 3. EMPRESAS
    -- =======================
    EXECUTE IMMEDIATE 'INSERT INTO "' || USER || '"."EMPRESAS" ("ID_EMPRESA", "NOME", "CNPJ", "EMAIL", "SENHA", "REFRESH_TOKEN", "EXPIRA_REFRESH_TOKEN") VALUES (1, ''Empresa Alpha'', ''12345678000100'', ''contato@alpha.com'', ''$2a$12$ExemploHash1'', NULL, NULL)';
    EXECUTE IMMEDIATE 'INSERT INTO "' || USER || '"."EMPRESAS" ("ID_EMPRESA", "NOME", "CNPJ", "EMAIL", "SENHA", "REFRESH_TOKEN", "EXPIRA_REFRESH_TOKEN") VALUES (2, ''Empresa Beta'', ''23456789000111'', ''contato@beta.com'', ''$2a$12$ExemploHash2'', NULL, NULL)';
    EXECUTE IMMEDIATE 'INSERT INTO "' || USER || '"."EMPRESAS" ("ID_EMPRESA", "NOME", "CNPJ", "EMAIL", "SENHA", "REFRESH_TOKEN", "EXPIRA_REFRESH_TOKEN") VALUES (3, ''Empresa Gamma'', ''34567890000122'', ''contato@gamma.com'', ''$2a$12$ExemploHash3'', NULL, NULL)';

    -- =======================
    -- 4. VAGAS
    -- =======================
    EXECUTE IMMEDIATE 'INSERT INTO "' || USER || '"."VAGAS" ("ID_VAGA", "TITULO", "EMPRESA_ID") VALUES (1, ''Desenvolvedor .NET'', 1)';
    EXECUTE IMMEDIATE 'INSERT INTO "' || USER || '"."VAGAS" ("ID_VAGA", "TITULO", "EMPRESA_ID") VALUES (2, ''Analista de Marketing'', 2)';
    EXECUTE IMMEDIATE 'INSERT INTO "' || USER || '"."VAGAS" ("ID_VAGA", "TITULO", "EMPRESA_ID") VALUES (3, ''Engenheiro de Dados'', 3)';

    -- =======================
    -- 5. HABILIDADES
    -- =======================
    EXECUTE IMMEDIATE 'INSERT INTO "' || USER || '"."HABILIDADES" ("ID_HABILIDADE", "NOME") VALUES (1, ''C#'')';
    EXECUTE IMMEDIATE 'INSERT INTO "' || USER || '"."HABILIDADES" ("ID_HABILIDADE", "NOME") VALUES (2, ''SQL'')';
    EXECUTE IMMEDIATE 'INSERT INTO "' || USER || '"."HABILIDADES" ("ID_HABILIDADE", "NOME") VALUES (3, ''Marketing Digital'')';

    -- =======================
    -- 6. USUARIO_HABILIDADE
    -- =======================
    EXECUTE IMMEDIATE 'INSERT INTO "' || USER || '"."USUARIO_HABILIDADE" ("ID_USUARIO_HABILIDADE", "USUARIO_ID", "HABILIDADE_ID") VALUES (1, 1, 1)';
    EXECUTE IMMEDIATE 'INSERT INTO "' || USER || '"."USUARIO_HABILIDADE" ("ID_USUARIO_HABILIDADE", "USUARIO_ID", "HABILIDADE_ID") VALUES (2, 1, 2)';
    EXECUTE IMMEDIATE 'INSERT INTO "' || USER || '"."USUARIO_HABILIDADE" ("ID_USUARIO_HABILIDADE", "USUARIO_ID", "HABILIDADE_ID") VALUES (3, 2, 3)';

    -- =======================
    -- 7. CURSOS
    -- =======================
    EXECUTE IMMEDIATE 'INSERT INTO "' || USER || '"."CURSOS" ("ID_CURSO", "NOME", "INSTITUICAO", "CARGA_HORARIA", "USUARIO_ID") VALUES (1, ''Curso C# Avançado'', ''Udemy'', 40, 1)';
    EXECUTE IMMEDIATE 'INSERT INTO "' || USER || '"."CURSOS" ("ID_CURSO", "NOME", "INSTITUICAO", "CARGA_HORARIA", "USUARIO_ID") VALUES (2, ''Excel para Negócios'', ''Coursera'', 30, 2)';
    EXECUTE IMMEDIATE 'INSERT INTO "' || USER || '"."CURSOS" ("ID_CURSO", "NOME", "INSTITUICAO", "CARGA_HORARIA", "USUARIO_ID") VALUES (3, ''Marketing Digital'', ''Alura'', 25, 3)';

    -- =======================
    -- 8. CANDIDATURAS
    -- =======================
    EXECUTE IMMEDIATE 'INSERT INTO "' || USER || '"."CANDIDATURAS" ("ID_CANDIDATURA", "USUARIO_ID", "VAGA_ID", "DATA_CANDIDATURA", "STATUS") VALUES (1, 1, 1, SYSDATE, ''Pendente'')';
    EXECUTE IMMEDIATE 'INSERT INTO "' || USER || '"."CANDIDATURAS" ("ID_CANDIDATURA", "USUARIO_ID", "VAGA_ID", "DATA_CANDIDATURA", "STATUS") VALUES (2, 2, 2, SYSDATE, ''Aprovado'')';
    EXECUTE IMMEDIATE 'INSERT INTO "' || USER || '"."CANDIDATURAS" ("ID_CANDIDATURA", "USUARIO_ID", "VAGA_ID", "DATA_CANDIDATURA", "STATUS") VALUES (3, 3, 3, SYSDATE, ''Reprovado'')';

    -- =======================
    -- 9. VAGA_HABILIDADE
    -- =======================
    EXECUTE IMMEDIATE 'INSERT INTO "' || USER || '"."VAGA_HABILIDADE" ("ID_VAGA_HABILIDADE", "VAGA_ID", "HABILIDADE_ID") VALUES (1, 1, 1)';
    EXECUTE IMMEDIATE 'INSERT INTO "' || USER || '"."VAGA_HABILIDADE" ("ID_VAGA_HABILIDADE", "VAGA_ID", "HABILIDADE_ID") VALUES (2, 1, 2)';
    EXECUTE IMMEDIATE 'INSERT INTO "' || USER || '"."VAGA_HABILIDADE" ("ID_VAGA_HABILIDADE", "VAGA_ID", "HABILIDADE_ID") VALUES (3, 2, 3)';

    -- =======================
    -- 10. AUDITORIA_LOG (CORRIGIDO: Adicionado DATA_OPERACAO e SYSDATE)
    -- =======================
    EXECUTE IMMEDIATE 'INSERT INTO "' || USER || '"."AUDITORIA_LOG" ("ID_AUDITORIA", "NOME_TABELA", "OPERACAO", "REGISTRO_ID", "DETALHE", "DATA_OPERACAO") VALUES (1, ''USUARIOS'', ''INSERT'', 1, ''Seed inicial de dados para testes'', SYSDATE)';
    EXECUTE IMMEDIATE 'INSERT INTO "' || USER || '"."AUDITORIA_LOG" ("ID_AUDITORIA", "NOME_TABELA", "OPERACAO", "REGISTRO_ID", "DETALHE", "DATA_OPERACAO") VALUES (2, ''EMPRESAS'', ''INSERT'', 1, ''Seed inicial de dados para testes'', SYSDATE)';
    EXECUTE IMMEDIATE 'INSERT INTO "' || USER || '"."AUDITORIA_LOG" ("ID_AUDITORIA", "NOME_TABELA", "OPERACAO", "REGISTRO_ID", "DETALHE", "DATA_OPERACAO") VALUES (3, ''VAGAS'', ''INSERT'', 1, ''Seed inicial de dados para testes'', SYSDATE)';

    -- =======================
    -- 11. FINALIZAÇÃO
    -- =======================
    COMMIT;

END;
/