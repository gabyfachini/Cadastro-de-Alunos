﻿using Cadastro_de_Alunos;
using Cadastro_de_Alunos.DAL;
using Cadastro_de_Alunos.Models;
using Cadastro_de_Alunos.Services;
using Microsoft.Extensions.Configuration;
using System;
using System.Data.SqlClient;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Linq;
using Dapper;

internal class Program
{
    private static void Main(string[] args)
    {
        IConfiguration _iconfiguration = null;

        Console.WriteLine("GERENCIADOR DE ALUNOS");
        Console.WriteLine();

        while (true)
        {
            Console.WriteLine("1. Cadastrar Aluno");
            Console.WriteLine("2. Listar Alunos");
            Console.WriteLine("3. Buscar Aluno");
            Console.WriteLine("4. Atualizar Aluno"); //Implementar depois, não é tão importante agora
            Console.WriteLine("5. Excluir Aluno"); 
            Console.WriteLine("0. Sair");
            Console.Write("Opção escolhida: ");
            string opcao = Console.ReadLine();
            Console.WriteLine();

            switch (opcao)
            {
                case "1":
                    var novoAluno = new Aluno();

                    Console.WriteLine("Digite o nome do aluno:");
                    novoAluno.Nome = Console.ReadLine();

                    Console.WriteLine("Digite o sobrenome do aluno:");
                    novoAluno.Sobrenome = Console.ReadLine();

                    Console.WriteLine("Digite a data de nascimento (formato: YYYY-MM-DD):"); //Depois colocar outras opções de formato de datas

                    if (DateTime.TryParse(Console.ReadLine(), out DateTime nascimento))
                    {
                        novoAluno.Nascimento = nascimento;
                    }
                    else
                    {
                        Console.WriteLine("Data de nascimento inválida! Tente novamente.");
                        return;
                    }

                    Console.WriteLine("Digite o sexo (M/F):");
                    string sexo;
                    do
                    {
                        sexo = Console.ReadLine().ToUpper();
                        if (sexo != "M" && sexo != "F")
                        {
                            Console.WriteLine("Entrada inválida. Digite Novamente!");
                            return;
                        }
                    } while (sexo != "M" && sexo != "F");
                    novoAluno.Sexo = char.Parse(sexo);

                    Console.WriteLine("Digite o email:");
                    novoAluno.Email = Console.ReadLine();

                    Console.WriteLine("Digite o telefone:");
                    novoAluno.Telefone = Console.ReadLine();

                    Console.WriteLine("Digite o CEP:");
                    //Colocar a conexão com a API
                    novoAluno.Cep = Console.ReadLine();

                    Console.WriteLine("Digite o logradouro:");
                    novoAluno.Logradouro = Console.ReadLine();

                    Console.WriteLine("Digite o complemento:");
                    novoAluno.Complemento = Console.ReadLine();

                    Console.WriteLine("Digite o bairro:");
                    novoAluno.Bairro = Console.ReadLine();

                    Console.WriteLine("Digite a localidade/cidade:");
                    novoAluno.Localidade = Console.ReadLine();

                    Console.WriteLine("Digite o UF:");
                    novoAluno.UF = Console.ReadLine();

                    novoAluno.DataDeAtualizacao = novoAluno.DataDeCadastro = DateTime.Now;
                    novoAluno.Ativo = true;

                    string connectionString = _connectionString.GetConnectionString("Default"); // Obtém a string de conexão usando o método GetConnectionString
                    CadastrarAluno(novoAluno, connectionString); //Passa a string de conexão como argumento ao método

                    Console.Clear();
                    Console.WriteLine("Aluno cadastrado com sucesso!");
                    Console.ReadKey();
                    Console.Clear();
                    break;

                case "2": 
                    Console.WriteLine("REGISTRO DE ALUNOS");
                    Console.WriteLine();
                    GetAppSettingsFile();
                    PrintListarAlunos();
                    Console.Clear();
                    break;

                case "3": 
                    Console.WriteLine("BUSCA DE ALUNO");
                    Console.WriteLine();
                    GetAppSettingsFile();
                    PrintBuscarAlunos();
                    //posteriormente eu vou colocar uma lista de opções para verificar qual informação do aluno a pessoa quer colocar para verificar no banco de dados (FONTEND)
                    Console.ReadKey();
                    break;

                case "4":
                    string connectionString2 = GetConnectionString();
                    var alunos = GetAlunosFromDatabase(connectionString2);
                    Console.WriteLine("ATUALIZAÇÃO DE CADASTRO");
                    Console.WriteLine();
                    Console.WriteLine("Qual informação a ser atualizada?");
                    //Criar o código
                    Console.ReadKey();
                    Console.Clear();
                    break;

                case "5": 
                    Console.WriteLine("EXCLUSÃO DE ALUNO");
                    Console.WriteLine();

                    GetAppSettingsFile();
                    // Obtendo a string de conexão do appsettings.json
                    string minhaConnectionString = _iconfiguration.GetConnectionString("Default");

                    while (true) 
                    {
                        Console.WriteLine("Qual o ID do aluno que cancelou o cadastro?");
                        if (int.TryParse(Console.ReadLine(), out int idEscolhido))
                        {
                            // Passa a string de conexão para o método SoftDelete
                            SoftDelete(minhaConnectionString, idEscolhido);

                        }
                        else
                        {
                            Console.WriteLine("ID inválido. Tente novamente.");
                        }

                        Console.WriteLine("Deseja excluir outro aluno? (S/N)");
                        string resposta2 = Console.ReadLine()?.ToUpper();
                        if (resposta2 != "S")
                        {
                            break;
                        }
                        Console.Clear(); 
                    }

                    Console.ReadKey(); 
                    Console.Clear();
                    break;

                case "0":
                    Console.WriteLine("O programa está prestes a ser encerrado.");
                    return; // Finaliza o método Main, encerrando o programa

                default:
                    Console.WriteLine("Opção inválida!");
                    continue; // Continua pedindo a opção
            }
        }

        void GetAppSettingsFile() //Método de conexão com o banco de dados
        {
            var builder = new ConfigurationBuilder()
                                 .SetBasePath(Directory.GetCurrentDirectory())
                                 .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            _iconfiguration = builder.Build();
        }

        void CadastrarAluno(Aluno aluno, string _connectionString) //Envio de informações para o Banco de Dados
        {
            if (aluno == null)
            {
                throw new ArgumentNullException(nameof(aluno), "O aluno não pode ser nulo.");
            }
            var sql = @"
        INSERT INTO Aluno 
        (Nome, Sobrenome, Nascimento, Sexo, Email, Telefone, Cep, Logradouro, Complemento, Bairro, Localidade, UF, DataDeCadastro, DataDeAtualizacao, Ativo)
        VALUES
        (@Nome, @Sobrenome, @Nascimento, @Sexo, @Email, @Telefone, @Cep, @Logradouro, @Complemento, @Bairro, @Localidade, @UF, @DataDeCadastro, @DataDeAtualizacao, @Ativo)";

            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Nome", aluno.Nome);
                    command.Parameters.AddWithValue("@Sobrenome", aluno.Sobrenome);
                    command.Parameters.AddWithValue("@Nascimento", aluno.Nascimento);
                    command.Parameters.AddWithValue("@Sexo", aluno.Sexo);
                    command.Parameters.AddWithValue("@Email", aluno.Email);
                    command.Parameters.AddWithValue("@Telefone", aluno.Telefone);
                    command.Parameters.AddWithValue("@Cep", aluno.Cep);
                    command.Parameters.AddWithValue("@Logradouro", aluno.Logradouro);
                    command.Parameters.AddWithValue("@Complemento", aluno.Complemento);
                    command.Parameters.AddWithValue("@Bairro", aluno.Bairro);
                    command.Parameters.AddWithValue("@Localidade", aluno.Localidade);
                    command.Parameters.AddWithValue("@UF", aluno.UF);
                    command.Parameters.AddWithValue("@DataDeCadastro", aluno.DataDeCadastro);
                    command.Parameters.AddWithValue("@DataDeAtualizacao", aluno.DataDeAtualizacao);
                    command.Parameters.AddWithValue("@Ativo", aluno.Ativo);

                    connection.Open();
                    command.ExecuteNonQuery(); // Executa o comando SQL
                }
            }

        }

        static List<Aluno> GetAlunosFromDatabase(string connectionString) // Método para buscar alunos no banco de dados
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var sql = "SELECT * FROM Aluno WHERE Ativo = 1";
                var alunos = connection.Query<Aluno>(sql).ToList(); // Usando Dapper para mapear os dados para objetos Aluno
                return alunos;
            }
        }

        void PrintListarAlunos()
        {
            var AlunoDAL = new AlunoDAL(_iconfiguration);
            var listAluno = AlunoDAL.GetList();
            listAluno.ForEach(item =>
            {
                Console.WriteLine($"ID: {item.Id}");
                Console.WriteLine($"Nome: {item.Nome}");
                Console.WriteLine($"Sobrenome: {item.Sobrenome}");
                Console.WriteLine($"Nascimento: {item.Nascimento}");
                Console.WriteLine($"Sexo: {item.Sexo}");
                Console.WriteLine($"Email: {item.Email}");
                Console.WriteLine($"Telefone: {item.Telefone}");
                Console.WriteLine($"CEP: {item.Cep}");
                Console.WriteLine($"Logradouro: {item.Logradouro}");
                Console.WriteLine($"Complemento: {item.Complemento}");
                Console.WriteLine($"Bairro: {item.Bairro}");
                Console.WriteLine($"Localidade/Cidade: {item.Localidade}");
                Console.WriteLine($"UF: {item.UF}");
                Console.WriteLine();
            });
            Console.WriteLine("Pressione qualquer tecla para voltar ao menu");
            Console.ReadKey();
            Console.Clear();
        }

        void PrintBuscarAlunos()
        {
            var Aluno = new AlunoDAL(_iconfiguration);
            var listAluno = Aluno.GetList();
            Console.Write("Digite o ID do aluno que deseja buscar: ");
            Console.WriteLine();

            while (true)
            {
                if (int.TryParse(Console.ReadLine(), out int idEscolhido))
                {
                    var aluno = listAluno.FirstOrDefault(a => a.Id == idEscolhido);

                    if (aluno != null)
                    {
                        Console.WriteLine($"ID: {aluno.Id}");
                        Console.WriteLine($"Nome: {aluno.Nome}");
                        Console.WriteLine($"Sobrenome: {aluno.Sobrenome}");
                        Console.WriteLine($"Nascimento: {aluno.Nascimento}");
                        Console.WriteLine($"Sexo: {aluno.Sexo}");
                        Console.WriteLine($"Email: {aluno.Email}");
                        Console.WriteLine($"Telefone: {aluno.Telefone}");
                        Console.WriteLine($"CEP: {aluno.Cep}");
                        Console.WriteLine($"Logradouro: {aluno.Logradouro}");
                        Console.WriteLine($"Complemento: {aluno.Complemento}");
                        Console.WriteLine($"Bairro: {aluno.Bairro}");
                        Console.WriteLine($"Localidade/Cidade: {aluno.Localidade}");
                        Console.WriteLine($"UF: {aluno.UF}");
                    }
                    else
                    {
                        Console.WriteLine("Aluno não encontrado.");
                    }
                }
                Console.WriteLine("Pressione qualquer tecla para voltar ao menu");
                Console.ReadKey();
                Console.Clear();
                break;
            }
        }

        static void SoftDelete(string connectionString2, int idAluno) // Método SoftDelete para desativar o aluno
        {
            // Query SQL para atualizar o campo Ativo no banco de dados
            var sql = @"
        UPDATE Aluno 
        SET Ativo = 0, DataDeAtualizacao = @DataDeAtualizacao 
        WHERE Id = @Id";

            // Conexão com o banco de dados
            using (var connection = new SqlConnection(connectionString2))
            {
                using (var command = new SqlCommand(sql, connection))
                {
                    // Adiciona os parâmetros
                    command.Parameters.AddWithValue("@Id", idAluno);
                    command.Parameters.AddWithValue("@DataDeAtualizacao", DateTime.Now);

                    try
                    {
                        connection.Open(); // Abre a conexão
                        int rowsAffected = command.ExecuteNonQuery(); // Executa o comando SQL

                        if (rowsAffected > 0)
                        {
                            Console.WriteLine($"Aluno com ID {idAluno} foi marcado como inativo.");
                        }
                        else
                        {
                            Console.WriteLine("Aluno não encontrado no banco de dados.");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Erro ao desativar aluno: {ex.Message}");
                    }
                }
            }
        }

        static string GetConnectionString() // Método para obter o connectionString do appsettings.json ou de algum outro lugar
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            var configuration = builder.Build();
            return configuration.GetConnectionString("Default");
        }

       

    }
}
