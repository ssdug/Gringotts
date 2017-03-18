ALTER DATABASE CURRENT SET READ_COMMITTED_SNAPSHOT ON
ALTER DATABASE CURRENT SET ALLOW_SNAPSHOT_ISOLATION ON
ALTER TABLE dbo.Clients DROP COLUMN DisplayName
DELETE [dbo].[__MigrationHistory]
WHERE (([MigrationId] = N'201601052103167_AddDisplayNameToClient') AND ([ContextKey] = N'Wiz.Gringotts.UIWeb.Helpers.TestConfiguration'))
ALTER DATABASE CURRENT SET READ_COMMITTED_SNAPSHOT ON
ALTER DATABASE CURRENT SET ALLOW_SNAPSHOT_ISOLATION ON
DECLARE @var0 nvarchar(128)
SELECT @var0 = name
FROM sys.default_constraints
WHERE parent_object_id = object_id(N'dbo.Payees')
AND col_name(parent_object_id, parent_column_id) = 'IsUserSelectable';
IF @var0 IS NOT NULL
    EXECUTE('ALTER TABLE [dbo].[Payees] DROP CONSTRAINT [' + @var0 + ']')
ALTER TABLE [dbo].[Payees] DROP COLUMN [IsUserSelectable]
DELETE [dbo].[__MigrationHistory]
WHERE (([MigrationId] = N'201512311813520_AddUserSelectableFlagToPayee') AND ([ContextKey] = N'Wiz.Gringotts.UIWeb.Helpers.TestConfiguration'))
ALTER DATABASE CURRENT SET READ_COMMITTED_SNAPSHOT ON
ALTER DATABASE CURRENT SET ALLOW_SNAPSHOT_ISOLATION ON
IF object_id(N'[dbo].[FK_dbo.TransactionBatches_dbo.Transactions_TriggerId_TriggerAccountId]', N'F') IS NOT NULL
    ALTER TABLE [dbo].[TransactionBatches] DROP CONSTRAINT [FK_dbo.TransactionBatches_dbo.Transactions_TriggerId_TriggerAccountId]
IF object_id(N'[dbo].[FK_dbo.TransactionBatches_dbo.TransactionBatches_ParentId]', N'F') IS NOT NULL
    ALTER TABLE [dbo].[TransactionBatches] DROP CONSTRAINT [FK_dbo.TransactionBatches_dbo.TransactionBatches_ParentId]
IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_ParentId' AND object_id = object_id(N'[dbo].[TransactionBatches]', N'U'))
    DROP INDEX [IX_ParentId] ON [dbo].[TransactionBatches]
IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_TriggerId_TriggerAccountId' AND object_id = object_id(N'[dbo].[TransactionBatches]', N'U'))
    DROP INDEX [IX_TriggerId_TriggerAccountId] ON [dbo].[TransactionBatches]
DECLARE @var1 nvarchar(128)
SELECT @var1 = name
FROM sys.default_constraints
WHERE parent_object_id = object_id(N'dbo.TransactionBatches')
AND col_name(parent_object_id, parent_column_id) = 'ParentId';
IF @var1 IS NOT NULL
    EXECUTE('ALTER TABLE [dbo].[TransactionBatches] DROP CONSTRAINT [' + @var1 + ']')
ALTER TABLE [dbo].[TransactionBatches] DROP COLUMN [ParentId]
DECLARE @var2 nvarchar(128)
SELECT @var2 = name
FROM sys.default_constraints
WHERE parent_object_id = object_id(N'dbo.TransactionBatches')
AND col_name(parent_object_id, parent_column_id) = 'TriggerAccountId';
IF @var2 IS NOT NULL
    EXECUTE('ALTER TABLE [dbo].[TransactionBatches] DROP CONSTRAINT [' + @var2 + ']')
ALTER TABLE [dbo].[TransactionBatches] DROP COLUMN [TriggerAccountId]
DECLARE @var3 nvarchar(128)
SELECT @var3 = name
FROM sys.default_constraints
WHERE parent_object_id = object_id(N'dbo.TransactionBatches')
AND col_name(parent_object_id, parent_column_id) = 'TriggerId';
IF @var3 IS NOT NULL
    EXECUTE('ALTER TABLE [dbo].[TransactionBatches] DROP CONSTRAINT [' + @var3 + ']')
ALTER TABLE [dbo].[TransactionBatches] DROP COLUMN [TriggerId]
DELETE [dbo].[__MigrationHistory]
WHERE (([MigrationId] = N'201512152031053_AddParentChildrenToBatch') AND ([ContextKey] = N'Wiz.Gringotts.UIWeb.Helpers.TestConfiguration'))
ALTER DATABASE CURRENT SET READ_COMMITTED_SNAPSHOT ON
ALTER DATABASE CURRENT SET ALLOW_SNAPSHOT_ISOLATION ON
IF object_id(N'[dbo].[FK_dbo.Checks_dbo.TransactionBatches_TransactionBatchId]', N'F') IS NOT NULL
    ALTER TABLE [dbo].[Checks] DROP CONSTRAINT [FK_dbo.Checks_dbo.TransactionBatches_TransactionBatchId]
IF object_id(N'[dbo].[FK_dbo.Checks_dbo.Accounts_AccountId]', N'F') IS NOT NULL
    ALTER TABLE [dbo].[Checks] DROP CONSTRAINT [FK_dbo.Checks_dbo.Accounts_AccountId]
IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_TransactionBatchId' AND object_id = object_id(N'[dbo].[Checks]', N'U'))
    DROP INDEX [IX_TransactionBatchId] ON [dbo].[Checks]
IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_TransactionId_AccountId' AND object_id = object_id(N'[dbo].[Checks]', N'U'))
    DROP INDEX [IX_TransactionId_AccountId] ON [dbo].[Checks]
IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_AccountId' AND object_id = object_id(N'[dbo].[Checks]', N'U'))
    DROP INDEX [IX_AccountId] ON [dbo].[Checks]
ALTER TABLE [dbo].[Checks] ALTER COLUMN [AccountId] [int] NOT NULL
ALTER TABLE [dbo].[Checks] ALTER COLUMN [TransactionId] [int] NOT NULL
DECLARE @var4 nvarchar(128)
SELECT @var4 = name
FROM sys.default_constraints
WHERE parent_object_id = object_id(N'dbo.Checks')
AND col_name(parent_object_id, parent_column_id) = 'TransactionBatchId';
IF @var4 IS NOT NULL
    EXECUTE('ALTER TABLE [dbo].[Checks] DROP CONSTRAINT [' + @var4 + ']')
ALTER TABLE [dbo].[Checks] DROP COLUMN [TransactionBatchId]
DECLARE @var5 nvarchar(128)
SELECT @var5 = name
FROM sys.default_constraints
WHERE parent_object_id = object_id(N'dbo.Checks')
AND col_name(parent_object_id, parent_column_id) = 'Memo';
IF @var5 IS NOT NULL
    EXECUTE('ALTER TABLE [dbo].[Checks] DROP CONSTRAINT [' + @var5 + ']')
ALTER TABLE [dbo].[Checks] DROP COLUMN [Memo]
CREATE INDEX [IX_TransactionId_AccountId] ON [dbo].[Checks]([TransactionId], [AccountId])
DELETE [dbo].[__MigrationHistory]
WHERE (([MigrationId] = N'201511172046029_AddTransBatchAndMemoToCheck') AND ([ContextKey] = N'Wiz.Gringotts.UIWeb.Helpers.TestConfiguration'))
ALTER DATABASE CURRENT SET READ_COMMITTED_SNAPSHOT ON
ALTER DATABASE CURRENT SET ALLOW_SNAPSHOT_ISOLATION ON
IF object_id(N'[dbo].[FK_dbo.Checks_dbo.Funds_FundId]', N'F') IS NOT NULL
    ALTER TABLE [dbo].[Checks] DROP CONSTRAINT [FK_dbo.Checks_dbo.Funds_FundId]
IF object_id(N'[dbo].[FK_dbo.Checks_dbo.Transactions_TransactionId_AccountId]', N'F') IS NOT NULL
    ALTER TABLE [dbo].[Checks] DROP CONSTRAINT [FK_dbo.Checks_dbo.Transactions_TransactionId_AccountId]
IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_TransactionId_AccountId' AND object_id = object_id(N'[dbo].[Checks]', N'U'))
    DROP INDEX [IX_TransactionId_AccountId] ON [dbo].[Checks]
IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_FundId' AND object_id = object_id(N'[dbo].[Checks]', N'U'))
    DROP INDEX [IX_FundId] ON [dbo].[Checks]
DROP TABLE [dbo].[Checks]
DELETE [dbo].[__MigrationHistory]
WHERE (([MigrationId] = N'201511041914384_AddChecks') AND ([ContextKey] = N'Wiz.Gringotts.UIWeb.Helpers.TestConfiguration'))
ALTER DATABASE CURRENT SET READ_COMMITTED_SNAPSHOT ON
ALTER DATABASE CURRENT SET ALLOW_SNAPSHOT_ISOLATION ON
IF object_id(N'[dbo].[FK_dbo.Transactions_dbo.ReceiptSources_ReceiptSourceId]', N'F') IS NOT NULL
    ALTER TABLE [dbo].[Transactions] DROP CONSTRAINT [FK_dbo.Transactions_dbo.ReceiptSources_ReceiptSourceId]
IF object_id(N'[dbo].[FK_dbo.ReceiptSources_dbo.Organizations_OrganizationId]', N'F') IS NOT NULL
    ALTER TABLE [dbo].[ReceiptSources] DROP CONSTRAINT [FK_dbo.ReceiptSources_dbo.Organizations_OrganizationId]
IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_ReceiptSourceNameOrganizationId' AND object_id = object_id(N'[dbo].[ReceiptSources]', N'U'))
    DROP INDEX [IX_ReceiptSourceNameOrganizationId] ON [dbo].[ReceiptSources]
IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_ReceiptSourceId' AND object_id = object_id(N'[dbo].[Transactions]', N'U'))
    DROP INDEX [IX_ReceiptSourceId] ON [dbo].[Transactions]
DECLARE @var6 nvarchar(128)
SELECT @var6 = name
FROM sys.default_constraints
WHERE parent_object_id = object_id(N'dbo.Transactions')
AND col_name(parent_object_id, parent_column_id) = 'ReceiptSourceId';
IF @var6 IS NOT NULL
    EXECUTE('ALTER TABLE [dbo].[Transactions] DROP CONSTRAINT [' + @var6 + ']')
