CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" TEXT NOT NULL CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY,
    "ProductVersion" TEXT NOT NULL
);

BEGIN TRANSACTION;

CREATE TABLE "Account" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_Account" PRIMARY KEY,
    "Name" TEXT NOT NULL,
    "Description" TEXT NULL,
    "Type" INTEGER NOT NULL,
    "InitialBalance" REAL NOT NULL,
    "InitialBalanceDate" TEXT NOT NULL,
    "CurrentBalance" REAL NOT NULL,
    "CreatedDate" TEXT NOT NULL,
    "LastModifiedDate" TEXT NOT NULL
);

CREATE TABLE "Budget" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_Budget" PRIMARY KEY,
    "Name" TEXT NULL,
    "Description" TEXT NULL,
    "SortOrder" INTEGER NOT NULL,
    "Timespan" INTEGER NOT NULL,
    "CreatedDate" TEXT NOT NULL,
    "LastModifiedDate" TEXT NOT NULL
);

CREATE TABLE "Category" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_Category" PRIMARY KEY,
    "Name" TEXT NULL,
    "ParentCategoryId" TEXT NULL,
    "CreatedDate" TEXT NOT NULL,
    "LastModifiedDate" TEXT NOT NULL,
    CONSTRAINT "FK_Category_Category_ParentCategoryId" FOREIGN KEY ("ParentCategoryId") REFERENCES "Category" ("Id")
);

CREATE TABLE "Merchant" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_Merchant" PRIMARY KEY,
    "Name" TEXT NULL,
    "CreatedDate" TEXT NOT NULL,
    "LastModifiedDate" TEXT NOT NULL
);

CREATE TABLE "BudgetLineItem" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_BudgetLineItem" PRIMARY KEY,
    "BudgetCategoryId" TEXT NOT NULL,
    "BudgetId" TEXT NOT NULL,
    "LineType" INTEGER NOT NULL,
    "BudgetedAmount" REAL NOT NULL,
    "SpentAmount" REAL NOT NULL,
    "CreatedDate" TEXT NOT NULL,
    "LastModifiedDate" TEXT NOT NULL,
    CONSTRAINT "FK_BudgetLineItem_Budget_BudgetId" FOREIGN KEY ("BudgetId") REFERENCES "Budget" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_BudgetLineItem_Category_BudgetCategoryId" FOREIGN KEY ("BudgetCategoryId") REFERENCES "Category" ("Id") ON DELETE CASCADE
);

CREATE TABLE "Transaction" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_Transaction" PRIMARY KEY,
    "AccountId" TEXT NOT NULL,
    "Date" TEXT NOT NULL,
    "MerchantId" TEXT NOT NULL,
    "CategoryId" TEXT NOT NULL,
    "Amount" REAL NOT NULL,
    "Notes" TEXT NULL,
    "CreatedDate" TEXT NOT NULL,
    "LastModifiedDate" TEXT NOT NULL,
    CONSTRAINT "FK_Transaction_Account_AccountId" FOREIGN KEY ("AccountId") REFERENCES "Account" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_Transaction_Category_CategoryId" FOREIGN KEY ("CategoryId") REFERENCES "Category" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_Transaction_Merchant_MerchantId" FOREIGN KEY ("MerchantId") REFERENCES "Merchant" ("Id") ON DELETE CASCADE
);

CREATE INDEX "IX_BudgetLineItem_BudgetCategoryId" ON "BudgetLineItem" ("BudgetCategoryId");

CREATE INDEX "IX_BudgetLineItem_BudgetId" ON "BudgetLineItem" ("BudgetId");

CREATE UNIQUE INDEX "IX_Category_Name" ON "Category" ("Name");

CREATE INDEX "IX_Category_ParentCategoryId" ON "Category" ("ParentCategoryId");

CREATE UNIQUE INDEX "IX_Merchant_Name" ON "Merchant" ("Name");

CREATE INDEX "IX_Transaction_AccountId" ON "Transaction" ("AccountId");

CREATE INDEX "IX_Transaction_CategoryId" ON "Transaction" ("CategoryId");

CREATE INDEX "IX_Transaction_MerchantId" ON "Transaction" ("MerchantId");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20240113004102_InitialCreate', '8.0.2');

COMMIT;

BEGIN TRANSACTION;

ALTER TABLE "Transaction" ADD "DoNotSkip" INTEGER NOT NULL DEFAULT 0;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20240113201821_AddDoNotSkipTransaction', '8.0.2');

COMMIT;

BEGIN TRANSACTION;

ALTER TABLE "Category" ADD "CategoryType" INTEGER NULL;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20240117134206_CategoryTypesAdded', '8.0.2');

COMMIT;

BEGIN TRANSACTION;

CREATE TABLE "ef_temp_BudgetLineItem" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_BudgetLineItem" PRIMARY KEY,
    "BudgetCategoryId" TEXT NOT NULL,
    "BudgetId" TEXT NOT NULL,
    "BudgetedAmount" REAL NOT NULL,
    "CreatedDate" TEXT NOT NULL,
    "LastModifiedDate" TEXT NOT NULL,
    "SpentAmount" REAL NOT NULL,
    CONSTRAINT "FK_BudgetLineItem_Budget_BudgetId" FOREIGN KEY ("BudgetId") REFERENCES "Budget" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_BudgetLineItem_Category_BudgetCategoryId" FOREIGN KEY ("BudgetCategoryId") REFERENCES "Category" ("Id") ON DELETE CASCADE
);

INSERT INTO "ef_temp_BudgetLineItem" ("Id", "BudgetCategoryId", "BudgetId", "BudgetedAmount", "CreatedDate", "LastModifiedDate", "SpentAmount")
SELECT "Id", "BudgetCategoryId", "BudgetId", "BudgetedAmount", "CreatedDate", "LastModifiedDate", "SpentAmount"
FROM "BudgetLineItem";

COMMIT;

PRAGMA foreign_keys = 0;

BEGIN TRANSACTION;

DROP TABLE "BudgetLineItem";

ALTER TABLE "ef_temp_BudgetLineItem" RENAME TO "BudgetLineItem";

COMMIT;

PRAGMA foreign_keys = 1;

BEGIN TRANSACTION;

CREATE INDEX "IX_BudgetLineItem_BudgetCategoryId" ON "BudgetLineItem" ("BudgetCategoryId");

CREATE INDEX "IX_BudgetLineItem_BudgetId" ON "BudgetLineItem" ("BudgetId");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20240117153923_BudgetLineUpdate', '8.0.2');

COMMIT;

BEGIN TRANSACTION;

ALTER TABLE "BudgetLineItem" ADD "DoRollover" INTEGER NOT NULL DEFAULT 0;

CREATE TABLE "BudgetPeriod" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_BudgetPeriod" PRIMARY KEY,
    "BudgetLineId" TEXT NULL,
    "StartDate" TEXT NOT NULL,
    "EndDate" TEXT NOT NULL,
    "StartingAmount" REAL NOT NULL,
    "SpentAmount" REAL NOT NULL,
    CONSTRAINT "FK_BudgetPeriod_BudgetLineItem_BudgetLineId" FOREIGN KEY ("BudgetLineId") REFERENCES "BudgetLineItem" ("Id")
);

CREATE INDEX "IX_BudgetPeriod_BudgetLineId" ON "BudgetPeriod" ("BudgetLineId");

CREATE TABLE "ef_temp_BudgetLineItem" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_BudgetLineItem" PRIMARY KEY,
    "BudgetCategoryId" TEXT NOT NULL,
    "BudgetId" TEXT NOT NULL,
    "BudgetedAmount" REAL NOT NULL,
    "CreatedDate" TEXT NOT NULL,
    "DoRollover" INTEGER NOT NULL,
    "LastModifiedDate" TEXT NOT NULL,
    CONSTRAINT "FK_BudgetLineItem_Budget_BudgetId" FOREIGN KEY ("BudgetId") REFERENCES "Budget" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_BudgetLineItem_Category_BudgetCategoryId" FOREIGN KEY ("BudgetCategoryId") REFERENCES "Category" ("Id") ON DELETE CASCADE
);

INSERT INTO "ef_temp_BudgetLineItem" ("Id", "BudgetCategoryId", "BudgetId", "BudgetedAmount", "CreatedDate", "DoRollover", "LastModifiedDate")
SELECT "Id", "BudgetCategoryId", "BudgetId", "BudgetedAmount", "CreatedDate", "DoRollover", "LastModifiedDate"
FROM "BudgetLineItem";

CREATE TABLE "ef_temp_Account" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_Account" PRIMARY KEY,
    "CreatedDate" TEXT NOT NULL,
    "Description" TEXT NULL,
    "InitialBalance" REAL NOT NULL,
    "InitialBalanceDate" TEXT NOT NULL,
    "LastModifiedDate" TEXT NOT NULL,
    "Name" TEXT NOT NULL,
    "Type" INTEGER NOT NULL
);

INSERT INTO "ef_temp_Account" ("Id", "CreatedDate", "Description", "InitialBalance", "InitialBalanceDate", "LastModifiedDate", "Name", "Type")
SELECT "Id", "CreatedDate", "Description", "InitialBalance", "InitialBalanceDate", "LastModifiedDate", "Name", "Type"
FROM "Account";

COMMIT;

PRAGMA foreign_keys = 0;

BEGIN TRANSACTION;

DROP TABLE "BudgetLineItem";

ALTER TABLE "ef_temp_BudgetLineItem" RENAME TO "BudgetLineItem";

DROP TABLE "Account";

ALTER TABLE "ef_temp_Account" RENAME TO "Account";

COMMIT;

PRAGMA foreign_keys = 1;

BEGIN TRANSACTION;

CREATE INDEX "IX_BudgetLineItem_BudgetCategoryId" ON "BudgetLineItem" ("BudgetCategoryId");

CREATE INDEX "IX_BudgetLineItem_BudgetId" ON "BudgetLineItem" ("BudgetId");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20240118154914_BudgetLinePeriodsForRollover', '8.0.2');

COMMIT;

BEGIN TRANSACTION;

ALTER TABLE "Transaction" ADD "AccountName" TEXT NULL;

ALTER TABLE "Transaction" ADD "CategoryName" TEXT NULL;

ALTER TABLE "Transaction" ADD "MerchantName" TEXT NULL;

ALTER TABLE "Transaction" ADD "TagNames" TEXT NULL;

ALTER TABLE "Category" ADD "ParentCategoryName" TEXT NULL;

ALTER TABLE "BudgetLineItem" ADD "BudgetCategoryName" TEXT NULL;

ALTER TABLE "BudgetLineItem" ADD "BudgetName" TEXT NULL;

ALTER TABLE "Budget" ADD "BudgetTagId" TEXT NULL;

ALTER TABLE "Budget" ADD "BudgetTagName" TEXT NULL;

CREATE TABLE "Tag" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_Tag" PRIMARY KEY,
    "Name" TEXT NULL
);

CREATE TABLE "TransactionTag" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_TransactionTag" PRIMARY KEY,
    "TransactionId" TEXT NOT NULL,
    "TagId" TEXT NOT NULL,
    CONSTRAINT "FK_TransactionTag_Tag_TagId" FOREIGN KEY ("TagId") REFERENCES "Tag" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_TransactionTag_Transaction_TransactionId" FOREIGN KEY ("TransactionId") REFERENCES "Transaction" ("Id") ON DELETE CASCADE
);

CREATE INDEX "IX_Budget_BudgetTagId" ON "Budget" ("BudgetTagId");

CREATE INDEX "IX_TransactionTag_TagId" ON "TransactionTag" ("TagId");

CREATE INDEX "IX_TransactionTag_TransactionId" ON "TransactionTag" ("TransactionId");

CREATE TABLE "ef_temp_BudgetPeriod" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_BudgetPeriod" PRIMARY KEY,
    "BudgetLineId" TEXT NOT NULL,
    "EndDate" TEXT NOT NULL,
    "SpentAmount" REAL NOT NULL,
    "StartDate" TEXT NOT NULL,
    "StartingAmount" REAL NOT NULL,
    CONSTRAINT "FK_BudgetPeriod_BudgetLineItem_BudgetLineId" FOREIGN KEY ("BudgetLineId") REFERENCES "BudgetLineItem" ("Id") ON DELETE CASCADE
);

INSERT INTO "ef_temp_BudgetPeriod" ("Id", "BudgetLineId", "EndDate", "SpentAmount", "StartDate", "StartingAmount")
SELECT "Id", IFNULL("BudgetLineId", '00000000-0000-0000-0000-000000000000'), "EndDate", "SpentAmount", "StartDate", "StartingAmount"
FROM "BudgetPeriod";

CREATE TABLE "ef_temp_Budget" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_Budget" PRIMARY KEY,
    "BudgetTagId" TEXT NULL,
    "BudgetTagName" TEXT NULL,
    "CreatedDate" TEXT NOT NULL,
    "Description" TEXT NULL,
    "LastModifiedDate" TEXT NOT NULL,
    "Name" TEXT NULL,
    "SortOrder" INTEGER NOT NULL,
    "Timespan" INTEGER NOT NULL,
    CONSTRAINT "FK_Budget_Tag_BudgetTagId" FOREIGN KEY ("BudgetTagId") REFERENCES "Tag" ("Id")
);

