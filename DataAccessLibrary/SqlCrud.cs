using DataAccessLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLibrary
{
    public class SqlCrud
    {
        private readonly string _connectionString;
        private SqlDataAccess db = new SqlDataAccess();

        public SqlCrud(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<PersonModel> GetAllContacts()
        {
            string sql = "select Id, FirstName, LastName from dbo.Person";

            return db.LoadData <PersonModel, dynamic > (sql, new { }, _connectionString);
        }

        public FullContactModel GetFullContactById(int id)
        {
            string sql = "select Id, FirstName, LastName from dbo.Person where Id = @Id";
            FullContactModel output = new FullContactModel();

            output.BasicInfo = db.LoadData<PersonModel, dynamic>(sql, new { Id = id }, _connectionString).FirstOrDefault();

            if (output.BasicInfo == null)
            {
                //throw new Exception("User not found.");
                return null;
            }

            sql = @"select a.*, ce.*
                    from dbo.Addresses a
                    inner join dbo.Employers e on e.AddressesId = a.Id
     !!               where ce.ContactID = @Id";

            output.Addresses = db.LoadData<AddressModel, dynamic>(sql, new { Id = id }, _connectionString);

            sql = @"select p.*
                    from dbo.Employers p
                    inner join dbo.ContactPhone cp on cp.PhoneNumberId = p.Id
     !!               where cp.ContactID = @Id";

            output.Employers = db.LoadData<EmployerModel, dynamic>(sql, new { Id = id }, _connectionString);

            return output;
        }

        public void CreateContact(FullContactModel contact)
        {
            string sql = "insert into dbo.Person (FirstName, LastName) values (@FirstName, @LastName);";
            db.SaveData(sql, 
                        new {contact.BasicInfo.FirstName, contact.BasicInfo.LastName},
                        _connectionString);

            sql = "select Id from dbo.Person where FirstName= @FirstName and LastName= @LastName;";
     !!       int contactId = db.LoadData<IdLookUpModel, dynamic>(sql,
                new { contact.BasicInfo.FirstName, contact.BasicInfo.LastName },
                _connectionString).First().Id;

            foreach(var employee in contact.Employers)
            {
                if (employee.Id== 0)
                {
                    sql = "insert into dbo.Employers(CompanyName) values(@CompanyName); ";
                    db.SaveData(sql, new {employee.CompanyName }, _connectionString);

     !!               sql = "select Id from dbo.Employers where CompanyName = @CompanyName;";
                    employee.Id = db.LoadData<IdLookUpModel, dynamic>
                        (sql, new { employee.CompanyName },
                        _connectionString).First().Id;
                }

                sql = "insert into dbo.ContactPhone(ContactID, PhoneNumberId) values(@ContactID, @PhoneNumberId); ";
     !!           db.SaveData(sql, new {ContactId = contactId, PhoneNumberId = employee.Id }, _connectionString);
            }


            foreach (var email in contact.Addresses)
            {
                if (email.Id == 0)
                {
                    sql = "insert into dbo.Addresses(EmailAddress) values(@EmailAddress); ";
                    db.SaveData(sql, new { email.EmailAddress }, _connectionString);

                    sql = "select Id from dbo.Addresses where EmailAddress = @EmailAddress;";
                    email.Id = db.LoadData<IdLookUpModel, dynamic>
                        (sql, new { email.EmailAddress },
                        _connectionString).First().Id;
                }

                sql = "insert into dbo.ContactEmail(ContactID, EmailAddressId) values(@ContactID, @EmailAddressId); ";
                db.SaveData(sql, new { ContactId = contactId, EmailAddressId = email.Id }, _connectionString);
            }

        }

        public void UpdateContactName(PersonModel contact)
        {
            string sql = "update dbo.Person set FirstName = @FirstName, LastName = @LastName where Id = @Id;";
            db.SaveData(sql, contact, _connectionString);
        }

        public void RemovePhoneNumberFromContact(int contactId, int phoneNumberId)
        {
            string sql = "select Id, ContactId,PhoneNumberId from dbo.ContactPhone where PhoneNumberId = @PhoneNumberId;";
 !!           var links = db.LoadData<ContactEmployerModel, dynamic>
                (sql, new { PhoneNumberId = phoneNumberId },
                _connectionString);

            sql = "delete from dbo.ContactPhone where PhoneNumberId = @PhoneNumberId and ContactId= @ContactId;";
            db.SaveData(sql, new { PhoneNumberId = phoneNumberId, ContactId = contactId },
                _connectionString );

 !!           if (links.Count == 1)
            {
                sql = "delete from dbo.Employers where Id = @PhoneNumberId ;";
                db.SaveData(sql, new { PhoneNumberId = phoneNumberId },
                _connectionString);
            }
        }

    }
}
