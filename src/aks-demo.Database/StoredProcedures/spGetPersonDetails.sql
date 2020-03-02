CREATE PROCEDURE spGetPersonDetails
(
	@personId int = null
)
AS
BEGIN
    SET NOCOUNT ON

    select p.lastName, p.firstName, p.emailAddress
	from dbo.Person p 
	where (@personId is null or (p.personId = @personId))
END