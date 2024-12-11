using Cadastro_de_Alunos.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Cadastro_de_Alunos.Models;

namespace Cadastro_de_Alunos.Services
{
    public class AlunoService
    {
        private readonly ApplicationDbContext _context;

        public AlunoService(ApplicationDbContext context)
        {
            _context = context;
        }

        #region Create
        public async Task CreateAlunoAsync(Aluno aluno)
        {
            if (aluno == null)
                throw new ArgumentNullException(nameof(aluno));

            aluno.DataDeCadastro = DateTime.Now; // Definindo a data de cadastro
            aluno.Ativo = true; // O aluno será ativo por padrão ao ser criado

            await _context.Alunos.AddAsync(aluno);
            await _context.SaveChangesAsync();
        }
        #endregion

        #region Read
        public async Task<Aluno> GetAlunoByIdAsync(int id)
        {
            var aluno = await _context.Alunos
                .FirstOrDefaultAsync(a => a.Id == id);
            return aluno;
        }

        public async Task<List<Aluno>> GetAllAlunosAsync()
        {
            return await _context.Alunos.ToListAsync();
        }
        #endregion

        #region Update
        public async Task UpdateAlunoAsync(Aluno aluno)
        {
            if (aluno == null)
                throw new ArgumentNullException(nameof(aluno));

            var alunoExistente = await _context.Alunos.FindAsync(aluno.Id);
            if (alunoExistente == null)
                throw new KeyNotFoundException("Aluno não encontrado.");

            alunoExistente.Nome = aluno.Nome;
            alunoExistente.Sobrenome = aluno.Sobrenome;
            alunoExistente.Nascimento = aluno.Nascimento;
            alunoExistente.Sexo = aluno.Sexo;
            alunoExistente.Email = aluno.Email;
            alunoExistente.Telefone = aluno.Telefone;
            alunoExistente.Cep = aluno.Cep;
            alunoExistente.Logradouro = aluno.Logradouro;
            alunoExistente.Complemento = aluno.Complemento;
            alunoExistente.Bairro = aluno.Bairro;
            alunoExistente.Localidade = aluno.Localidade;
            alunoExistente.UF = aluno.UF;
            alunoExistente.DataDeAtualizacao = DateTime.Now;
            alunoExistente.Ativo = aluno.Ativo;

            _context.Alunos.Update(alunoExistente);
            await _context.SaveChangesAsync();
        }
        #endregion

        #region Delete
        public async Task DeleteAlunoAsync(int id)
        {
            var aluno = await _context.Alunos.FindAsync(id);
            if (aluno == null)
                throw new KeyNotFoundException("Aluno não encontrado.");

            _context.Alunos.Remove(aluno);
            await _context.SaveChangesAsync();
        }
        #endregion
    }
}
