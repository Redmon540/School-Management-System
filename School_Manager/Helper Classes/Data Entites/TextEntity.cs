using System;
using System.Text.RegularExpressions;

namespace School_Manager
{
    public class TextEntity : BasePropertyChanged
    {
        
        /// <summary>
        /// Name of the feild in Database table (ColumnName)
        /// </summary>
        public string FeildName { get; set; }

        /// <summary>
        /// Value of the Feild
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// tells if the control is enabled
        /// </summary>
        public bool IsEnabled { get; set; } = true;

        /// <summary>
        /// Type of validation for this entity
        /// </summary>
        public ValidationType ValidationType { get; set; } = ValidationType.None;

        /// <summary>
        /// Valid if value is not null or empty
        /// </summary>
        public bool IsValid
        {
            get
            {
                {
                    if (!IsEnabled)
                        return true;
                    switch(ValidationType)
                    {
                        case ValidationType.None:
                            {
                                return true;
                            }
                        case ValidationType.NotEmpty:
                            {
                                return !Value.IsNullOrEmpty();
                            }
                        case ValidationType.Numeric:
                            {
                                if (Value.IsNullOrEmpty())
                                    return true;
                                return Regex.Match(Value,@"^[0-9]*$").Success;
                            }
                        case ValidationType.Alphabetic:
                            {
                                if (Value.IsNullOrEmpty())
                                    return true;
                                return Regex.Match(Value, @"^[a-zA-z]*$").Success;
                            }
                        case ValidationType.ClassName:
                            {
                                return !Value.IsNullOrEmpty();
                            }
                        case ValidationType.PhoneNumber:
                            {
                                if (Value.IsNullOrEmpty())
                                    return true;
                                return Regex.Match(Value, @"^\+92[0-9]{3}-[0-9]{7}$").Success;
                            }
                        case ValidationType.Email:
                            {
                                if (Value.IsNullOrEmpty())
                                    return true;
                                return Regex.IsMatch(Value, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase);
                            }
                        case ValidationType.Decimal:
                            {
                                if (Value.IsNullOrEmpty())
                                    return true;
                                return Regex.Match(Value, @"^[0-9]*([\.\,][0-9]*)?$").Success;
                            }
                        case ValidationType.NumericWithNonEmpty:
                            {
                                if (Value.IsNullOrEmpty())
                                    return false;
                                return Regex.Match(Value, @"^[0-9]*$").Success;
                            }
                        case ValidationType.RegistrationKey:
                            {
                                if (Value.IsNullOrEmpty())
                                    return false;
                                return Regex.Match(Value, @"^[A-Fa-f0-9]{4}-[A-Fa-f0-9]{4}-[A-Za-z0-9]{4}-[A-Za-z0-9]{4}$").Success;
                            }
                        default:
                            return true;
                    }                    
                }
            }
        }

        /// <summary>
        /// Alert message that shows of the feild is invalid
        /// </summary>
        public string AlertText
        {
            get
            {
                switch (ValidationType)
                {
                    case ValidationType.None:
                        {
                            return string.Empty;
                        }
                    case ValidationType.NotEmpty:
                        {
                            return IsEnabled ? "Feild is required." : "";
                        }
                    case ValidationType.Numeric:
                        {
                            return IsEnabled ? "Only numeric data is allowed." : "";
                        }
                    case ValidationType.Alphabetic:
                        {
                            return IsEnabled ? "Numbers aren't allowed." : "";
                        }
                    case ValidationType.ClassName:
                        {
                            return IsEnabled ? "Feild is required." : "";
                        }
                    case ValidationType.PhoneNumber:
                        {
                            return IsEnabled ? "Number must be in +923XX-XXXXXXX format." : "";
                        }
                    case ValidationType.Email:
                        {
                            return IsEnabled ? "Invalid email address." : "";
                        }
                    case ValidationType.Decimal:
                        {
                            return IsEnabled ? "Invalid Value" : "";
                        }
                    case ValidationType.NumericWithNonEmpty:
                        {
                            return IsEnabled ? "Feild cannot be empty." : "";
                        }
                    case ValidationType.RegistrationKey:
                        {
                            return IsEnabled ? "Key must be in XXXX-XXXX-XXXX-XXXX format." : "";
                        }
                    default:
                        return string.Empty;
                }
            }
        }

        public bool IsOld { get; set; } = false;

        public string OriginalValue { get; set; }

        public string SectionID { get; set; }

        public DateTime DueDate { get; set; }
    }
}
