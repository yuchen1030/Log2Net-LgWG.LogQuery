using LgWG.LogQuery.EntityFramework;
using EntityFramework.DynamicFilters;

namespace LgWG.LogQuery.Migrations.SeedData
{
    public class InitialHostDbBuilder
    {
        private readonly LogQueryDbContext _context;

        public InitialHostDbBuilder(LogQueryDbContext context)
        {
            _context = context;
        }

        public void Create()
        {
            _context.DisableAllFilters();

            new DefaultEditionsCreator(_context).Create();
            new DefaultLanguagesCreator(_context).Create();
            new HostRoleAndUserCreator(_context).Create();
            new DefaultSettingsCreator(_context).Create();
        }
    }
}
