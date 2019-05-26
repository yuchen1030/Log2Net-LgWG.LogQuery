using Abp.Domain.Entities;
using Abp.EntityFramework;
using Abp.EntityFramework.Repositories;

namespace LgWG.LogQuery.EntityFramework.Repositories
{
    public abstract class LogQueryRepositoryBase<TEntity, TPrimaryKey> : EfRepositoryBase<LogQueryDbContext, TEntity, TPrimaryKey>
        where TEntity : class, IEntity<TPrimaryKey>
    {
        protected LogQueryRepositoryBase(IDbContextProvider<LogQueryDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        //add common methods for all repositories
    }

    public abstract class LogQueryRepositoryBase<TEntity> : LogQueryRepositoryBase<TEntity, int>
        where TEntity : class, IEntity<int>
    {
        protected LogQueryRepositoryBase(IDbContextProvider<LogQueryDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        //do not add any method here, add to the class above (since this inherits it)
    }


}