ALTER TABLE [dbo].[Transactions] DROP COLUMN [ReceiptSourceId]
DROP TABLE [dbo].[ReceiptSources]
DELETE [dbo].[__MigrationHistory]
WHERE (([MigrationId] = N'201511022148520_AddReceiptSource') AND ([ContextKey] = N'Wiz.Gringotts.UIWeb.Helpers.TestConfiguration'))
ALTER DATABASE CURRENT SET READ_COMMITTED_SNAPSHOT ON
ALTER DATABASE CURRENT SET ALLOW_SNAPSHOT_ISOLATION ON
ALTER TABLE [dbo].[Transactions] DROP CONSTRAINT [PK_dbo.Transactions]
ALTER TABLE [dbo].[Transactions] ADD CONSTRAINT [PK_dbo.Transactions] PRIMARY KEY ([TransactionId])
DELETE [dbo].[__MigrationHistory]
WHERE (([MigrationId] = N'201511011746349_UpdateTransactionPK') AND ([ContextKey] = N'Wiz.Gringotts.UIWeb.Helpers.TestConfiguration'))
ALTER DATABASE CURRENT SET READ_COMMITTED_SNAPSHOT ON
ALTER DATABASE CURRENT SET ALLOW_SNAPSHOT_ISOLATION ON
ALTER TABLE [dbo].[TransactionBatches] ADD [ExpenseCategoryId] [int]
DECLARE @var7 nvarchar(128)
SELECT @var7 = name
FROM sys.default_constraints
WHERE parent_object_id = object_id(N'dbo.Transactions')
AND col_name(parent_object_id, parent_column_id) = 'Comments';
IF @var7 IS NOT NULL
    EXECUTE('ALTER TABLE [dbo].[Transactions] DROP CONSTRAINT [' + @var7 + ']')
ALTER TABLE [dbo].[Transactions] DROP COLUMN [Comments]
CREATE INDEX [IX_ExpenseCategoryId] ON [dbo].[TransactionBatches]([ExpenseCategoryId])
ALTER TABLE [dbo].[TransactionBatches] ADD CONSTRAINT [FK_dbo.TransactionBatches_dbo.ExpenseCategories_ExpenseCategoryId] FOREIGN KEY ([ExpenseCategoryId]) REFERENCES [dbo].[ExpenseCategories] ([ExpenseCategoryId])
DELETE [dbo].[__MigrationHistory]
WHERE (([MigrationId] = N'201510282200263_AddCommentsToTransaction') AND ([ContextKey] = N'Wiz.Gringotts.UIWeb.Helpers.TestConfiguration'))
ALTER DATABASE CURRENT SET READ_COMMITTED_SNAPSHOT ON
ALTER DATABASE CURRENT SET ALLOW_SNAPSHOT_ISOLATION ON
ALTER TABLE [dbo].[Transactions] ADD [Committed] [bit] NOT NULL DEFAULT 0
IF object_id(N'[dbo].[FK_dbo.TransactionBatches_dbo.Accounts_ToAccountId]', N'F') IS NOT NULL
    ALTER TABLE [dbo].[TransactionBatches] DROP CONSTRAINT [FK_dbo.TransactionBatches_dbo.Accounts_ToAccountId]
IF object_id(N'[dbo].[FK_dbo.TransactionBatches_dbo.Accounts_FromAccountId]', N'F') IS NOT NULL
    ALTER TABLE [dbo].[TransactionBatches] DROP CONSTRAINT [FK_dbo.TransactionBatches_dbo.Accounts_FromAccountId]
IF object_id(N'[dbo].[FK_dbo.TransactionBatches_dbo.Payees_PayeeId]', N'F') IS NOT NULL
    ALTER TABLE [dbo].[TransactionBatches] DROP CONSTRAINT [FK_dbo.TransactionBatches_dbo.Payees_PayeeId]
IF object_id(N'[dbo].[FK_dbo.TransactionBatches_dbo.ExpenseCategories_ExpenseCategoryId]', N'F') IS NOT NULL
    ALTER TABLE [dbo].[TransactionBatches] DROP CONSTRAINT [FK_dbo.TransactionBatches_dbo.ExpenseCategories_ExpenseCategoryId]
IF object_id(N'[dbo].[FK_dbo.TransactionBatches_dbo.TransactionSubTypes_TransactionSubTypeId]', N'F') IS NOT NULL
    ALTER TABLE [dbo].[TransactionBatches] DROP CONSTRAINT [FK_dbo.TransactionBatches_dbo.TransactionSubTypes_TransactionSubTypeId]
IF object_id(N'[dbo].[FK_dbo.Transactions_dbo.TransactionBatches_TransactionBatchId]', N'F') IS NOT NULL
    ALTER TABLE [dbo].[Transactions] DROP CONSTRAINT [FK_dbo.Transactions_dbo.TransactionBatches_TransactionBatchId]
IF object_id(N'[dbo].[FK_dbo.TransactionBatches_dbo.Organizations_OrganizationId]', N'F') IS NOT NULL
    ALTER TABLE [dbo].[TransactionBatches] DROP CONSTRAINT [FK_dbo.TransactionBatches_dbo.Organizations_OrganizationId]
IF object_id(N'[dbo].[FK_dbo.TransactionBatches_dbo.Funds_FundId]', N'F') IS NOT NULL
    ALTER TABLE [dbo].[TransactionBatches] DROP CONSTRAINT [FK_dbo.TransactionBatches_dbo.Funds_FundId]
IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_ToAccountId' AND object_id = object_id(N'[dbo].[TransactionBatches]', N'U'))
    DROP INDEX [IX_ToAccountId] ON [dbo].[TransactionBatches]
IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_FromAccountId' AND object_id = object_id(N'[dbo].[TransactionBatches]', N'U'))
    DROP INDEX [IX_FromAccountId] ON [dbo].[TransactionBatches]
IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_ExpenseCategoryId' AND object_id = object_id(N'[dbo].[TransactionBatches]', N'U'))
    DROP INDEX [IX_ExpenseCategoryId] ON [dbo].[TransactionBatches]
IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_PayeeId' AND object_id = object_id(N'[dbo].[TransactionBatches]', N'U'))
    DROP INDEX [IX_PayeeId] ON [dbo].[TransactionBatches]
IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_OrganizationId' AND object_id = object_id(N'[dbo].[TransactionBatches]', N'U'))
    DROP INDEX [IX_OrganizationId] ON [dbo].[TransactionBatches]
IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_FundId' AND object_id = object_id(N'[dbo].[TransactionBatches]', N'U'))
    DROP INDEX [IX_FundId] ON [dbo].[TransactionBatches]
IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_TransactionSubTypeId' AND object_id = object_id(N'[dbo].[TransactionBatches]', N'U'))
    DROP INDEX [IX_TransactionSubTypeId] ON [dbo].[TransactionBatches]
IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_TransactionBatchId' AND object_id = object_id(N'[dbo].[Transactions]', N'U'))
    DROP INDEX [IX_TransactionBatchId] ON [dbo].[Transactions]
DECLARE @var8 nvarchar(128)
SELECT @var8 = name
FROM sys.default_constraints
WHERE parent_object_id = object_id(N'dbo.Transactions')
AND col_name(parent_object_id, parent_column_id) = 'TransactionBatchId';
IF @var8 IS NOT NULL
    EXECUTE('ALTER TABLE [dbo].[Transactions] DROP CONSTRAINT [' + @var8 + ']')
ALTER TABLE [dbo].[Transactions] DROP COLUMN [TransactionBatchId]
DECLARE @var9 nvarchar(128)
SELECT @var9 = name
FROM sys.default_constraints
WHERE parent_object_id = object_id(N'dbo.Transactions')
AND col_name(parent_object_id, parent_column_id) = 'BatchReferenceNumber';
IF @var9 IS NOT NULL
    EXECUTE('ALTER TABLE [dbo].[Transactions] DROP CONSTRAINT [' + @var9 + ']')
ALTER TABLE [dbo].[Transactions] DROP COLUMN [BatchReferenceNumber]
DROP TABLE [dbo].[TransactionBatches]
DELETE [dbo].[__MigrationHistory]
WHERE (([MigrationId] = N'201510201947104_AddTransactionBatches') AND ([ContextKey] = N'Wiz.Gringotts.UIWeb.Helpers.TestConfiguration'))
ALTER DATABASE CURRENT SET READ_COMMITTED_SNAPSHOT ON
ALTER DATABASE CURRENT SET ALLOW_SNAPSHOT_ISOLATION ON
DECLARE @var10 nvarchar(128)
SELECT @var10 = name
FROM sys.default_constraints
WHERE parent_object_id = object_id(N'dbo.ExpenseCategories')
AND col_name(parent_object_id, parent_column_id) = 'IsActive';
IF @var10 IS NOT NULL
    EXECUTE('ALTER TABLE [dbo].[ExpenseCategories] DROP CONSTRAINT [' + @var10 + ']')
ALTER TABLE [dbo].[ExpenseCategories] DROP COLUMN [IsActive]
DELETE [dbo].[__MigrationHistory]
WHERE (([MigrationId] = N'201510161811171_AddActiveToExpenseCategory') AND ([ContextKey] = N'Wiz.Gringotts.UIWeb.Helpers.TestConfiguration'))
ALTER DATABASE CURRENT SET READ_COMMITTED_SNAPSHOT ON
ALTER DATABASE CURRENT SET ALLOW_SNAPSHOT_ISOLATION ON
IF object_id(N'[dbo].[FK_dbo.Transactions_dbo.ExpenseCategories_ExpenseCategoryId]', N'F') IS NOT NULL
    ALTER TABLE [dbo].[Transactions] DROP CONSTRAINT [FK_dbo.Transactions_dbo.ExpenseCategories_ExpenseCategoryId]
IF object_id(N'[dbo].[FK_dbo.ExpenseCategories_dbo.Organizations_OrganizationId]', N'F') IS NOT NULL
    ALTER TABLE [dbo].[ExpenseCategories] DROP CONSTRAINT [FK_dbo.ExpenseCategories_dbo.Organizations_OrganizationId]
IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_ExpenseCategoryNameOrganizationId' AND object_id = object_id(N'[dbo].[ExpenseCategories]', N'U'))
    DROP INDEX [IX_ExpenseCategoryNameOrganizationId] ON [dbo].[ExpenseCategories]
IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_ExpenseCategoryId' AND object_id = object_id(N'[dbo].[Transactions]', N'U'))
    DROP INDEX [IX_ExpenseCategoryId] ON [dbo].[Transactions]
DECLARE @var11 nvarchar(128)
SELECT @var11 = name
FROM sys.default_constraints
WHERE parent_object_id = object_id(N'dbo.Transactions')
AND col_name(parent_object_id, parent_column_id) = 'ExpenseCategoryId';
IF @var11 IS NOT NULL
    EXECUTE('ALTER TABLE [dbo].[Transactions] DROP CONSTRAINT [' + @var11 + ']')
