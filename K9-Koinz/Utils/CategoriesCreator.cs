using K9_Koinz.Models;
using System.Collections.Generic;

namespace K9_Koinz.Utils {
    public static class CategoriesCreator {

        public static List<Category> MakeTopLevelCategories() {
            List<Category> categories = new List<Category> {
                new Category { Id = Guid.NewGuid(), Name = "Auto & Transport", ParentCategoryId = null },
                new Category { Id = Guid.NewGuid(), Name = "Bills & Utilities", ParentCategoryId = null },
                new Category { Id = Guid.NewGuid(), Name = "Business Services", ParentCategoryId = null },
                new Category { Id = Guid.NewGuid(), Name = "Education", ParentCategoryId = null },
                new Category { Id = Guid.NewGuid(), Name = "Entertainment", ParentCategoryId = null },
                new Category { Id = Guid.NewGuid(), Name = "Fees & Charges", ParentCategoryId = null },
                new Category { Id = Guid.NewGuid(), Name = "Financial", ParentCategoryId = null },
                new Category { Id = Guid.NewGuid(), Name = "Food & Dining", ParentCategoryId = null },
                new Category { Id = Guid.NewGuid(), Name = "Gifts & Donations", ParentCategoryId = null },
                new Category { Id = Guid.NewGuid(), Name = "Health & Fitness", ParentCategoryId = null },
                new Category { Id = Guid.NewGuid(), Name = "Hide from Budgets & Trends", ParentCategoryId = null },
                new Category { Id = Guid.NewGuid(), Name = "Home", ParentCategoryId = null },
                new Category { Id = Guid.NewGuid(), Name = "Income", ParentCategoryId = null },
                new Category { Id = Guid.NewGuid(), Name = "Investments", ParentCategoryId = null },
                new Category { Id = Guid.NewGuid(), Name = "Kids", ParentCategoryId = null },
                new Category { Id = Guid.NewGuid(), Name = "Loans", ParentCategoryId = null },
                new Category { Id = Guid.NewGuid(), Name = "Misc Expenses", ParentCategoryId = null },
                new Category { Id = Guid.NewGuid(), Name = "Personal Care", ParentCategoryId = null },
                new Category { Id = Guid.NewGuid(), Name = "Pets", ParentCategoryId = null },
                new Category { Id = Guid.NewGuid(), Name = "Shopping", ParentCategoryId = null },
                new Category { Id = Guid.NewGuid(), Name = "Taxes", ParentCategoryId = null },
                new Category { Id = Guid.NewGuid(), Name = "Transfer", ParentCategoryId = null },
                new Category { Id = Guid.NewGuid(), Name = "Travel", ParentCategoryId = null },
                new Category { Id = Guid.NewGuid(), Name = "Uncategorized", ParentCategoryId = null }
            };

            return categories;
        }

