
using System;
using System.IO;
using System.Text;

namespace ProtecaoCibernetica
{
    public static class ErrorLogger
    {
        // Caminhos dos arquivos de log (todos salvos na área de trabalho)
        private static readonly string DesktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        private static readonly string ErrorLogPath = Path.Combine(DesktopPath, "error_log.txt");
        private static readonly string SystemLogPath = Path.Combine(DesktopPath, "system_log.txt");

        // ID da sessão atual (para identificar fechamentos anômalos)
        private static string currentSessionId = "";

        // Função que registra o início de uma sessão de usuário
        // IMPORTANTE: Cria um "erro falso" que será removido se o sistema fechar normalmente
        public static void LogSessionStart(string username)
        {
            try
            {
                // Gera um ID único para esta sessão (só os primeiros 8 caracteres)
                currentSessionId = Guid.NewGuid().ToString("N")[..8];

                // Cria uma exceção fictícia que representa "fechamento espontâneo"
                // Se o sistema fechar normalmente, este erro será removido
                // Se o sistema fechar anormalmente, este erro permanecerá no log
                var spontaneousShutdownException = new Exception($"Spontaneous shutdown - Sessão ID: {currentSessionId}");
                string sessionStartMessage = FormatErrorMessage(username, spontaneousShutdownException);

                // Adiciona o "erro" no arquivo de log
                File.AppendAllText(ErrorLogPath, sessionStartMessage + Environment.NewLine, Encoding.UTF8);

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("📝 Erro de 'spontaneous shutdown' registrado (será removido ao sair corretamente)");
                Console.ForegroundColor = ConsoleColor.White;

                // Também registra no log normal do sistema
                LogSystemEvent(username, $"Sessão iniciada - ID: {currentSessionId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao registrar início da sessão: {ex.Message}");
            }
        }

        // Função que marca o fim normal de uma sessão
        // Remove o "erro falso" de spontaneous shutdown se encontrar
        public static void LogSessionEnd(string username)
        {
            try
            {
                // Verifica se existe uma sessão para remover
                if (string.IsNullOrEmpty(currentSessionId))
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("⚠️ Nenhuma sessão ativa para remover");
                    Console.ForegroundColor = ConsoleColor.White;
                    return;
                }

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"🔍 Procurando sessão ID: {currentSessionId} no log...");
                Console.ForegroundColor = ConsoleColor.White;

                // Lê todo o conteúdo do arquivo de log de erros
                string logContent = "";
                if (File.Exists(ErrorLogPath))
                {
                    logContent = File.ReadAllText(ErrorLogPath, Encoding.UTF8);
                }

                if (string.IsNullOrEmpty(logContent))
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("⚠️ Log de erros está vazio");
                    Console.ForegroundColor = ConsoleColor.White;
                    currentSessionId = "";
                    return;
                }

                // Procura pelo erro específico desta sessão
                string sessionPattern = $"Spontaneous shutdown - Sessão ID: {currentSessionId}";
                int sessionIndex = logContent.IndexOf(sessionPattern);

