using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Cynthia.Card
{
    public interface IRepository<TModel> : IRepository, IQueryable<TModel>, ICollection<TModel>
    {
        TModel this[string id] { get; set; }
        void Add(IEnumerable<TModel> items);
        int Remove(Expression<Func<TModel, bool>> predicate);
        bool Update(TModel item);
        int Update(IEnumerable<TModel> items);
        int Update(Expression<Func<TModel, bool>> predicate, TModel item);
    }
}