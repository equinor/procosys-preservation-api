using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.TagAggregate;
using FluentValidation;

namespace Equinor.Procosys.Preservation.Command.Validators
{
    public static class RuleBuilderExtensions
    {
        public static IRuleBuilder<T, int> JourneyMustExist<T>(this IRuleBuilder<T, int> ruleBuilder, IJourneyRepository journeyRepository) =>
            ruleBuilder
                .SetValidator(new JourneyExistsValidator(journeyRepository));

        public static IRuleBuilder<T, int> TagMustExist<T>(this IRuleBuilder<T, int> ruleBuilder, ITagRepository tagRepository) =>
            ruleBuilder
                .SetValidator(new TagExistsValidator(tagRepository));

        public static IRuleBuilder<T, StepExistsValidator.JourneyStep> StepMustExist<T>(this IRuleBuilder<T, StepExistsValidator.JourneyStep> ruleBuilder, IJourneyRepository journeyRepository) =>
            ruleBuilder
                .SetValidator(new StepExistsValidator(journeyRepository));

        public static IRuleBuilder<T, int> StepMustExist2<T>(this IRuleBuilder<T, int> ruleBuilder, IJourneyRepository journeyRepository) =>
            ruleBuilder
                .SetValidator(new StepExistsValidator(journeyRepository));
    }
}
