using LgWG.LogQuery.BasicRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Abp.Domain.Entities;
using Abp.EntityFramework;
using Abp.EntityFramework.Repositories;
using LgWG.LogQuery.LogMonitor;
using System.Collections.Generic;
using System.Linq;

namespace LgWG.LogQuery.EntityFramework.Repositories
{

    public class BaseRepository<dbContext, TEntity, TPrimaryKey> : BaseRepositoryBase<dbContext, TEntity, TPrimaryKey>, IBaseRepository<TEntity, TPrimaryKey>
   where TEntity : class, IEntity<TPrimaryKey>
       where dbContext : AbpDbContext
    {

        public BaseRepository(IDbContextProvider<dbContext> dbContextProvider) : base(dbContextProvider)
        {

        }


    }


    public abstract class BaseRepositoryBase<dbContext, TEntity, TPrimaryKey> : EfRepositoryBase<dbContext, TEntity, TPrimaryKey>
    where TEntity : class, IEntity<TPrimaryKey>
        where dbContext : AbpDbContext
    {
        protected BaseRepositoryBase(IDbContextProvider<dbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        //add common methods for all repositories
    }

    public abstract class BaseRepositoryBase<dbContext, TEntity> : BaseRepositoryBase<dbContext, TEntity, int>
        where TEntity : class, IEntity<int>
        where dbContext : AbpDbContext
    {
        protected BaseRepositoryBase(IDbContextProvider<dbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        //do not add any method here, add to the class above (since this inherits it)
    }







}