        public static List<Category> MakeChildCategories(Dictionary<string, Guid> topLevelCategories) {
            List<Category> categories = new List<Category>();

            var autoAndTransport = topLevelCategories["Auto & Transport"];
            categories.AddRange([
                new Category { Id = Guid.NewGuid(), Name = "Auto Insurance", ParentCategoryId = autoAndTransport },
                new Category { Id = Guid.NewGuid(), Name = "Auto Payment", ParentCategoryId = autoAndTransport },
                new Category { Id = Guid.NewGuid(), Name = "Gas & Fuel", ParentCategoryId = autoAndTransport },
                new Category { Id = Guid.NewGuid(), Name = "Parking", ParentCategoryId = autoAndTransport },
                new Category { Id = Guid.NewGuid(), Name = "Ride Share", ParentCategoryId = autoAndTransport },
                new Category { Id = Guid.NewGuid(), Name = "Service & Parts", ParentCategoryId = autoAndTransport }
            ]);

            var billsAndUtilities = topLevelCategories["Bills & Utilities"];
            categories.AddRange([
                new Category { Id = Guid.NewGuid(), Name = "Home Phone", ParentCategoryId = billsAndUtilities },
                new Category { Id = Guid.NewGuid(), Name = "Internet", ParentCategoryId = billsAndUtilities },
                new Category { Id = Guid.NewGuid(), Name = "Mobile Phone", ParentCategoryId = billsAndUtilities },
                new Category { Id = Guid.NewGuid(), Name = "Television", ParentCategoryId = billsAndUtilities },
                new Category { Id = Guid.NewGuid(), Name = "Utilities", ParentCategoryId = billsAndUtilities },
                new Category { Id = Guid.NewGuid(), Name = "Electric and Gas", ParentCategoryId = billsAndUtilities },
                new Category { Id = Guid.NewGuid(), Name = "Subscription", ParentCategoryId = billsAndUtilities },
                new Category { Id = Guid.NewGuid(), Name = "Water and Sewer", ParentCategoryId = billsAndUtilities }
            ]);

            var businessServices = topLevelCategories["Business Services"];
            categories.AddRange([
                new Category { Id = Guid.NewGuid(), Name = "Advertising", ParentCategoryId = businessServices },
                new Category { Id = Guid.NewGuid(), Name = "Legal", ParentCategoryId = businessServices },
                new Category { Id = Guid.NewGuid(), Name = "Office Supplies", ParentCategoryId = businessServices },
                new Category { Id = Guid.NewGuid(), Name = "Printing", ParentCategoryId = businessServices },
                new Category { Id = Guid.NewGuid(), Name = "Shipping", ParentCategoryId = businessServices }
            ]);

            var education = topLevelCategories["Education"];
            categories.AddRange([
                new Category { Id = Guid.NewGuid(), Name = "Books & Supplies", ParentCategoryId = education },
                new Category { Id = Guid.NewGuid(), Name = "Student Loans", ParentCategoryId = education },
                new Category { Id = Guid.NewGuid(), Name = "Tuition", ParentCategoryId = education }
            ]);

            var entertainment = topLevelCategories["Entertainment"];
            categories.AddRange([
                new Category { Id = Guid.NewGuid(), Name = "Amusement", ParentCategoryId = entertainment },
                new Category { Id = Guid.NewGuid(), Name = "Arts", ParentCategoryId = entertainment },
                new Category { Id = Guid.NewGuid(), Name = "Movies & DVDs", ParentCategoryId = entertainment },
                new Category { Id = Guid.NewGuid(), Name = "Music", ParentCategoryId = entertainment },
                new Category { Id = Guid.NewGuid(), Name = "Newspaper & Magazines", ParentCategoryId = entertainment },
                new Category { Id = Guid.NewGuid(), Name = "Gaming", ParentCategoryId = entertainment }
            ]);

            var feesAndCharges = topLevelCategories["Fees & Charges"];
            categories.AddRange([
                new Category { Id = Guid.NewGuid(), Name = "ATM Fee", ParentCategoryId = feesAndCharges },
                new Category { Id = Guid.NewGuid(), Name = "Bank Fee", ParentCategoryId = feesAndCharges },
                new Category { Id = Guid.NewGuid(), Name = "Finance Charge", ParentCategoryId = feesAndCharges },
                new Category { Id = Guid.NewGuid(), Name = "Late Fee", ParentCategoryId = feesAndCharges },
                new Category { Id = Guid.NewGuid(), Name = "Service Fee", ParentCategoryId = feesAndCharges },
                new Category { Id = Guid.NewGuid(), Name = "Trade Commissions", ParentCategoryId = feesAndCharges }
            ]);

            var financial = topLevelCategories["Financial"];
            categories.AddRange([
                new Category { Id = Guid.NewGuid(), Name = "Financial Advisor", ParentCategoryId = financial },
                new Category { Id = Guid.NewGuid(), Name = "Life Insurance", ParentCategoryId = financial }
            ]);

            var foodAndDining = topLevelCategories["Food & Dining"];
            categories.AddRange([
                new Category { Id = Guid.NewGuid(), Name = "Alcohol & Bars", ParentCategoryId = foodAndDining },
                new Category { Id = Guid.NewGuid(), Name = "Coffee Shops", ParentCategoryId = foodAndDining },
                new Category { Id = Guid.NewGuid(), Name = "Fast Food", ParentCategoryId = foodAndDining },
                new Category { Id = Guid.NewGuid(), Name = "Food Delivery", ParentCategoryId = foodAndDining },
                new Category { Id = Guid.NewGuid(), Name = "Groceries", ParentCategoryId = foodAndDining },
                new Category { Id = Guid.NewGuid(), Name = "Long Term Groceries", ParentCategoryId = foodAndDining },
                new Category { Id = Guid.NewGuid(), Name = "Restaurants", ParentCategoryId = foodAndDining }
            ]);

            var giftsAndDonations = topLevelCategories["Gifts & Donations"];
            categories.AddRange([
                new Category { Id = Guid.NewGuid(), Name = "Charity", ParentCategoryId = giftsAndDonations },
                new Category { Id = Guid.NewGuid(), Name = "Gift", ParentCategoryId = giftsAndDonations }
            ]);

            var healthAndFitness = topLevelCategories["Health & Fitness"];
            categories.AddRange([
                new Category { Id = Guid.NewGuid(), Name = "Dentist", ParentCategoryId = healthAndFitness },
                new Category { Id = Guid.NewGuid(), Name = "Doctor", ParentCategoryId = healthAndFitness },
                new Category { Id = Guid.NewGuid(), Name = "Eyecare", ParentCategoryId = healthAndFitness },
                new Category { Id = Guid.NewGuid(), Name = "Gym", ParentCategoryId = healthAndFitness },
                new Category { Id = Guid.NewGuid(), Name = "Health Insurance", ParentCategoryId = healthAndFitness },
                new Category { Id = Guid.NewGuid(), Name = "Pharmacy", ParentCategoryId = healthAndFitness },
                new Category { Id = Guid.NewGuid(), Name = "Sports", ParentCategoryId = healthAndFitness },
                new Category { Id = Guid.NewGuid(), Name = "Mental Health", ParentCategoryId = healthAndFitness },
                new Category { Id = Guid.NewGuid(), Name = "Urgent Care", ParentCategoryId = healthAndFitness }
            ]);

            var home = topLevelCategories["Home"];
            categories.AddRange([
                new Category { Id = Guid.NewGuid(), Name = "Furnishings", ParentCategoryId = home },
                new Category { Id = Guid.NewGuid(), Name = "Home Improvement", ParentCategoryId = home },
                new Category { Id = Guid.NewGuid(), Name = "Home Insurance", ParentCategoryId = home },
                new Category { Id = Guid.NewGuid(), Name = "Home Services", ParentCategoryId = home },
                new Category { Id = Guid.NewGuid(), Name = "Lawn & Garden", ParentCategoryId = home },
                new Category { Id = Guid.NewGuid(), Name = "Mortgage & Rent", ParentCategoryId = home }
            ]);

            var income = topLevelCategories["Income"];
            categories.AddRange([
                new Category { Id = Guid.NewGuid(), Name = "Bonus", ParentCategoryId = income },
                new Category { Id = Guid.NewGuid(), Name = "Interest Income", ParentCategoryId = income },
                new Category { Id = Guid.NewGuid(), Name = "Paycheck", ParentCategoryId = income },
                new Category { Id = Guid.NewGuid(), Name = "Reimbursement", ParentCategoryId = income },
                new Category { Id = Guid.NewGuid(), Name = "Rental Income", ParentCategoryId = income },
                new Category { Id = Guid.NewGuid(), Name = "Returned Purchase", ParentCategoryId = income },
                new Category { Id = Guid.NewGuid(), Name = "Gift Money", ParentCategoryId = income }
            ]);

            var investments = topLevelCategories["Investments"];
            categories.AddRange([
                new Category { Id = Guid.NewGuid(), Name = "Buy", ParentCategoryId = investments },
                new Category { Id = Guid.NewGuid(), Name = "Deposit", ParentCategoryId = investments },
                new Category { Id = Guid.NewGuid(), Name = "Dividend & Cap Gains", ParentCategoryId = investments },
                new Category { Id = Guid.NewGuid(), Name = "Sell", ParentCategoryId = investments },
                new Category { Id = Guid.NewGuid(), Name = "Withdrawal", ParentCategoryId = investments }
            ]);

            var kids = topLevelCategories["Kids"];
            categories.AddRange([
                new Category { Id = Guid.NewGuid(), Name = "Allowance", ParentCategoryId = kids },
                new Category { Id = Guid.NewGuid(), Name = "Baby Supplies", ParentCategoryId = kids },
                new Category { Id = Guid.NewGuid(), Name = "Baby Sitter & Daycare", ParentCategoryId = kids },
                new Category { Id = Guid.NewGuid(), Name = "Child Support", ParentCategoryId = kids },
                new Category { Id = Guid.NewGuid(), Name = "Kids Activities", ParentCategoryId = kids },
                new Category { Id = Guid.NewGuid(), Name = "Toys", ParentCategoryId = kids }
            ]);

            var loans = topLevelCategories["Loans"];
            categories.AddRange([
                new Category { Id = Guid.NewGuid(), Name = "Loan Fees and Charges", ParentCategoryId = loans },
                new Category { Id = Guid.NewGuid(), Name = "Loan Insurance", ParentCategoryId = loans },
                new Category { Id = Guid.NewGuid(), Name = "Loan Interest", ParentCategoryId = loans },
                new Category { Id = Guid.NewGuid(), Name = "Loan Payment", ParentCategoryId = loans },
                new Category { Id = Guid.NewGuid(), Name = "Loan Principal", ParentCategoryId = loans }
            ]);

            var miscExpenses = topLevelCategories["Misc Expenses"];
            categories.AddRange([
                new Category { Id = Guid.NewGuid(), Name = "Debt Repayment", ParentCategoryId = miscExpenses },
                new Category { Id = Guid.NewGuid(), Name = "Loan to Friend", ParentCategoryId = miscExpenses },
                new Category { Id = Guid.NewGuid(), Name = "Savings Spending", ParentCategoryId = miscExpenses }
            ]);

            var personalCare = topLevelCategories["Personal Care"];
            categories.AddRange([
                new Category { Id = Guid.NewGuid(), Name = "Hair", ParentCategoryId = personalCare },
                new Category { Id = Guid.NewGuid(), Name = "Laundry", ParentCategoryId = personalCare },
                new Category { Id = Guid.NewGuid(), Name = "Spa & Massage", ParentCategoryId = personalCare }
            ]);

            var pets = topLevelCategories["Pets"];
            categories.AddRange([
                new Category { Id = Guid.NewGuid(), Name = "Pet Food & Supplies", ParentCategoryId = pets },
                new Category { Id = Guid.NewGuid(), Name = "Pet Grooming", ParentCategoryId = pets },
                new Category { Id = Guid.NewGuid(), Name = "Veterinary", ParentCategoryId = pets }
            ]);

            var shopping = topLevelCategories["Shopping"];
            categories.AddRange([
                new Category { Id = Guid.NewGuid(), Name = "Books", ParentCategoryId = shopping },
                new Category { Id = Guid.NewGuid(), Name = "Clothing", ParentCategoryId = shopping },
                new Category { Id = Guid.NewGuid(), Name = "Electronics & Software", ParentCategoryId = shopping },
                new Category { Id = Guid.NewGuid(), Name = "Hobbies", ParentCategoryId = shopping },
                new Category { Id = Guid.NewGuid(), Name = "Sporting Goods", ParentCategoryId = shopping },
                new Category { Id = Guid.NewGuid(), Name = "Home Goods", ParentCategoryId = shopping },
                new Category { Id = Guid.NewGuid(), Name = "Jewelry", ParentCategoryId = shopping }
            ]);

            var taxes = topLevelCategories["Taxes"];
            categories.AddRange([
                new Category { Id = Guid.NewGuid(), Name = "Federal Tax", ParentCategoryId = taxes },
                new Category { Id = Guid.NewGuid(), Name = "Local Tax", ParentCategoryId = taxes },
                new Category { Id = Guid.NewGuid(), Name = "Property Tax", ParentCategoryId = taxes },
                new Category { Id = Guid.NewGuid(), Name = "Sales Tax", ParentCategoryId = taxes },
                new Category { Id = Guid.NewGuid(), Name = "State Tax", ParentCategoryId = taxes }
            ]);

            var transfer = topLevelCategories["Transfer"];
            categories.AddRange([
                new Category { Id = Guid.NewGuid(), Name = "Credit Card Payment", ParentCategoryId = transfer },
                new Category { Id = Guid.NewGuid(), Name = "Transfer for Cash Spending", ParentCategoryId = transfer },
                new Category { Id = Guid.NewGuid(), Name = "Liz Transfer", ParentCategoryId = transfer }
            ]);

            var travel = topLevelCategories["Travel"];
            categories.AddRange([
                new Category { Id = Guid.NewGuid(), Name = "Air Travel", ParentCategoryId = travel },
                new Category { Id = Guid.NewGuid(), Name = "Hotel", ParentCategoryId = travel },
                new Category { Id = Guid.NewGuid(), Name = "Rental Car & Taxi", ParentCategoryId = travel },
                new Category { Id = Guid.NewGuid(), Name = "Vacation", ParentCategoryId = travel }
            ]);

            var uncategorized = topLevelCategories["Uncategorized"];
            categories.AddRange([
                new Category { Id = Guid.NewGuid(), Name = "Cash & ATM", ParentCategoryId = uncategorized },
                new Category { Id = Guid.NewGuid(), Name = "Check", ParentCategoryId = uncategorized }
            ]);

            return categories;
        }
    }
}
