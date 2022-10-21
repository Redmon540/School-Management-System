using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace School_Manager
{
    public class NotEmptyValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            return string.IsNullOrWhiteSpace((value ?? "").ToString())
                ? new ValidationResult(false, "Field is required.")
                : ValidationResult.ValidResult;
        }
    }

    public class NumericValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            return Regex.Match((value ?? "").ToString(), @"^[0-9]+$").Success
                ? ValidationResult.ValidResult
                : new ValidationResult(false, "Invalid Input");
        }
    }

    public class AlphabeticValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            return Regex.Match((value ?? "").ToString(), @"^[a-zA-Z]+$").Success
                ? ValidationResult.ValidResult
                : new ValidationResult(false, "Invalid input.");
        }
    }
}
