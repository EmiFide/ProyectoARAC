CREATE DATABASE ARAC_DB;
GO

USE ARAC_DB;
GO

-- TABLAS PRINCIPALES
CREATE TABLE Rol (
    ID_Rol INT IDENTITY PRIMARY KEY,
    Nombre_Rol VARCHAR(50) NOT NULL,
    Descripcion VARCHAR(200),
    Estado BIT DEFAULT 1,
    Fecha_Creacion DATETIME DEFAULT GETDATE()
);
GO

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
GO

CREATE TABLE Tipo_Animal (
    ID_TipoAnimal INT IDENTITY PRIMARY KEY,
    Nombre_Tipo_Animal VARCHAR(50),
    Descripcion VARCHAR(200),
    Estado BIT DEFAULT 1,
    Fecha_Creacion DATETIME DEFAULT GETDATE()
);
GO

CREATE TABLE Raza (
    ID_Raza INT IDENTITY PRIMARY KEY,
    Nombre VARCHAR(50),
    Descripcion VARCHAR(200),
    Fecha_Creacion DATETIME DEFAULT GETDATE()
);
GO

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
GO

-- ALTERACIONES DE ANIMAL
ALTER TABLE Animal
ADD Estado VARCHAR(20) NOT NULL DEFAULT 'Disponible';
GO

ALTER TABLE Animal
ADD NombreRaza NVARCHAR(100),
    NombreTipo NVARCHAR(100);
GO

-- TABLAS RELACIONADAS CON ANIMALES
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
GO

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
GO

CREATE TABLE NotificacionCorreo (
    ID_Notificacion INT IDENTITY PRIMARY KEY,
    ID_Usuario INT,
    Asunto VARCHAR(100),
    Mensaje VARCHAR(500),
    Fecha_Envio DATETIME,
    Estado_Envio VARCHAR(50),
    CONSTRAINT FK_Notificacion_Usuario FOREIGN KEY (ID_Usuario) REFERENCES Usuario(ID_Usuario)
);
GO

-- TABLAS DE DONACIONES E INVENTARIO
CREATE TABLE Tipo_Donacion (
    ID_Tipo_Donacion INT IDENTITY PRIMARY KEY,
    Nombre VARCHAR(50),
    Descripcion VARCHAR(200),
    Fecha_Registro DATETIME DEFAULT GETDATE()
);
GO

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
GO

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
GO

-- ALTERACIONES DE INVENTARIO
IF COL_LENGTH('dbo.Item_Inventario', 'Stock_Minimo') IS NULL
BEGIN
    ALTER TABLE dbo.Item_Inventario
    ADD Stock_Minimo INT NOT NULL CONSTRAINT DF_ItemInventario_StockMinimo DEFAULT(0);
END
GO

IF COL_LENGTH('dbo.Item_Inventario', 'Fecha_Caducidad') IS NULL
BEGIN
    ALTER TABLE dbo.Item_Inventario
    ADD Fecha_Caducidad DATE NULL;
END
GO

CREATE TABLE Movimiento_Inventario (
    ID_Movimiento_Inventario INT IDENTITY PRIMARY KEY,
    ID_Item_Inventario INT,
    Tipo_Movimiento VARCHAR(20),
    Fecha_Movimiento DATETIME DEFAULT GETDATE(),
    Motivo VARCHAR(200),
    CONSTRAINT FK_Movimiento_Item FOREIGN KEY (ID_Item_Inventario) REFERENCES Item_Inventario(ID_Item_Inventario)
);
GO

-- ALTERACIONES DE MOVIMIENTO DE INVENTARIO
IF COL_LENGTH('dbo.Movimiento_Inventario', 'Cantidad') IS NULL
BEGIN
    ALTER TABLE dbo.Movimiento_Inventario
    ADD Cantidad INT NOT NULL CONSTRAINT DF_MovInv_Cantidad DEFAULT(0);
END
GO

IF COL_LENGTH('dbo.Movimiento_Inventario', 'Stock_Anterior') IS NULL
BEGIN
    ALTER TABLE dbo.Movimiento_Inventario
    ADD Stock_Anterior INT NULL;
END
GO

IF COL_LENGTH('dbo.Movimiento_Inventario', 'Stock_Nuevo') IS NULL
BEGIN
    ALTER TABLE dbo.Movimiento_Inventario
    ADD Stock_Nuevo INT NULL;
END
GO

IF COL_LENGTH('dbo.Movimiento_Inventario', 'Destinatario') IS NULL
BEGIN
    ALTER TABLE dbo.Movimiento_Inventario
    ADD Destinatario NVARCHAR(100) NULL;
END
GO

IF COL_LENGTH('dbo.Movimiento_Inventario', 'UsuarioId') IS NULL
BEGIN
    ALTER TABLE dbo.Movimiento_Inventario
    ADD UsuarioId NVARCHAR(128) NULL;
END
GO

CREATE TABLE Detalle_Donacion (
    ID_Det_Donacion INT IDENTITY PRIMARY KEY,
    ID_Donacion INT,
    ID_Item_Inventario INT,
    Descripcion VARCHAR(200),
    CONSTRAINT FK_Detalle_Donacion FOREIGN KEY (ID_Donacion) REFERENCES Donacion(ID_Donacion),
    CONSTRAINT FK_Detalle_Item FOREIGN KEY (ID_Item_Inventario) REFERENCES Item_Inventario(ID_Item_Inventario)
);
GO

-- TABLAS DE CAMPAÑAS
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
GO

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
GO

CREATE TABLE Gasto_Campania (
    ID_Gasto_Campania INT IDENTITY PRIMARY KEY,
    ID_Campania INT,
    Monto DECIMAL(10,2),
    Fecha DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_Gasto_Campania FOREIGN KEY (ID_Campania) REFERENCES Campania_Castracion(ID_Campania)
);
GO

-- TABLAS DE ADOPCIONES
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
GO

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
GO

CREATE TABLE Historial_Medico (
    ID_Historial INT IDENTITY PRIMARY KEY,
    ID_Animal INT NOT NULL,
    Fecha DATETIME NOT NULL DEFAULT GETDATE(),
    Tipo VARCHAR(40) NOT NULL,
    Detalle VARCHAR(600) NULL,
    CONSTRAINT FK_Historial_Animal FOREIGN KEY (ID_Animal) REFERENCES Animal(ID_Animal)
);
GO

-- TABLAS ADICIONALES
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

CREATE TABLE CampaniasCastracion (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Nombre NVARCHAR(100),
    Fecha DATE,
    Lugar NVARCHAR(150),
    Cupos INT
);
GO

