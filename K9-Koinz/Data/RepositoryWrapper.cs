using K9_Koinz.Models.Meta;

namespace K9_Koinz.Data {
    public interface IRepositoryWrapper {
        AccountRepository AccountRepository { get; }
        BillRepository BillRepository { get; }
        BudgetLineRepository BudgetLineRepository { get; }
        BudgetLinePeriodRepository BudgetLinePeriodRepository { get; }
        BudgetRepository BudgetRepository { get; }
        CategoryRepository CategoryRepository { get; }
        JobStatusRepository JobStatusRepository { get; }
        MerchantRepository MerchantRepository { get; }
        SavingsGoalRepository SavingsGoalRepository { get; }
        TagRepository TagRepository { get; }
        TransactionRepository TransactionRepository { get; }
        TransferRepository TransferRepository { get; }

        void Save();
        Task SaveAsync();

        GenericRepository<TEntity> GetGenericRepository<TEntity>() where TEntity : BaseEntity;
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
        public virtual AccountRepository AccountRepository {
            get {
                accountRepository ??= new AccountRepository(_context);
                return accountRepository;
            }
        }

        public virtual TransactionRepository TransactionRepository {
            get {
                transactionRepository ??= new TransactionRepository(_context);
                return transactionRepository;
            }
        }

        public virtual BillRepository BillRepository {
            get {
                billRepository ??= new BillRepository(_context);
                return billRepository;
            }
        }

        public virtual BudgetLineRepository BudgetLineRepository {
            get {
                budgetLineRepository ??= new BudgetLineRepository(_context);
                return budgetLineRepository;
            }
        }

        public virtual BudgetRepository BudgetRepository {
            get {
                budgetRepository ??= new BudgetRepository(_context);
                return budgetRepository;
            }
        }

        public virtual CategoryRepository CategoryRepository {
            get {
                categoryRepository ??= new CategoryRepository(_context);
                return categoryRepository;
            }
        }

        public virtual JobStatusRepository JobStatusRepository {
            get {
                jobStatusRepository ??= new JobStatusRepository(_context);
                return jobStatusRepository;
            }
        }

        public virtual MerchantRepository MerchantRepository {
            get {
                merchantRepository ??= new MerchantRepository(_context);
                return merchantRepository;
            }
        }

        public virtual SavingsGoalRepository SavingsGoalRepository {
            get {
                savingsGoalRepository ??= new SavingsGoalRepository(_context);
                return savingsGoalRepository;
            }
        }

        public virtual TagRepository TagRepository {
            get {
                tagRepository ??= new TagRepository(_context);
                return tagRepository;
            }
        }

        public virtual TransferRepository TransferRepository {
            get {
                transferRepository ??= new TransferRepository(_context);
                return transferRepository;
            }
        }
        public virtual BudgetLinePeriodRepository BudgetLinePeriodRepository {
            get {
                budgetLinePeriodRepository ??= new BudgetLinePeriodRepository(_context);
                return budgetLinePeriodRepository;
            }
        }

        public virtual GenericRepository<TEntity> GetGenericRepository<TEntity>() where TEntity : BaseEntity {
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
