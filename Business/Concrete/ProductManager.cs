﻿using Business.Abstract;
using Entities.Concrete;
using Business.Constants;
using DataAccess.Abstract;
using Core.Utilities.Results;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Validation;
using Core.Aspects.Autofac.Transaction;
using Business.ValidationRules.FluentValidation;

namespace Business.Concrete
{
    public class ProductManager(IProductDal productDal) : IProductService
    {
        private readonly IProductDal _productDal = productDal;

        [ValidationAspect(typeof(ProductValidator))]
        [CacheRemoveAspect("IProductService.Get")]
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

        [CacheRemoveAspect("IProductService.Get")]
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

        [CacheAspect()]
        public IDataResult<Product> GetById(int productId)
        {
            return new SuccessDataResult<Product>(_productDal.Get(p => p.ProductID == productId));
        }

        [CacheAspect()]
        public IDataResult<List<Product>> GetList()
        {
            return new SuccessDataResult<List<Product>>([.. _productDal.GetList()]);
        }

        [CacheAspect()]
        public IDataResult<List<Product>> GetListByCategory(int categoryId)
        {
            return new SuccessDataResult<List<Product>>([.. _productDal.GetList(p => p.CategoryID == categoryId)]);
        }

        [TransactionScopeAspect]
        [CacheRemoveAspect("IProductService.Get")]
        public IResult TransactionalOperation(Product product)
        {
            _productDal.Update(product);
            _productDal.Add(product);
            return new SuccessResult(Messages.ProductUpdated);
        }

        [ValidationAspect(typeof(ProductValidator))]
        [CacheRemoveAspect("IProductService.Get")]
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