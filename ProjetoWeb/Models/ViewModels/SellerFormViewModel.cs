namespace ProjetoWeb.Models.ViewModels
{
    // Classe "Pacote" criada apenas para levar dados múltiplos para as telas de formulário (Create/Edit)
    public class SellerFormViewModel
    {
        // Propriedade para carregar os dados cadastrais de um único Vendedor
        public Seller Seller { get; set; }

        // Propriedade para carregar a lista de opções de Departamentos que preencherão o select HTML
        public ICollection<Department> Departments { get; set; }
    }
}