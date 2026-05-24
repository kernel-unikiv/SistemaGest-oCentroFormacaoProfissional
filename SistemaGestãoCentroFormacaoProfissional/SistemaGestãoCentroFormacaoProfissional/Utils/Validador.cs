using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaGestãoCentroFormacaoProfissional.Utils
{
    public class Validador
    {
        public static bool NomeValido(string nome)
            => !string.IsNullOrWhiteSpace(nome) && nome.Trim().Length >= 3;

        public static bool CodigoValido(string codigo)
            => !string.IsNullOrWhiteSpace(codigo) && codigo.Trim().Length >= 2;

        public static bool EmailValido(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;
            return email.Contains('@') && email.Contains('.') && email.Length >= 5;
        }

        public static bool TelefoneValido(string telefone)
        {
            if (string.IsNullOrWhiteSpace(telefone)) return false;
            string apenas = new string(telefone.Where(char.IsDigit).ToArray());
            return apenas.Length >= 9;
        }

        public static bool NotaValida(double nota)
            => nota >= 0.0 && nota <= 20.0;

        public static bool CargaHorariaValida(int horas)
            => horas > 0 && horas <= 2000;

        public static bool IdadeMinValida(int idade)
            => idade >= 14 && idade <= 65;

        public static bool NotaMinAprovacaoValida(double nota)
            => nota >= 1.0 && nota <= 20.0;

        public static bool DataNascimentoValida(DateTime data)
        {
            int idade = DateTime.Today.Year - data.Year;
            if (data.Date > DateTime.Today.AddYears(-idade)) idade--;
            return data < DateTime.Today && idade >= 14 && idade <= 80;
        }

        public static bool IntervaloDatasValido(DateTime inicio, DateTime fim)
            => fim > inicio;


        public static int LerInteiroValido(string mensagem, int min = int.MinValue, int max = int.MaxValue)
        {
            while (true)
            {
                Console.Write(mensagem);
                string? entrada = Console.ReadLine();
                if (int.TryParse(entrada, out int resultado) &&
                    resultado >= min && resultado <= max)
                    return resultado;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"  Valor inválido. Introduza um inteiro entre {min} e {max}.");
                Console.ResetColor();
            }
        }

       
        public static double LerDoubleValido(string mensagem, double min = 0.0, double max = double.MaxValue)
        {
            while (true)
            {
                Console.Write(mensagem);
                string? entrada = Console.ReadLine();
                if (double.TryParse(entrada?.Replace(',', '.'),
                    System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture,
                    out double resultado) &&
                    resultado >= min && resultado <= max)
                    return resultado;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"  Valor inválido. Introduza um número entre {min:F1} e {max:F1}.");
                Console.ResetColor();
            }
        }

    
        public static DateTime LerDataValida(string mensagem)
        {
            while (true)
            {
                Console.Write(mensagem);
                string? entrada = Console.ReadLine();
                if (DateTime.TryParseExact(entrada, "dd/MM/yyyy",
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None, out DateTime data))
                    return data;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("  Formato inválido. Use dd/MM/yyyy (ex: 15/03/1990).");
                Console.ResetColor();
            }
        }

     
        public static string LerStringObrigatoria(string mensagem)
        {
            while (true)
            {
                Console.Write(mensagem);
                string? entrada = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(entrada))
                    return entrada.Trim();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("  Este campo é obrigatório.");
                Console.ResetColor();
            }
        }


        public static int EscolherOpcaoLista<T>(string titulo, IReadOnlyList<T> lista)
        {
            if (!lista.Any())
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("  (Lista vazia — nenhuma opção disponível)");
                Console.ResetColor();
                return -1;
            }

            Console.WriteLine($"\n  {titulo}");
            for (int i = 0; i < lista.Count; i++)
                Console.WriteLine($"    {i + 1}. {lista[i]}");

            int opcao = LerInteiroValido("  Escolha (número): ", 1, lista.Count);
            return opcao - 1;
        }

        public static bool ConfirmarAccao(string mensagem)
        {
            while (true)
            {
                Console.Write($"  {mensagem} (S/N): ");
                string? resp = Console.ReadLine()?.Trim().ToUpper();
                if (resp == "S") return true;
                if (resp == "N") return false;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("  Resposta inválida. Introduza S ou N.");
                Console.ResetColor();
            }
        }
    }
}
