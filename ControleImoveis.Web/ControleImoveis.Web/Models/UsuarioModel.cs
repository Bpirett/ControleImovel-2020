using ControleImoveis.Web.Helpers;
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
    public class UsuarioModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Informe o Login")]
        public string Login { get; set; }

        [Required(ErrorMessage = "Informe o Senha")]
        public string Senha { get; set; }

        [Required(ErrorMessage = "Informe o Nome")]
        public string Nome { get; set; }
        public bool Ativo { get; set; }

        [Required(ErrorMessage = "Informe o perfil")]
        public int IdPerfil { get; set; }

        [Required(ErrorMessage = "Informe o e-mail")]
        public string Email { get; set; }

        public static UsuarioModel ValidarUsuario(string login, string senha)
        {
            UsuarioModel ret = null;
            using (var conexao = new SqlConnection())
            {
                conexao.ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString;
                conexao.Open();
                using (var comando = new SqlCommand())
                {
                    comando.Connection = conexao;
                    comando.CommandText = string.Format("Select * from Usuario where Login=@login and Senha=@senha");

                    comando.Parameters.Add("login", SqlDbType.VarChar).Value = login;
                    comando.Parameters.Add("senha", SqlDbType.VarChar).Value = CriptoHelper.HashMD5(senha);

                    var reader = comando.ExecuteReader();
                    if (reader.Read())
                    {
                        ret = new UsuarioModel
                        {
                            Id = (int)reader["Id"],
                            Login = (string)reader["Login"],
                            Senha = (string)reader["Senha"],
                            Nome = (string)reader["Nome"],
                            Ativo = (bool)reader["Ativo"],
                            IdPerfil = (int)reader["id_perfil"],
                            Email = (string)reader["Email"]
                        };
                    }
                }
            }
            return ret;
        }

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
                    comando.CommandText = "select count(*) from Usuario";
                    ret = (int)comando.ExecuteScalar();
                }
            }

            return ret;
        }

        public static List<UsuarioModel> RecuperarLista(int pagina, int tamPagina)
        {
            var ret = new List<UsuarioModel>();

            using (var conexao = new SqlConnection())
            {
                conexao.ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString;
                conexao.Open();
                using (var comando = new SqlCommand())
                {
                    var pos = (pagina - 1) * tamPagina;
                    comando.Connection = conexao;
                    comando.CommandText = string.Format("select * from Usuario order by nome offset {0} rows fetch next {1} rows only",
                        pos > 0 ? pos - 1 : 0, tamPagina);
                    var reader = comando.ExecuteReader();
                    while (reader.Read())
                    {
                        ret.Add(new UsuarioModel
                        {
                            Id = (int)reader["Id"],
                            Nome = (string)reader["Nome"],
                            Login = (string)reader["Login"],
                            Ativo = (bool)reader["ativo"],
                            IdPerfil = (int)reader["id_perfil"],
                            Email = (string)reader["Email"]
                        });
                    }
                }
            }

            return ret;
        }

        public static UsuarioModel RecuperarPeloId(int id)
        {
            UsuarioModel ret = null;

            using (var conexao = new SqlConnection())
            {
                conexao.ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString;
                conexao.Open();
                using (var comando = new SqlCommand())
                {
                    comando.Connection = conexao;
                    comando.CommandText = "select * from usuario where (id = @id)";
                    comando.Parameters.Add("id", SqlDbType.Int).Value = id;

                    var reader = comando.ExecuteReader();
                    if (reader.Read())
                    {
                        ret = new UsuarioModel
                        {
                            Id = (int)reader["id"],
                            Nome = (string)reader["nome"],
                            Login = (string)reader["login"],
                            Ativo = (bool)reader["ativo"],
                            IdPerfil = (int)reader["id_perfil"],
                            Email = (string)reader["email"]

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
                        comando.CommandText = "delete from usuario where (id = @id)";

                        comando.Parameters.Add("id", SqlDbType.Int).Value = id;
                        ret = (comando.ExecuteNonQuery() > 0);
                    }
                }
            }

            return ret;
        }

        public int Salvar(string login)
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
                        var existe = Existelogin(login);

                        if (existe)
                            throw new Exception("Esse usuario já existe");

                        comando.CommandText = "insert into Usuario (Nome, Senha, Login, Ativo, id_perfil,email) values (@nome, @senha, @login,@ativo,@id_perfil, @email); select convert(int, scope_identity())";

                        comando.Parameters.Add("@nome", SqlDbType.VarChar).Value = this.Nome;
                        comando.Parameters.Add("@senha", SqlDbType.VarChar).Value = CriptoHelper.HashMD5(this.Senha);
                        comando.Parameters.Add("@login", SqlDbType.VarChar).Value = this.Login;
                        comando.Parameters.Add("@ativo", SqlDbType.VarChar).Value = (this.Ativo ? 1 : 0);
                        comando.Parameters.Add("@id_perfil", SqlDbType.Int).Value = this.IdPerfil;
                        comando.Parameters.Add("@email", SqlDbType.VarChar).Value = this.Email;



                        ret = (int)comando.ExecuteScalar();

                    }
                    else
                    {
                        comando.CommandText =
                            "update Usuario set nome=@nome, login=@login,ativo = @ativo, id_perfil=@id_perfil, email=@email" +
                           (!string.IsNullOrEmpty(this.Senha) ? ", senha=@senha" : "") +
                            " where id = @id";

                        comando.Parameters.Add("@nome", SqlDbType.VarChar).Value = this.Nome;
                        comando.Parameters.Add("@login", SqlDbType.VarChar).Value = this.Login;
                        comando.Parameters.Add("@ativo", SqlDbType.VarChar).Value = (this.Ativo ? 1 : 0);
                        comando.Parameters.Add("@id_perfil", SqlDbType.Int).Value = this.IdPerfil;
                        comando.Parameters.Add("@email", SqlDbType.VarChar).Value = this.Email;

                        if (!string.IsNullOrEmpty(this.Senha))
                        {
                            comando.Parameters.Add("@senha", SqlDbType.VarChar).Value = CriptoHelper.HashMD5(this.Senha);
                        }

                        comando.Parameters.Add("@id", SqlDbType.Int).Value = this.Id;

                        if (comando.ExecuteNonQuery() > 0)
                        {
                            ret = this.Id;
                        }
                    }
                }
            }

            return ret;
        }
        public bool Existelogin(string pLogin)
        {
            bool ret = false;

            using (var conexao = new SqlConnection())
            {
                conexao.ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString;
                conexao.Open();
                using (var comando = new SqlCommand())
                {
                    comando.Connection = conexao;
                    comando.CommandText = "SELECT * FROM Usuario WHERE Login = @login";

                    comando.Parameters.Add("login", SqlDbType.VarChar).Value = pLogin;

                    var reader = comando.ExecuteReader();
                    UsuarioModel model = new UsuarioModel();
                    if (reader.Read())
                    {
                        model = new UsuarioModel
                        {
                            Id = (int)reader["id"],
                            Nome = (string)reader["nome"],
                            Login = (string)reader["login"],
                            Ativo = (bool)reader["ativo"],
                            IdPerfil = (int)reader["id_perfil"],
                            Email = (string)reader["Email"]
                        };
                    }

                    if (!string.IsNullOrEmpty(model.Login))
                    {
                        ret = true;
                    }

                }
            }

            return ret;
        }

        public bool ValidarSenhaAtual(string senhaAtual)
        {
            var ret = false;
            using (var conexao = new SqlConnection())
            {
                conexao.ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString;
                conexao.Open();
                using (var comando = new SqlCommand())
                {

                    comando.Connection = conexao;
                    comando.CommandText = "select count(*) from usuario where Senha = @senhaAtual and Id =@id";

                    comando.Parameters.Add("@id", SqlDbType.Int).Value = this.Id;
                    comando.Parameters.Add("@senhaAtual", SqlDbType.VarChar).Value = CriptoHelper.HashMD5(senhaAtual);

                    ret = ((int) comando.ExecuteScalar() > 0);

                }
            }
            return ret;
        }

        public bool AlterarSenha(string novaSenha)
        {
            var ret = false;

            using (var conexao = new SqlConnection())
            {
                conexao.ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString;
                conexao.Open();
                using (var comando = new SqlCommand())
                {
                    comando.Connection = conexao;
                    comando.CommandText = "update Usuario SET senha = @senha WHERE id = @id";

                    comando.Parameters.Add("@id", SqlDbType.Int).Value = this.Id;
                    comando.Parameters.Add("@senha", SqlDbType.VarChar).Value = CriptoHelper.HashMD5(novaSenha);

                    ret = (comando.ExecuteNonQuery() > 0);

                }
            }

            return ret;

        }

        public static UsuarioModel RecuperarPeloLogin(string plogin)
        {
            UsuarioModel ret = null;
            using (var conexao = new SqlConnection())
            {
                conexao.ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString;
                conexao.Open();
                using (var comando = new SqlCommand())
                {
                    comando.Connection = conexao;
                    comando.CommandText = "SELECT * FROM Usuario WHERE Login = @login";

                    comando.Parameters.Add("login", SqlDbType.VarChar).Value = plogin;

                    var reader = comando.ExecuteReader();
                    UsuarioModel model = new UsuarioModel();
                    if (reader.Read())
                    {
                        ret = new UsuarioModel
                        {
                            Id = (int)reader["id"],
                            Nome = (string)reader["nome"],
                            Login = (string)reader["login"],
                            Ativo = (bool)reader["ativo"],
                            IdPerfil = (int)reader["id_perfil"],
                            Email = (string)reader["Email"]
                        };
                    }
                                      

                }
            }
            return ret;
        }

    }

}