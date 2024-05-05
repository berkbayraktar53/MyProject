using Business.Abstract;
using Entities.Concrete;
using Business.Constants;
using DataAccess.Abstract;
using Core.Utilities.Results;

namespace Business.Concrete
{
    public class ProductManager(IProductDal productDal) : IProductService
    {
        private readonly IProductDal _productDal = productDal;

        public IResult Add(Product product)
        {
            try
            {
                _productDal.Add(product);
                return new SuccessResult(Messages.ProductAdded);
            }
            catch (Exception)
            {
                return new ErrorResult(Messages.ProductCouldNotBeAdded);
            }
        }

        public IResult Delete(Product product)
        {
            try
            {
                _productDal.Delete(product);
                return new SuccessResult(Messages.ProductDeleted);
            }
            catch (Exception)
            {
                return new ErrorResult(Messages.ProductCouldNotBeDeleted);
            }
        }

        public IDataResult<Product> GetById(int productId)
        {
            return new SuccessDataResult<Product>(_productDal.Get(p => p.ProductID == productId));
        }

        public IDataResult<List<Product>> GetList()
        {
            return new SuccessDataResult<List<Product>>([.. _productDal.GetList()]);
        }

        public IDataResult<List<Product>> GetListByCategory(int categoryId)
        {
            return new SuccessDataResult<List<Product>>([.. _productDal.GetList(p => p.CategoryID == categoryId)]);
        }

        public IResult Update(Product product)
        {
            try
            {
                _productDal.Update(product);
                return new SuccessResult(Messages.ProductUpdated);
            }
            catch (Exception)
            {
                return new ErrorResult(Messages.ProductCouldNotBeUpdated);
            }
        }
    }
}