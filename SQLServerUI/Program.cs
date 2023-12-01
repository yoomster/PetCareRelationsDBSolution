﻿
using Microsoft.Extensions.Configuration;
using DataAccessLibrary;
using DataAccessLibrary.Models;
using System.Data;

internal class Program
{
    private static void Main(string[] args)
    {
        SqlCrud sql = new SqlCrud(GetConnectionString());

        ReadAllContacts(sql);

        //CreateNewContact(sql);

        //UpdateContact(sql);
        //ReadContact(sql, 3);

        //RemoveEmployerFromContact(sql, 3, 2);


        Console.WriteLine("Done processing Sql Server");
        Console.ReadLine();


    }

    private static void RemoveEmployerFromContact(SqlCrud sql, int contactId, int employerId)
    {
        sql.RemoveEmployerFromContact(contactId, employerId);
    }

    private static void UpdateContact(SqlCrud sql)
    {
        PersonModel contact = new PersonModel
        {
            Id = 3,
            FirstName = "Teddy-Saurus",
            LastName = "Rex"
        };

        sql.UpdateContactName(contact);

    }

    private static void CreateNewContact(SqlCrud sql)
    {
        FullContactModel user = new FullContactModel
        {
            BasicInfo = new PersonModel
            {
                FirstName = "Teddy",
                LastName = "Akil"
            },
        };

        user.Addresses.Add(new AddressModel { AddressName = "Frits Philips", HouseNumber = "104", Zipcode = "5616TZ", City = "Ehv", Country = "NL" });

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