CREATE TABLE InscripcionesCastracion (
    Id INT PRIMARY KEY IDENTITY(1,1),
    CampaniaCastracionId INT NOT NULL,
    AnimalId INT NOT NULL,
    VeterinarioAsignado NVARCHAR(100) NULL,
    Resultado NVARCHAR(200) NULL,
    FOREIGN KEY (CampaniaCastracionId) REFERENCES CampaniasCastracion(Id),
    FOREIGN KEY (AnimalId) REFERENCES Animal(ID_Animal)
);
GO

CREATE TABLE Razas (
    ID_Raza INT PRIMARY KEY IDENTITY(1,1),
    NombreRaza NVARCHAR(100) NOT NULL,
    Descripcion NVARCHAR(255) NULL,
    ID_TipoAnimal INT NOT NULL,
    CONSTRAINT FK_Raza_TipoAnimal
        FOREIGN KEY (ID_TipoAnimal)
        REFERENCES Tipo_Animal(ID_TipoAnimal)
);
GO

CREATE TABLE Voluntario (
    ID_Voluntario INT IDENTITY PRIMARY KEY,
    ID_Usuario INT NULL,
    Nombre VARCHAR(100) NOT NULL,
    Apellido1 VARCHAR(50) NULL,
    Apellido2 VARCHAR(50) NULL,
    Correo VARCHAR(100) NOT NULL,
    Telefono VARCHAR(30) NOT NULL,
    Disponibilidad VARCHAR(200) NULL,
    Habilidades VARCHAR(300) NULL,
    Estado BIT NOT NULL DEFAULT 1,
    Fecha_Registro DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_Voluntario_Usuario FOREIGN KEY (ID_Usuario) REFERENCES Usuario(ID_Usuario)
);
GO

CREATE TABLE Tarea_Voluntariado (
    ID_Tarea INT IDENTITY PRIMARY KEY,
    Titulo VARCHAR(100) NOT NULL,
    Descripcion VARCHAR(300) NOT NULL,
    Fecha DATE NOT NULL,
    Hora TIME NULL,
    Lugar VARCHAR(100) NULL,
    Estado BIT NOT NULL DEFAULT 1,
    Fecha_Registro DATETIME NOT NULL DEFAULT GETDATE()
);
GO

CREATE TABLE Participacion_Voluntario (
    ID_Participacion INT IDENTITY PRIMARY KEY,
    ID_Voluntario INT NOT NULL,
    ID_Tarea INT NOT NULL,
    Asistio BIT NOT NULL DEFAULT 0,
    Observaciones VARCHAR(300) NULL,
    Fecha_Registro DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_Participacion_Voluntario FOREIGN KEY (ID_Voluntario) REFERENCES Voluntario(ID_Voluntario),
    CONSTRAINT FK_Participacion_Tarea FOREIGN KEY (ID_Tarea) REFERENCES Tarea_Voluntariado(ID_Tarea)
);
GO

CREATE TABLE Comentario_Publicacion (
    ID_Comentario INT IDENTITY PRIMARY KEY,
    ID_Publicacion INT NOT NULL,
    ID_Usuario INT NOT NULL,
    Contenido VARCHAR(500) NOT NULL,
    Fecha DATETIME NOT NULL DEFAULT GETDATE(),
    Estado BIT NOT NULL DEFAULT 1,
    CONSTRAINT FK_Comentario_Publicacion FOREIGN KEY (ID_Publicacion) REFERENCES Publicacion_Comunidad(ID_Publicacion),
    CONSTRAINT FK_Comentario_Usuario FOREIGN KEY (ID_Usuario) REFERENCES Usuario(ID_Usuario)
);
GO

-- INSERTS PRINCIPALES
INSERT INTO Rol (Nombre_Rol, Descripcion)
VALUES 
('Administrador', 'Control total del sistema'),
('Usuario', 'Usuario general del sistema, puede consultar información y realizar procesos permitidos'),
('Voluntario', 'Persona que participa en actividades de apoyo, jornadas y tareas del refugio');
GO

INSERT INTO Usuario (ID_Rol, Nombre, Apellido1, Apellido2, Correo)
VALUES
(2, 'Mariana', 'López', 'Vargas', 'mariana.lopez@correo.com'),
(3, 'Kevin', 'Mora', 'Solano', 'kevin.mora@correo.com'),
(2, 'Andrea', 'Ramírez', 'Castro', 'andrea.ramirez@correo.com'),
(3, 'Luis', 'Fernández', 'Rojas', 'luis.fernandez@correo.com'),
(2, 'Sofía', 'Jiménez', 'León', 'sofia.jimenez@correo.com'),
(3, 'Daniel', 'Cordero', 'Núñez', 'daniel.cordero@correo.com'),
(2, 'Valeria', 'Soto', 'Mena', 'valeria.soto@correo.com'),
(3, 'José', 'Araya', 'Chaves', 'jose.araya@correo.com'),
(2, 'Camila', 'Vargas', 'Pérez', 'camila.vargas@correo.com'),
(3, 'Esteban', 'Herrera', 'Campos', 'esteban.herrera@correo.com'),
(2, 'Paula', 'Rojas', 'Segura', 'paula.rojas@correo.com'),
(3, 'Fernando', 'Salazar', 'Mora', 'fernando.salazar@correo.com'),
(2, 'Natalia', 'Ureña', 'Solís', 'natalia.urena@correo.com'),
(3, 'Javier', 'Zamora', 'Durán', 'javier.zamora@correo.com'),
(2, 'Melissa', 'Alvarado', 'Céspedes', 'melissa.alvarado@correo.com'),
(3, 'Lucía', 'Castro', 'Rojas', 'lucia.castro@correo.com'),
(2, 'Marco', 'Segura', 'León', 'marco.segura@correo.com'),
(3, 'Elena', 'Quesada', 'Mora', 'elena.quesada@correo.com');
GO

INSERT INTO Tipo_Animal (Nombre_Tipo_Animal, Descripcion, Estado)
VALUES
('Perro', 'Animal doméstico leal, activo y protector, ideal para compañía y seguridad en el hogar',1),
('Gato', 'Animal doméstico independiente, limpio y tranquilo, ideal para espacios pequeños y compañía relajada',1);
GO


SELECT * FROM Animal;

DBCC CHECKIDENT ('Animal', RESEED, 0);

DELETE FROM Animal;

