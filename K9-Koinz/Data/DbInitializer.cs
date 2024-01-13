using K9_Koinz.Models;
using K9_Koinz.Factories;

namespace K9_Koinz.Data {
    public static class DbInitializer {

        // Accounts
        public static readonly Guid CREDIT_CART = Guid.Parse("41C3584E-38F6-49DE-B81B-C400B74A4DB0");
        public static readonly Guid CHECKING = Guid.Parse("F2D0F69D-3593-4DDA-A1C5-61155B430CE6");
        public static readonly Guid SAVINGS = Guid.Parse("A8E495B0-573F-4236-9621-303F35725F6A");
        public static readonly Guid SAVINGS_2 = Guid.Parse("B00DD7F2-4A3D-417E-A9EA-A686C6CCDEDE");
        public static readonly Guid HOME_MORTGAGE = Guid.Parse("198368d5-28c0-490c-ba66-e30dda6d8de8");

        // Top level categories
        public static readonly Guid AUTO_AND_TRANSPORT = Guid.Parse("a660d677-3dd7-4329-8c8e-6bf8d4f98d05");
        public static readonly Guid BILLS_AND_UTILITIES = Guid.Parse("2BCB67B0-B396-4EE9-B783-642D49A10896");
        public static readonly Guid ENTERTAINMENT = Guid.Parse("3BE359D5-C708-4DE7-8920-D5D4F1896082");
        public static readonly Guid FOOD_AND_DINING = Guid.Parse("D6A9E9B4-85DC-4025-A02A-258E00286757");
        public static readonly Guid HEALTH_AND_FITNESS = Guid.Parse("4C9C61CF-393B-4A93-A352-39A65B87E195");
        public static readonly Guid HOME = Guid.Parse("c8fb795c-387d-4c76-93fc-284c01c85021");
        public static readonly Guid INCOME = Guid.Parse("3a36de01-169c-444b-b285-21025030b040");
        public static readonly Guid INVESTMENTS = Guid.Parse("3ca1e23a-232e-483e-9b97-cab3c0254ec5");
        public static readonly Guid LOANS = Guid.Parse("3150278d-6cfa-410d-800f-edd0873f35af");
        public static readonly Guid PERSONAL_CARE = Guid.Parse("06e53ff2-d5ca-40e0-88df-9bc20636f419");
        public static readonly Guid PETS = Guid.Parse("6d1eb6db-7e58-4f19-9b52-02853c650ce1");
        public static readonly Guid SHOPPING = Guid.Parse("1fa0f522-ecec-40c9-91f0-1ddfd4ddda92");
        public static readonly Guid TRAVEL = Guid.Parse("29A42336-AFA9-4D9E-932B-9A92407B936A");

