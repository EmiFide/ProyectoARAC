namespace AdoptameLiberia.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateModel : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CategoriaFinancieras",
                c => new
                    {
                        ID_Categoria = c.Int(nullable: false, identity: true),
                        Nombre = c.String(nullable: false),
                        Tipo = c.String(nullable: false),
                        Estado = c.Boolean(nullable: false),
                        FechaRegistro = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID_Categoria);
            
            CreateTable(
                "dbo.Detalle_Donacion",
                c => new
                    {
                        ID_Det_Donacion = c.Int(nullable: false, identity: true),
                        ID_Donacion = c.Int(nullable: false),
                        ID_Item_Inventario = c.Int(),
                        Descripcion = c.String(maxLength: 200),
                    })
                .PrimaryKey(t => t.ID_Det_Donacion)
                .ForeignKey("dbo.Donacion", t => t.ID_Donacion, cascadeDelete: true)
                .Index(t => t.ID_Donacion);
            
            CreateTable(
                "dbo.Donacion",
                c => new
                    {
                        ID_Donacion = c.Int(nullable: false, identity: true),
                        ID_Usuario = c.Int(nullable: false),
                        ID_Tipo_Donacion = c.Int(nullable: false),
                        Monto = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Fecha = c.DateTime(nullable: false),
                        Metodo = c.String(maxLength: 50),
                        Descripcion = c.String(maxLength: 200),
                        Fecha_Registro = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID_Donacion)
                .ForeignKey("dbo.Tipo_Donacion", t => t.ID_Tipo_Donacion, cascadeDelete: true)
                .Index(t => t.ID_Tipo_Donacion);
            
            CreateTable(
                "dbo.Observacion_Donacion",
                c => new
                    {
                        ID_Observacion = c.Int(nullable: false, identity: true),
                        ID_Donacion = c.Int(nullable: false),
                        Comentario = c.String(nullable: false, maxLength: 400),
                        Fecha = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID_Observacion)
                .ForeignKey("dbo.Donacion", t => t.ID_Donacion, cascadeDelete: true)
                .Index(t => t.ID_Donacion);
            
            CreateTable(
                "dbo.Tipo_Donacion",
                c => new
                    {
                        ID_Tipo_Donacion = c.Int(nullable: false, identity: true),
                        Nombre = c.String(maxLength: 50),
                        Descripcion = c.String(maxLength: 200),
                        Fecha_Registro = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID_Tipo_Donacion);
            
            CreateTable(
                "dbo.Gastoes",
                c => new
                    {
                        ID_Gasto = c.Int(nullable: false, identity: true),
                        ID_Categoria = c.Int(nullable: false),
                        Monto = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Descripcion = c.String(),
                        Fecha = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID_Gasto)
                .ForeignKey("dbo.CategoriaFinancieras", t => t.ID_Categoria, cascadeDelete: true)
                .Index(t => t.ID_Categoria);
            
            CreateTable(
                "dbo.Noticias",
                c => new
                    {
                        ID_Noticia = c.Int(nullable: false, identity: true),
                        ID_Usuario = c.String(),
                        Titulo = c.String(nullable: false),
                        Contenido = c.String(nullable: false),
                        Fecha_Publicacion = c.DateTime(nullable: false),
                        Estado = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID_Noticia);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Gastoes", "ID_Categoria", "dbo.CategoriaFinancieras");
            DropForeignKey("dbo.Donacion", "ID_Tipo_Donacion", "dbo.Tipo_Donacion");
            DropForeignKey("dbo.Observacion_Donacion", "ID_Donacion", "dbo.Donacion");
            DropForeignKey("dbo.Detalle_Donacion", "ID_Donacion", "dbo.Donacion");
            DropIndex("dbo.Gastoes", new[] { "ID_Categoria" });
            DropIndex("dbo.Observacion_Donacion", new[] { "ID_Donacion" });
            DropIndex("dbo.Donacion", new[] { "ID_Tipo_Donacion" });
            DropIndex("dbo.Detalle_Donacion", new[] { "ID_Donacion" });
            DropTable("dbo.Noticias");
            DropTable("dbo.Gastoes");
            DropTable("dbo.Tipo_Donacion");
            DropTable("dbo.Observacion_Donacion");
            DropTable("dbo.Donacion");
            DropTable("dbo.Detalle_Donacion");
            DropTable("dbo.CategoriaFinancieras");
        }
    }
}
