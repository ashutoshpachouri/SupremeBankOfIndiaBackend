using BackendSBI.Models;
using BackendSBI.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Crypto;
using System.Security.Claims;

namespace BackendSBI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
   /* [Authorize]*/
    [Authorize(Roles = "User")]
    public class DashboardController : ControllerBase
    {
        
        private readonly IAccountRepository _accountRepository;
        private readonly ILogger<DashboardController> _logger;
        public DashboardController(IAccountRepository accountRepository, ILogger<DashboardController> _logger)
        {
          
            _accountRepository = accountRepository;
            this._logger = _logger;
        }
      
        [HttpGet("GetUserDetails")]
        public async Task<IActionResult> GetDashboardDetails()
        {
            try
            {
                _logger.LogInformation("Retrieving dashboard details");
                // Retrieve the current user's email from the claims
                var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;

                // Query the repository to get Internet banking details for the user
                var internetBankingDetails = await _accountRepository.GetInternetBankingByUserEmailAsync(userEmail);
                var accountDetails = await _accountRepository.GetAccountByUserEmailAsync(userEmail);

                if (accountDetails != null && internetBankingDetails != null)
                {
                    var dashboardData = new
                    {
                        AccountDetails = accountDetails,
                        InternetBankingDetails = internetBankingDetails
                    };
                    return Ok(dashboardData);
                }

                if (internetBankingDetails == null)
                {
                    return NotFound("Internet banking details not found for the user.");
                }

                return Ok(internetBankingDetails);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving dashboard data.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving dashboard data.");
            }
        }
        /**/
        [HttpPut("changepassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePassword model)
        {
            try
            {
                _logger.LogInformation("Changing password");
                var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
                var user = await _accountRepository.GetInternetBankingByUserEmailAsync(userEmail);

                if (user == null)
                {
                    return NotFound("User not found.");
                }

                if (model.OldPassword != user.Password)
                {
                    return BadRequest("Old password is incorrect.");
                }

                await _accountRepository.UpdatePasswordAsync(user, model.NewPassword);

                return Ok("Password changed successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing password.");
                // Handle any exceptions that may occur during password change
                return StatusCode(500, "Error changing password."); 
            }
        }
        [HttpPost("SaveBeneficiary")]
        public IActionResult AddBeneficiary(Beneficiary beneficiary)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                _logger.LogInformation("Saving beneficiary");
                _accountRepository.AddBeneficiaryAsync(beneficiary);
                //return Ok(new { message = "Beneficiary saved successfully" });
                 return Ok("Beneficiary saved successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving beneficiary.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error saving beneficiary.");
            }
        }

        /**/
        [HttpPost("Transaction")]
        public async Task<IActionResult> TransactionAsync(Transaction trans)
        {
            try
            {
                _logger.LogInformation("Performing transaction");
                // Validate the model
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Check if payer and payee accounts exist
                var payerAccount = await _accountRepository.GetInternetBankingByAccountNumberAsync(trans.PayerAccount);
                var payeeAccount = await _accountRepository.GetBeneficiaryByIdAsync(trans.PayeeAccount);

                if (payerAccount == null)
                {
                    return BadRequest("Payer account not found.");
                }

                if (payeeAccount == null)
                {
                    return BadRequest("Payee account not found.");
                }

                var transaction = new Transaction
                {
                    TransactionId = Guid.NewGuid(),
                    PayeeAccount = payeeAccount.AccountNumber,
                    PayerAccount = payerAccount.AccountNumber,
                    Amount = trans.Amount,
                    TDate = trans.TDate,
                    Remark = trans.Remark,
                    Mode = trans.Mode
                };

                await _accountRepository.AddTransactionAsync(transaction);

                // Implement your RTGS transaction logic here, including updating balances, recording the transaction, etc.
                var payerEmail = User.FindFirst(ClaimTypes.Email)?.Value;
                var subject = "NEFT Transaction Confirmation";
                var content = $@"
                <p>Dear Customer,</p>
                <p>Your {trans.Mode} transaction of {trans.Amount} to account {trans.PayeeAccount} was successful.</p>
                <p>Transaction Date: {trans.TDate}</p>
                <p>Remark: {trans.Remark}</p>
                <p>Thank you for choosing our bank for your financial transactions.</p>";

                await _accountRepository.SendEmailAsync(payerEmail, subject,content);

                return Ok("Transaction successful");
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "Error performing transaction.");
                // Handle any exceptions that may occur during the transaction
                return StatusCode(500, "Error performing transaction.");
            }
        }



        [HttpGet("GetMyTransactions")]
        public async Task<IActionResult> GetMyTransactions()
        {
            try
            {
                _logger.LogInformation("Retrieving user transactions");
                // Retrieve the current user's email and account number from the claims
                // var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
                var userAccountNumber = User.FindFirst("acnumber")?.Value; // Retrieve the acnumber claim

                if (/*string.IsNullOrEmpty(userEmail) ||*/ string.IsNullOrEmpty(userAccountNumber))
                {
                    return BadRequest("User email or account number not found in claims.");
                }

                // Query the repository to retrieve transactions for the user's account number
                var transactions = await _accountRepository.GetTransactionsForUserAsync(userAccountNumber);

                if (transactions.Count == 0)
                {
                    return NotFound("No transactions found for the user.");
                }

                return Ok(transactions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user transactions.");    
                // Handle any exceptions that may occur during the retrieval
                return StatusCode(500, "Error retrieving transactions.");
            }
        }

        [HttpGet("GetTransactionRecipt")]
        public async Task<IActionResult> GetTransactionRecipt()
        {
            var transaction = _accountRepository.GetTransactionRecipt();
            if(transaction == null)
            {
                return BadRequest("not found");
            }
            return Ok(transaction);
        }

       
    }
}
