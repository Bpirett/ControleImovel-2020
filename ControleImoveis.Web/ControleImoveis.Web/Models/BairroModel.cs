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
    public class BairroModel
    {
        #region Atributos

        public enum Regiao
        {
            [Display(Name = "Zona Norte")]
            Norte,
            [Display(Name = "Zona Sul")]
            Sul,
            [Display(Name = "Zona Oeste")]
            Oeste,
            [Display(Name = "Zona Leste")]
            Leste
        }

        public int Id { get; set; }
        public string Nome { get; set; }
        public bool Ativo { get; set; }

        [Required(ErrorMessage = "Selecione o país.")]
        public int IdPais { get; set; }

        [Required(ErrorMessage = "Selecione o estado.")]
        public int IdEstado { get; set; }


        [Required(ErrorMessage = "Selecione o cidade.")]
        public int IdCidade { get; set; }

        public string NomePais { get; set; }

        public string NomeEstado { get; set; }

        public string NomeCidade { get; set; }

        public string NomeRegiao { get; set; }

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
                    comando.CommandText = "select count(*) from Bairro";
                    ret = (int)comando.ExecuteScalar();
                }
            }

            return ret;
        }

        public static List<BairroModel> RecuperarLista(int pagina = 0, int tamPagina = 0, string filtro = "", string ordem = "", int IdCidade = 0, string regiao = "")
        {
            var ret = new List<BairroModel>();

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
                        filtroWhere = string.Format(" (lower(c.nome) like '%{0}%') and ", filtro.ToLower());
                    }

                    if (IdCidade > 0)
                    {
                        filtroWhere += string.Format(" (id_cidade = {0}) and ", IdCidade);
                    }

                    if (!string.IsNullOrEmpty(regiao))
                    {
                        filtroWhere += string.Format(" (b.regiao = @regiao) and ");

                        comando.Parameters.Add("@regiao", SqlDbType.VarChar).Value = regiao;
                    }

                    var paginacao = "";
                    if (pagina > 0 && tamPagina > 0)
                    {
                        paginacao = string.Format(" offset {0} rows fetch next {1} rows only ",
                            pos, tamPagina);
                    }

                    var sql =
                        "select b.id, b.nome, b.ativo, b.id_cidade as IdCidade, b.regiao as NomeRegiao, c.id_estado as IdEstado, e.id_pais as IdPais, e.nome as NomeEstado, c.nome as NomeCidade," +
                        "p.nome as NomePais " +
                        "from Bairro b, cidade c,estado e, pais p " +
                        " where " +
                        filtroWhere +
                     "(b.id_cidade = c.id) and (c.id_estado = e.id) and (e.id_pais = p.id)" +
                      " order by "
                      + (!string.IsNullOrEmpty(ordem) ? ordem : "c.nome") +
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

                            BairroModel bairro = new BairroModel();

                            if (ds.Tables[0].Rows[i]["id"] != DBNull.Value)
                                bairro.Id = (int)ds.Tables[0].Rows[i]["id"];

                            if (ds.Tables[0].Rows[i]["Nome"] != DBNull.Value)
                                bairro.Nome = ds.Tables[0].Rows[i]["Nome"].ToString();

                            if (ds.Tables[0].Rows[i]["ativo"] != DBNull.Value)
                                bairro.Ativo = (bool)ds.Tables[0].Rows[i]["ativo"];

                            if (ds.Tables[0].Rows[i]["IdEstado"] != DBNull.Value)
                                bairro.IdEstado = (int)ds.Tables[0].Rows[i]["IdEstado"];

                            if (ds.Tables[0].Rows[i]["IdPais"] != DBNull.Value)
                                bairro.IdPais = (int)ds.Tables[0].Rows[i]["IdPais"];

                            if (ds.Tables[0].Rows[i]["IdCidade"] != DBNull.Value)
                                bairro.IdCidade = (int)ds.Tables[0].Rows[i]["IdCidade"];

                            if (ds.Tables[0].Rows[i]["NomeEstado"] != DBNull.Value)
                                bairro.NomeEstado = ds.Tables[0].Rows[i]["NomeEstado"].ToString();

                            if (ds.Tables[0].Rows[i]["NomePais"] != DBNull.Value)
                                bairro.NomePais = ds.Tables[0].Rows[i]["NomePais"].ToString();

                            if (ds.Tables[0].Rows[i]["NomeCidade"] != DBNull.Value)
                                bairro.NomeCidade = ds.Tables[0].Rows[i]["NomeCidade"].ToString();

                            if (ds.Tables[0].Rows[i]["NomeRegiao"] != DBNull.Value)
                                bairro.NomeRegiao = ds.Tables[0].Rows[i]["NomeRegiao"].ToString();

                            ret.Add(bairro);
                        }
                    }


                }
            }

            return ret;
        }

        public static BairroModel RecuperarPeloId(int id)
        {
            BairroModel ret = null;
            using (var conexao = new SqlConnection())
            {
                conexao.ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString;
                conexao.Open();
                using (var comando = new SqlCommand())
                {
                    comando.Connection = conexao;
                    comando.CommandText = "select b.id,b.nome,b.ativo,b.id_cidade, b.regiao, c.id_estado, e.id_pais from Bairro b " +
                        "inner join cidade c on b.id_cidade = c.id " +
                        "inner join estado e on c.id_estado = e.id " +
                        "where (b.id = @id)";
                    comando.Parameters.Add("@id", SqlDbType.Int).Value = id;

                    var reader = comando.ExecuteReader();
                    if (reader.Read())
                    {
                        ret = new BairroModel
                        {
                            Id = (int)reader["id"],
                            Nome = (string)reader["nome"],
                            IdCidade = (int)reader["id_cidade"],
                            IdPais = (int)reader["id_pais"],
                            IdEstado = (int)reader["id_Estado"],
                            NomeRegiao = (string)reader["regiao"],
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
                        comando.CommandText = "delete from Bairro where (id = @id)";

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
                        comando.CommandText = "insert into Bairro (Nome,id_cidade,regiao,ativo) values (@nome,@id_cidade,@regiao,@ativo); select convert(int, scope_identity())";

                        comando.Parameters.Add("@nome", SqlDbType.VarChar).Value = this.Nome;
                        comando.Parameters.Add("@id_cidade", SqlDbType.Int).Value = this.IdCidade;
                        comando.Parameters.Add("@regiao", SqlDbType.VarChar).Value = this.NomeRegiao;
                        comando.Parameters.Add("@ativo", SqlDbType.VarChar).Value = (this.Ativo ? 1 : 0);

                        ret = (int)comando.ExecuteScalar();

                    }
                    else
                    {
                        comando.CommandText =
                            "update Bairro set nome=@nome, id_cidade = @id_cidade, regiao = @regiao, ativo = @ativo" +
                            " where id = @id";

                        comando.Parameters.Add("@id", SqlDbType.VarChar).Value = this.Id;
                        comando.Parameters.Add("@nome", SqlDbType.VarChar).Value = this.Nome;
                        comando.Parameters.Add("@id_cidade", SqlDbType.VarChar).Value = this.IdCidade;
                        comando.Parameters.Add("@regiao", SqlDbType.VarChar).Value = this.NomeRegiao;
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