INSERT INTO Raza (Nombre, Descripcion)
VALUES
('Labrador', 'Perro amigable y familiar'),
('Pastor Alemán', 'Perro inteligente y protector'),
('Chihuahua', 'Perro pequeño y energético'),
('Poodle', 'Perro inteligente y de fácil entrenamiento'),
('Bulldog', 'Perro tranquilo y robusto'),
('Beagle', 'Perro curioso y activo'),
('Husky', 'Perro fuerte y resistente'),
('Rottweiler', 'Perro protector y leal'),
('Siamés', 'Gato activo y vocal'),
('Persa', 'Gato tranquilo y elegante'),
('Maine Coon', 'Gato grande y sociable'),
('Bengalí', 'Gato energético y juguetón'),
('Azul Ruso', 'Gato reservado y elegante'),
('Angora', 'Gato de pelaje largo'),
('Esfinge', 'Gato sin pelo, muy sociable');
GO

INSERT INTO Animal
(Nombre_Animal, ID_Raza, ID_TipoAnimal, Edad, Sexo, Tamano, Peso, Descripcion, NombreRaza, NombreTipo, Estado)
VALUES
('Max', 1, 1, 3, 'Macho', 'Grande', 28.5, 'Perro muy juguetón y amigable', 'Labrador', 'Perro', 'Disponible'),
('Rocky', 2, 1, 5, 'Macho', 'Grande', 30.0, 'Protector y obediente', 'Pastor Alemán', 'Perro', 'Disponible'),
('Luna', 3, 1, 2, 'Hembra', 'Pequeño', 3.2, 'Muy activa y cariñosa', 'Chihuahua', 'Perro', 'Disponible'),
('Toby', 4, 1, 4, 'Macho', 'Mediano', 12.5, 'Inteligente y fácil de entrenar', 'Poodle', 'Perro', 'Disponible'),
('Bruno', 5, 1, 6, 'Macho', 'Mediano', 20.0, 'Tranquilo y leal', 'Bulldog', 'Perro', 'Disponible'),
('Simba', 6, 1, 3, 'Macho', 'Mediano', 14.0, 'Le encanta correr', 'Beagle', 'Perro', 'Disponible'),
('Thor', 7, 1, 4, 'Macho', 'Grande', 25.0, 'Muy energético', 'Husky', 'Perro', 'Disponible'),
('Zeus', 8, 1, 5, 'Macho', 'Grande', 32.0, 'Protector del hogar', 'Rottweiler', 'Perro', 'Disponible'),
('Mia', 9, 2, 2, 'Hembra', 'Pequeño', 3.5, 'Muy sociable', 'Siamés', 'Gato', 'Disponible'),
('Nina', 10, 2, 3, 'Hembra', 'Pequeño', 4.0, 'Tranquila y cariñosa', 'Persa', 'Gato', 'Disponible'),
('Leo', 11, 2, 4, 'Macho', 'Grande', 6.5, 'Muy amigable', 'Maine Coon', 'Gato', 'Disponible'),
('Tigre', 12, 2, 2, 'Macho', 'Mediano', 5.0, 'Activo y juguetón', 'Bengalí', 'Gato', 'Disponible'),
('Nube', 13, 2, 3, 'Hembra', 'Pequeño', 3.8, 'Reservada', 'Azul Ruso', 'Gato', 'Disponible'),
('Lola', 14, 2, 1, 'Hembra', 'Pequeño', 3.2, 'Muy elegante', 'Angora', 'Gato', 'Disponible'),
('Kira', 15, 2, 2, 'Hembra', 'Pequeño', 3.0, 'Muy sociable', 'Esfinge', 'Gato', 'Disponible');
GO

INSERT INTO Perfil_Animal
(ID_Animal, Nivel_Energia, Nivel_Socializacion, Temperamento, Convivencia,
 Nivel_Entrenamiento, Necesidades_Especiales, Sintesis)
VALUES
(1, 'Alta', 'Alta', 'Juguetón', 'Familia', 'Básico', 'Ninguna', 'Ideal para familias'),
(2, 'Media', 'Media', 'Protector', 'Adultos', 'Avanzado', 'Ejercicio diario', 'Buen guardián'),
(3, 'Alta', 'Alta', 'Activo', 'Niños', 'Básico', 'Ninguna', 'Pequeño y energético'),
(4, 'Media', 'Alta', 'Inteligente', 'Familia', 'Intermedio', 'Ninguna', 'Fácil de entrenar'),
(5, 'Baja', 'Media', 'Tranquilo', 'Apartamentos', 'Ninguno', 'Control de peso', 'Muy calmado'),
(6, 'Alta', 'Alta', 'Curioso', 'Familia', 'Básico', 'Ejercicio', 'Le gusta explorar'),
(7, 'Alta', 'Media', 'Activo', 'Adultos', 'Intermedio', 'Espacio amplio', 'Muy energético'),
(8, 'Media', 'Alta', 'Protector', 'Familia', 'Avanzado', 'Entrenamiento', 'Fiel guardián'),
(9, 'Media', 'Alta', 'Sociable', 'Familia', 'Ninguno', 'Ninguna', 'Muy cariñosa'),
(10, 'Baja', 'Media', 'Tranquilo', 'Apartamentos', 'Ninguno', 'Cepillado', 'Ideal para interiores'),
(11, 'Media', 'Alta', 'Amigable', 'Familia', 'Ninguno', 'Espacio', 'Muy sociable'),
(12, 'Alta', 'Media', 'Activo', 'Adultos', 'Ninguno', 'Juguetes', 'Muy juguetón'),
(13, 'Baja', 'Media', 'Reservado', 'Adultos', 'Ninguno', 'Ambiente tranquilo', 'Muy calmado'),
(14, 'Media', 'Alta', 'Elegante', 'Familia', 'Ninguno', 'Cepillado', 'Muy limpio'),
(15, 'Alta', 'Alta', 'Sociable', 'Familia', 'Ninguno', 'Protección térmica', 'Muy amigable');
GO

