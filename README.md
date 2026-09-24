# USA Despachos (CLUSA) - Sistema de Gestão Aduaneira e Processos

[![Language](https://img.shields.io/badge/Language-C%23%2012-blue.svg)](https://docs.microsoft.com/en-us/dotnet/csharp/)
[![Framework](https://img.shields.io/badge/Framework-.NET%208.0%20(Windows%20Forms)-512BD4.svg)](https://dotnet.microsoft.com/)
[![Database](https://img.shields.io/badge/Database-MongoDB-47A248.svg)](https://www.mongodb.com/)
[![Version](https://img.shields.io/badge/Version-1.5.2.6-green.svg)](https://github.com/MatheusMeloSDEV/USADespachos)
[![License](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

---

## 📋 Sobre o Projeto

O **USA Despachos (CLUSA)** é um sistema desktop de alta performance desenvolvido em **C# / Windows Forms (.NET 8)** para a gestão completa de processos aduaneiros e despacho de comércio exterior. 

O sistema centraliza operações portuárias (Santos e Itajaí), controle de Licenças de Importação (LI), LPCO, DUIMP, Catálogo de Produtos, Órgãos Anuentes, monitoramento de vistorias, faturamento, controle de vencimentos/free time e geração automatizada de relatórios e documentos fiscais em PDF e Excel.

---

## 🎯 Funcionalidades Principais

### 🚢 Gestão de Processos Aduaneiros
- **Operações Multilocal:** Telas dedicadas e otimizadas para processos de **Santos** e **Itajaí**.
- **Ciclo de Vida do Processo:** Acompanhamento de status, CE Mercante, atracação, presença de carga, canal, conferência física e desembaraço.
- **LIs e LPCOs:** Gestão granular de Licenças de Importação e vinculação de múltiplos LPCOs com histórico de exigências e parametrizações.
- **Catálogo de Produtos & Órgãos Anuentes:** Cadastro de mercadorias, NCM, atributos e acompanhamento junto a órgãos reguladores (MAPA, ANVISA, DECEX, etc.).

### 🔍 Controle de Vistorias e DUIMP
- **Fluxo Visual de Vistorias:** Pipeline de movimentação de status (Aguardando Chegada -> Solicitado -> Vistoria Agendada -> Aguardando Deferimento / Laudo -> Deferido / Cancelado).
- **Tratamento Offline / Resiliência:** Fila de operações pendentes com sincronização automática em caso de oscilações de conexão com o banco.
- **Suporte Integrado a DUIMP:** Gestão paralela de vistorias com sincronização visual de cabeçalhos e dados.

### ⏰ Gestão de Vencimentos & Prazos
- **Alertas de Prazos Críticos:** Monitoramento de vencimentos de *Free Time*, FMA, LIs, LPCOs e eventos personalizados.
- **Filtros e Janelas Temporais:** Visão detalhada de processos dentro de faixas de atenção (3 dias, 7 dias, vencidos).
- **Notificações Urgentes:** Sistema de comunicação interna e lembretes para tarefas críticas e urgentes.

### 💰 Faturamento & Documentos Fiscais
- **Capas de Processo:** Geração e exportação automática da folha de rosto/capa de processo com dados consolidados.
- **Faturas e Recibos:** Emissão, cálculo e controle de faturas e recibos aduaneiros.
- **Exportação Multiformato:** Emissão de relatórios em **PDF** de alta fidelidade (via iText) e planilhas **Excel** dinâmicas (via Aspose.Cells e ClosedXML).

### 🤖 Automação & Rastreio (Web Scraping)
- Integrações automatizadas com armadores (ex.: **Maersk**, **CMA CGM**) utilizando **Playwright** para consulta e atualização de status de atracação e contêineres.

### 🔐 Segurança & Auditoria
- **Autenticação e Perfis:** Acesso restrito por usuário com permissões administrativas e personalização de colunas por usuário.
- **Trilha de Auditoria (Logs):** Registro detalhado em banco de dados de todas as criações, alterações, exclusões e acessos a processos.
- **Atualização Automática:** Suporte a updates remotos via **AutoUpdater.NET**.

---

## 🛠️ Tecnologias e Bibliotecas

| Categoria | Tecnologia / Biblioteca |
|---|---|
| **Linguagem & Runtime** | C# 12 / .NET 8.0 (`net8.0-windows`) |
| **Interface Visual** | Windows Forms com ReaLTaiizor (Modern UI) |
| **Banco de Dados** | MongoDB (Driver Oficial `MongoDB.Driver 3.8.0`) |
| **Relatórios & PDF** | iText 9 (`itext`, `itext.bouncy-castle-adapter`) |
| **Manipulação Excel** | Aspose.Cells, ClosedXML |
| **Automação Web** | Microsoft Playwright |
| **Serialização** | Newtonsoft.Json / System.Text.Json |
| **Atualizador** | AutoUpdater.NET Official |

---

## 📁 Estrutura da Solução

```text
USADespachos/
├── CLUSA/                          # Biblioteca de Classes (Regras de Negócio, Modelos e Serviços)
│   ├── Helpers/                    # Utilitários, configurações de e-mail e scraping (CMA, Maersk)
│   ├── Interfaces/                 # Contratos e interfaces base (IEntidadeBase)
│   ├── Models/                     # Entidades de domínio (Processo, Capa, Fatura, Vistoria, etc.)
│   ├── Repositories/               # Camada de persistência MongoDB (RepositorioProcesso, Users, etc.)
│   └── Services/                   # Serviços de negócio (CapaService, Faturamento, Relatórios, etc.)
│
├── Trabalho/                       # Aplicação Principal Windows Forms
│   ├── Imagens/ & Resources/       # Ícones, botões e ativos gráficos da interface
│   ├── Controls/                   # UserControls customizados (LIEditControl, NotificacaoUrgente, etc.)
│   ├── FrmPrincipal.cs             # Formulário MDI / Hub Principal
│   ├── frmSantos.cs & FrmItajaí.cs # Visões operacionais por porto
│   ├── frmModificaProcesso.cs      # Edição e ficha completa de processos
│   ├── FrmVistorias.cs             # Gestão de vistorias e laudos
│   ├── FrmStatusProcessos.cs       # Painel e filtros de status
│   ├── FrmVencimentos.cs           # Painel de controle de prazos e Free Time
│   ├── FrmFinanceiro.cs            # Módulo financeiro e faturamento
│   ├── frmADMIN.cs                 # Administração de usuários e logs de auditoria
│   └── Program.cs                  # Ponto de entrada e rotinas automatizadas
│
└── Trabalho.sln                    # Solução .NET 8
```

---

## 🚀 Como Executar o Projeto

### Pré-requisitos
* [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) instalado
* Acesso a uma instância do **MongoDB** (Local ou Nuvem via MongoDB Atlas)
* [Visual Studio 2022+](https://visualstudio.microsoft.com/) ou editor compatível com .NET (Antigravity IDE / VS Code)

### Passo a Passo

1. **Clone o repositório:**
   ```bash
   git clone https://github.com/MatheusMeloSDEV/USADespachos.git
   cd USADespachos
   ```

2. **Restaurar as dependências NuGet:**
   ```bash
   dotnet restore
   ```

3. **Configurar a Conexão com o Banco:**
   - O sistema suporta variável de ambiente `MONGODB_URI` para deploy/CI ou configuração local em `CLUSA/ConfigDatabaseSettings.cs`.

4. **Compilar e Executar:**
   ```bash
   dotnet build
   dotnet run --project Trabalho/Trabalho.csproj
   ```

---

## 📄 Licença

Este projeto está sob a licença [MIT](LICENSE).

---

<p align="center">
  Desenvolvido por <b>Matheus Melo</b> — <a href="https://github.com/MatheusMeloSDEV">GitHub</a>
</p>