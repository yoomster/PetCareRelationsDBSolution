
using Microsoft.Extensions.Configuration;
using DataAccessLibrary;
using DataAccessLibrary.Models;
using System.Data;

internal class Program
{
    private static void Main(string[] args)
    {
        SqlCrud sql = new SqlCrud(GetConnectionString());

        //ReadAllContacts(sql);

        //ReadContact(sql, 3);

        //CreateNewContact(sql);

        //UpdateContact(sql);

        //RemovePhoneNumberFromContact(sql, 3, 1004);

        Console.WriteLine("Done processing Sql Server");


        Console.ReadLine();


    }

    private static void RemovePhoneNumberFromContact(SqlCrud sql, int contactId, int phoneNumberId)
    {
        sql.RemovePhoneNumberFromContact(contactId, phoneNumberId);
    }

    private static void UpdateContact(SqlCrud sql)
    {
        PersonModel contact = new PersonModel
        {
            Id = 1,
            FirstName = "Baby",
            LastName = "Akil"
        };

        sql.UpdateContactName(contact);

    }

    private static void CreateNewContact(SqlCrud sql)
    {
        FullContactModel user = new FullContactModel
        {
            BasicInfo = new PersonModel
            {
                FirstName = "Adam",
                LastName = "Ak"
            },
        };

        user.Addresses.Add(new AddressModel { AddressName = "Street", HouseNumber = "1", Zipcode = "1234AB", City = "Ehv", Country = "NL" });

        user.Employers.Add(new EmployerModel { CompanyName = "Bank", JobTitle = "banking" });


        sql.CreateContact(user);
    }

    private static void ReadAllContacts(SqlCrud sql)
    {
        var rows = sql.GetAllContacts();

        foreach (var row in rows)
        {
            Console.WriteLine($"{row.Id}: {row.FirstName} {row.LastName}");
        }
    }

    private static void ReadContact(SqlCrud sql, int contactId)
    {
        var contact = sql.GetFullContactById(contactId);

        Console.WriteLine($"{contact.BasicInfo.Id}: {contact.BasicInfo.FirstName} {contact.BasicInfo.LastName}");
    }

    private static string GetConnectionString(string connectionStringName = "Default")
    {
        string output = "";

        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json");

        var config = builder.Build();

        output = config.GetConnectionString(connectionStringName);

        return output;
    }
}