INSERT INTO Publicacion_Comunidad
(ID_Usuario, ID_Categoria, Titulo, Contenido)
VALUES
(2, 1, 'Jornada de adopción este fin de semana', 'Este sábado tendremos jornada de adopción con varios perros y gatos disponibles.'),
(3, 1, 'Gracias por las donaciones recibidas', 'Agradecemos a todas las personas que han apoyado con alimento, medicinas y accesorios.'),
(4, 2, 'Se busca hogar para perro rescatado', 'Tenemos un perro muy noble que necesita una familia responsable y amorosa.'),
(5, 2, 'Gatita en adopción responsable', 'Una gatita tranquila y sociable está lista para encontrar un hogar definitivo.'),
(6, 3, 'Necesitamos voluntarios para limpieza', 'Este viernes ocupamos apoyo para limpieza general del refugio en la mañana.'),
(7, 3, 'Campaña de castración abierta', 'Ya están abiertas las inscripciones para la próxima campaña de castración.'),
(8, 1, 'Nuevo ingreso de alimento al refugio', 'Gracias a varias donaciones, logramos reabastecer parte del inventario de alimento.'),
(9, 2, 'Perro mediano disponible para adopción', 'Es un perro leal, activo y muy bueno con familias que tengan espacio.'),
(10, 2, 'Gato tranquilo busca hogar', 'Tenemos un gato ideal para apartamento, muy limpio y de temperamento calmado.'),
(11, 3, 'Recordatorio sobre adopción responsable', 'Adoptar implica compromiso, tiempo, cuidados médicos y mucho cariño.'),
(12, 1, 'Agradecimiento especial a donadores', 'Queremos agradecer a quienes apoyaron con cobijas, comida y medicamentos.'),
(13, 3, 'Necesitamos hogares temporales', 'Estamos buscando hogares temporales para animales en proceso de recuperación.'),
(14, 2, 'Conoce a nuestros gatos en adopción', 'Tenemos varios gatos con diferentes personalidades listos para encontrar familia.'),
(15, 1, 'Historias felices de adopción', 'Compartimos algunos casos de adopciones exitosas que nos motivan a seguir.'),
(16, 3, 'Invitación a jornada comunitaria', 'Les invitamos a participar en la próxima actividad del refugio y apoyar la causa.');
GO

INSERT INTO NotificacionCorreo
(ID_Usuario, Asunto, Mensaje, Fecha_Envio, Estado_Envio)
VALUES
(1, 'Recordatorio campaña', 'Su mascota está agendada', GETDATE(), 'Pendiente'),
(2, 'Estado de solicitud', 'Su solicitud está en revisión', GETDATE(), 'Enviado'),
(3, 'Solicitud recibida', 'Su solicitud de adopción fue recibida', GETDATE(), 'Enviado'),
(4, 'Campaña confirmada', 'Su espacio en la campaña fue reservado', GETDATE(), 'Enviado'),
(5, 'Donación registrada', 'Gracias por su aporte', GETDATE(), 'Enviado'),
(6, 'Seguimiento adopción', 'Nos comunicaremos pronto', GETDATE(), 'Pendiente'),
(7, 'Cita veterinaria', 'Su cita fue programada', GETDATE(), 'Enviado'),
(8, 'Nueva campaña', 'Ya hay nueva jornada disponible', GETDATE(), 'Enviado'),
(9, 'Recordatorio', 'Recuerde asistir mañana', GETDATE(), 'Pendiente'),
(10, 'Aprobación', 'Su proceso fue aprobado', GETDATE(), 'Enviado'),
(11, 'Actualización', 'Hubo un cambio en su estado', GETDATE(), 'Enviado'),
(12, 'Bienvenida', 'Gracias por registrarse', GETDATE(), 'Enviado'),
(13, 'Recibo de donación', 'Su donación quedó registrada', GETDATE(), 'Pendiente'),
(14, 'Invitación', 'Se le invita a participar como voluntario', GETDATE(), 'Enviado'),
(15, 'Seguimiento inicial', 'Queremos saber cómo va todo', GETDATE(), 'Enviado');
GO

INSERT INTO Tipo_Donacion (Nombre, Descripcion)
VALUES
('Monetaria', 'Donación en efectivo'),
('Insumos', 'Alimentos o artículos'),
('Medicinas', 'Donación de medicamentos veterinarios'),
('Limpieza', 'Productos de limpieza para refugio'),
('Accesorios', 'Correas, platos y camas'),
('Higiene', 'Champú y artículos de aseo'),
('Arena', 'Arena sanitaria para gatos'),
('Cobijas', 'Cobertores y ropa de cama'),
('Juguetes', 'Juguetes para perros y gatos'),
('Veterinaria', 'Aporte para procedimientos médicos'),
('Transporte', 'Apoyo para traslados'),
('General', 'Aporte económico general');
GO

INSERT INTO Donacion
(ID_Usuario, ID_Tipo_Donacion, Monto, Metodo, Descripcion)
VALUES
(1, 1, 50000, 'Transferencia', 'Apoyo mensual'),
(2, 2, 0, 'Entrega directa', 'Donación de alimentos'),
(3, 1, 25000, 'Sinpe', 'Aporte para vacunas'),
(4, 2, 0, 'Entrega directa', 'Cloro y jabón'),
(5, 3, 0, 'Entrega directa', 'Correas y collares'),
(6, 4, 12000, 'Transferencia', 'Productos de higiene'),
(7, 5, 0, 'Entrega directa', 'Arena sanitaria'),
(8, 6, 0, 'Entrega directa', 'Cobijas para perros'),
(9, 7, 8000, 'Sinpe', 'Compra de juguetes'),
(10, 8, 30000, 'Transferencia', 'Gastos veterinarios'),
(11, 9, 15000, 'Sinpe', 'Apoyo para transporte'),
(12, 10, 10000, 'Efectivo', 'Donación general'),
(13, 1, 20000, 'Transferencia', 'Medicamentos'),
(14, 2, 0, 'Entrega directa', 'Desinfectantes'),
(15, 3, 0, 'Entrega directa', 'Comederos'),
(16, 8, 40000, 'Sinpe', 'Apoyo a cirugías'),
(17, 10, 5000, 'Efectivo', 'Aporte solidario');
GO

INSERT INTO Item_Inventario
(Nombre, Descripcion, Categoria, Unidad_Medida, Stock_Actual, Stock_Minimo, Fecha_Caducidad)
VALUES
('Alimento perro cachorro', 'Bolsa 10kg', 'Alimentos', 'Saco', 10, 4, '2026-10-10'),
('Alimento gato adulto', 'Bolsa 8kg', 'Alimentos', 'Saco', 12, 5, '2026-09-20'),
('Arena sanitaria', 'Arena para gatos', 'Higiene', 'Bolsa', 18, 6, '2027-01-15'),
('Shampoo canino', 'Aseo para perros', 'Higiene', 'Botella', 9, 3, '2026-08-30'),
('Antiparasitario', 'Uso veterinario', 'Salud', 'Unidad', 20, 8, '2026-12-01'),
('Gasas', 'Material médico', 'Salud', 'Paquete', 25, 10, '2027-02-01'),
('Guantes desechables', 'Uso médico y limpieza', 'Salud', 'Caja', 14, 5, '2027-03-20'),
('Collares', 'Accesorio para perros y gatos', 'Accesorios', 'Unidad', 16, 5, NULL),
('Correas', 'Accesorio para paseos', 'Accesorios', 'Unidad', 11, 4, NULL),
('Comederos', 'Plato para alimento', 'Accesorios', 'Unidad', 13, 4, NULL),
('Bebederos', 'Plato para agua', 'Accesorios', 'Unidad', 13, 4, NULL),
('Cobijas', 'Abrigo para animales', 'Textiles', 'Unidad', 17, 6, NULL),
('Desinfectante', 'Limpieza de instalaciones', 'Limpieza', 'Galón', 8, 3, '2027-01-10'),
('Cepillos', 'Aseo para pelaje', 'Higiene', 'Unidad', 10, 4, NULL),
('Toallas', 'Secado y limpieza', 'Textiles', 'Unidad', 19, 7, NULL);
GO