                if (sessionIndex < 0)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"⚠️ Sessão ID {currentSessionId} não encontrada no log");
                    Console.ForegroundColor = ConsoleColor.White;
                    currentSessionId = "";
                    return;
                }

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"✅ Sessão ID {currentSessionId} encontrada no log");
                Console.ForegroundColor = ConsoleColor.White;

                // Encontra onde começa a seção de erro completa
                string errorStartMarker = "=== ERRO REGISTRADO ===";
                int errorStartIndex = logContent.LastIndexOf(errorStartMarker, sessionIndex);

                if (errorStartIndex < 0)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("❌ Início da seção de erro não encontrado");
                    Console.ForegroundColor = ConsoleColor.White;
                    currentSessionId = "";
                    return;
                }

                // Encontra onde termina a seção de erro
                string errorEndMarker = "========================";
                int errorEndIndex = logContent.IndexOf(errorEndMarker, sessionIndex);

                if (errorEndIndex < 0)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("❌ Fim da seção de erro não encontrado");
                    Console.ForegroundColor = ConsoleColor.White;
                    currentSessionId = "";
                    return;
                }

                // Calcula exatamente o que deve ser removido
                int startToRemove = errorStartIndex;
                int endToRemove = errorEndIndex + errorEndMarker.Length;

                // Inclui quebras de linha que vêm depois do marcador final
                while (endToRemove < logContent.Length &&
                       (logContent[endToRemove] == '\r' || logContent[endToRemove] == '\n'))
                {
                    endToRemove++;
                }

                // Remove a seção do erro do conteúdo
                string beforeError = logContent.Substring(0, startToRemove);
                string afterError = endToRemove < logContent.Length ? logContent.Substring(endToRemove) : "";

                // Reescreve o arquivo sem o registro da sessão
                File.WriteAllText(ErrorLogPath, beforeError + afterError, Encoding.UTF8);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"✅ Erro de spontaneous shutdown da sessão ID {currentSessionId} removido (fechamento correto)");
                Console.ForegroundColor = ConsoleColor.White;

                // Registra no log normal que a sessão foi encerrada corretamente
                LogSystemEvent(username, $"Sessão encerrada corretamente - ID: {currentSessionId}");
                currentSessionId = "";
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"❌ Erro ao remover registro da sessão: {ex.Message}");
                Console.ForegroundColor = ConsoleColor.White;
                LogError("Sistema", ex);
            }
        }

        // Função que verifica se houve fechamento anômalo na última execução
        // Procura por erros de "spontaneous shutdown" que não foram removidos
        public static bool CheckForAbnormalShutdown()
        {
            try
            {
                if (File.Exists(ErrorLogPath))
                {
                    string logContent = File.ReadAllText(ErrorLogPath, Encoding.UTF8);

                    // Se encontrar erros de spontaneous shutdown, significa que houve fechamento anômalo
                    if (logContent.Contains("Spontaneous shutdown - Sessão ID:"))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("⚠️ AVISO: Detectados fechamentos anômalos no log!");
                        Console.WriteLine("📝 Registros de 'spontaneous shutdown' encontrados.");
                        Console.ForegroundColor = ConsoleColor.White;

                        LogSystemEvent("Sistema", "Fechamento anômalo detectado - erros de spontaneous shutdown no log");
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao verificar fechamento anômalo: {ex.Message}");
                return false;
            }
        }

        // Função que registra erros que acontecem no sistema
        public static void LogError(string user, Exception ex)
        {
            try
            {
                // Formata a mensagem de erro com todas as informações importantes
                string logMessage = FormatErrorMessage(user, ex);
                File.AppendAllText(ErrorLogPath, logMessage + Environment.NewLine, Encoding.UTF8);

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"❌ Erro registrado: {ex.Message}");
                Console.ForegroundColor = ConsoleColor.White;

                // Também registra uma versão simplificada no log do sistema
                LogSystemEvent(user, $"ERRO: {ex.Message}");
            }
            catch (Exception logEx)
            {
                // Se não conseguir escrever no log principal, tenta arquivo de emergência
                try
                {
                    string criticalLogPath = Path.Combine(DesktopPath, "critical_error.txt");
                    File.WriteAllText(criticalLogPath, $"ERRO CRÍTICO: {logEx.Message} | ERRO ORIGINAL: {ex.Message}");
                }
                catch
                {
                    // Se nem isso funcionar, pelo menos mostra no console
                    Console.WriteLine($"ERRO CRÍTICO DE LOGGING: {logEx.Message}");
                }
            }
        }

        // Função que registra eventos normais do sistema (não são erros)
        public static void LogSystemEvent(string user, string eventDescription)
        {
            try
            {
                string logMessage = FormatSystemEvent(user, eventDescription);
                File.AppendAllText(SystemLogPath, logMessage + Environment.NewLine, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Falha ao registrar evento do sistema: {ex.Message}");
            }
        }

        // Função que lê e retorna o conteúdo do arquivo de log de erros
        public static string GetErrorLogContent()
        {
            try
            {
                if (File.Exists(ErrorLogPath))
                {
                    return File.ReadAllText(ErrorLogPath, Encoding.UTF8);
                }
                return "Nenhum erro registrado.";
            }
            catch (Exception ex)
            {
                return $"Erro ao ler logs: {ex.Message}";
            }
        }

        // Função que lê e retorna o conteúdo do arquivo de log do sistema
        public static string GetSystemLogContent()
        {
            try
            {
                if (File.Exists(SystemLogPath))
                {
                    return File.ReadAllText(SystemLogPath, Encoding.UTF8);
                }
                return "Nenhum evento do sistema registrado.";
            }
            catch (Exception ex)
            {
                return $"Erro ao ler logs do sistema: {ex.Message}";
            }
        }

        // Função que apaga todos os arquivos de log do sistema
        public static void ClearLogs()
        {
            try
            {
                // Lista de todos os arquivos de log que devem ser apagados
                if (File.Exists(ErrorLogPath))
                    File.Delete(ErrorLogPath);

                if (File.Exists(SystemLogPath))
                    File.Delete(SystemLogPath);

                string loginLogPath = Path.Combine(DesktopPath, "login_log.txt");
                if (File.Exists(loginLogPath))
                    File.Delete(loginLogPath);

                string attemptsLogPath = Path.Combine(DesktopPath, "login_attempts.txt");
                if (File.Exists(attemptsLogPath))
                    File.Delete(attemptsLogPath);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("✅ Todos os logs foram limpos com sucesso.");
                Console.ForegroundColor = ConsoleColor.White;

                // Registra que os logs foram limpos (irônico, mas importante para auditoria)
                LogSystemEvent("Sistema", "Logs limpos pelo usuário");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"❌ Erro ao limpar logs: {ex.Message}");
                Console.ForegroundColor = ConsoleColor.White;
            }
        }

        // Função que formata uma mensagem de erro com todas as informações
        private static string FormatErrorMessage(string user, Exception ex)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("=== ERRO REGISTRADO ===");
            sb.AppendLine($"Usuário: {user}");
            sb.AppendLine($"Data/Hora: {DateTime.Now:dd/MM/yyyy HH:mm:ss}");
            sb.AppendLine($"Tipo de Erro: {ex.GetType().Name}");
            sb.AppendLine($"Mensagem: {ex.Message}");
            sb.AppendLine($"Stack Trace: {ex.StackTrace}");
            sb.AppendLine($"Máquina: {Environment.MachineName}");
            sb.AppendLine($"SO: {Environment.OSVersion}");
            sb.AppendLine("========================");

            return sb.ToString();
        }

        // Função que formata um evento normal do sistema
        private static string FormatSystemEvent(string user, string eventDescription)
        {
            return $"[{DateTime.Now:dd/MM/yyyy HH:mm:ss}] [{user}] {eventDescription}";
        }
    }
}
