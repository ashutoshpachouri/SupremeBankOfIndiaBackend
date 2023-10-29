using BackendSBI.Models;
using BackendSBI.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackendSBITest
{
    internal class AccountRepositoryTests
    {
        [TestMethod]
        public async Task AddBeneficiaryAsync_ShouldAddBeneficiary()
        {
            // Arrange
            var dbContextOptions = new DbContextOptionsBuilder<AccountsDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            using (var context = new AccountsDbContext(dbContextOptions))
            {
                var repository = new AccountRepository(context, Mock.Of<IOptions<MailSettings>>());

                var beneficiary = new Beneficiary
                {
                    // Initialize beneficiary properties as needed for your test
                };

                // Act
                await repository.AddBeneficiaryAsync(beneficiary);

                // Assert
                var addedBeneficiary = await context.Beneficiaries.FirstOrDefaultAsync();
                Assert.IsNotNull(addedBeneficiary);
                // Add more specific assertions based on your test data and expectations.
            }
        }

        [TestMethod]
        public async Task AddTransactionAsync_ShouldAddTransaction()
        {
            // Arrange
            var dbContextOptions = new DbContextOptionsBuilder<AccountsDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            using (var context = new AccountsDbContext(dbContextOptions))
            {
                var repository = new AccountRepository(context, Mock.Of<IOptions<MailSettings>>());

                var transaction = new Transaction
                {
                    // Initialize transaction properties as needed for your test
                };

                // Act
                await repository.AddTransactionAsync(transaction);

                // Assert
                var addedTransaction = await context.Transactions.FirstOrDefaultAsync();
                Assert.IsNotNull(addedTransaction);
                // Add more specific assertions based on your test data and expectations.
            }
        }

        [TestMethod]
        public async Task UpdatePasswordAsync_ShouldUpdatePassword()
        {
            // Arrange
            var dbContextOptions = new DbContextOptionsBuilder<AccountsDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            using (var context = new AccountsDbContext(dbContextOptions))
            {
                var repository = new AccountRepository(context, Mock.Of<IOptions<MailSettings>>());

                var user = new InternetBanking
                {
                    // Initialize user properties as needed for your test
                };

                var newPassword = "newPassword";

                // Act
                await repository.UpdatePasswordAsync(user, newPassword);

                // Assert
                var updatedUser = await context.InternetBankings.FirstOrDefaultAsync();
                Assert.AreEqual(newPassword, updatedUser.Password);
                // Add more specific assertions based on your test data and expectations.
            }
        }
    }
}

