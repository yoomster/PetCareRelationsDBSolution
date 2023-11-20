using DataAccessLibrary;
using DataAccessLibrary.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;



namespace SqliteUI
{
    class Program
    {
        static void Main(string[] args)
        {
            //SqliteCrud sql = new SqliteCrud(GetConnectionString());

            //ReadAllContacts(sql);

            //ReadContact(sql, 13);

            //CreateNewContact(sql);
            //ReadAllContacts(sql);

            //UpdateContact(sql);



            //RemovePhoneNumberFromContact(sql, 1, 1);


            Console.WriteLine("Done processing Sqlite");

            Console.ReadLine();

        }
        private static void RemovePhoneNumberFromContact(SqliteCrud sql, int contactId, int phoneNumberId)
        {
            sql.RemovePhoneNumberFromContact(contactId, phoneNumberId);
        }

        private static void UpdateContact(SqliteCrud sql)
        {
            PersonModel contact = new PersonModel
            {
                Id = 1,
                FirstName = "Nayoomi",
                LastName = "Peeer"
            };

            sql.UpdateContactName(contact);
        }

        private static void CreateNewContact(SqliteCrud sql)
        {
            FullContactModel user = new FullContactModel
            {
                BasicInfo = new PersonModel
                {
                    FirstName = "Baby",
                    LastName = "Akil"
                }
            };

            user.Addresses.Add(new AddressModel { AddressName ="Street", HouseNumber ="1", Zipcode="1234AB", City="Ehv", Country ="NL" });


            user.Employers.Add(new EmployerModel { CompanyName = "Bank", JobTitle = "banking" });

            sql.CreateContact(user);
        }

        private static void ReadAllContacts(SqliteCrud sql)
        {
            var rows = sql.GetAllContacts();

            foreach (var row in rows)
            {
                Console.WriteLine($"{row.Id}: {row.FirstName} {row.LastName}");
            }
        }

        private static void ReadContact(SqliteCrud sql, int contactId)
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
}