using SistemaGestãoCentroFormacaoProfissional.Exceptions;
using SistemaGestãoCentroFormacaoProfissional.Models;
using System;
using System.Collections.Generic;
using System.Text;
using static SistemaGestãoCentroFormacaoProfissional.Models.Enumeracoes;

namespace SistemaGestãoCentroFormacaoProfissional.Services
{
    public class GestorFormacao
    {
        private readonly List<Formador> _formadores;
        private readonly List<Formando> _formandos;
        private readonly List<Curso> _cursos;
        private readonly List<Inscricao> _inscricoes;
        private readonly List<Presenca> _presencas;

        public IReadOnlyList<Formador> Formadores => _formadores.AsReadOnly();
        public IReadOnlyList<Formando> Formandos => _formandos.AsReadOnly();
        public IReadOnlyList<Curso> Cursos => _cursos.AsReadOnly();
        public IReadOnlyList<Inscricao> Inscricoes => _inscricoes.AsReadOnly();
        public IReadOnlyList<Presenca> Presencas => _presencas.AsReadOnly();

        public GestorFormacao()
        {
            _formadores = new List<Formador>();
            _formandos = new List<Formando>();
            _cursos = new List<Curso>();
            _inscricoes = new List<Inscricao>();
            _presencas = new List<Presenca>();
        }


