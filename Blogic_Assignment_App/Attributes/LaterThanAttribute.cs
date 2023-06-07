using System.ComponentModel.DataAnnotations;

namespace InsuranceApp.Attributes
{
    /// <summary>
    /// Attribute that compares the value of the decorated property with another property of type DateTime.
    /// Has an option to treat equality as success.
    /// </summary>
    public class LaterThanAttribute : ValidationAttribute
    {
        private readonly string _otherPropertyName;
        private readonly bool _allowEquality;

        public LaterThanAttribute(string otherPropertyName, bool allowEquality = false)
        {
            _otherPropertyName = otherPropertyName;
            _allowEquality = allowEquality;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var otherPropertyInfo = validationContext.ObjectType.GetProperty(_otherPropertyName);
            if (otherPropertyInfo is null)
            {
                throw new ArgumentException($"Property {_otherPropertyName} not found on {validationContext.ObjectType.Name}.");
            }

            var otherPropertyValue = otherPropertyInfo.GetValue(validationContext.ObjectInstance);

            if (value is null || otherPropertyValue is null)
            {
                return ValidationResult.Success!;
            }

            if (value.GetType() != otherPropertyValue.GetType())
            {
                throw new ArgumentException("The compared properties must have the same type.");
            }

            if (value is not DateTime || otherPropertyValue is not DateTime)
            {
                throw new ArgumentException("The compared properties must be of type DateTime.");
            }

            var dateValue = (DateTime)value;
            var otherDateValue = (DateTime)otherPropertyValue;

            int comparisonResult = DateTime.Compare(dateValue, otherDateValue);

            if (_allowEquality ? comparisonResult <= 0 : comparisonResult < 0)
            {
                return new ValidationResult(ErrorMessage ?? $"{validationContext.MemberName} musí být později nebo ve stejný den jako {otherPropertyInfo.Name}.");
            }

            return ValidationResult.Success!;
        }
    }
}
