using Application.Queries;
using FluentValidation;

namespace Application.Activities.Validators
{
    public class GetActivityListValidator : AbstractValidator<GetActivityList.Query>
    {
        public GetActivityListValidator()
        {
            RuleFor(x => x.Params.PageSize)
                .GreaterThan(0).WithMessage("Page size must be greater than 0");
            RuleFor(x => x.Params.Filter)
                .Must(x => x is null or "isGoing" or "isHost")
                .WithMessage("Filter must be isGoing or isHost");
            RuleFor(x => x.Params.SortOrder)
                .Must(x => x is "asc" or "desc")
                .WithMessage("Sort order must be asc or desc");
        }
    }
}
