namespace DataConcentrator.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAlarmAndActivatedAlarmTables : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ActivatedAlarms",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AlarmId = c.Int(nullable: false),
                        TagName = c.String(),
                        Message = c.String(),
                        Timestamp = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Alarms", t => t.AlarmId, cascadeDelete: true)
                .Index(t => t.AlarmId);
            
            CreateTable(
                "dbo.Alarms",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        LimitValue = c.Double(nullable: false),
                        Direction = c.Int(nullable: false),
                        Message = c.String(),
                        State = c.Int(nullable: false),
                        TagName = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Tags", t => t.TagName)
                .Index(t => t.TagName);
            
            CreateTable(
                "dbo.Tags",
                c => new
                    {
                        Name = c.String(nullable: false, maxLength: 128),
                        Description = c.String(),
                        Address = c.String(),
                        ScanTime = c.Time(precision: 7),
                        IsOnScan = c.Boolean(),
                        Value = c.Double(),
                        InitValue = c.Double(),
                        ScanTime1 = c.Time(precision: 7),
                        IsOnScan1 = c.Boolean(),
                        InitValue1 = c.Double(),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Name);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ActivatedAlarms", "AlarmId", "dbo.Alarms");
            DropForeignKey("dbo.Alarms", "TagName", "dbo.Tags");
            DropIndex("dbo.Alarms", new[] { "TagName" });
            DropIndex("dbo.ActivatedAlarms", new[] { "AlarmId" });
            DropTable("dbo.Tags");
            DropTable("dbo.Alarms");
            DropTable("dbo.ActivatedAlarms");
        }
    }
}