        // Child categories
        public static readonly Guid AUTO_INSURANCE = Guid.Parse("193cfd51-1c73-45cd-bf27-db5ef2ceb349");
        public static readonly Guid AUTO_PAYMENT = Guid.Parse("eff66a69-39e8-4004-a3c2-574393277a6c");
        public static readonly Guid GAS_AND_FUEL = Guid.Parse("3469c099-ca79-40fa-bd59-bde51afa9188");
        public static readonly Guid PARKING = Guid.Parse("ab87a89b-ce75-4dcb-a16c-6d9e83225bb6");
        public static readonly Guid PUBLIC_TRANSPORTATION = Guid.Parse("a2090a60-1c67-4edc-baa9-e7f9208e3da5");
        public static readonly Guid SERVICE_AND_PARTS = Guid.Parse("157094f6-856a-45b1-986f-f44e4c54b3a8");
        public static readonly Guid INTERNET = Guid.Parse("681e8b97-244f-42cb-bded-0ab327c97198");
        public static readonly Guid MOBILE_PHONE = Guid.Parse("c2b23d98-1426-421d-9a5f-8d654e84901f");
        public static readonly Guid TELEVISION = Guid.Parse("990cf6dd-c45b-4901-a2e2-5ce3d66b17a8");
        public static readonly Guid UTILITIES = Guid.Parse("4e3a1657-dbfe-468d-9ff3-16fe64c1f4bb");
        public static readonly Guid ELECTRIC_AND_GAS = Guid.Parse("cecf41a3-2a65-4526-a18e-e60e8840e1b6");
        public static readonly Guid SUBSCRIPTION = Guid.Parse("6e22c6a3-626d-4eda-aab7-79578ed0ad57");
        public static readonly Guid WATER_AND_SEWER = Guid.Parse("0bc6f546-6e81-4c5f-9dd6-10ef058bca69");
        public static readonly Guid MUSIC = Guid.Parse("5fffaf6d-25ff-4629-8643-25e9dc137946");
        public static readonly Guid GAMING = Guid.Parse("d48bb2ed-10d4-47d0-abc1-6fd727b33d9d");
        public static readonly Guid COFFEE_SHOPS = Guid.Parse("fe253b21-183c-4a59-a2b8-7431928f47dd");
        public static readonly Guid FOOD_DELIVERY = Guid.Parse("5d5df718-b6b2-4be7-b853-c1e5a288716f");
        public static readonly Guid GROCERIES = Guid.Parse("a77662b5-4561-4f66-afec-27e37dc509ab");
        public static readonly Guid RESTAURANTS = Guid.Parse("27577a21-9adc-46e1-9cf6-8ddf14043dfb");
        public static readonly Guid LONG_TERM_GROCERIES = Guid.Parse("1124f5c5-424c-4d0e-9e09-d46c8b381265");
        public static readonly Guid DOCTOR = Guid.Parse("ee5803a0-81bd-4575-a020-9c0697608eb3");
        public static readonly Guid EYECARE = Guid.Parse("b8eb68f2-dee1-437e-8dff-e9416c08a27a");
        public static readonly Guid PHARMACY = Guid.Parse("b8544d5a-350b-46a7-8927-e26f755cb293");
        public static readonly Guid MENTAL_HEALTH = Guid.Parse("70204ff6-6187-43e2-966a-d689c59c55e4");
        public static readonly Guid URGENT_CARE = Guid.Parse("efb6f81b-a5aa-467f-960f-550a7af1d885");
        public static readonly Guid MORTGAGE_AND_RENT = Guid.Parse("ecc0473c-7a33-4abf-9057-700f07eceea3");
        public static readonly Guid GIFT_MONEY = Guid.Parse("ce21d448-f71c-43cf-8e2d-dc0e7f8f2bc0");
        public static readonly Guid BONUS = Guid.Parse("443e34c7-19a1-446d-9859-49c517681113");
        public static readonly Guid PAYCHECK = Guid.Parse("d2efeed6-ef93-4b1e-9989-44dd67d779b8");
        public static readonly Guid RETURNED_PURCHASE = Guid.Parse("668c88f0-814a-4fe0-a8ce-6c6a9e45878a");
        public static readonly Guid LOAN_INTEREST = Guid.Parse("3edbc3ff-6357-4dc5-85e3-d4b8bef70322");
        public static readonly Guid LOAN_PRINCIPAL = Guid.Parse("4f93ff46-6f83-49d0-9076-b58db9953c27");
        public static readonly Guid LOAN_FEES_AND_CHARGES = Guid.Parse("1690d93c-4bc0-4566-98ba-50b600b92bb9");
        public static readonly Guid MONTHLY_PAYMENT = Guid.Parse("f4406cf9-bfff-4647-9545-4f2f60d38154");
        public static readonly Guid HAIR = Guid.Parse("48563ec8-27c3-4df7-941e-9508d774c2b5");
        public static readonly Guid LAUNDRY = Guid.Parse("b797864c-28b0-422a-bd1d-4a10a50b3d16");
        public static readonly Guid SPA_AND_MASSAGE = Guid.Parse("d4227b92-a270-47df-8a89-656fcac5d472");
        public static readonly Guid PET_FOOD_AND_SUPPLIES = Guid.Parse("5006e760-e04a-4777-9423-a32c7a63b578");
        public static readonly Guid PET_GROOMING = Guid.Parse("498689b1-27e1-47e7-9d0b-7e9fdb482026");
        public static readonly Guid VETERINARY = Guid.Parse("a3484da0-3d0e-4a1f-b35c-73c87ce50146");
        public static readonly Guid CLOTHING = Guid.Parse("14e4b03f-4cbc-446a-b442-a6ba9a3c0185");
        public static readonly Guid HOME_GOODS = Guid.Parse("f9967ed9-f43d-4c30-864b-10743e96aedc");
        public static readonly Guid ELECTRONICS_AND_SHOPPING = Guid.Parse("10a0786b-0dcb-4e0e-93a8-bdbb032ede9b");
        public static readonly Guid HOBBIES = Guid.Parse("7415750f-7ca1-428f-bfe6-5de04f0915a3");
        public static readonly Guid LODGING = Guid.Parse("4B8C6A35-43A9-468A-8BA7-158B1DFBAF08");
        public static readonly Guid AIRFARE = Guid.Parse("19835903-288E-4A4D-BCD7-EB3113003BC9");

