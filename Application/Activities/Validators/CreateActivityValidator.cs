using Application.Activities.Commands;
using Application.Activities.DTOS;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Activities.Validators
{
    public class CreateActivityValidator : BaseActivityValidator<CreateActivity.Command,CreateActivityDto>
    {
        public CreateActivityValidator() : base(x=>x.ActivityDto)
        {

        }
    }
}
