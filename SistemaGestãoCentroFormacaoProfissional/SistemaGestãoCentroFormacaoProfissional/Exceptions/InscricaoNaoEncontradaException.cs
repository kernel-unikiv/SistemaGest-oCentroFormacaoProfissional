using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaGestãoCentroFormacaoProfissional.Exceptions
{
    public class InscricaoNaoEncontradaException:Exception
    {
        public int IdInscricao { get; }

        public InscricaoNaoEncontradaException(int idInscricao)
            : base($"A inscrição com ID {idInscricao} não foi encontrada no sistema.")
        {
            IdInscricao = idInscricao;
        }
    }
}
