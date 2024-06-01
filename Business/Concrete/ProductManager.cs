using Business.Abstract;
using Entities.Concrete;
using Business.Constants;
using DataAccess.Abstract;
using Core.Utilities.Results;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.Aspects.Autofac.Validation;
using Business.BusinessAspects.Autofac;
using Core.Aspects.Autofac.Performance;
using Core.Aspects.Autofac.Transaction;
using Business.ValidationRules.FluentValidation;
using Core.CrossCuttingConcerns.Logging.Log4Net.Loggers;

namespace Business.Concrete
{
    public class ProductManager(IProductDal productDal) : IProductService
    {
        private readonly IProductDal _productDal = productDal;

        [SecuredOperation("Product.Add,Admin", Priority = 1)]
        [ValidationAspect(typeof(ProductValidator), Priority = 2)]
        [CacheRemoveAspect("IProductService.Get", Priority = 3)]
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

        [SecuredOperation("Product.Delete,Admin", Priority = 1)]
        [CacheRemoveAspect("IProductService.Get", Priority = 2)]
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

        [SecuredOperation("Product.GetById,Admin", Priority = 1)]
        [CacheAspect(Priority = 2)]
        [LogAspect(typeof(DatabaseLogger), Priority = 3)]
        public IDataResult<Product> GetById(int productId)
        {
            return new SuccessDataResult<Product>(_productDal.Get(p => p.ProductID == productId));
        }

        [SecuredOperation("Product.GetList,Admin", Priority = 1)]
        [CacheAspect(Priority = 2)]
        [LogAspect(typeof(DatabaseLogger), Priority = 3)]
        [PerformanceAspect(interval: 5, Priority = 4)]
        public IDataResult<List<Product>> GetList()
        {
            return new SuccessDataResult<List<Product>>([.. _productDal.GetList()]);
        }

        [SecuredOperation("Product.GetListByCategory,Admin", Priority = 1)]
        [CacheAspect(Priority = 2)]
        public IDataResult<List<Product>> GetListByCategory(int categoryId)
        {
            return new SuccessDataResult<List<Product>>([.. _productDal.GetList(p => p.CategoryID == categoryId)]);
        }

        [SecuredOperation("Product.Add,Product.Update,Admin", Priority = 1)]
        [CacheRemoveAspect("IProductService.Get", Priority = 2)]
        [TransactionScopeAspect(Priority = 3)]
        public IResult TransactionalOperation(Product product)
        {
            _productDal.Update(product);
            _productDal.Add(product);
            return new SuccessResult(Messages.ProductUpdated);
        }

        [SecuredOperation("Product.Update,Admin", Priority = 1)]
        [ValidationAspect(typeof(ProductValidator), Priority = 2)]
        [CacheRemoveAspect("IProductService.Get", Priority = 3)]
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