using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MVC.DataAccess.Data;
using MVC.DataAccess.Repository.IRepository;

namespace MVC.DataAccess.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {

        private readonly ApplicationDbContext _db;
        internal DbSet<T> dbSet;

        public Repository(ApplicationDbContext db)
        {
            _db = db;
            this.dbSet = _db.Set<T>();
        }
        public void Add(T obj)
        {
            dbSet.Add(obj);
        }

        public void Remove(T obj)
        {
            dbSet.Remove(obj);
        }

        public void RemoveRange(IEnumerable<T> entities)
        {
            dbSet.RemoveRange(entities);
        }

        public T Get(Expression<Func<T, bool>> filter)
        {

            IQueryable<T> querry = dbSet;
            querry = querry.Where(filter);
            return querry.FirstOrDefault();
        }

        public IEnumerable<T> GetAll()
        {
            IQueryable<T> querry = dbSet;
            return querry.ToList();
        }
    }
}
