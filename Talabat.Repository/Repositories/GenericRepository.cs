using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Interfaces;
using Talabat.Core.Specifications;
using Talabat.Repository.Data;
using Talabat.Repository.Specifications;

namespace Talabat.Repository.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        private readonly StoreDbContext _context;

        public GenericRepository( StoreDbContext context)
        {
           _context = context;
        }

        public async Task<IReadOnlyList<T>> GetAllAsync()
        {
            if (typeof(T) == typeof(Product))
            {
                var products = await _context.Set<Product>().Include(p => p.Brand).ToListAsync();
                return products as List<T>;
            }
            return await _context.Set<T>().ToListAsync();
        }


        public async Task<T?> GetByIdAsync(int id)
        {
            if (typeof(T) == typeof(Product))
            {
                return await _context.Products
                    .Where(p => p.Id == id)
                    //.Include(p=>p.Category)
                    //.Include(p=>p.Brand)
                    .FirstOrDefaultAsync() as T;
            }

            return await _context.Set<T>().FindAsync(id);
        }



        public async Task<IReadOnlyList<T>> GetAllWithSpecAsync(ISpecificatio<T> spec )
        {
           return await SpecificationsEvaluator<T>.GetQuery(_context.Set<T>() , spec).ToListAsync();
        }




        public async Task<T?> GetWithSpecByIdAsync(ISpecificatio<T> spec)
        {
           return await SpecificationsEvaluator<T>.GetQuery(_context.Set<T>() , spec).FirstOrDefaultAsync();
        }

        public async Task<int> GetCountAsync(ISpecificatio<T> spec)
        {
            return await SpecificationsEvaluator<T>.GetQuery(_context.Set<T>(), spec).CountAsync();
        }



        public async Task AddAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
        }

        public void Delete(T entity)
        {
             _context.Set<T>().Remove(entity);

        }

        public void Update(T entity)
        {
            _context.Set<T>().Update(entity);

        }
    }
}
