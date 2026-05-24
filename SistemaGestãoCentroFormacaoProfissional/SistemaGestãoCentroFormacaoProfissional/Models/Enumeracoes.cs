using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaGestãoCentroFormacaoProfissional.Models
{
    public class Enumeracoes
    {
        public enum Modalidade
        {
            Presencial,
            Online,
            Misto
        }

        public enum EstadoInscricao
        {
            Activa,
            Concluida,
            Cancelada
        }

        public enum TipoAvaliacao
        {
            Teorica,
            Pratica,
            Oral,
            Projecto
        }

        public enum NivelHabilitacao
        {
            CursoTecnico,
            Licenciatura,
            Mestrado,
            Doutoramento
        }
    }
}