        // Merchants
        public static readonly Guid PUBLIX = Guid.Parse("11402675-89f0-49b7-8dc4-5e7c74384d02");
        public static readonly Guid SPOTIFY = Guid.Parse("ede21958-a981-4255-b344-8f50cae17d7a");
        public static readonly Guid WALMART = Guid.Parse("2c5ba7e9-d20d-404e-ac32-c29dafa07fd3");
        public static readonly Guid AMAZON = Guid.Parse("d53eb941-a948-4c9d-b09a-4271903e6a04");
        public static readonly Guid OLD_CHICAGO = Guid.Parse("02b4c26d-675b-45c5-8b4f-a3003b641b72");
        public static readonly Guid UPS_STORE = Guid.Parse("294d6f42-ea5b-43e5-be89-7d2cf130ac38");
        public static readonly Guid DOMINION_ENERGY = Guid.Parse("8a44a06b-a816-487c-938e-d731b0bd940d");
        public static readonly Guid FOOD_LION = Guid.Parse("c9ea4bfe-5a08-4feb-be95-a6fff5078ebb");
        public static readonly Guid SYD_STYLIST = Guid.Parse("46a80251-0360-4cfd-a8e7-90b429b4b143");
        public static readonly Guid MICHAELS_STORE = Guid.Parse("4c0dd04a-c153-43ac-8ac9-edc21468cf31");
        public static readonly Guid STEAM_GAMES = Guid.Parse("40e837d0-91de-48f7-a353-b075099a8c83");
        public static readonly Guid DOLLAR_GENERAL = Guid.Parse("9f53db79-648e-4803-a22b-7cf42a6642a6");
        public static readonly Guid MR_COOPER = Guid.Parse("4f850fd3-1ec2-44b8-9428-d5eb835aefd5");
        public static readonly Guid SPECTRUM = Guid.Parse("19d93738-0b42-4dc8-a6a1-c243e6e60efc");
        public static readonly Guid CVS = Guid.Parse("e21688fb-3139-4911-8344-7e508c8a6ff1");
        public static readonly Guid CHICK_FIL_A = Guid.Parse("e3f9228c-644f-4c4d-b859-de78ce92807a");
        public static readonly Guid APPLE = Guid.Parse("6aaf0dae-7495-42da-ba9f-21cc8d141186");
        public static readonly Guid GSS_ASSOC = Guid.Parse("6461d5c4-4f1d-47b1-9e01-c34fd99e3414");
        public static readonly Guid ADDITIONAL_PRINCIPAL = Guid.Parse("116938c5-6bf3-4a8f-a1bf-221ead7910c0");
        public static readonly Guid CAPGEMINI_SALARY = Guid.Parse("ce755632-3399-4471-b1d0-1387342ec0fe");
        public static readonly Guid INTEREST_PAID = Guid.Parse("01e94766-c95b-418c-bc46-b28713861d49");
        public static readonly Guid WALGREENS = Guid.Parse("b809a53d-00b4-41cb-8a0f-4434f9b7a7fd");
        public static readonly Guid RUSHS = Guid.Parse("11cada92-9e40-428d-ba9e-de716d0aff0c");
        public static readonly Guid MJS_RESIDENT = Guid.Parse("df7a73c0-6f73-4f60-83f1-40e31514b4e0");
        public static readonly Guid PRISMA_HEALTH = Guid.Parse("afaa49cc-3e68-44a5-b95d-3b95595b08c8");
        public static readonly Guid CAR_WASH = Guid.Parse("a17bb627-23b8-4a8b-a2b2-0717eddb1ef7");
        public static readonly Guid ACORNS = Guid.Parse("328d3894-41f9-4155-ae4e-8e32426d53bc");
        public static readonly Guid MCDONALDS = Guid.Parse("8d51b96e-7d61-4f95-ba4d-650628a8ec2d");
        public static readonly Guid HIQUEST_PAYROLL = Guid.Parse("4e3895ce-c4b3-4272-88e7-c684ce37f16e");
        public static readonly Guid ZORBAS = Guid.Parse("fadb3b84-334c-4be1-874a-da811cc68954");
        public static readonly Guid PROGRESSIVE = Guid.Parse("50ffc950-b561-4354-a7d3-4b164a07d94a");

        // Budgets
        public static readonly Guid MAIN_BUDGET = Guid.Parse("88f66859-5ff8-4921-9ad2-0eee09c95cb8");
        public static readonly Guid WEEKLY_SHOPPING_BUDGET = Guid.Parse("3E9692BC-5AA1-4D42-A486-6824075B9A6E");
        public static readonly Guid CCFF_BUDGET = Guid.Parse("93d0c1e2-c42f-42aa-9d98-fc99745963c3");

