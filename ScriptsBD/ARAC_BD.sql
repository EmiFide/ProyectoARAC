CREATE DATABASE ARAC_DB;
GO

USE ARAC_DB;
GO


/*Tabla de roles*/
CREATE TABLE Rol (
    ID_Rol INT IDENTITY PRIMARY KEY,
    Nombre_Rol VARCHAR(50) NOT NULL,
    Descripcion VARCHAR(200),
    Estado BIT DEFAULT 1,
    Fecha_Creacion DATETIME DEFAULT GETDATE()
);

INSERT INTO Rol (Nombre_Rol, Descripcion)
VALUES 
('Administrador', 'Control total del sistema'),
('Voluntario', 'Apoyo en campañas y gestión'),
('Adoptante', 'Usuario interesado en adoptar');


/*Tabla de usuarios*/
CREATE TABLE Usuario (
    ID_Usuario INT IDENTITY PRIMARY KEY,
    ID_Rol INT NOT NULL,
    Nombre VARCHAR(50),
    Apellido1 VARCHAR(50),
    Apellido2 VARCHAR(50),
    Correo VARCHAR(100),
    Fecha_Creacion DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_Usuario_Rol FOREIGN KEY (ID_Rol) REFERENCES Rol(ID_Rol)
);

INSERT INTO Usuario (ID_Rol, Nombre, Apellido1, Apellido2, Correo)
VALUES
(1, 'Carlos', 'Ramírez', 'López', 'admin@refugio.com'),
(2, 'Ana', 'Mora', 'Jiménez', 'ana@correo.com'),
(3, 'Luis', 'Soto', 'Vargas', 'luis@gmail.com');


/*Tabla de tipo de animal*/
CREATE TABLE Tipo_Animal (
    ID_TipoAnimal INT IDENTITY PRIMARY KEY,
    Nombre_Tipo_Animal VARCHAR(50),
    Descripcion VARCHAR(200),
    Estado BIT DEFAULT 1,
    Fecha_Creacion DATETIME DEFAULT GETDATE()
);

INSERT INTO Tipo_Animal (Nombre_Tipo_Animal, Descripcion)
VALUES
('Perro', 'Canino doméstico'),
('Gato', 'Felino doméstico');


/*Tabla de raza*/
CREATE TABLE Raza (
    ID_Raza INT IDENTITY PRIMARY KEY,
    Nombre VARCHAR(50),
    Descripcion VARCHAR(200),
    Fecha_Creacion DATETIME DEFAULT GETDATE()
);

INSERT INTO Raza (Nombre, Descripcion)
VALUES
('Labrador', 'Raza mediana y amigable'),
('Criollo', 'Sin raza definida');


/*Tabla de animal*/
CREATE TABLE Animal (
    ID_Animal INT IDENTITY PRIMARY KEY,
    Nombre_Animal VARCHAR(50),
    ID_Raza INT,
    ID_TipoAnimal INT,
    Edad INT,
    Sexo VARCHAR(10),
    Tamano VARCHAR(20),
    Peso DECIMAL(6,2),
    Descripcion VARCHAR(300),
    Fecha_Registro DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_Animal_Raza FOREIGN KEY (ID_Raza) REFERENCES Raza(ID_Raza),
    CONSTRAINT FK_Animal_Tipo FOREIGN KEY (ID_TipoAnimal) REFERENCES Tipo_Animal(ID_TipoAnimal)
);

INSERT INTO Animal
(Nombre_Animal, ID_Raza, ID_TipoAnimal, Edad, Sexo, Tamano, Peso, Descripcion)
VALUES
('Max', 1, 1, 3, 'Macho', 'Grande', 28.5, 'Muy juguetón'),
('Luna', 2, 2, 2, 'Hembra', 'Pequeño', 4.2, 'Cariñosa y tranquila');


/*Tabla de perfil animal*/
CREATE TABLE Perfil_Animal (
    ID_Perfil_Animal INT IDENTITY PRIMARY KEY,
    ID_Animal INT,
    Nivel_Energia VARCHAR(50),
    Nivel_Socializacion VARCHAR(50),
    Temperamento VARCHAR(50),
    Convivencia VARCHAR(100),
    Nivel_Entrenamiento VARCHAR(50),
    Necesidades_Especiales VARCHAR(200),
    Sintesis VARCHAR(300),
    CONSTRAINT FK_Perfil_Animal FOREIGN KEY (ID_Animal) REFERENCES Animal(ID_Animal)
);

