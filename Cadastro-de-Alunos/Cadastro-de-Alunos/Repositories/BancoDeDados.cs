using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cadastro_de_Alunos.Models;
using Cadastro_de_Alunos.Services;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Cadastro_de_Alunos.Repositories
{
    public class CountryDAL
    {
        private string _connectionString;
        public CountryDAL(IConfiguration iconfiguration)
        {
            _connectionString = iconfiguration.GetConnectionString("Default");
        }
        public List<CountryModel> GetList()
        {
            var listCountryModel = new List<CountryModel>();
            try
            {
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    SqlCommand cmd = new SqlCommand("SP_COUNTRY_GET_LIST", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    con.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        listCountryModel.Add(new CountryModel
                        {
                            Id = Convert.ToInt32(rdr[0]),
                            Country = rdr[1].ToString(),
                            Active = Convert.ToBoolean(rdr[2])
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return listCountryModel;
        }
    }
}  
}