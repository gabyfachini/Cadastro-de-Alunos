using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cadastro_de_Alunos.Models;
using System.ComponentModel.DataAnnotations;
using Cadastro_de_Alunos.Services;

//Representa a camada de acesso a dados
namespace Cadastro_de_Alunos.DAL
{
    public class AlunoDAL
    {
        private string _connectionString;
        public AlunoDAL(IConfiguration iconfiguration)
        {
            _connectionString = iconfiguration.GetConnectionString("Default");
        }
        public List<Aluno> GetList()
        {
            var listAluno = new List<Aluno>();
            try
            {
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    SqlCommand cmd = new SqlCommand("SP_ALUNO_GET_LIST", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    con.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        listAluno.Add(new Aluno
                        {
                            Id = Convert.ToInt32(rdr[0]),
                            Nome = Convert.ToString(rdr[1]),
                            Sobrenome = Convert.ToString(rdr[2]),
                            Nascimento = Convert.ToDateTime(rdr[3]),
                            Sexo = Convert.ToChar(rdr[4]),
                            Email = Convert.ToString(rdr[5]),
                            /*DataDeCadastro = Convert.ToDateTime(rdr[15]),*/ //Essa informção está vindo errada
                            Telefone = Convert.ToString(rdr[6]),
                            Cep = Convert.ToString(rdr[7]),
                            Logradouro = Convert.ToString(rdr[8]),
                            Complemento = Convert.ToString(rdr[9]),
                            Bairro = Convert.ToString(rdr[10]),
                            Localidade = Convert.ToString(rdr[11]),
                            UF = Convert.ToString(rdr[12]),
                            DataDeAtualizacao = Convert.ToDateTime(rdr[13]),
                            Ativo = Convert.ToBoolean(rdr[14])
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return listAluno;
        }
    }
}
