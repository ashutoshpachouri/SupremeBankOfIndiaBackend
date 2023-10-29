using BackendSBI.Models;

namespace BackendSBI.Repository.RegisterRepository
{
    public interface IRegisterRepository
    {
        Task AddAccount(Accounts account);
        Task AddInternetBanking(InternetBanking IB);
        Task DeleteAccount(Accounts account, InternetBanking IB);
        Task<List<Accounts>> GetApproval();
        Task ApproveUser(string email, bool isApproved);
    }
}