using SistemaGestãoCentroFormacaoProfissional.Exceptions;
using SistemaGestãoCentroFormacaoProfissional.Models;
using SistemaGestãoCentroFormacaoProfissional.Services;
using SistemaGestãoCentroFormacaoProfissional.Utils;
using static SistemaGestãoCentroFormacaoProfissional.Models.Enumeracoes;

namespace SistemaGestãoCentroFormacaoProfissional
{
    public class Program
    {
        private static readonly GestorFormacao _gestor = new GestorFormacao();
        private static readonly ServicoRelatorio _relatorio = new ServicoRelatorio();

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            InicializarDadosDemo();

            bool executar = true;
            while (executar)
            {
                MostrarMenuPrincipal();
                string opcao = (Console.ReadLine() ?? "").Trim();

                try
                {
                    switch (opcao)
                    {
                        case "1": MenuCursos(); break;
                        case "2": MenuFormadores(); break;
                        case "3": MenuFormandos(); break;
                        case "4": MenuInscricoes(); break;
                        case "5": MenuAvaliacoes(); break;
                        case "6": MenuCertificados(); break;
                        case "7": MenuRelatorios(); break;
                        case "0": executar = false; break;
                        default:
                            MostrarErro("Opção inválida. Escolha uma opção do menu.");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    MostrarErro($"Erro inesperado: {ex.Message}");
                }

                if (executar) PauseParaContinuar();
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n  Sistema encerrado. Até breve!");
            Console.ResetColor();
        }

        //  MENU PRINCIPAL
        private static void MostrarMenuPrincipal()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("""
  
         SISTEMA DE GESTÃO DE CENTRO DE FORMAÇÃO PROFISSIONAL  
                              
  """);
            Console.ResetColor();
            Console.WriteLine("  1.  Gestão de Cursos");
            Console.WriteLine("  2.  Gestão de Formadores");
            Console.WriteLine("  3.  Gestão de Formandos");
            Console.WriteLine("  4.  Gestão de Inscrições");
            Console.WriteLine("  5.  Avaliações e Presenças");
            Console.WriteLine("  6.  Certificados");
            Console.WriteLine("  7.  Relatórios");
            Console.WriteLine("  0.  Sair");
            Console.WriteLine();
            Console.Write("  Escolha: ");
        }

        //  MENU: CURSOS
        private static void MenuCursos()
        {
            bool voltar = false;
            while (!voltar)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("\n  »» GESTÃO DE CURSOS »»»»»»»»»»»»»»»»»»»»»»»»»»»»»»");
                Console.ResetColor();
                Console.WriteLine("  1. Criar novo curso");
                Console.WriteLine("  2. Adicionar módulo a um curso");
                Console.WriteLine("  3. Listar todos os cursos");
                Console.WriteLine("  4. Ver detalhes de um curso");
                Console.WriteLine("  5. Activar / Desactivar curso");
                Console.WriteLine("  0. Voltar");
                Console.Write("\n  Escolha: ");

                switch ((Console.ReadLine() ?? "").Trim())
                {
                    case "1": CriarCurso(); break;
                    case "2": AdicionarModuloACurso(); break;
                    case "3": ListarCursos(); break;
                    case "4": DetalhesCurso(); break;
                    case "5": AlterarEstadoCurso(); break;
                    case "0": voltar = true; break;
                    default: MostrarErro("Opção inválida."); break;
                }
                if (!voltar) PauseParaContinuar();
            }
        }

        private static void CriarCurso()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n  »» Criar Novo Curso »»»»»»»»»»»»»»»»»»»»»»»»»»»»»");
            Console.ResetColor();

            string codigo = Validador.LerStringObrigatoria("  Código do curso     : ");
            string nome = Validador.LerStringObrigatoria("  Nome do curso       : ");
            string desc = Validador.LerStringObrigatoria("  Descrição           : ");
            int horas = Validador.LerInteiroValido("  Duração total (h)  : ", 1, 2000);

            Console.WriteLine("  Modalidade: 1-Presencial  2-Online  3-Misto");
            int modOpc = Validador.LerInteiroValido("  Escolha             : ", 1, 3);
            Modalidade modalidade = (Modalidade)(modOpc - 1);

