using FluentValidation;
using System.ComponentModel.DataAnnotations;

namespace Datalayer.ViewModels
{
    public class ChangePasswordViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 4)]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare(nameof(NewPassword))]
        public string ConfirmPassword { get; set; }
    }

    public class ChangePasswordViewModelValidator : AbstractValidator<ChangePasswordViewModel>
    {
        public ChangePasswordViewModelValidator()
        {
            RuleFor(q => q.OldPassword)
                .NotEmpty()
                .WithMessage("گذرواژه قبلی را وارد کنید.");

            RuleFor(q => q.NewPassword)
                .NotEmpty()
                .WithMessage("گذرواژه نباید خالی باشد.")
                .MinimumLength(8)
                .WithMessage("گذرواژه باید حداقل 8 کاراکتر داشته باشد.")
                .MaximumLength(50)
                .WithMessage("گذرواژه باید حداکثر 50 کاراکتر داشته باشد.")
                .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[a-zA-Z\d]{8,50}$")
                .WithMessage("گذرواژه باید حداقل 8 کاراکتر، حداکثر 50 کاراکتر و شامل 1 کاراکتر بزرگ، یک کاراکتر کوچک و یک عدد باشد.");

            RuleFor(q => q.NewPassword)
                .NotEmpty()
                .WithMessage("تکرار گذرواژه نباید خالی باشد.")
                .Equal(q=>q.NewPassword)
                .WithMessage("تکرار گذرواژه باید با گذرواژه برابر باشد.")
                .MinimumLength(8)
                .WithMessage("تکرار گذرواژه باید حداقل 8 کاراکتر داشته باشد.")
                .MaximumLength(50)
                .WithMessage("تکرار گذرواژه باید حداکثر 50 کاراکتر داشته باشد.")
                .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[a-zA-Z\d]{8,50}$")
                .WithMessage("تکرار گذرواژه باید حداقل 8 کاراکتر، حداکثر 50 کاراکتر و شامل 1 کاراکتر بزرگ، یک کاراکتر کوچک و یک عدد باشد.");
        }
    }
}