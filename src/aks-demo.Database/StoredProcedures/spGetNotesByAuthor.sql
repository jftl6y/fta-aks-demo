CREATE PROCEDURE spGetNotesByAuthor
(
	@authorId int = null
)
AS
BEGIN
    SET NOCOUNT ON

    select an.noteId, an.personId, an.CreateDate, an.modifyDate, an.NoteContent, p.lastName, p.firstName, p.emailAddress
	from dbo.AuthorNote an
	join dbo.Person p on p.personId = an.personId
	where (@authorId is null or (p.personId = @authorId))
END