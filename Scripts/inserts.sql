--SELECIONE O CÓDIGO INTEIRO ABAIXO E EXECUTE NO SEU BANCO DE DADOS PARA INSERIR DADOS INICIAIS.

INSERT INTO "USUARIOS" ("nome", "email", "senha")
VALUES ('Gerson', 'gerson@email.com', 'gerson123');
INSERT INTO "USUARIOS" ("nome", "email", "senha")
VALUES ('Admin', 'admin@jobfitscore.com', 'admin123');

INSERT INTO "EMPRESAS" ("nome", "cnpj", "email", "senha")
VALUES ('TechFlow Ltda', '00000000000000', 'contato@techflow.com', '123');

INSERT INTO "EMPRESAS" ("nome", "cnpj", "email", "senha")
VALUES ('DataMind Solutions', '11111111111111', 'contato@datamind.com', 'data789');

INSERT INTO "VAGAS" ("titulo", "empresa_id")
VALUES ('Desenvolvedor Backend', 1);
INSERT INTO "VAGAS" ("titulo", "empresa_id")
VALUES ('Analista de Dados', 2);

INSERT INTO "HABILIDADES" ("nome") VALUES ('C#');
INSERT INTO "HABILIDADES" ("nome") VALUES ('SQL');
INSERT INTO "HABILIDADES" ("nome") VALUES ('.NET');
INSERT INTO "HABILIDADES" ("nome") VALUES ('Python');
INSERT INTO "HABILIDADES" ("nome") VALUES ('Power BI');

INSERT INTO "USUARIO_HABILIDADE" ("usuario_id", "habilidade_id", "id_usuario_habilidade")
VALUES (1, 1, 1);
INSERT INTO "USUARIO_HABILIDADE" ("usuario_id", "habilidade_id", "id_usuario_habilidade")
VALUES (1, 2, 2);

INSERT INTO "CURSOS" ("nome", "instituicao", "carga_horaria", "usuario_id")
VALUES ('Introdução a C#', 'FIAP', 20, 1);
INSERT INTO "CURSOS" ("nome", "instituicao", "carga_horaria", "usuario_id")
VALUES ('SQL Avançado para Análise de Dados', 'FIAP', 40, 1);

INSERT INTO "CANDIDATURAS" ("usuario_id", "vaga_id", "data_candidatura", "status")
VALUES (1, 1, SYSDATE, 'ENVIADO');

COMMIT;