ALTER TABLE [dbo].[Transactions] DROP COLUMN [ExpenseCategoryId]
DECLARE @var12 nvarchar(128)
SELECT @var12 = name
FROM sys.default_constraints
WHERE parent_object_id = object_id(N'dbo.Transactions')
AND col_name(parent_object_id, parent_column_id) = 'Committed';
IF @var12 IS NOT NULL
    EXECUTE('ALTER TABLE [dbo].[Transactions] DROP CONSTRAINT [' + @var12 + ']')
ALTER TABLE [dbo].[Transactions] DROP COLUMN [Committed]
DECLARE @var13 nvarchar(128)
SELECT @var13 = name
FROM sys.default_constraints
WHERE parent_object_id = object_id(N'dbo.Transactions')
AND col_name(parent_object_id, parent_column_id) = 'Effective';
IF @var13 IS NOT NULL
    EXECUTE('ALTER TABLE [dbo].[Transactions] DROP CONSTRAINT [' + @var13 + ']')
ALTER TABLE [dbo].[Transactions] DROP COLUMN [Effective]
DROP TABLE [dbo].[ExpenseCategories]
DELETE [dbo].[__MigrationHistory]
WHERE (([MigrationId] = N'201510152028035_AddExpenseCategories') AND ([ContextKey] = N'Wiz.Gringotts.UIWeb.Helpers.TestConfiguration'))
ALTER DATABASE CURRENT SET READ_COMMITTED_SNAPSHOT ON
ALTER DATABASE CURRENT SET ALLOW_SNAPSHOT_ISOLATION ON
IF object_id(N'[dbo].[FK_dbo.Accounts_dbo.Organizations_OrganizationId]', N'F') IS NOT NULL
    ALTER TABLE [dbo].[Accounts] DROP CONSTRAINT [FK_dbo.Accounts_dbo.Organizations_OrganizationId]
IF object_id(N'[dbo].[FK_dbo.Accounts_dbo.Funds_FundId]', N'F') IS NOT NULL
    ALTER TABLE [dbo].[Accounts] DROP CONSTRAINT [FK_dbo.Accounts_dbo.Funds_FundId]
IF object_id(N'[dbo].[FK_dbo.FundsSubsidiaryAccounts_dbo.Accounts_AccountId]', N'F') IS NOT NULL
    ALTER TABLE [dbo].[FundsSubsidiaryAccounts] DROP CONSTRAINT [FK_dbo.FundsSubsidiaryAccounts_dbo.Accounts_AccountId]
IF object_id(N'[dbo].[FK_dbo.FundsSubsidiaryAccounts_dbo.Funds_FundId]', N'F') IS NOT NULL
    ALTER TABLE [dbo].[FundsSubsidiaryAccounts] DROP CONSTRAINT [FK_dbo.FundsSubsidiaryAccounts_dbo.Funds_FundId]
IF object_id(N'[dbo].[FK_dbo.FundsClientAccounts_dbo.Accounts_AccountId]', N'F') IS NOT NULL
    ALTER TABLE [dbo].[FundsClientAccounts] DROP CONSTRAINT [FK_dbo.FundsClientAccounts_dbo.Accounts_AccountId]
IF object_id(N'[dbo].[FK_dbo.FundsClientAccounts_dbo.Funds_FundId]', N'F') IS NOT NULL
    ALTER TABLE [dbo].[FundsClientAccounts] DROP CONSTRAINT [FK_dbo.FundsClientAccounts_dbo.Funds_FundId]
IF object_id(N'[dbo].[FK_dbo.Residencies_dbo.Organizations_OrganizationId]', N'F') IS NOT NULL
    ALTER TABLE [dbo].[Residencies] DROP CONSTRAINT [FK_dbo.Residencies_dbo.Organizations_OrganizationId]
IF object_id(N'[dbo].[FK_dbo.Orders_dbo.Residencies_ResidencyId]', N'F') IS NOT NULL
    ALTER TABLE [dbo].[Orders] DROP CONSTRAINT [FK_dbo.Orders_dbo.Residencies_ResidencyId]
IF object_id(N'[dbo].[FK_dbo.Orders_dbo.Payees_PayeeId]', N'F') IS NOT NULL
    ALTER TABLE [dbo].[Orders] DROP CONSTRAINT [FK_dbo.Orders_dbo.Payees_PayeeId]
IF object_id(N'[dbo].[FK_dbo.Orders_dbo.Organizations_OrganizationId]', N'F') IS NOT NULL
    ALTER TABLE [dbo].[Orders] DROP CONSTRAINT [FK_dbo.Orders_dbo.Organizations_OrganizationId]
IF object_id(N'[dbo].[FK_dbo.Residencies_dbo.LivingUnits_LivingUnitId]', N'F') IS NOT NULL
    ALTER TABLE [dbo].[Residencies] DROP CONSTRAINT [FK_dbo.Residencies_dbo.LivingUnits_LivingUnitId]
IF object_id(N'[dbo].[FK_dbo.LivingUnits_dbo.Organizations_OrganizationId]', N'F') IS NOT NULL
    ALTER TABLE [dbo].[LivingUnits] DROP CONSTRAINT [FK_dbo.LivingUnits_dbo.Organizations_OrganizationId]
