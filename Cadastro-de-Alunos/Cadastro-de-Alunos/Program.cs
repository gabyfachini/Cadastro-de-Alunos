using Cadastro_de_Alunos;
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
using System.Net.Http;
using Newtonsoft.Json;
using System.Reflection.Metadata;

internal class Program
{
    private static async Task /*void*/ Main(string[] args)
    {
        IConfiguration _iconfiguration = null;

        Console.WriteLine("GERENCIADOR DE ALUNOS");
        Console.WriteLine();

        while (true)
        {
            Console.WriteLine("1. Cadastrar Aluno");
            Console.WriteLine("2. Listar Alunos");
            Console.WriteLine("3. Buscar Aluno");
            Console.WriteLine("4. Atualizar Aluno - Não configurado"); //Implementar depois, não é tão importante agora (FRONTEND?)
            Console.WriteLine("5. Excluir Aluno"); 
            Console.WriteLine("0. Sair");
            Console.Write("Opção escolhida: ");
            string opcao = Console.ReadLine();
            Console.WriteLine();

            switch (opcao)
            {
                case "1":
                    var novoAluno = new Aluno();

                    Console.WriteLine("Digite o nome* do aluno:");
                    novoAluno.Nome = Console.ReadLine();

                    Console.WriteLine("Digite o sobrenome* do aluno:");
                    novoAluno.Sobrenome = Console.ReadLine();

                    Console.WriteLine("Digite a data de nascimento* (formato: YYYY-MM-DD):"); //O tratamento de formato de datas acontece no Front-end

                    if (DateTime.TryParse(Console.ReadLine(), out DateTime nascimento))
                    {
                        novoAluno.Nascimento = nascimento;
                    }
                    else
                    {
                        Console.WriteLine("Data de nascimento inválida! Tente novamente.");
                        return;
                    }

                    Console.WriteLine("Digite o sexo* (M/F):");
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

                    novoAluno.DataDeAtualizacao = novoAluno.DataDeCadastro = DateTime.Now;
                    novoAluno.Ativo = true;
                    
                    Console.WriteLine("Digite o CEP:");
                    var viaCepService = new ViaCepService(new HttpClient());
                    string cep = Console.ReadLine();
                    var endereco = await viaCepService.BuscarEnderecoPorCepAsync(cep);

                    if (endereco != null)
                    {
                        Console.WriteLine();
                        Console.WriteLine("Dados do Endereço do Aluno:");
                        Console.WriteLine($"CEP: {endereco.Cep}");
                        Console.WriteLine($"Logradouro: {endereco.Logradouro}");
                        Console.WriteLine($"Complemento: {endereco.Complemento}");
                        Console.WriteLine($"Bairro: {endereco.Bairro}");
                        Console.WriteLine($"Localidade: {endereco.Localidade}");
                        Console.WriteLine($"UF: {endereco.UF}");

                        novoAluno.Cep = endereco.Cep;
                        novoAluno.Logradouro = endereco.Logradouro;
                        novoAluno.Complemento = endereco.Complemento ?? "N/A"; // Se complemento for nulo subtitui por N/A
                        novoAluno.Bairro = endereco.Bairro;
                        novoAluno.Localidade = endereco.Localidade;
                        novoAluno.UF = endereco.UF;

                        string connectionString = _connectionString.GetConnectionString("Default");
                        CadastrarAluno(novoAluno, connectionString);

                        Console.WriteLine();
                        Console.WriteLine("Aluno cadastrado com sucesso!");
                    }
                    else
                    {
                        Console.WriteLine("Endereço não encontrado.");
                    }

                    Console.ReadKey();
                    Console.Clear();
                    break;

                case "2":
                    Console.Clear();
                    Console.WriteLine("REGISTRO DE ALUNOS");
                    Console.WriteLine();
                    GetAppSettingsFile();
                    PrintListarAlunos();
                    Console.Clear();
                    break;

                case "3": 
                    Console.WriteLine("BUSCA DE ALUNO");
                    GetAppSettingsFile();
                    PrintBuscarAlunos();
                    //posteriormente eu vou colocar uma lista de opções para verificar qual informação do aluno a pessoa quer colocar para verificar no banco de dados (FONTEND)
                    Console.ReadKey();
                    Console.Clear();
                    break;

                case "4":
                    string connectionString2 = GetConnectionString();
                    Console.WriteLine("ATUALIZAÇÃO DE CADASTRO");
                    Console.WriteLine("Qual informação a ser atualizada?");
                    //Criar o código
                    Console.ReadKey();
                    Console.Clear();
                    break;

                case "5": 
                    Console.WriteLine("EXCLUSÃO DE ALUNO");
                    GetAppSettingsFile();
                    string minhaConnectionString = _iconfiguration.GetConnectionString("Default");

                    while (true) 
                    {
                        Console.WriteLine("Qual o ID do aluno que cancelou o cadastro?");
                        if (int.TryParse(Console.ReadLine(), out int idEscolhido))
                        {
                            SoftDelete(minhaConnectionString, idEscolhido);
                        }
                        else
                        {
                            Console.WriteLine("ID inválido. Tente novamente.");
                        }
                        Console.WriteLine();
                        Console.WriteLine("Deseja excluir outro aluno? (S/N)");
                        string resposta2 = Console.ReadLine()?.ToUpper();
                        if (resposta2 != "S")
                        {
                            break;
                        }
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
                throw new ArgumentNullException(nameof(aluno), "O nome do aluno precisa ser preenchido.");
            }
            var sql = @"
        INSERT INTO Aluno 
        (Nome, Sobrenome, Nascimento, Sexo, Email, Telefone, Cep, Logradouro, Complemento, Bairro, Localidade, UF, DataDeCadastro, DataDeAtualizacao, Ativo)
        VALUES
        (@Nome, @Sobrenome, @Nascimento, @Sexo, @Email, @Telefone, @Cep, @Logradouro, @Complemento, @Bairro, @Localidade, @UF,  @DataDeCadastro, @DataDeAtualizacao, @Ativo)";

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

        void PrintListarAlunos()
        {
            var AlunoDAL = new AlunoDAL(_iconfiguration);
            var listAluno = AlunoDAL.GetList();
            listAluno.ForEach(item =>
            {
                Console.WriteLine($"ID: {item.Id}");
                Console.WriteLine($"Nome: {item.Nome} {item.Sobrenome}");
                Console.WriteLine($"Nascimento: {item.Nascimento}");
                Console.WriteLine($"Sexo: {item.Sexo}");
                Console.WriteLine($"Email: {item.Email}");
                Console.WriteLine($"Telefone: {item.Telefone}");
                Console.WriteLine($"CEP: {item.Cep}");
                Console.WriteLine($"Logradouro: {item.Logradouro}");
                Console.WriteLine($"Complemento: {item.Complemento}");
                Console.WriteLine($"Bairro: {item.Bairro}");
                Console.WriteLine($"Localidade/Cidade: {item.Localidade} {item.UF}");
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

            while (true)
            {
                if (int.TryParse(Console.ReadLine(), out int idEscolhido))
                {
                    var aluno = listAluno.FirstOrDefault(a => a.Id == idEscolhido);

                    if (aluno != null)
                    {
                        Console.WriteLine();
                        Console.WriteLine($"ID: {aluno.Id}");
                        Console.WriteLine($"Nome: {aluno.Nome} {aluno.Sobrenome}");
                        Console.WriteLine($"Nascimento: {aluno.Nascimento}");
                        Console.WriteLine($"Sexo: {aluno.Sexo}");
                        Console.WriteLine($"Email: {aluno.Email}");
                        Console.WriteLine($"Telefone: {aluno.Telefone}");
                        Console.WriteLine($"CEP: {aluno.Cep}");
                        Console.WriteLine($"Logradouro: {aluno.Logradouro}");
                        Console.WriteLine($"Complemento: {aluno.Complemento}");
                        Console.WriteLine($"Bairro: {aluno.Bairro}");
                        Console.WriteLine($"Localidade/Cidade: {aluno.Localidade} {aluno.UF}");
                    }
                    else
                    {
                        Console.WriteLine();
                        Console.WriteLine("Aluno não encontrado.");
                    }
                }
                Console.WriteLine();
                Console.WriteLine("Pressione qualquer tecla para voltar ao menu");
                return;
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
                    command.Parameters.AddWithValue("@Id", idAluno);
                    command.Parameters.AddWithValue("@DataDeAtualizacao", DateTime.Now);

                    try
                    {
                        connection.Open(); 
                        int rowsAffected = command.ExecuteNonQuery(); 

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

        string GetConnectionString() // Método para obter o connectionString do appsettings.json
        {
            return _iconfiguration.GetConnectionString("Default");
        }

    }
}
