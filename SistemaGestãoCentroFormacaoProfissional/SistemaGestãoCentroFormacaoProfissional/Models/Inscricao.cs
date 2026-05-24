using SistemaGestãoCentroFormacaoProfissional.Interfaces;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using static SistemaGestãoCentroFormacaoProfissional.Models.Enumeracoes;

namespace SistemaGestãoCentroFormacaoProfissional.Models
{
    public class Inscricao: ICertificavel
    {
        private static int _contadorId = 1;

        private readonly int _id;
        private readonly Formando _formando;
        private readonly Curso _curso;
        private readonly DateTime _dataInscricao;
        private EstadoInscricao _estado;
        private Certificado? _certificado;  

        public int Id => _id;
        public Formando Formando => _formando;
        public Curso Curso => _curso;
        public DateTime DataInscricao => _dataInscricao;
        public EstadoInscricao Estado => _estado;
        public Certificado? Certificado => _certificado;

        public Inscricao(Formando formando, Curso curso)
        {
            _id = _contadorId++;
            _formando = formando;
            _curso = curso;
            _dataInscricao = DateTime.Now;
            _estado = EstadoInscricao.Activa;
            _certificado = null;
        }

        public double CalcularClassificacaoFinal()
        {
            var mediasModulos = new List<double>();

            foreach (Modulo modulo in _curso.Modulos)
            {
                double? media = modulo.ObterMediaFormando(_formando.CodigoFormando);
                if (media.HasValue)
                    mediasModulos.Add(media.Value);
            }

            return mediasModulos.Any()
                ? Math.Round(mediasModulos.Average(), 1)
                : 0.0;
        }

        public bool EstaAprovado()
        {
            return CalcularClassificacaoFinal() >= _curso.NotaMinAprovacao;
        }

        public bool PodeCertificar()
        {
            return EstaAprovado()
                && _estado == EstadoInscricao.Concluida
                && _certificado == null;
        }


        public Certificado EmitirCertificado()
        {
            if (_certificado != null)
                return _certificado;

            if (!EstaAprovado())
                throw new InvalidOperationException(
                    $"O formando '{_formando.Nome}' não está aprovado no curso '{_curso.Nome}'.");

            if (_estado != EstadoInscricao.Concluida)
                throw new InvalidOperationException(
                    "A inscrição deve estar no estado 'Concluida' para emitir certificado.");

            _certificado = new Certificado(
                _formando.CodigoFormando,
                _formando.Nome,
                _curso.Codigo,
                _curso.Nome,
                CalcularClassificacaoFinal()
            );
            return _certificado;
        }

        public void Concluir()
        {
            if (_estado == EstadoInscricao.Cancelada)
                throw new InvalidOperationException(
                    "Não é possível concluir uma inscrição cancelada.");
            _estado = EstadoInscricao.Concluida;
        }

        public void Cancelar()
        {
            if (_estado == EstadoInscricao.Concluida)
                throw new InvalidOperationException(
                    "Não é possível cancelar uma inscrição já concluída.");
            _estado = EstadoInscricao.Cancelada;
        }

        public override string ToString()
        {
            return $"Inscrição #{_id:D3} | {_formando.Nome} → {_curso.Nome} | " +
                   $"Estado: {_estado} | Data: {_dataInscricao:dd/MM/yyyy}";
        }
    }
}
