namespace AdoptameLiberia.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdRazas : DbMigration
    {
        public override void Up()
        {
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Razas", "ID_TipoAnimal", "dbo.Tipo_Animal");
            DropIndex("dbo.Razas", new[] { "ID_TipoAnimal" });
            DropColumn("dbo.Razas", "ID_TipoAnimal");
            DropTable("dbo.Tipo_Animal");
        }
    }
}
