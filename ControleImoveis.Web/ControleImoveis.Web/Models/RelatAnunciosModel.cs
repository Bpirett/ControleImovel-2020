using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web;

namespace ControleImoveis.Web.Models
{
    public class RelatAnunciosModel
    {
        public string Referencia { get; set; }
        public string Preco { get; set; }
        public string datacadastro { get; set; }
        public bool Ativo { get; set; }
        public string Cidade { get; set; }
        public string Bairro { get; set; }
        public string Regiao { get; set; }
        public string TipoNegocio { get; set; }
        public string TipoImovel { get; set; }



        public static List<RelatAnunciosModel> relatAnuncios(string nomeusu = "")
        {
            var ret = new List<RelatAnunciosModel>();

            using (var conexao = new SqlConnection())
            {
                conexao.ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString;
                conexao.Open();

                using (var comando = new SqlCommand())
                {

                    var sql =
                    "SELECT [Referencia],[PrecoImovel],[Ativo],[NomeUsuario],[datacadastro],[Negocio],[Tipoimovel],[Cidade],[Bairro],[Regiao] FROM[ControleImoveis].[dbo].[Anuncio]" +
                    "where NomeUsuario = @NomeUsuario";

                    comando.Parameters.Add("@NomeUsuario", SqlDbType.VarChar).Value = nomeusu;

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

                            RelatAnunciosModel Anuncio = new RelatAnunciosModel();

                            if (ds.Tables[0].Rows[i]["Referencia"] != DBNull.Value)
                                Anuncio.Referencia = (string)ds.Tables[0].Rows[i]["Referencia"];

                            if (ds.Tables[0].Rows[i]["PrecoImovel"] != DBNull.Value)
                                Anuncio.Preco = string.Format(CultureInfo.GetCultureInfo("pt-BR"), "{0:C}", ds.Tables[0].Rows[i]["PrecoImovel"]);

                            if (ds.Tables[0].Rows[i]["Ativo"] != DBNull.Value)
                                Anuncio.Ativo = (bool)ds.Tables[0].Rows[i]["Ativo"];

                            if (ds.Tables[0].Rows[i]["datacadastro"] != DBNull.Value)
                                Anuncio.datacadastro = ds.Tables[0].Rows[i]["datacadastro"].ToString();

                            if (ds.Tables[0].Rows[i]["Cidade"] != DBNull.Value)
                                Anuncio.Cidade = (string)ds.Tables[0].Rows[i]["Cidade"];

                            if (ds.Tables[0].Rows[i]["Bairro"] != DBNull.Value)
                                Anuncio.Bairro = (string)ds.Tables[0].Rows[i]["Bairro"];

                            if (ds.Tables[0].Rows[i]["Negocio"] != DBNull.Value)
                                Anuncio.TipoNegocio = (string)ds.Tables[0].Rows[i]["Negocio"];

                            if (ds.Tables[0].Rows[i]["Tipoimovel"] != DBNull.Value)
                                Anuncio.TipoImovel = (string)ds.Tables[0].Rows[i]["Tipoimovel"];

                            if (ds.Tables[0].Rows[i]["Regiao"] != DBNull.Value)
                                Anuncio.Regiao = (string)ds.Tables[0].Rows[i]["Regiao"];

                            ret.Add(Anuncio);
                        }
                    }


                }
            }

            return ret;
        }


    }
}