INSERT INTO "ef_temp_Budget" ("Id", "BudgetTagId", "BudgetTagName", "CreatedDate", "Description", "LastModifiedDate", "Name", "SortOrder", "Timespan")
SELECT "Id", "BudgetTagId", "BudgetTagName", "CreatedDate", "Description", "LastModifiedDate", "Name", "SortOrder", "Timespan"
FROM "Budget";

COMMIT;

PRAGMA foreign_keys = 0;

BEGIN TRANSACTION;

DROP TABLE "BudgetPeriod";

ALTER TABLE "ef_temp_BudgetPeriod" RENAME TO "BudgetPeriod";

DROP TABLE "Budget";

ALTER TABLE "ef_temp_Budget" RENAME TO "Budget";

COMMIT;

PRAGMA foreign_keys = 1;

BEGIN TRANSACTION;

CREATE INDEX "IX_BudgetPeriod_BudgetLineId" ON "BudgetPeriod" ("BudgetLineId");

CREATE INDEX "IX_Budget_BudgetTagId" ON "Budget" ("BudgetTagId");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20240121183729_TagsAndForeignKeyNames', '8.0.2');

COMMIT;

BEGIN TRANSACTION;

DROP TABLE "TransactionTag";

ALTER TABLE "Transaction" RENAME COLUMN "TagNames" TO "TagId";

ALTER TABLE "Tag" ADD "ShortForm" TEXT NOT NULL DEFAULT '';

CREATE INDEX "IX_Transaction_TagId" ON "Transaction" ("TagId");

CREATE TABLE "ef_temp_Tag" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_Tag" PRIMARY KEY,
    "Name" TEXT NOT NULL,
    "ShortForm" TEXT NOT NULL
);

INSERT INTO "ef_temp_Tag" ("Id", "Name", "ShortForm")
SELECT "Id", IFNULL("Name", ''), "ShortForm"
FROM "Tag";

CREATE TABLE "ef_temp_Transaction" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_Transaction" PRIMARY KEY,
    "AccountId" TEXT NOT NULL,
    "AccountName" TEXT NULL,
    "Amount" REAL NOT NULL,
    "CategoryId" TEXT NOT NULL,
    "CategoryName" TEXT NULL,
    "CreatedDate" TEXT NOT NULL,
    "Date" TEXT NOT NULL,
    "DoNotSkip" INTEGER NOT NULL,
    "LastModifiedDate" TEXT NOT NULL,
    "MerchantId" TEXT NOT NULL,
    "MerchantName" TEXT NULL,
    "Notes" TEXT NULL,
    "TagId" TEXT NULL,
    CONSTRAINT "FK_Transaction_Account_AccountId" FOREIGN KEY ("AccountId") REFERENCES "Account" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_Transaction_Category_CategoryId" FOREIGN KEY ("CategoryId") REFERENCES "Category" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_Transaction_Merchant_MerchantId" FOREIGN KEY ("MerchantId") REFERENCES "Merchant" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_Transaction_Tag_TagId" FOREIGN KEY ("TagId") REFERENCES "Tag" ("Id")
);

INSERT INTO "ef_temp_Transaction" ("Id", "AccountId", "AccountName", "Amount", "CategoryId", "CategoryName", "CreatedDate", "Date", "DoNotSkip", "LastModifiedDate", "MerchantId", "MerchantName", "Notes", "TagId")
SELECT "Id", "AccountId", "AccountName", "Amount", "CategoryId", "CategoryName", "CreatedDate", "Date", "DoNotSkip", "LastModifiedDate", "MerchantId", "MerchantName", "Notes", "TagId"
FROM "Transaction";

COMMIT;

PRAGMA foreign_keys = 0;

BEGIN TRANSACTION;

DROP TABLE "Tag";

ALTER TABLE "ef_temp_Tag" RENAME TO "Tag";

DROP TABLE "Transaction";

ALTER TABLE "ef_temp_Transaction" RENAME TO "Transaction";

COMMIT;

PRAGMA foreign_keys = 1;

BEGIN TRANSACTION;

CREATE INDEX "IX_Transaction_AccountId" ON "Transaction" ("AccountId");

CREATE INDEX "IX_Transaction_CategoryId" ON "Transaction" ("CategoryId");

CREATE INDEX "IX_Transaction_MerchantId" ON "Transaction" ("MerchantId");

CREATE INDEX "IX_Transaction_TagId" ON "Transaction" ("TagId");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20240121194317_LimitingTransactionToSingleTag', '8.0.2');

COMMIT;

BEGIN TRANSACTION;

ALTER TABLE "Transaction" ADD "TagShort" TEXT NULL;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20240121200405_AddingTagShortFormToTransaction', '8.0.2');

COMMIT;

BEGIN TRANSACTION;

ALTER TABLE "Tag" ADD "HexColor" TEXT NULL;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20240121201127_AddingCustomTagColors', '8.0.2');

COMMIT;

BEGIN TRANSACTION;

CREATE TABLE "ef_temp_Transaction" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_Transaction" PRIMARY KEY,
    "AccountId" TEXT NOT NULL,
    "AccountName" TEXT NULL,
    "Amount" REAL NOT NULL,
    "CategoryId" TEXT NOT NULL,
    "CategoryName" TEXT NULL,
    "CreatedDate" TEXT NOT NULL,
    "Date" TEXT NOT NULL,
    "DoNotSkip" INTEGER NOT NULL,
    "LastModifiedDate" TEXT NOT NULL,
    "MerchantId" TEXT NOT NULL,
    "MerchantName" TEXT NULL,
    "Notes" TEXT NULL,
    "TagId" TEXT NULL,
    CONSTRAINT "FK_Transaction_Account_AccountId" FOREIGN KEY ("AccountId") REFERENCES "Account" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_Transaction_Category_CategoryId" FOREIGN KEY ("CategoryId") REFERENCES "Category" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_Transaction_Merchant_MerchantId" FOREIGN KEY ("MerchantId") REFERENCES "Merchant" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_Transaction_Tag_TagId" FOREIGN KEY ("TagId") REFERENCES "Tag" ("Id")
);

INSERT INTO "ef_temp_Transaction" ("Id", "AccountId", "AccountName", "Amount", "CategoryId", "CategoryName", "CreatedDate", "Date", "DoNotSkip", "LastModifiedDate", "MerchantId", "MerchantName", "Notes", "TagId")
SELECT "Id", "AccountId", "AccountName", "Amount", "CategoryId", "CategoryName", "CreatedDate", "Date", "DoNotSkip", "LastModifiedDate", "MerchantId", "MerchantName", "Notes", "TagId"
FROM "Transaction";

COMMIT;

PRAGMA foreign_keys = 0;

BEGIN TRANSACTION;

DROP TABLE "Transaction";

ALTER TABLE "ef_temp_Transaction" RENAME TO "Transaction";

COMMIT;

PRAGMA foreign_keys = 1;

BEGIN TRANSACTION;

CREATE INDEX "IX_Transaction_AccountId" ON "Transaction" ("AccountId");

CREATE INDEX "IX_Transaction_CategoryId" ON "Transaction" ("CategoryId");

CREATE INDEX "IX_Transaction_MerchantId" ON "Transaction" ("MerchantId");

CREATE INDEX "IX_Transaction_TagId" ON "Transaction" ("TagId");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20240121201617_RemovingTagShortForm', '8.0.2');

COMMIT;

BEGIN TRANSACTION;

ALTER TABLE "Budget" ADD "DoNotUseCategories" INTEGER NOT NULL DEFAULT 0;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20240123150802_NoCategoryBudgets', '8.0.2');

COMMIT;

BEGIN TRANSACTION;

CREATE TABLE "ef_temp_Transaction" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_Transaction" PRIMARY KEY,
    "AccountId" TEXT NOT NULL,
    "AccountName" TEXT NULL,
    "Amount" decimal(10, 2) NOT NULL,
    "CategoryId" TEXT NOT NULL,
    "CategoryName" TEXT NULL,
    "CreatedDate" TEXT NOT NULL,
    "Date" TEXT NOT NULL,
    "DoNotSkip" INTEGER NOT NULL,
    "LastModifiedDate" TEXT NOT NULL,
    "MerchantId" TEXT NOT NULL,
    "MerchantName" TEXT NULL,
    "Notes" TEXT NULL,
    "TagId" TEXT NULL,
    CONSTRAINT "FK_Transaction_Account_AccountId" FOREIGN KEY ("AccountId") REFERENCES "Account" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_Transaction_Category_CategoryId" FOREIGN KEY ("CategoryId") REFERENCES "Category" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_Transaction_Merchant_MerchantId" FOREIGN KEY ("MerchantId") REFERENCES "Merchant" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_Transaction_Tag_TagId" FOREIGN KEY ("TagId") REFERENCES "Tag" ("Id")
);

INSERT INTO "ef_temp_Transaction" ("Id", "AccountId", "AccountName", "Amount", "CategoryId", "CategoryName", "CreatedDate", "Date", "DoNotSkip", "LastModifiedDate", "MerchantId", "MerchantName", "Notes", "TagId")
SELECT "Id", "AccountId", "AccountName", "Amount", "CategoryId", "CategoryName", "CreatedDate", "Date", "DoNotSkip", "LastModifiedDate", "MerchantId", "MerchantName", "Notes", "TagId"
FROM "Transaction";

CREATE TABLE "ef_temp_Category" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_Category" PRIMARY KEY,
    "CategoryType" INTEGER NOT NULL,
    "CreatedDate" TEXT NOT NULL,
    "LastModifiedDate" TEXT NOT NULL,
    "Name" TEXT NULL,
    "ParentCategoryId" TEXT NULL,
    "ParentCategoryName" TEXT NULL,
    CONSTRAINT "FK_Category_Category_ParentCategoryId" FOREIGN KEY ("ParentCategoryId") REFERENCES "Category" ("Id")
);

INSERT INTO "ef_temp_Category" ("Id", "CategoryType", "CreatedDate", "LastModifiedDate", "Name", "ParentCategoryId", "ParentCategoryName")
SELECT "Id", IFNULL("CategoryType", 0), "CreatedDate", "LastModifiedDate", "Name", "ParentCategoryId", "ParentCategoryName"
FROM "Category";

CREATE TABLE "ef_temp_BudgetPeriod" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_BudgetPeriod" PRIMARY KEY,
    "BudgetLineId" TEXT NOT NULL,
    "EndDate" TEXT NOT NULL,
    "SpentAmount" decimal(10, 2) NOT NULL,
    "StartDate" TEXT NOT NULL,
    "StartingAmount" decimal(10, 2) NOT NULL,
    CONSTRAINT "FK_BudgetPeriod_BudgetLineItem_BudgetLineId" FOREIGN KEY ("BudgetLineId") REFERENCES "BudgetLineItem" ("Id") ON DELETE CASCADE
);

INSERT INTO "ef_temp_BudgetPeriod" ("Id", "BudgetLineId", "EndDate", "SpentAmount", "StartDate", "StartingAmount")
SELECT "Id", "BudgetLineId", "EndDate", "SpentAmount", "StartDate", "StartingAmount"
FROM "BudgetPeriod";

CREATE TABLE "ef_temp_BudgetLineItem" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_BudgetLineItem" PRIMARY KEY,
    "BudgetCategoryId" TEXT NOT NULL,
    "BudgetCategoryName" TEXT NULL,
    "BudgetId" TEXT NOT NULL,
    "BudgetName" TEXT NULL,
    "BudgetedAmount" decimal(10, 2) NOT NULL,
    "CreatedDate" TEXT NOT NULL,
    "DoRollover" INTEGER NOT NULL,
    "LastModifiedDate" TEXT NOT NULL,
    CONSTRAINT "FK_BudgetLineItem_Budget_BudgetId" FOREIGN KEY ("BudgetId") REFERENCES "Budget" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_BudgetLineItem_Category_BudgetCategoryId" FOREIGN KEY ("BudgetCategoryId") REFERENCES "Category" ("Id") ON DELETE CASCADE
);

INSERT INTO "ef_temp_BudgetLineItem" ("Id", "BudgetCategoryId", "BudgetCategoryName", "BudgetId", "BudgetName", "BudgetedAmount", "CreatedDate", "DoRollover", "LastModifiedDate")
SELECT "Id", "BudgetCategoryId", "BudgetCategoryName", "BudgetId", "BudgetName", "BudgetedAmount", "CreatedDate", "DoRollover", "LastModifiedDate"
FROM "BudgetLineItem";

CREATE TABLE "ef_temp_Account" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_Account" PRIMARY KEY,
    "CreatedDate" TEXT NOT NULL,
    "Description" TEXT NULL,
    "InitialBalance" decimal(10, 2) NOT NULL,
    "InitialBalanceDate" TEXT NOT NULL,
    "LastModifiedDate" TEXT NOT NULL,
    "Name" TEXT NOT NULL,
    "Type" INTEGER NOT NULL
);