INSERT INTO Movimiento_Inventario
(ID_Item_Inventario, Tipo_Movimiento, Fecha_Movimiento, Motivo, Cantidad, Stock_Anterior, Stock_Nuevo, Destinatario)
VALUES
(1, 'Entrada', GETDATE(), 'Compra mensual', 5, 10, 15, 'Bodega'),
(2, 'Entrada', GETDATE(), 'Donación recibida', 4, 12, 16, 'Área de gatos'),
(3, 'Salida', GETDATE(), 'Uso diario', 3, 18, 15, 'Refugio'),
(4, 'Salida', GETDATE(), 'Baño de perros', 2, 9, 7, 'Área aseo'),
(5, 'Entrada', GETDATE(), 'Compra veterinaria', 8, 20, 28, 'Consultorio'),
(6, 'Salida', GETDATE(), 'Curaciones', 4, 25, 21, 'Veterinaria'),
(7, 'Entrada', GETDATE(), 'Reposición', 5, 14, 19, 'Consultorio'),
(8, 'Salida', GETDATE(), 'Entrega a adoptante', 2, 16, 14, 'Adoptante'),
(9, 'Entrada', GETDATE(), 'Donación', 3, 11, 14, 'Bodega'),
(10, 'Salida', GETDATE(), 'Uso diario', 2, 13, 11, 'Refugio'),
(11, 'Salida', GETDATE(), 'Uso diario', 2, 13, 11, 'Refugio'),
(12, 'Entrada', GETDATE(), 'Donación textil', 4, 17, 21, 'Lavandería'),
(13, 'Salida', GETDATE(), 'Desinfección', 1, 8, 7, 'Instalaciones'),
(14, 'Entrada', GETDATE(), 'Compra', 3, 10, 13, 'Bodega'),
(15, 'Salida', GETDATE(), 'Limpieza', 4, 19, 15, 'Refugio');
GO

INSERT INTO Detalle_Donacion
(ID_Donacion, ID_Item_Inventario, Descripcion)
VALUES
(3, 5, 'Medicamentos para control veterinario'),
(4, 13, 'Productos desinfectantes'),
(5, 8, 'Collares y accesorios'),
(6, 14, 'Shampoo y cepillos'),
(7, 3, 'Arena sanitaria para gatos'),
(8, 12, 'Cobijas para refugio'),
(9, 10, 'Juguetes y accesorios'),
(10, 5, 'Apoyo para compra de antiparasitarios'),
(11, 9, 'Apoyo de transporte'),
(12, 1, 'Donación económica general'),
(13, 5, 'Compra de medicamentos'),
(14, 13, 'Limpieza general'),
(15, 10, 'Comederos nuevos'),
(16, 6, 'Apoyo para cirugía'),
(17, 2, 'Aporte solidario al alimento');
GO

INSERT INTO Campania_Castracion
(Nombre, Descripcion, Ubicacion, Costo_Por_Animal, Hora_Inicio, Hora_Finalizacion, Fecha)
VALUES
('Campaña Liberia Norte', 'Castración para perros y gatos', 'Liberia Norte', 12000, '08:00', '15:00', '2026-04-10'),
('Campaña Nicoya Centro', 'Jornada comunitaria', 'Nicoya', 13000, '08:00', '16:00', '2026-04-17'),
('Campaña Santa Cruz', 'Atención preventiva animal', 'Santa Cruz', 12500, '09:00', '15:00', '2026-04-24'),
('Campaña Bagaces', 'Castración de mascotas', 'Bagaces', 11000, '08:30', '14:30', '2026-05-01'),
('Campaña Carrillo', 'Jornada social', 'Carrillo', 10000, '08:00', '14:00', '2026-05-08'),
('Campaña Upala', 'Atención a animales de comunidad', 'Upala', 14000, '08:00', '16:00', '2026-05-15'),
('Campaña Tilarán', 'Bienestar animal', 'Tilarán', 12000, '08:30', '15:30', '2026-05-22'),
('Campaña Cañas', 'Control responsable', 'Cañas', 11500, '07:30', '13:30', '2026-05-29'),
('Campaña Hojancha', 'Operativo de castración', 'Hojancha', 13500, '08:00', '16:00', '2026-06-05'),
('Campaña Abangares', 'Apoyo a familias de bajos recursos', 'Abangares', 9500, '08:00', '14:00', '2026-06-12'),
('Campaña Nandayure', 'Jornada médica y castración', 'Nandayure', 13000, '09:00', '16:00', '2026-06-19'),
('Campaña Sardinal', 'Mascotas seguras', 'Sardinal', 12000, '08:00', '15:00', '2026-06-26'),
('Campaña Filadelfia', 'Atención comunitaria', 'Filadelfia', 12500, '08:30', '15:30', '2026-07-03'),
('Campaña Belén', 'Castración y orientación', 'Belén', 11000, '08:00', '14:00', '2026-07-10'),
('Campaña La Cruz', 'Bienestar para perros y gatos', 'La Cruz', 14500, '08:00', '16:30', '2026-07-17'),
('Campaña Carrizal', 'Jornada solidaria de castración', 'Carrizal', 11800, '08:00', '15:00', '2026-07-24');
GO

INSERT INTO Participante_Campania
(ID_Campania, ID_Usuario, Hora_Programada, Estado_Participacion, Detalles)
VALUES
(2, 3, '08:30', 'Confirmado', 'Perro macho'),
(3, 4, '09:00', 'Confirmado', 'Gata adulta'),
(4, 5, '09:30', 'Pendiente', 'Perra pequeña'),
(5, 6, '10:00', 'Confirmado', 'Gato macho'),
(6, 7, '10:30', 'Confirmado', 'Dos mascotas'),
(7, 8, '11:00', 'Pendiente', 'Perro mediano'),
(8, 9, '11:30', 'Confirmado', 'Gata rescatada'),
(9, 10, '12:00', 'Confirmado', 'Perro adulto'),
(10, 11, '12:30', 'Pendiente', 'Mascota comunitaria'),
(11, 12, '13:00', 'Confirmado', 'Gata pequeña'),
(12, 13, '13:30', 'Confirmado', 'Perro criollo'),
(13, 14, '14:00', 'Pendiente', 'Animal familiar'),
(14, 15, '14:30', 'Confirmado', 'Gato rescatado'),
(15, 16, '15:00', 'Confirmado', 'Perro joven'),
(16, 17, '15:30', 'Pendiente', 'Gato comunitario');
GO

