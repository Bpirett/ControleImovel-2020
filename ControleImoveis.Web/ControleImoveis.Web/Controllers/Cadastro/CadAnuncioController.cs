using ControleImoveis.Web.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace ControleImoveis.Web.Controllers.Cadastro
{
    public class CadAnuncioController : Controller
    {

        private const int _quantMaxLinhasPorPagina = 5;



        public ActionResult Indexx()
        {
            ViewBag.ListaTamPag = new SelectList(new int[] { _quantMaxLinhasPorPagina, 10, 15, 20 }, _quantMaxLinhasPorPagina);
            ViewBag.QuantMaxLinhasPorPagina = _quantMaxLinhasPorPagina;
            ViewBag.PaginaAtual = 1;

            var usuariologado = User.Identity.Name;

            var lista = AnuncioModel.RecuperarLista(ViewBag.PaginaAtual, _quantMaxLinhasPorPagina, nomeusu: usuariologado);
            var quant = AnuncioModel.RecuperarQuantidade(nomeusu: usuariologado);
            ViewBag.QuantidadeRegistros = quant;

            var difQuantPaginas = (quant % ViewBag.QuantMaxLinhasPorPagina) > 0 ? 1 : 0;
            ViewBag.QuantPaginas = (quant / ViewBag.QuantMaxLinhasPorPagina) + difQuantPaginas;
            ViewBag.Estados = EstadoModel.RecuperarLista();
            ViewBag.Cidade = CidadeModel.RecuperarLista();
            ViewBag.Bairro = BairroModel.RecuperarLista();
            ViewBag.TipoImovel = TipoImovelModel.RecuperarLista();
            var bairro = Enum.GetNames(typeof(BairroModel.Regiao)).ToList();
            var negocio = Enum.GetNames(typeof(AnuncioModel.Negocio)).ToList();

            ViewBag.regiao = bairro;
            ViewBag.negocio = negocio;

            return View(lista);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult AnuncioPagina(int pagina, int tamPag, string filtro, string ordem)
        {
            var lista = AnuncioModel.RecuperarLista(pagina, tamPag, filtro, ordem);

            return Json(lista);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult RecuperarAnuncio(int id)
        {
            var vm = AnuncioModel.RecuperarPeloId(id);

            return Json(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult ExcluirAnuncio(int id)
        {
            return Json(AnuncioModel.ExcluirPeloId(id));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult SalvarAnuncio(AnuncioModel model)
        {
            var resultado = "OK";
            var mensagens = new List<string>();
            var idSalvo = string.Empty;
            model.Nomeusu = User.Identity.Name;

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
        public ActionResult UploadFiles()
        {
            // Checking no of files injected in Request object  
            if (Request.Files.Count > 0)
            {
                try
                {
                    var anuncio = AnuncioModel.RecuperarId(User.Identity.Name);
                   
                    //  Get all files from Request object  
                    HttpFileCollectionBase files = Request.Files;
                    for (int i = 0; i < files.Count; i++)
                    {
                        //string path = AppDomain.CurrentDomain.BaseDirectory + "Uploads/";  
                        //string filename = Path.GetFileName(Request.Files[i].FileName);  

                        HttpPostedFileBase file = files[i];
                        string fname;

                        // Checking for Internet Explorer  
                        if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
                        {
                            string[] testfiles = file.FileName.Split(new char[] { '\\' });
                            fname = testfiles[testfiles.Length - 1];
                        }
                        else
                        {
                            fname = file.FileName;
                        }

                        string folder = @"~/Uploads/" + anuncio.Id; //nome do diretorio a ser criado
                     
                        //Se o diretório não existir...

                        if (!Directory.Exists(folder))
                        {
                            //Criamos um com o nome folder
                            Directory.CreateDirectory(folder);
                        }

                        // Get the complete folder path and store the file inside it.  
                        fname = Path.Combine(Server.MapPath(folder), fname);
                        file.SaveAs(fname);
                    }
                    // Returns message that successfully uploaded  
                    return Json("File Uploaded Successfully!");
                }
                catch (Exception ex)
                {
                    return Json("Error occurred. Error details: " + ex.Message);
                }
            }
            else
            {
                return Json("No files selected.");
            }
        }



    }
}