INSERT INTO "ef_temp_Account" ("Id", "CreatedDate", "Description", "InitialBalance", "InitialBalanceDate", "LastModifiedDate", "Name", "Type")
SELECT "Id", "CreatedDate", "Description", "InitialBalance", "InitialBalanceDate", "LastModifiedDate", "Name", "Type"
FROM "Account";

COMMIT;

PRAGMA foreign_keys = 0;

BEGIN TRANSACTION;

DROP TABLE "Transaction";

ALTER TABLE "ef_temp_Transaction" RENAME TO "Transaction";

DROP TABLE "Category";

ALTER TABLE "ef_temp_Category" RENAME TO "Category";

DROP TABLE "BudgetPeriod";

ALTER TABLE "ef_temp_BudgetPeriod" RENAME TO "BudgetPeriod";

DROP TABLE "BudgetLineItem";

ALTER TABLE "ef_temp_BudgetLineItem" RENAME TO "BudgetLineItem";

DROP TABLE "Account";

ALTER TABLE "ef_temp_Account" RENAME TO "Account";

COMMIT;

PRAGMA foreign_keys = 1;

BEGIN TRANSACTION;

CREATE INDEX "IX_Transaction_AccountId" ON "Transaction" ("AccountId");

CREATE INDEX "IX_Transaction_CategoryId" ON "Transaction" ("CategoryId");

CREATE INDEX "IX_Transaction_MerchantId" ON "Transaction" ("MerchantId");

CREATE INDEX "IX_Transaction_TagId" ON "Transaction" ("TagId");

CREATE UNIQUE INDEX "IX_Category_Name" ON "Category" ("Name");

CREATE INDEX "IX_Category_ParentCategoryId" ON "Category" ("ParentCategoryId");

CREATE INDEX "IX_BudgetPeriod_BudgetLineId" ON "BudgetPeriod" ("BudgetLineId");

CREATE INDEX "IX_BudgetLineItem_BudgetCategoryId" ON "BudgetLineItem" ("BudgetCategoryId");

CREATE INDEX "IX_BudgetLineItem_BudgetId" ON "BudgetLineItem" ("BudgetId");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20240203190254_SettingPrecision', '8.0.2');

COMMIT;

BEGIN TRANSACTION;

ALTER TABLE "Transaction" ADD "BillId" TEXT NULL;

CREATE TABLE "Bill" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_Bill" PRIMARY KEY,
    "Name" TEXT NULL,
    "AccountId" TEXT NOT NULL,
    "AccountName" TEXT NULL,
    "MerchantId" TEXT NOT NULL,
    "MerchantName" TEXT NULL,
    "StartDate" TEXT NOT NULL,
    "RepeatFrequency" INTEGER NOT NULL,
    "RepeatFrequencyCount" INTEGER NOT NULL,
    "BillAmount" REAL NOT NULL,
    CONSTRAINT "FK_Bill_Account_AccountId" FOREIGN KEY ("AccountId") REFERENCES "Account" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_Bill_Merchant_MerchantId" FOREIGN KEY ("MerchantId") REFERENCES "Merchant" ("Id") ON DELETE CASCADE
);

CREATE INDEX "IX_Transaction_BillId" ON "Transaction" ("BillId");

CREATE UNIQUE INDEX "IX_Tag_Name" ON "Tag" ("Name");

CREATE INDEX "IX_Bill_AccountId" ON "Bill" ("AccountId");

CREATE INDEX "IX_Bill_MerchantId" ON "Bill" ("MerchantId");

CREATE UNIQUE INDEX "IX_Bill_Name" ON "Bill" ("Name");

CREATE TABLE "ef_temp_Transaction" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_Transaction" PRIMARY KEY,
    "AccountId" TEXT NOT NULL,
    "AccountName" TEXT NULL,
    "Amount" decimal(10, 2) NOT NULL,
    "BillId" TEXT NULL,
    "CategoryId" TEXT NOT NULL,
    "CategoryName" TEXT NULL,
    "CreatedDate" TEXT NOT NULL,
    "Date" TEXT NOT NULL,
    "DoNotSkip" INTEGER NOT NULL,
    "LastModifiedDate" TEXT NOT NULL,
    "MerchantId" TEXT NOT NULL,
    "MerchantName" TEXT NULL,
    "Notes" TEXT NULL,
    "TagId" TEXT NULL,
    CONSTRAINT "FK_Transaction_Account_AccountId" FOREIGN KEY ("AccountId") REFERENCES "Account" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_Transaction_Bill_BillId" FOREIGN KEY ("BillId") REFERENCES "Bill" ("Id"),
    CONSTRAINT "FK_Transaction_Category_CategoryId" FOREIGN KEY ("CategoryId") REFERENCES "Category" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_Transaction_Merchant_MerchantId" FOREIGN KEY ("MerchantId") REFERENCES "Merchant" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_Transaction_Tag_TagId" FOREIGN KEY ("TagId") REFERENCES "Tag" ("Id")
);

INSERT INTO "ef_temp_Transaction" ("Id", "AccountId", "AccountName", "Amount", "BillId", "CategoryId", "CategoryName", "CreatedDate", "Date", "DoNotSkip", "LastModifiedDate", "MerchantId", "MerchantName", "Notes", "TagId")
SELECT "Id", "AccountId", "AccountName", "Amount", "BillId", "CategoryId", "CategoryName", "CreatedDate", "Date", "DoNotSkip", "LastModifiedDate", "MerchantId", "MerchantName", "Notes", "TagId"
FROM "Transaction";

COMMIT;

PRAGMA foreign_keys = 0;

BEGIN TRANSACTION;

DROP TABLE "Transaction";

ALTER TABLE "ef_temp_Transaction" RENAME TO "Transaction";

COMMIT;

PRAGMA foreign_keys = 1;

BEGIN TRANSACTION;

CREATE INDEX "IX_Transaction_AccountId" ON "Transaction" ("AccountId");

CREATE INDEX "IX_Transaction_BillId" ON "Transaction" ("BillId");

CREATE INDEX "IX_Transaction_CategoryId" ON "Transaction" ("CategoryId");

CREATE INDEX "IX_Transaction_MerchantId" ON "Transaction" ("MerchantId");

CREATE INDEX "IX_Transaction_TagId" ON "Transaction" ("TagId");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20240204201242_Bills', '8.0.2');

COMMIT;

BEGIN TRANSACTION;

ALTER TABLE "Bill" RENAME COLUMN "StartDate" TO "FirstDueDate";

ALTER TABLE "Bill" ADD "LastDueDate" TEXT NULL;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20240204210115_BillDatesFix', '8.0.2');

COMMIT;

BEGIN TRANSACTION;

ALTER TABLE "Bill" ADD "CategoryId" TEXT NULL;

ALTER TABLE "Bill" ADD "CategoryName" TEXT NULL;

CREATE INDEX "IX_Bill_CategoryId" ON "Bill" ("CategoryId");

CREATE TABLE "ef_temp_Bill" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_Bill" PRIMARY KEY,
    "AccountId" TEXT NOT NULL,
    "AccountName" TEXT NULL,
    "BillAmount" REAL NOT NULL,
    "CategoryId" TEXT NULL,
    "CategoryName" TEXT NULL,
    "FirstDueDate" TEXT NOT NULL,
    "LastDueDate" TEXT NULL,
    "MerchantId" TEXT NOT NULL,
    "MerchantName" TEXT NULL,
    "Name" TEXT NULL,
    "RepeatFrequency" INTEGER NOT NULL,
    "RepeatFrequencyCount" INTEGER NOT NULL,
    CONSTRAINT "FK_Bill_Account_AccountId" FOREIGN KEY ("AccountId") REFERENCES "Account" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_Bill_Category_CategoryId" FOREIGN KEY ("CategoryId") REFERENCES "Category" ("Id"),
    CONSTRAINT "FK_Bill_Merchant_MerchantId" FOREIGN KEY ("MerchantId") REFERENCES "Merchant" ("Id") ON DELETE CASCADE
);

INSERT INTO "ef_temp_Bill" ("Id", "AccountId", "AccountName", "BillAmount", "CategoryId", "CategoryName", "FirstDueDate", "LastDueDate", "MerchantId", "MerchantName", "Name", "RepeatFrequency", "RepeatFrequencyCount")
SELECT "Id", "AccountId", "AccountName", "BillAmount", "CategoryId", "CategoryName", "FirstDueDate", "LastDueDate", "MerchantId", "MerchantName", "Name", "RepeatFrequency", "RepeatFrequencyCount"
FROM "Bill";

COMMIT;

PRAGMA foreign_keys = 0;

BEGIN TRANSACTION;

DROP TABLE "Bill";

ALTER TABLE "ef_temp_Bill" RENAME TO "Bill";

COMMIT;

PRAGMA foreign_keys = 1;

BEGIN TRANSACTION;

CREATE INDEX "IX_Bill_AccountId" ON "Bill" ("AccountId");

CREATE INDEX "IX_Bill_CategoryId" ON "Bill" ("CategoryId");

CREATE INDEX "IX_Bill_MerchantId" ON "Bill" ("MerchantId");

CREATE UNIQUE INDEX "IX_Bill_Name" ON "Bill" ("Name");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20240210220712_AddingCategoryToBill', '8.0.2');

COMMIT;

BEGIN TRANSACTION;

CREATE TABLE "ef_temp_Transaction" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_Transaction" PRIMARY KEY,
    "AccountId" TEXT NOT NULL,
    "AccountName" TEXT NULL,
    "Amount" decimal(10, 2) NOT NULL,
    "BillId" TEXT NULL,
    "CategoryId" TEXT NOT NULL,
    "CategoryName" TEXT NULL,
    "CreatedDate" TEXT NOT NULL,
    "Date" TEXT NOT NULL,
    "DoNotSkip" INTEGER NOT NULL,
    "LastModifiedDate" TEXT NOT NULL,
    "MerchantId" TEXT NOT NULL,
    "MerchantName" TEXT NULL,
    "Notes" TEXT NULL,
    "TagId" TEXT NULL,
    CONSTRAINT "FK_Transaction_Account_AccountId" FOREIGN KEY ("AccountId") REFERENCES "Account" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_Transaction_Bill_BillId" FOREIGN KEY ("BillId") REFERENCES "Bill" ("Id") ON DELETE SET NULL,
    CONSTRAINT "FK_Transaction_Category_CategoryId" FOREIGN KEY ("CategoryId") REFERENCES "Category" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_Transaction_Merchant_MerchantId" FOREIGN KEY ("MerchantId") REFERENCES "Merchant" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_Transaction_Tag_TagId" FOREIGN KEY ("TagId") REFERENCES "Tag" ("Id")
);

INSERT INTO "ef_temp_Transaction" ("Id", "AccountId", "AccountName", "Amount", "BillId", "CategoryId", "CategoryName", "CreatedDate", "Date", "DoNotSkip", "LastModifiedDate", "MerchantId", "MerchantName", "Notes", "TagId")
SELECT "Id", "AccountId", "AccountName", "Amount", "BillId", "CategoryId", "CategoryName", "CreatedDate", "Date", "DoNotSkip", "LastModifiedDate", "MerchantId", "MerchantName", "Notes", "TagId"
FROM "Transaction";

COMMIT;

PRAGMA foreign_keys = 0;

BEGIN TRANSACTION;

DROP TABLE "Transaction";

ALTER TABLE "ef_temp_Transaction" RENAME TO "Transaction";

COMMIT;

PRAGMA foreign_keys = 1;

BEGIN TRANSACTION;

CREATE INDEX "IX_Transaction_AccountId" ON "Transaction" ("AccountId");

CREATE INDEX "IX_Transaction_BillId" ON "Transaction" ("BillId");

CREATE INDEX "IX_Transaction_CategoryId" ON "Transaction" ("CategoryId");

CREATE INDEX "IX_Transaction_MerchantId" ON "Transaction" ("MerchantId");

CREATE INDEX "IX_Transaction_TagId" ON "Transaction" ("TagId");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20240210224633_BillsDontDeleteTransactions', '8.0.2');

COMMIT;

BEGIN TRANSACTION;

ALTER TABLE "BudgetLineItem" ADD "GreenBarAlways" INTEGER NOT NULL DEFAULT 0;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20240212200939_GreenBarOptionsForBudgetLine', '8.0.2');

COMMIT;

BEGIN TRANSACTION;

ALTER TABLE "Transaction" ADD "SavingsGoalId" TEXT NULL;

ALTER TABLE "Transaction" ADD "SavingsGoalName" TEXT NULL;

CREATE TABLE "Goals" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_Goals" PRIMARY KEY,
    "Name" TEXT NULL,
    "Description" TEXT NULL,
    "TargetAmount" REAL NOT NULL,
    "SavedAmount" REAL NOT NULL,
    "TargetDate" TEXT NULL,
    "StartDate" TEXT NOT NULL,
    "AccountId" TEXT NOT NULL,
    "AccountName" TEXT NULL,
    CONSTRAINT "FK_Goals_Account_AccountId" FOREIGN KEY ("AccountId") REFERENCES "Account" ("Id") ON DELETE CASCADE
);

