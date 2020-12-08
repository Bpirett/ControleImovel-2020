using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web;

namespace ControleImoveis.Web.Models
{
    public class AnuncioModel
    {

        public enum Negocio
        {
            Venda,
            Aluga
        }

        #region Propriedades
        public int Id { get; set; }

        [Required(ErrorMessage = "Informe a Região")]
        public string Regiao { get; set; }

        [Required(ErrorMessage = "Informe o Negocio")]
        public string TipoNegocio { get; set; }

        [Required(ErrorMessage = "Referência obrigatoria")]
        public string Referencia { get; set; }

        [Required(ErrorMessage = "Informe o tipo do imovel")]
        public string TipoImovel { get; set; }

        [Required(ErrorMessage = "Informe a Preço")]
        public string Preco { get; set; }

        [Required(ErrorMessage = "Informe a quantidade de dormitorios")]
        public int Dormitorio { get; set; }

        [Required(ErrorMessage = "Informe a quantidade de vagas")]
        public int Vagas { get; set; }

        public int Area { get; set; }

        [Required(ErrorMessage = "Informe a Cidade")]
        public string Cidade { get; set; }

        [Required(ErrorMessage = "Informe a Bairro")]
        public string Bairro { get; set; }

        [Required(ErrorMessage = "Informe a Descrição")]
        public string Descricao { get; set; }

        public bool Ativo { get; set; }
        public string Nomeusu { get; set; }
        public bool Permuta { get; set; }

        public string[] Imagem { get; set; }


        #endregion



        #region Métodos

        public static int RecuperarQuantidade(string nomeusu = "")
        {
            var ret = 0;

            using (var conexao = new SqlConnection())
            {
                conexao.ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString;
                conexao.Open();
                using (var comando = new SqlCommand())
                {

                    var filtroWhere = "";
                    if (!string.IsNullOrEmpty(nomeusu))
                    {
                        filtroWhere += string.Format(" where (NomeUsuario = @nomeUsuario) ");

                        comando.Parameters.Add("@nomeUsuario", SqlDbType.VarChar).Value = nomeusu;
                    }

                    comando.Connection = conexao;
                    comando.CommandText = "select count(*) from Anuncio" +
                      filtroWhere;

                    ret = (int)comando.ExecuteScalar();
                }
            }

            return ret;
        }

