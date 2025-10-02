using BusinessObjects.Models;
using DataAccessLayer.Repositories;

namespace Services.Service
{
    public interface IDealerService
    {
        Task<List<Dealer>> GetAllDealers();
        Task<Dealer?> GetById(Guid id);
    }

    public class DealerService : IDealerService
    {
        private readonly IDealerRepository _dealerRepository;
        public DealerService(IDealerRepository dealerRepository)
        {
            _dealerRepository = dealerRepository;
        }

        public Task<List<Dealer>> GetAllDealers() => _dealerRepository.GetAll();
        public Task<Dealer?> GetById(Guid id) => _dealerRepository.GetById(id);
    }
}
