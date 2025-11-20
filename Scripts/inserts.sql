BEGIN
    -- 1. TRUNCATE
    EXECUTE IMMEDIATE 'TRUNCATE TABLE "' || USER || '"."CANDIDATURAS"';
    EXECUTE IMMEDIATE 'TRUNCATE TABLE "' || USER || '"."USUARIO_HABILIDADE"';
    EXECUTE IMMEDIATE 'TRUNCATE TABLE "' || USER || '"."VAGA_HABILIDADE"';
    EXECUTE IMMEDIATE 'TRUNCATE TABLE "' || USER || '"."CURSOS"';
    EXECUTE IMMEDIATE 'TRUNCATE TABLE "' || USER || '"."HABILIDADES"';
    EXECUTE IMMEDIATE 'TRUNCATE TABLE "' || USER || '"."VAGAS"';
    EXECUTE IMMEDIATE 'TRUNCATE TABLE "' || USER || '"."EMPRESAS"';
    EXECUTE IMMEDIATE 'TRUNCATE TABLE "' || USER || '"."USUARIOS"';

    /*=======================
    2. USUARIOS
    Senhas:
    - Admin: admin
    - Maria Souza: maria
    - Carlos Lima: carlos
    =======================*/
    EXECUTE IMMEDIATE 'INSERT INTO "' || USER || '"."USUARIOS" 
        ("id_usuario","nome","email","senha","refresh_token","expira_refresh_token")
        VALUES (1,''Admin'',''admin@jobfitscore.com'',
        ''$2a$12$3uKG7GyamTjjg2FLKKkbgOOYtw7.Ky2r9VfH.LSXEC9oE01/cEMy6'',NULL,NULL)';

    EXECUTE IMMEDIATE 'INSERT INTO "' || USER || '"."USUARIOS" 
        ("id_usuario","nome","email","senha","refresh_token","expira_refresh_token")
        VALUES (2,''Maria Souza'',''maria@email.com'',
        ''$2a$12$PZ8DNWydk98kLXei361IAuPK9dLkV50lhS2r.AvLoCUCjG7VLwLMW'',NULL,NULL)';

    EXECUTE IMMEDIATE 'INSERT INTO "' || USER || '"."USUARIOS" 
        ("id_usuario","nome","email","senha","refresh_token","expira_refresh_token")
        VALUES (3,''Carlos Lima'',''carlos@email.com'',
        ''$2a$12$uxhQky4PasnB2CQmR44Yee1yI0MbXlJUEwK88uz1.paXcbz1ew.k.'',NULL,NULL)';

    /*=======================
    3. EMPRESAS
    Senhas:
    - Empresa Alpha: alpha
    - Empresa Beta: beta
    - Empresa Gamma: gamma
    =======================*/
    EXECUTE IMMEDIATE 'INSERT INTO "' || USER || '"."EMPRESAS"
        ("id_empresa","nome","cnpj","email","senha","refresh_token","expira_refresh_token")
        VALUES (1,''Empresa Alpha'',''12345678000100'',''contato@alpha.com'',
        ''$2a$12$iGbpaLY9ozT0UY0siGYewOjcl8rvigWNS4qLKp8S3g5hSR0XIZ7qi'',NULL,NULL)';

    EXECUTE IMMEDIATE 'INSERT INTO "' || USER || '"."EMPRESAS"
        ("id_empresa","nome","cnpj","email","senha","refresh_token","expira_refresh_token")
        VALUES (2,''Empresa Beta'',''23456789000111'',''contato@beta.com'',
        ''$2a$12$SJ.afe1EDlzCKEaSjnHmHO1QHZXtcZGiwDroFTqF8hYWkDgOXSw1.'',NULL,NULL)';

    EXECUTE IMMEDIATE 'INSERT INTO "' || USER || '"."EMPRESAS"
        ("id_empresa","nome","cnpj","email","senha","refresh_token","expira_refresh_token")
        VALUES (3,''Empresa Gamma'',''34567890000122'',''contato@gamma.com'',
        ''$2a$12$FLy31xZVIUG2xV4luEKr7efUN0KeVUTH1gx30d3Osq4.mun5D20lS'',NULL,NULL)';

    -- 4. VAGAS
    EXECUTE IMMEDIATE 'INSERT INTO "' || USER || '"."VAGAS" 
        ("id_vaga","titulo","empresa_id") VALUES (1,''Desenvolvedor .NET'',1)';

    EXECUTE IMMEDIATE 'INSERT INTO "' || USER || '"."VAGAS" 
        ("id_vaga","titulo","empresa_id") VALUES (2,''Analista de Marketing'',2)';

    EXECUTE IMMEDIATE 'INSERT INTO "' || USER || '"."VAGAS" 
        ("id_vaga","titulo","empresa_id") VALUES (3,''Engenheiro de Dados'',3)';

    -- 5. HABILIDADES
    EXECUTE IMMEDIATE 'INSERT INTO "' || USER || '"."HABILIDADES"
        ("id_habilidade","nome") VALUES (1,''C#'')';

    EXECUTE IMMEDIATE 'INSERT INTO "' || USER || '"."HABILIDADES"
        ("id_habilidade","nome") VALUES (2,''SQL'')';

    EXECUTE IMMEDIATE 'INSERT INTO "' || USER || '"."HABILIDADES"
        ("id_habilidade","nome") VALUES (3,''Marketing Digital'')';

    -- 6. USUARIO_HABILIDADE
    EXECUTE IMMEDIATE 'INSERT INTO "' || USER || '"."USUARIO_HABILIDADE"
        ("id_usuario_habilidade","usuario_id","habilidade_id") VALUES (1,1,1)';

    EXECUTE IMMEDIATE 'INSERT INTO "' || USER || '"."USUARIO_HABILIDADE"
        ("id_usuario_habilidade","usuario_id","habilidade_id") VALUES (2,1,2)';

    EXECUTE IMMEDIATE 'INSERT INTO "' || USER || '"."USUARIO_HABILIDADE"
        ("id_usuario_habilidade","usuario_id","habilidade_id") VALUES (3,2,3)';

    -- 7. CURSOS
    EXECUTE IMMEDIATE 'INSERT INTO "' || USER || '"."CURSOS"
        ("id_curso","nome","instituicao","carga_horaria","usuario_id")
        VALUES (1,''Curso C# Avançado'',''Udemy'',40,1)';

    EXECUTE IMMEDIATE 'INSERT INTO "' || USER || '"."CURSOS"
        ("id_curso","nome","instituicao","carga_horaria","usuario_id")
        VALUES (2,''Excel para Negócios'',''Coursera'',30,2)';

    EXECUTE IMMEDIATE 'INSERT INTO "' || USER || '"."CURSOS"
        ("id_curso","nome","instituicao","carga_horaria","usuario_id")
        VALUES (3,''Marketing Digital'',''Alura'',25,3)';

    -- 8. CANDIDATURAS
    EXECUTE IMMEDIATE 'INSERT INTO "' || USER || '"."CANDIDATURAS"
        ("id_candidatura","usuario_id","vaga_id","data_candidatura","status")
        VALUES (1,1,1,SYSDATE,''Pendente'')';

    EXECUTE IMMEDIATE 'INSERT INTO "' || USER || '"."CANDIDATURAS"
        ("id_candidatura","usuario_id","vaga_id","data_candidatura","status")
        VALUES (2,2,2,SYSDATE,''Aprovado'')';

    EXECUTE IMMEDIATE 'INSERT INTO "' || USER || '"."CANDIDATURAS"
        ("id_candidatura","usuario_id","vaga_id","data_candidatura","status")
        VALUES (3,3,3,SYSDATE,''Reprovado'')';

    -- 9. VAGA_HABILIDADE
    EXECUTE IMMEDIATE 'INSERT INTO "' || USER || '"."VAGA_HABILIDADE"
        ("id_vaga_habilidade","vaga_id","habilidade_id") VALUES (1,1,1)';

    EXECUTE IMMEDIATE 'INSERT INTO "' || USER || '"."VAGA_HABILIDADE"
        ("id_vaga_habilidade","vaga_id","habilidade_id") VALUES (2,1,2)';

    EXECUTE IMMEDIATE 'INSERT INTO "' || USER || '"."VAGA_HABILIDADE"
        ("id_vaga_habilidade","vaga_id","habilidade_id") VALUES (3,2,3)';

    COMMIT;
END;
