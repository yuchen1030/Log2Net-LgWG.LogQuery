namespace LgWG.LogQuery.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSysCateIDs : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AbpRoles", "SysCateIDs", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AbpRoles", "SysCateIDs");
        }
    }
}
