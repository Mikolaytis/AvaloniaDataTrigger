using System.Globalization;

namespace Avalonia.DataTrigger;

public static class BindingValueComparer
{
    public static bool Compare(object? leftOperand, object? rightOperand)
    {
        if (leftOperand is { } && rightOperand is { })
        {
            var value = rightOperand.ToString();
            var destinationType = leftOperand.GetType();
            if (value is { })
            {
                rightOperand = TypeConverterHelper.Convert(value, destinationType);
            }
        }

        if (leftOperand is IComparable leftComparableOperand && 
            rightOperand is IComparable rightComparableOperand)
        {
            return EvaluateComparable(leftComparableOperand, rightComparableOperand);
        }
            
        return Equals(leftOperand, rightOperand);
    }
        
    /// <summary>
    /// Evaluates both operands that implement the IComparable interface.
    /// </summary>
    private static bool EvaluateComparable(IComparable leftOperand, IComparable rightOperand)
    {
        object? convertedOperand = null;
        try
        {
            convertedOperand = Convert.ChangeType(rightOperand, leftOperand.GetType(), CultureInfo.CurrentCulture);
        }
        catch (FormatException)
        {
            // FormatException: Convert.ChangeType("hello", typeof(double), ...);
        }
        catch (InvalidCastException)
        {
            // InvalidCastException: Convert.ChangeType(4.0d, typeof(Rectangle), ...);
        }

        if (convertedOperand is null)
        {
            return false;
        }

        return leftOperand.CompareTo((IComparable) convertedOperand) == 0;
    }
}
