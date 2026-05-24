using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaGestãoCentroFormacaoProfissional.Exceptions
{
    public class CursoNaoEncontradoException:Exception
    {
        public string CodigoCurso { get; }

        public CursoNaoEncontradoException(string codigoCurso)
            : base($"O curso com código '{codigoCurso}' não foi encontrado no sistema.")
        {
            CodigoCurso = codigoCurso;
        }
    }
}