INSERT INTO Gasto_Campania
(ID_Campania, Monto)
VALUES
(2, 90000),
(3, 98000),
(4, 87000),
(5, 76000),
(6, 110000),
(7, 95000),
(8, 89000),
(9, 103000),
(10, 72000),
(11, 97000),
(12, 88000),
(13, 93000),
(14, 85000),
(15, 101000),
(16, 112000);
GO

INSERT INTO Solicitud_Adopcion
(ID_Usuario, ID_Animal, Condiciones_Hogar, Motivo_Adopcion, Otros_Animales, Detalle_Otros_Animales, Estado)
VALUES
(4, 2, 'Casa con patio cerrado', 'Busca un perro protector y de compañía para la familia', 0, NULL, 'En revisión'),
(5, 9, 'Apartamento amplio con mallas de seguridad', 'Desea adoptar una gata sociable para compañía', 0, NULL, 'Pendiente'),
(6, 4, 'Casa con jardín y espacio para jugar', 'Quiere un perro activo para convivir con sus hijos', 1, 'Un gato adulto tranquilo', 'Aprobada'),
(7, 10, 'Apartamento ordenado y tranquilo', 'Busca una gata calmada y de interior', 0, NULL, 'En revisión'),
(8, 7, 'Casa grande con patio', 'Desea adoptar un perro enérgico para hacer ejercicio diario', 1, 'Otro perro macho mediano', 'Pendiente'),
(9, 11, 'Casa familiar con amplios espacios', 'Quiere darle hogar a un gato grande y amigable', 0, NULL, 'Aprobada'),
(10, 5, 'Casa con patio seguro', 'Le gustó el temperamento tranquilo del perro y quiere adoptarlo', 1, 'Una perrita pequeña esterilizada', 'En revisión'),
(11, 13, 'Apartamento silencioso y seguro', 'Desea adoptar una gata reservada y tranquila', 0, NULL, 'Pendiente'),
(12, 8, 'Casa en zona rural con bastante espacio', 'Busca un perro guardián y leal para el hogar', 1, 'Dos perros adultos', 'Aprobada'),
(13, 12, 'Apartamento con enriquecimiento y juguetes', 'Quiere un gato activo y juguetón para compañía', 0, NULL, 'En revisión'),
(14, 3, 'Casa con patio y tiempo para paseos diarios', 'Desea un perro pequeño y cariñoso', 0, NULL, 'Pendiente'),
(15, 14, 'Casa tranquila con ventanas protegidas', 'Busca una gata elegante y dócil para compañía', 1, 'Un gato macho adulto', 'Aprobada'),
(16, 6, 'Casa con jardín amplio', 'Quiere adoptar un perro curioso y sociable', 0, NULL, 'En revisión'),
(17, 15, 'Apartamento con ambiente cálido y seguro', 'Desea una gata muy sociable y de fácil convivencia', 0, NULL, 'Pendiente'),
(18, 1, 'Casa familiar con patio grande', 'Busca un perro juguetón y amigable para toda la familia', 1, 'Un perro senior tranquilo', 'Aprobada');
GO

INSERT INTO Adopcion (ID_Solicitud, ID_Animal, Fecha_Adopcion, Estado_Adopcion, Seguimiento_Inicial) 
VALUES 
(3, 5, GETDATE(), 'Aprobada', 'Llamada en 7 días'), 
(6, 8, GETDATE(), 'Aprobada', 'Visita en 15 días'), (9, 11, GETDATE(), 'Aprobada', 'Seguimiento por correo'), 
(12, 14, GETDATE(), 'Aprobada', 'Llamada de control'), 
(1, 3, GETDATE(), 'Aprobada', 'Visita de seguimiento inicial');
GO

INSERT INTO Historial_Medico (ID_Animal, Fecha, Tipo, Detalle)
VALUES
(1, GETDATE(), 'Vacuna', 'Vacuna antirrábica aplicada'),
(2, GETDATE(), 'Desparasitacion', 'Desparasitación interna completada'),
(3, GETDATE(), 'Tratamiento', 'Tratamiento por dermatitis leve'),
(4, GETDATE(), 'Vacuna', 'Vacuna múltiple aplicada'),
(5, GETDATE(), 'Cirugia', 'Esterilización realizada sin complicaciones'),
(6, GETDATE(), 'Tratamiento', 'Control por resfriado leve'),
(7, GETDATE(), 'Vacuna', 'Refuerzo anual aplicado'),
(8, GETDATE(), 'Desparasitacion', 'Control preventivo mensual'),
(9, GETDATE(), 'Tratamiento', 'Limpieza de herida superficial'),
(10, GETDATE(), 'Vacuna', 'Primera dosis aplicada'),
(11, GETDATE(), 'Cirugia', 'Castración exitosa'),
(12, GETDATE(), 'Tratamiento', 'Revisión general satisfactoria'),
(13, GETDATE(), 'Vacuna', 'Esquema básico completo'),
(14, GETDATE(), 'Desparasitacion', 'Desparasitación externa aplicada'),
(15, GETDATE(), 'Tratamiento', 'Control de peso recomendado');
GO

-- CONFIGURACIÓN DE IDENTITY
IF COL_LENGTH('dbo.AspNetRoles', 'Description') IS NULL
BEGIN
    ALTER TABLE dbo.AspNetRoles
    ADD [Description] NVARCHAR(256) NULL;
END
GO

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

-- INSERTS DE MÓDULOS
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

IF NOT EXISTS (SELECT 1 FROM dbo.Modules WHERE [Name] = 'Inventario')
    INSERT INTO dbo.Modules([Name],[Description]) VALUES ('Inventario','Gestión del inventario');
GO

-- INSERTS DE ROLES DE IDENTITY
IF NOT EXISTS (SELECT 1 FROM dbo.AspNetRoles WHERE [Name] = 'Administrador')
    INSERT INTO dbo.AspNetRoles(Id, [Name], Description) VALUES (NEWID(), 'Administrador', 'Acceso total al sistema');

IF NOT EXISTS (SELECT 1 FROM dbo.AspNetRoles WHERE [Name] = 'Colaborador')
    INSERT INTO dbo.AspNetRoles(Id, [Name], Description) VALUES (NEWID(), 'Colaborador', 'Acceso operativo (lectura/escritura según permisos)');

