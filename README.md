
# 🛡️ GLOBAL SOLUTIONS C#

Um sistema completo de monitoramento e segurança desenvolvido em C# para demonstrar conceitos de autenticação, logging e detecção de falhas do sistema.

## Integrantes

- Gabriel Oliveira Rodrigues - RM 98565
- Felipe de Campos Mello Arnus - RM 550923
- Bianca Carvalho Dancs Firsoff - RM 551645


## 📋 Características Principais

- ✅ **Sistema de Autenticação Segura** - Login com usuário e senha
- 📊 **Monitoramento do Sistema** - Verificação de status em tempo real
- 📝 **Sistema de Logs Avançado** - Registro detalhado de eventos e erros
- 🔍 **Detecção de Falhas** - Identifica fechamentos anômalos do sistema
- 📋 **Relatórios de Segurança** - Geração automática de relatórios
- 🧹 **Limpeza de Logs** - Gerenciamento de arquivos de log
- ⚠️ **Teste de Falhas** - Simulação de erros para teste do sistema

## 🖥️ Requisitos do Sistema

- .NET Framework 5.0 ou superior
- Windows (testado no Windows 10/11)
- Acesso à área de trabalho para criação de logs

## 📁 Estrutura do Projeto

```
src/
├── Program.cs         # Arquivo principal - ponto de entrada do sistema
├── LoginManager.cs    # Gerencia autenticação e sessões de usuário
├── SystemManager.cs   # Monitora status do sistema e gera relatórios
└── ErrorLogger.cs     # Sistema de logs e detecção de falhas
```

### 📄 Descrição dos Arquivos

**Program.cs**
- Controla o fluxo principal do programa
- Gerencia loops de login e menu principal
- Trata erros críticos do sistema

**LoginManager.cs**
- Autentica usuários no sistema
- Registra tentativas de login (bem-sucedidas e falhadas)
- Calcula duração das sessões

**SystemManager.cs**
- Verifica status do sistema operacional
- Gera relatórios completos de segurança
- Permite limpeza dos arquivos de log

**ErrorLogger.cs**
- Registra todos os erros do sistema
- Detecta fechamentos anômalos (spontaneous shutdown)
- Gerencia múltiplos tipos de log

## ⚙️ Funcionalidades Detalhadas

### 🔐 Sistema de Autenticação
O sistema possui usuários pré-configurados com diferentes níveis de acesso:
- Autenticação por usuário e senha
- Diferentes papéis (Admin, User, Guest)
- Registro de tentativas de login

### 📊 Monitoramento do Sistema
- Informações da máquina e sistema operacional
- Status dos discos e espaço disponível
- Contagem de processos em execução
- Tempo de atividade do sistema

### 🔍 Detecção de Falhas Anômalas
Sistema inovador que detecta se o programa foi fechado incorretamente:
- Cria um "erro falso" ao iniciar uma sessão
- Remove o erro quando o programa é fechado normalmente
- Se o erro permanecer, indica fechamento anômalo

## 👤 Usuários Padrão

| Usuário | Senha  | 
|---------|--------|
| Felipe  | 550923 | 
| admin   | 123456 |
| user    | user123|
| guest   | guest  |

## 🚀 Como Usar

1. **Executar o Sistema**

2. **Fazer Login**
   - Digite um dos usuários da tabela acima
   - Digite a senha correspondente
   - O sistema confirma o login e abre o menu principal

3. **Usar o Menu Principal**
   - **Opção 1**: Logout (volta para tela de login)
   - **Opção 2**: Verificar Status do Sistema
   - **Opção 3**: Gerar Relatório de Segurança
   - **Opção 4**: Teste de Falha Crítica (simula erro)
   - **Opção 5**: Limpar Logs
   - **Opção 6**: Sair do Sistema

## 📂 Arquivos Gerados

O sistema cria automaticamente os seguintes arquivos na área de trabalho:

### Logs de Sistema
- **`error_log.txt`** - Registro de todos os erros do sistema
- **`system_log.txt`** - Log de eventos normais do sistema
- **`login_log.txt`** - Histórico de logins bem-sucedidos e logouts
- **`login_attempts.txt`** - Tentativas de login falhadas

### Relatórios
- **`relatorio_seguranca_YYYYMMDD_HHMMSS.txt`** - Relatórios de segurança com timestamp quando requesitado

## 🔒 Características de Segurança

- **Senhas Ocultas**: As senhas são mascaradas durante a digitação
- **Log de Tentativas**: Todas as tentativas de login são registradas
- **Detecção de Anomalias**: Sistema detecta fechamentos incorretos
- **Auditoria Completa**: Todos os eventos são logados com timestamp
- **Múltiplos Níveis de Usuário**: Diferentes papéis para controle de acesso
