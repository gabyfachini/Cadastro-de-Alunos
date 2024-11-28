using Cadastro_de_Alunos.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Cadastro_de_Alunos.Services
{
    public class ViaCepService
    {
        private readonly HttpClient _httpClient;
        public ViaCepService ()
        {
            _httpClient = new HttpClient (); //estudar injeção de dependencia, trycatch
        }
        public async Task<Endereco?> BuscarEnderecoPorCepAsync(string cep)
        {
            try
            {
                cep = cep.Replace("-", "").Replace(".", "").Trim();
                var response = await _httpClient.GetAsync($"https://viacep.com.br/ws/{cep}/json/");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<Endereco>(content);
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao buscar CEP: {ex.Message}");
                return null;
            }
        }
    }


}
