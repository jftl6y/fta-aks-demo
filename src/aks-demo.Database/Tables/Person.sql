CREATE TABLE [dbo].[Person](
	[personId] [int] IDENTITY(1,1) NOT NULL,
	[lastName] [nvarchar](500) NOT NULL,
	[firstName] [nvarchar](500) NULL,
	[emailAddress] [nvarchar](500) NOT NULL,
 CONSTRAINT [PK_Person] PRIMARY KEY CLUSTERED 
(
	[personId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