            int idadeMin = Validador.LerInteiroValido("  Idade mínima (anos): ", 14, 65);
            string escMin = Validador.LerStringObrigatoria("  Escolaridade mínima (ex: 12ª classe): ");
            double notaMin = Validador.LerDoubleValido("  Nota mínima de aprovação (0-20): ", 0, 20);

            DateTime dataInicio = Validador.LerDataValida("  Data de início (dd/MM/yyyy): ");
            DateTime dataFim;
            do
            {
                dataFim = Validador.LerDataValida("  Data de fim    (dd/MM/yyyy): ");
                if (dataFim <= dataInicio)
                    MostrarErro("A data de fim deve ser posterior à data de início.");
            } while (dataFim <= dataInicio);

            try
            {
                Curso curso = new Curso(codigo, nome, desc, horas, modalidade,
                                        idadeMin, escMin, notaMin, dataInicio, dataFim);
                _gestor.CriarCurso(curso);
                MostrarSucesso($"Curso '{nome}' criado com sucesso!");
            }
            catch (Exception ex) { MostrarErro(ex.Message); }
        }

        private static void AdicionarModuloACurso()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n  »» Adicionar Módulo a Curso »»»»»»»»»»»»»»»»»»»»»»»»»»»»");
            Console.ResetColor();

            if (!_gestor.Cursos.Any()) { MostrarErro("Não existem cursos registados."); return; }

            ListarCursos();
            string codigoCurso = Validador.LerStringObrigatoria("\n  Código do curso : ");

            if (!_gestor.Formadores.Any()) { MostrarErro("Registar formadores primeiro."); return; }

            Console.WriteLine("\n  Formadores disponíveis:");
            foreach (var f in _gestor.Formadores)
                Console.WriteLine($"    [{f.CodigoFormador}] {f.Nome}");

            string codFormador = Validador.LerStringObrigatoria("  Código do formador: ");

            string nomeM = Validador.LerStringObrigatoria("  Nome do módulo     : ");
            string descM = Validador.LerStringObrigatoria("  Descrição          : ");
            int horas = Validador.LerInteiroValido("  Carga horária (h) : ", 1, 500);
            DateTime dI = Validador.LerDataValida("  Início (dd/MM/yyyy): ");
            DateTime dF = Validador.LerDataValida("  Fim    (dd/MM/yyyy): ");

            try
            {
                Formador formador = _gestor.ObterFormador(codFormador);
                Modulo modulo = new Modulo(nomeM, descM, horas, formador, dI, dF);
                _gestor.AdicionarModuloACurso(codigoCurso, modulo);
                MostrarSucesso($"Módulo '{nomeM}' (ID:{modulo.Id}) adicionado ao curso '{codigoCurso}'.");
            }
            catch (Exception ex) { MostrarErro(ex.Message); }
        }

        private static void ListarCursos()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n  »» Cursos Registados »»»»»»»»»»»»»»»»»»»»»»»»»»»»»");
            Console.ResetColor();

            var cursos = _gestor.ListarCursos();
            if (!cursos.Any()) { Console.WriteLine("  (Nenhum curso registado)"); return; }

            foreach (Curso c in cursos)
            {
                Console.ForegroundColor = c.Activo ? ConsoleColor.White : ConsoleColor.DarkGray;
                Console.WriteLine($"  {c}");
                Console.ResetColor();
            }
        }

        private static void DetalhesCurso()
        {
            string cod = Validador.LerStringObrigatoria("  Código do curso: ");
            try
            {
                Curso curso = _gestor.ObterCurso(cod);
                Console.WriteLine($"\n  [{curso.Codigo}] {curso.Nome}");
                Console.WriteLine($"  Descrição       : {curso.Descricao}");
                Console.WriteLine($"  Duração         : {curso.DuracaoHoras}h | Modalidade: {curso.Modalidade}");
                Console.WriteLine($"  Vigência        : {curso.DataInicio:dd/MM/yyyy} → {curso.DataFim:dd/MM/yyyy}");
                Console.WriteLine($"  Req. Acesso     : Idade ≥ {curso.IdadeMinima} | Esc.: {curso.EscolaridadeMinima}");
                Console.WriteLine($"  Nota mín. aprov.: {curso.NotaMinAprovacao:F1}");
                Console.WriteLine($"  Estado          : {(curso.Activo ? "Activo" : "Inactivo")}");
                Console.WriteLine($"\n  Módulos ({curso.TotalModulos}):");
                if (!curso.Modulos.Any())
                    Console.WriteLine("    (Nenhum módulo adicionado)");
                else
                    foreach (Modulo m in curso.Modulos)
                        Console.WriteLine($"    {m}");
            }
            catch (Exception ex) { MostrarErro(ex.Message); }
        }

        private static void AlterarEstadoCurso()
        {
            string cod = Validador.LerStringObrigatoria("  Código do curso: ");
            try
            {
                Curso curso = _gestor.ObterCurso(cod);
                if (curso.Activo)
                {
                    if (Validador.ConfirmarAccao($"Desactivar o curso '{curso.Nome}'?"))
                    { curso.Desactivar(); MostrarSucesso("Curso desactivado."); }
                }
                else
                {
                    if (Validador.ConfirmarAccao($"Activar o curso '{curso.Nome}'?"))
                    { curso.Activar(); MostrarSucesso("Curso activado."); }
                }
            }
            catch (Exception ex) { MostrarErro(ex.Message); }
        }

        //  MENU: FORMADORES
        private static void MenuFormadores()
        {
            bool voltar = false;
            while (!voltar)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("\n  »» GESTÃO DE FORMADORES »»»»»»»»»»»»»»»»»»»»»»»»»»»»");
                Console.ResetColor();
                Console.WriteLine("  1. Registar formador");
                Console.WriteLine("  2. Listar formadores");
                Console.WriteLine("  3. Ver detalhes de formador");
                Console.WriteLine("  4. Adicionar área de competência");
                Console.WriteLine("  0. Voltar");
                Console.Write("\n  Escolha: ");

                switch ((Console.ReadLine() ?? "").Trim())
                {
                    case "1": RegistarFormador(); break;
                    case "2": ListarFormadores(); break;
                    case "3": DetalhesFormador(); break;
                    case "4": AdicionarAreaCompetencia(); break;
                    case "0": voltar = true; break;
                    default: MostrarErro("Opção inválida."); break;
                }
                if (!voltar) PauseParaContinuar();
            }
        }

        private static void RegistarFormador()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n  »» Registar Formador »»»»»»»»»»»»»»»»»»»»»»»");
            Console.ResetColor();

            string nome = Validador.LerStringObrigatoria("  Nome completo      : ");
            string bi = Validador.LerStringObrigatoria("  Número de BI       : ");
            string email = Validador.LerStringObrigatoria("  E-mail             : ");
            string tel = Validador.LerStringObrigatoria("  Telefone           : ");
            DateTime nascimento = Validador.LerDataValida("  Data nascimento (dd/MM/yyyy): ");
            string codigo = Validador.LerStringObrigatoria("  Código do formador : ");
            string registo = Validador.LerStringObrigatoria("  Nº Registo Prof.   : ");

            Console.WriteLine("  Habilitação: 1-CursoTécnico  2-Licenciatura  3-Mestrado  4-Doutoramento");
            int habOpc = Validador.LerInteiroValido("  Escolha: ", 1, 4);
            NivelHabilitacao hab = (NivelHabilitacao)(habOpc - 1);

            try
            {
                Formador f = new Formador(nome, bi, email, tel, nascimento, codigo, hab, registo);
                _gestor.RegistarFormador(f);
                MostrarSucesso($"Formador '{nome}' registado com código '{codigo}'.");
            }
            catch (Exception ex) { MostrarErro(ex.Message); }
        }

        private static void ListarFormadores()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n  »» Formadores Registados »»»»»»»»»»»»»»»»»»»»»»»»»»»»»");
            Console.ResetColor();

            var lista = _gestor.ListarFormadores();
            if (!lista.Any()) { Console.WriteLine("  (Nenhum formador registado)"); return; }
            foreach (Formador f in lista)
                Console.WriteLine($"  [{f.CodigoFormador}] {f.Nome} | {f.Habilitacao}");
        }

        private static void DetalhesFormador()
        {
            string cod = Validador.LerStringObrigatoria("  Código do formador: ");
            try { Console.WriteLine($"\n{_gestor.ObterFormador(cod).ObterDetalhes()}"); }
            catch (Exception ex) { MostrarErro(ex.Message); }
        }

        private static void AdicionarAreaCompetencia()
        {
            string cod = Validador.LerStringObrigatoria("  Código do formador: ");
            string area = Validador.LerStringObrigatoria("  Área de competência: ");
            try
            {
                _gestor.ObterFormador(cod).AdicionarAreaCompetencia(area);
                MostrarSucesso("Área adicionada.");
            }
            catch (Exception ex) { MostrarErro(ex.Message); }
        }

        //  MENU: FORMANDOS
        private static void MenuFormandos()
        {
            bool voltar = false;
            while (!voltar)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("\n  »» GESTÃO DE FORMANDOS »»»»»»»»»»»»»»»»»»»»»»»»»»»");
                Console.ResetColor();
                Console.WriteLine("  1. Registar formando");
                Console.WriteLine("  2. Listar formandos");
                Console.WriteLine("  3. Ver detalhes de formando");
                Console.WriteLine("  0. Voltar");
                Console.Write("\n  Escolha: ");

                switch ((Console.ReadLine() ?? "").Trim())
                {
                    case "1": RegistarFormando(); break;
                    case "2": ListarFormandos(); break;
                    case "3": DetalhesFormando(); break;
                    case "0": voltar = true; break;
                    default: MostrarErro("Opção inválida."); break;
                }
                if (!voltar) PauseParaContinuar();
            }
        }

        private static void RegistarFormando()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n  »» Registar Formando »»»»»»»»»»»»»»»»»»»»»»»»»»»»»»»»»");
            Console.ResetColor();

            string nome = Validador.LerStringObrigatoria("  Nome completo      : ");
            string bi = Validador.LerStringObrigatoria("  Número de BI       : ");
            string email = Validador.LerStringObrigatoria("  E-mail             : ");
            string tel = Validador.LerStringObrigatoria("  Telefone           : ");
            DateTime nascimento = Validador.LerDataValida("  Data nascimento (dd/MM/yyyy): ");
            string codigo = Validador.LerStringObrigatoria("  Código do formando : ");
            Console.WriteLine("  Escolaridade (ex: 12ª classe / licenciatura / mestrado)");
            string escolar = Validador.LerStringObrigatoria("  Escolaridade       : ");

            try
            {
                Formando fd = new Formando(nome, bi, email, tel, nascimento, codigo, escolar);
                _gestor.RegistarFormando(fd);
                MostrarSucesso($"Formando '{nome}' registado com código '{codigo}'.");
            }
            catch (Exception ex) { MostrarErro(ex.Message); }
        }

        private static void ListarFormandos()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n  »» Formandos Registados »»»»»»»»»»»»»»»»»»»»»»»»»»»»»»");
            Console.ResetColor();

            var lista = _gestor.ListarFormandos();
            if (!lista.Any()) { Console.WriteLine("  (Nenhum formando registado)"); return; }
            foreach (Formando f in lista)
                Console.WriteLine($"  [{f.CodigoFormando}] {f.Nome} | {f.NivelEscolaridade} | {f.CalcularIdade()} anos");
        }

        private static void DetalhesFormando()
        {
            string cod = Validador.LerStringObrigatoria("  Código do formando: ");
            try { Console.WriteLine($"\n{_gestor.ObterFormando(cod).ObterDetalhes()}"); }
            catch (Exception ex) { MostrarErro(ex.Message); }
        }

        
        //  MENU: INSCRIÇÕES
        private static void MenuInscricoes()
        {
            bool voltar = false;
            while (!voltar)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("\n  »» GESTÃO DE INSCRIÇÕES »»»»»»»»»»»»»»»»»»»»»»»»»»");
                Console.ResetColor();
                Console.WriteLine("  1. Inscrever formando num curso");
                Console.WriteLine("  2. Listar todas as inscrições");
                Console.WriteLine("  3. Ver classificação e estado de inscrição");
                Console.WriteLine("  4. Concluir inscrição");
                Console.WriteLine("  5. Cancelar inscrição");
                Console.WriteLine("  0. Voltar");
                Console.Write("\n  Escolha: ");

                switch ((Console.ReadLine() ?? "").Trim())
                {
                    case "1": InscreverFormando(); break;
                    case "2": ListarInscricoes(); break;
                    case "3": VerResultadoInscricao(); break;
                    case "4": ConcluirInscricao(); break;
                    case "5": CancelarInscricao(); break;
                    case "0": voltar = true; break;
                    default: MostrarErro("Opção inválida."); break;
                }
                if (!voltar) PauseParaContinuar();
            }
        }

        private static void InscreverFormando()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n  »» Inscrever Formando »»»»»»»»»»»»»»»»»»»»»»»»»»»»»»»»");
            Console.ResetColor();

            string codFormando = Validador.LerStringObrigatoria("  Código do formando : ");
            string codCurso = Validador.LerStringObrigatoria("  Código do curso    : ");

            try
            {
                Inscricao ins = _gestor.InscreverFormando(codFormando, codCurso);
                MostrarSucesso($"Inscrição #{ins.Id:D3} criada com sucesso!");
            }
            catch (FormandoJaInscritoException ex)
            {
                MostrarErro($"[FormandoJaInscrito] {ex.Message}");
            }
            catch (RequisitoAcessoNaoSatisfeitoException ex)
            {
                MostrarErro($"[RequisitoAcesso] {ex.Message}");
            }
            catch (Exception ex)
            {
                MostrarErro(ex.Message);
            }
        }

        private static void ListarInscricoes()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n  »» Inscrições Registadas »»»»»»»»»»»»»»»»»»»»»»»»»»»»»");
            Console.ResetColor();

            var lista = _gestor.ListarInscricoes();
            if (!lista.Any()) { Console.WriteLine("  (Nenhuma inscrição registada)"); return; }
            foreach (Inscricao i in lista)
            {
                Console.ForegroundColor = i.Estado switch
                {
                    EstadoInscricao.Activa => ConsoleColor.White,
                    EstadoInscricao.Concluida => ConsoleColor.Green,
                    EstadoInscricao.Cancelada => ConsoleColor.DarkGray,
                    _ => ConsoleColor.White
                };
                Console.WriteLine($"  {i}");
                Console.ResetColor();
            }
        }

        private static void VerResultadoInscricao()
        {
            int id = Validador.LerInteiroValido("  ID da inscrição: ", 1, int.MaxValue);
            try
            {
                var (classificacao, aprovado) = _gestor.ObterResultadoFormando(id);
                Inscricao ins = _gestor.ObterInscricao(id);

                Console.WriteLine($"\n  Inscrição  : {ins}");
                Console.Write($"  Classif.   : {classificacao:F1} valores  →  ");
                Console.ForegroundColor = aprovado ? ConsoleColor.Green : ConsoleColor.Red;
                Console.WriteLine(aprovado ? "✔ APROVADO" : "✘ REPROVADO");
                Console.ResetColor();

                if (aprovado && ins.Estado == EstadoInscricao.Concluida && ins.Certificado != null)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"  Certificado: {ins.Certificado.NumeroCertificado}");
                    Console.ResetColor();
                }
            }
            catch (Exception ex) { MostrarErro(ex.Message); }
        }

        private static void ConcluirInscricao()
        {
            int id = Validador.LerInteiroValido("  ID da inscrição a concluir: ", 1, int.MaxValue);
            try
            {
                _gestor.ConcluirInscricao(id);
                MostrarSucesso($"Inscrição #{id:D3} marcada como Concluída.");
            }
            catch (Exception ex) { MostrarErro(ex.Message); }
        }

        private static void CancelarInscricao()
        {
            int id = Validador.LerInteiroValido("  ID da inscrição a cancelar: ", 1, int.MaxValue);
            if (Validador.ConfirmarAccao("Confirmar cancelamento?"))
            {
                try
                {
                    _gestor.CancelarInscricao(id);
                    MostrarSucesso($"Inscrição #{id:D3} cancelada.");
                }
                catch (Exception ex) { MostrarErro(ex.Message); }
            }
        }

        //  MENU: AVALIAÇÕES E PRESENÇAS
        
        private static void MenuAvaliacoes()
        {
            bool voltar = false;
            while (!voltar)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("\n  »» AVALIAÇÕES E PRESENÇAS »»»»»»»»»»»»»»»»»»»»»»»»");
                Console.ResetColor();
                Console.WriteLine("  1. Lançar avaliação");
                Console.WriteLine("  2. Ver avaliações de um módulo");
                Console.WriteLine("  3. Registar presença");
                Console.WriteLine("  4. Ver presenças de um módulo");
                Console.WriteLine("  0. Voltar");
                Console.Write("\n  Escolha: ");

                switch ((Console.ReadLine() ?? "").Trim())
                {
                    case "1": LancarAvaliacao(); break;
                    case "2": VerAvaliacoesModulo(); break;
                    case "3": RegistarPresenca(); break;
                    case "4": VerPresencasModulo(); break;
                    case "0": voltar = true; break;
                    default: MostrarErro("Opção inválida."); break;
                }
                if (!voltar) PauseParaContinuar();
            }
        }

        private static void LancarAvaliacao()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n  »» Lançar Avaliação »»»»»»»»»»»»»»»»»»»»»»»»»»»»»»»»»»");
            Console.ResetColor();

            string codCurso = Validador.LerStringObrigatoria("  Código do curso   : ");
            int moduloId = Validador.LerInteiroValido("  ID do módulo      : ", 1, int.MaxValue);
            string codFormando = Validador.LerStringObrigatoria("  Código do formando: ");
            double nota = Validador.LerDoubleValido("  Nota (0 a 20)     : ", 0, 20);

            Console.WriteLine("  Tipo: 1-Teórica  2-Prática  3-Oral  4-Projecto");
            int tipoOpc = Validador.LerInteiroValido("  Escolha: ", 1, 4);
            TipoAvaliacao tipo = (TipoAvaliacao)(tipoOpc - 1);

            Console.Write("  Observações (Enter para omitir): ");
            string obs = Console.ReadLine() ?? string.Empty;

            try
            {
                _gestor.LancarAvaliacao(codCurso, moduloId, codFormando, nota, tipo, obs);
                MostrarSucesso($"Avaliação lançada: {nota:F1} valores ({tipo}).");
            }
            catch (AvaliacaoForaDoPrazoException ex)
            {
                MostrarErro($"[ForaDoPrazo] {ex.Message}");
            }
            catch (Exception ex)
            {
                MostrarErro(ex.Message);
            }
        }

        private static void VerAvaliacoesModulo()
        {
            string codCurso = Validador.LerStringObrigatoria("  Código do curso: ");
            int moduloId = Validador.LerInteiroValido("  ID do módulo   : ", 1, int.MaxValue);
            _relatorio.RelatorioAvaliacoesPorModulo(_gestor, codCurso, moduloId);
        }

        private static void RegistarPresenca()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n  »» Registar Presença »»»»»»»»»»»»»»»»»»»»»»»»»»»»»»»»»");
            Console.ResetColor();

            string codFormando = Validador.LerStringObrigatoria("  Código do formando: ");
            int moduloId = Validador.LerInteiroValido("  ID do módulo      : ", 1, int.MaxValue);
            bool presente = Validador.ConfirmarAccao("O formando esteve presente?");
            string? just = null;

            if (!presente)
            {
                Console.Write("  Justificação da falta (Enter para omitir): ");
                just = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(just)) just = null;
            }

            try
            {
                _gestor.RegistarPresenca(codFormando, moduloId, presente, just);
                MostrarSucesso($"Presença registada: {(presente ? "Presente" : "Ausente")}.");
            }
            catch (Exception ex) { MostrarErro(ex.Message); }
        }

        private static void VerPresencasModulo()
        {
            int moduloId = Validador.LerInteiroValido("  ID do módulo: ", 1, int.MaxValue);
            var lista = _gestor.ListarPresencasPorModulo(moduloId);
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"\n  »» Presenças do Módulo {moduloId} »»»»»»»»»»»»»»»»»»»");
            Console.ResetColor();
            if (!lista.Any()) { Console.WriteLine("  (Nenhuma presença registada)"); return; }
            foreach (Presenca p in lista)
            {
                Console.ForegroundColor = p.Presente ? ConsoleColor.Green : ConsoleColor.Red;
                Console.WriteLine($"  {p}");
                Console.ResetColor();
            }
        }

        
        //  MENU: CERTIFICADOS
        private static void MenuCertificados()
        {
            bool voltar = false;
            while (!voltar)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("\n  »» CERTIFICADOS »»»»»»»»»»»»»»»»»»»»»»»»»»»»»»»»»»");
                Console.ResetColor();
                Console.WriteLine("  1. Emitir certificado (por ID de inscrição)");
                Console.WriteLine("  2. Listar todos os certificados emitidos");
                Console.WriteLine("  0. Voltar");
                Console.Write("\n  Escolha: ");

                switch ((Console.ReadLine() ?? "").Trim())
                {
                    case "1": EmitirCertificado(); break;
                    case "2": ListarCertificados(); break;
                    case "0": voltar = true; break;
                    default: MostrarErro("Opção inválida."); break;
                }
                if (!voltar) PauseParaContinuar();
            }
        }

        private static void EmitirCertificado()
        {
            int id = Validador.LerInteiroValido("  ID da inscrição (concluída e aprovada): ", 1, int.MaxValue);
            try
            {
                Certificado cert = _gestor.EmitirCertificado(id);
                cert.Imprimir();
                MostrarSucesso("Certificado emitido com sucesso!");
            }
            catch (InvalidOperationException ex)
            {
                MostrarErro($"[Certificação] {ex.Message}");
            }
            catch (Exception ex) { MostrarErro(ex.Message); }
        }

        private static void ListarCertificados()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n  »» Certificados Emitidos »»»»»»»»»»»»»»»»»»»»»»»»»»»»»");
            Console.ResetColor();
            var lista = _gestor.ListarTodosCertificados();
            if (!lista.Any()) { Console.WriteLine("  (Nenhum certificado emitido)"); return; }
            foreach (Certificado c in lista)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"  {c}");
                Console.ResetColor();
            }
        }

        //  MENU: RELATÓRIOS
        private static void MenuRelatorios()
        {
            bool voltar = false;
            while (!voltar)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("\n  »» RELATÓRIOS »»»»»»»»»»»»»»»»»»»»»»»»»»»»»»»»»»»»");
                Console.ResetColor();
                Console.WriteLine("  1. Formandos aprovados por curso");
                Console.WriteLine("  2. Desempenho de um formando");
                Console.WriteLine("  3. Sumário geral do sistema");
                Console.WriteLine("  0. Voltar");
                Console.Write("\n  Escolha: ");

                switch ((Console.ReadLine() ?? "").Trim())
                {
                    case "1":
                        string codC = Validador.LerStringObrigatoria("  Código do curso: ");
                        _relatorio.RelatorioAprovadosPorCurso(_gestor, codC);
                        break;
                    case "2":
                        string codF = Validador.LerStringObrigatoria("  Código do formando: ");
                        _relatorio.RelatorioFormando(_gestor, codF);
                        break;
                    case "3":
                        _relatorio.RelatorioSumarioGeral(_gestor);
                        break;
                    case "0": voltar = true; break;
                    default: MostrarErro("Opção inválida."); break;
                }
                if (!voltar) PauseParaContinuar();
            }
        }

        
        //  DADOS DE DEMONSTRAÇÃO
        private static void InicializarDadosDemo()
        {
            // ── Formadores ────────────────────────────────────────────────────
            var f1 = new Formador("Carlos Mendes", "001235784KA058",
                                  "carlosmendes@gmail.com", "923000001",
                                  new DateTime(1978, 4, 12), "FORM001",
                                  NivelHabilitacao.Mestrado, "RP/2018/001");
            f1.AdicionarAreaCompetencia("Programação");
            f1.AdicionarAreaCompetencia("Bases de Dados");

            var f2 = new Formador("Ana Lopes", "005678912KA020",
                                  "analopes@gmail.com", "924000002",
                                  new DateTime(1985, 9, 3), "FORM002",
                                  NivelHabilitacao.Licenciatura, "RP/2020/015");
            f2.AdicionarAreaCompetencia("Redes");
            f2.AdicionarAreaCompetencia("Segurança Informática");

            _gestor.RegistarFormador(f1);
            _gestor.RegistarFormador(f2);

            // ── Formandos ─────────────────────────────────────────────────────
            var fd1 = new Formando("João Silva", "123456789KA001",
                                   "jaosilva@gmail.com", "921111111",
                                   new DateTime(2000, 6, 15), "FD001", "12ª classe");
            var fd2 = new Formando("Maria Neto", "987654321KA002",
                                   "marianeto@gmail.com", "922222222",
                                   new DateTime(1998, 11, 22), "FD002", "licenciatura");
            var fd3 = new Formando("Pedro Gomes", "111222333KA003",
                                   "pedrogomes@gmail.com", "923333333",
                                   new DateTime(2002, 1, 30), "FD003", "12ª classe");
            var fd4 = new Formando("Luísa Matos", "444555666KA004",
                                   "l.matos@mail.ao", "924444444",
                                   new DateTime(2004, 3, 8), "FD004", "9ª classe"); 

            _gestor.RegistarFormando(fd1);
            _gestor.RegistarFormando(fd2);
            _gestor.RegistarFormando(fd3);
            _gestor.RegistarFormando(fd4);

            // ── Curso ─────────────────────────────────────────────────────────
            Curso c1 = new Curso(
                "TI-001", "Técnico de Informática",
                "Formação profissional na área de Tecnologias de Informação.",
                300, Modalidade.Presencial,
                17, "12ª classe", 10.0,
                new DateTime(2025, 1, 15), new DateTime(2025, 12, 15));
            _gestor.CriarCurso(c1);

            // ── Módulos do curso ──────────────────────────────────────────────
            Modulo m1 = new Modulo("Algoritmos e Programação",
                                   "Fundamentos de lógica e programação estruturada.",
                                   80, f1,
                                   new DateTime(2025, 1, 15), new DateTime(2999, 12, 31));
            Modulo m2 = new Modulo("Redes de Computadores",
                                   "Protocolos, topologias e configuração de redes.",
                                   80, f2,
                                   new DateTime(2025, 4, 1), new DateTime(2999, 12, 31));
            Modulo m3 = new Modulo("Bases de Dados",
                                   "Modelação relacional e linguagem SQL.",
                                   80, f1,
                                   new DateTime(2025, 7, 1), new DateTime(2999, 12, 31));

            _gestor.AdicionarModuloACurso("TI-001", m1);
            _gestor.AdicionarModuloACurso("TI-001", m2);
            _gestor.AdicionarModuloACurso("TI-001", m3);

            // ── Inscrições ────────────────────────────────────────────────────
            Inscricao ins1 = _gestor.InscreverFormando("FD001", "TI-001");
            Inscricao ins2 = _gestor.InscreverFormando("FD002", "TI-001");
            Inscricao ins3 = _gestor.InscreverFormando("FD003", "TI-001");

            // ── Avaliações pré-carregadas (FD001 aprovado, FD003 reprovado) ────
            _gestor.LancarAvaliacao("TI-001", m1.Id, "FD001", 15.0, TipoAvaliacao.Teorica);
            _gestor.LancarAvaliacao("TI-001", m1.Id, "FD001", 14.0, TipoAvaliacao.Pratica);
            _gestor.LancarAvaliacao("TI-001", m2.Id, "FD001", 16.0, TipoAvaliacao.Teorica);
            _gestor.LancarAvaliacao("TI-001", m3.Id, "FD001", 13.0, TipoAvaliacao.Projecto);

            _gestor.LancarAvaliacao("TI-001", m1.Id, "FD002", 18.0, TipoAvaliacao.Teorica);
            _gestor.LancarAvaliacao("TI-001", m2.Id, "FD002", 17.5, TipoAvaliacao.Teorica);
            _gestor.LancarAvaliacao("TI-001", m3.Id, "FD002", 19.0, TipoAvaliacao.Projecto);

            _gestor.LancarAvaliacao("TI-001", m1.Id, "FD003", 7.0, TipoAvaliacao.Teorica);
            _gestor.LancarAvaliacao("TI-001", m2.Id, "FD003", 8.0, TipoAvaliacao.Teorica);
            _gestor.LancarAvaliacao("TI-001", m3.Id, "FD003", 6.5, TipoAvaliacao.Projecto);

            // ── Concluir inscrições e emitir certificados ─────────────────────
            _gestor.ConcluirInscricao(ins1.Id); 
            _gestor.ConcluirInscricao(ins2.Id); 
            _gestor.ConcluirInscricao(ins3.Id); 

            _gestor.EmitirCertificado(ins1.Id);
            _gestor.EmitirCertificado(ins2.Id);
        }

        //  UTILITÁRIOS DE INTERFACE
        private static void MostrarErro(string mensagem)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\n  ERRO: {mensagem}");
            Console.ResetColor();
        }

        private static void MostrarSucesso(string mensagem)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\n  {mensagem}");
            Console.ResetColor();
        }

        private static void PauseParaContinuar()
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write("\n  Prima qualquer tecla para continuar...");
            Console.ResetColor();
            Console.ReadKey(true);
        }
    }
}