CREATE INDEX "IX_Transaction_SavingsGoalId" ON "Transaction" ("SavingsGoalId");

CREATE INDEX "IX_Goals_AccountId" ON "Goals" ("AccountId");

CREATE UNIQUE INDEX "IX_Goals_Name" ON "Goals" ("Name");

CREATE TABLE "ef_temp_Transaction" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_Transaction" PRIMARY KEY,
    "AccountId" TEXT NOT NULL,
    "AccountName" TEXT NULL,
    "Amount" decimal(10, 2) NOT NULL,
    "BillId" TEXT NULL,
    "CategoryId" TEXT NOT NULL,
    "CategoryName" TEXT NULL,
    "CreatedDate" TEXT NOT NULL,
    "Date" TEXT NOT NULL,
    "DoNotSkip" INTEGER NOT NULL,
    "LastModifiedDate" TEXT NOT NULL,
    "MerchantId" TEXT NOT NULL,
    "MerchantName" TEXT NULL,
    "Notes" TEXT NULL,
    "SavingsGoalId" TEXT NULL,
    "SavingsGoalName" TEXT NULL,
    "TagId" TEXT NULL,
    CONSTRAINT "FK_Transaction_Account_AccountId" FOREIGN KEY ("AccountId") REFERENCES "Account" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_Transaction_Bill_BillId" FOREIGN KEY ("BillId") REFERENCES "Bill" ("Id") ON DELETE SET NULL,
    CONSTRAINT "FK_Transaction_Category_CategoryId" FOREIGN KEY ("CategoryId") REFERENCES "Category" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_Transaction_Goals_SavingsGoalId" FOREIGN KEY ("SavingsGoalId") REFERENCES "Goals" ("Id"),
    CONSTRAINT "FK_Transaction_Merchant_MerchantId" FOREIGN KEY ("MerchantId") REFERENCES "Merchant" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_Transaction_Tag_TagId" FOREIGN KEY ("TagId") REFERENCES "Tag" ("Id")
);

INSERT INTO "ef_temp_Transaction" ("Id", "AccountId", "AccountName", "Amount", "BillId", "CategoryId", "CategoryName", "CreatedDate", "Date", "DoNotSkip", "LastModifiedDate", "MerchantId", "MerchantName", "Notes", "SavingsGoalId", "SavingsGoalName", "TagId")
SELECT "Id", "AccountId", "AccountName", "Amount", "BillId", "CategoryId", "CategoryName", "CreatedDate", "Date", "DoNotSkip", "LastModifiedDate", "MerchantId", "MerchantName", "Notes", "SavingsGoalId", "SavingsGoalName", "TagId"
FROM "Transaction";

COMMIT;

PRAGMA foreign_keys = 0;

BEGIN TRANSACTION;

DROP TABLE "Transaction";

ALTER TABLE "ef_temp_Transaction" RENAME TO "Transaction";

COMMIT;

PRAGMA foreign_keys = 1;

BEGIN TRANSACTION;

CREATE INDEX "IX_Transaction_AccountId" ON "Transaction" ("AccountId");

CREATE INDEX "IX_Transaction_BillId" ON "Transaction" ("BillId");

CREATE INDEX "IX_Transaction_CategoryId" ON "Transaction" ("CategoryId");

CREATE INDEX "IX_Transaction_MerchantId" ON "Transaction" ("MerchantId");

CREATE INDEX "IX_Transaction_SavingsGoalId" ON "Transaction" ("SavingsGoalId");

CREATE INDEX "IX_Transaction_TagId" ON "Transaction" ("TagId");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20240217185802_SavingsGoals', '8.0.2');

COMMIT;

BEGIN TRANSACTION;

ALTER TABLE "Goals" ADD "StartingAmount" REAL NOT NULL DEFAULT 0.0;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20240217202535_AddingStartinAmountForSavingsGoal', '8.0.2');

COMMIT;

BEGIN TRANSACTION;

ALTER TABLE "Bill" ADD "EndDate" TEXT NULL;

CREATE TABLE "ef_temp_Goals" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_Goals" PRIMARY KEY,
    "AccountId" TEXT NOT NULL,
    "AccountName" TEXT NULL,
    "Description" TEXT NULL,
    "Name" TEXT NULL,
    "SavedAmount" REAL NOT NULL,
    "StartDate" TEXT NOT NULL,
    "StartingAmount" REAL NULL,
    "TargetAmount" REAL NOT NULL,
    "TargetDate" TEXT NULL,
    CONSTRAINT "FK_Goals_Account_AccountId" FOREIGN KEY ("AccountId") REFERENCES "Account" ("Id") ON DELETE CASCADE
);

INSERT INTO "ef_temp_Goals" ("Id", "AccountId", "AccountName", "Description", "Name", "SavedAmount", "StartDate", "StartingAmount", "TargetAmount", "TargetDate")
SELECT "Id", "AccountId", "AccountName", "Description", "Name", "SavedAmount", "StartDate", "StartingAmount", "TargetAmount", "TargetDate"
FROM "Goals";

COMMIT;

PRAGMA foreign_keys = 0;

BEGIN TRANSACTION;

DROP TABLE "Goals";

ALTER TABLE "ef_temp_Goals" RENAME TO "Goals";

COMMIT;

PRAGMA foreign_keys = 1;

BEGIN TRANSACTION;

CREATE INDEX "IX_Goals_AccountId" ON "Goals" ("AccountId");

CREATE UNIQUE INDEX "IX_Goals_Name" ON "Goals" ("Name");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20240228223440_BillEndDate', '8.0.2');

COMMIT;

BEGIN TRANSACTION;

ALTER TABLE "Transaction" ADD "PairedTransactionId" TEXT NULL;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20240301151117_PairingTransferTransactions', '8.0.2');

COMMIT;

BEGIN TRANSACTION;

ALTER TABLE "Transaction" ADD "IsSavingsSpending" INTEGER NOT NULL DEFAULT 0;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20240301191014_SavingsSpendingTransactions', '8.0.2');

COMMIT;

BEGIN TRANSACTION;

CREATE INDEX "IX_Transaction_PairedTransactionId" ON "Transaction" ("PairedTransactionId");

CREATE TABLE "ef_temp_Transaction" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_Transaction" PRIMARY KEY,
    "AccountId" TEXT NOT NULL,
    "AccountName" TEXT NULL,
    "Amount" decimal(10, 2) NOT NULL,
    "BillId" TEXT NULL,
    "CategoryId" TEXT NOT NULL,
    "CategoryName" TEXT NULL,
    "CreatedDate" TEXT NOT NULL,
    "Date" TEXT NOT NULL,
    "DoNotSkip" INTEGER NOT NULL,
    "IsSavingsSpending" INTEGER NOT NULL,
    "LastModifiedDate" TEXT NOT NULL,
    "MerchantId" TEXT NOT NULL,
    "MerchantName" TEXT NULL,
    "Notes" TEXT NULL,
    "PairedTransactionId" TEXT NULL,
    "SavingsGoalId" TEXT NULL,
    "SavingsGoalName" TEXT NULL,
    "TagId" TEXT NULL,
    CONSTRAINT "FK_Transaction_Account_AccountId" FOREIGN KEY ("AccountId") REFERENCES "Account" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_Transaction_Bill_BillId" FOREIGN KEY ("BillId") REFERENCES "Bill" ("Id") ON DELETE SET NULL,
    CONSTRAINT "FK_Transaction_Category_CategoryId" FOREIGN KEY ("CategoryId") REFERENCES "Category" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_Transaction_Goals_SavingsGoalId" FOREIGN KEY ("SavingsGoalId") REFERENCES "Goals" ("Id") ON DELETE SET NULL,
    CONSTRAINT "FK_Transaction_Merchant_MerchantId" FOREIGN KEY ("MerchantId") REFERENCES "Merchant" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_Transaction_Tag_TagId" FOREIGN KEY ("TagId") REFERENCES "Tag" ("Id"),
    CONSTRAINT "FK_Transaction_Transaction_PairedTransactionId" FOREIGN KEY ("PairedTransactionId") REFERENCES "Transaction" ("Id")
);

INSERT INTO "ef_temp_Transaction" ("Id", "AccountId", "AccountName", "Amount", "BillId", "CategoryId", "CategoryName", "CreatedDate", "Date", "DoNotSkip", "IsSavingsSpending", "LastModifiedDate", "MerchantId", "MerchantName", "Notes", "PairedTransactionId", "SavingsGoalId", "SavingsGoalName", "TagId")
SELECT "Id", "AccountId", "AccountName", "Amount", "BillId", "CategoryId", "CategoryName", "CreatedDate", "Date", "DoNotSkip", "IsSavingsSpending", "LastModifiedDate", "MerchantId", "MerchantName", "Notes", "PairedTransactionId", "SavingsGoalId", "SavingsGoalName", "TagId"
FROM "Transaction";

COMMIT;

PRAGMA foreign_keys = 0;

BEGIN TRANSACTION;

DROP TABLE "Transaction";

ALTER TABLE "ef_temp_Transaction" RENAME TO "Transaction";

COMMIT;

PRAGMA foreign_keys = 1;

BEGIN TRANSACTION;

CREATE INDEX "IX_Transaction_AccountId" ON "Transaction" ("AccountId");

CREATE INDEX "IX_Transaction_BillId" ON "Transaction" ("BillId");

CREATE INDEX "IX_Transaction_CategoryId" ON "Transaction" ("CategoryId");

CREATE INDEX "IX_Transaction_MerchantId" ON "Transaction" ("MerchantId");

CREATE INDEX "IX_Transaction_PairedTransactionId" ON "Transaction" ("PairedTransactionId");

CREATE INDEX "IX_Transaction_SavingsGoalId" ON "Transaction" ("SavingsGoalId");

CREATE INDEX "IX_Transaction_TagId" ON "Transaction" ("TagId");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20240301214011_PairingTransactions', '8.0.2');

COMMIT;

BEGIN TRANSACTION;

DROP INDEX "IX_Transaction_PairedTransactionId";

CREATE TABLE "ef_temp_Transaction" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_Transaction" PRIMARY KEY,
    "AccountId" TEXT NOT NULL,
    "AccountName" TEXT NULL,
    "Amount" decimal(10, 2) NOT NULL,
    "BillId" TEXT NULL,
    "CategoryId" TEXT NOT NULL,
    "CategoryName" TEXT NULL,
    "CreatedDate" TEXT NOT NULL,
    "Date" TEXT NOT NULL,
    "DoNotSkip" INTEGER NOT NULL,
    "IsSavingsSpending" INTEGER NOT NULL,
    "LastModifiedDate" TEXT NOT NULL,
    "MerchantId" TEXT NOT NULL,
    "MerchantName" TEXT NULL,
    "Notes" TEXT NULL,
    "SavingsGoalId" TEXT NULL,
    "SavingsGoalName" TEXT NULL,
    "TagId" TEXT NULL,
    CONSTRAINT "FK_Transaction_Account_AccountId" FOREIGN KEY ("AccountId") REFERENCES "Account" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_Transaction_Bill_BillId" FOREIGN KEY ("BillId") REFERENCES "Bill" ("Id") ON DELETE SET NULL,
    CONSTRAINT "FK_Transaction_Category_CategoryId" FOREIGN KEY ("CategoryId") REFERENCES "Category" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_Transaction_Goals_SavingsGoalId" FOREIGN KEY ("SavingsGoalId") REFERENCES "Goals" ("Id") ON DELETE SET NULL,
    CONSTRAINT "FK_Transaction_Merchant_MerchantId" FOREIGN KEY ("MerchantId") REFERENCES "Merchant" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_Transaction_Tag_TagId" FOREIGN KEY ("TagId") REFERENCES "Tag" ("Id")
);

INSERT INTO "ef_temp_Transaction" ("Id", "AccountId", "AccountName", "Amount", "BillId", "CategoryId", "CategoryName", "CreatedDate", "Date", "DoNotSkip", "IsSavingsSpending", "LastModifiedDate", "MerchantId", "MerchantName", "Notes", "SavingsGoalId", "SavingsGoalName", "TagId")
SELECT "Id", "AccountId", "AccountName", "Amount", "BillId", "CategoryId", "CategoryName", "CreatedDate", "Date", "DoNotSkip", "IsSavingsSpending", "LastModifiedDate", "MerchantId", "MerchantName", "Notes", "SavingsGoalId", "SavingsGoalName", "TagId"
FROM "Transaction";

COMMIT;

PRAGMA foreign_keys = 0;

BEGIN TRANSACTION;

DROP TABLE "Transaction";

ALTER TABLE "ef_temp_Transaction" RENAME TO "Transaction";

COMMIT;

PRAGMA foreign_keys = 1;

BEGIN TRANSACTION;

CREATE INDEX "IX_Transaction_AccountId" ON "Transaction" ("AccountId");

CREATE INDEX "IX_Transaction_BillId" ON "Transaction" ("BillId");

CREATE INDEX "IX_Transaction_CategoryId" ON "Transaction" ("CategoryId");

