CREATE TABLE [dbo].[AuthorNote](
	[noteId] [int] IDENTITY(1,1) NOT NULL,
	[personId] [int] NOT NULL,
	[createDate] [datetime] NOT NULL DEFAULT getutcDate(),
	[modifyDate] [datetime] NOT NULL DEFAULT getutcDate(),
	[noteContent] [varchar](max) NULL,
 CONSTRAINT [PK_AuthorNote] PRIMARY KEY CLUSTERED 
(
	[noteId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY], 
    CONSTRAINT [FK_AuthorNote_To_Person] FOREIGN KEY ([personId]) REFERENCES [Person]([personId])
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
