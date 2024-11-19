SELECT t.Id, t.CountAgainstBudget, f.RecurringTransferId
FROM BankTransaction as t
INNER JOIN Transfer as f ON t.TransferId = f.Id
WHERE t.TransferId != ""
    AND t.SavingsGoalId != ""
	AND t.IsSavingsSpending = 0
    AND f.RecurringTransferId IS NULL
	AND t.Amount > 0