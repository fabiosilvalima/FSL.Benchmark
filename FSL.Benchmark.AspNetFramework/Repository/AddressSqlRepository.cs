using Dapper;
using FSL.Benchmark.AspNetFramework.Models;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace FSL.Benchmark.AspNetFramework.Repository
{
    public sealed class AddressSqlRepository
    {
        public async Task<Address> GetAddressAsync(
            string zipCode)
        {
            using (var connection = CreateConnection())
            {
                await connection.OpenAsync();

                var parameters = new
                {
                    zipCode
                };

                var sql = @"SELECT              a.cod_postal AS ZipCode,
                                                b.des_cidade AS City,
                                                c.des_sigla AS State,
                                                (t.des_tipo_logradouro + ' ' + d.des_logradouro) AS Street, 
                                                r.des_bairro AS Neighborhood
                            FROM                dbo.tb_cep AS a
                            LEFT OUTER JOIN     dbo.tb_cidade AS b ON a.cod_cidade = b.cod_cidade
                            LEFT OUTER JOIN     dbo.tb_estado AS c ON a.cod_estado = c.cod_estado
                            LEFT OUTER JOIN     dbo.tb_logradouro AS d ON a.cod_logradouro = d.cod_logradouro
                            LEFT OUTER JOIN     dbo.tb_tipo_logradouro AS t ON a.cod_tipo_logradouro = t.cod_tipo_logradouro
                            LEFT OUTER JOIN     dbo.tb_bairro AS r ON a.cod_bairro = r.cod_bairro
                            WHERE               a.cod_postal = @zipCode";

                var data = await connection.QueryFirstOrDefaultAsync<Address>(
                    sql,
                    parameters);

                connection.Close();

                return data;
            };
        }
        
        public async Task<IEnumerable<Address>> GetAddressRangeAsync(
            string start,
            string end)
        {
            using (var connection = CreateConnection())
            {
                await connection.OpenAsync();

                var parameters = new
                {
                    start,
                    end
                };

                var sql = @"SELECT              a.cod_postal AS ZipCode,
                                                b.des_cidade AS City,
                                                c.des_sigla AS State,
                                                (t.des_tipo_logradouro + ' ' + d.des_logradouro) AS Street, 
                                                r.des_bairro AS Neighborhood
                            FROM                dbo.tb_cep AS a
                            LEFT OUTER JOIN     dbo.tb_cidade AS b ON a.cod_cidade = b.cod_cidade
                            LEFT OUTER JOIN     dbo.tb_estado AS c ON a.cod_estado = c.cod_estado
                            LEFT OUTER JOIN     dbo.tb_logradouro AS d ON a.cod_logradouro = d.cod_logradouro
                            LEFT OUTER JOIN     dbo.tb_tipo_logradouro AS t ON a.cod_tipo_logradouro = t.cod_tipo_logradouro
                            LEFT OUTER JOIN     dbo.tb_bairro AS r ON a.cod_bairro = r.cod_bairro
                            WHERE               a.cod_postal BETWEEN @start AND @end";

                var data = await connection.QueryAsync<Address>(
                    sql,
                    parameters);

                connection.Close();

                return data;
            };
        }

        private SqlConnection CreateConnection()
        {
            return new SqlConnection(ConfigurationManager.ConnectionStrings["Default"].ConnectionString);
        }
    }
}
