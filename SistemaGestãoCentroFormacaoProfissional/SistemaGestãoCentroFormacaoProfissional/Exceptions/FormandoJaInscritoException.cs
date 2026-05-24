using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaGestãoCentroFormacaoProfissional.Exceptions
{
    public class FormandoJaInscritoException: Exception
    {
        public string CodigoFormando { get; }
        public string CodigoCurso { get; }

        public FormandoJaInscritoException(string codigoFormando, string codigoCurso)
            : base($"O formando '{codigoFormando}' já se encontra inscrito no curso '{codigoCurso}'.")
        {
            CodigoFormando = codigoFormando;
            CodigoCurso = codigoCurso;
        }

        public FormandoJaInscritoException(string mensagem) : base(mensagem)
        {
            CodigoFormando = string.Empty;
            CodigoCurso = string.Empty;
        }
    }
}
