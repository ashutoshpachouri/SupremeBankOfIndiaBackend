using BackendSBI.Models;
using BackendSBI.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Moq;
using System.ComponentModel.DataAnnotations;

namespace BackendSBITest
{
    [TestClass]
    public class UnitTest1
    {

        [TestMethod]
        public void Beneficiary_Name_Required()
        {
            // Arrange
            var beneficiary = new Beneficiary { Name = null, AccountNumber = "12345" };

            // Act
            var context = new ValidationContext(beneficiary, null, null);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(beneficiary, context, results, true);

            // Assert
            Assert.IsFalse(isValid, "Name should be required.");
            Assert.AreEqual(1, results.Count, "Validation should produce one error for Name.");
        }

        [TestMethod]
        public void Beneficiary_AccountNumber_Required()
        {
            // Arrange
            var beneficiary = new Beneficiary { Name = "John Doe", AccountNumber = null };

            // Act
            var context = new ValidationContext(beneficiary, null, null);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(beneficiary, context, results, true);

            // Assert
            Assert.IsFalse(isValid, "AccountNumber should be required.");
            Assert.AreEqual(1, results.Count, "Validation should produce one error for AccountNumber.");
        }

        [TestMethod]
        public void Beneficiary_ValidData_Valid()
        {
            // Arrange
            var beneficiary = new Beneficiary { Name = "John Doe", AccountNumber = "12345" };

            // Act
            var context = new ValidationContext(beneficiary, null, null);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(beneficiary, context, results, true);

            // Assert
            Assert.IsTrue(isValid, "Beneficiary with valid data should pass validation.");
            Assert.AreEqual(0, results.Count, "Validation should not produce errors for valid data.");
        }
       /* [TestMethod]
        public async Task AddTransactionAsync_AddsTransactionToContext()
        {
            // Arrange
            var mockSet = new Mock<DbSet<Transaction>>();
            var mockContext = new Mock<AccountsDbContext>();
            mockContext.Setup(c => c.Transactions).Returns(mockSet.Object);
            mockContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1); // Return a positive result to indicate success

            var transactionService = new AccountRepository(mockContext.Object, null);
            var transactionToAdd = new Transaction
            {
                TransactionId = Guid.NewGuid(),
                PayeeAccount = "Payee123",
                PayerAccount = "Payer456",
                Amount = 100.00m,
                TDate = DateTime.Now,
                Remark = "Test Transaction",
                Mode = "Online"
            };

            // Act
            await transactionService.AddTransactionAsync(transactionToAdd);

            // Assert
            mockSet.Verify(set => set.Add(It.IsAny<Transaction>()), Times.Once);
            mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }*/


        [TestMethod]
        public void Transaction_PayeeAccount_Required()
        {
            // Arrange
            var transaction = new Transaction
            {
                PayeeAccount = null,
                PayerAccount = "Payer456",
                Amount = 100.00m,
                TDate = DateTime.Now,
                Remark = "Test Transaction",
                Mode = "Online"
            };

            // Act
            var context = new ValidationContext(transaction, null, null);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(transaction, context, results, true);

            // Assert
            Assert.IsFalse(isValid, "PayeeAccount should be required.");
            Assert.AreEqual(1, results.Count, "Validation should produce one error for PayeeAccount.");
        }

        [TestMethod]
        public void Transaction_PayerAccount_Required()
        {
            // Arrange
            var transaction = new Transaction
            {
                PayeeAccount = "Payee123",
                PayerAccount = null,
                Amount = 100.00m,
                TDate = DateTime.Now,
                Remark = "Test Transaction",
                Mode = "Online"
            };

            // Act
            var context = new ValidationContext(transaction, null, null);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(transaction, context, results, true);

            // Assert
            Assert.IsFalse(isValid, "PayerAccount should be required.");
            Assert.AreEqual(1, results.Count, "Validation should produce one error for PayerAccount.");
        }

       /* [TestMethod]
        public void Transaction_Amount_Required()
        {
            // Arrange
            var transaction = new Transaction
            {
                PayeeAccount = "Payee123",
                PayerAccount = "Payer456",
                Amount = 1000, // Zero amount, which is not allowed
                TDate = DateTime.Now,
                Remark = "Test Transaction",
                Mode = "Online"
            };

            // Act
            var context = new ValidationContext(transaction, null, null);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(transaction, context, results, true);

            // Assert
            Assert.IsFalse(isValid, "Amount should be required.");
            Assert.AreEqual(1, results.Count, "Validation should produce one error for Amount.");
        }
*/
        [TestMethod]
        public void Transaction_TDate_Required()
        {
            // Arrange
            var transaction = new Transaction
            {
                PayeeAccount = "Payee123",
                PayerAccount = "Payer456",
                Amount = 100.00m,
                TDate = DateTime.Now,
                Remark = "Test Transaction",
                Mode = "Online"
            };

            // Act
            var context = new ValidationContext(transaction, null, null);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(transaction, context, results, true);

            // Assert
            Assert.IsTrue(isValid, "TDate should be required.");
            Assert.AreEqual(0, results.Count, "Validation should not produce errors for a valid TDate.");
        }

        [TestMethod]
        public void Transaction_Remark_Required()
        {
            // Arrange
            var transaction = new Transaction
            {
                PayeeAccount = "Payee123",
                PayerAccount = "Payer456",
                Amount = 100.00m,
                TDate = DateTime.Now,
                Remark = null,
                Mode = "Online"
            };

            // Act
            var context = new ValidationContext(transaction, null, null);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(transaction, context, results, true);

            // Assert
            Assert.IsFalse(isValid, "Remark should be required.");
            Assert.AreEqual(1, results.Count, "Validation should produce one error for Remark.");
        }

        [TestMethod]
        public void Transaction_ValidData_Valid()
        {
            // Arrange
            var transaction = new Transaction
            {
                TransactionId = Guid.NewGuid(),
                PayeeAccount = "Payee123",
                PayerAccount = "Payer456",
                Amount = 100.00m,
                TDate = DateTime.Now,
                Remark = "Test Transaction",
                Mode = "Online"
            };

            // Act
            var context = new ValidationContext(transaction, null, null);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(transaction, context, results, true);

            // Assert
            Assert.IsTrue(isValid, "Transaction with valid data should pass validation.");
            Assert.AreEqual(0, results.Count, "Validation should not produce errors for valid data.");
        }
    }
}
