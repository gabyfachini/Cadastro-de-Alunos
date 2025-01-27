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
using System.ComponentModel.DataAnnotations;

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
            Console.WriteLine("4. Atualizar Aluno - Em configuração"); //Geralmente nesse caso é melhor validar pelo front-end já considerando a usabilidade do cliente
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
                    if (string.IsNullOrWhiteSpace(novoAluno.Nome) || novoAluno.Nome.Length < 2 || novoAluno.Nome.Length > 100)
                    {
                        Console.WriteLine("Nome deve ter entre 2 e 100 caracteres e não pode ser vazio.");
                        return;
                    }

                    Console.WriteLine("Digite o sobrenome do aluno:");
                    novoAluno.Sobrenome = Console.ReadLine();
                    if (string.IsNullOrWhiteSpace(novoAluno.Sobrenome) || novoAluno.Sobrenome.Length < 2 || novoAluno.Sobrenome.Length > 100)
                    {
                        Console.WriteLine("Sobrenome deve ter entre 2 e 100 caracteres e não pode ser vazio.");
                        return;
                    }

                    Console.WriteLine("Digite a data de nascimento (formato: YYYY-MM-DD):"); //O tratamento de formato de datas acontece no Front-end
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
                    if (string.IsNullOrWhiteSpace(novoAluno.Email) || !new EmailAddressAttribute().IsValid(novoAluno.Email) || !novoAluno.Email.EndsWith("@exemplo.com"))
                    {
                        Console.WriteLine("E-mail inválido ou o e-mail não pertence ao domínio 'exemplo.com'.");
                        return;
                    }

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
                        novoAluno.Complemento = string.IsNullOrWhiteSpace(endereco.Complemento) ? "N/A" : endereco.Complemento;
                        novoAluno.Bairro = endereco.Bairro;
                        novoAluno.Localidade = endereco.Localidade;
                        novoAluno.UF = endereco.UF;

                        // Validação do modelo usando os validadores de data annotations
                        var validationContext = new ValidationContext(novoAluno);
                        var validationResults = new List<ValidationResult>();
                        bool isValid = Validator.TryValidateObject(novoAluno, validationContext, validationResults, true);

                        if (isValid)
                        {
                            string connectionString = _connectionString.GetConnectionString("Default");
                            CadastrarAluno(novoAluno, connectionString);

                            Console.WriteLine();
                            Console.WriteLine("Aluno cadastrado com sucesso!");
                        }
                        else
                        {
                            foreach (var validationResult in validationResults)
                            {
                                Console.WriteLine($"Erro de validação: {validationResult.ErrorMessage}");
                            }
                        }
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
                    //posteriormente eu vou colocar uma lista de opções para verificar qual informação do aluno a pessoa quer colocar para verificar no banco de dados (Front-end)
                    Console.ReadKey();
                    Console.Clear();
                    break;

                case "4":
                    string connectionStringAdjust = GetConnectionString();

                    if (string.IsNullOrEmpty(connectionStringAdjust))
                    {
                        Console.WriteLine("A string de conexão não foi inicializada corretamente.");
                        return;
                    }

                    Console.WriteLine("ATUALIZAÇÃO DE CADASTRO");
                    GetAppSettingsFile();
                    Console.Write("Digite o ID do aluno que deseja buscar: ");
                    int alunoId;

                    if (int.TryParse(Console.ReadLine(), out alunoId))
                    {
                        Console.WriteLine();
                        PrintAjustarAlunos(alunoId);

                        Console.WriteLine("Qual informação deseja atualizar?");
                        Console.WriteLine("A. Nome");
                        Console.WriteLine("B. Sobrenome");
                        Console.WriteLine("C. Nascimento");
                        Console.WriteLine("D. Sexo");
                        Console.WriteLine("E. Email");
                        Console.WriteLine("F. Telefone");
                        Console.WriteLine("G. Endereço");
                        Console.Write("Digite a opção: ");
                        string escolha = Console.ReadLine()?.ToUpper();
                        Console.WriteLine();

                        switch (escolha)
                        {
                            case "A":
                                Console.WriteLine("Digite o novo nome:");
                                string novoNome = Console.ReadLine();
                                AtualizarAluno(connectionStringAdjust, alunoId, "Nome", novoNome);
                                break;

                            case "B":
                                Console.WriteLine("Digite o novo sobrenome:");
                                string novoSobrenome = Console.ReadLine();
                                AtualizarAluno(connectionStringAdjust, alunoId, "Sobrenome", novoSobrenome);
                                break;

                            case "C":
                                Console.WriteLine("Digite a nova data de nascimento (formato: YYYY-MM-DD):");
                                if (DateTime.TryParse(Console.ReadLine(), out DateTime novaDataNascimento))
                                {
                                    AtualizarAluno(connectionStringAdjust, alunoId, "Nascimento", novaDataNascimento.ToString("yyyy-MM-dd"));
                                }
                                else
                                {
                                    Console.WriteLine("Data inválida!");
                                }
                                break;

                            case "D":
                                Console.WriteLine("Digite o novo sexo (M/F):");
                                string novoSexo = Console.ReadLine().ToUpper();
                                if (novoSexo == "M" || novoSexo == "F")
                                {
                                    AtualizarAluno(connectionStringAdjust, alunoId, "Sexo", novoSexo);
                                }
                                else
                                {
                                    Console.WriteLine("Sexo inválido!");
                                }
                                break;

                            case "E":
                                Console.WriteLine("Digite o novo e-mail:");
                                string novoEmail = Console.ReadLine();
                                AtualizarAluno(connectionStringAdjust, alunoId, "Email", novoEmail);
                                break;

                            case "F":
                                Console.WriteLine("Digite o novo telefone:");
                                string novoTelefone = Console.ReadLine();
                                AtualizarAluno(connectionStringAdjust, alunoId, "Telefone", novoTelefone);
                                break;

                            case "G": //Implementar usando a API
                                Console.WriteLine("Digite o novo CEP:");
                                string novoCep = Console.ReadLine();
                                break;

                            default:
                                Console.WriteLine("Opção inválida. Tente novamente.");
                                break;
                        }
                    }
                    else
                    {
                        Console.WriteLine("ID inválido.");
                    }

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
            string connectionString = GetConnectionString();
            Console.WriteLine("String de Conexão Carregada: " + connectionString);
        }

        string GetConnectionString() // Método para obter o connectionString do appsettings.json e é bom para usar em injeção de dependência
        {
            return _iconfiguration.GetConnectionString("Default");
        }

        void CadastrarAluno(Aluno aluno, string _connectionString) //Envio de informações para o Banco de Dados
        {
            string connectionString = _iconfiguration.GetConnectionString("Default");

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

        void PrintListarAlunos() //Lista todos os alunos do banco de dados
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

        void PrintBuscarAlunos() //Busca um aluno por ID específico
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

        void PrintAjustarAlunos(int alunoId) //Busca o aluno que vai ter as iñformações alteradas
        {
            var Aluno = new AlunoDAL(_iconfiguration);
            var listAluno = Aluno.GetList();

            var aluno = listAluno.FirstOrDefault(a => a.Id == alunoId);

            if (aluno != null)
            {
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
                Console.WriteLine($"Localidade/Cidade: {aluno.Localidade} {aluno.UF}");
            }
            else
            {
                Console.WriteLine("Aluno não encontrado.");
            }

            Console.WriteLine();
        }

        static void AtualizarAluno(string connectionString, int alunoId, string campo, string novoValor)
        {

            if (string.IsNullOrEmpty(connectionString))
            {
                Console.WriteLine("A string de conexão não foi inicializada corretamente.");
                return;
            }

            // Query SQL para atualizar o campo específico do aluno
            var sql = $@"
        UPDATE Aluno 
        SET {campo} = @NovoValor, DataDeAtualizacao = @DataDeAtualizacao
        WHERE Id = @Id";

            // Conexão com o banco de dados
            using (var connection = new SqlConnection(connectionString))
            {
                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Id", alunoId);
                    command.Parameters.AddWithValue("@NovoValor", novoValor);
                    command.Parameters.AddWithValue("@DataDeAtualizacao", DateTime.Now);

                    try
                    {
                        connection.Open();
                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            Console.WriteLine($"Cadastro do aluno com ID {alunoId} foi atualizado com sucesso.");
                        }
                        else
                        {
                            Console.WriteLine("Aluno não encontrado no banco de dados.");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Erro ao atualizar aluno: {ex.Message}");
                    }
                }
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
    }
}
