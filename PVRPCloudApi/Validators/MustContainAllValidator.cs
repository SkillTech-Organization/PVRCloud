using FluentValidation;
using PVRPCloud;

namespace PVRPCloudApi.Validators;

public static class MustContainAllValidator
{
    public static IRuleBuilderOptions<T, IEnumerable<TElement>> MustContainAll<T, TElement>(this IRuleBuilder<T, IEnumerable<TElement>> ruleBuilder,
                                                                                                    IEnumerable<TElement> validValues) =>
        ruleBuilder.Must((_, values, context) =>
        {
            List<TElement> invalidValues = new(20);

            bool isValid = true;
            foreach (var value in values)
            {
                bool contains = validValues.Contains(value);

                if (!contains)
                {
                    invalidValues.Add(value);
                }

                isValid &= contains;
            }

            context.MessageFormatter.AppendArgument("CollectionValues", string.Join(", ", invalidValues));
            return isValid;
        }).WithMessage(Messages.ERR_NOT_FOUND_COLLECTION);
}
