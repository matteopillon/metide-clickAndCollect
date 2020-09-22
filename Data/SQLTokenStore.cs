using ClickAndCollect.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace ClickAndCollect.Data
{
    public class SQLTokenStore : ITokenStore
    {
        private readonly string connectionString;

        public SQLTokenStore(string connectionString)
        {
            this.connectionString = connectionString;
        }

        private SqlConnection OpenConnection()
        {
            var connection = new SqlConnection(connectionString);
            connection.Open();
            return connection;
        }

        public bool DeleteAuthToken(string jwtAuthToken)
        {
            try
            {
                using (var connection = OpenConnection())
                {
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "DELETE FROM TOKEN WHERE JWTTOKEN= @token";
                        var parameter = command.CreateParameter();
                        parameter.ParameterName = "token";
                        parameter.Value = jwtAuthToken;

                        command.ExecuteNonQuery();

                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public AuthTokenData GetAuthToken(string jwtAuthToken)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<AuthTokenData> GetAuthTokens()
        {
            throw new NotImplementedException();
        }

        public bool SaveAuthToken(AuthTokenData jwtAuthTokenData)
        {
            throw new NotImplementedException();
        }
    }
}