CREATE INDEX "IX_Transaction_MerchantId" ON "Transaction" ("MerchantId");

CREATE INDEX "IX_Transaction_SavingsGoalId" ON "Transaction" ("SavingsGoalId");

CREATE INDEX "IX_Transaction_TagId" ON "Transaction" ("TagId");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20240301220607_RemovingPairedTransactionsForNow', '8.0.2');

COMMIT;

BEGIN TRANSACTION;

ALTER TABLE "Account" ADD "MinimumBalance" decimal(10, 2) NULL;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20240302183658_AccountMinimumBalances', '8.0.2');

COMMIT;

BEGIN TRANSACTION;

CREATE TABLE "ef_temp_Transaction" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_Transaction" PRIMARY KEY,
    "AccountId" TEXT NOT NULL,
    "AccountName" TEXT NULL,
    "Amount" decimal(10, 2) NOT NULL,
    "BillId" TEXT NULL,
    "CategoryId" TEXT NOT NULL,
    "CategoryName" TEXT NULL,
    "CreatedDate" TEXT NOT NULL,
    "Date" TEXT NOT NULL,
    "DoNotSkip" INTEGER NOT NULL,
    "LastModifiedDate" TEXT NOT NULL,
    "MerchantId" TEXT NOT NULL,
    "MerchantName" TEXT NULL,
    "Notes" TEXT NULL,
    "SavingsGoalId" TEXT NULL,
    "SavingsGoalName" TEXT NULL,
    "TagId" TEXT NULL,
    CONSTRAINT "FK_Transaction_Account_AccountId" FOREIGN KEY ("AccountId") REFERENCES "Account" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_Transaction_Bill_BillId" FOREIGN KEY ("BillId") REFERENCES "Bill" ("Id") ON DELETE SET NULL,
    CONSTRAINT "FK_Transaction_Category_CategoryId" FOREIGN KEY ("CategoryId") REFERENCES "Category" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_Transaction_Goals_SavingsGoalId" FOREIGN KEY ("SavingsGoalId") REFERENCES "Goals" ("Id"),
    CONSTRAINT "FK_Transaction_Merchant_MerchantId" FOREIGN KEY ("MerchantId") REFERENCES "Merchant" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_Transaction_Tag_TagId" FOREIGN KEY ("TagId") REFERENCES "Tag" ("Id")
);

INSERT INTO "ef_temp_Transaction" ("Id", "AccountId", "AccountName", "Amount", "BillId", "CategoryId", "CategoryName", "CreatedDate", "Date", "DoNotSkip", "LastModifiedDate", "MerchantId", "MerchantName", "Notes", "SavingsGoalId", "SavingsGoalName", "TagId")
SELECT "Id", "AccountId", "AccountName", "Amount", "BillId", "CategoryId", "CategoryName", "CreatedDate", "Date", "DoNotSkip", "LastModifiedDate", "MerchantId", "MerchantName", "Notes", "SavingsGoalId", "SavingsGoalName", "TagId"
FROM "Transaction";

COMMIT;

PRAGMA foreign_keys = 0;

BEGIN TRANSACTION;

DROP TABLE "Transaction";

ALTER TABLE "ef_temp_Transaction" RENAME TO "Transaction";

COMMIT;

PRAGMA foreign_keys = 1;

BEGIN TRANSACTION;

CREATE INDEX "IX_Transaction_AccountId" ON "Transaction" ("AccountId");

CREATE INDEX "IX_Transaction_BillId" ON "Transaction" ("BillId");

CREATE INDEX "IX_Transaction_CategoryId" ON "Transaction" ("CategoryId");

CREATE INDEX "IX_Transaction_MerchantId" ON "Transaction" ("MerchantId");

CREATE INDEX "IX_Transaction_SavingsGoalId" ON "Transaction" ("SavingsGoalId");

CREATE INDEX "IX_Transaction_TagId" ON "Transaction" ("TagId");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20240302183915_AccountMinimumBalances2', '8.0.2');

COMMIT;

BEGIN TRANSACTION;

ALTER TABLE "Bill" ADD "RepeatConfigId" TEXT NULL;

CREATE TABLE "RepeatConfig" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_RepeatConfig" PRIMARY KEY,
    "Mode" INTEGER NOT NULL,
    "Frequency" INTEGER NOT NULL,
    "IntervalGap" INTEGER NOT NULL,
    "FirstFiring" TEXT NOT NULL,
    "TerminationDate" TEXT NULL,
    "LastFiring" TEXT NULL
);

CREATE TABLE "RepeatTransfer" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_RepeatTransfer" PRIMARY KEY,
    "RepeatConfigId" TEXT NOT NULL,
    "FromAccountId" TEXT NOT NULL,
    "ToAccountId" TEXT NOT NULL,
    "Date" TEXT NOT NULL,
    "MerchantId" TEXT NOT NULL,
    "CategoryId" TEXT NOT NULL,
    "Amount" decimal(10, 2) NOT NULL,
    "Notes" TEXT NULL,
    "TagId" TEXT NULL,
    CONSTRAINT "FK_RepeatTransfer_Account_FromAccountId" FOREIGN KEY ("FromAccountId") REFERENCES "Account" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_RepeatTransfer_Account_ToAccountId" FOREIGN KEY ("ToAccountId") REFERENCES "Account" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_RepeatTransfer_Category_CategoryId" FOREIGN KEY ("CategoryId") REFERENCES "Category" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_RepeatTransfer_Merchant_MerchantId" FOREIGN KEY ("MerchantId") REFERENCES "Merchant" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_RepeatTransfer_RepeatConfig_RepeatConfigId" FOREIGN KEY ("RepeatConfigId") REFERENCES "RepeatConfig" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_RepeatTransfer_Tag_TagId" FOREIGN KEY ("TagId") REFERENCES "Tag" ("Id")
);

CREATE INDEX "IX_Bill_RepeatConfigId" ON "Bill" ("RepeatConfigId");

CREATE INDEX "IX_RepeatTransfer_CategoryId" ON "RepeatTransfer" ("CategoryId");

CREATE INDEX "IX_RepeatTransfer_FromAccountId" ON "RepeatTransfer" ("FromAccountId");

CREATE INDEX "IX_RepeatTransfer_MerchantId" ON "RepeatTransfer" ("MerchantId");

CREATE INDEX "IX_RepeatTransfer_RepeatConfigId" ON "RepeatTransfer" ("RepeatConfigId");

CREATE INDEX "IX_RepeatTransfer_TagId" ON "RepeatTransfer" ("TagId");

CREATE INDEX "IX_RepeatTransfer_ToAccountId" ON "RepeatTransfer" ("ToAccountId");

CREATE TABLE "ef_temp_Transaction" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_Transaction" PRIMARY KEY,
    "AccountId" TEXT NOT NULL,
    "AccountName" TEXT NULL,
    "Amount" decimal(10, 2) NOT NULL,
    "BillId" TEXT NULL,
    "CategoryId" TEXT NOT NULL,
    "CategoryName" TEXT NULL,
    "Date" TEXT NOT NULL,
    "DoNotSkip" INTEGER NOT NULL,
    "IsSavingsSpending" INTEGER NOT NULL,
    "MerchantId" TEXT NOT NULL,
    "MerchantName" TEXT NULL,
    "Notes" TEXT NULL,
    "SavingsGoalId" TEXT NULL,
    "SavingsGoalName" TEXT NULL,
    "TagId" TEXT NULL,
    CONSTRAINT "FK_Transaction_Account_AccountId" FOREIGN KEY ("AccountId") REFERENCES "Account" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_Transaction_Bill_BillId" FOREIGN KEY ("BillId") REFERENCES "Bill" ("Id") ON DELETE SET NULL,
    CONSTRAINT "FK_Transaction_Category_CategoryId" FOREIGN KEY ("CategoryId") REFERENCES "Category" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_Transaction_Goals_SavingsGoalId" FOREIGN KEY ("SavingsGoalId") REFERENCES "Goals" ("Id") ON DELETE SET NULL,
    CONSTRAINT "FK_Transaction_Merchant_MerchantId" FOREIGN KEY ("MerchantId") REFERENCES "Merchant" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_Transaction_Tag_TagId" FOREIGN KEY ("TagId") REFERENCES "Tag" ("Id")
);

INSERT INTO "ef_temp_Transaction" ("Id", "AccountId", "AccountName", "Amount", "BillId", "CategoryId", "CategoryName", "Date", "DoNotSkip", "IsSavingsSpending", "MerchantId", "MerchantName", "Notes", "SavingsGoalId", "SavingsGoalName", "TagId")
SELECT "Id", "AccountId", "AccountName", "Amount", "BillId", "CategoryId", "CategoryName", "Date", "DoNotSkip", "IsSavingsSpending", "MerchantId", "MerchantName", "Notes", "SavingsGoalId", "SavingsGoalName", "TagId"
FROM "Transaction";

CREATE TABLE "ef_temp_Merchant" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_Merchant" PRIMARY KEY,
    "Name" TEXT NULL
);

INSERT INTO "ef_temp_Merchant" ("Id", "Name")
SELECT "Id", "Name"
FROM "Merchant";

CREATE TABLE "ef_temp_Category" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_Category" PRIMARY KEY,
    "CategoryType" INTEGER NOT NULL,
    "Name" TEXT NULL,
    "ParentCategoryId" TEXT NULL,
    "ParentCategoryName" TEXT NULL,
    CONSTRAINT "FK_Category_Category_ParentCategoryId" FOREIGN KEY ("ParentCategoryId") REFERENCES "Category" ("Id")
);

INSERT INTO "ef_temp_Category" ("Id", "CategoryType", "Name", "ParentCategoryId", "ParentCategoryName")
SELECT "Id", "CategoryType", "Name", "ParentCategoryId", "ParentCategoryName"
FROM "Category";

CREATE TABLE "ef_temp_BudgetLineItem" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_BudgetLineItem" PRIMARY KEY,
    "BudgetCategoryId" TEXT NOT NULL,
    "BudgetCategoryName" TEXT NULL,
    "BudgetId" TEXT NOT NULL,
    "BudgetName" TEXT NULL,
    "BudgetedAmount" decimal(10, 2) NOT NULL,
    "DoRollover" INTEGER NOT NULL,
    "GreenBarAlways" INTEGER NOT NULL,
    CONSTRAINT "FK_BudgetLineItem_Budget_BudgetId" FOREIGN KEY ("BudgetId") REFERENCES "Budget" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_BudgetLineItem_Category_BudgetCategoryId" FOREIGN KEY ("BudgetCategoryId") REFERENCES "Category" ("Id") ON DELETE CASCADE
);

INSERT INTO "ef_temp_BudgetLineItem" ("Id", "BudgetCategoryId", "BudgetCategoryName", "BudgetId", "BudgetName", "BudgetedAmount", "DoRollover", "GreenBarAlways")
SELECT "Id", "BudgetCategoryId", "BudgetCategoryName", "BudgetId", "BudgetName", "BudgetedAmount", "DoRollover", "GreenBarAlways"
FROM "BudgetLineItem";

CREATE TABLE "ef_temp_Budget" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_Budget" PRIMARY KEY,
    "BudgetTagId" TEXT NULL,
    "BudgetTagName" TEXT NULL,
    "Description" TEXT NULL,
    "DoNotUseCategories" INTEGER NOT NULL,
    "Name" TEXT NULL,
    "SortOrder" INTEGER NOT NULL,
    "Timespan" INTEGER NOT NULL,
    CONSTRAINT "FK_Budget_Tag_BudgetTagId" FOREIGN KEY ("BudgetTagId") REFERENCES "Tag" ("Id")
);

INSERT INTO "ef_temp_Budget" ("Id", "BudgetTagId", "BudgetTagName", "Description", "DoNotUseCategories", "Name", "SortOrder", "Timespan")
SELECT "Id", "BudgetTagId", "BudgetTagName", "Description", "DoNotUseCategories", "Name", "SortOrder", "Timespan"
FROM "Budget";

CREATE TABLE "ef_temp_Account" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_Account" PRIMARY KEY,
    "Description" TEXT NULL,
    "InitialBalance" decimal(10, 2) NOT NULL,
    "InitialBalanceDate" TEXT NOT NULL,
    "MinimumBalance" decimal(10, 2) NULL,
    "Name" TEXT NOT NULL,
    "Type" INTEGER NOT NULL
);

INSERT INTO "ef_temp_Account" ("Id", "Description", "InitialBalance", "InitialBalanceDate", "MinimumBalance", "Name", "Type")
SELECT "Id", "Description", "InitialBalance", "InitialBalanceDate", "MinimumBalance", "Name", "Type"
FROM "Account";

