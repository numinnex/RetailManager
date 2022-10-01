CREATE PROCEDURE [dbo].[spUser_Insert]
	@Id nvarchar(128),
	@FirstName nvarchar(50),
	@LastName nvarchar(50),
	@EmailAdress nvarchar(256)
AS
begin
set nocount on;
	insert into dbo.[User] ( Id, FirstName, LastName , EmailAdress)
	values(@Id,@FirstName,@LastName,@EmailAdress);

end

