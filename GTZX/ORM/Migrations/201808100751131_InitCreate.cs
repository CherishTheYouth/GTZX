namespace ORM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitCreate : DbMigration
    {
        public override void Up()
        {
            DropTable("dbo.ArcGisMap");
            DropTable("dbo.Sys_LocationRelation");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Sys_LocationRelation",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        TargetId = c.Guid(nullable: false),
                        Name = c.String(maxLength: 100),
                        Remark = c.String(),
                        Type = c.Int(nullable: false),
                        LocationDetail = c.String(),
                        CreateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ArcGisMap",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        MapUrl = c.String(maxLength: 100),
                        MapApiUrl = c.String(maxLength: 100),
                        IsDelete = c.Boolean(nullable: false),
                        CreateTime = c.DateTime(),
                        ModifyTime = c.DateTime(),
                        CreateBy = c.Guid(),
                        ModifyBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
    }
}