        public static void Initialize(KoinzContext context) {
            return;

            if (context.Accounts.Any()) {
                return;
            }

            try {
                var accounts = CreateAccounts();
                context.Accounts.AddRange(accounts);
                context.SaveChanges();

                var merchants = CreateMerchants();
                context.Merchants.AddRange(merchants);
                context.SaveChanges();

                var categories = CreateCategories();
                context.Categories.AddRange(categories);
                context.SaveChanges();

                var childCategories = CreateChildCategories();
                context.Categories.AddRange(childCategories);
                context.SaveChanges();

                var transactions = CreateTransactions();
                context.Transactions.AddRange(transactions);
                context.SaveChanges();

                var budgets = CreateBudgets();
                context.Budgets.AddRange(budgets);
                context.SaveChanges();

                var budgetLines = CreateBudgetLines();
                context.BudgetLines.AddRange(budgetLines);
                context.SaveChanges();
            } catch (Exception) {
                context.Database.EnsureDeleted();
                throw;
            }
        }

        private static List<Account> CreateAccounts() {

            return [
                new Account { Id = CHECKING, Name = "Chase Checking", Description = "My primary checking account", Type = AccountType.CHECKING, InitialBalance = 4476.66d, InitialBalanceDate = DateTime.Parse("1/9/2024") },
                new Account { Id = SAVINGS, Name = "Chase Savings", Description = "I don't really use this account", Type = AccountType.SAVINGS, InitialBalance = 50d, InitialBalanceDate = DateTime.Parse("1/9/2024") },
                new Account { Id = SAVINGS_2, Name = "Axos Savings", Description = "A high yield savings account", Type = AccountType.SAVINGS, InitialBalance = 29458.51d, InitialBalanceDate = DateTime.Parse("1/9/2024") },
                new Account { Id = CREDIT_CART, Name = "Amazon Credit Card", Description = "My Amazon Prime credit card", Type = AccountType.CREDIT_CARD, InitialBalance = -816.98d, InitialBalanceDate = DateTime.Parse("1/9/2024") },
                new Account { Id = HOME_MORTGAGE, Name = "Home Mortgage", Description = "Mortgage loan with Mr. Cooper", Type = AccountType.LOAN, InitialBalance = -180122.54d, InitialBalanceDate = DateTime.Parse("1/9/2024") }
            ];
        }

        private static List<Merchant> CreateMerchants() {
            return [
                new Merchant { Id = PUBLIX, Name = "Publix" },
                new Merchant { Id = SPOTIFY, Name = "Paypal - Spotify USA" },
                new Merchant { Id = WALMART, Name = "Walmart" },
                new Merchant { Id = AMAZON, Name = "Amazon" },
                new Merchant { Id = OLD_CHICAGO, Name = "Old Chicago" },
                new Merchant { Id = UPS_STORE, Name = "The UPS Store" },
                new Merchant { Id = DOMINION_ENERGY, Name = "Dominion Energy" },
                new Merchant { Id = FOOD_LION, Name = "Food Lion" },
                new Merchant { Id = SYD_STYLIST, Name = "Syd Stylist" },
                new Merchant { Id = MICHAELS_STORE, Name = "Michaels Store" },
                new Merchant { Id = STEAM_GAMES, Name = "Paypal - Steam Games" },
                new Merchant { Id = DOLLAR_GENERAL, Name = "Dollar General" },
                new Merchant { Id = MR_COOPER, Name = "Mr Cooper" },
                new Merchant { Id = SPECTRUM, Name = "Spectrum" },
                new Merchant { Id = CVS, Name = "CVS Pharmacy" },
                new Merchant { Id = CHICK_FIL_A, Name = "Chick Fil A" },
                new Merchant { Id = APPLE, Name = "Apple.com" },
                new Merchant { Id = GSS_ASSOC, Name = "GSS Assoc" },
                new Merchant { Id = ADDITIONAL_PRINCIPAL, Name = "Additional Principal" },
                new Merchant { Id = CAPGEMINI_SALARY, Name = "Capgemini Reg Salary" },
                new Merchant { Id = INTEREST_PAID, Name = "Interest Paid" },
                new Merchant { Id = WALGREENS, Name = "Walgreens" },
                new Merchant { Id = RUSHS, Name = "Rush's" },
                new Merchant { Id = MJS_RESIDENT, Name = "MJS Resident" },
                new Merchant { Id = PRISMA_HEALTH, Name = "Prisma Health" },
                new Merchant { Id = CAR_WASH, Name = "Car Wash Club" },
                new Merchant { Id = ACORNS, Name = "Acorns Investment" },
                new Merchant { Id = MCDONALDS, Name = "McDonalds" },
                new Merchant { Id = HIQUEST_PAYROLL, Name = "HiQuest Payroll" },
                new Merchant { Id = ZORBAS, Name = "Zorbas" },
                new Merchant { Id = PROGRESSIVE, Name = "Progressive Ins. Premium" }
            ];
        }

