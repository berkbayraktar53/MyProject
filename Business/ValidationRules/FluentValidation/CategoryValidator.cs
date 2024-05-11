using FluentValidation;
using Entities.Concrete;

namespace Business.ValidationRules.FluentValidation
{
    public class CategoryValidator : AbstractValidator<Category>
    {
        public CategoryValidator()
        {
            RuleFor(c => c.CategoryName).NotEmpty().WithMessage("Kategori adı boş geçilemez");
            RuleFor(c => c.CategoryName).Length(2, 30);
            RuleFor(c => c.Description).NotEmpty().WithMessage("Kategori açıklaması boş geçilemez");
            RuleFor(c => c.CategoryName).Length(2, 60);
            RuleFor(c => c.CategoryName).Must(StartsWithA).WithMessage("Kategori adı A ile başlamalıdır");
        }

        private bool StartsWithA(string arg)
        {
            if (string.IsNullOrEmpty(arg))
            {
                return false;
            }
            return arg[0] == 'A';
        }
    }
}