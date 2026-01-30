# CLUSA - Sistema de Gestão de Processos

[![Language](https://img.shields.io/badge/language-C%23-blue.svg)](https://docs.microsoft.com/en-us/dotnet/csharp/)
[![Version](https://img.shields.io/badge/version-1.2.2-green.svg)](https://github.com/MatheusMeloSDEV/Trabalho/releases)
[![License](https://img.shields.io/badge/license-MIT-yellow.svg)](LICENSE)

## 📋 Sobre o Projeto

CLUSA é uma aplicação desktop desenvolvida em **C# com Windows Forms** que oferece um sistema completo de gestão de processos administrativos. O projeto foi desenvolvido como parte do **TechChallenge da FIAP** e continua em evolução com novas funcionalidades.

O sistema permite gerenciar agências, processos, faturas, recibos, vencimentos e vistorias com uma interface intuitiva e funcionalidades robustas.

## 🎯 Funcionalidades Principais

### 👥 Gerenciamento de Usuários
- **Login Seguro**: Autenticação de usuários com suporte a diferentes perfis
- **Gerenciamento de Senhas**: Funcionalidade de alteração de senha
- **Controle de Perfis**: Sistema admin para gerenciamento de usuários

### 📊 Gestão de Processos
- **Cadastro de Processos**: Criação e edição de novos processos
- **Status de Processos**: Acompanhamento do status em tempo real
- **Detalhes de Processo**: Visualização detalhada e histórico de alterações
- **Modificação de Dados**: Edição segura de informações de processos

### 🏢 Gestão de Agências
- **Cadastro de Agências**: Gerenciamento de agências parceiras
- **Detalhes de Agência**: Informações completas de cada agência
- **Modificação de Órgão Anuente**: Atualização de dados administrativos

### 💰 Gestão Financeira
- **Gerenciamento de Faturas**: Cadastro e acompanhamento de faturas
- **Controle de Recibos**: Emissão e rastreamento de recibos
- **Relatório Financeiro**: Dashboard financeiro com análises
- **Vencimentos**: Acompanhamento de datas de vencimento

### 📝 Funcionalidades Adicionais
- **Vistorias**: Registro e gerenciamento de vistorias
- **Importação de Dados**: Sistema de importação de dados em lote
- **Notificações Urgentes**: Sistema de notificações para eventos importantes
- **Documentos**: Gerenciamento de documentos associados

## 🛠️ Tecnologias Utilizadas

```
C#
Windows Forms
.NET Framework / .NET Core
SQL Server
Git & GitHub
```

## 📁 Estrutura do Projeto

```
Trabalho/
├── CLUSA/                          # Biblioteca principal com lógica de negócio
│   ├── Agencia.cs                  # Gerenciamento de agências
│   ├── ConfigDatabase.cs           # Configuração de banco de dados
│   ├── DataHelper.cs              # Auxiliar para operações de dados
│   ├── Fatura.cs                  # Gerenciamento de faturas
│   ├── IEntidadeBase.cs           # Interface base para entidades
│   ├── LicencaImportacao.cs       # Importação de licenças
│   ├── LpcoInfo.cs                # Informações de LPCO
│   ├── NotifUrgente.cs            # Notificações urgentes
│   └── ... (outras classes)
│
├── Trabalho/                        # Aplicação Windows Forms
│   ├── FrmPrincipal.cs            # Tela principal
│   ├── frmLogin.cs                # Tela de login
│   ├── DetalhesForm.cs            # Tela de detalhes genérica
│   ├── FrmFinanceiro.cs           # Dashboard financeiro
│   ├── FrmStatusProcessos.cs      # Status dos processos
│   ├── FrmVistorias.cs            # Gerenciamento de vistorias
│   ├── FrmItajaí.cs               # Tela específica (Itajaí)
│   ├── FrmModificaProcesso.cs     # Edição de processos
│   ├── FrmOrgaoAnuente.cs         # Gerenciamento de órgão
│   ├── FrmVencimentos.cs          # Controle de vencimentos
│   ├── NotificacaoUrgente.cs      # Componente de notificação
│   ├── LIEditControl.cs           # Controle customizado de edição
│   ├── LiDisplayControl.cs        # Controle customizado de exibição
│   └── ... (outras forms e controles)
│
├── Icons/                           # Recursos visuais
│   └── logo-removebg-preview.png   # Logo da aplicação
│
├── Trabalho.sln                    # Solution do Visual Studio
└── testEnvironments.json           # Configuração de ambientes de teste
```

## 🚀 Como Usar

### Pré-requisitos
- **.NET Framework 4.7.2+** ou **.NET 6.0+**
- **Visual Studio 2019+** (para desenvolvimento)
- **SQL Server 2019+** (ou SQL Server Express)

### Instalação

1. **Clone o repositório**
   ```bash
   git clone https://github.com/MatheusMeloSDEV/Trabalho.git
   cd Trabalho
   ```

2. **Abra a solution no Visual Studio**
   ```bash
   start Trabalho.sln
   ```

3. **Restaure os pacotes NuGet**
   ```bash
   dotnet restore
   ```

4. **Configure o banco de dados**
   - Edite o arquivo `CLUSA/ConfigDatabase.cs` com suas credenciais
   - Execute as migrações necessárias

5. **Compile e execute**
   - Pressione `F5` no Visual Studio ou execute via linha de comando:
   ```bash
   dotnet run
   ```

## 📝 Notas de Versão

### v1.2.2 (Atual)
- Melhorias na interface de usuário
- Otimizações de performance
- Correção de bugs

### v1.2.0
- Novo sistema de notificações urgentes
- Melhorias no gerenciamento de processos
- Interface redesenhada

### v1.0.1
- Versão inicial estável
- Funcionalidades básicas implementadas

## 👥 Contribuidores

- **MatheusMeloSDEV** - Desenvolvedor

## 📧 Contato

- GitHub: [@MatheusMeloSDEV](https://github.com/MatheusMeloSDEV)
- Email: [matheusmvsj@gmail.com]

## 📄 Licença

Este projeto está licenciado sob a Licença MIT - veja o arquivo [LICENSE](LICENSE) para detalhes.
---

