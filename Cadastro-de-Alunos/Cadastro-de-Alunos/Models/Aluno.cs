using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cadastro_de_Alunos.Models
{
    public class Aluno
    {
        public Aluno(string nome, string sobrenome)
        {
            Nome = nome;
            Sobrenome = sobrenome;
        }
        public int Id { get; set; } //obrigatório
        public string Nome { get; set; } //obrigatório
        public string Sobrenome { get; set; } //obrigatório
        public DateTime Nascimento { get; set; } //obrigatório
        public char Sexo { get; set; } //obrigatório
        public string Email { get; set; } //colocar como obrigatório
        public DateTime DataDeCadastro { get; set; } //obrigatório
        public string Telefone { get; set; } //colocar como obrigatório e posteriormente fazer um tratamento para ele ser identificado como Br ou demais países
        public string Cep { get; set; } //colocar como obrigatório
        public string Logradouro { get; set; }
        public string Complemento { get; set; }
        public string Bairro { get; set; }
        public string Localidade { get; set; }
        public string UF { get; set; }
        public DateTime? DataDeAtualizacao { get; set; }     //CAMPO DE AUDITORIA - Data da última atualização (pode ser nula) 
        public bool Ativo { get; set; }                       //CAMPO DE AUDITORIA - 1 = ativo e 0 = excluído - campo obrigatório

    }

    class Program
    {
        private static IConfiguration _iconfiguration;
        static void Main(string[] args)
        {
            GetAppSettingsFile();
            PrintCountries();
        }
        static void GetAppSettingsFile()
        {
            var builder = new ConfigurationBuilder()
                                 .SetBasePath(Directory.GetCurrentDirectory())
                                 .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            _iconfiguration = builder.Build();
        }
        static void PrintCountries()
        {
            var CountryDAL = new CountryDAL(_iconfiguration);
            var listCountryModel = countryDAL.GetList();
            listCountryModel.ForEach(item =>
            {
                Console.WriteLine(item.Country);
            });
            Console.WriteLine("Press any key to stop.");
            Console.ReadKey();
        }
    }
}