using K9_Koinz.Data;
using K9_Koinz.Models;
using Moq;

namespace K9_Tests.Mocks {
    internal class MockITransactionRepository {
        public static Mock<ITransactionRepository> GetMock() {
            var mock = new Mock<ITransactionRepository>();

            var transactions = DbInitializer.CreateTransactions();

            mock.Setup(m => m.GetByAccountId(It.IsAny<Guid>()))
                .Returns((Guid id) => transactions.Where(t => t.AccountId == id).ToList());

            mock.Setup(m => m.GetByMerchant(It.IsAny<Guid>()))
                .Returns((Guid id) => transactions.Where(t => t.MerchantId == id).ToList());

            // Other mock setups

            return mock;
        }
    }
}