CREATE TABLE "ef_temp_Bill" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_Bill" PRIMARY KEY,
    "AccountId" TEXT NOT NULL,
    "AccountName" TEXT NULL,
    "BillAmount" REAL NOT NULL,
    "CategoryId" TEXT NULL,
    "CategoryName" TEXT NULL,
    "EndDate" TEXT NULL,
    "FirstDueDate" TEXT NOT NULL,
    "MerchantId" TEXT NOT NULL,
    "MerchantName" TEXT NULL,
    "Name" TEXT NULL,
    "RepeatConfigId" TEXT NULL,
    "RepeatFrequency" INTEGER NOT NULL,
    "RepeatFrequencyCount" INTEGER NOT NULL,
    CONSTRAINT "FK_Bill_Account_AccountId" FOREIGN KEY ("AccountId") REFERENCES "Account" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_Bill_Category_CategoryId" FOREIGN KEY ("CategoryId") REFERENCES "Category" ("Id"),
    CONSTRAINT "FK_Bill_Merchant_MerchantId" FOREIGN KEY ("MerchantId") REFERENCES "Merchant" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_Bill_RepeatConfig_RepeatConfigId" FOREIGN KEY ("RepeatConfigId") REFERENCES "RepeatConfig" ("Id")
);

INSERT INTO "ef_temp_Bill" ("Id", "AccountId", "AccountName", "BillAmount", "CategoryId", "CategoryName", "EndDate", "FirstDueDate", "MerchantId", "MerchantName", "Name", "RepeatConfigId", "RepeatFrequency", "RepeatFrequencyCount")
SELECT "Id", "AccountId", "AccountName", "BillAmount", "CategoryId", "CategoryName", "EndDate", "FirstDueDate", "MerchantId", "MerchantName", "Name", "RepeatConfigId", "RepeatFrequency", "RepeatFrequencyCount"
FROM "Bill";

COMMIT;

PRAGMA foreign_keys = 0;

BEGIN TRANSACTION;

DROP TABLE "Transaction";

ALTER TABLE "ef_temp_Transaction" RENAME TO "Transaction";

DROP TABLE "Merchant";

ALTER TABLE "ef_temp_Merchant" RENAME TO "Merchant";

DROP TABLE "Category";

ALTER TABLE "ef_temp_Category" RENAME TO "Category";

DROP TABLE "BudgetLineItem";

ALTER TABLE "ef_temp_BudgetLineItem" RENAME TO "BudgetLineItem";

DROP TABLE "Budget";

ALTER TABLE "ef_temp_Budget" RENAME TO "Budget";

DROP TABLE "Account";

ALTER TABLE "ef_temp_Account" RENAME TO "Account";

DROP TABLE "Bill";

ALTER TABLE "ef_temp_Bill" RENAME TO "Bill";

COMMIT;

PRAGMA foreign_keys = 1;

BEGIN TRANSACTION;

CREATE INDEX "IX_Transaction_AccountId" ON "Transaction" ("AccountId");

CREATE INDEX "IX_Transaction_BillId" ON "Transaction" ("BillId");

CREATE INDEX "IX_Transaction_CategoryId" ON "Transaction" ("CategoryId");

CREATE INDEX "IX_Transaction_MerchantId" ON "Transaction" ("MerchantId");

CREATE INDEX "IX_Transaction_SavingsGoalId" ON "Transaction" ("SavingsGoalId");

CREATE INDEX "IX_Transaction_TagId" ON "Transaction" ("TagId");

CREATE UNIQUE INDEX "IX_Merchant_Name" ON "Merchant" ("Name");

CREATE UNIQUE INDEX "IX_Category_Name" ON "Category" ("Name");

CREATE INDEX "IX_Category_ParentCategoryId" ON "Category" ("ParentCategoryId");

CREATE INDEX "IX_BudgetLineItem_BudgetCategoryId" ON "BudgetLineItem" ("BudgetCategoryId");

CREATE INDEX "IX_BudgetLineItem_BudgetId" ON "BudgetLineItem" ("BudgetId");

CREATE INDEX "IX_Budget_BudgetTagId" ON "Budget" ("BudgetTagId");

CREATE INDEX "IX_Bill_AccountId" ON "Bill" ("AccountId");

CREATE INDEX "IX_Bill_CategoryId" ON "Bill" ("CategoryId");

CREATE INDEX "IX_Bill_MerchantId" ON "Bill" ("MerchantId");

CREATE UNIQUE INDEX "IX_Bill_Name" ON "Bill" ("Name");

CREATE INDEX "IX_Bill_RepeatConfigId" ON "Bill" ("RepeatConfigId");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20240304173401_RepeatingBehaviorUpdate', '8.0.2');

COMMIT;

BEGIN TRANSACTION;

ALTER TABLE "Bill" ADD "LastDueDate" TEXT NULL;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20240304174545_RestoringBillField', '8.0.2');

COMMIT;

BEGIN TRANSACTION;

ALTER TABLE "RepeatTransfer" ADD "SavingsGoalId" TEXT NULL;

CREATE INDEX "IX_RepeatTransfer_SavingsGoalId" ON "RepeatTransfer" ("SavingsGoalId");

CREATE TABLE "ef_temp_RepeatTransfer" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_RepeatTransfer" PRIMARY KEY,
    "Amount" decimal(10, 2) NOT NULL,
    "CategoryId" TEXT NOT NULL,
    "Date" TEXT NOT NULL,
    "FromAccountId" TEXT NOT NULL,
    "MerchantId" TEXT NOT NULL,
    "Notes" TEXT NULL,
    "RepeatConfigId" TEXT NOT NULL,
    "SavingsGoalId" TEXT NULL,
    "TagId" TEXT NULL,
    "ToAccountId" TEXT NOT NULL,
    CONSTRAINT "FK_RepeatTransfer_Account_FromAccountId" FOREIGN KEY ("FromAccountId") REFERENCES "Account" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_RepeatTransfer_Account_ToAccountId" FOREIGN KEY ("ToAccountId") REFERENCES "Account" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_RepeatTransfer_Category_CategoryId" FOREIGN KEY ("CategoryId") REFERENCES "Category" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_RepeatTransfer_Goals_SavingsGoalId" FOREIGN KEY ("SavingsGoalId") REFERENCES "Goals" ("Id"),
    CONSTRAINT "FK_RepeatTransfer_Merchant_MerchantId" FOREIGN KEY ("MerchantId") REFERENCES "Merchant" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_RepeatTransfer_RepeatConfig_RepeatConfigId" FOREIGN KEY ("RepeatConfigId") REFERENCES "RepeatConfig" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_RepeatTransfer_Tag_TagId" FOREIGN KEY ("TagId") REFERENCES "Tag" ("Id")
);

INSERT INTO "ef_temp_RepeatTransfer" ("Id", "Amount", "CategoryId", "Date", "FromAccountId", "MerchantId", "Notes", "RepeatConfigId", "SavingsGoalId", "TagId", "ToAccountId")
SELECT "Id", "Amount", "CategoryId", "Date", "FromAccountId", "MerchantId", "Notes", "RepeatConfigId", "SavingsGoalId", "TagId", "ToAccountId"
FROM "RepeatTransfer";

COMMIT;

PRAGMA foreign_keys = 0;

BEGIN TRANSACTION;

DROP TABLE "RepeatTransfer";

ALTER TABLE "ef_temp_RepeatTransfer" RENAME TO "RepeatTransfer";

COMMIT;

PRAGMA foreign_keys = 1;

BEGIN TRANSACTION;

CREATE INDEX "IX_RepeatTransfer_CategoryId" ON "RepeatTransfer" ("CategoryId");

CREATE INDEX "IX_RepeatTransfer_FromAccountId" ON "RepeatTransfer" ("FromAccountId");

CREATE INDEX "IX_RepeatTransfer_MerchantId" ON "RepeatTransfer" ("MerchantId");

CREATE INDEX "IX_RepeatTransfer_RepeatConfigId" ON "RepeatTransfer" ("RepeatConfigId");

CREATE INDEX "IX_RepeatTransfer_SavingsGoalId" ON "RepeatTransfer" ("SavingsGoalId");

CREATE INDEX "IX_RepeatTransfer_TagId" ON "RepeatTransfer" ("TagId");

CREATE INDEX "IX_RepeatTransfer_ToAccountId" ON "RepeatTransfer" ("ToAccountId");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20240304182819_SavingsGoalsForRepeatTransfers', '8.0.2');

COMMIT;

BEGIN TRANSACTION;

CREATE TABLE "ef_temp_RepeatConfig" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_RepeatConfig" PRIMARY KEY,
    "FirstFiring" TEXT NOT NULL,
    "Frequency" INTEGER NOT NULL,
    "IntervalGap" INTEGER NULL,
    "LastFiring" TEXT NULL,
    "Mode" INTEGER NOT NULL,
    "TerminationDate" TEXT NULL
);

INSERT INTO "ef_temp_RepeatConfig" ("Id", "FirstFiring", "Frequency", "IntervalGap", "LastFiring", "Mode", "TerminationDate")
SELECT "Id", "FirstFiring", "Frequency", "IntervalGap", "LastFiring", "Mode", "TerminationDate"
FROM "RepeatConfig";

COMMIT;

PRAGMA foreign_keys = 0;

BEGIN TRANSACTION;

DROP TABLE "RepeatConfig";

ALTER TABLE "ef_temp_RepeatConfig" RENAME TO "RepeatConfig";

COMMIT;

PRAGMA foreign_keys = 1;

BEGIN TRANSACTION;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20240304184739_OptionalRepeatGap', '8.0.2');

COMMIT;

BEGIN TRANSACTION;

CREATE TABLE "JobStatus" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_JobStatus" PRIMARY KEY,
    "JobName" TEXT NULL,
    "StartTime" TEXT NOT NULL,
    "EndTime" TEXT NOT NULL,
    "Status" TEXT NULL,
    "StackTrace" TEXT NULL,
    "ErrorMessages" TEXT NULL,
    "NextRunTime" TEXT NOT NULL
);

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20240304190533_JobStatusTrackerTable', '8.0.2');

COMMIT;

BEGIN TRANSACTION;

DROP TABLE "RepeatTransfer";

ALTER TABLE "Transaction" ADD "TransferId" TEXT NULL;

CREATE TABLE "Transfer" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_Transfer" PRIMARY KEY,
    "FromAccountId" TEXT NOT NULL,
    "ToAccountId" TEXT NOT NULL,
    "Date" TEXT NOT NULL,
    "MerchantId" TEXT NOT NULL,
    "CategoryId" TEXT NOT NULL,
    "Amount" decimal(10, 2) NOT NULL,
    "TagId" TEXT NULL,
    "SavingsGoalId" TEXT NULL,
    "RepeatConfigId" TEXT NULL,
    CONSTRAINT "FK_Transfer_Account_FromAccountId" FOREIGN KEY ("FromAccountId") REFERENCES "Account" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_Transfer_Account_ToAccountId" FOREIGN KEY ("ToAccountId") REFERENCES "Account" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_Transfer_Category_CategoryId" FOREIGN KEY ("CategoryId") REFERENCES "Category" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_Transfer_Goals_SavingsGoalId" FOREIGN KEY ("SavingsGoalId") REFERENCES "Goals" ("Id"),
    CONSTRAINT "FK_Transfer_Merchant_MerchantId" FOREIGN KEY ("MerchantId") REFERENCES "Merchant" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_Transfer_RepeatConfig_RepeatConfigId" FOREIGN KEY ("RepeatConfigId") REFERENCES "RepeatConfig" ("Id"),
    CONSTRAINT "FK_Transfer_Tag_TagId" FOREIGN KEY ("TagId") REFERENCES "Tag" ("Id")
);

CREATE INDEX "IX_Transaction_TransferId" ON "Transaction" ("TransferId");

CREATE INDEX "IX_Transfer_CategoryId" ON "Transfer" ("CategoryId");

CREATE INDEX "IX_Transfer_FromAccountId" ON "Transfer" ("FromAccountId");

CREATE INDEX "IX_Transfer_MerchantId" ON "Transfer" ("MerchantId");

CREATE INDEX "IX_Transfer_RepeatConfigId" ON "Transfer" ("RepeatConfigId");

CREATE INDEX "IX_Transfer_SavingsGoalId" ON "Transfer" ("SavingsGoalId");

CREATE INDEX "IX_Transfer_TagId" ON "Transfer" ("TagId");

CREATE INDEX "IX_Transfer_ToAccountId" ON "Transfer" ("ToAccountId");

CREATE TABLE "ef_temp_Category" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_Category" PRIMARY KEY,
    "CategoryType" INTEGER NOT NULL,
    "Name" TEXT NULL,
    "ParentCategoryId" TEXT NULL,
    "ParentCategoryName" TEXT NULL,
    CONSTRAINT "FK_Category_Category_ParentCategoryId" FOREIGN KEY ("ParentCategoryId") REFERENCES "Category" ("Id") ON DELETE SET NULL
);

INSERT INTO "ef_temp_Category" ("Id", "CategoryType", "Name", "ParentCategoryId", "ParentCategoryName")
SELECT "Id", "CategoryType", "Name", "ParentCategoryId", "ParentCategoryName"
FROM "Category";

