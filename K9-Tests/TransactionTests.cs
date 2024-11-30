using K9_Koinz.Data.Repositories;
using K9_Koinz.Models;
using Microsoft.Extensions.DependencyInjection;

namespace K9_Tests {
    public class TransactionTests : IClassFixture<TestFixture> {
        TestFixture fixture;

        public TransactionTests(TestFixture fixture) {
            this.fixture = fixture;
        }

        [Fact]
        public async Task GetAllTransactions_ReturnsSeededTransactions() {
            var transactionRepo = fixture.ServiceProvider.GetRequiredService<TransactionRepository>();

            // Act
            var transactions = await transactionRepo.GetAllAsync();

            // Assert
            Assert.Equal(29, transactions.Count());
        }
    }
}