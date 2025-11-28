using Oracle.ManagedDataAccess.Client;
using System.Data;

public class OracleProcedureService
{
    private readonly string _connectionString;

    public OracleProcedureService(string connectionString)
    {
        _connectionString = connectionString;
    }

    public long InserirUsuario(string nome, string email, string senha)
    {
        using var conn = new OracleConnection(_connectionString);
        conn.Open();

        using var cmd = new OracleCommand("pkg_insertDados.sp_inserir_usuario", conn)
        {
            CommandType = CommandType.StoredProcedure
        };

        cmd.Parameters.Add("p_nome", OracleDbType.Varchar2).Value = nome;
        cmd.Parameters.Add("p_email", OracleDbType.Varchar2).Value = email;
        cmd.Parameters.Add("p_senha", OracleDbType.Varchar2).Value = senha;
        cmd.Parameters.Add("p_id_usuario", OracleDbType.Int64).Direction = ParameterDirection.Output;

        cmd.ExecuteNonQuery();
        return Convert.ToInt64(cmd.Parameters["p_id_usuario"].Value.ToString());
    }

    public long InserirEmpresa(string nome, string cnpj, string email, string senha)
    {
        using var conn = new OracleConnection(_connectionString);
        conn.Open();

        using var cmd = new OracleCommand("pkg_insertDados.sp_inserir_empresa", conn)
        {
            CommandType = CommandType.StoredProcedure
        };

        cmd.Parameters.Add("p_nome", OracleDbType.Varchar2).Value = nome;
        cmd.Parameters.Add("p_cnpj", OracleDbType.Varchar2).Value = cnpj;
        cmd.Parameters.Add("p_email", OracleDbType.Varchar2).Value = email;
        cmd.Parameters.Add("p_senha", OracleDbType.Varchar2).Value = senha;
        cmd.Parameters.Add("p_id_empresa", OracleDbType.Int64).Direction = ParameterDirection.Output;

        cmd.ExecuteNonQuery();
        return Convert.ToInt64(cmd.Parameters["p_id_empresa"].Value.ToString());
    }

    public long InserirVaga(string titulo, long empresaId)
    {
        using var conn = new OracleConnection(_connectionString);
        conn.Open();

        using var cmd = new OracleCommand("pkg_insertDados.sp_inserir_vaga", conn)
        {
            CommandType = CommandType.StoredProcedure
        };

        cmd.Parameters.Add("p_titulo", OracleDbType.Varchar2).Value = titulo;
        cmd.Parameters.Add("p_empresa_id", OracleDbType.Int64).Value = empresaId;
        cmd.Parameters.Add("p_id_vaga", OracleDbType.Int64).Direction = ParameterDirection.Output;

        cmd.ExecuteNonQuery();
        return Convert.ToInt64(cmd.Parameters["p_id_vaga"].Value.ToString());
    }

    public void InserirVagaHabilidade(long vagaId, long habId)
    {
        using var conn = new OracleConnection(_connectionString);
        conn.Open();

        using var cmd = new OracleCommand("pkg_insertDados.sp_inserir_vaga_habilidade", conn)
        {
            CommandType = CommandType.StoredProcedure
        };

        cmd.Parameters.Add("p_vaga_id", OracleDbType.Int64).Value = vagaId;
        cmd.Parameters.Add("p_habilidade_id", OracleDbType.Int64).Value = habId;

        cmd.ExecuteNonQuery();
    }

    public long InserirHabilidade(string nome)
    {
        using var conn = new OracleConnection(_connectionString);
        conn.Open();

        using var cmd = new OracleCommand("pkg_insertDados.sp_inserir_habilidade", conn)
        {
            CommandType = CommandType.StoredProcedure
        };

        cmd.Parameters.Add("p_nome", OracleDbType.Varchar2).Value = nome;
        cmd.Parameters.Add("p_id_habilidade", OracleDbType.Int64).Direction = ParameterDirection.Output;

        cmd.ExecuteNonQuery();
        return Convert.ToInt64(cmd.Parameters["p_id_habilidade"].Value.ToString());
    }

    public void InserirUsuarioHabilidade(long usuarioId, long habId)
    {
        using var conn = new OracleConnection(_connectionString);
        conn.Open();

        using var cmd = new OracleCommand("pkg_insertDados.sp_inserir_usuario_habilidade", conn)
        {
            CommandType = CommandType.StoredProcedure
        };

        cmd.Parameters.Add("p_usuario_id", OracleDbType.Int64).Value = usuarioId;
        cmd.Parameters.Add("p_habilidade_id", OracleDbType.Int64).Value = habId;

        cmd.ExecuteNonQuery();
    }

    public long InserirCurso(string nome, string instituicao, int cargaHoraria, long usuarioId)
    {
        using var conn = new OracleConnection(_connectionString);
        conn.Open();

        using var cmd = new OracleCommand("pkg_insertDados.sp_inserir_curso", conn)
        {
            CommandType = CommandType.StoredProcedure
        };

        cmd.Parameters.Add("p_nome", OracleDbType.Varchar2).Value = nome;
        cmd.Parameters.Add("p_instituicao", OracleDbType.Varchar2).Value = instituicao;
        cmd.Parameters.Add("p_carga_horaria", OracleDbType.Int32).Value = cargaHoraria;
        cmd.Parameters.Add("p_usuario_id", OracleDbType.Int64).Value = usuarioId;
        cmd.Parameters.Add("p_id_curso", OracleDbType.Int64).Direction = ParameterDirection.Output;

        cmd.ExecuteNonQuery();
        return Convert.ToInt64(cmd.Parameters["p_id_curso"].Value.ToString());
    }

    public long InserirCandidatura(long usuarioId, long vagaId)
    {
        using var conn = new OracleConnection(_connectionString);
        conn.Open();

        using var cmd = new OracleCommand("pkg_insertDados.sp_inserir_candidatura", conn)
        {
            CommandType = CommandType.StoredProcedure
        };

        cmd.Parameters.Add("p_usuario_id", OracleDbType.Int64).Value = usuarioId;
        cmd.Parameters.Add("p_vaga_id", OracleDbType.Int64).Value = vagaId;
        cmd.Parameters.Add("p_id_candidatura", OracleDbType.Int64).Direction = ParameterDirection.Output;

        cmd.ExecuteNonQuery();
        return Convert.ToInt64(cmd.Parameters["p_id_candidatura"].Value.ToString());
    }
}
