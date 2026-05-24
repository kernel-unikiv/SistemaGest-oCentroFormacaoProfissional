using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaGestãoCentroFormacaoProfissional.Exceptions
{
    public class FormadorNaoEncontradoException:Exception
    {
        public string CodigoFormador { get; }

        public FormadorNaoEncontradoException(string codigoFormador)
            : base($"O formador com código '{codigoFormador}' não foi encontrado no sistema.")
        {
            CodigoFormador = codigoFormador;
        }
    }
}
