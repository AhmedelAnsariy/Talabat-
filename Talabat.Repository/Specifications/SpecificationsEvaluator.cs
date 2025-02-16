﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Specifications;

namespace Talabat.Repository.Specifications
{
    public class SpecificationsEvaluator<TEntity> where TEntity : BaseEntity
    {

        public static  IQueryable<TEntity> GetQuery( IQueryable<TEntity> inputQuery , ISpecificatio<TEntity> spec)
        {
            var query = inputQuery;   // Starting of Query ==> Context.Product

            if(spec.Criteria is not null)
            {
                query = query.Where(spec.Criteria);
            } // Context.Product.where(Id == id )



            if(spec.OrderBy is not null)
            {
                query = query.OrderBy(spec.OrderBy);
            }


            if (spec.OrderByDesc is not null)
            {
                query = query.OrderByDescending(spec.OrderByDesc);
            }


            if(spec.IsPaginationEnabled)
            {
                query = query.Skip(spec.Skip).Take(spec.Take);
            }




            query = spec.Includes.Aggregate(query , (CurrentQuery , IncludeExpression)  => CurrentQuery.Include(IncludeExpression));

            return query;
        }






    }
}