        public void RegistarFormador(Formador formador)
        {
            if (_formadores.Any(f =>
                f.CodigoFormador.Equals(formador.CodigoFormador,
                                        StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException(
                    $"Já existe um formador com o código '{formador.CodigoFormador}'.");

            _formadores.Add(formador);
        }

        public Formador ObterFormador(string codigo)
        {
            return _formadores.FirstOrDefault(f =>
                f.CodigoFormador.Equals(codigo, StringComparison.OrdinalIgnoreCase))
                ?? throw new FormadorNaoEncontradoException(codigo);
        }

        public List<Formador> ListarFormadores() => new List<Formador>(_formadores);


        public void RegistarFormando(Formando formando)
        {
            if (_formandos.Any(f =>
                f.CodigoFormando.Equals(formando.CodigoFormando,
                                        StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException(
                    $"Já existe um formando com o código '{formando.CodigoFormando}'.");

            if (_formandos.Any(f =>
                f.NumeroBi.Equals(formando.NumeroBi, StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException(
                    $"Já existe um formando com o BI '{formando.NumeroBi}'.");

            _formandos.Add(formando);
        }

        public Formando ObterFormando(string codigo)
        {
            return _formandos.FirstOrDefault(f =>
                f.CodigoFormando.Equals(codigo, StringComparison.OrdinalIgnoreCase))
                ?? throw new FormandoNaoEncontradoException(codigo);
        }

        public List<Formando> ListarFormandos() => new List<Formando>(_formandos);


        public void CriarCurso(Curso curso)
        {
            if (_cursos.Any(c =>
                c.Codigo.Equals(curso.Codigo, StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException(
                    $"Já existe um curso com o código '{curso.Codigo}'.");

            _cursos.Add(curso);
        }

        public Curso ObterCurso(string codigo)
        {
            return _cursos.FirstOrDefault(c =>
                c.Codigo.Equals(codigo, StringComparison.OrdinalIgnoreCase))
                ?? throw new CursoNaoEncontradoException(codigo);
        }

        public List<Curso> ListarCursos() => new List<Curso>(_cursos);

        public void AdicionarModuloACurso(string codigoCurso, Modulo modulo)
        {
            Curso curso = ObterCurso(codigoCurso);
            curso.AdicionarModulo(modulo);
        }

        public Inscricao InscreverFormando(string codigoFormando, string codigoCurso)
        {
            Formando formando = ObterFormando(codigoFormando);
            Curso curso = ObterCurso(codigoCurso);

            if (!curso.Activo)
                throw new InvalidOperationException(
                    $"O curso '{curso.Nome}' não se encontra activo.");

            if (!curso.FormandoCumpreRequisitos(formando))
            {
                string motivo = formando.CalcularIdade() < curso.IdadeMinima
                    ? $"Idade mínima exigida: {curso.IdadeMinima} anos " +
                      $"(formando tem {formando.CalcularIdade()} anos)"
                    : $"Escolaridade mínima exigida: {curso.EscolaridadeMinima}";

                throw new RequisitoAcessoNaoSatisfeitoException(
                    codigoFormando, codigoCurso, motivo);
            }

            bool jaInscrito = _inscricoes.Any(i =>
                i.Formando.CodigoFormando == codigoFormando &&
                i.Curso.Codigo == codigoCurso &&
                i.Estado == EstadoInscricao.Activa);

            if (jaInscrito)
                throw new FormandoJaInscritoException(codigoFormando, codigoCurso);

            Inscricao inscricao = new Inscricao(formando, curso);
            _inscricoes.Add(inscricao);
            return inscricao;
        }

        public Inscricao ObterInscricao(int id)
        {
            return _inscricoes.FirstOrDefault(i => i.Id == id)
                ?? throw new InscricaoNaoEncontradaException(id);
        }

        public void ConcluirInscricao(int idInscricao)
        {
            Inscricao inscricao = ObterInscricao(idInscricao);
            inscricao.Concluir();
        }

        public void CancelarInscricao(int idInscricao)
        {
            Inscricao inscricao = ObterInscricao(idInscricao);
            inscricao.Cancelar();
        }

        public List<Inscricao> ListarInscricoes() => new List<Inscricao>(_inscricoes);

        public List<Inscricao> ListarInscricoesFormando(string codigoFormando)
        {
            return _inscricoes
                .Where(i => i.Formando.CodigoFormando == codigoFormando)
                .ToList();
        }

        public void LancarAvaliacao(string codigoCurso, int moduloId,
                                    string codigoFormando, double nota,
                                    TipoAvaliacao tipo, string observacoes = "")
        {
            Curso curso = ObterCurso(codigoCurso);

            Modulo? modulo = curso.Modulos.FirstOrDefault(m => m.Id == moduloId);
            if (modulo == null)
                throw new InvalidOperationException(
                    $"O módulo com ID {moduloId} não pertence ao curso '{codigoCurso}'.");

            if (!modulo.DentroDoPrazo(DateTime.Today))
                throw new AvaliacaoForaDoPrazoException(moduloId, modulo.DataFim);

            bool inscritoActivo = _inscricoes.Any(i =>
                i.Formando.CodigoFormando == codigoFormando &&
                i.Curso.Codigo == codigoCurso &&
                i.Estado == EstadoInscricao.Activa);

            if (!inscritoActivo)
                throw new InvalidOperationException(
                    $"O formando '{codigoFormando}' não possui inscrição activa no curso '{codigoCurso}'.");

            if (nota < 0.0 || nota > 20.0)
                throw new ArgumentOutOfRangeException(nameof(nota),
                    "A nota deve ser um valor entre 0 e 20.");

            Avaliacao avaliacao = new Avaliacao(codigoFormando, nota, tipo, observacoes);
            modulo.AdicionarAvaliacao(avaliacao);
        }

        public void RegistarPresenca(string codigoFormando, int moduloId, bool presente,
                                     string? justificacao = null)
        {
            _ = ObterFormando(codigoFormando);

            Presenca presenca = new Presenca(codigoFormando, moduloId, presente, justificacao);
            _presencas.Add(presenca);
        }

        public List<Presenca> ListarPresencasPorModulo(int moduloId)
        {
            return _presencas.Where(p => p.ModuloId == moduloId).ToList();
        }

        public List<Presenca> ListarPresencasPorFormando(string codigoFormando)
        {
            return _presencas.Where(p => p.CodigoFormando == codigoFormando).ToList();
        }


        public Certificado EmitirCertificado(int idInscricao)
        {
            Inscricao inscricao = ObterInscricao(idInscricao);
            return inscricao.EmitirCertificado();
        }

        public List<Certificado> ListarTodosCertificados()
        {
            return _inscricoes
                .Where(i => i.Certificado != null)
                .Select(i => i.Certificado!)
                .ToList();
        }

       
        public List<Inscricao> ListarAprovadosPorCurso(string codigoCurso)
        {
            return _inscricoes
                .Where(i => i.Curso.Codigo == codigoCurso && i.EstaAprovado())
                .OrderByDescending(i => i.CalcularClassificacaoFinal())
                .ToList();
        }

        public (double Classificacao, bool Aprovado) ObterResultadoFormando(int idInscricao)
        {
            Inscricao inscricao = ObterInscricao(idInscricao);
            double cf = inscricao.CalcularClassificacaoFinal();
            return (cf, inscricao.EstaAprovado());
        }

        public Modulo? LocalizarModulo(int moduloId)
        {
            foreach (Curso curso in _cursos)
            {
                Modulo? mod = curso.Modulos.FirstOrDefault(m => m.Id == moduloId);
                if (mod != null) return mod;
            }
            return null;
        }

   
        public int ContarInscritosActivosCurso(string codigoCurso)
        {
            return _inscricoes.Count(i =>
                i.Curso.Codigo == codigoCurso &&
                i.Estado == EstadoInscricao.Activa);
        }
    }
}
