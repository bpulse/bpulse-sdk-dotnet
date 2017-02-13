using bpulse_sdk_csharp.bpulsesConstants;
using bpulse_sdk_csharp.bpulseSender;
using log4net;
using me.bpulse.domain.proto.collector;
using System;
using System.Collections.Generic;

namespace bpulse_sdk_csharp.timer
{
    /// <summary>
    ///      Clase que contiene los procesos del pulsos
    /// </summary>
    public class BpulseRestSenderTimer
    {
        #region Public Constructors

        public BpulseRestSenderTimer(BpulseSender bpulseSender, string propDbMode)
        {
            _bpulseSender = bpulseSender;
            _dbMode = propDbMode;
            Run();
        }

        #endregion Public Constructors

        #region Private Fields

        private static readonly ILog Logger = LogManager.GetLogger("bpulseLogger");
        private readonly BpulseSender _bpulseSender;
        private readonly string _dbMode;

        #endregion Private Fields

        #region Private Methods

        /// <summary>
        ///      metodo que maneja los envios por rest de los pulsos.
        /// </summary>
        private void ExecuteRestPulsesSending()
        {
            try
            {
                //obtain the current key list
                var pulsesRepository = _bpulseSender.GetPulsesRepository();
                var keys = pulsesRepository.GetSortedbpulseRqMapKeys();
                var keyPulseListToDelete = new List<string>();
                PulsesRQ summarizedPulsesRQToSend = null;
                var pulses = new PulsesRQ();
                var totalOfPulsesToSend = BPulsesConstants.COMMON_NUMBER_0;
                var totalOfProcessedPulses = BPulsesConstants.COMMON_NUMBER_0;
                long summarizedTime = BPulsesConstants.COMMON_NUMBER_0;
                long init;
                long initGets;
                long summarizeGets = BPulsesConstants.COMMON_NUMBER_0;
                // logger.Info("BEGIN TIMER PULSES PROCESSING..." + " RECORDS READ FROM DB: " +
                // pulsesRepository.countBpulsesRQ() + " IN PROGRESS: " + pulsesRepository.countMarkBpulseKeyInProgress());
                foreach (var keyToProcess in keys)
                {
                    var keyPulse = (string)keyToProcess;
                    //obtain the associated pulse

                    initGets = Calendar.EpochinMilis;
                    PulsesRQ selectedPulsesRQ;
                    try
                    {
                        selectedPulsesRQ = pulsesRepository.GetBpulseRqByKey(keyPulse);
                    }
                    catch (Exception e)
                    {
                        Logger.Error("ERROR TIMER PROCESSING", e);
                        continue;
                    }

                    summarizeGets = summarizeGets + (Calendar.EpochinMilis - initGets);
                    var totalPulsesOfCurrentKey = BPulsesConstants.COMMON_NUMBER_0;
                    if (selectedPulsesRQ != null)
                        totalPulsesOfCurrentKey = selectedPulsesRQ.Pulse.Count;
                    if (selectedPulsesRQ == null) continue;
                    //mark bpulse key as INPROGRESS
                    pulsesRepository.MarkBpulseKeyInProgress(keyPulse);
                    totalOfProcessedPulses++;
                    //System.out.println("CURRENT NUMBER OF PULSES TO PROCESS: " + cantidadpulsosreal + " " + Calendar.getInstance().getTime() + " GET AVERAGEMILLIS " + summarizeGets + " PULSE PROCESSING AVERAGE TIME: "  + summarizedTime);
                    if (totalOfPulsesToSend + totalPulsesOfCurrentKey <=
                        BPulsesConstants.DEFAULT_TIMER_MAX_NUMBER_GROUPED_PULSES)
                    {
                        init = Calendar.EpochinMilis;
                        pulses.Version = selectedPulsesRQ.Version;
                        pulses.Pulse.Add(selectedPulsesRQ.Pulse);

                        totalOfPulsesToSend = totalOfPulsesToSend + totalPulsesOfCurrentKey;
                        keyPulseListToDelete.Add(keyPulse);
                        summarizedTime = summarizedTime + (Calendar.EpochinMilis - init);
                    }
                    else
                    {
                        //prepare to send the pulsesRQ to the RestService
                        summarizedPulsesRQToSend = pulses;
                        InvokeBPulseRestService(_bpulseSender, summarizedPulsesRQToSend, keyPulseListToDelete, 0);
                        summarizedPulsesRQToSend = null;
                        pulses = new PulsesRQ { Version = selectedPulsesRQ.Version };
                        keyPulseListToDelete = new List<string>();
                        pulses.Pulse.Add(selectedPulsesRQ.Pulse);
                        totalOfPulsesToSend = totalPulsesOfCurrentKey;
                        keyPulseListToDelete.Add(keyPulse);
                    }
                }

                if (pulses.Pulse.Count > 0)
                {
                    summarizedPulsesRQToSend = pulses;
                    InvokeBPulseRestService(_bpulseSender, summarizedPulsesRQToSend, keyPulseListToDelete, 0);
                }
                Logger.Info("END TIMER PULSES PROCESSING...PROCESSED PULSES: " + totalOfProcessedPulses);
            }
            catch (Exception e)
            {
                Logger.Error("ERROR TIMER PROCESSING", e);
            }
        }

        /// <summary>
        ///      Metodo que envia los pulsos seleccionados por Rest
        /// </summary>
        /// <param name="client">cliente</param>
        /// <param name="summarizedPulsesRQToSend">pulsos a enviar</param>
        /// <param name="keysToDelete">lista de guis con los pulsos a eliminar del repositorio</param>
        /// <param name="tableIndex"></param>
        private void InvokeBPulseRestService(BpulseSender client, PulsesRQ summarizedPulsesRQToSend,
            List<string> keysToDelete, int tableIndex)
        {
            client.ExecuteBPulseRestService(summarizedPulsesRQToSend, keysToDelete, tableIndex, _dbMode);
        }

        /// <summary>
        ///      metodo que llama a la ejecucion del envio
        /// </summary>
        private void Run()
        {
            ExecuteRestPulsesSending();
        }

        #endregion Private Methods
    }
}