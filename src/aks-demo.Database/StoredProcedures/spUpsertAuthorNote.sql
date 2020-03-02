CREATE PROCEDURE spUpsertAuthorNote
(
    -- Add the parameters for the stored procedure here
    @noteId int = null,
    @authorId int = null,
	@lastName nvarchar(500) = null,
	@firstName nvarchar(500) = null,
	@emailAddress nvarchar(500) = null,
	@noteContent nvarchar(max)
)
AS
BEGIN
     SET NOCOUNT ON
	 DECLARE @personId int
	 MERGE dbo.Person as p
	  USING (SELECT @authorId, @lastName, @firstName, @emailAddress) as source (authorId, lastName, firstName, emailAddress)
	  ON (p.personId = source.authorId)
	  WHEN MATCHED THEN
		UPDATE SET lastName = COALESCE(source.lastName, p.lastName),
					firstName = COALESCE(source.firstName,p.firstName),
					emailAddress = COALESCE(source.emailAddress,p.emailAddress)
	  WHEN NOT MATCHED THEN 
		INSERT (lastName, firstName, emailAddress)
		VALUES (source.lastName, source.firstName, source.emailAddress);

	  SELECT @authorId = personId
	  FROM dbo.Person
	  WHERE (personId = @authorId) OR (lastName = @lastName and firstName = @firstName and emailAddress = @emailAddress)

	  MERGE dbo.AuthorNote an
	  USING (SELECT @noteId, @authorId, @noteContent) as source (noteId, authorId, noteContent)
	  ON (@noteId = an.noteId)
	  WHEN MATCHED THEN
		UPDATE SET modifyDate = getutcdate(), 
					noteContent = source.noteContent
	  WHEN NOT MATCHED THEN
		INSERT (personId, createDate, modifyDate, noteContent)
		VALUES (source.authorId, getutcdate(), getutcdate(), noteContent);
END;

GO