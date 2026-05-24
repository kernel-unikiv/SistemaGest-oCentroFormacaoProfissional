using System;
using System.Collections.Generic;
using System.Text;
using static SistemaGestãoCentroFormacaoProfissional.Models.Enumeracoes;

namespace SistemaGestãoCentroFormacaoProfissional.Models
{
    public class Formador:Pessoa
    {
        private readonly string _codigoFormador;
        private NivelHabilitacao _habilitacao;
        private readonly List<string> _areasCompetencia;
        private readonly string _numeroRegistoProfissional;

        public string CodigoFormador => _codigoFormador;
        public NivelHabilitacao Habilitacao => _habilitacao;
        public string NumeroRegistoProfissional => _numeroRegistoProfissional;
        public IReadOnlyList<string> AreasCompetencia => _areasCompetencia.AsReadOnly();

        public Formador(string nome, string numeroBi, string email, string telefone,
                        DateTime dataNascimento, string codigoFormador,
                        NivelHabilitacao habilitacao, string numeroRegistoProfissional)
            : base(nome, numeroBi, email, telefone, dataNascimento)
        {
            _codigoFormador = codigoFormador;
            _habilitacao = habilitacao;
            _numeroRegistoProfissional = numeroRegistoProfissional;
            _areasCompetencia = new List<string>();
        }

        public void AdicionarAreaCompetencia(string area)
        {
            if (!string.IsNullOrWhiteSpace(area) &&
                !_areasCompetencia.Contains(area, StringComparer.OrdinalIgnoreCase))
            {
                _areasCompetencia.Add(area.Trim());
            }
        }

        public bool TemCompetenciaEm(string area)
        {
            return _areasCompetencia.Any(a =>
                a.Equals(area, StringComparison.OrdinalIgnoreCase));
        }

        public void ActualizarHabilitacao(NivelHabilitacao novaHabilitacao)
        {
            _habilitacao = novaHabilitacao;
        }

        public override string ObterDetalhes()
        {
            string areas = _areasCompetencia.Any()
                ? string.Join(", ", _areasCompetencia)
                : "Nenhuma área registada";

            return $"Formador     : {Nome}\n" +
                   $"BI           : {NumeroBi}\n" +
                   $"Código       : {_codigoFormador}\n" +
                   $"Habilitação  : {_habilitacao}\n" +
                   $"Registo Prof.: {_numeroRegistoProfissional}\n" +
                   $"Email        : {Email}\n" +
                   $"Telefone     : {Telefone}\n" +
                   $"Áreas        : {areas}";
        }
    }
}
