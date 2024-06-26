﻿using Business.Abstract;
using Entities.Concrete;
using Business.Constants;
using DataAccess.Abstract;
using Core.Utilities.Results;
using Core.Utilities.Business;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Validation;
using Business.BusinessAspects.Autofac;
using Core.Aspects.Autofac.Performance;
using Core.Aspects.Autofac.Transaction;
using Business.ValidationRules.FluentValidation;

namespace Business.Concrete
{
    public class ProductManager(IProductDal productDal, ICategoryService categoryService) : IProductService
    {
        private readonly IProductDal _productDal = productDal;
        private readonly ICategoryService _categoryService = categoryService;

        [SecuredOperation("Product.Add,Admin", Priority = 1)]
        [ValidationAspect(typeof(ProductValidator), Priority = 2)]
        [CacheRemoveAspect("IProductService.Get", Priority = 3)]
        public IResult Add(Product product)
        {
            try
            {
                IResult result = BusinessRules.Run(CheckIfProductNameExists(product.ProductName), CheckIfProductIsEnabled());
                if (result != null)
                {
                    return result;
                }
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
        public IDataResult<Product> GetById(int productId)
        {
            return new SuccessDataResult<Product>(_productDal.Get(p => p.ProductID == productId));
        }

        [SecuredOperation("Product.GetList,Admin", Priority = 1)]
        [CacheAspect(Priority = 2)]
        [PerformanceAspect(interval: 5, Priority = 3)]
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

        private IResult CheckIfProductNameExists(string productName)
        {
            var result = _productDal.GetList(x => x.ProductName == productName).Any();
            if (result)
            {
                return new ErrorResult(Messages.ProductNameAlreadyExists);
            }
            return new SuccessResult();
        }

        private IResult CheckIfProductIsEnabled()
        {
            var result = _categoryService.GetList();
            if (result.Data.Count < 10)
            {
                return new ErrorResult(Messages.ProductNameAlreadyExists);
            }
            return new SuccessResult();
        }
    }
}