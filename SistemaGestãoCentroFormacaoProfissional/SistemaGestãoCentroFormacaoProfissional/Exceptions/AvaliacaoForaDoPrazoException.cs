using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaGestãoCentroFormacaoProfissional.Exceptions
{
    public class AvaliacaoForaDoPrazoException: Exception
    {
        public int ModuloId { get; }
        public DateTime DataTentativa { get; }
        public DateTime DataFimModulo { get; }

        public AvaliacaoForaDoPrazoException(int moduloId, DateTime dataFimModulo)
            : base($"Não é possível lançar avaliações no módulo {moduloId}: " +
                   $"o prazo encerrou em {dataFimModulo:dd/MM/yyyy}.")
        {
            ModuloId = moduloId;
            DataTentativa = DateTime.Now;
            DataFimModulo = dataFimModulo;
        }

        public AvaliacaoForaDoPrazoException(string mensagem) : base(mensagem)
        {
            ModuloId = 0;
            DataTentativa = DateTime.Now;
            DataFimModulo = DateTime.MinValue;
        }
    }
}
