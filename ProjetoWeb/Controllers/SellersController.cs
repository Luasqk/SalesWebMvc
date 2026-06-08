using Microsoft.AspNetCore.Mvc;
using ProjetoWeb.Models;
using ProjetoWeb.Models.ViewModels;
using ProjetoWeb.Services;
using ProjetoWeb.Services.Exceptions;
using System.Diagnostics;

namespace ProjetoWeb.Controllers
{
    // Controller responsável por intermediar as ações da tela de Vendedores (Sellers)
    public class SellersController : Controller
    {
        // Declaração das dependências dos serviços que o controller vai usar
        private readonly SellerService _sellerService;
        private readonly DepartmentService _departmentService;

        // Construtor: Injeta automaticamente as instâncias dos serviços configurados no Program.cs
        public SellersController(SellerService sellerService, DepartmentService departmentService)
        {
            _sellerService = sellerService;
            _departmentService = departmentService;
        }

        // GET: Sellers (Tela Principal/Listagem)
        public IActionResult Index()
        {
            // Busca a lista de todos os vendedores chamando a camada de serviço
            var list = _sellerService.FindAll();
            // Envia a lista para a View (Index.cshtml) renderizar a tabela na tela
            return View(list);
        }

        // GET: Sellers/Create (Apenas abre a tela do formulário de cadastro)
        public IActionResult Create()
        {
            // Busca todos os departamentos para preencher a caixinha de seleção (dropdown)
            var departments = _departmentService.FindAll();

            // Cria o pacote (ViewModel) juntando a lista de departamentos que a tela precisa
            var viewModel = new SellerFormViewModel { Departments = departments };

            // Envia o pacote para a View (Create.cshtml)
            return View(viewModel);
        }

        // POST: Sellers/Create (Disparado quando o usuário clica em "Salvar" no formulário)
        [HttpPost]
        [ValidateAntiForgeryToken] // Trava de segurança contra ataques de falsificação de requisição (CSRF)
        public IActionResult Create(Seller seller)
        {
            if (!ModelState.IsValid)
            {
                var departments = _departmentService.FindAll();
                var viewModel = new SellerFormViewModel { Seller = seller, Departments = departments };
                return View(viewModel);
            }
            // Envia os dados preenchidos do vendedor para o serviço salvar no banco de dados
            _sellerService.Insert(seller);
            // Redireciona o usuário de volta para a tela de listagem (Index)
            return RedirectToAction(nameof(Index));
        }

        // GET: Sellers/Delete/5 (Abre a tela de confirmação de exclusão)
        public IActionResult Delete(int? id)
        {
            // Segurança: Se o ID não veio na URL, redireciona para a tela de erro customizada
            if (id == null)
            {
                return RedirectToAction(nameof(Error), new { message = "Id not provided. " });
            }

            // Busca o vendedor no banco usando o valor real do ID (id.Value resolve o tipo anulável int?)
            var obj = _sellerService.FindById(id.Value);

            // Segurança: Se buscou no banco e não achou ninguém com esse ID, vai para a tela de erro
            if (obj == null)
            {
                return RedirectToAction(nameof(Error), new { message = "Id not found. " });
            }

            // Envia o vendedor encontrado para a View (Delete.cshtml) exibir os dados dele antes de deletar
            return View(obj);
        }

        // POST: Sellers/Delete/5 (Disparado quando o usuário confirma que quer deletar na tela)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            // Chama o serviço para remover o registro do banco de dados pelo ID
            _sellerService.Remove(id);
            // Redireciona de volta para a listagem atualizada
            return RedirectToAction(nameof(Index));
        }

        // GET: Sellers/Details/5 (Abre a tela com informações detalhadas do vendedor)
        public IActionResult Details(int? id)
        {
            // Segurança: Valida se o ID foi fornecido na requisição
            if (id == null)
            {
                return RedirectToAction(nameof(Error), new { message = "Id not provided. " });
            }

            // Busca o vendedor (o Service aqui já traz os dados do Departamento junto por causa do Include)
            var obj = _sellerService.FindById(id.Value);

            // Segurança: Valida se o vendedor realmente existe no sistema
            if (obj == null)
            {
                return RedirectToAction(nameof(Error), new { message = "Id not found. " });
            }

            // Envia os dados completos do vendedor para a View (Details.cshtml)
            return View(obj);
        }

        // GET: Sellers/Edit/5 (Abre a tela do formulário de edição pré-preenchido)
        public IActionResult Edit(int? id)
        {
            // Segurança: Valida se o ID veio na URL
            if (id == null)
            {
                return RedirectToAction(nameof(Error), new { message = "Id not provided. " });
            }

            // Busca o vendedor atual para saber os dados antigos dele
            var obj = _sellerService.FindById(id.Value);

            // Segurança: Valida se o registro existe no banco
            if (obj == null)
            {
                return RedirectToAction(nameof(Error), new { message = "Id not found. " });
            }

            // Busca a lista de departamentos para o usuário poder alterar o departamento do vendedor se quiser
            List<Department> departments = _departmentService.FindAll();

            // Monta o ViewModel casando o Vendedor encontrado e a lista de Departamentos do sistema
            SellerFormViewModel viewModel = new SellerFormViewModel { Seller = obj, Departments = departments };

            // Envia o pacote completo para preencher o formulário da View (Edit.cshtml)
            return View(viewModel);
        }

        // POST: Sellers/Edit/5 (Disparado quando o usuário clica em "Salvar Alterações")
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Seller seller)
        {

            if (!ModelState.IsValid)
            {
                var departments = _departmentService.FindAll();
                var viewModel = new SellerFormViewModel { Seller = seller, Departments = departments };
                return View(viewModel);
            }

            // Segurança: Evita que os dados de um vendedor sejam salvos no ID de outro (Inconsistência de URL)
            if (id != seller.Id)
            {
                return RedirectToAction(nameof(Error), new { message = "Id mismatch " });
            }

            try
            {
                // Tenta atualizar os dados na camada de serviço
                _sellerService.Update(seller);
                // Se der certo, volta para a tabela principal de vendedores
                return RedirectToAction(nameof(Index));
            }
            catch (ApplicationException e) // Captura qualquer erro de negócio/aplicação mapeado
            {
                // Se der erro (ex: Id sumiu do banco no meio do processo), joga a mensagem na tela de erro
                return RedirectToAction(nameof(Error), new { message = e.Message });
            }
        }

        // GET: Sellers/Error (Ação centralizada que exibe a tela de erro amigável do sistema)
        public IActionResult Error(string message)
        {
            // Monta o modelo de erro injetando a mensagem dinâmica e capturando o ID interno do servidor
            var viewModel = new ErrorViewModel
            {
                Message = message,
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier // Pega o rastro do erro
            };
            // Envia para a página Error.cshtml exibir o Alerta Vermelho
            return View(viewModel);
        }
    }
}