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
        public int Id { get; set; } //Obrigatório no Banco de Dados
        public string Nome { get; set; } //Obrigatório no Banco de Dados
        public string Sobrenome { get; set; } //Obrigatório no Banco de Dados
        public DateTime Nascimento { get; set; } //Obrigatório no Banco de Dados
        public char Sexo { get; set; } //Obrigatório no Banco de Dados
        public string Email { get; set; } 
        public DateTime DataDeCadastro { get; set; } //Obrigatório no Banco de Dados
        public string Telefone { get; set; } 
        public string Cep { get; set; }
        public string Logradouro { get; set; }
        public string Complemento { get; set; }
        public string Bairro { get; set; }
        public string Localidade { get; set; }
        public string UF { get; set; }
        public DateTime? DataDeAtualizacao { get; set; } //CAMPO DE AUDITORIA - Data da última atualização (pode ser nula) 
        public bool Ativo { get; set; } //CAMPO DE AUDITORIA | 1 = ativo e 0 = excluído | Obrigatório no Banco de Dados

    }
}