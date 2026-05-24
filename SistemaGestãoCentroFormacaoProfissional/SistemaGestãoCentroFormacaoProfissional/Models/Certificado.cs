using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaGestãoCentroFormacaoProfissional.Models
{
    public class Certificado
    {
        private readonly string _numeroCertificado;
        private readonly string _codigoFormando;
        private readonly string _nomeFormando;
        private readonly string _codigoCurso;
        private readonly string _nomeCurso;
        private readonly double _classificacaoFinal;
        private readonly DateTime _dataEmissao;

        public string NumeroCertificado => _numeroCertificado;
        public string CodigoFormando => _codigoFormando;
        public string NomeFormando => _nomeFormando;
        public string CodigoCurso => _codigoCurso;
        public string NomeCurso => _nomeCurso;
        public double ClassificacaoFinal => _classificacaoFinal;
        public DateTime DataEmissao => _dataEmissao;

        public Certificado(string codigoFormando, string nomeFormando,
                           string codigoCurso, string nomeCurso,
                           double classificacaoFinal)
        {
            _codigoFormando = codigoFormando;
            _nomeFormando = nomeFormando;
            _codigoCurso = codigoCurso;
            _nomeCurso = nomeCurso;
            _classificacaoFinal = classificacaoFinal;
            _dataEmissao = DateTime.Now;
            _numeroCertificado = GerarNumeroCertificado(codigoFormando, codigoCurso);
        }

        private static string GerarNumeroCertificado(string codFormando, string codCurso)
        {
            return $"CERT/{codCurso}/{codFormando}/{DateTime.Now:yyyyMMddHHmmss}";
        }

        public void Imprimir()
        {
            string separador = new string('═', 62);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"\n  {separador}");
            Console.WriteLine("             ★  CERTIFICADO DE CONCLUSÃO  ★");
            Console.WriteLine($"  {separador}");
            Console.ResetColor();
            Console.WriteLine($"  Nº Certificado  : {_numeroCertificado}");
            Console.WriteLine($"  Formando        : {_nomeFormando}");
            Console.WriteLine($"  Código Formando : {_codigoFormando}");
            Console.WriteLine($"  Curso           : {_nomeCurso}");
            Console.WriteLine($"  Código do Curso : {_codigoCurso}");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"  Classificação   : {_classificacaoFinal:F1} valores  " +
                              $"({ObterMencao()})");
            Console.ResetColor();
            Console.WriteLine($"  Data de Emissão : {_dataEmissao:dd/MM/yyyy}");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"  {separador}\n");
            Console.ResetColor();
        }

        public string ObterMencao()
        {
            return _classificacaoFinal switch
            {
                >= 18 => "Distinção com Louvor",
                >= 16 => "Distinção",
                >= 14 => "Muito Bom",
                >= 10 => "Aprovado",
                _ => "Reprovado"
            };
        }

        public override string ToString()
        {
            return $"[{_numeroCertificado}] {_nomeFormando} | " +
                   $"Curso: {_nomeCurso} | Classificação: {_classificacaoFinal:F1} | " +
                   $"Emitido: {_dataEmissao:dd/MM/yyyy}";
        }
    }
}