INSERT INTO Perfil_Animal
(ID_Animal, Nivel_Energia, Nivel_Socializacion, Temperamento, Convivencia,
 Nivel_Entrenamiento, Necesidades_Especiales, Sintesis)
VALUES
(1, 'Alta', 'Alta', 'Amigable', 'Niños y otros perros', 'Básico', 'Ninguna', 'Ideal para familia activa'),
(2, 'Media', 'Media', 'Tranquila', 'Adultos', 'Ninguno', 'Control veterinario', 'Perfecta para apartamento');


/*Tabla de publicacion comunidad*/
CREATE TABLE Publicacion_Comunidad (
    ID_Publicacion INT IDENTITY PRIMARY KEY,
    ID_Usuario INT,
    ID_Categoria INT,
    Titulo VARCHAR(100),
    Contenido VARCHAR(500),
    Fecha DATETIME DEFAULT GETDATE(),
    Estado BIT DEFAULT 1,
    CONSTRAINT FK_Publicacion_Usuario FOREIGN KEY (ID_Usuario) REFERENCES Usuario(ID_Usuario)
);

INSERT INTO Publicacion_Comunidad
(ID_Usuario, ID_Categoria, Titulo, Contenido)
VALUES
(2, 1, 'Jornada de adopción', 'Este sábado tendremos jornada de adopción'),
(3, 1, 'Donaciones recibidas', 'Gracias por el apoyo recibido');


/*Tabla de notificaciones correo*/
CREATE TABLE NotificacionCorreo (
    ID_Notificacion INT IDENTITY PRIMARY KEY,
    ID_Usuario INT,
    Asunto VARCHAR(100),
    Mensaje VARCHAR(500),
    Fecha_Envio DATETIME,
    Estado_Envio VARCHAR(50),
    CONSTRAINT FK_Notificacion_Usuario FOREIGN KEY (ID_Usuario) REFERENCES Usuario(ID_Usuario)
);

INSERT INTO NotificacionCorreo
(ID_Usuario, Asunto, Mensaje, Fecha_Envio, Estado_Envio)
VALUES
(3, 'Estado de solicitud', 'Su solicitud está en revisión', GETDATE(), 'Enviado');


/*Tabla de tipo donacion*/
CREATE TABLE Tipo_Donacion (
    ID_Tipo_Donacion INT IDENTITY PRIMARY KEY,
    Nombre VARCHAR(50),
    Descripcion VARCHAR(200),
    Fecha_Registro DATETIME DEFAULT GETDATE()
);

INSERT INTO Tipo_Donacion (Nombre, Descripcion)
VALUES
('Monetaria', 'Donación en efectivo'),
('Insumos', 'Alimentos o artículos');


/*Tabla de donacion*/
CREATE TABLE Donacion (
    ID_Donacion INT IDENTITY PRIMARY KEY,
    ID_Usuario INT,
    ID_Tipo_Donacion INT,
    Monto DECIMAL(10,2),
    Fecha DATETIME DEFAULT GETDATE(),
    Metodo VARCHAR(50),
    Descripcion VARCHAR(200),
    Fecha_Registro DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_Donacion_Usuario FOREIGN KEY (ID_Usuario) REFERENCES Usuario(ID_Usuario),
    CONSTRAINT FK_Donacion_Tipo FOREIGN KEY (ID_Tipo_Donacion) REFERENCES Tipo_Donacion(ID_Tipo_Donacion)
);

INSERT INTO Donacion
(ID_Usuario, ID_Tipo_Donacion, Monto, Metodo, Descripcion)
VALUES
(3, 1, 50000, 'Transferencia', 'Apoyo mensual'),
(2, 2, 0, 'Entrega directa', 'Donación de alimentos');


