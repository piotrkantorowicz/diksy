using System.ComponentModel.DataAnnotations;

namespace Diksy.Translation.Validation
{
    public class MinLengthIfNotNullAttribute(int minLength, int maxLength) : ValidationAttribute
    {
        private readonly int _maxLength = maxLength;
        private readonly int _minLength = minLength;

        public override bool IsValid(object? value)
        {
            if (value is null)
            {
                return true;
            }

            if (value is string stringValue)
            {
                return stringValue.Length >= _minLength && stringValue.Length <= _maxLength;
            }

            return false;
        }
    }
}