USE ABC_DB;

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