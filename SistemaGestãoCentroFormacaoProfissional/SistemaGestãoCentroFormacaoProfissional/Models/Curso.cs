using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using static SistemaGestãoCentroFormacaoProfissional.Models.Enumeracoes;

namespace SistemaGestãoCentroFormacaoProfissional.Models
{
    public class Curso
    {
        private readonly string _codigo;
        private string _nome;
        private string _descricao;
        private int _duracaoHoras;
        private Modalidade _modalidade;
        private readonly List<Modulo> _modulos;  
        private int _idadeMinima;
        private string _escolaridadeMinima;
        private double _notaMinAprovacao;
        private readonly DateTime _dataInicio;
        private readonly DateTime _dataFim;
        private bool _activo;

        public string Codigo => _codigo;
        public string Nome => _nome;
        public string Descricao => _descricao;
        public int DuracaoHoras => _duracaoHoras;
        public Modalidade Modalidade => _modalidade;
        public int IdadeMinima => _idadeMinima;
        public string EscolaridadeMinima => _escolaridadeMinima;
        public double NotaMinAprovacao => _notaMinAprovacao;
        public DateTime DataInicio => _dataInicio;
        public DateTime DataFim => _dataFim;
        public bool Activo => _activo;
        public IReadOnlyList<Modulo> Modulos => _modulos.AsReadOnly();

        public Curso(string codigo, string nome, string descricao,
                     int duracaoHoras, Modalidade modalidade,
                     int idadeMinima, string escolaridadeMinima,
                     double notaMinAprovacao,
                     DateTime dataInicio, DateTime dataFim)
        {
            _codigo = codigo;
            _nome = nome;
            _descricao = descricao;
            _duracaoHoras = duracaoHoras;
            _modalidade = modalidade;
            _idadeMinima = idadeMinima;
            _escolaridadeMinima = escolaridadeMinima;
            _notaMinAprovacao = notaMinAprovacao;
            _dataInicio = dataInicio;
            _dataFim = dataFim;
            _activo = true;
            _modulos = new List<Modulo>();
        }

        public void AdicionarModulo(Modulo modulo)
        {
            if (_modulos.Any(m => m.Id == modulo.Id))
                return; 
            _modulos.Add(modulo);
        }

        public bool RemoverModulo(int moduloId)
        {
            var modulo = _modulos.FirstOrDefault(m => m.Id == moduloId);
            if (modulo == null) return false;
            _modulos.Remove(modulo);
            return true;
        }


        public bool FormandoCumpreRequisitos(Formando formando)
        {
            if (formando.CalcularIdade() < _idadeMinima)
                return false;

            if (!CumpreEscolaridade(formando.NivelEscolaridade, _escolaridadeMinima))
                return false;

            return true;
        }

        private static bool CumpreEscolaridade(string nivelFormando, string nivelMinimo)
        {
            var ordem = new List<string>
            {
                "6ª classe", "9ª classe", "12ª classe",
                "licenciatura", "mestrado", "doutoramento"
            };

            int indexFormando = ordem.FindIndex(n =>
                n.Equals(nivelFormando.Trim(), StringComparison.OrdinalIgnoreCase));
            int indexMinimo = ordem.FindIndex(n =>
                n.Equals(nivelMinimo.Trim(), StringComparison.OrdinalIgnoreCase));

            if (indexFormando < 0 || indexMinimo < 0) return true;
            return indexFormando >= indexMinimo;
        }

        public void Desactivar() => _activo = false;
        public void Activar() => _activo = true;

        public int TotalModulos => _modulos.Count;

        public override string ToString()
        {
            return $"[{_codigo}] {_nome} | {_modalidade} | " +
                   $"{_duracaoHoras}h | {TotalModulos} módulo(s) | " +
                   $"{(_activo ? "Activo" : "Inactivo")}";
        }
    }
}
