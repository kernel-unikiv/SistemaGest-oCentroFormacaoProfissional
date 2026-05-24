using SistemaGestãoCentroFormacaoProfissional.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaGestãoCentroFormacaoProfissional.Interfaces
{
    public interface ICertificavel
    {
        double CalcularClassificacaoFinal();

        bool EstaAprovado();

        bool PodeCertificar();
        Certificado EmitirCertificado();
    }
}
