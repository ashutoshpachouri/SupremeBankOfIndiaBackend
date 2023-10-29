using BackendSBI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MimeKit;
//using SendGrid.Helpers.Mail;
using System.Security.Principal;
using MimeKit;
//using System.Net.Mail;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Mvc;

namespace BackendSBI.Repository
{
    public class AccountRepository : IAccountRepository
    {
        private readonly AccountsDbContext _context;
        private readonly MailSettings _mailSettings;

        public AccountRepository(AccountsDbContext context, IOptions<MailSettings> mailSettingsOptions)
        {
            _context = context;
            _mailSettings = mailSettingsOptions.Value;
        } 
        public async Task<InternetBanking> GetInternetBankingByUserEmailAsync(string email)
        {
            return await _context.InternetBankings.FirstOrDefaultAsync(u => u.Email == email);
        }
        public async Task<InternetBanking> GetInternetBankingByAccountNumberAsync(string accountNumber)
        {
            return await _context.InternetBankings.FirstOrDefaultAsync(u => u.AccountNumber == accountNumber);
        }

        public async Task<Accounts> GetAccountByUserEmailAsync(string email)
        {
            return await _context.Account.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<Beneficiary> GetBeneficiaryByIdAsync(string accountNumber)
        {
            return await _context.Beneficiaries.FirstOrDefaultAsync(b => b.AccountNumber == accountNumber);
        }

        public async Task<Transaction> GetTransactionByIdAsync(Guid id)
        {
            return await _context.Transactions.FirstOrDefaultAsync(t => t.TransactionId == id);
        }
        public async Task<List<Transaction>> GetTransactionsForUserAsync(string userAccountNumber)
        {
            return await _context.Transactions
                .Where(t => t.PayerAccount == userAccountNumber || t.PayeeAccount == userAccountNumber)
                .ToListAsync();
        }


        public async Task AddBeneficiaryAsync(Beneficiary beneficiary)
        {
            _context.Beneficiaries.Add(beneficiary);
            await _context.SaveChangesAsync();
        }

        public async Task AddTransactionAsync(Transaction transaction)
        {
            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();
        }

        public async Task UpdatePasswordAsync(InternetBanking user, string newPassword)
        {
            
            user.Password = newPassword;
            await _context.SaveChangesAsync();
        }

    
        public async Task<object> GetUserDataAsync(string email)
        {
            var internetBankingDetails = await _context.InternetBankings.FirstOrDefaultAsync(u => u.Email == email);
            var accountDetails = await _context.Account.FirstOrDefaultAsync(u => u.Email == email);

            if (internetBankingDetails != null || accountDetails != null)
            {
                return new
                {
                    AccountDetails = accountDetails,
                    InternetBankingDetails = internetBankingDetails
                };
            }
            else
            {
                return null;
            }
        }
        public void SaveOtpToDatabase(string userId, string newOtp)
        {
            var user = _context.InternetBankings.FirstOrDefault(u => u.Email == userId);

            if (user != null)
            {
                // Replace the previous OTP with the new one
                user.Otp = newOtp;

                _context.SaveChanges();
            }
        }

        public bool ValidateOtpFromDatabase(string userId, string otp)
        {
            var user = _context.InternetBankings.FirstOrDefault(u => u.Email == userId && u.Otp == otp);

            if (user != null)
            {
                
                //user.Otp = null;

                _context.SaveChanges();

                return true;
            }

            return false;
        }
        public async Task SendEmailAsync(string emailAddress, string subject, string content)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(_mailSettings.DisplayName, _mailSettings.Mail)); // Replace with your Mailtrap email
                message.To.Add(new MailboxAddress("", emailAddress));
                message.Subject = subject;

                var bodyBuilder = new BodyBuilder();
                bodyBuilder.TextBody = content;

                message.Body = bodyBuilder.ToMessageBody();

                using (var client = new SmtpClient())
                {
                    await client.ConnectAsync(_mailSettings.Host, 587, false); // Replace with Mailtrap SMTP server and port
                    await client.AuthenticateAsync(_mailSettings.UserName, _mailSettings.Password); // Replace with your Mailtrap credentials
                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                }
            }
            catch (Exception ex)
            {
                // Handle email sending exceptions...
                throw;
            }
        }

        public async Task<object> GetTransactionRecipt()
        {
            var mostRecentTransaction = _context.Transactions
            .OrderByDescending(t => t.TDate) // Assuming "TransactionDate" is the date field
            .FirstOrDefault();
            return mostRecentTransaction;
        }
    }
}
