﻿using ControleImoveis.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace ControleImoveis.Web.Controllers.Cadastro
{
    
    public class CadPerfilController : Controller
    {
        // GET: CadPerfil
        private const int _quantMaxLinhasPorPagina = 5;

        #region Perfil

        public ActionResult Index()
        {
            ViewBag.ListaTamPag = new SelectList(new int[] { _quantMaxLinhasPorPagina, 10, 15, 20 }, _quantMaxLinhasPorPagina);
            ViewBag.QuantMaxLinhasPorPagina = _quantMaxLinhasPorPagina;
            ViewBag.PaginaAtual = 1;

            var lista = PerfilModel.RecuperarLista(ViewBag.PaginaAtual, _quantMaxLinhasPorPagina);
            var quant = PerfilModel.RecuperarQuantidade();

            var difQuantPaginas = (quant % ViewBag.QuantMaxLinhasPorPagina) > 0 ? 1 : 0;
            ViewBag.QuantPaginas = (quant / ViewBag.QuantMaxLinhasPorPagina) + difQuantPaginas;

            return View(lista);
        }


        [ValidateAntiForgeryToken]
        public JsonResult PerfilPagina(int pagina, int tamPag)
        {
            var lista = PerfilModel.RecuperarLista(pagina, tamPag);

            return Json(lista);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RecuperarPerfil(int id)
        {
            return Json(PerfilModel.RecuperarPeloId(id));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ExcluirPerfil(int id)
        {
            return Json(PerfilModel.ExcluirPeloId(id));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SalvarPerfil(PerfilModel model)
        {
            var resultado = "OK";
            var mensagens = new List<string>();
            var idSalvo = string.Empty;

            if (!ModelState.IsValid)
            {
                resultado = "AVISO";
                mensagens = ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage).ToList();
            }
            else
            {
                try
                {
                    var id = model.Salvar();
                    if (id > 0)
                    {
                        idSalvo = id.ToString();
                    }
                    else
                    {
                        resultado = "ERRO";
                    }
                }
                catch (Exception ex)
                {
                    resultado = "ERRO";
                    mensagens.Add(ex.Message);

                }
            }
            return Json(new { Resultado = resultado, Mensagens = mensagens, IdSalvo = idSalvo });
        }
        #endregion
    }
}