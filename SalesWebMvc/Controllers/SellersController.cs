using Microsoft.AspNetCore.Mvc;
using SalesWebMvc.Models;
using SalesWebMvc.Services;
using SalesWebMvc.Models.ViewModels;
using System.Collections.Generic;
using SalesWebMvc.Services.Exceptions;
using System.Diagnostics;
using System;
using System.Threading.Tasks;
using System.Linq;
namespace SalesWebMvc.Controllers
{
    public class SellersController : Controller
    {
        private readonly SellerService _sellerService;// variável privada do tipo SellerService que será usada para acessar os métodos do serviço de vendedores
        private readonly DepartmentService _departmentService;
        public SellersController(Services.SellerService sellerService, DepartmentService departmentService)// construtor que recebe uma instância do SellerService como parâmetro e a atribui à variável _sellerService
        {
            _sellerService = sellerService;
            _departmentService = departmentService;
        }

        public async Task<IActionResult> Index()// ação Index é responsável por lidar com as requisições para a página inicial dos vendedores. Ela chama o método FindAll do SellerService para obter a lista de vendedores e, em seguida, retorna essa lista para a view correspondente.
        {
            var list = await _sellerService.FindAllAsync(); // Call the FindAll method from the SellerService to get the list of sellers
                                                 // A operação retorna uma lista de vendedores, que é então passada para a view para exibição.

            return View(list);// passar alista de vendedores para a view
        }
        public async Task<IActionResult> Create()
        {
            var departments =  await _departmentService.FindAllAsync();// chama o método FindAll do DepartmentService para obter a lista de departamentos
            var viewModel = new SellerFormViewModel { Departments = departments };// cria uma instância de SellerFormViewModel e atribui a lista de departamentos obtida ao atributo Departments do view ;model
            return View(viewModel);

        }

        [HttpPost]// atributo que indica que o método Create é responsável por lidar com requisições HTTP POST.
        [ValidateAntiForgeryToken]// atributo que indica que o método Create é responsável por lidar com requisições HTTP POST e que a validação do token antifalsificação deve ser aplicada para proteger contra ataques CSRF (Cross-Site Request Forgery).
        public async Task <IActionResult> Create(Seller seller)// ação Create é responsável por lidar com a criação de um novo vendedor. Ela recebe um objeto Seller como parâmetro, que contém os dados do vendedor a ser criado. O método chama o método Insert do SellerService para inserir o novo vendedor no banco de dados e, em seguida, redireciona para a ação Index.
        {
            if(!ModelState.IsValid)
            {
                var departments = await _departmentService.FindAllAsync();
                var viewModel = new SellerFormViewModel { Seller = seller, Departments = departments };
                return View(viewModel);
            }
            await _sellerService.InsertAsync(seller);// chama o método Insert do SellerService para inserir o novo vendedor no banco de dados
            return RedirectToAction(nameof(Index));// redireciona para a ação Index após a criação do vendedor
        }
        public async Task<IActionResult> Delete(int? id)// ação Delete é responsável por lidar com a exclusão de um vendedor. Ela recebe um parâmetro id que representa o identificador do vendedor a ser excluído. O método chama o método Remove do SellerService para remover o vendedor do banco de dados e, em seguida, redireciona para a ação Index.
        {
            if (id == null)
            {
                return RedirectToAction(nameof(Error), new {message="Id not provided"});
            }
            var obj = await _sellerService.FindByIdAsync(id.Value);// chama o método FindById do SellerService para buscar o vendedor pelo id
            if (obj == null)
            {
                return RedirectToAction(nameof(Error), new { message = "Id not found" });
            }
            return View(obj);// retorna a view de confirmação de exclusão, passando o objeto do vendedor para exibição

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task <IActionResult> Delete(int id)
        {
            await  _sellerService.RemoveAsync(id);
            return RedirectToAction(nameof(Index));
        }
        public async Task <IActionResult> Details (int? id)
        {
            if (id == null)
            {
                return RedirectToAction(nameof(Error), new { message = "Id not provided" });
            }
            var obj = await _sellerService.FindByIdAsync(id.Value);
            if (obj == null)
            {
                return RedirectToAction(nameof(Error), new { message = "Id not found" });
            }
            return View(obj);

        }
        public async Task<IActionResult> Edit(int? id)
        {


            if (id == null)
            {
                return RedirectToAction(nameof(Error), new { message = "Id not provided" });
            }
            var obj = await _sellerService.FindByIdAsync(id.Value);
            if (obj == null)
            {
                return RedirectToAction(nameof(Error), new { message = "Id not found" });
            }
            List<Department> departments = await _departmentService.FindAllAsync();
            SellerFormViewModel viewModel = new SellerFormViewModel { Seller = obj, Departments = departments };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Seller seller)
        {
            if (!ModelState.IsValid)
            {
                var departments = await _departmentService.FindAllAsync();
                var viewModel = new SellerFormViewModel { Seller = seller, Departments = departments };
                return View(viewModel);
            }
            if (id != seller.Id)
            {
                return RedirectToAction(nameof(Error), new { message = "Id mismatch" });
            }
            try { 

            await _sellerService.UpdateAsync(seller);

            return RedirectToAction(nameof(Index));
            }
            catch (ApplicationException e)
            {
                return RedirectToAction(nameof(Error), new { message = e.Message });
            }
           
        }
        public IActionResult Error(string message)
        {
            var viewModel = new ErrorViewModel
            {
                Message = message,
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            };
            return View(viewModel);
        }
    }
}
