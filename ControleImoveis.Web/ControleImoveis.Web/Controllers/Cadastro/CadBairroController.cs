using ControleImoveis.Web.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ControleImoveis.Web.Controllers.Cadastro
{
    public class CadBairroController : Controller
    {

        private const int _quantMaxLinhasPorPagina = 5;

       

        public ActionResult Index()
        {
            ViewBag.ListaTamPag = new SelectList(new int[] { _quantMaxLinhasPorPagina, 10, 15, 20 }, _quantMaxLinhasPorPagina);
            ViewBag.QuantMaxLinhasPorPagina = _quantMaxLinhasPorPagina;
            ViewBag.PaginaAtual = 1;

            var lista = BairroModel.RecuperarLista(ViewBag.PaginaAtual, _quantMaxLinhasPorPagina);
            var quant = BairroModel.RecuperarQuantidade();
            ViewBag.QuantidadeRegistros = quant;

            var difQuantPaginas = (quant % ViewBag.QuantMaxLinhasPorPagina) > 0 ? 1 : 0;
            ViewBag.QuantPaginas = (quant / ViewBag.QuantMaxLinhasPorPagina) + difQuantPaginas;
            ViewBag.Paises = PaisModel.RecuperarLista();
            ViewBag.Estados = EstadoModel.RecuperarLista();
            ViewBag.Cidade = CidadeModel.RecuperarLista();

            var enumData = Enum.GetNames(typeof(BairroModel.Regiao)).ToList();

            ViewBag.regiao = enumData;

            return View(lista);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult BairroPagina(int pagina, int tamPag, string filtro, string ordem)
        {
            var lista = BairroModel.RecuperarLista(pagina, tamPag, filtro, ordem);

            return Json(lista);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult RecuperarBairro(int id)
        {
            var vm = BairroModel.RecuperarPeloId(id);

            return Json(vm);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult ExcluirBairro(int id)
        {
            return Json(BairroModel.ExcluirPeloId(id));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult SalvarBairro(BairroModel model)
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


        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult RecuperarBairrosDaCidade(string bairro, int IdCidade)
        {
            var lista = BairroModel.RecuperarLista(IdCidade: IdCidade, regiao: bairro);
            if(lista != null)
            lista.Insert(0, new BairroModel { Id = -1, Nome = "-- Não Selecionado --" });

            return Json(lista);
        }
    }
}