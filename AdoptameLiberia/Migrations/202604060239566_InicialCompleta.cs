namespace AdoptameLiberia.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InicialCompleta : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Adopcion",
                c => new
                    {
                        ID_Adopcion = c.Int(nullable: false, identity: true),
                        ID_Solicitud = c.Int(nullable: false),
                        ID_Animal = c.Int(nullable: false),
                        Fecha_Adopcion = c.DateTime(nullable: false),
                        Estado_Adopcion = c.String(),
                        Seguimiento_Inicial = c.String(),
                    })
                .PrimaryKey(t => t.ID_Adopcion)
                .ForeignKey("dbo.Animal", t => t.ID_Animal, cascadeDelete: true)
                .ForeignKey("dbo.Solicitud_Adopcion", t => t.ID_Solicitud, cascadeDelete: true)
                .Index(t => t.ID_Solicitud)
                .Index(t => t.ID_Animal);
            
            CreateTable(
                "dbo.Animal",
                c => new
                    {
                        ID_Animal = c.Int(nullable: false, identity: true),
                        Nombre_Animal = c.String(),
                        ID_Raza = c.Int(nullable: false),
                        ID_TipoAnimal = c.Int(nullable: false),
                        Edad = c.Int(),
                        Sexo = c.String(),
                        Tamano = c.String(),
                        Peso = c.Decimal(precision: 18, scale: 2),
                        Descripcion = c.String(),
                        Estado = c.String(),
                    })
                .PrimaryKey(t => t.ID_Animal);
            
            CreateTable(
                "dbo.Solicitud_Adopcion",
                c => new
                    {
                        ID_Solicitud = c.Int(nullable: false, identity: true),
                        ID_Usuario = c.Int(),
                        ID_Animal = c.Int(),
                        Condiciones_Hogar = c.String(maxLength: 300),
                        Motivo_Adopcion = c.String(maxLength: 200),
                        Otros_Animales = c.Boolean(),
                        Fecha_Solicitud = c.DateTime(),
                        Detalle_Otros_Animales = c.String(maxLength: 200),
                        Estado = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.ID_Solicitud);
            
            CreateTable(
                "dbo.CategoriaFinanciera",
                c => new
                    {
                        ID_Categoria = c.Int(nullable: false, identity: true),
                        Nombre = c.String(),
                        Tipo = c.String(),
                        Estado = c.Boolean(),
                        FechaRegistro = c.DateTime(),
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
                        Cantidad = c.Int(nullable: false),
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
                .ForeignKey("dbo.Usuario", t => t.ID_Usuario, cascadeDelete: true)
                .Index(t => t.ID_Usuario)
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
                "dbo.Usuario",
                c => new
                    {
                        ID_Usuario = c.Int(nullable: false, identity: true),
                        ID_Rol = c.Int(nullable: false),
                        Nombre = c.String(),
                        Apellido1 = c.String(),
                        Apellido2 = c.String(),
                        Correo = c.String(),
                        IdAspNetUser = c.String(),
                    })
                .PrimaryKey(t => t.ID_Usuario);
            
            CreateTable(
                "dbo.Gasto",
                c => new
                    {
                        ID_Gasto = c.Int(nullable: false, identity: true),
                        ID_Categoria = c.Int(),
                        Monto = c.Decimal(precision: 18, scale: 2),
                        Descripcion = c.String(),
                        Fecha = c.DateTime(),
                    })
                .PrimaryKey(t => t.ID_Gasto)
                .ForeignKey("dbo.CategoriaFinanciera", t => t.ID_Categoria)
                .Index(t => t.ID_Categoria);
            
            CreateTable(
                "dbo.Modules",
                c => new
                    {
                        ModuleId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 100),
                        Description = c.String(maxLength: 250),
                    })
                .PrimaryKey(t => t.ModuleId);
            
            CreateTable(
                "dbo.RoleModulePermissions",
                c => new
                    {
                        RoleId = c.String(nullable: false, maxLength: 128),
                        ModuleId = c.Int(nullable: false),
                        CanRead = c.Boolean(nullable: false),
                        CanWrite = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => new { t.RoleId, t.ModuleId })
                .ForeignKey("dbo.Modules", t => t.ModuleId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.RoleId)
                .Index(t => t.ModuleId);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Description = c.String(maxLength: 256),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.Noticia",
                c => new
                    {
                        ID_Noticia = c.Int(nullable: false, identity: true),
                        ID_Usuario = c.String(),
                        Titulo = c.String(),
                        Contenido = c.String(),
                        Fecha_Publicacion = c.DateTime(),
                        Estado = c.Boolean(nullable: false),
                        Likes = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID_Noticia);
            
            CreateTable(
                "dbo.Razas",
                c => new
                    {
                        ID_Raza = c.Int(nullable: false, identity: true),
                        NombreRaza = c.String(),
                        Descripcion = c.String(),
                        ID_TipoAnimal = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID_Raza)
                .ForeignKey("dbo.Tipo_Animal", t => t.ID_TipoAnimal, cascadeDelete: true)
                .Index(t => t.ID_TipoAnimal);
            
            CreateTable(
                "dbo.Tipo_Animal",
                c => new
                    {
                        ID_TipoAnimal = c.Int(nullable: false, identity: true),
                        Nombre_Tipo_Animal = c.String(nullable: false, maxLength: 50),
                        Descripcion = c.String(maxLength: 200),
                        Estado = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID_TipoAnimal);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Nombre = c.String(),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Razas", "ID_TipoAnimal", "dbo.Tipo_Animal");
            DropForeignKey("dbo.RoleModulePermissions", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.RoleModulePermissions", "ModuleId", "dbo.Modules");
            DropForeignKey("dbo.Gasto", "ID_Categoria", "dbo.CategoriaFinanciera");
            DropForeignKey("dbo.Donacion", "ID_Usuario", "dbo.Usuario");
            DropForeignKey("dbo.Donacion", "ID_Tipo_Donacion", "dbo.Tipo_Donacion");
            DropForeignKey("dbo.Observacion_Donacion", "ID_Donacion", "dbo.Donacion");
            DropForeignKey("dbo.Detalle_Donacion", "ID_Donacion", "dbo.Donacion");
            DropForeignKey("dbo.Adopcion", "ID_Solicitud", "dbo.Solicitud_Adopcion");
            DropForeignKey("dbo.Adopcion", "ID_Animal", "dbo.Animal");
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.Razas", new[] { "ID_TipoAnimal" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.RoleModulePermissions", new[] { "ModuleId" });
            DropIndex("dbo.RoleModulePermissions", new[] { "RoleId" });
            DropIndex("dbo.Gasto", new[] { "ID_Categoria" });
            DropIndex("dbo.Observacion_Donacion", new[] { "ID_Donacion" });
            DropIndex("dbo.Donacion", new[] { "ID_Tipo_Donacion" });
            DropIndex("dbo.Donacion", new[] { "ID_Usuario" });
            DropIndex("dbo.Detalle_Donacion", new[] { "ID_Donacion" });
            DropIndex("dbo.Adopcion", new[] { "ID_Animal" });
            DropIndex("dbo.Adopcion", new[] { "ID_Solicitud" });
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.Tipo_Animal");
            DropTable("dbo.Razas");
            DropTable("dbo.Noticia");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.RoleModulePermissions");
            DropTable("dbo.Modules");
            DropTable("dbo.Gasto");
            DropTable("dbo.Usuario");
            DropTable("dbo.Tipo_Donacion");
            DropTable("dbo.Observacion_Donacion");
            DropTable("dbo.Donacion");
            DropTable("dbo.Detalle_Donacion");
            DropTable("dbo.CategoriaFinanciera");
            DropTable("dbo.Solicitud_Adopcion");
            DropTable("dbo.Animal");
            DropTable("dbo.Adopcion");
        }
    }
}
