DECLARE @UserId NVARCHAR(128) = (SELECT TOP 1 Id FROM AspNetUsers WHERE Email = 'TU_CORREO@DOMINIO.COM');
DECLARE @RoleId NVARCHAR(128) = (SELECT TOP 1 Id FROM AspNetRoles WHERE Name = 'Administrador');

IF NOT EXISTS (SELECT 1 FROM AspNetUserRoles WHERE UserId = @UserId AND RoleId = @RoleId)
BEGIN
  INSERT INTO AspNetUserRoles (UserId, RoleId) VALUES (@UserId, @RoleId);
END