﻿using BackendSBI.Models;

namespace BackendSBI.Repository
{
    public interface IAccountRepository
    {
        Task AddBeneficiaryAsync(Beneficiary beneficiary);
        Task AddTransactionAsync(Transaction transaction);
        Task<Accounts> GetAccountByUserEmailAsync(string email);
        Task<Beneficiary> GetBeneficiaryByIdAsync(string accountNumber);
        Task<InternetBanking> GetInternetBankingByUserEmailAsync(string email);
        Task<InternetBanking> GetInternetBankingByAccountNumberAsync(string accountNumber);
        Task<List<Transaction>> GetTransactionsForUserAsync(string userAccountNumber);
        Task<Transaction> GetTransactionByIdAsync(Guid id);
        Task UpdatePasswordAsync(InternetBanking user, string newPassword);
        Task SendEmailAsync(string emailAddress, string subject, string content);
        public void SaveOtpToDatabase(string userId, string newOtp);
        public bool ValidateOtpFromDatabase(string userId, string otp);

        public Task<object> GetTransactionRecipt();
    }
}