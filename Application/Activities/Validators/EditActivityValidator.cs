using Application.Activities.Commands;
using Application.Activities.DTOS;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Activities.Validators
{
    public class EditActivityValidator : BaseActivityValidator<EditActivity.Command,EditActivityDto>
    {
        public EditActivityValidator() : base (x=>x.ActivityDto)
        {
            RuleFor(x => x.ActivityDto.Id).NotEmpty().WithMessage("Id is required");
        }
    }
}
