using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace ControleImoveis.Web.Models
{
    public class CidadeModel
    {

        #region Atributos

        public int Id { get; set; }
        public string Nome { get; set; }
        public bool Ativo { get; set; }
     
        [Required(ErrorMessage = "Selecione o país.")]
        public int IdPais { get; set; }

        [Required(ErrorMessage = "Selecione o estado.")]
        public int IdEstado { get; set; }

        public string NomePais { get; set; }

        public string NomeEstado { get; set; }

        #endregion

        #region Métodos

        public static int RecuperarQuantidade()
        {
            var ret = 0;

            using (var conexao = new SqlConnection())
            {
                conexao.ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString;
                conexao.Open();
                using (var comando = new SqlCommand())
                {
                    comando.Connection = conexao;
                    comando.CommandText = "select count(*) from estado";
                    ret = (int)comando.ExecuteScalar();
                }
            }

            return ret;
        }

        public static List<CidadeModel> RecuperarLista(int pagina = 0, int tamPagina = 0, string filtro = "", string ordem = "", int idEstado = 0)
        {
            var ret = new List<CidadeModel>();

            using (var conexao = new SqlConnection())
            {
                conexao.ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString;
                conexao.Open();

                using (var comando = new SqlCommand())
                {

                    var pos = (pagina - 1) * tamPagina;

                    var filtroWhere = "";
                    if (!string.IsNullOrEmpty(filtro))
                    {
                        filtroWhere = string.Format(" (lower(c.nome) like '%{0}%') and", filtro.ToLower());
                    }

                    if (idEstado > 0)
                    {
                        filtroWhere += string.Format(" (id_estado = {0}) and", idEstado);
                    }

                    var paginacao = "";
                    if (pagina > 0 && tamPagina > 0)
                    {
                        paginacao = string.Format(" offset {0} rows fetch next {1} rows only",
                            pos, tamPagina);
                    }

                    var sql =
                        "select c.id, c.nome, c.ativo, c.id_estado as IdEstado, e.id_pais as IdPais," +
                        " e.nome as NomeEstado, p.nome as NomePais" +
                        " from cidade c, estado e, pais p" +
                        " where" +
                        filtroWhere +
                        " (c.id_estado = e.id) and" +
                        " (e.id_pais = p.id)" +
                        " order by " + (!string.IsNullOrEmpty(ordem) ? ordem : "c.nome") +
                        paginacao;


                    comando.Connection = conexao;
                    comando.CommandText = sql;

                    SqlDataAdapter adapter = new SqlDataAdapter(comando);
                    DataSet ds = new DataSet();
                    adapter.Fill(ds);

                    if (ds != null || ds.Tables.Count > 0)
                    {
                        DataTable dt = ds.Tables[0];

                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            
                            CidadeModel cid = new CidadeModel();

                            if (ds.Tables[0].Rows[i]["id"] != DBNull.Value)
                                cid.Id = (int)ds.Tables[0].Rows[i]["id"];

                            if (ds.Tables[0].Rows[i]["Nome"] != DBNull.Value)
                                cid.Nome = ds.Tables[0].Rows[i]["Nome"].ToString();

                            if (ds.Tables[0].Rows[i]["ativo"] != DBNull.Value)
                                cid.Ativo = (bool)ds.Tables[0].Rows[i]["ativo"];

                            if (ds.Tables[0].Rows[i]["IdEstado"] != DBNull.Value)
                                cid.IdEstado = (int)ds.Tables[0].Rows[i]["IdEstado"];

                            if (ds.Tables[0].Rows[i]["IdPais"] != DBNull.Value)
                                cid.IdPais = (int)ds.Tables[0].Rows[i]["IdPais"];

                            if (ds.Tables[0].Rows[i]["NomeEstado"] != DBNull.Value)
                                cid.NomeEstado = ds.Tables[0].Rows[i]["NomeEstado"].ToString();

                            if (ds.Tables[0].Rows[i]["NomePais"] != DBNull.Value)
                                cid.NomePais = ds.Tables[0].Rows[i]["NomePais"].ToString();

                            ret.Add(cid);
                        }
                    }


                }
            }

            return ret;
        }

        public static CidadeModel RecuperarPeloId(int id)
        {
            CidadeModel ret = null;
            using (var conexao = new SqlConnection())
            {
                conexao.ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString;
                conexao.Open();
                using (var comando = new SqlCommand())
                {
                    comando.Connection = conexao;
                    comando.CommandText = "select c.id,c.nome,c.ativo,c.id_estado, e.id_pais  from cidade c inner join estado e on c.id_estado = e.id where (c.id = @id)";
                    comando.Parameters.Add("@id", SqlDbType.Int).Value = id;

                    var reader = comando.ExecuteReader();
                    if (reader.Read())
                    {
                        ret = new CidadeModel
                        {
                            Id = (int)reader["id"],
                            Nome = (string)reader["nome"],
                            IdPais = (int)reader["id_pais"],
                            IdEstado = (int)reader["id_Estado"],
                            Ativo = (bool)reader["ativo"]
                        };
                    }
                }
            }

            return ret;
        }

        public static bool ExcluirPeloId(int id)
        {
            var ret = false;

            if (RecuperarPeloId(id) != null)
            {
                using (var conexao = new SqlConnection())
                {
                    conexao.ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString;
                    conexao.Open();
                    using (var comando = new SqlCommand())
                    {
                        comando.Connection = conexao;
                        comando.CommandText = "delete from cidade where (id = @id)";

                        comando.Parameters.Add("id", SqlDbType.Int).Value = id;
                        ret = (comando.ExecuteNonQuery() > 0);
                    }
                }
            }

            return ret;
        }

        public int Salvar()
        {

            var ret = 0;

            var modelo = RecuperarPeloId(this.Id);

            using (var conexao = new SqlConnection())
            {
                conexao.ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString;
                conexao.Open();
                using (var comando = new SqlCommand())
                {
                    comando.Connection = conexao;

                    if (modelo == null)
                    {
                        comando.CommandText = "insert into Cidade (Nome,id_estado,ativo) values (@nome,@id_estado,@ativo); select convert(int, scope_identity())";

                        comando.Parameters.Add("@nome", SqlDbType.VarChar).Value = this.Nome;
                        comando.Parameters.Add("@id_estado", SqlDbType.Int).Value = this.IdEstado;
                        comando.Parameters.Add("@ativo", SqlDbType.VarChar).Value = (this.Ativo ? 1 : 0);



                        ret = (int)comando.ExecuteScalar();

                    }
                    else
                    {
                        comando.CommandText =
                            "update Cidade set nome=@nome, id_estado = @id_estado,ativo = @ativo" +
                            " where id = @id";

                        comando.Parameters.Add("@id", SqlDbType.VarChar).Value = this.Id;
                        comando.Parameters.Add("@nome", SqlDbType.VarChar).Value = this.Nome;
                        comando.Parameters.Add("@id_estado", SqlDbType.VarChar).Value = this.IdEstado;
                        comando.Parameters.Add("@ativo", SqlDbType.VarChar).Value = (this.Ativo ? 1 : 0);


                        if (comando.ExecuteNonQuery() > 0)
                        {
                            ret = this.Id;
                        }
                    }
                }
            }

            return ret;
        }

        #endregion

    }
}