/*Tabla de item inventario*/
CREATE TABLE Item_Inventario (
    ID_Item_Inventario INT IDENTITY PRIMARY KEY,
    Nombre VARCHAR(100),
    Descripcion VARCHAR(200),
    Categoria VARCHAR(50),
    Unidad_Medida VARCHAR(20),
    Stock_Actual INT,
    Estado BIT DEFAULT 1,
    Fecha_Registro DATETIME DEFAULT GETDATE()
);

INSERT INTO Item_Inventario
(Nombre, Descripcion, Categoria, Unidad_Medida, Stock_Actual)
VALUES
('Alimento para perro', 'Bolsa 20kg', 'Alimentos', 'Saco', 15),
('Medicamento', 'Antiparasitario', 'Salud', 'Unidad', 30);


/*Tabla de los movimientos de los inventario*/
CREATE TABLE Movimiento_Inventario (
    ID_Movimiento_Inventario INT IDENTITY PRIMARY KEY,
    ID_Item_Inventario INT,
    Tipo_Movimiento VARCHAR(20),
    Fecha_Movimiento DATETIME DEFAULT GETDATE(),
    Motivo VARCHAR(200),
    CONSTRAINT FK_Movimiento_Item FOREIGN KEY (ID_Item_Inventario) REFERENCES Item_Inventario(ID_Item_Inventario)
);

INSERT INTO Movimiento_Inventario
(ID_Item_Inventario, Tipo_Movimiento, Motivo)
VALUES
(1, 'Entrada', 'Donación recibida'),
(2, 'Salida', 'Uso veterinario');


/*Tabla de detalle donacion*/
CREATE TABLE Detalle_Donacion (
    ID_Det_Donacion INT IDENTITY PRIMARY KEY,
    ID_Donacion INT,
    ID_Item_Inventario INT,
    Descripcion VARCHAR(200),
    CONSTRAINT FK_Detalle_Donacion FOREIGN KEY (ID_Donacion) REFERENCES Donacion(ID_Donacion),
    CONSTRAINT FK_Detalle_Item FOREIGN KEY (ID_Item_Inventario) REFERENCES Item_Inventario(ID_Item_Inventario)
);

INSERT INTO Detalle_Donacion
(ID_Donacion, ID_Item_Inventario, Descripcion)
VALUES
(2, 1, '5 sacos de alimento');


/*Tabla de los campaña castracion*/
CREATE TABLE Campania_Castracion (
    ID_Campania INT IDENTITY PRIMARY KEY,
    Nombre VARCHAR(100),
    Descripcion VARCHAR(200),
    Ubicacion VARCHAR(100),
    Costo_Por_Animal DECIMAL(10,2),
    Hora_Inicio TIME,
    Hora_Finalizacion TIME,
    Fecha DATE,
    Estado BIT DEFAULT 1
);

INSERT INTO Campania_Castracion
(Nombre, Descripcion, Ubicacion, Costo_Por_Animal, Hora_Inicio, Hora_Finalizacion, Fecha)
VALUES
('Castración Solidaria', 'Campaña comunitaria', 'San José', 15000, '08:00', '16:00', '2026-03-10');


/*Tabla de participante campaña*/
CREATE TABLE Participante_Campania (
    ID_Participante_Campania INT IDENTITY PRIMARY KEY,
    ID_Campania INT,
    ID_Usuario INT,
    Hora_Programada TIME,
    Estado_Participacion VARCHAR(50),
    Detalles VARCHAR(200),
    Fecha_Registro DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_Participante_Campania FOREIGN KEY (ID_Campania) REFERENCES Campania_Castracion(ID_Campania),
    CONSTRAINT FK_Participante_Usuario FOREIGN KEY (ID_Usuario) REFERENCES Usuario(ID_Usuario)
);

INSERT INTO Participante_Campania
(ID_Campania, ID_Usuario, Hora_Programada, Estado_Participacion, Detalles)
VALUES
(1, 2, '09:30', 'Confirmado', 'Perro adulto');


/*Tabla de gasto campaña*/
CREATE TABLE Gasto_Campania (
    ID_Gasto_Campania INT IDENTITY PRIMARY KEY,
    ID_Campania INT,
    Monto DECIMAL(10,2),
    Fecha DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_Gasto_Campania FOREIGN KEY (ID_Campania) REFERENCES Campania_Castracion(ID_Campania)
);

INSERT INTO Gasto_Campania
(ID_Campania, Monto)
VALUES
(1, 120000);