        private static List<Category> CreateCategories() {
            return [
                new Category { Id = AUTO_AND_TRANSPORT, Name = "Auto & Transport", ParentCategoryId = null },
                new Category { Id = BILLS_AND_UTILITIES, Name = "Bills & Utilities", ParentCategoryId = null },
                new Category { Id = ENTERTAINMENT, Name = "Entertainment", ParentCategoryId = null },
                new Category { Id = FOOD_AND_DINING, Name = "Food & Dining", ParentCategoryId = null },
                new Category { Id = HEALTH_AND_FITNESS, Name = "Health & Fitness", ParentCategoryId = null },
                new Category { Id = HOME, Name = "Home", ParentCategoryId = null },
                new Category { Id = INCOME, Name = "Income", ParentCategoryId = null },
                new Category { Id = INVESTMENTS, Name = "Investments", ParentCategoryId = null },
                new Category { Id = LOANS, Name = "Loans", ParentCategoryId = null },
                new Category { Id = PERSONAL_CARE, Name = "Personal Care", ParentCategoryId = null },
                new Category { Id = PETS, Name = "Pets", ParentCategoryId = null },
                new Category { Id = SHOPPING, Name = "Shopping", ParentCategoryId = null },
                new Category { Id = TRAVEL, Name = "Travel", ParentCategoryId = null }
            ];
        }

        private static List<Category> CreateChildCategories() {
            return [
                // Auto and Transport
                new Category { Id = AUTO_INSURANCE, Name = "Auto Insurance", ParentCategoryId = AUTO_AND_TRANSPORT },
                new Category { Id = AUTO_PAYMENT, Name = "Auto Payment", ParentCategoryId = AUTO_AND_TRANSPORT },
                new Category { Id = GAS_AND_FUEL, Name = "Gas & Fuel", ParentCategoryId = AUTO_AND_TRANSPORT },
                new Category { Id = PARKING, Name = "Parking", ParentCategoryId = AUTO_AND_TRANSPORT },
                new Category { Id = PUBLIC_TRANSPORTATION, Name = "Public Transportation", ParentCategoryId = AUTO_AND_TRANSPORT },
                new Category { Id = SERVICE_AND_PARTS, Name = "Service & Parts", ParentCategoryId = AUTO_AND_TRANSPORT },

                // Bills and Utilities
                new Category { Id = INTERNET, Name = "Internet", ParentCategoryId = BILLS_AND_UTILITIES },
                new Category { Id = MOBILE_PHONE, Name = "Mobile Phone", ParentCategoryId = BILLS_AND_UTILITIES },
                new Category { Id = TELEVISION, Name = "Television", ParentCategoryId = BILLS_AND_UTILITIES },
                new Category { Id = UTILITIES, Name = "Utilities", ParentCategoryId = BILLS_AND_UTILITIES },
                new Category { Id = ELECTRIC_AND_GAS, Name = "Electric & Gas", ParentCategoryId = BILLS_AND_UTILITIES },
                new Category { Id = SUBSCRIPTION, Name = "Subscription", ParentCategoryId = BILLS_AND_UTILITIES },
                new Category { Id = WATER_AND_SEWER, Name = "Water & Sewer", ParentCategoryId = BILLS_AND_UTILITIES },

                // Entertainment
                new Category { Id = MUSIC, Name = "Music", ParentCategoryId = ENTERTAINMENT },
                new Category { Id = GAMING, Name = "Gaming", ParentCategoryId = ENTERTAINMENT },

                // Food and Dining
                new Category { Id = COFFEE_SHOPS, Name = "Coffee Shops", ParentCategoryId = FOOD_AND_DINING },
                new Category { Id = FOOD_DELIVERY, Name = "Food Delivery", ParentCategoryId = FOOD_AND_DINING },
                new Category { Id = GROCERIES, Name = "Groceries", ParentCategoryId = FOOD_AND_DINING },
                new Category { Id = RESTAURANTS, Name = "Restaurants", ParentCategoryId = FOOD_AND_DINING },
                new Category { Id = LONG_TERM_GROCERIES, Name = "Long Term Groceries", ParentCategoryId = FOOD_AND_DINING },

                // Health and Fitness
                new Category { Id = DOCTOR, Name = "Doctor", ParentCategoryId = HEALTH_AND_FITNESS },
                new Category { Id = EYECARE, Name = "Eyecare", ParentCategoryId = HEALTH_AND_FITNESS },
                new Category { Id = PHARMACY, Name = "Pharmacy", ParentCategoryId = HEALTH_AND_FITNESS },
                new Category { Id = MENTAL_HEALTH, Name = "Mental Health", ParentCategoryId = HEALTH_AND_FITNESS },
                new Category { Id = URGENT_CARE, Name = "Urgent Care", ParentCategoryId = HEALTH_AND_FITNESS },

                // Home
                new Category { Id = MORTGAGE_AND_RENT, Name = "Mortgage & Rent", ParentCategoryId = HOME },

                // Income
                new Category { Id = GIFT_MONEY, Name = "Gift Money", ParentCategoryId = INCOME },
                new Category { Id = BONUS, Name = "Bonus", ParentCategoryId = INCOME },
                new Category { Id = PAYCHECK, Name = "Paycheck", ParentCategoryId = INCOME },
                new Category { Id = RETURNED_PURCHASE, Name = "Returned Purchase", ParentCategoryId = INCOME },

                // Loans
                new Category { Id = LOAN_INTEREST, Name = "Loan Interest", ParentCategoryId = LOANS },
                new Category { Id = LOAN_PRINCIPAL, Name = "Loan Principal", ParentCategoryId = LOANS },
                new Category { Id = LOAN_FEES_AND_CHARGES, Name = "Loan Fees and Chages", ParentCategoryId = LOANS },
                new Category { Id = MONTHLY_PAYMENT, Name = "Monthly Paymenty", ParentCategoryId = LOANS },
                new Category { Id = ADDITIONAL_PRINCIPAL, Name = "Additional Principal", ParentCategoryId = LOANS },

                // Personal Care
                new Category { Id = HAIR, Name = "Hair", ParentCategoryId = PERSONAL_CARE },
                new Category { Id = LAUNDRY, Name = "Laundry", ParentCategoryId = PERSONAL_CARE },
                new Category { Id = SPA_AND_MASSAGE, Name = "Spa & Massage", ParentCategoryId = PERSONAL_CARE },

                // Pets
                new Category { Id = PET_FOOD_AND_SUPPLIES, Name = "Pet Food & Supplies", ParentCategoryId = PETS },
                new Category { Id = PET_GROOMING, Name = "Pet Grooming", ParentCategoryId = PETS },
                new Category { Id = VETERINARY, Name = "Veterinary", ParentCategoryId = PETS },

                // Shopping
                new Category { Id = CLOTHING, Name = "Clothing", ParentCategoryId = SHOPPING },
                new Category { Id = HOME_GOODS, Name = "Home Goods", ParentCategoryId = SHOPPING },
                new Category { Id = ELECTRONICS_AND_SHOPPING, Name = "Electronics & Shopping", ParentCategoryId = SHOPPING },
                new Category { Id = HOBBIES, Name = "Hobbies", ParentCategoryId = SHOPPING },

                // Travel
                new Category { Id = LODGING, Name = "Lodging", ParentCategoryId = TRAVEL },
                new Category { Id = AIRFARE, Name = "Airlines", ParentCategoryId = TRAVEL }
            ];
        }

