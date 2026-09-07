using Microsoft.AspNetCore.Mvc;

namespace SalesWebMvc.Controllers
{
    public class SellersController : Controller 
    {
        private readonly Services.SellerService _sellerService;// variável privada do tipo SellerService que será usada para acessar os métodos do serviço de vendedores

        public SellersController(Services.SellerService sellerService)// construtor que recebe uma instância do SellerService como parâmetro e a atribui à variável _sellerService
        {
            _sellerService = sellerService;
        }

        public IActionResult Index()// ação Index é responsável por lidar com as requisições para a página inicial dos vendedores. Ela chama o método FindAll do SellerService para obter a lista de vendedores e, em seguida, retorna essa lista para a view correspondente.
        {
            var list = _sellerService.FindAll(); // Call the FindAll method from the SellerService to get the list of sellers
                                                 // A operação retorna uma lista de vendedores, que é então passada para a view para exibição.

            return View(list);// passar alista de vendedores para a view
        }
    }
}