CREATE TABLE "ef_temp_Transaction" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_Transaction" PRIMARY KEY,
    "AccountId" TEXT NOT NULL,
    "AccountName" TEXT NULL,
    "Amount" decimal(10, 2) NOT NULL,
    "BillId" TEXT NULL,
    "CategoryId" TEXT NOT NULL,
    "CategoryName" TEXT NULL,
    "Date" TEXT NOT NULL,
    "DoNotSkip" INTEGER NOT NULL,
    "IsSavingsSpending" INTEGER NOT NULL,
    "MerchantId" TEXT NOT NULL,
    "MerchantName" TEXT NULL,
    "Notes" TEXT NULL,
    "SavingsGoalId" TEXT NULL,
    "SavingsGoalName" TEXT NULL,
    "TagId" TEXT NULL,
    "TransferId" TEXT NULL,
    CONSTRAINT "FK_Transaction_Account_AccountId" FOREIGN KEY ("AccountId") REFERENCES "Account" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_Transaction_Bill_BillId" FOREIGN KEY ("BillId") REFERENCES "Bill" ("Id") ON DELETE SET NULL,
    CONSTRAINT "FK_Transaction_Category_CategoryId" FOREIGN KEY ("CategoryId") REFERENCES "Category" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_Transaction_Goals_SavingsGoalId" FOREIGN KEY ("SavingsGoalId") REFERENCES "Goals" ("Id") ON DELETE SET NULL,
    CONSTRAINT "FK_Transaction_Merchant_MerchantId" FOREIGN KEY ("MerchantId") REFERENCES "Merchant" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_Transaction_Tag_TagId" FOREIGN KEY ("TagId") REFERENCES "Tag" ("Id"),
    CONSTRAINT "FK_Transaction_Transfer_TransferId" FOREIGN KEY ("TransferId") REFERENCES "Transfer" ("Id") ON DELETE SET NULL
);

INSERT INTO "ef_temp_Transaction" ("Id", "AccountId", "AccountName", "Amount", "BillId", "CategoryId", "CategoryName", "Date", "DoNotSkip", "IsSavingsSpending", "MerchantId", "MerchantName", "Notes", "SavingsGoalId", "SavingsGoalName", "TagId", "TransferId")
SELECT "Id", "AccountId", "AccountName", "Amount", "BillId", "CategoryId", "CategoryName", "Date", "DoNotSkip", "IsSavingsSpending", "MerchantId", "MerchantName", "Notes", "SavingsGoalId", "SavingsGoalName", "TagId", "TransferId"
FROM "Transaction";

COMMIT;

PRAGMA foreign_keys = 0;

BEGIN TRANSACTION;

DROP TABLE "Category";

ALTER TABLE "ef_temp_Category" RENAME TO "Category";

DROP TABLE "Transaction";

ALTER TABLE "ef_temp_Transaction" RENAME TO "Transaction";

COMMIT;

PRAGMA foreign_keys = 1;

BEGIN TRANSACTION;

CREATE UNIQUE INDEX "IX_Category_Name" ON "Category" ("Name");

CREATE INDEX "IX_Category_ParentCategoryId" ON "Category" ("ParentCategoryId");

CREATE INDEX "IX_Transaction_AccountId" ON "Transaction" ("AccountId");

CREATE INDEX "IX_Transaction_BillId" ON "Transaction" ("BillId");

CREATE INDEX "IX_Transaction_CategoryId" ON "Transaction" ("CategoryId");

CREATE INDEX "IX_Transaction_MerchantId" ON "Transaction" ("MerchantId");

CREATE INDEX "IX_Transaction_SavingsGoalId" ON "Transaction" ("SavingsGoalId");

CREATE INDEX "IX_Transaction_TagId" ON "Transaction" ("TagId");

CREATE INDEX "IX_Transaction_TransferId" ON "Transaction" ("TransferId");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20240304205902_LinkingTransferTransactions', '8.0.2');

COMMIT;

BEGIN TRANSACTION;

ALTER TABLE "Transaction" RENAME TO "BankTransaction";

ALTER TABLE "Goals" RENAME TO "Goal";

DROP INDEX "IX_Transaction_TransferId";

CREATE INDEX "IX_BankTransaction_TransferId" ON "BankTransaction" ("TransferId");

DROP INDEX "IX_Transaction_TagId";

CREATE INDEX "IX_BankTransaction_TagId" ON "BankTransaction" ("TagId");

DROP INDEX "IX_Transaction_SavingsGoalId";

CREATE INDEX "IX_BankTransaction_SavingsGoalId" ON "BankTransaction" ("SavingsGoalId");

DROP INDEX "IX_Transaction_MerchantId";

CREATE INDEX "IX_BankTransaction_MerchantId" ON "BankTransaction" ("MerchantId");

DROP INDEX "IX_Transaction_CategoryId";

CREATE INDEX "IX_BankTransaction_CategoryId" ON "BankTransaction" ("CategoryId");

DROP INDEX "IX_Transaction_BillId";

CREATE INDEX "IX_BankTransaction_BillId" ON "BankTransaction" ("BillId");

DROP INDEX "IX_Transaction_AccountId";

CREATE INDEX "IX_BankTransaction_AccountId" ON "BankTransaction" ("AccountId");

DROP INDEX "IX_Goals_Name";

CREATE UNIQUE INDEX "IX_Goal_Name" ON "Goal" ("Name");

DROP INDEX "IX_Goals_AccountId";

CREATE INDEX "IX_Goal_AccountId" ON "Goal" ("AccountId");

CREATE TABLE "ef_temp_Goal" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_Goal" PRIMARY KEY,
    "AccountId" TEXT NOT NULL,
    "AccountName" TEXT NULL,
    "Description" TEXT NULL,
    "Name" TEXT NULL,
    "SavedAmount" REAL NOT NULL,
    "StartDate" TEXT NOT NULL,
    "StartingAmount" REAL NULL,
    "TargetAmount" REAL NOT NULL,
    "TargetDate" TEXT NULL,
    CONSTRAINT "FK_Goal_Account_AccountId" FOREIGN KEY ("AccountId") REFERENCES "Account" ("Id") ON DELETE CASCADE
);

INSERT INTO "ef_temp_Goal" ("Id", "AccountId", "AccountName", "Description", "Name", "SavedAmount", "StartDate", "StartingAmount", "TargetAmount", "TargetDate")
SELECT "Id", "AccountId", "AccountName", "Description", "Name", "SavedAmount", "StartDate", "StartingAmount", "TargetAmount", "TargetDate"
FROM "Goal";

CREATE TABLE "ef_temp_BankTransaction" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_BankTransaction" PRIMARY KEY,
    "AccountId" TEXT NOT NULL,
    "AccountName" TEXT NULL,
    "Amount" decimal(10, 2) NOT NULL,
    "BillId" TEXT NULL,
    "CategoryId" TEXT NOT NULL,
    "CategoryName" TEXT NULL,
    "Date" TEXT NOT NULL,
    "DoNotSkip" INTEGER NOT NULL,
    "IsSavingsSpending" INTEGER NOT NULL,
    "MerchantId" TEXT NOT NULL,
    "MerchantName" TEXT NULL,
    "Notes" TEXT NULL,
    "SavingsGoalId" TEXT NULL,
    "SavingsGoalName" TEXT NULL,
    "TagId" TEXT NULL,
    "TransferId" TEXT NULL,
    CONSTRAINT "FK_BankTransaction_Account_AccountId" FOREIGN KEY ("AccountId") REFERENCES "Account" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_BankTransaction_Bill_BillId" FOREIGN KEY ("BillId") REFERENCES "Bill" ("Id") ON DELETE SET NULL,
    CONSTRAINT "FK_BankTransaction_Category_CategoryId" FOREIGN KEY ("CategoryId") REFERENCES "Category" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_BankTransaction_Goal_SavingsGoalId" FOREIGN KEY ("SavingsGoalId") REFERENCES "Goal" ("Id") ON DELETE SET NULL,
    CONSTRAINT "FK_BankTransaction_Merchant_MerchantId" FOREIGN KEY ("MerchantId") REFERENCES "Merchant" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_BankTransaction_Tag_TagId" FOREIGN KEY ("TagId") REFERENCES "Tag" ("Id"),
    CONSTRAINT "FK_BankTransaction_Transfer_TransferId" FOREIGN KEY ("TransferId") REFERENCES "Transfer" ("Id") ON DELETE SET NULL
);

INSERT INTO "ef_temp_BankTransaction" ("Id", "AccountId", "AccountName", "Amount", "BillId", "CategoryId", "CategoryName", "Date", "DoNotSkip", "IsSavingsSpending", "MerchantId", "MerchantName", "Notes", "SavingsGoalId", "SavingsGoalName", "TagId", "TransferId")
SELECT "Id", "AccountId", "AccountName", "Amount", "BillId", "CategoryId", "CategoryName", "Date", "DoNotSkip", "IsSavingsSpending", "MerchantId", "MerchantName", "Notes", "SavingsGoalId", "SavingsGoalName", "TagId", "TransferId"
FROM "BankTransaction";

CREATE TABLE "ef_temp_Transfer" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_Transfer" PRIMARY KEY,
    "Amount" decimal(10, 2) NOT NULL,
    "CategoryId" TEXT NOT NULL,
    "Date" TEXT NOT NULL,
    "FromAccountId" TEXT NOT NULL,
    "MerchantId" TEXT NOT NULL,
    "RepeatConfigId" TEXT NULL,
    "SavingsGoalId" TEXT NULL,
    "TagId" TEXT NULL,
    "ToAccountId" TEXT NOT NULL,
    CONSTRAINT "FK_Transfer_Account_FromAccountId" FOREIGN KEY ("FromAccountId") REFERENCES "Account" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_Transfer_Account_ToAccountId" FOREIGN KEY ("ToAccountId") REFERENCES "Account" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_Transfer_Category_CategoryId" FOREIGN KEY ("CategoryId") REFERENCES "Category" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_Transfer_Goal_SavingsGoalId" FOREIGN KEY ("SavingsGoalId") REFERENCES "Goal" ("Id"),
    CONSTRAINT "FK_Transfer_Merchant_MerchantId" FOREIGN KEY ("MerchantId") REFERENCES "Merchant" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_Transfer_RepeatConfig_RepeatConfigId" FOREIGN KEY ("RepeatConfigId") REFERENCES "RepeatConfig" ("Id"),
    CONSTRAINT "FK_Transfer_Tag_TagId" FOREIGN KEY ("TagId") REFERENCES "Tag" ("Id")
);

INSERT INTO "ef_temp_Transfer" ("Id", "Amount", "CategoryId", "Date", "FromAccountId", "MerchantId", "RepeatConfigId", "SavingsGoalId", "TagId", "ToAccountId")
SELECT "Id", "Amount", "CategoryId", "Date", "FromAccountId", "MerchantId", "RepeatConfigId", "SavingsGoalId", "TagId", "ToAccountId"
FROM "Transfer";

COMMIT;

PRAGMA foreign_keys = 0;

BEGIN TRANSACTION;

DROP TABLE "Goal";

ALTER TABLE "ef_temp_Goal" RENAME TO "Goal";

DROP TABLE "BankTransaction";

ALTER TABLE "ef_temp_BankTransaction" RENAME TO "BankTransaction";

DROP TABLE "Transfer";

ALTER TABLE "ef_temp_Transfer" RENAME TO "Transfer";

COMMIT;

PRAGMA foreign_keys = 1;

BEGIN TRANSACTION;

CREATE INDEX "IX_Goal_AccountId" ON "Goal" ("AccountId");

CREATE UNIQUE INDEX "IX_Goal_Name" ON "Goal" ("Name");

CREATE INDEX "IX_BankTransaction_AccountId" ON "BankTransaction" ("AccountId");

CREATE INDEX "IX_BankTransaction_BillId" ON "BankTransaction" ("BillId");

CREATE INDEX "IX_BankTransaction_CategoryId" ON "BankTransaction" ("CategoryId");

CREATE INDEX "IX_BankTransaction_MerchantId" ON "BankTransaction" ("MerchantId");

CREATE INDEX "IX_BankTransaction_SavingsGoalId" ON "BankTransaction" ("SavingsGoalId");

CREATE INDEX "IX_BankTransaction_TagId" ON "BankTransaction" ("TagId");

CREATE INDEX "IX_BankTransaction_TransferId" ON "BankTransaction" ("TransferId");

CREATE INDEX "IX_Transfer_CategoryId" ON "Transfer" ("CategoryId");

CREATE INDEX "IX_Transfer_FromAccountId" ON "Transfer" ("FromAccountId");

CREATE INDEX "IX_Transfer_MerchantId" ON "Transfer" ("MerchantId");

CREATE INDEX "IX_Transfer_RepeatConfigId" ON "Transfer" ("RepeatConfigId");

CREATE INDEX "IX_Transfer_SavingsGoalId" ON "Transfer" ("SavingsGoalId");

CREATE INDEX "IX_Transfer_TagId" ON "Transfer" ("TagId");

CREATE INDEX "IX_Transfer_ToAccountId" ON "Transfer" ("ToAccountId");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20240306221430_ChangingTableNames', '8.0.2');

COMMIT;

BEGIN TRANSACTION;

ALTER TABLE "BankTransaction" ADD "ParentTransactionId" TEXT NULL;