        public static List<AnuncioModel> RecuperarLista(int pagina = 0, int tamPagina = 0, string filtro = "", string ordem = "", bool somenteAtivos = false, string nomeusu = "")
        {
            var ret = new List<AnuncioModel>();

            using (var conexao = new SqlConnection())
            {
                conexao.ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString;
                conexao.Open();

                using (var comando = new SqlCommand())
                {
                    var filtroWhere = "";
                    if (!string.IsNullOrEmpty(filtro))
                    {
                        filtroWhere = string.Format("where (lower(nome) like '%{0}%') ", filtro.ToLower());
                    }

                    if (somenteAtivos)
                    {
                        filtroWhere = (string.IsNullOrEmpty(filtroWhere) ? "where" : "and ") + "(ativo = 1) ";
                    }

                    if (!string.IsNullOrEmpty(nomeusu))
                    {
                        filtroWhere += string.Format(" (NomeUsuario = @nomeUsuario) ");

                        comando.Parameters.Add("@nomeUsuario", SqlDbType.VarChar).Value = nomeusu;
                    }


                    var pos = (pagina - 1) * tamPagina;
                    var paginacao = "";
                    if (pagina > 0 && tamPagina > 0)
                    {
                        paginacao = string.Format(" offset {0} rows fetch next {1} rows only",
                            pos, tamPagina);
                    }


                    var sql =
                    "SELECT [IdImovel],[Regiao],[Referencia],[Negocio],[Tipoimovel],[PrecoImovel],[Dormitorio],[Vaga],[Area],[Cidade],[Bairro],[Ativo],[NomeUsuario],[Permuta], [descricao] FROM[ControleImoveis].[dbo].[Anuncio]" +
                    "where " +
                    filtroWhere +
                    "order by " + (!string.IsNullOrEmpty(ordem) ? ordem : "Referencia") +
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

                            AnuncioModel Anuncio = new AnuncioModel();

                            if (ds.Tables[0].Rows[i]["IdImovel"] != DBNull.Value)
                                Anuncio.Id = (int)ds.Tables[0].Rows[i]["IdImovel"];

                            if (ds.Tables[0].Rows[i]["Regiao"] != DBNull.Value)
                                Anuncio.Regiao = (string)ds.Tables[0].Rows[i]["Regiao"];

                            if (ds.Tables[0].Rows[i]["Referencia"] != DBNull.Value)
                                Anuncio.Referencia = (string)ds.Tables[0].Rows[i]["Referencia"];

                            if (ds.Tables[0].Rows[i]["Negocio"] != DBNull.Value)
                                Anuncio.TipoNegocio = (string)ds.Tables[0].Rows[i]["Negocio"];

                            if (ds.Tables[0].Rows[i]["Tipoimovel"] != DBNull.Value)
                                Anuncio.TipoImovel = (string)ds.Tables[0].Rows[i]["Tipoimovel"];

                            if (ds.Tables[0].Rows[i]["PrecoImovel"] != DBNull.Value)
                                Anuncio.Preco = string.Format(CultureInfo.GetCultureInfo("pt-BR"), "{0:C}", ds.Tables[0].Rows[i]["PrecoImovel"]);

                            if (ds.Tables[0].Rows[i]["Dormitorio"] != DBNull.Value)
                                Anuncio.Dormitorio = (int)ds.Tables[0].Rows[i]["Dormitorio"];

                            if (ds.Tables[0].Rows[i]["Vaga"] != DBNull.Value)
                                Anuncio.Vagas = (int)ds.Tables[0].Rows[i]["Vaga"];

                            if (ds.Tables[0].Rows[i]["Area"] != DBNull.Value)
                                Anuncio.Area = Convert.ToInt32(ds.Tables[0].Rows[i]["Area"]);

                            if (ds.Tables[0].Rows[i]["Cidade"] != DBNull.Value)
                                Anuncio.Cidade = (string)ds.Tables[0].Rows[i]["Cidade"];

                            if (ds.Tables[0].Rows[i]["Bairro"] != DBNull.Value)
                                Anuncio.Bairro = (string)ds.Tables[0].Rows[i]["Bairro"];

                            if (ds.Tables[0].Rows[i]["Ativo"] != DBNull.Value)
                                Anuncio.Ativo = (bool)ds.Tables[0].Rows[i]["Ativo"];

                            if (ds.Tables[0].Rows[i]["NomeUsuario"] != DBNull.Value)
                                Anuncio.Nomeusu = (string)ds.Tables[0].Rows[i]["NomeUsuario"];

                            if (ds.Tables[0].Rows[i]["Permuta"] != DBNull.Value)
                                Anuncio.Permuta = (bool)ds.Tables[0].Rows[i]["Permuta"];

                            ret.Add(Anuncio);
                        }
                    }


                }
            }

