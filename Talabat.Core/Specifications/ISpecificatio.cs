﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;

namespace Talabat.Core.Specifications
{
    public interface ISpecificatio<T> where T : BaseEntity
    {

        public Expression<Func<T, bool>> Criteria { get; set; }   // Where 

        public List<Expression<Func<T,object>>> Includes { get; set; }

        public Expression<Func<T, object>> OrderBy { get; set; }
        public Expression<Func<T, object>> OrderByDesc { get; set; }




        // Skip Take
        public int Skip { get; set; }
        public int Take { get; set; }




        public bool IsPaginationEnabled { get; set; }




    }
}
