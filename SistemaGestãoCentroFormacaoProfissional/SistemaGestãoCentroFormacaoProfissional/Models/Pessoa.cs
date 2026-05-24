using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaGestãoCentroFormacaoProfissional.Models
{
    public abstract class Pessoa
    {
        private readonly string _nome;
        private readonly string _numeroBi;
        private string _email;
        private string _telefone;
        private readonly DateTime _dataNascimento;

        public string Nome => _nome;
        public string NumeroBi => _numeroBi;
        public DateTime DataNascimento => _dataNascimento;

        public string Email
        {
            get => _email;
            set => _email = value;
        }

        public string Telefone
        {
            get => _telefone;
            set => _telefone = value;
        }

        protected Pessoa(string nome, string numeroBi, string email,
                         string telefone, DateTime dataNascimento)
        {
            _nome = nome;
            _numeroBi = numeroBi;
            _email = email;
            _telefone = telefone;
            _dataNascimento = dataNascimento;
        }


        public abstract string ObterDetalhes();

        public int CalcularIdade()
        {
            DateTime hoje = DateTime.Today;
            int idade = hoje.Year - _dataNascimento.Year;
            if (_dataNascimento.Date > hoje.AddYears(-idade)) idade--;
            return idade;
        }

        public override string ToString() => $"[{_numeroBi}] {_nome}";
    }
}