CREATE INDEX "IX_BankTransaction_ParentTransactionId" ON "BankTransaction" ("ParentTransactionId");

CREATE TABLE "ef_temp_BankTransaction" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_BankTransaction" PRIMARY KEY,
    "AccountId" TEXT NOT NULL,
    "AccountName" TEXT NULL,
    "Amount" decimal(10, 2) NOT NULL,
    "BillId" TEXT NULL,
    "CategoryId" TEXT NOT NULL,
    "CategoryName" TEXT NULL,
    "Date" TEXT NOT NULL,
    "DoNotSkip" INTEGER NOT NULL,
    "IsSavingsSpending" INTEGER NOT NULL,
    "MerchantId" TEXT NOT NULL,
    "MerchantName" TEXT NULL,
    "Notes" TEXT NULL,
    "ParentTransactionId" TEXT NULL,
    "SavingsGoalId" TEXT NULL,
    "SavingsGoalName" TEXT NULL,
    "TagId" TEXT NULL,
    "TransferId" TEXT NULL,
    CONSTRAINT "FK_BankTransaction_Account_AccountId" FOREIGN KEY ("AccountId") REFERENCES "Account" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_BankTransaction_BankTransaction_ParentTransactionId" FOREIGN KEY ("ParentTransactionId") REFERENCES "BankTransaction" ("Id"),
    CONSTRAINT "FK_BankTransaction_Bill_BillId" FOREIGN KEY ("BillId") REFERENCES "Bill" ("Id") ON DELETE SET NULL,
    CONSTRAINT "FK_BankTransaction_Category_CategoryId" FOREIGN KEY ("CategoryId") REFERENCES "Category" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_BankTransaction_Goal_SavingsGoalId" FOREIGN KEY ("SavingsGoalId") REFERENCES "Goal" ("Id") ON DELETE SET NULL,
    CONSTRAINT "FK_BankTransaction_Merchant_MerchantId" FOREIGN KEY ("MerchantId") REFERENCES "Merchant" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_BankTransaction_Tag_TagId" FOREIGN KEY ("TagId") REFERENCES "Tag" ("Id"),
    CONSTRAINT "FK_BankTransaction_Transfer_TransferId" FOREIGN KEY ("TransferId") REFERENCES "Transfer" ("Id") ON DELETE SET NULL
);

INSERT INTO "ef_temp_BankTransaction" ("Id", "AccountId", "AccountName", "Amount", "BillId", "CategoryId", "CategoryName", "Date", "DoNotSkip", "IsSavingsSpending", "MerchantId", "MerchantName", "Notes", "ParentTransactionId", "SavingsGoalId", "SavingsGoalName", "TagId", "TransferId")
SELECT "Id", "AccountId", "AccountName", "Amount", "BillId", "CategoryId", "CategoryName", "Date", "DoNotSkip", "IsSavingsSpending", "MerchantId", "MerchantName", "Notes", "ParentTransactionId", "SavingsGoalId", "SavingsGoalName", "TagId", "TransferId"
FROM "BankTransaction";

COMMIT;

PRAGMA foreign_keys = 0;

BEGIN TRANSACTION;

DROP TABLE "BankTransaction";

ALTER TABLE "ef_temp_BankTransaction" RENAME TO "BankTransaction";

COMMIT;

PRAGMA foreign_keys = 1;

BEGIN TRANSACTION;

CREATE INDEX "IX_BankTransaction_AccountId" ON "BankTransaction" ("AccountId");

CREATE INDEX "IX_BankTransaction_BillId" ON "BankTransaction" ("BillId");

CREATE INDEX "IX_BankTransaction_CategoryId" ON "BankTransaction" ("CategoryId");

CREATE INDEX "IX_BankTransaction_MerchantId" ON "BankTransaction" ("MerchantId");

CREATE INDEX "IX_BankTransaction_ParentTransactionId" ON "BankTransaction" ("ParentTransactionId");

CREATE INDEX "IX_BankTransaction_SavingsGoalId" ON "BankTransaction" ("SavingsGoalId");

CREATE INDEX "IX_BankTransaction_TagId" ON "BankTransaction" ("TagId");

CREATE INDEX "IX_BankTransaction_TransferId" ON "BankTransaction" ("TransferId");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20240306221600_SplitTransactions', '8.0.2');

COMMIT;

BEGIN TRANSACTION;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20240307171943_OptionalCategory', '8.0.2');

COMMIT;

BEGIN TRANSACTION;

CREATE TABLE "ef_temp_BankTransaction" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_BankTransaction" PRIMARY KEY,
    "AccountId" TEXT NOT NULL,
    "AccountName" TEXT NULL,
    "Amount" decimal(10, 2) NOT NULL,
    "BillId" TEXT NULL,
    "CategoryId" TEXT NULL,
    "CategoryName" TEXT NULL,
    "Date" TEXT NOT NULL,
    "DoNotSkip" INTEGER NOT NULL,
    "IsSavingsSpending" INTEGER NOT NULL,
    "MerchantId" TEXT NOT NULL,
    "MerchantName" TEXT NULL,
    "Notes" TEXT NULL,
    "ParentTransactionId" TEXT NULL,
    "SavingsGoalId" TEXT NULL,
    "SavingsGoalName" TEXT NULL,
    "TagId" TEXT NULL,
    "TransferId" TEXT NULL,
    CONSTRAINT "FK_BankTransaction_Account_AccountId" FOREIGN KEY ("AccountId") REFERENCES "Account" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_BankTransaction_BankTransaction_ParentTransactionId" FOREIGN KEY ("ParentTransactionId") REFERENCES "BankTransaction" ("Id"),
    CONSTRAINT "FK_BankTransaction_Bill_BillId" FOREIGN KEY ("BillId") REFERENCES "Bill" ("Id") ON DELETE SET NULL,
    CONSTRAINT "FK_BankTransaction_Category_CategoryId" FOREIGN KEY ("CategoryId") REFERENCES "Category" ("Id"),
    CONSTRAINT "FK_BankTransaction_Goal_SavingsGoalId" FOREIGN KEY ("SavingsGoalId") REFERENCES "Goal" ("Id") ON DELETE SET NULL,
    CONSTRAINT "FK_BankTransaction_Merchant_MerchantId" FOREIGN KEY ("MerchantId") REFERENCES "Merchant" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_BankTransaction_Tag_TagId" FOREIGN KEY ("TagId") REFERENCES "Tag" ("Id"),
    CONSTRAINT "FK_BankTransaction_Transfer_TransferId" FOREIGN KEY ("TransferId") REFERENCES "Transfer" ("Id") ON DELETE SET NULL
);

INSERT INTO "ef_temp_BankTransaction" ("Id", "AccountId", "AccountName", "Amount", "BillId", "CategoryId", "CategoryName", "Date", "DoNotSkip", "IsSavingsSpending", "MerchantId", "MerchantName", "Notes", "ParentTransactionId", "SavingsGoalId", "SavingsGoalName", "TagId", "TransferId")
SELECT "Id", "AccountId", "AccountName", "Amount", "BillId", "CategoryId", "CategoryName", "Date", "DoNotSkip", "IsSavingsSpending", "MerchantId", "MerchantName", "Notes", "ParentTransactionId", "SavingsGoalId", "SavingsGoalName", "TagId", "TransferId"
FROM "BankTransaction";

COMMIT;

PRAGMA foreign_keys = 0;

BEGIN TRANSACTION;

DROP TABLE "BankTransaction";

ALTER TABLE "ef_temp_BankTransaction" RENAME TO "BankTransaction";

COMMIT;

PRAGMA foreign_keys = 1;

BEGIN TRANSACTION;

CREATE INDEX "IX_BankTransaction_AccountId" ON "BankTransaction" ("AccountId");

CREATE INDEX "IX_BankTransaction_BillId" ON "BankTransaction" ("BillId");

CREATE INDEX "IX_BankTransaction_CategoryId" ON "BankTransaction" ("CategoryId");

CREATE INDEX "IX_BankTransaction_MerchantId" ON "BankTransaction" ("MerchantId");

CREATE INDEX "IX_BankTransaction_ParentTransactionId" ON "BankTransaction" ("ParentTransactionId");

CREATE INDEX "IX_BankTransaction_SavingsGoalId" ON "BankTransaction" ("SavingsGoalId");

CREATE INDEX "IX_BankTransaction_TagId" ON "BankTransaction" ("TagId");

CREATE INDEX "IX_BankTransaction_TransferId" ON "BankTransaction" ("TransferId");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20240307175730_OptionalCategory2', '8.0.2');

COMMIT;

BEGIN TRANSACTION;

ALTER TABLE "BankTransaction" ADD "IsSplit" INTEGER NOT NULL DEFAULT 0;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20240307181724_AddingSplitFlag', '8.0.2');

COMMIT;

BEGIN TRANSACTION;

CREATE TABLE "ef_temp_BankTransaction" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_BankTransaction" PRIMARY KEY,
    "AccountId" TEXT NOT NULL,
    "AccountName" TEXT NULL,
    "Amount" decimal(10, 2) NOT NULL,
    "BillId" TEXT NULL,
    "CategoryId" TEXT NULL,
    "CategoryName" TEXT NULL,
    "Date" TEXT NOT NULL,
    "DoNotSkip" INTEGER NOT NULL,
    "IsSavingsSpending" INTEGER NOT NULL,
    "IsSplit" INTEGER NOT NULL,
    "MerchantId" TEXT NOT NULL,
    "MerchantName" TEXT NULL,
    "Notes" TEXT NULL,
    "ParentTransactionId" TEXT NULL,
    "SavingsGoalId" TEXT NULL,
    "SavingsGoalName" TEXT NULL,
    "TagId" TEXT NULL,
    "TransferId" TEXT NULL,
    CONSTRAINT "FK_BankTransaction_Account_AccountId" FOREIGN KEY ("AccountId") REFERENCES "Account" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_BankTransaction_BankTransaction_ParentTransactionId" FOREIGN KEY ("ParentTransactionId") REFERENCES "BankTransaction" ("Id"),
    CONSTRAINT "FK_BankTransaction_Bill_BillId" FOREIGN KEY ("BillId") REFERENCES "Bill" ("Id") ON DELETE SET NULL,
    CONSTRAINT "FK_BankTransaction_Category_CategoryId" FOREIGN KEY ("CategoryId") REFERENCES "Category" ("Id"),
    CONSTRAINT "FK_BankTransaction_Goal_SavingsGoalId" FOREIGN KEY ("SavingsGoalId") REFERENCES "Goal" ("Id") ON DELETE SET NULL,
    CONSTRAINT "FK_BankTransaction_Merchant_MerchantId" FOREIGN KEY ("MerchantId") REFERENCES "Merchant" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_BankTransaction_Tag_TagId" FOREIGN KEY ("TagId") REFERENCES "Tag" ("Id") ON DELETE SET NULL,
    CONSTRAINT "FK_BankTransaction_Transfer_TransferId" FOREIGN KEY ("TransferId") REFERENCES "Transfer" ("Id") ON DELETE SET NULL
);

INSERT INTO "ef_temp_BankTransaction" ("Id", "AccountId", "AccountName", "Amount", "BillId", "CategoryId", "CategoryName", "Date", "DoNotSkip", "IsSavingsSpending", "IsSplit", "MerchantId", "MerchantName", "Notes", "ParentTransactionId", "SavingsGoalId", "SavingsGoalName", "TagId", "TransferId")
SELECT "Id", "AccountId", "AccountName", "Amount", "BillId", "CategoryId", "CategoryName", "Date", "DoNotSkip", "IsSavingsSpending", "IsSplit", "MerchantId", "MerchantName", "Notes", "ParentTransactionId", "SavingsGoalId", "SavingsGoalName", "TagId", "TransferId"
FROM "BankTransaction";

COMMIT;

PRAGMA foreign_keys = 0;

BEGIN TRANSACTION;

DROP TABLE "BankTransaction";

ALTER TABLE "ef_temp_BankTransaction" RENAME TO "BankTransaction";

COMMIT;

PRAGMA foreign_keys = 1;

BEGIN TRANSACTION;

CREATE INDEX "IX_BankTransaction_AccountId" ON "BankTransaction" ("AccountId");

CREATE INDEX "IX_BankTransaction_BillId" ON "BankTransaction" ("BillId");

CREATE INDEX "IX_BankTransaction_CategoryId" ON "BankTransaction" ("CategoryId");

CREATE INDEX "IX_BankTransaction_MerchantId" ON "BankTransaction" ("MerchantId");

CREATE INDEX "IX_BankTransaction_ParentTransactionId" ON "BankTransaction" ("ParentTransactionId");

CREATE INDEX "IX_BankTransaction_SavingsGoalId" ON "BankTransaction" ("SavingsGoalId");

CREATE INDEX "IX_BankTransaction_TagId" ON "BankTransaction" ("TagId");

CREATE INDEX "IX_BankTransaction_TransferId" ON "BankTransaction" ("TransferId");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20240308143037_TagsUpdate', '8.0.2');

COMMIT;

