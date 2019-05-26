using Abp.Authorization.Roles;
using LgWG.LogQuery.Authorization.Users;
using System.ComponentModel.DataAnnotations;


namespace LgWG.LogQuery.Authorization.Roles
{
    public class Role : AbpRole<User>
    {
        public const int MaxDescriptionLength = 5000;

        public Role()
        {
        }

        public Role(int? tenantId, string displayName)
            : base(tenantId, displayName)
        {

        }

        public Role(int? tenantId, string name, string displayName)
            : base(tenantId, name, displayName)
        {

        }

        [MaxLength(MaxDescriptionLength)]
        public string Description { get; set; }

        [Required]
        [MaxLength(MaxDescriptionLength)]

        public string SysCateIDs { get; set; }

    }
}