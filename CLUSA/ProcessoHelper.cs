using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CLUSA
{
    public static class ProcessoHelper
    {
        /// <summary>
        /// Atualiza automaticamente a CondicaoProcesso baseado nos campos do processo
        /// </summary>
        public static void AtualizarCondicaoProcesso(Processo processo)
        {
            if (processo.Capa == null) processo.Capa = new Capa();

            // --- 1. REGRAS DE AÇÃO IMEDIATA (Prioridade Máxima) ---

            // 10. FINALIZADO
            if (processo.DataRegistroDI.HasValue)
            {
                processo.CondicaoProcesso = "Finalizado";
                return;
            }

            // 9. DI/DUIMP PARA DIGITAÇÃO
            if (processo.DataEmbarque.HasValue &&
                processo.DataEmbarque.Value.Date <= DateTime.Now.Date &&
                (string.IsNullOrWhiteSpace(processo.RascunhoDI) && string.IsNullOrWhiteSpace(processo.DI)))
            {
                processo.CondicaoProcesso = "DIDUIMPParaDigitacao";
                return;
            }

            // 8. SOLICITAR NUMERÁRIO
            if (processo.DataEmbarque.HasValue && processo.Numerario == false)
            {
                processo.CondicaoProcesso = "SolicitarNumerario";
                return;
            }

            // 6. ATRACADOS COM PRESENÇA DE CARGA
            if (processo.PresencaDeCarga)
            {
                processo.CondicaoProcesso = "AtracadosComPresencaCarga";
                return;
            }

            // 5. SITUAÇÃO SIGVIG
            if (processo.DataDeAtracacao.HasValue &&
                processo.DataDeAtracacao.Value.Date <= DateTime.Now.Date &&
                !processo.SigVig)
            {
                processo.CondicaoProcesso = "SituacaoSIGVIG";
                return;
            }

            // 4. ATRACADOS SEM PRESENÇA DE CARGA
            if (processo.DataDeAtracacao.HasValue &&
                processo.DataDeAtracacao.Value.Date <= DateTime.Now.Date)
            {
                processo.CondicaoProcesso = "AtracadosSemPresencaCarga";
                return;
            }

            // 3. REDESTINADOS
            if (processo.Redestinacao == true &&
                (!processo.DataDeAtracacao.HasValue || processo.DataDeAtracacao.Value.Date > DateTime.Now.Date))
            {
                processo.CondicaoProcesso = "Redestinados";
                return;
            }

            // 2. PARA REDESTINAR
            if (!string.IsNullOrWhiteSpace(processo.Capa.CE) && processo.Redestinacao != true)
            {
                processo.CondicaoProcesso = "ParaRedestinar";
                return;
            }

            // 1. DEFAULT
            processo.CondicaoProcesso = "AguardandoCE";
        }

        public static string ObterResumoLPCOs(Processo processo)
        {
            if (processo.LI == null || !processo.LI.Any())
                return "Sem LI/LPCO";

            int totalLPCOs = processo.LI
                .Where(li => li.LPCO != null)
                .Sum(li => li.LPCO.Count);

            if (totalLPCOs == 0)
                return "Sem LPCOs";

            int lpcosDeferidos = processo.LI
                .Where(li => li.LPCO != null)
                .SelectMany(li => li.LPCO)
                .Count(lpco => lpco.DataDeferimentoLPCO.HasValue ||
                   (lpco.MotivoExigencia?.ToUpper() == "DEFERIDO"));

            int lpcosEmExigencia = processo.LI
                .Where(li => li.LPCO != null)
                .SelectMany(li => li.LPCO)
                .Count(lpco => lpco.EmExigencia);

            if (lpcosEmExigencia > 0)
                return $"{lpcosDeferidos}/{totalLPCOs} (⚠{lpcosEmExigencia} em exigência)";

            return $"{lpcosDeferidos}/{totalLPCOs}";
        }

        /// <summary>
        /// Verifica se todos os LPCOs de todas as LIs estão deferidos
        /// </summary>
        public static bool IsDeferido(Processo processo)
        {
            // Se não tem LI, considera não deferido/não aplicável
            if (processo.LI == null || !processo.LI.Any())
                return false;

            // Se tem LI mas não tem LPCO, ou se todos LPCOs estão OK
            // A lógica original era: Se tem LPCO, todos devem estar deferidos.

            bool temAlgumLPCO = processo.LI.Any(li => li.LPCO != null && li.LPCO.Any());
            if (!temAlgumLPCO) return true; // Sem LPCO = "Ok" para Deferido (Conforme sua regra original)

            // Verifica se existe algum RUIM
            bool existePendente = processo.LI
                .SelectMany(li => li.LPCO ?? Enumerable.Empty<LpcoInfo>())
                .Any(lpco => !lpco.DataDeferimentoLPCO.HasValue || lpco.EmExigencia);

            return !existePendente;
        }
    }

}
