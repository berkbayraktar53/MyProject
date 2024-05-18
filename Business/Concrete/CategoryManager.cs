using Business.Abstract;
using Entities.Concrete;
using Business.Constants;
using DataAccess.Abstract;
using Core.Utilities.Results;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Validation;
using Business.BusinessAspects.Autofac;
using Business.ValidationRules.FluentValidation;

namespace Business.Concrete
{
    public class CategoryManager(ICategoryDal categoryDal) : ICategoryService
    {
        private readonly ICategoryDal _categoryDal = categoryDal;

        [SecuredOperation("Category.Add,Admin", Priority = 1)]
        [ValidationAspect(typeof(CategoryValidator), Priority = 2)]
        [CacheRemoveAspect("ICategoryService.Get", Priority = 3)]
        public IResult Add(Category category)
        {
            try
            {
                _categoryDal.Add(category);
                return new SuccessResult(Messages.CategoryAdded);
            }
            catch (Exception)
            {
                return new ErrorResult(Messages.CategoryCouldNotBeAdded);
            }
        }

        [SecuredOperation("Category.Delete,Admin", Priority = 1)]
        [CacheRemoveAspect("ICategoryService.Get", Priority = 2)]
        public IResult Delete(Category category)
        {
            try
            {
                _categoryDal.Delete(category);
                return new SuccessResult(Messages.CategoryDeleted);
            }
            catch (Exception)
            {
                return new ErrorResult(Messages.CategoryCouldNotBeDeleted);
            }
        }

        [SecuredOperation("Category.GetById,Admin", Priority = 1)]
        [CacheAspect(Priority = 2)]
        public IDataResult<Category> GetById(int categoryId)
        {
            return new SuccessDataResult<Category>(_categoryDal.Get(p => p.CategoryID == categoryId));
        }

        [SecuredOperation("Category.GetList,Admin", Priority = 1)]
        [CacheAspect(Priority = 2)]
        public IDataResult<List<Category>> GetList()
        {
            return new SuccessDataResult<List<Category>>([.. _categoryDal.GetList()]);
        }

        [SecuredOperation("Category.Update,Admin", Priority = 1)]
        [ValidationAspect(typeof(CategoryValidator), Priority = 2)]
        [CacheRemoveAspect("ICategoryService.Get", Priority = 3)]
        public IResult Update(Category category)
        {
            try
            {
                _categoryDal.Update(category);
                return new SuccessResult(Messages.CategoryUpdated);
            }
            catch (Exception)
            {
                return new ErrorResult(Messages.CategoryCouldNotBeUpdated);
            }
        }
    }
}