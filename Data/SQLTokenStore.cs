using ClickAndCollect.Logs;
using ClickAndCollect.Models;
using Newtonsoft.Json;
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
        private readonly ILogger logger;

        public SQLTokenStore(string connectionString, ILogger logger)
        {
            this.connectionString = connectionString;
            this.logger = logger;
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
                if (string.IsNullOrWhiteSpace(jwtAuthToken)) return false;

                using (var connection = OpenConnection())
                {
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "DELETE FROM TOKEN WHERE JWTTOKEN= @token";
                        var parameter = command.CreateParameter();
                        parameter.ParameterName = "token";
                        parameter.Value = jwtAuthToken;
                        command.Parameters.Add(parameter);
                        command.ExecuteNonQuery();

                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error($"Error on delete token '{jwtAuthToken}'", ex);
                throw ex;
            }
        }

        private const string SQL_TOKEN_SELECT = "SELECT JWTTOKEN, EXTERNALJWTTOKEN, DATA, UPDATEDONUTC FROM TOKEN";

        public AuthTokenData GetAuthToken(string jwtAuthToken)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(jwtAuthToken)) return null;

                using (var connection = OpenConnection())
                {
                    return GetAuthToken(connection, jwtAuthToken);
                    
                }
            }
            catch (Exception ex)
            {
                logger.Error($"Error on read token '{jwtAuthToken}'", ex);
                throw ex;
            }
        }

        private AuthTokenData GetAuthToken(SqlConnection connection, string jwtAuthToken)
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = $"{SQL_TOKEN_SELECT} WHERE JWTTOKEN= @token";
                var parameter = command.CreateParameter();
                parameter.ParameterName = "token";
                parameter.Value = jwtAuthToken;
                command.Parameters.Add(parameter);

                using (var dr = command.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        return ReadAuthToken(dr);
                    }
                }
            }
            return null;
        }

        private AuthTokenData ReadAuthToken(SqlDataReader dr)
        {
            var tokenData = new AuthTokenData();
            tokenData.JwtToken = dr.GetString(0)?.Trim();
            tokenData.ExternalToken = dr.GetString(1)?.Trim();
            tokenData.Data = dr.GetString(2)?.Trim();
            tokenData.UpdatedOnUTC = dr.GetDateTime(3);
            return tokenData;
        }

        public IEnumerable<AuthTokenData> GetAuthTokens()
        {
            try
            {
                var tokens = new List<AuthTokenData>();

                using (var connection = OpenConnection())
                {
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = SQL_TOKEN_SELECT;

                        using (var dr = command.ExecuteReader())
                        {
                            if (dr.Read())
                            {
                                tokens.Add(ReadAuthToken(dr));
                            }
                        }
                    }
                }

                return tokens;
            }
            catch (Exception ex)
            {
                logger.Error($"Error on read all tokens", ex);
                throw ex;
            }
        }

        public bool SaveAuthToken(AuthTokenData tokenData)
        {
            try
            {
                if (tokenData == null) throw new ArgumentNullException(nameof(tokenData));
                if (string.IsNullOrWhiteSpace(tokenData.JwtToken)) throw new ApplicationException("JWT Token cannot be null or empty");
                if (string.IsNullOrWhiteSpace(tokenData.ExternalToken)) throw new ApplicationException("External JWT Token cannot be null or empty");

                using (var connection = OpenConnection())
                {
                    var existingTokenData = GetAuthToken(connection, tokenData.JwtToken);
                    
                    using (var command = connection.CreateCommand())
                    {
                        if (existingTokenData == null)
                            command.CommandText = "INSERT INTO TOKEN (JWTTOKEN, EXTERNALJWTTOKEN, DATA, UPDATEDONUTC) VALUES ( @token, @extoken, @data, GETUTCDATE())";
                        else
                            command.CommandText = "UPDATE TOKEN SET EXTERNALJWTTOKEN=@extoken, DATA = @data, UPDATEDONUTC =  GETUTCDATE() WHERE JWTTOKEN= @token";
                        
                        var parameter = command.CreateParameter();
                        parameter.ParameterName = "token";
                        parameter.Value = tokenData.JwtToken;
                        command.Parameters.Add(parameter);
                        
                        parameter = command.CreateParameter();
                        parameter.ParameterName = "extoken";
                        parameter.Value = tokenData.ExternalToken;
                        command.Parameters.Add(parameter);

                        parameter = command.CreateParameter();
                        parameter.ParameterName = "data";
                        parameter.Value = tokenData.Data;
                        command.Parameters.Add(parameter);

                        command.ExecuteNonQuery();

                        tokenData.UpdatedOnUTC = DateTime.UtcNow;

                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error($"Error on save token '{tokenData.JwtToken}'", ex);
                return false;
            }
        }
    }
}