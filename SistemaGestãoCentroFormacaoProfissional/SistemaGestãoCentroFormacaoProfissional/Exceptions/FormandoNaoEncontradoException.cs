using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaGestãoCentroFormacaoProfissional.Exceptions
{
    public class FormandoNaoEncontradoException:Exception
    {
        public string CodigoFormando { get; }

        public FormandoNaoEncontradoException(string codigoFormando)
            : base($"O formando com código '{codigoFormando}' não foi encontrado no sistema.")
        {
            CodigoFormando = codigoFormando;
        }
    }
}
