using ControleImoveis.Web.Models;
using Rotativa;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ControleImoveis.Web.Controllers.Relatorio
{
    public class RelatAnunciosController : Controller
    {
        public ActionResult Index()
        {
            var anuncios = RelatAnunciosModel.relatAnuncios(User.Identity.Name);

            return new ViewAsPdf("~/Views/Relatorio/RelatAnunciosView.cshtml", anuncios);
        }
    }
}