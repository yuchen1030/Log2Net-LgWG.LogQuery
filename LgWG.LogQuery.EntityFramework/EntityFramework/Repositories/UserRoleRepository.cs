using Abp.Authorization.Users;
using Abp.EntityFramework;
using LgWG.LogQuery.Authorization;
using System;

namespace LgWG.LogQuery.EntityFramework.Repositories
{

    //<UserRole, long>
    public class UserRoleRepository: BaseRepository<LogQueryDbContext, UserRole, long>, IUserRoleRepository
    {
        public UserRoleRepository(IDbContextProvider<LogQueryDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        public void mYtt()
        {
            throw new NotImplementedException();
        }


    }

}
