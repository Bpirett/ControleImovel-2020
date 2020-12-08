using ControleImoveis.Web.Models;
using System.Security.Principal;

namespace ControleImoveis.Web
{
    public class AplicacaoPrincipal : GenericPrincipal
    {
        public UsuarioModel Dados { get; set; }

        public AplicacaoPrincipal(IIdentity identity, string[] roles, int id) : base(identity, roles)
        {
            Dados = UsuarioModel.RecuperarPeloId(id);
        }
    }
}