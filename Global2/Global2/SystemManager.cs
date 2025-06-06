
using System;
using System.IO;
using System.Text;
using System.Diagnostics;

namespace ProtecaoCibernetica
{
    public class SystemManager
    {
        // Função que verifica e mostra informações sobre o status do sistema
        public void VerificarStatusSistema()
        {
            try
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("\n=== STATUS DO SISTEMA ===");
                Console.ForegroundColor = ConsoleColor.White;

                // Mostra informações básicas do sistema
                Console.WriteLine($"🖥️ Máquina: {Environment.MachineName}");
                Console.WriteLine($"👤 Usuário SO: {Environment.UserName}");
                Console.WriteLine($"💾 Sistema Operacional: {Environment.OSVersion}");
                Console.WriteLine($"⏱️ Uptime: {TimeSpan.FromMilliseconds(Environment.TickCount):dd\\:hh\\:mm\\:ss}");
                Console.WriteLine($"🔧 Versão .NET: {Environment.Version}");
                Console.WriteLine($"💼 Working Directory: {Environment.CurrentDirectory}");
                Console.WriteLine($"🏠 Área de Trabalho: {Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}");

                // Verifica informações dos discos do computador
                DriveInfo[] drives = DriveInfo.GetDrives();
                Console.WriteLine("\n💽 Informações de Disco:");
                foreach (DriveInfo drive in drives)
                {
                    if (drive.IsReady) // Só mostra se o disco estiver disponível
                    {
                        // Converte bytes para GB para ficar mais fácil de ler
                        long freeSpace = drive.AvailableFreeSpace / (1024 * 1024 * 1024);
                        long totalSpace = drive.TotalSize / (1024 * 1024 * 1024);
                        Console.WriteLine($"   {drive.Name} - Livre: {freeSpace}GB / Total: {totalSpace}GB");
                    }
                }

                // Conta quantos processos estão rodando no sistema
                Process[] processes = Process.GetProcesses();
                Console.WriteLine($"\n⚙️ Processos em execução: {processes.Length}");

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\n✅ Verificação de status concluída.");
                Console.ForegroundColor = ConsoleColor.White;

                // Registra que a verificação foi feita
                ErrorLogger.LogSystemEvent("Sistema", "Verificação de status realizada");
            }
            catch (Exception ex)
            {
                // Se aconteceu erro durante a verificação
                ErrorLogger.LogError("SystemManager", ex);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"❌ Erro ao verificar status: {ex.Message}");
                Console.ForegroundColor = ConsoleColor.White;
            }
        }

        // Função que gera um relatório completo de segurança do sistema
        public void GerarRelatorioSeguranca()
        {
            try
            {
                // Define onde o relatório será salvo (na área de trabalho)
                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string reportPath = Path.Combine(desktopPath, $"relatorio_seguranca_{DateTime.Now:yyyyMMdd_HHmmss}.txt");

                // Usa StringBuilder para montar o relatório de forma eficiente
                StringBuilder relatorio = new StringBuilder();

                // Cabeçalho do relatório
                relatorio.AppendLine("╔══════════════════════════════════════════════════════════════╗");
                relatorio.AppendLine("║              RELATÓRIO DE SEGURANÇA DO SISTEMA              ║");
                relatorio.AppendLine("║                Sistema de Proteção Cibernética              ║");
                relatorio.AppendLine("╚══════════════════════════════════════════════════════════════╝");
                relatorio.AppendLine($"Data/Hora: {DateTime.Now:dd/MM/yyyy HH:mm:ss}");
                relatorio.AppendLine($"Gerado por: Sistema Automatizado");
                relatorio.AppendLine();

                // Seção com informações do sistema
                relatorio.AppendLine("=== INFORMAÇÕES DO SISTEMA ===");
                relatorio.AppendLine($"Máquina: {Environment.MachineName}");
                relatorio.AppendLine($"Usuário: {Environment.UserName}");
                relatorio.AppendLine($"SO: {Environment.OSVersion}");
                relatorio.AppendLine($"Uptime: {TimeSpan.FromMilliseconds(Environment.TickCount):dd\\:hh\\:mm\\:ss}");
                relatorio.AppendLine();

                // Seção com logs de erro do sistema
                relatorio.AppendLine("=== LOGS DE ERRO ===");
                relatorio.AppendLine(ErrorLogger.GetErrorLogContent());
                relatorio.AppendLine();

                // Seção com logs gerais do sistema
                relatorio.AppendLine("=== LOGS DO SISTEMA ===");
                relatorio.AppendLine(ErrorLogger.GetSystemLogContent());
                relatorio.AppendLine();

                // Seção com recomendações de segurança
                relatorio.AppendLine("=== RECOMENDAÇÕES DE SEGURANÇA ===");
                relatorio.AppendLine("• Manter o sistema sempre atualizado");
                relatorio.AppendLine("• Verificar logs regularmente");
                relatorio.AppendLine("• Realizar backup dos dados críticos");
                relatorio.AppendLine("• Monitorar tentativas de login falhadas");
                relatorio.AppendLine("• Implementar autenticação de dois fatores");
                relatorio.AppendLine();

                relatorio.AppendLine("=== FIM DO RELATÓRIO ===");

                // Salva o relatório no arquivo
                File.WriteAllText(reportPath, relatorio.ToString(), Encoding.UTF8);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"✅ Relatório de segurança gerado: {reportPath}");
                Console.ForegroundColor = ConsoleColor.White;

                // Registra que o relatório foi gerado
                ErrorLogger.LogSystemEvent("Sistema", $"Relatório de segurança gerado: {reportPath}");
            }
            catch (Exception ex)
            {
                // Se aconteceu erro durante a geração do relatório
                ErrorLogger.LogError("SystemManager", ex);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"❌ Erro ao gerar relatório: {ex.Message}");
                Console.ForegroundColor = ConsoleColor.White;
            }
        }

        // Função que limpa todos os arquivos de log do sistema
        public void LimparLogs()
        {
            // Pede confirmação antes de apagar os logs
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("⚠️ Tem certeza que deseja limpar todos os logs? (S/N): ");
            Console.ForegroundColor = ConsoleColor.White;

            string resposta = Console.ReadLine();

            // Se o usuário confirmou (digitou S ou SIM)
            if (resposta?.ToUpper() == "S" || resposta?.ToUpper() == "SIM")
            {
                ErrorLogger.ClearLogs(); // Chama a função que realmente apaga os logs
            }
            else
            {
                // Se o usuário cancelou a operação
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("⚠️ Operação cancelada.");
                Console.ForegroundColor = ConsoleColor.White;
            }
        }
    }
}