IF NOT EXISTS (SELECT 1 FROM dbo.AspNetRoles WHERE [Name] = 'Lector')
    INSERT INTO dbo.AspNetRoles(Id, [Name], Description) VALUES (NEWID(), 'Lector', 'Acceso solo lectura');
GO

-- ASIGNACIÓN DE PERMISOS
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

-- CONSULTA DE USUARIOS Y ROLES
SELECT TOP (1000) [UserId], [RoleId]
FROM [ARAC_DB].[dbo].[AspNetUserRoles];
GO

-- ASIGNACIÓN DE ROL A USUARIO
DECLARE @UserId NVARCHAR(128) = (SELECT TOP 1 Id FROM AspNetUsers WHERE Email = 'franffv080905@gmail.com');
DECLARE @RoleId NVARCHAR(128) = (SELECT TOP 1 Id FROM AspNetRoles WHERE Name = 'Administrador');

IF NOT EXISTS (SELECT 1 FROM AspNetUserRoles WHERE UserId = @UserId AND RoleId = @RoleId)
BEGIN
    INSERT INTO AspNetUserRoles (UserId, RoleId) VALUES (@UserId, @RoleId);
END
GO

-- RELACIÓN CON ASPNETUSERS
IF EXISTS (SELECT 1 FROM sys.tables WHERE name = 'AspNetUsers')
AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_MovInv_Usuario')
BEGIN
    ALTER TABLE dbo.Movimiento_Inventario WITH CHECK
    ADD CONSTRAINT FK_MovInv_Usuario FOREIGN KEY (UsuarioId) REFERENCES dbo.AspNetUsers(Id);
END
GO

-- ÍNDICE DE INVENTARIO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_MovInv_Fecha' AND object_id = OBJECT_ID('dbo.Movimiento_Inventario'))
BEGIN
    CREATE INDEX IX_MovInv_Fecha ON dbo.Movimiento_Inventario(Fecha_Movimiento DESC);
END
GO

-- CATEGORIA
CREATE TABLE CategoriaFinanciera (
    ID_Categoria INT IDENTITY PRIMARY KEY,
    Nombre VARCHAR(100),
    Tipo VARCHAR(50),
    Estado BIT,
    FechaRegistro DATETIME
);

-- GASTO
CREATE TABLE Gasto (
    ID_Gasto INT IDENTITY PRIMARY KEY,
    ID_Categoria INT,
    Monto DECIMAL(18,2),
    Descripcion VARCHAR(200),
    Fecha DATETIME,

    CONSTRAINT FK_Gasto_Categoria
    FOREIGN KEY (ID_Categoria)
    REFERENCES CategoriaFinanciera(ID_Categoria)
);

-- NOTICIA
CREATE TABLE Noticia (
    ID_Noticia INT IDENTITY PRIMARY KEY,
    ID_Usuario NVARCHAR(128),
    Titulo VARCHAR(150),
    Contenido VARCHAR(500),
    Fecha_Publicacion DATETIME DEFAULT GETDATE(),
    Estado BIT DEFAULT 1,

    CONSTRAINT FK_Noticia_Usuario 
    FOREIGN KEY (ID_Usuario)
    REFERENCES AspNetUsers(Id)
);

SELECT Id, Email FROM AspNetUsers

INSERT INTO Noticia (ID_Usuario, Titulo, Contenido)
VALUES 
('ee541788-4fed-459f-b856-19a538f78af3', 'Primera noticia', 'Bienvenido al sistema ARAC');

--Se altera la tabla de Usuario agregando un nuevo valor 
ALTER TABLE dbo.Usuario
ADD IdAspNetUser NVARCHAR(128);

--Se altera la tabla de Usuario conectando con la tabla que se crea con el Login 
ALTER TABLE dbo.Usuario
ADD CONSTRAINT FK_Usuario_AspNetUsers
FOREIGN KEY (IdAspNetUser) REFERENCES dbo.AspNetUsers(Id);

--Esto para comprobar que si se crea
SELECT 
    U.ID_Usuario,
    U.Correo,
    U.IdAspNetUser,
    A.Email
FROM dbo.Usuario U
LEFT JOIN dbo.AspNetUsers A 
    ON U.IdAspNetUser = A.Id;

--Esto para ver su id y su email y poder comparar
SELECT Id, Email
FROM dbo.AspNetUsers;

--Se agarra el id y se remplaza en el IdAspNetUser
UPDATE dbo.Usuario
SET IdAspNetUser = 'ee541788-4fed-459f-b856-19a538f78af3'
WHERE ID_Usuario = 1;


--Para comprobar
SELECT ID_Usuario, Correo, IdAspNetUser
FROM dbo.Usuario
WHERE ID_Usuario = 1;

--Para cambiar el id 1
UPDATE dbo.Usuario
SET Correo = 'franffv0809@gmail.com'
WHERE ID_Usuario = 1;
-----------------------------------------------
--Para meter cantidades en las donaciones
ALTER TABLE Detalle_Donacion
ADD Cantidad INT NOT NULL DEFAULT 1;




USE ARAC_DB;
GO

;WITH TiposBase AS
(
    SELECT
        ID_Tipo_Donacion,
        Nombre = LTRIM(RTRIM(Nombre)),
        KeepId = MIN(ID_Tipo_Donacion) OVER (PARTITION BY LTRIM(RTRIM(LOWER(Nombre))))
    FROM Tipo_Donacion
),
Duplicados AS
(
    SELECT ID_Tipo_Donacion, KeepId
    FROM TiposBase
    WHERE ID_Tipo_Donacion <> KeepId
)
UPDATE D
SET D.ID_Tipo_Donacion = X.KeepId
FROM Donacion D
INNER JOIN Duplicados X
    ON D.ID_Tipo_Donacion = X.ID_Tipo_Donacion;
GO

;WITH TiposBase AS
(
    SELECT
        ID_Tipo_Donacion,
        KeepId = MIN(ID_Tipo_Donacion) OVER (PARTITION BY LTRIM(RTRIM(LOWER(Nombre))))
    FROM Tipo_Donacion
)
DELETE T
FROM Tipo_Donacion T
INNER JOIN TiposBase X
    ON T.ID_Tipo_Donacion = X.ID_Tipo_Donacion
WHERE T.ID_Tipo_Donacion <> X.KeepId;
GO

IF NOT EXISTS (SELECT 1 FROM Tipo_Donacion WHERE LTRIM(RTRIM(LOWER(Nombre))) = 'monetaria')
INSERT INTO Tipo_Donacion (Nombre, Descripcion) VALUES ('Monetaria', 'Donación en efectivo');

IF NOT EXISTS (SELECT 1 FROM Tipo_Donacion WHERE LTRIM(RTRIM(LOWER(Nombre))) = 'insumos')
INSERT INTO Tipo_Donacion (Nombre, Descripcion) VALUES ('Insumos', 'Alimentos o artículos');

