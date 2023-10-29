using BackendSBI.Models;
using BackendSBI.Repository;
using BackendSBI.Repository.RegisterRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Crypto.Generators;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;
using System.Security.Principal;
using static System.Net.WebRequestMethods;

namespace BackendSBI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegisterController : ControllerBase
    {
        private readonly IConfiguration Config;
        private readonly AccountsDbContext _context;
        private readonly IAccountRepository _accountRepository;
        private readonly IRegisterRepository _registerRepository;
        private readonly ILogger<AccountRepository> _logger;
        public RegisterController(IConfiguration Config, AccountsDbContext _context,IAccountRepository accountRepository,IRegisterRepository registerRepository, ILogger<AccountRepository> _logger)
        {
            this.Config = Config;
            this._context = _context;
            this._accountRepository = accountRepository;
            this._registerRepository = registerRepository;
            this._logger = _logger;
        }


        [AllowAnonymous]
        [HttpPost("RegisterAccount")]
        public IActionResult CreateUser(Accounts account)
        {
            _logger.LogInformation("Creating a new user account for email: {email}", account.Email);
            if (_context.Account.Where(u => u.Email == account.Email).FirstOrDefault() != null)
            {
                _logger.LogInformation("Email already exists for email: {email}", account.Email);
                return Ok("Email already exists");
            }
            account.isApproved = false;
            _registerRepository.AddAccount(account);
            /* _context.Account.Add(account);
             _context.SaveChanges();*/
           /* var newOtp = GenerateOtp();
            var subject = "Registration Successful";
            var content = "Dear User,\n\n" +
                  "Congratulations and welcome to our community! Your registration is complete, and we're thrilled to have you on board.\n" +
                  "...\n" +  
                  "To enjoy the full benefits of our services, we invite you to register for Internet Banking:\n\n" +
                  $" Your Otp for Internet Banking  is {newOtp} Do not share this otp with anyone.\n"+
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
             
             _accountRepository.SendEmailAsync(account.Email, subject, content).Wait();*/ // Use .Wait() to make it synchronous
            
            _logger.LogInformation("User registration successful for email: {email}", account.Email);
            return Ok("Success");
        }

        [AllowAnonymous]
        [HttpPost("RegisterInternetBanking")]
        public IActionResult CreateInternetBanking(InternetBanking user)
        {
            if (_context.Account.Where(u => u.Email == user.Email).FirstOrDefault() == null)
            {
                return Ok("Email not matched");
            }
            var userName = _context.Account
        .Where(u => u.Email == user.Email)
        .Select(u => u.FullName)
        .FirstOrDefault();
            _registerRepository.AddInternetBanking(user);
            /*   _context.InternetBankings.Add(user);
               _context.SaveChanges();*/
            var subject = "Welcome to Our Community! Internet Banking Registration Successful";
            var content = $"Dear {userName},\n\n" +
                      "We are delighted to welcome you to our community! Your registration is complete, and you are now a valued member of our growing family.\n" +
                      "Here are a few things you can do now:\n\n" +
                      "Explore Your Account: Log in to your account at [www.supremebankofindia.com] to access all the features and services we offer.\n" +
                      "Connect with Us: Follow us on social media ([Links to Social Media Profiles]) to stay updated on the latest news, events, and exclusive offers.\n" +
                      "Get Started: If you have any questions or need assistance, our support team is here to help. Contact us at [supremebankofindia@sobi.com].\n" +
                      "...\n" +
                      "Thank you for choosing [www.supremebankofindia.com]. We are here to make your journey with us memorable and enjoyable.\n" +
                      "Warm regards,\n" +
                      "[Ashutosh Pachouri]\n" +
                  "[CEO at Supreme Bank Of India]\n" +
                  "[Supreme Bank Of India]\n" +
                  "[www.supremebankofindia.com]\n" +
                  "[8434652389]";

            _accountRepository.SendEmailAsync(user.Email, subject, content).Wait(); // Use .Wait() to make it synchronous

            return Ok("Success");
        }
       
        [HttpDelete("Delete/{email}")]
        public IActionResult DeleteAccount(string email)
        {
            _logger.LogInformation("Deleting account with email: {email}", email);
            var accountToDel = _context.Account.Where(u=> u.Email == email).FirstOrDefault();
            if (accountToDel == null)
            {
                _logger.LogWarning("Account not found for email: {email}", email);
                return NotFound("AccountNotFound");
            }
            var IBToDel = _context.InternetBankings.Where(u => u.Email == email).FirstOrDefault();
            if (IBToDel == null)
            {
                _logger.LogWarning("Internet Banking not found for email: {email}", email);
                return NotFound("InternetBankingNotFound");
            }
            _registerRepository.DeleteAccount(accountToDel,IBToDel);
            _logger.LogInformation("Account deleted successfully for email: {email}", email);
            return Ok("Success");
        }


        [AllowAnonymous]
        [HttpPost("Login")]
        public IActionResult LoginUser(Login user)
        {
            var user1 = _context.InternetBankings.FirstOrDefault(u => u.Email == user.Email && u.Password == user.Password);
            if(user1 == null)
            {
                return Ok("Failure");
            }
            if (user != null)
            {
                var accountNumber = user1.AccountNumber;
                var claims = new[]
                {
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim("acnumber", accountNumber),
                    new Claim(ClaimTypes.Role, "User")
                };
                return Ok(new jwtService(Config).GenerateJwtToken(claims));
            }
            else
            {
                return Ok("Failure");
            }
            /*_logger.LogInformation("User attempting to log in with email: {email}", user.Email);
            var UserDetails = _context.InternetBankings.Where(u => u.Email == user.Email && u.Password == user.Password).FirstOrDefault();
            if (UserDetails != null)
            {
                _logger.LogInformation("User logged in successfully with email: {email}", user.Email);
                return Ok(new jwtService(Config).GenerateToken(UserDetails.Email, UserDetails.AccountNumber));
            }
            _logger.LogWarning("Login failed for email: {email}", user.Email);
            return Ok("Failure");*/
        }

        [AllowAnonymous]
        [HttpPost("Login/Admin")]
        public IActionResult LoginAdmin(Login user)
        {
            bool check = false;
            string eml = "Ashu@gmail.com"; string pass = "Ashu@123";
            if (user.Email == eml && user.Password == pass)
            {
                check = true;
            }
            if (check == false)
            {
                return Ok("Failure");
            }
            else
            {
                var claims = new[]
                {
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, "Admin")
                };
                return Ok(new jwtService(Config).GenerateJwtToken(claims));
            }
        }

        [HttpPost("forgotpassword")]
        public IActionResult ForgotPassword([FromBody] ForgotPasswordRequest model)
        {
            _logger.LogInformation("Forgot password request for user with email: {email}", model.UserId);

            var user = _context.InternetBankings.FirstOrDefault(u => u.Email == model.UserId);

            if (user == null)
            {
                _logger.LogWarning("User not found for email: {email}", model.UserId);
                return NotFound("User not found.");
            }

            // Generate and send a new OTP
            var newOtp = GenerateOtp();
            // For simplicity, you can create a method to send an email with the OTP
            _accountRepository.SendEmailAsync(user.Email, "Reset OTP SBI", $" Your Otp for reset password is {newOtp} Do not share this otp with anyone.");

            // Replace the previous OTP with the new one in the database
            _accountRepository.SaveOtpToDatabase(user.Email, newOtp);
            _logger.LogInformation("OTP sent successfully to user with email: {email}", user.Email);

            return Ok("OTP sent successfully.");
        }


        [HttpPost("resetpassword")]
        public IActionResult ResetPassword([FromBody] ResetPasswordRequest model)
        {
            _logger.LogInformation("Resetting password for user with email: {email}", model.UserId);
            var user = _context.InternetBankings.FirstOrDefault(u => u.Email == model.UserId);

            if (user == null)
            {
                _logger.LogWarning("User not found for email: {email}", model.UserId);
                return NotFound("User not found.");
            }

            // Validate the OTP
            if (_accountRepository.ValidateOtpFromDatabase(user.Email, model.Otp))
            {
                user.Password = model.NewPassword;

        
                _context.SaveChanges();
                _logger.LogInformation("Password reset successfully for user with email: {email}", user.Email);

                return Ok("Password reset successfully.");
            }
            else
            {

                _logger.LogWarning("Password reset failed for user with email: {email} - Invalid OTP", user.Email);
                return BadRequest("Invalid OTP.");
            }
        }

      
        private string GenerateOtp()
        {
            
            
            return new Random().Next(100000, 999999).ToString();
        }


    }
}
