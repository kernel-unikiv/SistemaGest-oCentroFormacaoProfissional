using System;
using System.Collections.Generic;
using System.Text;
using static SistemaGestãoCentroFormacaoProfissional.Models.Enumeracoes;

namespace SistemaGestãoCentroFormacaoProfissional.Models
{
    public class Avaliacao
    {
        private static int _contadorId = 1;

        private readonly int _id;
        private readonly string _codigoFormando;
        private readonly double _nota;
        private readonly TipoAvaliacao _tipo;
        private readonly DateTime _dataAvaliacao;
        private readonly string _observacoes;

        public int Id => _id;
        public string CodigoFormando => _codigoFormando;
        public double Nota => _nota;
        public TipoAvaliacao Tipo => _tipo;
        public DateTime DataAvaliacao => _dataAvaliacao;
        public string Observacoes => _observacoes;

        public Avaliacao(string codigoFormando, double nota,
                         TipoAvaliacao tipo, string observacoes = "")
        {
            _id = _contadorId++;
            _codigoFormando = codigoFormando;
            _nota = nota;
            _tipo = tipo;
            _dataAvaliacao = DateTime.Now;
            _observacoes = observacoes;
        }

        public string ObterClassificacaoQualitativa()
        {
            return _nota switch
            {
                >= 18 => "Muito Bom",
                >= 14 => "Bom",
                >= 10 => "Suficiente",
                >= 5 => "Medíocre",
                _ => "Insuficiente"
            };
        }

        public override string ToString()
        {
            return $"  [ID:{_id}] [{_tipo}] Formando: {_codigoFormando} | " +
                   $"Nota: {_nota:F1} ({ObterClassificacaoQualitativa()}) | " +
                   $"Data: {_dataAvaliacao:dd/MM/yyyy}";
        }
    }
}
