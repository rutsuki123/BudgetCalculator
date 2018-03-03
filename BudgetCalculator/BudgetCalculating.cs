using BudgetCalculator;

static internal class BudgetCalculating
{
    public static int DailyAmount(Budget budget)
    {
        var amount = budget.Amount;
        var totalDays = budget.TotalDays();
        var dailyAmount = (amount / totalDays);
        return dailyAmount;
    }
}