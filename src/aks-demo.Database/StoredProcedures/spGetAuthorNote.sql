﻿CREATE PROCEDURE spGetAuthorNote
(
    @noteId int = null
)
AS
BEGIN
    SET NOCOUNT ON

    select an.noteId, an.personId, an.createDate, an.modifyDate, an.noteContent, p.lastName, p.firstName, p.emailAddress
	from dbo.AuthorNote an
	join dbo.Person p on p.personId = an.personId
	where (@noteId is null or (@noteId = an.noteId))
END
GO