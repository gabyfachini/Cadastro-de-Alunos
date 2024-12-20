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
        /*async Task Main(string[] args)
        {*/

        Console.WriteLine("GERENCIADOR DE ALUNOS");
        Console.WriteLine();

        while (true)
        {
            Console.WriteLine("1. Cadastrar Aluno");
            Console.WriteLine("2. Listar Alunos");
            Console.WriteLine("3. Buscar Aluno");
            Console.WriteLine("4. Atualizar Aluno"); //Aqui eu coloco se o aluno ainda esta ativo ou não
            Console.WriteLine("5. Excluir Aluno"); //Não era para eu inativar aqui?
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
                    Console.WriteLine("Digite a data de nascimento (formato: YYYY-MM-DD):");
                    if (DateTime.TryParse(Console.ReadLine(), out DateTime nascimento))
                    {
                        novoAluno.Nascimento = nascimento;
                    }
                    else
                    {
                        Console.WriteLine("Data de nascimento inválida!");
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
                        }
                    } while (sexo != "M" && sexo != "F");
                    novoAluno.Sexo = char.Parse(sexo);

                    Console.WriteLine("Digite o email:");
                    novoAluno.Email = Console.ReadLine();
                    Console.WriteLine("Digite o telefone:");
                    novoAluno.Telefone = Console.ReadLine();
                    Console.WriteLine("Digite o CEP:");
                    /*await CadastrarAlunoAsync();*/
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
                    CadastrarAluno(novoAluno, connectionString); //Passe a string de conexão como argumento ao método

                    Console.Clear();
                    Console.WriteLine("Aluno cadastrado com sucesso!");
                    Console.ReadKey();
                    Console.Clear();
                    break;

                case "2": //Fazer um filtro para vir só aluno ativos
                    Console.WriteLine("REGISTRO DE ALUNOS");
                    Console.WriteLine();
                    GetAppSettingsFile();// Configurações de leitura do arquivo
                    PrintAlunos();  // Exibe os dados dos alunos
                    Console.Clear();
                    break;

                case "3": 
                    Console.WriteLine("BUSCA DE ALUNO");
                    Console.WriteLine();
                    GetAppSettingsFile();
                    PrintCountries2();
                    //posteriormente eu vou colocar uma lista de opções para verificar qual informação do aluno a pessoa quer colocar para verificar no banco de dados (FONTEND)
                    //Depois incrementar aqui a opção da pessoa fazer uma nova pesquisa de aluno sem sair do case, promove agilidade
                    Console.ReadKey(); //Continua a execução do sistema quando digitar qualquer tecla
                    break;

                case "4":
                    string connectionString2 = GetConnectionString();
                    var alunos = GetAlunosFromDatabase(connectionString2);
                    Console.WriteLine("ATUALIZAÇÃO DE CADASTRO");
                    Console.WriteLine();
                    Console.WriteLine("Qual o ID do aluno a ser desativado?");
                    int idEscolhido = int.Parse(Console.ReadLine());
                    SoftDelete(alunos, idEscolhido);
                    //Depois incrementar aqui a opção da pessoa fazer uma nova atualização sem sair do case, promove agilidade*/
                    Console.ReadKey();
                    Console.Clear();
                    break;
                case "5": //Remover isso, trazer o case 4 para ca
                    Console.WriteLine("EXCLUSÃO DE ALUNO");
                    Console.WriteLine();
                    Console.WriteLine("Qual o ID do aluno que cancelou o cadastro?");
                    string nome3 = Console.ReadLine();
                    Console.ReadKey(); //Continua a execução do sistema quando digitar qualquer tecla
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

        void GetAppSettingsFile()
        {
            var builder = new ConfigurationBuilder()
                                 .SetBasePath(Directory.GetCurrentDirectory())
                                 .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            _iconfiguration = builder.Build();
        }

        void PrintAlunos()
        {
            var Aluno = new AlunoDAL(_iconfiguration);
            var listAluno = Aluno.GetList();
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
        }
        void PrintCountries2() 
        {
            var Aluno = new AlunoDAL(_iconfiguration);
            var listAluno = Aluno.GetList();
            Console.Write("Digite o ID do aluno que deseja buscar: ");
            Console.WriteLine();
            while (true) {

                if (int.TryParse(Console.ReadLine(), out int idEscolhido))
                {

                    var aluno = listAluno.FirstOrDefault(a => a.Id == idEscolhido); // Procurar o aluno na lista com o ID fornecido
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
                        Console.WriteLine("Aluno não encontrado com o ID fornecido.");
                    }
                }
                else
                {
                    Console.WriteLine("ID inválido. Por favor, digite um número inteiro.");
                }
                Console.ReadKey();
            }
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
                connection.Execute(sql, aluno);
            }
        }

        static List<Aluno> GetAlunosFromDatabase(string connectionString) // Método para buscar alunos no banco de dados
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var sql = "SELECT Id, Nome, Ativo, DataDeAtualizacao FROM Aluno"; // Ajuste a consulta conforme sua tabela
                var alunos = connection.Query<Aluno>(sql).ToList(); // Usando Dapper para mapear os dados para objetos Aluno
                return alunos;
            }
        }

        static void SoftDelete(List<Aluno> alunos, int idAluno) // Método SoftDelete para desativar o aluno
        {
            var aluno = alunos.FirstOrDefault(a => a.Id == idAluno);
            if (aluno != null)
            {
                aluno.Ativo = false;
                aluno.DataDeAtualizacao = DateTime.Now;
                Console.WriteLine($"Aluno {aluno.Nome} foi marcado como inativo."); //Conectar com o banco
            }
            else
            {
                Console.WriteLine("Aluno não encontrado.");
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

/*}async Task CadastrarAlunoAsync()
{
    // Coletar informações do aluno e interligar ao Banco de Dados
    Console.WriteLine("CADASTRO DE ALUNO");
    Console.WriteLine();
    Console.WriteLine("Digite as seguintes informações: ");
    Console.WriteLine("a. Nome:");
    string nome = Console.ReadLine();
    Console.WriteLine("b. Sobrenome: ");
    string sobrenome = Console.ReadLine();
    Console.WriteLine("c. Data de Nascimento: ");
    string nascimento = Console.ReadLine();
    Console.WriteLine("d. Sexo (M/F): ");
    string sexo = Console.ReadLine();
    Console.WriteLine("e. E-mail: ");
    string email = Console.ReadLine();
    Console.WriteLine("f. Telefone: ");
    string telefone = Console.ReadLine();

    // Solicita o CEP ao usuário
    var viaCepService = new ViaCepService();
    Console.WriteLine("Digite o CEP para buscar o endereço: ");
    string cep = Console.ReadLine();

    // Realiza a busca do endereço pelo CEP
    var endereco = await viaCepService.BuscarEnderecoPorCepAsync(cep);

    if (endereco != null)
    {
        Console.WriteLine("Endereço encontrado:");
        Console.WriteLine($"CEP: {endereco.Cep}");
        Console.WriteLine($"Logradouro: {endereco.Logradouro}");
        Console.WriteLine($"Bairro: {endereco.Bairro}");
        Console.WriteLine($"Localidade: {endereco.Localidade}");
        Console.WriteLine($"UF: {endereco.UF}");
    }
    else
    {
        Console.WriteLine("Não foi possível encontrar o endereço para o CEP informado.");
        // Possibilidade de digitar novamente o CEP
    }

    Console.WriteLine("Cadastro efetuado com sucesso!");
    Console.ReadKey(); // Continua a execução do sistema quando digitar qualquer tecla
    Console.Clear();
}*/