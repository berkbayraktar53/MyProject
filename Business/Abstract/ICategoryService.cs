using Entities.Concrete;
using Core.Utilities.Results;

namespace Business.Abstract
{
    public interface ICategoryService
    {
        IDataResult<Category> GetById(int categoryId);
        IDataResult<List<Category>> GetList();
        IResult Add(Category category);
        IResult Delete(Category category);
        IResult Update(Category category);
    }
}