        private static List<Transaction> CreateTransactions() {
            return [
                new Transaction { Id = Guid.NewGuid(), Date = DateTime.Parse("1/7/2024"), AccountId = CREDIT_CART, MerchantId = PUBLIX, CategoryId = GROCERIES, Amount = -27.27d },
                new Transaction { Id = Guid.NewGuid(), Date = DateTime.Parse("1/6/2024"), AccountId = CREDIT_CART, MerchantId = SPOTIFY, CategoryId = MUSIC, Amount = -16.04d },
                new Transaction { Id = Guid.NewGuid(), Date = DateTime.Parse("1/5/2024"), AccountId = CHECKING, MerchantId = DOMINION_ENERGY, CategoryId = UTILITIES, Amount = -118d },
                new Transaction { Id = Guid.NewGuid(), Date = DateTime.Parse("1/5/2024"), AccountId = CHECKING, MerchantId = ACORNS, CategoryId = INVESTMENTS, Amount = -6.8d },
                new Transaction { Id = Guid.NewGuid(), Date = DateTime.Parse("1/5/2024"), AccountId = CREDIT_CART, MerchantId = WALMART, CategoryId = GROCERIES, Amount = -44.14d },
                new Transaction { Id = Guid.NewGuid(), Date = DateTime.Parse("1/5/2024"), AccountId = CREDIT_CART, MerchantId = FOOD_LION, CategoryId = GROCERIES, Amount = -2.19d },
                new Transaction { Id = Guid.NewGuid(), Date = DateTime.Parse("1/5/2024"), AccountId = CREDIT_CART, MerchantId = AMAZON, CategoryId = SHOPPING, Amount = -16.16d },
                new Transaction { Id = Guid.NewGuid(), Date = DateTime.Parse("1/5/2024"), AccountId = CREDIT_CART, MerchantId = OLD_CHICAGO, CategoryId = RESTAURANTS, Amount = -36.27d },
                new Transaction { Id = Guid.NewGuid(), Date = DateTime.Parse("1/5/2024"), AccountId = CREDIT_CART, MerchantId = SYD_STYLIST, CategoryId = HAIR, Amount = -37.5d },
                new Transaction { Id = Guid.NewGuid(), Date = DateTime.Parse("1/5/2024"), AccountId = CREDIT_CART, MerchantId = MICHAELS_STORE, CategoryId = SHOPPING, Amount = -18.35d },
                new Transaction { Id = Guid.NewGuid(), Date = DateTime.Parse("1/5/2024"), AccountId = CREDIT_CART, MerchantId = UPS_STORE, CategoryId = SHOPPING, Amount = -39.56d },
                new Transaction { Id = Guid.NewGuid(), Date = DateTime.Parse("1/5/2024"), AccountId = CREDIT_CART, MerchantId = MCDONALDS, CategoryId = RESTAURANTS, Amount = -11.84d },
                new Transaction { Id = Guid.NewGuid(), Date = DateTime.Parse("1/4/2024"), AccountId = CREDIT_CART, MerchantId = STEAM_GAMES, CategoryId = GAMING, Amount = -9.99d },
                new Transaction { Id = Guid.NewGuid(), Date = DateTime.Parse("1/4/2024"), AccountId = CHECKING, MerchantId = ACORNS, CategoryId = INVESTMENTS, Amount = -5.56d },
                new Transaction { Id = Guid.NewGuid(), Date = DateTime.Parse("1/4/2024"), AccountId = CHECKING, MerchantId = HIQUEST_PAYROLL, CategoryId = PAYCHECK, Amount = 51.94d },
                new Transaction { Id = Guid.NewGuid(), Date = DateTime.Parse("1/4/2024"), AccountId = CREDIT_CART, MerchantId = ZORBAS, CategoryId = RESTAURANTS, Amount = -18.45d },
                new Transaction { Id = Guid.NewGuid(), Date = DateTime.Parse("1/3/2024"), AccountId = CHECKING, MerchantId = MR_COOPER, CategoryId = MORTGAGE_AND_RENT, Amount = -1669.01d },
                new Transaction { Id = Guid.NewGuid(), Date = DateTime.Parse("1/3/2024"), AccountId = CHECKING, MerchantId = ACORNS, CategoryId = INVESTMENTS, Amount = -5d },
                new Transaction { Id = Guid.NewGuid(), Date = DateTime.Parse("1/3/2024"), AccountId = CREDIT_CART, MerchantId = DOLLAR_GENERAL, CategoryId = SHOPPING, Amount = -8.8d },
                new Transaction { Id = Guid.NewGuid(), Date = DateTime.Parse("1/3/2024"), AccountId = CREDIT_CART, MerchantId = GSS_ASSOC, CategoryId = MENTAL_HEALTH, Amount = -90d },
                new Transaction { Id = Guid.NewGuid(), Date = DateTime.Parse("1/2/2024"), AccountId = CHECKING, MerchantId = PROGRESSIVE, CategoryId = AUTO_INSURANCE, Amount = -121.17d },
                new Transaction { Id = Guid.NewGuid(), Date = DateTime.Parse("1/2/2024"), AccountId = CHECKING, MerchantId = SPECTRUM, CategoryId = INTERNET, Amount = -111.72d },
                new Transaction { Id = Guid.NewGuid(), Date = DateTime.Parse("1/2/2024"), AccountId = CREDIT_CART, MerchantId = AMAZON, CategoryId = SHOPPING, Amount = -16.16d },
                new Transaction { Id = Guid.NewGuid(), Date = DateTime.Parse("1/2/2024"), AccountId = HOME_MORTGAGE, MerchantId = MR_COOPER, CategoryId = ADDITIONAL_PRINCIPAL, Amount = 200d },
                new Transaction { Id = Guid.NewGuid(), Date = DateTime.Parse("1/2/2024"), AccountId = HOME_MORTGAGE, MerchantId = MR_COOPER, CategoryId = MONTHLY_PAYMENT, Amount = 1469.01d },
                new Transaction { Id = Guid.NewGuid(), Date = DateTime.Parse("1/1/2024"), AccountId = CREDIT_CART, MerchantId = CVS, CategoryId = PHARMACY, Amount = -12d },
                new Transaction { Id = Guid.NewGuid(), Date = DateTime.Parse("12/31/2023"), AccountId = CREDIT_CART, MerchantId = CHICK_FIL_A, CategoryId = RESTAURANTS, Amount = -22.98d },
                new Transaction { Id = Guid.NewGuid(), Date = DateTime.Parse("12/31/2023"), AccountId = CREDIT_CART, MerchantId = WALMART, CategoryId = GROCERIES, Amount = -90.71d },
                new Transaction { Id = Guid.NewGuid(), Date = DateTime.Parse("12/29/2023"), AccountId = CHECKING, MerchantId = CAPGEMINI_SALARY, CategoryId = PAYCHECK, Amount = 2837.29d }
            ];
        }

