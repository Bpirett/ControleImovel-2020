using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using ControleImoveis.Web.Models;

namespace ControleImoveis.Web.Controllers
{
    public class ContaController : Controller
    {
        // GET: Conta
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.returnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(LoginViewModel login, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(login);
            }

            var usuario = UsuarioModel.ValidarUsuario(login.Usuario, login.Senha);

            if (usuario != null)
            {
                if (usuario.Ativo == true)
                {
                    //FormsAuthentication.SetAuthCookie(usuario.Nome, login.LembrarMe);
                    var Ticket = FormsAuthentication.Encrypt(new FormsAuthenticationTicket(1, usuario.Nome, DateTime.Now, DateTime.Now.AddHours(12), login.LembrarMe, usuario.Id + "|" + PerfilModel.RecuperarPeloId(usuario.IdPerfil).Nome));
                    var coockie = new HttpCookie(FormsAuthentication.FormsCookieName, Ticket);
                    Response.Cookies.Add(coockie);

                    if (Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Usuario inativo.");
                }
            }
            else
            {
                ModelState.AddModelError("", "Login inválido.");
            }

            return View(login);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult LogOff(string returnUrl)
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        public ActionResult AlterarSenhaUsuario(AlteracaoSenhaUsuarioViewModel model)
        {
            if (HttpContext.Request.HttpMethod.ToUpper() == "POST")
            {
                ViewBag.Mensagem = null;
                var usuarioLogado = (HttpContext.User as AplicacaoPrincipal);

                var alterou = false;

                if (usuarioLogado != null)
                {
                    if (!usuarioLogado.Dados.ValidarSenhaAtual(model.SenhaAtual))
                    {
                        ModelState.AddModelError("SenhaAtual", "A senha atual nao confere");
                    }
                    else
                    {
                        alterou = usuarioLogado.Dados.AlterarSenha(model.NovaSenha);

                        if (alterou)
                        {
                            ViewBag.Mensagem = new string[] { "ok", "Senha alterada com sucesso." };
                        }
                        else
                        {
                            ViewBag.Mensagem = new string[] { "erro", "Nao foi possivel alterar a senha." };
                        }
                    }
                }

                return View();
            }
            else
            {
                ModelState.Clear();
                return View();

            }

        }

        [AllowAnonymous]
        public ActionResult EsqueciMinhaSenha(EsqueciMinhaSenhaViewModel model)
        {
            ViewBag.EmailEnviado = true;

            if (HttpContext.Request.HttpMethod.ToUpper() == "GET")
            {
                ViewBag.Emailenviado = false;
                ModelState.Clear();
            }
            else
            {
                var usuario = UsuarioModel.RecuperarPeloLogin(model.Login);
                if (usuario != null)
                {
                    EnviarEmailRedefinicaoSenha(usuario);
                }
            }
            return View(model);
        }


        private void EnviarEmailRedefinicaoSenha(UsuarioModel usuario)
        {
            var callbackUrl = Url.Action("RedefinirSenha", "Conta", new { id = usuario.Id }, protocol: Request.Url.Scheme);
            var client = new SmtpClient()
            {
                Host = ConfigurationManager.AppSettings["EmailServidor"],
                Port = Convert.ToInt32(ConfigurationManager.AppSettings["EmailPorta"]),
                EnableSsl = (ConfigurationManager.AppSettings["EmailSsl"] == "S"),
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(
                    ConfigurationManager.AppSettings["EmailUsuario"],
                    ConfigurationManager.AppSettings["EmailSenha"])
            };

            var mensagem = new MailMessage();
            mensagem.From = new MailAddress(ConfigurationManager.AppSettings["EmailOrigem"], "Controle de Imoveis - Como Programar Melhor");
            mensagem.To.Add(usuario.Email);
            mensagem.Subject = "Redefinição de senha";
            mensagem.Body = string.Format("Redefina a sua senha <a href='{0}'>aqui</a>", callbackUrl);
            mensagem.IsBodyHtml = true;

            client.Send(mensagem);
        }

        [AllowAnonymous]
        public ActionResult RedefinirSenha(int id)
        {
            var usuario = UsuarioModel.RecuperarPeloId(id);
            if (usuario == null)
            {
                id = -1;
            }

            var model = new NovaSenhaViewModel() { Usuario = id };

            ViewBag.Mensagem = null;

            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult RedefinirSenha(NovaSenhaViewModel model)
        {
            ViewBag.Mensagem = null;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var usuario = UsuarioModel.RecuperarPeloId(model.Usuario);
            if (usuario != null)
            {
                var ok = usuario.AlterarSenha(model.Senha);
                ViewBag.Mensagem = ok ? "Senha alterada com sucesso!" : "Não foi possível alterar a senha!";
            }

            return View();
        }

    }
}