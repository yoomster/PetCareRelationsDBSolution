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
        private SqlDataAccess db = new();


        public SqlCrud(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<PersonModel> GetAllContacts()
        {
            string sql = "select Id, FirstName, LastName from Contacts";

            return db.LoadData<PersonModel, dynamic>(sql, new { }, _connectionString);
        }

        public FullContactModel GetFullContactById(int id)
        {
            string sql = "select Id, FirstName, LastName from Contacts where Id = @Id";
            FullContactModel output = new()
            {
                BasicInfo = db.LoadData<PersonModel, dynamic>(sql, new { Id = id }, _connectionString).FirstOrDefault()
            };

            if (output.BasicInfo == null)
            {
                return null;
            }

            sql = @"select a.*, ca.*
                    from Addresses a
                    inner join ContactAddress ca on ca.AddressId = a.Id
                    where ca.ContactId = @Id";

            output.Addresses = db.LoadData<AddressModel, dynamic>(sql, new { Id = id }, _connectionString);

            sql = @"select e.*
                    from Employers e
                    inner join Contactemployer ce on ce.EmployerId = e.Id
                    where ce.ContactId = @Id";

            output.Employers = db.LoadData<EmployerModel, dynamic>(sql, new { Id = id }, _connectionString);

            return output;
        }

        public void CreateContact(FullContactModel contact)
        {
            string sql = "insert into contacts (FirstName, LastName) values (@FirstName, @LastName);";
            db.SaveData(sql,
                        new { contact.BasicInfo.FirstName, contact.BasicInfo.LastName },
                        _connectionString);

            sql = "select Id from contacts where FirstName= @FirstName and LastName= @LastName;";
            int contactId = db.LoadData<IdLookUpModel, dynamic>(sql,
                new { contact.BasicInfo.FirstName, contact.BasicInfo.LastName },
                _connectionString).First().Id;

            foreach (var address in contact.Addresses)
            {
                if (address.Id == 0)
                {
                    sql = "insert into addresses(AddressName, HouseNumber, Zipcode, City, Country) values (@AddressName, @HouseNumber, @Zipcode, @City, @Country); ";
                    db.SaveData(sql, new { address.AddressName, address.HouseNumber, address.Zipcode, address.City, address.Country }, _connectionString);

                    sql = "select Id from addresses where AddressName = @AddressName and HouseNumber= @HouseNumber and Zipcode= @Zipcode and City= @City and Country= @Country;";
                    address.Id = db.LoadData<IdLookUpModel, dynamic>
                        (sql, new { address.AddressName, address.HouseNumber, address.Zipcode, address.City, address.Country },
                        _connectionString).First().Id;
                }

                sql = "insert into contactaddress(ContactID, AddressId) values(@ContactId, @AddressId); ";
                db.SaveData(sql, new { ContactId = contactId, AddressId = address.Id }, _connectionString);
            }

            foreach (var employer in contact.Employers)
            {
                if (employer.Id == 0)
                {
                    sql = "insert into employers(CompanyName, JobTitle) values(@CompanyName, @JobTitle); ";
                    db.SaveData(sql, new { employer.CompanyName, employer.JobTitle }, _connectionString);

                    sql = "select Id from employers where CompanyName = @CompanyName and JobTitle= @JobTitle;";
                    employer.Id = db.LoadData<IdLookUpModel, dynamic>
                        (sql, new { employer.CompanyName, employer.JobTitle },
                        _connectionString).First().Id;
                }

                sql = "insert into contactemployer(ContactId, EmployerId) values(@ContactID, @EmployerId); ";
                db.SaveData(sql, new { ContactID = contactId, EmployerId = employer.Id }, _connectionString);
            }
        }

        public void UpdateContactName(PersonModel contact)
        {
            string sql = "update Contacts set FirstName = @FirstName, LastName = @LastName where Id = @Id;";
            db.SaveData(sql, contact, _connectionString);
        }

        public void RemoveEmployerFromContact(int contactId, int employerId)
        {
            string sql = "select Id, ContactId, EmployerId from contactemployer where EmployerId = @EmployerId;";
            var links = db.LoadData<EmployerModel, dynamic>
                (sql, new { EmployerId = employerId },
                _connectionString);

            sql = "delete from contactemployer where EmployerId = @EmployerId and ContactId= @ContactId;";
            db.SaveData(sql, new { EmployerId = employerId, ContactId = contactId },
                _connectionString);

            if (links.Count == 1)
            {
                sql = "delete from Employers where Id = @EmployerId ;";
                db.SaveData(sql, new { EmployerId = employerId },
                _connectionString);
            }
        }
    }
}