IF object_id(N'[dbo].[FK_dbo.ResidencyGuardians_dbo.Payees_PayeeId]', N'F') IS NOT NULL
    ALTER TABLE [dbo].[ResidencyGuardians] DROP CONSTRAINT [FK_dbo.ResidencyGuardians_dbo.Payees_PayeeId]
IF object_id(N'[dbo].[FK_dbo.ResidencyGuardians_dbo.Residencies_ResidencyId]', N'F') IS NOT NULL
    ALTER TABLE [dbo].[ResidencyGuardians] DROP CONSTRAINT [FK_dbo.ResidencyGuardians_dbo.Residencies_ResidencyId]
IF object_id(N'[dbo].[FK_dbo.Residencies_dbo.Clients_ClientId]', N'F') IS NOT NULL
    ALTER TABLE [dbo].[Residencies] DROP CONSTRAINT [FK_dbo.Residencies_dbo.Clients_ClientId]
IF object_id(N'[dbo].[FK_dbo.ClientIdentifiers_dbo.ClientIdentifierTypes_ClientIdentiferTypeId]', N'F') IS NOT NULL
    ALTER TABLE [dbo].[ClientIdentifiers] DROP CONSTRAINT [FK_dbo.ClientIdentifiers_dbo.ClientIdentifierTypes_ClientIdentiferTypeId]
IF object_id(N'[dbo].[FK_dbo.ClientIdentifiers_dbo.Clients_ClientId]', N'F') IS NOT NULL
    ALTER TABLE [dbo].[ClientIdentifiers] DROP CONSTRAINT [FK_dbo.ClientIdentifiers_dbo.Clients_ClientId]
IF object_id(N'[dbo].[FK_dbo.ResidencyAttorneys_dbo.Payees_PayeeId]', N'F') IS NOT NULL
    ALTER TABLE [dbo].[ResidencyAttorneys] DROP CONSTRAINT [FK_dbo.ResidencyAttorneys_dbo.Payees_PayeeId]
IF object_id(N'[dbo].[FK_dbo.ResidencyAttorneys_dbo.Residencies_ResidencyId]', N'F') IS NOT NULL
    ALTER TABLE [dbo].[ResidencyAttorneys] DROP CONSTRAINT [FK_dbo.ResidencyAttorneys_dbo.Residencies_ResidencyId]
IF object_id(N'[dbo].[FK_dbo.Accounts_dbo.Residencies_ResidencyId]', N'F') IS NOT NULL
    ALTER TABLE [dbo].[Accounts] DROP CONSTRAINT [FK_dbo.Accounts_dbo.Residencies_ResidencyId]
IF object_id(N'[dbo].[FK_dbo.Transactions_dbo.Payees_PayeeId]', N'F') IS NOT NULL
    ALTER TABLE [dbo].[Transactions] DROP CONSTRAINT [FK_dbo.Transactions_dbo.Payees_PayeeId]
IF object_id(N'[dbo].[FK_dbo.PayeeTypePayees_dbo.Payees_Payee_Id]', N'F') IS NOT NULL
    ALTER TABLE [dbo].[PayeeTypePayees] DROP CONSTRAINT [FK_dbo.PayeeTypePayees_dbo.Payees_Payee_Id]
IF object_id(N'[dbo].[FK_dbo.PayeeTypePayees_dbo.PayeeTypes_PayeeType_Id]', N'F') IS NOT NULL
    ALTER TABLE [dbo].[PayeeTypePayees] DROP CONSTRAINT [FK_dbo.PayeeTypePayees_dbo.PayeeTypes_PayeeType_Id]
IF object_id(N'[dbo].[FK_dbo.Payees_dbo.Organizations_OrganizationId]', N'F') IS NOT NULL
    ALTER TABLE [dbo].[Payees] DROP CONSTRAINT [FK_dbo.Payees_dbo.Organizations_OrganizationId]
IF object_id(N'[dbo].[FK_dbo.Transactions_dbo.TransactionSubTypes_TransactionSubTypeId]', N'F') IS NOT NULL
    ALTER TABLE [dbo].[Transactions] DROP CONSTRAINT [FK_dbo.Transactions_dbo.TransactionSubTypes_TransactionSubTypeId]
IF object_id(N'[dbo].[FK_dbo.Transactions_dbo.Organizations_OrganizationId]', N'F') IS NOT NULL
    ALTER TABLE [dbo].[Transactions] DROP CONSTRAINT [FK_dbo.Transactions_dbo.Organizations_OrganizationId]
IF object_id(N'[dbo].[FK_dbo.Transactions_dbo.Funds_FundId]', N'F') IS NOT NULL
    ALTER TABLE [dbo].[Transactions] DROP CONSTRAINT [FK_dbo.Transactions_dbo.Funds_FundId]
IF object_id(N'[dbo].[FK_dbo.Transactions_dbo.Accounts_AccountId]', N'F') IS NOT NULL
    ALTER TABLE [dbo].[Transactions] DROP CONSTRAINT [FK_dbo.Transactions_dbo.Accounts_AccountId]
IF object_id(N'[dbo].[FK_dbo.Funds_dbo.Organizations_OrganizationId]', N'F') IS NOT NULL
    ALTER TABLE [dbo].[Funds] DROP CONSTRAINT [FK_dbo.Funds_dbo.Organizations_OrganizationId]
