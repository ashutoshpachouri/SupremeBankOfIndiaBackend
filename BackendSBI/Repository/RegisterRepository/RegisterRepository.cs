using BackendSBI.Models;
using Microsoft.AspNetCore.Mvc;
using SendGrid.Helpers.Mail;
using System.Reflection.Metadata.Ecma335;
using System.Security.Principal;

namespace BackendSBI.Repository.RegisterRepository
{
    public class RegisterRepository : IRegisterRepository
    {
        private readonly AccountsDbContext _context;
        private readonly IAccountRepository _accountRepository;
        public RegisterRepository(AccountsDbContext context, IAccountRepository _accountRepository)
        {
            _context = context;
            this._accountRepository = _accountRepository;
        }
        public async Task AddAccount(Accounts account)
        {
            _context.Account.Add(account);
            await _context.SaveChangesAsync();
        }
        public async Task AddInternetBanking(InternetBanking IB)
        {
            _context.InternetBankings.Add(IB);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAccount(Accounts account,InternetBanking IB)
        {
            _context.Account.Remove(account);
            _context.InternetBankings.Remove(IB);
            await _context.SaveChangesAsync();
           
        }

        public async Task<List<Accounts>> GetApproval()
        {
            return _context.Account.Where(u => u.isApproved == false).ToList();
           
        }
        public async Task ApproveUser(string email, bool isApproved)
        {
            var user = _context.Account.Where(u => u.Email == email).FirstOrDefault();
            user.isApproved = isApproved;
            if (isApproved)
            {
                await _context.SaveChangesAsync();
            }
            else
            {
                _context.Account.Remove(user);
                await _context.SaveChangesAsync();
            }
        }
            /*  public async Task ApproveUser(string email, bool isApproved)
              {
                  var user = _context.Account.Where(u => u.Email == email).FirstOrDefault();
                  user.isApproved = isApproved;
                  await _context.SaveChangesAsync();
                  if (isApproved)
                  {
                      var newOtp = GenerateOtp();
                      var AccNo = GenerateAccountNo();
                      var subject = "Registration Successful";
                      var content = "Dear User,\n\n" +
                        "Congratulations and welcome to our community! Your registration is complete, and we're thrilled to have you on board.\n" +
                        "...\n" +
                        "To enjoy the full benefits of our services, we invite you to register for Internet Banking:\n\n" +
                        $" Your Otp for Internet Banking  is <h1>{newOtp}</h1> Do not share this otp with anyone.\n" +
                        $" Your AccountNumber for Internet Banking  is <h2>{AccNo}</h2>\n" +
                        "**How to Register for Internet Banking:**\n\n" +
                        "1. **Log In**: Visit our website at [www.supremebankofindia.com].\n" +
                        "2. **Register**: Click on the 'Register for Internet Banking' option.\n" +
                        "3. **Verification**: Provide your account details, including your [Account Number] and [Email Address].\n" +
                        "4. **Set Up**: Follow the on-screen instructions to set up your Internet Banking credentials.\n" +
                        "5. **Log In**: Once registered, log in to your new Internet Banking account using your username and password.\n" +
                        "...\n" +
                        "If you have any questions or need assistance, don't hesitate to reach out to our dedicated support team at [Support Email Address].\n" +
                        "...\n" +
                        "Warm regards,\n" +
                        "[Ashutosh Pachouri]\n" +
                        "[CEO at Supreme Bank Of India]\n" +
                        "[Supreme Bank Of India]\n" +
                        "[www.supremebankofindia.com]\n" +
                        "[8434652389]";
                      _accountRepository.SendEmailAsync(user.Email, subject, content).Wait(); // Use .Wait() to make it synchronous
                  }
                  else
                  {
                      var subject = "Registration Successful";
                      var content = "Dear User, Registration UnSuccessful";
                      _accountRepository.SendEmailAsync(user.Email, subject, content).Wait();
                      _context.Account.Remove(user);
                      await _context.SaveChangesAsync();
                  }
              }
              private string GenerateOtp()
              {


                  return new Random().Next(100000, 999999).ToString();
              }
              private string GenerateAccountNo()
              {

                  return new Random().Next(100000000, 999999999).ToString();
              }*/
    }
}
