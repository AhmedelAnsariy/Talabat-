using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Specifications;

namespace Talabat.Core.Interfaces
{
    public interface IGenericRepository<T> where T : BaseEntity
    {
        Task<T?> GetByIdAsync(int id);
        Task<IReadOnlyList<T>> GetAllAsync();




        Task<T?> GetWithSpecByIdAsync(ISpecificatio<T> spec);
        Task<IReadOnlyList<T>> GetAllWithSpecAsync(ISpecificatio<T> spec);

        Task AddAsync(T entity);
        void Delete(T entity);
        void Update(T entity);






        Task<int> GetCountAsync(ISpecificatio<T> spec);

    }
}