        private static List<Budget> CreateBudgets() {
            return [
                new Budget { Id = MAIN_BUDGET, Name = "Main Budget", Description = "This is a test of the budget table", SortOrder = 1, Timespan = BudgetTimeSpan.MONTHLY },
                new Budget { Id = WEEKLY_SHOPPING_BUDGET, Name = "Weekly Shopping", Description = "Weekly version of the main budget with more focused categories", SortOrder = 2, Timespan = BudgetTimeSpan.WEEKLY },
                new Budget { Id = CCFF_BUDGET, Name = "CCFF Budget", Description = "An example secondary budget", SortOrder = 3, Timespan = BudgetTimeSpan.MONTHLY }
            ];
        }

        private static List<BudgetLine> CreateBudgetLines() {
            return [
                // Main budget
                BudgetLineFactory.NewIncomeLine(MAIN_BUDGET, INCOME, 5674),
                BudgetLineFactory.NewExpenseLine(MAIN_BUDGET, AUTO_INSURANCE, 124),
                BudgetLineFactory.NewExpenseLine(MAIN_BUDGET, AUTO_PAYMENT, 246),
                BudgetLineFactory.NewExpenseLine(MAIN_BUDGET, GAS_AND_FUEL, 150),
                BudgetLineFactory.NewExpenseLine(MAIN_BUDGET, SERVICE_AND_PARTS, 45),
                BudgetLineFactory.NewExpenseLine(MAIN_BUDGET, BILLS_AND_UTILITIES, 625),
                BudgetLineFactory.NewExpenseLine(MAIN_BUDGET, ELECTRIC_AND_GAS, 118),
                BudgetLineFactory.NewExpenseLine(MAIN_BUDGET, MOBILE_PHONE, 117),
                BudgetLineFactory.NewExpenseLine(MAIN_BUDGET, TELEVISION, 48),
                BudgetLineFactory.NewExpenseLine(MAIN_BUDGET, WATER_AND_SEWER, 76),
                BudgetLineFactory.NewExpenseLine(MAIN_BUDGET, ENTERTAINMENT, 40),
                BudgetLineFactory.NewExpenseLine(MAIN_BUDGET, FOOD_DELIVERY, 40),
                BudgetLineFactory.NewExpenseLine(MAIN_BUDGET, GROCERIES, 500),
                BudgetLineFactory.NewExpenseLine(MAIN_BUDGET, RESTAURANTS, 150),
                BudgetLineFactory.NewExpenseLine(MAIN_BUDGET, MENTAL_HEALTH, 200),
                BudgetLineFactory.NewExpenseLine(MAIN_BUDGET, PHARMACY, 30),
                BudgetLineFactory.NewExpenseLine(MAIN_BUDGET, MORTGAGE_AND_RENT, 1669),
                BudgetLineFactory.NewExpenseLine(MAIN_BUDGET, PET_FOOD_AND_SUPPLIES, 100),
                BudgetLineFactory.NewExpenseLine(MAIN_BUDGET, SHOPPING, 450),

                // Weekly shopping budget
                BudgetLineFactory.NewIncomeLine(WEEKLY_SHOPPING_BUDGET, INCOME, (5674 * 12) / 52),
                BudgetLineFactory.NewExpenseLine(WEEKLY_SHOPPING_BUDGET, GROCERIES, (500 * 12) / 52),
                BudgetLineFactory.NewExpenseLine(WEEKLY_SHOPPING_BUDGET, SHOPPING, (450 * 12) / 52),
                BudgetLineFactory.NewExpenseLine(WEEKLY_SHOPPING_BUDGET, RESTAURANTS, (150 * 12) / 52),

                // CCFF budget
                BudgetLineFactory.NewIncomeLine(CCFF_BUDGET, INCOME, 5674),
                BudgetLineFactory.NewIncomeLine(CCFF_BUDGET, GIFT_MONEY, 100),
                BudgetLineFactory.NewExpenseLine(CCFF_BUDGET, GROCERIES, 500),
                BudgetLineFactory.NewExpenseLine(CCFF_BUDGET, LODGING, 1200)
            ];
        }
    }
}
