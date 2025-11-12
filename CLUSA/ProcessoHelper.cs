using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLUSA
{
        public static class ProcessoHelper
        {
            /// <summary>
            /// Atualiza automaticamente a CondicaoProcesso baseado nos campos do processo
            /// </summary>
            public static void AtualizarCondicaoProcesso(Processo processo)
            {
                // Garante que Capa existe
                if (processo.Capa == null)
                {
                    processo.Capa = new Capa();
                }

                // 1. AGUARDANDO CE - Entra quando CE estiver em branco
                if (string.IsNullOrWhiteSpace(processo.Capa.CE))
                {
                    processo.CondicaoProcesso = "AguardandoCE";
                    return;
                }

                // 2. PARA REDESTINAR - Entra quando CE preenchido, Sai quando REDESTINAÇÃO check
                if (!string.IsNullOrWhiteSpace(processo.Capa.CE) &&
                    processo.Redestinacao != true)
                {
                    processo.CondicaoProcesso = "ParaRedestinar";
                    return;
                }

                // 3. REDESTINADOS - Entra quando REDESTINAÇÃO check, Sai quando DATA DE ATRACAÇÃO for data atual ou menor
                if (processo.Redestinacao == true &&
                    (!processo.DataDeAtracacao.HasValue || processo.DataDeAtracacao.Value.Date > DateTime.Now.Date))
                {
                    processo.CondicaoProcesso = "Redestinados";
                    return;
                }

                // 4. ATRACADOS SEM PRESENÇA DE CARGA - Entra quando atracado, Sai quando presença de carga check
                if (processo.DataDeAtracacao.HasValue &&
                    processo.DataDeAtracacao.Value.Date <= DateTime.Now.Date &&
                    !processo.PresencaDeCarga)
                {
                    processo.CondicaoProcesso = "AtracadosSemPresencaCarga";
                    return;
                }

                // 5. SITUAÇÃO SIGVIG - Entra quando atracado, Sai quando SIGVIG liberado/selecionado
                if (processo.DataDeAtracacao.HasValue &&
                    processo.DataDeAtracacao.Value.Date <= DateTime.Now.Date &&
                    !processo.SigVig)
                {
                    processo.CondicaoProcesso = "SituacaoSIGVIG";
                    return;
                }

                // 6. ATRACADOS COM PRESENÇA DE CARGA - Entra quando presença de carga check, Sai quando todos LPCOs deferidos
                if (processo.PresencaDeCarga && !VerificarTodosLPCOsDeferidos(processo))
                {
                    processo.CondicaoProcesso = "AtracadosComPresencaCarga";
                    return;
                }

                // 7. DEFERIDOS - Entra quando todos LPCOs deferidos, Sai quando registrar DI
                if (VerificarTodosLPCOsDeferidos(processo) && !processo.DataRegistroDI.HasValue)
                {
                    processo.CondicaoProcesso = "Deferidos";
                    return;
                }

                // 8. SOLICITAR NUMERÁRIO - Entra quando tiver data de embarque, Sai quando numerário solicitado
                if (processo.DataEmbarque.HasValue &&
                    processo.Numerario == false)
            {
                    processo.CondicaoProcesso = "SolicitarNumerario";
                    return;
                }

                // 9. DI/DUIMP PARA DIGITAÇÃO - Entra quando data embarque <= hoje, Sai quando rascunho preenchido
                if (processo.DataEmbarque.HasValue &&
                    processo.DataEmbarque.Value.Date <= DateTime.Now.Date &&
                    string.IsNullOrWhiteSpace(processo.RascunhoDI))
                {
                    processo.CondicaoProcesso = "DIDUIMPParaDigitacao";
                    return;
                }

                // 10. FINALIZADO - Quando tiver registro de DI
                if (processo.DataRegistroDI.HasValue)
                {
                    processo.CondicaoProcesso = "Finalizado";
                    return;
                }

                // Default: se não se encaixar em nenhuma condição, mantém "AguardandoCE"
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
            private static bool VerificarTodosLPCOsDeferidos(Processo processo)
            {
                // Se não tem LI, considera como não deferido
                if (processo.LI == null || !processo.LI.Any())
                    return false;

                // Se tem LI mas nenhuma tem LPCO, considera deferido (não precisa de LPCO)
                bool temAlgumLPCO = processo.LI.Any(li => li.LPCO != null && li.LPCO.Any());
                if (!temAlgumLPCO)
                    return true; // Não tem LPCO para deferir, então está "ok"

                // Verifica cada LPCO
                foreach (var li in processo.LI)
                {
                    if (li.LPCO == null || !li.LPCO.Any())
                        continue;

                    foreach (var lpco in li.LPCO)
                    {
                        // LPCO é considerado deferido se tem data de deferimento
                        bool lpcoEstaDeferido = lpco.DataDeferimentoLPCO.HasValue;

                        // Se LPCO está em exigência, não está deferido
                        if (lpco.EmExigencia)
                            lpcoEstaDeferido = false;

                        // Se encontrar qualquer LPCO não deferido, retorna false
                        if (!lpcoEstaDeferido)
                            return false;
                    }
                }

                // Se chegou aqui, todos os LPCOs estão deferidos
                return true;
            }
        }

}
