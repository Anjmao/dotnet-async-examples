using System;
using System.Data;

using Dapper;

namespace ConsoleSamples
{
    public class DbInitializer
    {
        private Func<IDbConnection> GetDapperConnection => () => new MySql.Data.MySqlClient.MySqlConnection("Server=127.0.0.1;Database=demo;Uid=root;Pwd=admin;SslMode=None;");

        public void CreateData()
        {
            using (var connection = GetDapperConnection())
            {
                var usersTableSql = @"
                    CREATE TABLE IF NOT EXISTS `demo`.`user` (
                        `Id` varchar(32) NOT NULL,
                        `FirstName` varchar(32) NOT NULL,
                        `LastName` varchar(32) NULL,
                        PRIMARY KEY (`Id`)
                    );";
                connection.Execute(usersTableSql);

                for (var i = 0; i < 100; i++)
                {
                    connection.Execute("INSERT user(id, firstname, lastname) VALUES(@Id, @FirstName, @LastName)", new
                    {
                        Id = Guid.NewGuid().ToString("N"),
                        FirstName = "Name" + i,
                        LastName = "LastName" + i
                    });
                }
            }
        }
    }
}