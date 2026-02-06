-- =============================================
-- ARAC / AdoptameLiberia - Roles y Permisos
-- Compatible con SQL Server (ASP.NET Identity 2)
-- =============================================

/*
INSTRUCCIONES:
1) Ejecuta este script sobre la misma BD que usa tu "DefaultConnection".
2) Si aún no existe la BD (primera vez), lo usual es dejar que EF cree el esquema de Identity
   al correr la app. Luego ejecutas este script para extenderlo.
*/

USE ARAC_DB;

-- 1) Agregar columna Description a AspNetRoles (para roles personalizados)
IF COL_LENGTH('dbo.AspNetRoles', 'Description') IS NULL
BEGIN
    ALTER TABLE dbo.AspNetRoles
    ADD [Description] NVARCHAR(256) NULL;
END
GO

-- 2) Tabla Modules
IF OBJECT_ID('dbo.Modules', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Modules
    (
        ModuleId INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Modules PRIMARY KEY,
        [Name] NVARCHAR(100) NOT NULL,
        [Description] NVARCHAR(250) NULL,
        CONSTRAINT UQ_Modules_Name UNIQUE ([Name])
    );
END
GO

-- 3) Tabla RoleModulePermissions (permisos por rol y módulo)
IF OBJECT_ID('dbo.RoleModulePermissions', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.RoleModulePermissions
    (
        RoleId NVARCHAR(128) NOT NULL,
        ModuleId INT NOT NULL,
        CanRead BIT NOT NULL CONSTRAINT DF_RoleModulePermissions_CanRead DEFAULT(0),
        CanWrite BIT NOT NULL CONSTRAINT DF_RoleModulePermissions_CanWrite DEFAULT(0),
        CONSTRAINT PK_RoleModulePermissions PRIMARY KEY (RoleId, ModuleId),
        CONSTRAINT FK_RoleModulePermissions_Role FOREIGN KEY (RoleId) REFERENCES dbo.AspNetRoles(Id) ON DELETE CASCADE,
        CONSTRAINT FK_RoleModulePermissions_Module FOREIGN KEY (ModuleId) REFERENCES dbo.Modules(ModuleId) ON DELETE CASCADE
    );
END
GO

-- 4) Seed básico de módulos (si no existen)
IF NOT EXISTS (SELECT 1 FROM dbo.Modules WHERE [Name] = 'Animales')
    INSERT INTO dbo.Modules([Name],[Description]) VALUES ('Animales','Gestión de animales rescatados');

IF NOT EXISTS (SELECT 1 FROM dbo.Modules WHERE [Name] = 'Adopciones')
    INSERT INTO dbo.Modules([Name],[Description]) VALUES ('Adopciones','Gestión de procesos de adopción');

IF NOT EXISTS (SELECT 1 FROM dbo.Modules WHERE [Name] = 'Donaciones')
    INSERT INTO dbo.Modules([Name],[Description]) VALUES ('Donaciones','Gestión de donaciones');

IF NOT EXISTS (SELECT 1 FROM dbo.Modules WHERE [Name] = 'Usuarios')
    INSERT INTO dbo.Modules([Name],[Description]) VALUES ('Usuarios','Gestión de usuarios');

IF NOT EXISTS (SELECT 1 FROM dbo.Modules WHERE [Name] = 'Roles y permisos')
    INSERT INTO dbo.Modules([Name],[Description]) VALUES ('Roles y permisos','Gestión de roles y permisos');

-- 5) (Opcional) Crear roles base si no existen
IF NOT EXISTS (SELECT 1 FROM dbo.AspNetRoles WHERE [Name] = 'Administrador')
    INSERT INTO dbo.AspNetRoles(Id, [Name], Description) VALUES (NEWID(), 'Administrador', 'Acceso total al sistema');

IF NOT EXISTS (SELECT 1 FROM dbo.AspNetRoles WHERE [Name] = 'Colaborador')
    INSERT INTO dbo.AspNetRoles(Id, [Name], Description) VALUES (NEWID(), 'Colaborador', 'Acceso operativo (lectura/escritura según permisos)');

IF NOT EXISTS (SELECT 1 FROM dbo.AspNetRoles WHERE [Name] = 'Lector')
    INSERT INTO dbo.AspNetRoles(Id, [Name], Description) VALUES (NEWID(), 'Lector', 'Acceso solo lectura');

-- 6) (Opcional) Dar permisos full al Administrador
DECLARE @AdminRoleId NVARCHAR(128) = (SELECT TOP 1 Id FROM dbo.AspNetRoles WHERE [Name] = 'Administrador');

IF @AdminRoleId IS NOT NULL
BEGIN
    INSERT INTO dbo.RoleModulePermissions(RoleId, ModuleId, CanRead, CanWrite)
    SELECT @AdminRoleId, m.ModuleId, 1, 1
    FROM dbo.Modules m
    WHERE NOT EXISTS (
        SELECT 1 FROM dbo.RoleModulePermissions p
        WHERE p.RoleId = @AdminRoleId AND p.ModuleId = m.ModuleId
    );
END
GO
