using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaGestãoCentroFormacaoProfissional.Models
{
    public class Modulo
    {
        private static int _contadorId = 1;

        private readonly int _id;
        private string _nome;
        private string _descricao;
        private int _cargaHoraria;
        private Formador _formador;
        private readonly List<Avaliacao> _avaliacoes; 
        private readonly DateTime _dataInicio;
        private readonly DateTime _dataFim;

        public int Id => _id;
        public string Nome => _nome;
        public string Descricao => _descricao;
        public int CargaHoraria => _cargaHoraria;
        public Formador Formador => _formador;
        public DateTime DataInicio => _dataInicio;
        public DateTime DataFim => _dataFim;
        public IReadOnlyList<Avaliacao> Avaliacoes => _avaliacoes.AsReadOnly();

        public Modulo(string nome, string descricao, int cargaHoraria,
                      Formador formador, DateTime dataInicio, DateTime dataFim)
        {
            _id = _contadorId++;
            _nome = nome;
            _descricao = descricao;
            _cargaHoraria = cargaHoraria;
            _formador = formador;
            _dataInicio = dataInicio;
            _dataFim = dataFim;
            _avaliacoes = new List<Avaliacao>();
        }

        public void AdicionarAvaliacao(Avaliacao avaliacao)
        {
            _avaliacoes.Add(avaliacao);
        }

        public bool EstaActivo()
        {
            DateTime hoje = DateTime.Today;
            return hoje >= _dataInicio && hoje <= _dataFim;
        }

        public bool DentroDoPrazo(DateTime data)
        {
            return data >= _dataInicio && data <= _dataFim;
        }

        public double? ObterMediaFormando(string codigoFormando)
        {
            var notas = _avaliacoes
                .Where(a => a.CodigoFormando == codigoFormando)
                .Select(a => a.Nota)
                .ToList();

            return notas.Any() ? notas.Average() : (double?)null;
        }

        public void ActualizarFormador(Formador novoFormador)
        {
            _formador = novoFormador;
        }

        public override string ToString()
        {
            return $"[ID:{_id}] {_nome} | {_cargaHoraria}h | " +
                   $"Formador: {_formador.Nome} | " +
                   $"{_dataInicio:dd/MM/yyyy} → {_dataFim:dd/MM/yyyy}";
        }
    }
}
