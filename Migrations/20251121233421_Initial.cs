using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JobFitScoreAPI.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EMPRESAS",
                columns: table => new
                {
                    id_empresa = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    nome = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    cnpj = table.Column<string>(type: "NVARCHAR2(14)", maxLength: 14, nullable: false),
                    email = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    senha = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: false),
                    refresh_token = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: true),
                    expira_refresh_token = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EMPRESAS", x => x.id_empresa);
                });

            migrationBuilder.CreateTable(
                name: "HABILIDADES",
                columns: table => new
                {
                    id_habilidade = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    nome = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    categoria = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: true),
                    descricao = table.Column<string>(type: "NVARCHAR2(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HABILIDADES", x => x.id_habilidade);
                });

            migrationBuilder.CreateTable(
                name: "USUARIOS",
                columns: table => new
                {
                    id_usuario = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    nome = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    email = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    senha = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: false),
                    refresh_token = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: true),
                    expira_refresh_token = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_USUARIOS", x => x.id_usuario);
                });

            migrationBuilder.CreateTable(
                name: "VAGAS",
                columns: table => new
                {
                    id_vaga = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    titulo = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    empresa_id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VAGAS", x => x.id_vaga);
                    table.ForeignKey(
                        name: "FK_VAGAS_EMPRESAS_empresa_id",
                        column: x => x.empresa_id,
                        principalTable: "EMPRESAS",
                        principalColumn: "id_empresa",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CURSOS",
                columns: table => new
                {
                    id_curso = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    nome = table.Column<string>(type: "NVARCHAR2(150)", maxLength: 150, nullable: false),
                    instituicao = table.Column<string>(type: "NVARCHAR2(150)", maxLength: 150, nullable: true),
                    carga_horaria = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    data_conclusao = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    descricao = table.Column<string>(type: "NVARCHAR2(500)", maxLength: 500, nullable: true),
                    usuario_id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CURSOS", x => x.id_curso);
                    table.ForeignKey(
                        name: "FK_CURSOS_USUARIOS_usuario_id",
                        column: x => x.usuario_id,
                        principalTable: "USUARIOS",
                        principalColumn: "id_usuario",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "USUARIO_HABILIDADE",
                columns: table => new
                {
                    id_usuario_habilidade = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    usuario_id = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    habilidade_id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_USUARIO_HABILIDADE", x => x.id_usuario_habilidade);
                    table.ForeignKey(
                        name: "FK_USUARIO_HABILIDADE_HABILIDADES_habilidade_id",
                        column: x => x.habilidade_id,
                        principalTable: "HABILIDADES",
                        principalColumn: "id_habilidade",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_USUARIO_HABILIDADE_USUARIOS_usuario_id",
                        column: x => x.usuario_id,
                        principalTable: "USUARIOS",
                        principalColumn: "id_usuario",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CANDIDATURAS",
                columns: table => new
                {
                    id_candidatura = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    usuario_id = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    vaga_id = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    data_candidatura = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    status = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CANDIDATURAS", x => x.id_candidatura);
                    table.ForeignKey(
                        name: "FK_CANDIDATURAS_USUARIOS_usuario_id",
                        column: x => x.usuario_id,
                        principalTable: "USUARIOS",
                        principalColumn: "id_usuario",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CANDIDATURAS_VAGAS_vaga_id",
                        column: x => x.vaga_id,
                        principalTable: "VAGAS",
                        principalColumn: "id_vaga",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VAGA_HABILIDADE",
                columns: table => new
                {
                    id_vaga_habilidade = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    vaga_id = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    habilidade_id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VAGA_HABILIDADE", x => x.id_vaga_habilidade);
                    table.ForeignKey(
                        name: "FK_VAGA_HABILIDADE_HABILIDADES_habilidade_id",
                        column: x => x.habilidade_id,
                        principalTable: "HABILIDADES",
                        principalColumn: "id_habilidade",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VAGA_HABILIDADE_VAGAS_vaga_id",
                        column: x => x.vaga_id,
                        principalTable: "VAGAS",
                        principalColumn: "id_vaga",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CANDIDATURAS_usuario_id",
                table: "CANDIDATURAS",
                column: "usuario_id");

            migrationBuilder.CreateIndex(
                name: "IX_CANDIDATURAS_vaga_id",
                table: "CANDIDATURAS",
                column: "vaga_id");

            migrationBuilder.CreateIndex(
                name: "IX_CURSOS_usuario_id",
                table: "CURSOS",
                column: "usuario_id");

            migrationBuilder.CreateIndex(
                name: "IX_USUARIO_HABILIDADE_habilidade_id",
                table: "USUARIO_HABILIDADE",
                column: "habilidade_id");

            migrationBuilder.CreateIndex(
                name: "IX_USUARIO_HABILIDADE_usuario_id",
                table: "USUARIO_HABILIDADE",
                column: "usuario_id");

            migrationBuilder.CreateIndex(
                name: "IX_VAGA_HABILIDADE_habilidade_id",
                table: "VAGA_HABILIDADE",
                column: "habilidade_id");

            migrationBuilder.CreateIndex(
                name: "IX_VAGA_HABILIDADE_vaga_id",
                table: "VAGA_HABILIDADE",
                column: "vaga_id");

            migrationBuilder.CreateIndex(
                name: "IX_VAGAS_empresa_id",
                table: "VAGAS",
                column: "empresa_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CANDIDATURAS");

            migrationBuilder.DropTable(
                name: "CURSOS");

            migrationBuilder.DropTable(
                name: "USUARIO_HABILIDADE");

            migrationBuilder.DropTable(
                name: "VAGA_HABILIDADE");

            migrationBuilder.DropTable(
                name: "USUARIOS");

            migrationBuilder.DropTable(
                name: "HABILIDADES");

            migrationBuilder.DropTable(
                name: "VAGAS");

            migrationBuilder.DropTable(
                name: "EMPRESAS");
        }
    }
}
