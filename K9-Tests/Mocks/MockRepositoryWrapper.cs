using K9_Koinz.Data;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace K9_Tests.Mocks {
    internal class MockRepositoryWrapper {
        public static Mock<IRepositoryWrapper> GetMock() {
            var mock = new Mock<IRepositoryWrapper>();

            var transactionRepoMock = MockITransactionRepository.GetMock();

            // Setup mock
            mock.Setup(m => m.Transactions).Returns(() => transactionRepoMock.Object);
            mock.Setup(m => m.Merchants).Returns(() => new Mock<IMerchantRepository>().Object);
            mock.Setup(m => m.Transfers).Returns(() => new Mock<ITransferRepository>().Object);
            mock.Setup(m => m.BudgetLines).Returns(() => new Mock<IBudgetLineRepository>().Object);
            mock.Setup(m => m.Accounts).Returns(() => new Mock<IAccountRepository>().Object);
            mock.Setup(m => m.Bills).Returns(() => new Mock<IBillRepository>().Object);
            mock.Setup(m => m.BudgetLinePeriods).Returns(() => new Mock<BudgetLinePeriodRepository>().Object);
            mock.Setup(m => m.Budgets).Returns(() => new Mock<IBudgetRepository>().Object);
            mock.Setup(m => m.Categories).Returns(() => new Mock<ICategoryRepository>().Object);
            mock.Setup(m => m.JobStatuses).Returns(() => new Mock<IJobStatusRepository>().Object);
            mock.Setup(m => m.SavingsGoals).Returns(() => new Mock<ISavingsGoalRepository>().Object);

            mock.Setup(m => m.Save()).Callback(() => { return; });
            mock.Setup(m => m.SaveAsync()).Callback(() => { return; });

            return mock;
        }
    }
}
