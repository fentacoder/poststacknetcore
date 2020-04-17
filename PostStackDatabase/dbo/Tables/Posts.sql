CREATE TABLE [dbo].[Posts]
(
	[UserId] INT NOT NULL, 
    [Title] NVARCHAR(50) NOT NULL, 
    [Body] NVARCHAR(400) NOT NULL, 
    [CreatedAt] DATETIME2 NOT NULL , 
    [UpdatedAt] DATETIME2 NOT NULL
)
