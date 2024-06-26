﻿using DataAccess.Abstract;
using Core.Entities.Concrete;
using Core.DataAccess.EntityFramework;
using DataAccess.Concrete.EntityFramework.Contexts;

namespace DataAccess.Concrete.EntityFramework
{
    public class EfUserDal : EfEntityRepositoryBase<User, NorthwindContext>, IUserDal
    {
        public List<OperationClaim> GetClaims(User user)
        {
            var context = new NorthwindContext();
            var result = from operationClaim in context.OperationClaims
                         join userOperationClaim in context.UserOperationClaims
                         on operationClaim.OperationClaimID equals userOperationClaim.OperationClaimID
                         where userOperationClaim.UserID == user.UserID
                         select new OperationClaim
                         {
                             OperationClaimID = operationClaim.OperationClaimID,
                             Name = operationClaim.Name
                         };
            return [.. result];
        }
    }
}