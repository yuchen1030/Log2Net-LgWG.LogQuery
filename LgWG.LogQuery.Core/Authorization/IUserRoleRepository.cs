using Abp.Authorization.Users;
using Abp.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LgWG.LogQuery.Authorization
{
    public interface IUserRoleRepository: IRepository<UserRole, long>
    {
        void mYtt();
    }
}
