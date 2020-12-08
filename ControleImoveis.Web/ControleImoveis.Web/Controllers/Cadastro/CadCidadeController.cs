﻿using ControleImoveis.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ControleImoveis.Web.Controllers.Cadastro
{
    public class CadCidadeController : Controller
    {

        private const int _quantMaxLinhasPorPagina = 5;

        public ActionResult Index()
        {
            ViewBag.ListaTamPag = new SelectList(new int[] { _quantMaxLinhasPorPagina, 10, 15, 20 }, _quantMaxLinhasPorPagina);
            ViewBag.QuantMaxLinhasPorPagina = _quantMaxLinhasPorPagina;
            ViewBag.PaginaAtual = 1;

            var lista = CidadeModel.RecuperarLista(ViewBag.PaginaAtual, _quantMaxLinhasPorPagina);
            var quant = CidadeModel.RecuperarQuantidade();
            ViewBag.QuantidadeRegistros = quant;

            var difQuantPaginas = (quant % ViewBag.QuantMaxLinhasPorPagina) > 0 ? 1 : 0;
            ViewBag.QuantPaginas = (quant / ViewBag.QuantMaxLinhasPorPagina) + difQuantPaginas;
            ViewBag.Paises = PaisModel.RecuperarLista();
            ViewBag.Estados = EstadoModel.RecuperarLista();

            return View(lista);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult CidadePagina(int pagina, int tamPag, string filtro, string ordem)
        {
            var lista = CidadeModel.RecuperarLista(pagina, tamPag, filtro, ordem);

            return Json(lista);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult RecuperarCidade(int id)
        {
            var vm = CidadeModel.RecuperarPeloId(id);

            return Json(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult RecuperarCidadesDoEstado(int idEstado)
        {
            var lista = CidadeModel.RecuperarLista(idEstado: idEstado);
            lista.Insert(0, new CidadeModel { Id = -1, Nome = "-- Não Selecionado --" });
            return Json(lista);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult ExcluirCidade(int id)
        {
            return Json(CidadeModel.ExcluirPeloId(id));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult SalvarCidade(CidadeModel model)
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
                    var vm = model;
                    var id = vm.Salvar();
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
                }
            }

            return Json(new { Resultado = resultado, Mensagens = mensagens, IdSalvo = idSalvo });
        }
    }
}