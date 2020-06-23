namespace R.A.D.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TTTTT : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.RentProductsTbl", "Renting_Period", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.RentProductsTbl", "Renting_Period", c => c.DateTime());
        }
    }
}
