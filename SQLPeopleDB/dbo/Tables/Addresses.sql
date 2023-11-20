CREATE TABLE [dbo].[Addresses]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [AddressName] VARCHAR(50) NOT NULL, 
    [HouseNumber] VARCHAR(15) NOT NULL, 
    [Zipcode] VARCHAR(7) NOT NULL, 
    [City] VARCHAR(50) NOT NULL, 
    [Country] VARCHAR(50) NOT NULL
)
