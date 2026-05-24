using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaGestãoCentroFormacaoProfissional.Models
{
    public class Formando:Pessoa
    {
        private readonly string _codigoFormando;
        private string _nivelEscolaridade;
        private readonly DateTime _dataRegisto;

        public string CodigoFormando => _codigoFormando;
        public string NivelEscolaridade
        {
            get => _nivelEscolaridade;
            set => _nivelEscolaridade = value;
        }
        public DateTime DataRegisto => _dataRegisto;

        public Formando(string nome, string numeroBi, string email, string telefone,
                        DateTime dataNascimento, string codigoFormando,
                        string nivelEscolaridade)
            : base(nome, numeroBi, email, telefone, dataNascimento)
        {
            _codigoFormando = codigoFormando;
            _nivelEscolaridade = nivelEscolaridade;
            _dataRegisto = DateTime.Now;
        }

        public override string ObterDetalhes()
        {
            return $"Formando      : {Nome}\n" +
                   $"BI            : {NumeroBi}\n" +
                   $"Código        : {_codigoFormando}\n" +
                   $"Escolaridade  : {_nivelEscolaridade}\n" +
                   $"Idade         : {CalcularIdade()} anos\n" +
                   $"Email         : {Email}\n" +
                   $"Telefone      : {Telefone}\n" +
                   $"Data Registo  : {_dataRegisto:dd/MM/yyyy}";
        }
    }
}
