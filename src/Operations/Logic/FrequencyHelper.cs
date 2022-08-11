using Operations.Enums;

namespace Operations.Logic;

public static class FrequencyHelper
{
    public static DateTime CalculateNextExecution(DateTime fromDateTime, Frequency frequency)
    {
        var nextExecution = frequency switch
        {
            Frequency.Daily => fromDateTime.AddDays(1),
            Frequency.Weekly => fromDateTime.AddDays(7),
            Frequency.Monthly => fromDateTime.AddMonths(1),
            Frequency.Yearly => fromDateTime.AddYears(1),
            Frequency.Manual => throw new NotImplementedException(),
            _ => throw new ArgumentOutOfRangeException(nameof(frequency), frequency, null)
        };

        return nextExecution;
    }
}