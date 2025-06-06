
using System;
using System.Collections.Generic;
using System.IO;

namespace ProtecaoCibernetica
{
    public class LoginManager
    {
        // Propriedades para controlar o usuário logado
        public string CurrentUser { get; private set; }
        private DateTime LoginTime { get; set; }

        // Lista de usuários válidos do sistema (normalmente seria em banco de dados)
        private readonly Dictionary<string, UserInfo> usuarios = new Dictionary<string, UserInfo>
        {
            { "Felipe", new UserInfo { Password = "550923", Role = "Admin", LastLogin = DateTime.MinValue } },
            { "admin", new UserInfo { Password = "123456", Role = "Admin", LastLogin = DateTime.MinValue } },
            { "user", new UserInfo { Password = "user123", Role = "User", LastLogin = DateTime.MinValue } },
            { "guest", new UserInfo { Password = "guest", Role = "Guest", LastLogin = DateTime.MinValue } }
        };

        // Função que tenta fazer login com usuário e senha
        public bool AttemptLogin(string username, string password)
        {
            try
            {
                // Verifica se o usuário existe e se a senha está correta
                if (usuarios.ContainsKey(username) && usuarios[username].Password == password)
                {
                    // Login bem-sucedido - salva as informações
                    CurrentUser = username;
                    LoginTime = DateTime.Now;
                    usuarios[username].LastLogin = LoginTime;

                    // Registra o login bem-sucedido no arquivo de log
                    LogSuccessfulLogin();
                    return true;
                }
                else
                {
                    // Login falhou - registra a tentativa
                    LogFailedLoginAttempt(username);
                    return false;
                }
            }
            catch (Exception ex)
            {
                // Se aconteceu algum erro durante o login
                ErrorLogger.LogError("LoginManager", ex);
                return false;
            }
        }

        // Função que faz logout do usuário atual
        public void Logout()
        {
            if (!string.IsNullOrEmpty(CurrentUser))
            {
                // Registra o logout no arquivo de log
                LogLogout();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"✅ Logout realizado com sucesso para {CurrentUser}.");
                Console.ForegroundColor = ConsoleColor.White;
                CurrentUser = null; // Remove o usuário logado
            }
        }

        // Função que retorna as informações do usuário atual
        public UserInfo GetCurrentUserInfo()
        {
            if (!string.IsNullOrEmpty(CurrentUser) && usuarios.ContainsKey(CurrentUser))
            {
                return usuarios[CurrentUser];
            }
            return null;
        }

        // Função que calcula há quanto tempo o usuário está logado
        public TimeSpan GetSessionDuration()
        {
            if (!string.IsNullOrEmpty(CurrentUser))
            {
                return DateTime.Now - LoginTime;
            }
            return TimeSpan.Zero;
        }

        // Função que registra tentativas de login que falharam
        private void LogFailedLoginAttempt(string username)
        {
            try
            {
                // Caminho para o arquivo de tentativas de login
                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string logFilePath = Path.Combine(desktopPath, "login_attempts.txt");

                // Monta a mensagem de log com as informações da tentativa
                string logMessage = $"[FALHA] Usuário: {username} | Data: {DateTime.Now:dd/MM/yyyy HH:mm:ss} | IP: {GetLocalIP()}";

                // Adiciona a linha no final do arquivo
                File.AppendAllText(logFilePath, logMessage + Environment.NewLine);
            }
            catch (Exception ex)
            {
                // Se não conseguiu escrever no arquivo, registra o erro
                ErrorLogger.LogError("LoginManager", ex);
            }
        }

        // Função que registra logins bem-sucedidos
        private void LogSuccessfulLogin()
        {
            try
            {
                // Caminho para o arquivo de log de logins
                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string logFilePath = Path.Combine(desktopPath, "login_log.txt");

                // Pega as informações do usuário atual
                UserInfo currentUserInfo = GetCurrentUserInfo();

                // Monta a mensagem de log com as informações do login
                string logMessage = $"[SUCESSO] Usuário: {CurrentUser} | Role: {currentUserInfo.Role} | Login: {DateTime.Now:dd/MM/yyyy HH:mm:ss} | IP: {GetLocalIP()}";

                // Adiciona a linha no final do arquivo
                File.AppendAllText(logFilePath, logMessage + Environment.NewLine);
            }
            catch (Exception ex)
            {
                // Se não conseguiu escrever no arquivo, registra o erro
                ErrorLogger.LogError("LoginManager", ex);
            }
        }

        // Função que registra quando o usuário faz logout
        private void LogLogout()
        {
            try
            {
                // Caminho para o arquivo de log de logins
                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string logFilePath = Path.Combine(desktopPath, "login_log.txt");

                // Calcula quanto tempo o usuário ficou logado
                TimeSpan sessionDuration = GetSessionDuration();

                // Monta a mensagem de log com as informações do logout
                string logMessage = $"[LOGOUT] Usuário: {CurrentUser} | Logout: {DateTime.Now:dd/MM/yyyy HH:mm:ss} | Duração da sessão: {sessionDuration:hh\\:mm\\:ss}";

                // Adiciona a linha no final do arquivo
                File.AppendAllText(logFilePath, logMessage + Environment.NewLine);
            }
            catch (Exception ex)
            {
                // Se não conseguiu escrever no arquivo, registra o erro
                ErrorLogger.LogError("LoginManager", ex);
            }
        }

        // Função que tenta pegar o IP local da máquina
        private string GetLocalIP()
        {
            try
            {
                return System.Net.Dns.GetHostName();
            }
            catch
            {
                return "Unknown"; // Se não conseguir, retorna "Unknown"
            }
        }
    }

    // Classe que representa as informações de um usuário
    public class UserInfo
    {
        public string Password { get; set; }    // Senha do usuário
        public string Role { get; set; }        // Função do usuário (Admin, User, etc.)
        public DateTime LastLogin { get; set; } // Último login do usuário
    }
}
