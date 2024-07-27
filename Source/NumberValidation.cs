using System;
using System.Globalization;
using System.Windows.Controls;

namespace FriendlySetPlayTime
{

    public class NumberValidation : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            try
            {
                int number = Int32.Parse((String)value);
                if (number < 0)
                {
                    return new ValidationResult(false, "value cannot be less than 0");
                }
                return ValidationResult.ValidResult;
            }
            catch (Exception e)
            { //TODO: this isn't working/being used
                return new ValidationResult(false, $"{value} is not a number");
            }
        }
    }
}
