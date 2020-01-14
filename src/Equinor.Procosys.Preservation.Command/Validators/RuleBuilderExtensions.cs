using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ModeAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ResponsibleAggregate;
using FluentValidation;

namespace Equinor.Procosys.Preservation.Command.Validators
{
    public static class RuleBuilderExtensions
    {
        public static IRuleBuilder<T, int> JourneyMustExist<T>(this IRuleBuilder<T, int> ruleBuilder, IJourneyRepository journeyRepository) =>
            ruleBuilder
                .SetValidator(new JourneyExistsValidator(journeyRepository));

        public static IRuleBuilder<T, string> JourneyTitleIsUnique<T>(this IRuleBuilder<T, string> ruleBuilder, IJourneyRepository journeyRepository) =>
            ruleBuilder
                .SetValidator(new JourneyTitleIsUniqueValidator(journeyRepository));

        public static IRuleBuilder<T, int> ModeMustExist<T>(this IRuleBuilder<T, int> ruleBuilder, IModeRepository modeRepository) =>
            ruleBuilder
                .SetValidator(new ModeExistsValidator(modeRepository));

        public static IRuleBuilder<T, int> ResponsibleMustExist<T>(this IRuleBuilder<T, int> ruleBuilder, IResponsibleRepository responsibleRepository) =>
            ruleBuilder
                .SetValidator(new ResponsibleExistsValidator(responsibleRepository));
    }
}
