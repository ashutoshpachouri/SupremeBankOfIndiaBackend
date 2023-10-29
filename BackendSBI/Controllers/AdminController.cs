using BackendSBI.Repository;
using BackendSBI.Repository.RegisterRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Data;

namespace BackendSBI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IRegisterRepository _registerRepository;
        private readonly IAccountRepository _accountRepository;
        public AdminController(IRegisterRepository _registerRepository,IAccountRepository _accountRepository)
        {
            this._registerRepository = _registerRepository;
            this._accountRepository = _accountRepository;
        }
       
        [HttpGet("ApprovalList")]
        public IActionResult approvalList()
        {
            var list = _registerRepository.GetApproval();
            return Ok(list);
        }

        [HttpPost("ApproveUser/{email}/{isApproved}")]
        public IActionResult ApproveUser(string email, bool isApproved)
        {
            _registerRepository.ApproveUser(email, isApproved);
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
                  $" Your AccountNumber for Internet Banking  is <h1>{AccNo}</h1>\n" +
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
                _accountRepository.SendEmailAsync(email, subject, content).Wait();
            }
            else
            {
                var subject = "Registration Unsuccessfull";
                var content = "Dear User, Registration UnSuccessfull";
                _accountRepository.SendEmailAsync(email, subject, content).Wait();
            }
            return Ok("Account Updated");
        }
        private string GenerateOtp()
        {


            return new Random().Next(100000, 999999).ToString();
        }
        private string GenerateAccountNo()
        {

            return new Random().Next(100000000, 999999999).ToString();
        }
    }

}