IF object_id(N'[dbo].[FK_dbo.OrganizationFeatures_dbo.Features_FeatureId]', N'F') IS NOT NULL
    ALTER TABLE [dbo].[OrganizationFeatures] DROP CONSTRAINT [FK_dbo.OrganizationFeatures_dbo.Features_FeatureId]
IF object_id(N'[dbo].[FK_dbo.OrganizationFeatures_dbo.Organizations_OrganizationId]', N'F') IS NOT NULL
    ALTER TABLE [dbo].[OrganizationFeatures] DROP CONSTRAINT [FK_dbo.OrganizationFeatures_dbo.Organizations_OrganizationId]
IF object_id(N'[dbo].[FK_dbo.Organizations_dbo.Organizations_Parent_Id]', N'F') IS NOT NULL
    ALTER TABLE [dbo].[Organizations] DROP CONSTRAINT [FK_dbo.Organizations_dbo.Organizations_Parent_Id]
IF object_id(N'[dbo].[FK_dbo.Funds_dbo.FundTypes_FundTypeId]', N'F') IS NOT NULL
    ALTER TABLE [dbo].[Funds] DROP CONSTRAINT [FK_dbo.Funds_dbo.FundTypes_FundTypeId]
IF object_id(N'[dbo].[FK_dbo.Accounts_dbo.AccountTypes_AccountTypeId]', N'F') IS NOT NULL
    ALTER TABLE [dbo].[Accounts] DROP CONSTRAINT [FK_dbo.Accounts_dbo.AccountTypes_AccountTypeId]
IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_AccountId' AND object_id = object_id(N'[dbo].[FundsSubsidiaryAccounts]', N'U'))
    DROP INDEX [IX_AccountId] ON [dbo].[FundsSubsidiaryAccounts]
IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_FundId' AND object_id = object_id(N'[dbo].[FundsSubsidiaryAccounts]', N'U'))
    DROP INDEX [IX_FundId] ON [dbo].[FundsSubsidiaryAccounts]
IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_AccountId' AND object_id = object_id(N'[dbo].[FundsClientAccounts]', N'U'))
    DROP INDEX [IX_AccountId] ON [dbo].[FundsClientAccounts]
IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_FundId' AND object_id = object_id(N'[dbo].[FundsClientAccounts]', N'U'))
    DROP INDEX [IX_FundId] ON [dbo].[FundsClientAccounts]
IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_PayeeId' AND object_id = object_id(N'[dbo].[ResidencyGuardians]', N'U'))
    DROP INDEX [IX_PayeeId] ON [dbo].[ResidencyGuardians]
IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_ResidencyId' AND object_id = object_id(N'[dbo].[ResidencyGuardians]', N'U'))
    DROP INDEX [IX_ResidencyId] ON [dbo].[ResidencyGuardians]
IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_PayeeId' AND object_id = object_id(N'[dbo].[ResidencyAttorneys]', N'U'))
    DROP INDEX [IX_PayeeId] ON [dbo].[ResidencyAttorneys]
IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_ResidencyId' AND object_id = object_id(N'[dbo].[ResidencyAttorneys]', N'U'))
    DROP INDEX [IX_ResidencyId] ON [dbo].[ResidencyAttorneys]
IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_Payee_Id' AND object_id = object_id(N'[dbo].[PayeeTypePayees]', N'U'))
    DROP INDEX [IX_Payee_Id] ON [dbo].[PayeeTypePayees]
IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_PayeeType_Id' AND object_id = object_id(N'[dbo].[PayeeTypePayees]', N'U'))
    DROP INDEX [IX_PayeeType_Id] ON [dbo].[PayeeTypePayees]
IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_FeatureId' AND object_id = object_id(N'[dbo].[OrganizationFeatures]', N'U'))
    DROP INDEX [IX_FeatureId] ON [dbo].[OrganizationFeatures]
IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_OrganizationId' AND object_id = object_id(N'[dbo].[OrganizationFeatures]', N'U'))
    DROP INDEX [IX_OrganizationId] ON [dbo].[OrganizationFeatures]
IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_OrganizationId' AND object_id = object_id(N'[dbo].[Orders]', N'U'))
    DROP INDEX [IX_OrganizationId] ON [dbo].[Orders]
IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_ResidencyId' AND object_id = object_id(N'[dbo].[Orders]', N'U'))
    DROP INDEX [IX_ResidencyId] ON [dbo].[Orders]
IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_PayeeId' AND object_id = object_id(N'[dbo].[Orders]', N'U'))
    DROP INDEX [IX_PayeeId] ON [dbo].[Orders]
IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_OrganizationId' AND object_id = object_id(N'[dbo].[LivingUnits]', N'U'))
    DROP INDEX [IX_OrganizationId] ON [dbo].[LivingUnits]
IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_Name' AND object_id = object_id(N'[dbo].[ClientIdentifierTypes]', N'U'))
    DROP INDEX [IX_Name] ON [dbo].[ClientIdentifierTypes]
IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_ClientIdClientIdentifierTypeId' AND object_id = object_id(N'[dbo].[ClientIdentifiers]', N'U'))
    DROP INDEX [IX_ClientIdClientIdentifierTypeId] ON [dbo].[ClientIdentifiers]
IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_OrganizationId' AND object_id = object_id(N'[dbo].[Residencies]', N'U'))
    DROP INDEX [IX_OrganizationId] ON [dbo].[Residencies]
IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_LivingUnitId' AND object_id = object_id(N'[dbo].[Residencies]', N'U'))
    DROP INDEX [IX_LivingUnitId] ON [dbo].[Residencies]
IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_ClientId' AND object_id = object_id(N'[dbo].[Residencies]', N'U'))
    DROP INDEX [IX_ClientId] ON [dbo].[Residencies]
IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_Name' AND object_id = object_id(N'[dbo].[PayeeTypes]', N'U'))
    DROP INDEX [IX_Name] ON [dbo].[PayeeTypes]
IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_OrganizationId' AND object_id = object_id(N'[dbo].[Payees]', N'U'))
    DROP INDEX [IX_OrganizationId] ON [dbo].[Payees]
IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_Name' AND object_id = object_id(N'[dbo].[TransactionSubTypes]', N'U'))
    DROP INDEX [IX_Name] ON [dbo].[TransactionSubTypes]
IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_PayeeId' AND object_id = object_id(N'[dbo].[Transactions]', N'U'))
    DROP INDEX [IX_PayeeId] ON [dbo].[Transactions]
IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_OrganizationId' AND object_id = object_id(N'[dbo].[Transactions]', N'U'))
    DROP INDEX [IX_OrganizationId] ON [dbo].[Transactions]
IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_AccountId' AND object_id = object_id(N'[dbo].[Transactions]', N'U'))
    DROP INDEX [IX_AccountId] ON [dbo].[Transactions]
IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_FundId' AND object_id = object_id(N'[dbo].[Transactions]', N'U'))
    DROP INDEX [IX_FundId] ON [dbo].[Transactions]
IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_TransactionSubTypeId' AND object_id = object_id(N'[dbo].[Transactions]', N'U'))
    DROP INDEX [IX_TransactionSubTypeId] ON [dbo].[Transactions]
IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_Name' AND object_id = object_id(N'[dbo].[Features]', N'U'))
    DROP INDEX [IX_Name] ON [dbo].[Features]
IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_Parent_Id' AND object_id = object_id(N'[dbo].[Organizations]', N'U'))
    DROP INDEX [IX_Parent_Id] ON [dbo].[Organizations]
IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_Abbreviation' AND object_id = object_id(N'[dbo].[Organizations]', N'U'))
    DROP INDEX [IX_Abbreviation] ON [dbo].[Organizations]
IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_GroupName' AND object_id = object_id(N'[dbo].[Organizations]', N'U'))
    DROP INDEX [IX_GroupName] ON [dbo].[Organizations]
IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_Name' AND object_id = object_id(N'[dbo].[Organizations]', N'U'))
    DROP INDEX [IX_Name] ON [dbo].[Organizations]
IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_Name' AND object_id = object_id(N'[dbo].[FundTypes]', N'U'))
    DROP INDEX [IX_Name] ON [dbo].[FundTypes]
IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_Code' AND object_id = object_id(N'[dbo].[FundTypes]', N'U'))
    DROP INDEX [IX_Code] ON [dbo].[FundTypes]
IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_OrganizationId' AND object_id = object_id(N'[dbo].[Funds]', N'U'))
    DROP INDEX [IX_OrganizationId] ON [dbo].[Funds]
IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_FundTypeId' AND object_id = object_id(N'[dbo].[Funds]', N'U'))
    DROP INDEX [IX_FundTypeId] ON [dbo].[Funds]
IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_Name' AND object_id = object_id(N'[dbo].[AccountTypes]', N'U'))
    DROP INDEX [IX_Name] ON [dbo].[AccountTypes]
IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_ResidencyId' AND object_id = object_id(N'[dbo].[Accounts]', N'U'))
    DROP INDEX [IX_ResidencyId] ON [dbo].[Accounts]
IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_OrganizationId' AND object_id = object_id(N'[dbo].[Accounts]', N'U'))
    DROP INDEX [IX_OrganizationId] ON [dbo].[Accounts]
IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_AccountTypeId' AND object_id = object_id(N'[dbo].[Accounts]', N'U'))
    DROP INDEX [IX_AccountTypeId] ON [dbo].[Accounts]
IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_FundId' AND object_id = object_id(N'[dbo].[Accounts]', N'U'))
    DROP INDEX [IX_FundId] ON [dbo].[Accounts]
DROP TABLE [dbo].[FundsSubsidiaryAccounts]
DROP TABLE [dbo].[FundsClientAccounts]
DROP TABLE [dbo].[ResidencyGuardians]
DROP TABLE [dbo].[ResidencyAttorneys]
DROP TABLE [dbo].[PayeeTypePayees]
DROP TABLE [dbo].[OrganizationFeatures]
DROP TABLE [dbo].[Files]
DROP TABLE [dbo].[Orders]
DROP TABLE [dbo].[LivingUnits]
DROP TABLE [dbo].[ClientIdentifierTypes]
DROP TABLE [dbo].[ClientIdentifiers]
DROP TABLE [dbo].[Clients]
DROP TABLE [dbo].[Residencies]
DROP TABLE [dbo].[PayeeTypes]
DROP TABLE [dbo].[Payees]
DROP TABLE [dbo].[TransactionSubTypes]
DROP TABLE [dbo].[Transactions]
DROP TABLE [dbo].[Features]
DROP TABLE [dbo].[Organizations]
DROP TABLE [dbo].[FundTypes]
DROP TABLE [dbo].[Funds]
DROP TABLE [dbo].[AccountTypes]
DROP TABLE [dbo].[Accounts]
DROP TABLE [dbo].[__MigrationHistory]
