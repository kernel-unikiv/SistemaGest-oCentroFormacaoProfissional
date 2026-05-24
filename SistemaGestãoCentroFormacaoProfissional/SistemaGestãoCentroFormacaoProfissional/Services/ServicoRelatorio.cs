using SistemaGestãoCentroFormacaoProfissional.Models;
using System;
using System.Collections.Generic;
using System.Text;
using static SistemaGestãoCentroFormacaoProfissional.Models.Enumeracoes;

namespace SistemaGestãoCentroFormacaoProfissional.Services
{
    public class ServicoRelatorio
    {
        private static void Cabecalho(string titulo)
        {
            string linha = new string('─', 64);
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"  ┌{linha}┐");
            string tituloFormatado = titulo.PadLeft((64 + titulo.Length) / 2).PadRight(64);
            Console.WriteLine($"  │ {tituloFormatado}│");
            Console.WriteLine($"  └{linha}┘");
            Console.ResetColor();
        }

        private static void Rodape()
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($"\n  Relatório gerado em: {DateTime.Now:dd/MM/yyyy HH:mm:ss}");
            Console.ResetColor();
        }

        public void RelatorioAprovadosPorCurso(GestorFormacao gestor, string codigoCurso)
        {
            Curso curso;
            try { curso = gestor.ObterCurso(codigoCurso); }
            catch
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("  Curso não encontrado.");
                Console.ResetColor(); return;
            }

            var aprovados = gestor.ListarAprovadosPorCurso(codigoCurso);

            Cabecalho($"FORMANDOS APROVADOS  |  {curso.Nome}");

            if (!aprovados.Any())
            {
                Console.WriteLine("  Nenhum formando aprovado registado para este curso.");
                Rodape(); return;
            }

            Console.WriteLine($"  {"#",-4} {"Nome",-28} {"Código",-12} " +
                              $"{"Classif.",-10} {"Menção"}");
            Console.WriteLine("  " + new string('-', 62));

            int pos = 1;
            foreach (Inscricao ins in aprovados)
            {
                double cf = ins.CalcularClassificacaoFinal();
                string mencao = ObterMencao(cf);
                Console.Write($"  {pos,-4} {ins.Formando.Nome,-28} " +
                              $"{ins.Formando.CodigoFormando,-12} ");
                Console.ForegroundColor = cf >= 14 ? ConsoleColor.Green : ConsoleColor.Yellow;
                Console.WriteLine($"{cf,-10:F1} {mencao}");
                Console.ResetColor();
                pos++;
            }

            Console.WriteLine($"\n  Total aprovados: {aprovados.Count} de " +
                              $"{gestor.ContarInscritosActivosCurso(codigoCurso) + aprovados.Count} inscritos");
            Rodape();
        }

        public void RelatorioSumarioGeral(GestorFormacao gestor)
        {
            Cabecalho("SUMÁRIO GERAL DO SISTEMA");

            Console.WriteLine($"  Formadores registados  : {gestor.Formadores.Count}");
            Console.WriteLine($"  Formandos registados   : {gestor.Formandos.Count}");
            Console.WriteLine($"  Cursos no sistema      : {gestor.Cursos.Count}");
            Console.WriteLine($"  Total de inscrições    : {gestor.Inscricoes.Count}");
            Console.WriteLine($"  Certificados emitidos  : {gestor.ListarTodosCertificados().Count}");

            Console.WriteLine("\n  Inscrições por estado:");
            foreach (EstadoInscricao estado in Enum.GetValues<EstadoInscricao>())
            {
                int total = gestor.Inscricoes.Count(i => i.Estado == estado);
                Console.WriteLine($"    • {estado,-12}: {total}");
            }

            Console.WriteLine("\n  Cursos activos:");
            foreach (Curso c in gestor.Cursos.Where(c => c.Activo))
            {
                int inscritos = gestor.ContarInscritosActivosCurso(c.Codigo);
                Console.WriteLine($"    [{c.Codigo}] {c.Nome,-30} – {inscritos} inscrito(s) activo(s)");
            }
            Rodape();
        }

        public void RelatorioFormando(GestorFormacao gestor, string codigoFormando)
        {
            Formando formando;
            try { formando = gestor.ObterFormando(codigoFormando); }
            catch
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("  Formando não encontrado.");
                Console.ResetColor(); return;
            }

            Cabecalho($"RELATÓRIO DE DESEMPENHO  |  {formando.Nome}");
            Console.WriteLine($"  {formando.ObterDetalhes()}");

            var inscricoes = gestor.ListarInscricoesFormando(codigoFormando);
            if (!inscricoes.Any())
            {
                Console.WriteLine("\n  Sem inscrições registadas.");
                Rodape(); return;
            }

            Console.WriteLine($"\n  Inscrições ({inscricoes.Count}):");
            foreach (Inscricao ins in inscricoes)
            {
                double cf = ins.CalcularClassificacaoFinal();
                bool aprovado = ins.EstaAprovado();
                Console.Write($"    [{ins.Id}] {ins.Curso.Nome,-30} | " +
                              $"Estado: {ins.Estado,-10} | Classif: ");
                Console.ForegroundColor = aprovado ? ConsoleColor.Green : ConsoleColor.Red;
                Console.WriteLine($"{cf:F1}  {(aprovado ? "✔ Aprovado" : "✘ Reprovado")}");
                Console.ResetColor();

                // Detalhe por módulo
                foreach (Modulo mod in ins.Curso.Modulos)
                {
                    double? media = mod.ObterMediaFormando(codigoFormando);
                    string mediaStr = media.HasValue ? $"{media.Value:F1}" : "sem nota";
                    Console.WriteLine($"       ↳ Módulo: {mod.Nome,-28} | Média: {mediaStr}");
                }
            }
            Rodape();
        }

      
        public void RelatorioAvaliacoesPorModulo(GestorFormacao gestor,
                                                  string codigoCurso, int moduloId)
        {
            Curso curso;
            try { curso = gestor.ObterCurso(codigoCurso); }
            catch { Console.WriteLine("  Curso não encontrado."); return; }

            Modulo? modulo = curso.Modulos.FirstOrDefault(m => m.Id == moduloId);
            if (modulo == null)
            {
                Console.WriteLine("  Módulo não encontrado no curso.");
                return;
            }

            Cabecalho($"AVALIAÇÕES  |  {modulo.Nome}");
            if (!modulo.Avaliacoes.Any())
            {
                Console.WriteLine("  Nenhuma avaliação registada neste módulo.");
                Rodape(); return;
            }

            Console.WriteLine($"  {"Formando",-14} {"Tipo",-12} {"Nota",-8} {"Menção",-22} Data");
            Console.WriteLine("  " + new string('-', 60));
            foreach (Avaliacao av in modulo.Avaliacoes)
            {
                Console.ForegroundColor = av.Nota >= 10 ? ConsoleColor.Green : ConsoleColor.Red;
                Console.WriteLine($"  {av.CodigoFormando,-14} {av.Tipo,-12} " +
                                  $"{av.Nota,-8:F1} {av.ObterClassificacaoQualitativa(),-22} " +
                                  $"{av.DataAvaliacao:dd/MM/yyyy}");
                Console.ResetColor();
            }
            Rodape();
        }

        private static string ObterMencao(double nota) => nota switch
        {
            >= 18 => "Distinção com Louvor",
            >= 16 => "Distinção",
            >= 14 => "Muito Bom",
            >= 10 => "Aprovado",
            _ => "Reprovado"
        };
    }
}
