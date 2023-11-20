using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLibrary
{
    public class SqliteCrud
    {
        private readonly string _connectionString;

        public SqliteCrud(string connectionString)
        {
            _connectionString = connectionString;
        }


    }
}
