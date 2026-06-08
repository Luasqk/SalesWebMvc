using ProjetoWeb.Data;
using ProjetoWeb.Models;
using Microsoft.EntityFrameworkCore;
using ProjetoWeb.Services.Exceptions;

namespace ProjetoWeb.Services
{
    // Camada de Serviço: Responsável por processar e validar as regras de negócio antes de tocar no banco
    public class SellerService
    {
        // Dependência do contexto do Entity Framework (Acesso direto às tabelas)
        private readonly ProjetoWebContext _context;

        // Construtor: Injeta o contexto do banco de dados na classe
        public SellerService(ProjetoWebContext context)
        {
            _context = context;
        }

        // Retorna uma lista com todos os vendedores cadastrados no banco
        public List<Seller> FindAll()
        {
            return _context.Seller.ToList();
        }

        // Salva um novo vendedor no banco de dados
        public void Insert(Seller obj)
        {
            _context.Add(obj); // Prepara a inserção na memória do EF
            _context.SaveChanges(); // Commita/Executa o comando INSERT INTO no MySQL de verdade
        }

        // Busca um único vendedor baseado no ID dele
        public Seller FindById(int id)
        {
            // O .Include faz um INNER JOIN na tabela de Departamentos para carregar o objeto Department completo!
            // O .FirstOrDefault busca o primeiro que bater com a condição ou retorna nulo se não existir nada.
            return _context.Seller.Include(obj => obj.Department).FirstOrDefault(obj => obj.Id == id);
        }

        // Remove um vendedor do sistema baseado no ID
        public void Remove(int id)
        {
            // Busca a referência do objeto direto na tabela
            var obj = _context.Seller.Find(id);
            // Remove o registro do mapa do Entity Framework
            _context.Seller.Remove(obj);
            // Executa o comando DELETE no MySQL de verdade
            _context.SaveChanges();
        }

        // Atualiza as informações de um vendedor existente
        public void Update(Seller obj)
        {
            // Regra de Negócio/Segurança: Se não existir nenhum Vendedor com esse ID no banco, para tudo e avisa
            if (!_context.Seller.Any(x => x.Id == obj.Id))
            {
                throw new NotFoundException("Id not found. "); // Lança exceção customizada de não encontrado
            }

            try
            {
                _context.Update(obj); // Atualiza os campos modificados na memória
                _context.SaveChanges(); // Executa o comando UPDATE no banco
            }
            catch (DbConcurrencyException e) // Se houver erro de concorrência simultânea no banco (outro usuário deletou/editou ao mesmo tempo)
            {
                // Repassa o erro capturado tratando de forma especializada pela aplicação
                throw new DbConcurrencyException(e.Message);
            }
        }
    }
}