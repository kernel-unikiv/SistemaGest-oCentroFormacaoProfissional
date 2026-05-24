using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaGestãoCentroFormacaoProfissional.Models
{
    public class Presenca
    {
        private static int _contadorId = 1;

        private readonly int _id;
        private readonly string _codigoFormando;
        private readonly int _moduloId;
        private readonly DateTime _data;
        private readonly bool _presente;
        private readonly string? _justificacaoFalta;

        public int Id => _id;
        public string CodigoFormando => _codigoFormando;
        public int ModuloId => _moduloId;
        public DateTime Data => _data;
        public bool Presente => _presente;
        public string? JustificacaoFalta => _justificacaoFalta;

        public Presenca(string codigoFormando, int moduloId,
                        bool presente, string? justificacaoFalta = null)
        {
            _id = _contadorId++;
            _codigoFormando = codigoFormando;
            _moduloId = moduloId;
            _data = DateTime.Today;
            _presente = presente;
            _justificacaoFalta = justificacaoFalta;
        }

        public override string ToString()
        {
            string estado = _presente ? "Presente" : "Ausente";
            string just = (!_presente && !string.IsNullOrWhiteSpace(_justificacaoFalta))
                            ? $" (Justificação: {_justificacaoFalta})"
                            : string.Empty;
            return $"[{_data:dd/MM/yyyy}] Formando: {_codigoFormando} | " +
                   $"Módulo: {_moduloId} | {estado}{just}";
        }
    }
}
