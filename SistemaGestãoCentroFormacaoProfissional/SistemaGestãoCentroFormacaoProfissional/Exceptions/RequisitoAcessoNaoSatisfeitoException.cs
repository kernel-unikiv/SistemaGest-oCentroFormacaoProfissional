using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaGestãoCentroFormacaoProfissional.Exceptions
{
    public class RequisitoAcessoNaoSatisfeitoException:Exception
    {
        public string CodigoFormando { get; }
        public string CodigoCurso { get; }
        public string RequisitoPendente { get; }

        public RequisitoAcessoNaoSatisfeitoException(
            string codigoFormando, string codigoCurso, string requisitoPendente)
            : base($"O formando '{codigoFormando}' não cumpre o requisito " +
                   $"'{requisitoPendente}' exigido pelo curso '{codigoCurso}'.")
        {
            CodigoFormando = codigoFormando;
            CodigoCurso = codigoCurso;
            RequisitoPendente = requisitoPendente;
        }

        public RequisitoAcessoNaoSatisfeitoException(string mensagem) : base(mensagem)
        {
            CodigoFormando = string.Empty;
            CodigoCurso = string.Empty;
            RequisitoPendente = string.Empty;
        }
    }
}
