using Business.Abstract;
using Entities.Concrete;
using Business.Constants;
using DataAccess.Abstract;
using Core.Utilities.Results;

namespace Business.Concrete
{
    public class CategoryManager(ICategoryDal categoryDal) : ICategoryService
    {
        private readonly ICategoryDal _categoryDal = categoryDal;

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

        public IDataResult<Category> GetById(int categoryId)
        {
            return new SuccessDataResult<Category>(_categoryDal.Get(p => p.CategoryID == categoryId));
        }

        public IDataResult<List<Category>> GetList()
        {
            return new SuccessDataResult<List<Category>>([.. _categoryDal.GetList()]);
        }

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