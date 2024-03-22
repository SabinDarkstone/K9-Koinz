using K9_Koinz.Models.Meta;

namespace K9_Koinz.Data {
    public interface IRepositoryWrapper {
        IAccountRepository Accounts { get; }
        IBillRepository Bills { get; }
        IBudgetLineRepository BudgetLines { get; }
        IBudgetLinePeriodRepository BudgetLinePeriods { get; }
        IBudgetRepository Budgets { get; }
        ICategoryRepository Categories { get; }
        IJobStatusRepository JobStatuses { get; }
        IMerchantRepository Merchants { get; }
        ISavingsGoalRepository SavingsGoals { get; }
        ITagRepository Tags { get; }
        ITransactionRepository Transactions { get; }
        ITransferRepository Transfers { get; }

        void Save();
        Task SaveAsync();

        IGenericRepository<TEntity> GetGenericRepository<TEntity>() where TEntity : BaseEntity;
    }

    public class RepositoryWrapper : IRepositoryWrapper {
        private readonly KoinzContext _context;

        private AccountRepository accountRepository;
        private BillRepository billRepository;
        private BudgetLineRepository budgetLineRepository;
        private BudgetLinePeriodRepository budgetLinePeriodRepository;
        private BudgetRepository budgetRepository;
        private CategoryRepository categoryRepository;
        private JobStatusRepository jobStatusRepository;
        private MerchantRepository merchantRepository;
        private SavingsGoalRepository savingsGoalRepository;
        private TagRepository tagRepository;
        private TransactionRepository transactionRepository;
        private TransferRepository transferRepository;

        public RepositoryWrapper(KoinzContext context) {
            _context = context;
        }
        public virtual IAccountRepository Accounts {
            get {
                accountRepository ??= new AccountRepository(_context);
                return accountRepository;
            }
        }

        public virtual ITransactionRepository Transactions {
            get {
                transactionRepository ??= new TransactionRepository(_context);
                return transactionRepository;
            }
        }

        public virtual IBillRepository Bills {
            get {
                billRepository ??= new BillRepository(_context);
                return billRepository;
            }
        }

        public virtual IBudgetLineRepository BudgetLines {
            get {
                budgetLineRepository ??= new BudgetLineRepository(_context);
                return budgetLineRepository;
            }
        }

        public virtual IBudgetRepository Budgets {
            get {
                budgetRepository ??= new BudgetRepository(_context);
                return budgetRepository;
            }
        }

        public virtual ICategoryRepository Categories {
            get {
                categoryRepository ??= new CategoryRepository(_context);
                return categoryRepository;
            }
        }

        public virtual IJobStatusRepository JobStatuses {
            get {
                jobStatusRepository ??= new JobStatusRepository(_context);
                return jobStatusRepository;
            }
        }

        public virtual IMerchantRepository Merchants {
            get {
                merchantRepository ??= new MerchantRepository(_context);
                return merchantRepository;
            }
        }

        public virtual ISavingsGoalRepository SavingsGoals {
            get {
                savingsGoalRepository ??= new SavingsGoalRepository(_context);
                return savingsGoalRepository;
            }
        }

        public virtual ITagRepository Tags {
            get {
                tagRepository ??= new TagRepository(_context);
                return tagRepository;
            }
        }

        public virtual ITransferRepository Transfers {
            get {
                transferRepository ??= new TransferRepository(_context);
                return transferRepository;
            }
        }
        public virtual IBudgetLinePeriodRepository BudgetLinePeriods {
            get {
                budgetLinePeriodRepository ??= new BudgetLinePeriodRepository(_context);
                return budgetLinePeriodRepository;
            }
        }

        public virtual IGenericRepository<TEntity> GetGenericRepository<TEntity>() where TEntity : BaseEntity {
            return new GenericRepository<TEntity>(_context);
        }

        public virtual void Save() {
            _context.SaveChanges();
        }

        public virtual async Task SaveAsync() {
            await _context.SaveChangesAsync();
        }
    }
}