IF NOT EXISTS (SELECT 1 FROM Tipo_Donacion WHERE LTRIM(RTRIM(LOWER(Nombre))) = 'medicinas')
INSERT INTO Tipo_Donacion (Nombre, Descripcion) VALUES ('Medicinas', 'Donación de medicamentos veterinarios');

IF NOT EXISTS (SELECT 1 FROM Tipo_Donacion WHERE LTRIM(RTRIM(LOWER(Nombre))) = 'limpieza')
INSERT INTO Tipo_Donacion (Nombre, Descripcion) VALUES ('Limpieza', 'Productos de limpieza para refugio');

IF NOT EXISTS (SELECT 1 FROM Tipo_Donacion WHERE LTRIM(RTRIM(LOWER(Nombre))) = 'accesorios')
INSERT INTO Tipo_Donacion (Nombre, Descripcion) VALUES ('Accesorios', 'Correas, platos y camas');

IF NOT EXISTS (SELECT 1 FROM Tipo_Donacion WHERE LTRIM(RTRIM(LOWER(Nombre))) = 'higiene')
INSERT INTO Tipo_Donacion (Nombre, Descripcion) VALUES ('Higiene', 'Champú y artículos de aseo');

IF NOT EXISTS (SELECT 1 FROM Tipo_Donacion WHERE LTRIM(RTRIM(LOWER(Nombre))) = 'arena')
INSERT INTO Tipo_Donacion (Nombre, Descripcion) VALUES ('Arena', 'Arena sanitaria para gatos');

IF NOT EXISTS (SELECT 1 FROM Tipo_Donacion WHERE LTRIM(RTRIM(LOWER(Nombre))) = 'cobijas')
INSERT INTO Tipo_Donacion (Nombre, Descripcion) VALUES ('Cobijas', 'Cobertores y ropa de cama');

IF NOT EXISTS (SELECT 1 FROM Tipo_Donacion WHERE LTRIM(RTRIM(LOWER(Nombre))) = 'juguetes')
INSERT INTO Tipo_Donacion (Nombre, Descripcion) VALUES ('Juguetes', 'Juguetes para perros y gatos');

IF NOT EXISTS (SELECT 1 FROM Tipo_Donacion WHERE LTRIM(RTRIM(LOWER(Nombre))) = 'veterinaria')
INSERT INTO Tipo_Donacion (Nombre, Descripcion) VALUES ('Veterinaria', 'Aporte para procedimientos médicos');

IF NOT EXISTS (SELECT 1 FROM Tipo_Donacion WHERE LTRIM(RTRIM(LOWER(Nombre))) = 'transporte')
INSERT INTO Tipo_Donacion (Nombre, Descripcion) VALUES ('Transporte', 'Apoyo para traslados');

IF NOT EXISTS (SELECT 1 FROM Tipo_Donacion WHERE LTRIM(RTRIM(LOWER(Nombre))) = 'general')
INSERT INTO Tipo_Donacion (Nombre, Descripcion) VALUES ('General', 'Donación general de apoyo');
GO

SELECT ID_Tipo_Donacion, Nombre, Descripcion
FROM Tipo_Donacion
ORDER BY Nombre;
GO



USE ARAC_DB;
GO

IF OBJECT_ID('dbo.Inscripcion_Castracion', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Inscripcion_Castracion
    (
        Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        CampaniaCastracionId INT NOT NULL,
        AnimalId INT NOT NULL,
        VeterinarioAsignado VARCHAR(150) NULL,
        Resultado VARCHAR(500) NULL
    );
END
GO


IF COL_LENGTH('dbo.InscripcionesCastracion', 'IdUsuario') IS NULL
BEGIN
    ALTER TABLE dbo.InscripcionesCastracion
    ADD IdUsuario INT NULL;
END
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.foreign_keys
    WHERE name = 'FK_InscripcionesCastracion_Usuario'
)
BEGIN
    ALTER TABLE dbo.InscripcionesCastracion
    ADD CONSTRAINT FK_InscripcionesCastracion_Usuario
    FOREIGN KEY (IdUsuario) REFERENCES dbo.Usuario(ID_Usuario);
END
GO
---------------------------Bryan-------------------------------------
ALTER TABLE Noticia
ADD Likes INT DEFAULT 0;

UPDATE Noticia
SET Likes = 0
WHERE Likes IS NULL;

ALTER TABLE Noticia
ALTER COLUMN Likes INT NOT NULL;

--modificacion de roles
-- 1) Limpiar relaciones que dependen de AspNetRoles
DELETE FROM dbo.AspNetUserRoles;
GO

DELETE FROM dbo.RoleModulePermissions;
GO

-- 2) Limpiar roles actuales
DELETE FROM dbo.AspNetRoles;
GO

-- 3) Insertar roles correctos con Id explícito
INSERT INTO dbo.AspNetRoles (Id, [Name], [Description])
VALUES
('1', 'Administrador', 'Acceso total al sistema'),
('2', 'Colaborador', 'Acceso operativo (lectura y escritura según permisos)'),
('3', 'Usuario', 'Usuario normal del sistema');
GO

--Favoritos 
IF OBJECT_ID('dbo.Favorito', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Favorito
    (
        ID_Favorito INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        UserId NVARCHAR(128) NOT NULL,
        ID_Animal INT NOT NULL,
        Fecha_Registro DATETIME NOT NULL CONSTRAINT DF_Favorito_Fecha DEFAULT(GETDATE()),
        CONSTRAINT FK_Favorito_AspNetUsers FOREIGN KEY (UserId) REFERENCES dbo.AspNetUsers(Id) ON DELETE CASCADE,
        CONSTRAINT FK_Favorito_Animal FOREIGN KEY (ID_Animal) REFERENCES dbo.Animal(ID_Animal) ON DELETE CASCADE,
        CONSTRAINT UQ_Favorito_User_Animal UNIQUE (UserId, ID_Animal)
    );
END
GO

-- registro de msacotas por ususario 
IF COL_LENGTH('dbo.Animal', 'UsuarioRegistroId') IS NULL
BEGIN
    ALTER TABLE dbo.Animal
    ADD UsuarioRegistroId NVARCHAR(128) NULL;
END
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.foreign_keys
    WHERE name = 'FK_Animal_AspNetUsers_UsuarioRegistro'
)
BEGIN
    ALTER TABLE dbo.Animal
    ADD CONSTRAINT FK_Animal_AspNetUsers_UsuarioRegistro
    FOREIGN KEY (UsuarioRegistroId) REFERENCES dbo.AspNetUsers(Id);
END
GO
