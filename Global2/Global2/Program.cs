
using System;
using System.IO;

namespace ProtecaoCibernetica
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.Title = "Sistema de Proteção Cibernética v2.0";
                Console.ForegroundColor = ConsoleColor.Green;

                ExibirBanner();

                // Verifica se o sistema foi fechado de forma anômala na última vez
                ErrorLogger.CheckForAbnormalShutdown();

                // Cria os objetos principais do sistema
                LoginManager loginManager = new LoginManager();
                SystemManager systemManager = new SystemManager();
                bool sairDoSistema = false;

                // Registra que o sistema foi iniciado
                ErrorLogger.LogSystemEvent("Sistema", "Sistema iniciado");

                // Loop principal - continua até o usuário sair
                while (!sairDoSistema)
                {
                    bool usuarioLogado = false;

                    // Processo de login - continua até fazer login ou sair
                    while (!usuarioLogado && !sairDoSistema)
                    {
                        try
                        {
                            // Mostra a tela de login
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            Console.WriteLine("\n=== ACESSO SEGURO ===");
                            Console.ForegroundColor = ConsoleColor.White;

                            Console.Write("Usuário: ");
                            string nomeUsuario = Console.ReadLine();

                            // Verifica se o usuário digitou alguma coisa
                            if (string.IsNullOrEmpty(nomeUsuario))
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("Nome de usuário não pode estar vazio!");
                                Console.ForegroundColor = ConsoleColor.White;
                                continue; // Volta para pedir o usuário novamente
                            }

                            // Pede senha
                            Console.Write("Senha: ");
                            string senha = LerSenhaOculta();

                            // Tenta fazer o login
                            usuarioLogado = loginManager.AttemptLogin(nomeUsuario, senha);

                            if (!usuarioLogado)
                            {
                                // Se as credenciais estão erradas, mostra erro
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("❌ Credenciais inválidas. Tente novamente.");
                                Console.ForegroundColor = ConsoleColor.White;

                                // Registra a tentativa de login inválida
                                ErrorLogger.LogError(nomeUsuario, new Exception("Tentativa de login com credenciais inválidas"));
                            }
                        }
                        catch (Exception erro)
                        {
                            // Se aconteceu algum erro grave durante o login
                            ErrorLogger.LogError("Sistema", erro);
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine($"❌ Erro crítico no sistema: {erro.Message}");
                            Console.ForegroundColor = ConsoleColor.White;
                            sairDoSistema = true;
                        }
                    }

                    // Se decidiu sair durante o login, para o programa
                    if (sairDoSistema) break;

                    // Login foi bem-sucedido!
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"✅ Login bem-sucedido! Bem-vindo, {loginManager.CurrentUser}!");
                    Console.ForegroundColor = ConsoleColor.White;

                    // Registra o início da sessão (para detectar fechamento anômalo depois)
                    ErrorLogger.LogSessionStart(loginManager.CurrentUser);

                    // Menu principal - continua até fazer logout ou sair
                    bool sistemaRodando = true;
                    while (sistemaRodando)
                    {
                        try
                        {
                            // Mostra o menu e pede uma opção
                            ExibirMenuPrincipal();
                            string opcao = Console.ReadLine();

                            // Verifica qual opção o usuário escolheu
                            switch (opcao)
                            {
                                case "1": // Logout
                                    // Remove o registro de sessão (fechamento normal)
                                    ErrorLogger.LogSessionEnd(loginManager.CurrentUser);
                                    loginManager.Logout();
                                    sistemaRodando = false; // Volta para a tela de login
                                    break;

                                case "2": // Status do sistema
                                    systemManager.VerificarStatusSistema();
                                    break;

                                case "3": // Relatório de segurança
                                    systemManager.GerarRelatorioSeguranca();
                                    break;

                                case "4": // Teste de falha crítica
                                    Console.ForegroundColor = ConsoleColor.Yellow;
                                    Console.WriteLine("⚠️ Teste de falha crítica - Sistema será encerrado anormalmente...");
                                    Console.ForegroundColor = ConsoleColor.White;

                                    System.Threading.Thread.Sleep(2000);

                                    // Gera um erro de propósito para testar o sistema de logs
                                    throw new Exception("FALHA CRÍTICA - Teste de encerramento anômalo do sistema");

                                case "5": // Limpar logs
                                    systemManager.LimparLogs();
                                    break;

                                case "6": // Sair do sistema
                                    // Remove o registro de sessão (fechamento normal)
                                    ErrorLogger.LogSessionEnd(loginManager.CurrentUser);
                                    loginManager.Logout();
                                    sairDoSistema = true;
                                    sistemaRodando = false;
                                    ErrorLogger.LogSystemEvent(loginManager.CurrentUser, "Sistema encerrado normalmente pelo usuário");
                                    break;

                                default: // Opção inválida
                                    Console.ForegroundColor = ConsoleColor.Yellow;
                                    Console.WriteLine("⚠️ Opção inválida. Tente novamente.");
                                    Console.ForegroundColor = ConsoleColor.White;
                                    break;
                            }
                        }
                        catch (Exception erro)
                        {
                            // Se aconteceu um erro durante o uso do sistema
                            ErrorLogger.LogError(loginManager.CurrentUser, erro);
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine($"💥 FALHA CRÍTICA! Sistema encerrado por segurança: {erro.Message}");
                            Console.ForegroundColor = ConsoleColor.White;

                            // NÃO remove o registro da sessão - simula fechamento anômalo
                            sistemaRodando = false;
                            sairDoSistema = true;
                        }
                    }
                }

                // Mensagem final quando o sistema é encerrado
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("\n🔒 Sistema de Proteção Cibernética encerrado.");
                Console.ForegroundColor = ConsoleColor.White;
            }
            catch (Exception erro)
            {
                // Se aconteceu um erro muito grave que não foi tratado
                ErrorLogger.LogError("Sistema", new Exception($"Erro crítico não tratado: {erro.Message}"));
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("❌ Erro crítico. Sistema será encerrado.");
                Console.ForegroundColor = ConsoleColor.White;
            }
            finally
            {
                // Sempre executa, mesmo se houver erro
                Console.WriteLine("\nPressione qualquer tecla para sair...");
                Console.ReadKey();
            }
        }

        // Função que mostra o banner inicial do sistema
        private static void ExibirBanner()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("╔══════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║              SISTEMA DE PROTEÇÃO CIBERNÉTICA                ║");
            Console.WriteLine("║                        Versão 2.0                           ║");
            Console.WriteLine("║                                                              ║");
            Console.WriteLine("║              🛡️  SEGURANÇA AVANÇADA  🛡️                     ║");
            Console.WriteLine("╚══════════════════════════════════════════════════════════════╝");
            Console.ForegroundColor = ConsoleColor.White;
        }

        // Função que mostra o menu principal do sistema
        private static void ExibirMenuPrincipal()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n=== MENU PRINCIPAL ===");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("1. 🔓 Logout");
            Console.WriteLine("2. 📊 Verificar Status do Sistema");
            Console.WriteLine("3. 📋 Gerar Relatório de Segurança");
            Console.WriteLine("4. ⚠️ Teste de Falha Crítica");
            Console.WriteLine("5. 🧹 Limpar Logs");
            Console.WriteLine("6. 🚪 Sair do Sistema");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("Escolha uma opção: ");
            Console.ForegroundColor = ConsoleColor.White;
        }

        // Função que lê a senha escondendo os caracteres com asteriscos
        private static string LerSenhaOculta()
        {
            string senha = "";
            ConsoleKeyInfo tecla;

            do
            {
                // Lê uma tecla sem mostrar na tela
                tecla = Console.ReadKey(true);

                // Se apertou backspace e tem caracteres para apagar
                if (tecla.Key == ConsoleKey.Backspace && senha.Length > 0)
                {
                    // Remove o último caractere da senha
                    senha = senha.Substring(0, senha.Length - 1);
                    // Apaga o asterisco da tela
                    Console.Write("\b \b");
                }
                // Se não é Enter nem Backspace, adiciona o caractere
                else if (tecla.Key != ConsoleKey.Enter && tecla.Key != ConsoleKey.Backspace)
                {
                    senha += tecla.KeyChar;
                    Console.Write("*"); // Mostra asterisco no lugar do caractere real
                }
            } while (tecla.Key != ConsoleKey.Enter); // Continua até apertar Enter

            Console.WriteLine(); // Quebra de linha após terminar de digitar
            return senha;
        }
    }
}
