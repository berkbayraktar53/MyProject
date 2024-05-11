using FluentValidation;
using Entities.Concrete;

namespace Business.ValidationRules.FluentValidation
{
    public class ProductValidator : AbstractValidator<Product>
    {
        public ProductValidator()
        {
            RuleFor(p => p.ProductName).NotEmpty().WithMessage("Ürün adı boş geçilemez.");
            RuleFor(p => p.ProductName).Length(2, 30);
            RuleFor(p => p.UnitPrice).NotEmpty().WithMessage("Ürün fiyatı boş geçilemez.");
            RuleFor(p => p.UnitPrice).GreaterThanOrEqualTo(1);
            RuleFor(p => p.UnitPrice).GreaterThanOrEqualTo(10).When(p => p.CategoryID == 1);
            RuleFor(p => p.ProductName).Must(StartsWithA).WithMessage("Ürün adı A ile başlamalı.");
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