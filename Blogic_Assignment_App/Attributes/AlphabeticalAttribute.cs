using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace InsuranceApp.Attributes
{
    /// <summary>
    /// Attribute that validates string property.
    /// Checks if characters are from Czech alphabet and length of string is lesser than or equal to given integer value.
    /// </summary>
    public partial class AlphabeticalAttribute : ValidationAttribute
    {

        [GeneratedRegex(@"^[a-zA-ZáčďéěíňóřšťúůýžÁČĎÉĚÍŇÓŘŠŤÚŮÝŽ]+$")]
        private static partial Regex CzechAlphabetRegex();


        /// <summary>
        /// Validates the value of the decorated property.
        /// </summary>
        /// <param name="value">The value of the decorated property</param>
        /// <param name="validationContext">The validation context</param>
        /// <returns>A ValidationResult indicating the outcome of validation</returns>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value.ToString() != null)
            {
                string input = value.ToString()!.Trim();

                if (!string.IsNullOrEmpty(input) && CzechAlphabetRegex().IsMatch(input))
                {
                    return ValidationResult.Success!;
                }

                return new ValidationResult(ErrorMessage ?? $"{validationContext.DisplayName} musí obsahovat pouze symboly české abecedy!");
            }
            return ValidationResult.Success!;
        }
    }
}