/*Tabla de solicitud adopcion*/
CREATE TABLE Solicitud_Adopcion (
    ID_Solicitud INT IDENTITY PRIMARY KEY,
    ID_Usuario INT,
    ID_Animal INT,
    Condiciones_Hogar VARCHAR(300),
    Motivo_Adopcion VARCHAR(200),
    Otros_Animales BIT,
    Fecha_Solicitud DATETIME DEFAULT GETDATE(),
    Detalle_Otros_Animales VARCHAR(200),
    Estado VARCHAR(50),
    CONSTRAINT FK_Solicitud_Usuario FOREIGN KEY (ID_Usuario) REFERENCES Usuario(ID_Usuario),
    CONSTRAINT FK_Solicitud_Animal FOREIGN KEY (ID_Animal) REFERENCES Animal(ID_Animal)
);

INSERT INTO Solicitud_Adopcion
(ID_Usuario, ID_Animal, Condiciones_Hogar, Motivo_Adopcion,
 Otros_Animales, Detalle_Otros_Animales, Estado)
VALUES
(3, 1, 'Casa con patio', 'Compañía familiar', 0, NULL, 'En revisión');


/*Tabla de adopcion*/
CREATE TABLE Adopcion (
    ID_Adopcion INT IDENTITY PRIMARY KEY,
    ID_Solicitud INT,
    ID_Animal INT,
    Fecha_Adopcion DATETIME,
    Estado_Adopcion VARCHAR(50),
    Seguimiento_Inicial VARCHAR(300),
    CONSTRAINT FK_Adopcion_Solicitud FOREIGN KEY (ID_Solicitud) REFERENCES Solicitud_Adopcion(ID_Solicitud),
    CONSTRAINT FK_Adopcion_Animal FOREIGN KEY (ID_Animal) REFERENCES Animal(ID_Animal)
);


ALTER TABLE Animal
ADD Estado VARCHAR(20) NOT NULL DEFAULT 'Disponible';


CREATE TABLE Historial_Medico (
    ID_Historial INT IDENTITY PRIMARY KEY,
    ID_Animal INT NOT NULL,
    Fecha DATETIME NOT NULL DEFAULT GETDATE(),
    Tipo VARCHAR(40) NOT NULL,          -- Vacuna, Desparasitacion, Cirugia, Tratamiento
    Detalle VARCHAR(600) NULL,
    CONSTRAINT FK_Historial_Animal FOREIGN KEY (ID_Animal) REFERENCES Animal(ID_Animal)
);



INSERT INTO Adopcion
(ID_Solicitud, ID_Animal, Fecha_Adopcion, Estado_Adopcion, Seguimiento_Inicial)
VALUES
(1, 1, GETDATE(), 'Aprobada', 'Visita en 15 días');





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

--Crear el usuario con el correo ADMIN
SELECT TOP (1000) [UserId]
      ,[RoleId]
  FROM [ARAC_DB].[dbo].[AspNetUserRoles]

DECLARE @UserId NVARCHAR(128) = (SELECT TOP 1 Id FROM AspNetUsers WHERE Email = 'franffv0809@gmail.com');
DECLARE @RoleId NVARCHAR(128) = (SELECT TOP 1 Id FROM AspNetRoles WHERE Name = 'Administrador');

IF NOT EXISTS (SELECT 1 FROM AspNetUserRoles WHERE UserId = @UserId AND RoleId = @RoleId)
BEGIN
  INSERT INTO AspNetUserRoles (UserId, RoleId) VALUES (@UserId, @RoleId);
END

--Bryan, esto sirve para la parte de donaciones--
IF OBJECT_ID('dbo.Observacion_Donacion', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Observacion_Donacion
    (
        ID_Observacion INT IDENTITY PRIMARY KEY,
        ID_Donacion INT NOT NULL,
        Comentario VARCHAR(400) NOT NULL,
        Fecha DATETIME NOT NULL DEFAULT GETDATE(),
        CONSTRAINT FK_ObsDon_Donacion FOREIGN KEY (ID_Donacion) 
            REFERENCES dbo.Donacion(ID_Donacion)
            ON DELETE CASCADE
    );
END
GO
