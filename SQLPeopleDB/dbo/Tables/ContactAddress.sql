CREATE TABLE [dbo].[ContactAddress]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [ContactId] INT NOT NULL, 
    [AddressId] INT NOT NULL
)
