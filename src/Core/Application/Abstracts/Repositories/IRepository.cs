using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Application.Abstracts.Repositories;

public interface IRepository<T> where T : BaseEntity
{
    //Read IRepository
    Task<T?> GetByIdAsync(Guid id);
    IQueryable<T> GetAll(bool isTracking = false);


    IQueryable<T> GetByFiltered(Expression<Func<T, bool>> predicate,
                         Expression<Func<T, object>>[]? include = null,
                         bool isTracking = false);
    IQueryable<T> GetAllFiltered(Expression<Func<T, bool>> predicate,
                         Expression<Func<T, object>>[]? include = null,
                         Expression<Func<T, object>>? orderby = null,
                         bool isOrderByAsc = true,
                         bool isTracking = false);
    //Write IRepository
    Task AddAsync(T entity);
    void Delete(T entity);
    void Update(T entity);
    Task SaveChangeAsync();
}