            return ret;
        }

        public static AnuncioModel RecuperarPeloId(int id)
        {
            AnuncioModel ret = null;
            using (var conexao = new SqlConnection())
            {
                conexao.ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString;
                conexao.Open();
                using (var comando = new SqlCommand())
                {
                    comando.Connection = conexao;
                    comando.CommandText = "select [IdImovel],[Regiao],[Referencia],[Negocio],[Tipoimovel],[PrecoImovel],[Dormitorio],[Vaga],[Area],[Cidade],[Bairro],[Ativo],[NomeUsuario],[Permuta],[descricao]  from Anuncio " +
                                           "where (IdImovel = @IdImovel)";
                    comando.Parameters.Add("@IdImovel", SqlDbType.Int).Value = id;

                    var reader = comando.ExecuteReader();
                    if (reader.Read())
                    {
                        ret = new AnuncioModel
                        {

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
                        comando.CommandText = "delete from Anuncio where (IdImovel = @IdImovel)";

                        comando.Parameters.Add("IdImovel", SqlDbType.Int).Value = id;
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
                        comando.CommandText = "insert into Anuncio ([Regiao],[Referencia],[Negocio],[Tipoimovel],[PrecoImovel],[Dormitorio],[Vaga],[Area],[Cidade],[Bairro],[NomeUsuario],[Permuta],[descricao],[Ativo],[datacadastro])" +
                            "values (@Regiao,@Referencia,@Negocio,@Tipoimovel,@PrecoImovel,@Dormitorio,@Vaga,@Area,@Cidade,@Bairro,@NomeUsuario,@Permuta,@Descricao,@Ativo,@datacadastro); select convert(int, scope_identity())";

                        comando.Parameters.Add("@Regiao", SqlDbType.VarChar).Value = this.Regiao;
                        comando.Parameters.Add("@Referencia", SqlDbType.VarChar).Value = this.Referencia;
                        comando.Parameters.Add("@Negocio", SqlDbType.VarChar).Value = this.TipoNegocio;
                        comando.Parameters.Add("@Tipoimovel", SqlDbType.VarChar).Value = this.TipoImovel;
                        comando.Parameters.Add("@PrecoImovel", SqlDbType.Decimal).Value = this.Preco;
                        comando.Parameters.Add("@Dormitorio", SqlDbType.Int).Value = this.Dormitorio;
                        comando.Parameters.Add("@Vaga", SqlDbType.Int).Value = this.Vagas;
                        comando.Parameters.Add("@Area", SqlDbType.Int).Value = this.Area;
                        comando.Parameters.Add("@Cidade", SqlDbType.VarChar).Value = this.Cidade;
                        comando.Parameters.Add("@Bairro", SqlDbType.VarChar).Value = this.Bairro;
                        comando.Parameters.Add("@NomeUsuario", SqlDbType.VarChar).Value = this.Nomeusu;
                        comando.Parameters.Add("@Permuta", SqlDbType.Bit).Value = (this.Permuta ? 1 : 0);
                        comando.Parameters.Add("@Descricao", SqlDbType.VarChar).Value = this.Descricao;
                        comando.Parameters.Add("@Ativo", SqlDbType.Bit).Value = (this.Ativo ? 1 : 0);
                        comando.Parameters.Add("@datacadastro", SqlDbType.DateTime).Value = DateTime.Now;
                        ret = (int)comando.ExecuteScalar();

                    }
                    else
                    {
                        comando.CommandText =
                            "update Anuncio set  Regiao=@Regiao,Referencia=@Referencia,Negocio=@Negocio,Tipoimovel=@Tipoimovel,PrecoImovel=@PrecoImovel,Dormitorio=@Dormitorio,Vaga=@Vaga,Area=@Area,Cidade=@Cidade," +
                            "Bairro=@Bairro,NomeUsuario=@NomeUsuario,Permuta=@Permuta,Descricao=@Descricao, Ativo=@Ativo" +
                            " where IdImovel = @IdImovel";


                        comando.Parameters.Add("@IdImovel", SqlDbType.VarChar).Value = this.Id;
                        comando.Parameters.Add("@Regiao", SqlDbType.VarChar).Value = this.Regiao;
                        comando.Parameters.Add("@Referencia", SqlDbType.VarChar).Value = this.Referencia;
                        comando.Parameters.Add("@Negocio", SqlDbType.VarChar).Value = this.TipoNegocio;
                        comando.Parameters.Add("@Tipoimovel", SqlDbType.VarChar).Value = this.TipoImovel;
                        comando.Parameters.Add("@PrecoImovel", SqlDbType.Decimal).Value = this.Preco;
                        comando.Parameters.Add("@Dormitorio", SqlDbType.Int).Value = this.Dormitorio;
                        comando.Parameters.Add("@Vaga", SqlDbType.Int).Value = this.Vagas;
                        comando.Parameters.Add("@Area", SqlDbType.Int).Value = this.Area;
                        comando.Parameters.Add("@Cidade", SqlDbType.VarChar).Value = this.Cidade;
                        comando.Parameters.Add("@Bairro", SqlDbType.VarChar).Value = this.Bairro;
                        comando.Parameters.Add("@NomeUsuario", SqlDbType.VarChar).Value = this.Nomeusu;
                        comando.Parameters.Add("@Permuta", SqlDbType.VarChar).Value = (this.Permuta ? 1 : 0);
                        comando.Parameters.Add("@Descricao", SqlDbType.VarChar).Value = this.Descricao;
                        comando.Parameters.Add("@Ativo", SqlDbType.Bit).Value = (this.Ativo ? 1 : 0);


                        if (comando.ExecuteNonQuery() > 0)
                        {
                            ret = this.Id;
                        }
                    }
                }
            }

            return ret;
        }


        public static AnuncioModel RecuperarId(string usu)
        {
            var ret = new AnuncioModel();

            using (var conexao = new SqlConnection())
            {
                conexao.ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString;
                conexao.Open();
                using (var comando = new SqlCommand())
                {
                    comando.Connection = conexao;
                    comando.CommandText = "select top(1) [IdImovel] from Anuncio " +
                                           "where NomeUsuario = @Usu order by datacadastro desc";
                    comando.Parameters.Add("@Usu", SqlDbType.VarChar).Value = usu;

                    SqlDataAdapter adapter = new SqlDataAdapter(comando);
                    DataSet ds = new DataSet();
                    adapter.Fill(ds);

                    if (ds != null || ds.Tables.Count > 0)
                    {
                        DataTable dt = ds.Tables[0];

                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            AnuncioModel anuncio = new AnuncioModel();

                            if (ds.Tables[0].Rows[i]["IdImovel"] != DBNull.Value)
                                anuncio.Id = (int)ds.Tables[0].Rows[i]["IdImovel"];
                            
                            ret = anuncio;
                        }
                        
                    }
                }
            }
            return ret;

        }
        #endregion
    }
}