using Cadastro_de_Alunos;
using Cadastro_de_Alunos.DAL;
using Cadastro_de_Alunos.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

Console.WriteLine("GERENCIADOR DE ALUNOS");
Console.WriteLine();

while (true)
{
    Console.WriteLine("1. Cadastrar Aluno");
    Console.WriteLine("2. Listar Alunos");
    Console.WriteLine("3. Buscar Alunos"); 
    Console.WriteLine("4. Atualizar Aluno"); //Aqui eu coloco se o aluno ainda esta ativo ou não
    Console.WriteLine("5. Excluir Aluno");
    Console.WriteLine("0. Sair");
    Console.Write("Opção escolhida: ");
    string opcao = Console.ReadLine();
    Console.WriteLine();

    switch (opcao)
    {
        case "1":
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
            string Email = Console.ReadLine();
            Console.WriteLine("f. Telefone: ");
            string telefone = Console.ReadLine();
            Console.WriteLine("g. CEP residencial: ");
            string cep = Console.ReadLine(); //Aqui faz a consulta com a API? As demais informações vem por API, certo?
            //Aqui coloca a integração com o banco de dados 
            Console.WriteLine("Cadastro efetuado com sucesso!");
            Console.ReadKey(); //Continua a execução do sistema quando digitar qualquer tecla
            Console.Clear();
            //Depois incrementar aqui a opção da pessoa fazer um novo cadastro sem sair do case, casos de famílias que fazem cadastro junto
            break;

        case "2":
            Console.WriteLine("REGISTRO DE ALUNOS");
            Console.WriteLine();

            Console.Clear();
            break;

        case "3":
            Console.WriteLine("INFORMAÇÕES DOS ALUNOS");
            Console.WriteLine();
            Console.WriteLine("Qual o nome do aluno?"); //posteriormente eu vou colocar uma lista de opções para verificar qual informação do aluno a pessoa quer colocar para verificar no banco de dados
                                                        //aqui analisa o dado, procura no banco de dados e traz as informações para visualização
                                                        //Depois incrementar aqui a opção da pessoa fazer uma nova pesquisa de aluno sem sair do case, promove agilidade

            Console.ReadKey(); //Continua a execução do sistema quando digitar qualquer tecla
            Console.Clear();
            break;

        case "4":
            Console.WriteLine("ATUALIZAÇÃO DE CADASTRO");
            Console.WriteLine();
            Console.WriteLine("Nome do Aluno: ");
            string nome2 = Console.ReadLine();
            //Procura no banco e traz as informações, como no case 3
            Console.WriteLine("Qual informação deseja atualizar?");
            //Recebe a informação e compara com as informações mostradas
            Console.WriteLine("Nova informação: ");
            //Depois incrementar aqui a opção da pessoa fazer uma nova atualização sem sair do case, promove agilidade
            Console.ReadKey(); //Continua a execução do sistema quando digitar qualquer tecla
            Console.Clear();
            break; 
        case "5":
            Console.WriteLine("EXCLUSÃO DE ALUNO");
            Console.WriteLine();
            Console.WriteLine("Qual aluno cancelou cadastro?");
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

/*class Program
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
        var Aluno = new AlunoDAL(_iconfiguration);
        var listAluno = Aluno.GetList();
        listAluno.ForEach(item =>
        {
            Console.WriteLine(item.Id);
            Console.WriteLine(item.Nome);
            Console.WriteLine(item.Sobrenome);
            Console.WriteLine(item.Nascimento);
            Console.WriteLine(item.Sexo);
            Console.WriteLine(item.Email);
            Console.WriteLine(item.DataDeCadastro);
            Console.WriteLine(item.Telefone);
            Console.WriteLine(item.Cep);
            Console.WriteLine(item.Logradouro);
            Console.WriteLine(item.Complemento);
            Console.WriteLine(item.Bairro);
            Console.WriteLine(item.Localidade);
            Console.WriteLine(item.UF);
            Console.WriteLine(item.DataDeAtualizacao);
            Console.WriteLine(item.Ativo);
            Console.WriteLine();
        });
        Console.WriteLine("Press any key to stop.");
        Console.ReadKey();
    }
}

Console.ReadKey(); //Continua a execução do sistema quando digitar qualquer tecla*/