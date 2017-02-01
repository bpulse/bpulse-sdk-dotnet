using bpulse_sdk_csharp.bpulseSender;
using bpulse_sdk_csharp.pulseRepository;
using log4net;
using System;
using System.Collections.Generic;
using me.bpulse.domain.proto.collector;
using bpulse_sdk_csharp.bpulsesConstants;

namespace bpulse_sdk_csharp.timer
{
    /// <summary>
    /// Clase que contiene los procesos del pulsos
    /// </summary>
    public class BpulseRestSenderTimer
    {
        private BpulseSender _bpulseSender;
        private string _dbMode;
        private static readonly ILog logger = LogManager.GetLogger("bpulseLogger");

        public BpulseRestSenderTimer(BpulseSender bpulseSender, string propDBMode)
        {
            _bpulseSender = bpulseSender;
            _dbMode = propDBMode;
            Run();
        }

        /// <summary>
        /// metodo que llama a la ejecucion del envio
        /// </summary>
        private void Run()
        {
            executeRestPulsesSending();
        }
        /// <summary>
        /// metodo que maneja los envios por rest de los pulsos.
        /// </summary>
        private void executeRestPulsesSending()
        {
            try
            {
                //obtain the current key list
                IRepository pulsesRepository = _bpulseSender.getPulsesRepository();
                object[] keys = pulsesRepository.getSortedbpulseRQMapKeys();

                List<string> keyPulseListToDelete = new List<string>();

                PulsesRQ summarizedPulsesRQToSend = null;
                PulsesRQ pulses = new PulsesRQ();
                int totalOfPulsesToSend = BPulsesConstants.COMMON_NUMBER_0;
                int totalOfProcessedPulses = BPulsesConstants.COMMON_NUMBER_0;
                long summarizedTime = BPulsesConstants.COMMON_NUMBER_0;
                long init = BPulsesConstants.COMMON_NUMBER_0;
                long initGets = BPulsesConstants.COMMON_NUMBER_0;
                long summarizeGets = BPulsesConstants.COMMON_NUMBER_0;
                // logger.Info("BEGIN TIMER PULSES PROCESSING..." + " RECORDS READ FROM DB: " + pulsesRepository.countBpulsesRQ() + " IN PROGRESS: " + pulsesRepository.countMarkBpulseKeyInProgress());
                foreach (object keyToProcess in keys)
                {
                    string keyPulse = (string)keyToProcess;
                    //obtain the associated pulse

                    initGets = Calendar.EpochinMilis;
                    PulsesRQ selectedPulsesRQ = null;
                    try
                    {
                        selectedPulsesRQ = pulsesRepository.getBpulseRQByKey(keyPulse);
                    }
                    catch (Exception e)
                    {
                        logger.Error("ERROR TIMER PROCESSING", e);
                        continue;
                    }

                    summarizeGets = summarizeGets + (Calendar.EpochinMilis - initGets);
                    int totalPulsesOfCurrentKey = BPulsesConstants.COMMON_NUMBER_0;
                    if (selectedPulsesRQ != null)
                    {
                        totalPulsesOfCurrentKey = selectedPulsesRQ.Pulse.Count;
                    }
                    if (selectedPulsesRQ != null)
                    {

                        //mark bpulse key as INPROGRESS
                        pulsesRepository.markBpulseKeyInProgress(keyPulse);
                        totalOfProcessedPulses++;
                        //System.out.println("CURRENT NUMBER OF PULSES TO PROCESS: " + cantidadpulsosreal + " " + Calendar.getInstance().getTime() + " GET AVERAGEMILLIS " + summarizeGets + " PULSE PROCESSING AVERAGE TIME: "  + summarizedTime);
                        if (totalOfPulsesToSend + totalPulsesOfCurrentKey <= BPulsesConstants.DEFAULT_TIMER_MAX_NUMBER_GROUPED_PULSES)
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
                            invokeBPulseRestService(_bpulseSender, summarizedPulsesRQToSend, keyPulseListToDelete, 0);
                            summarizedPulsesRQToSend = null;
                            pulses = new PulsesRQ();
                            pulses.Version = selectedPulsesRQ.Version;
                            keyPulseListToDelete = new List<string>();
                            pulses.Pulse.Add(selectedPulsesRQ.Pulse);
                            totalOfPulsesToSend = totalPulsesOfCurrentKey;
                            keyPulseListToDelete.Add(keyPulse);
                        }
                    }
                }

                if (pulses != null && pulses.Pulse.Count > 0)
                {
                    summarizedPulsesRQToSend = pulses;
                    invokeBPulseRestService(_bpulseSender, summarizedPulsesRQToSend, keyPulseListToDelete, 0);
                }
                logger.Info("END TIMER PULSES PROCESSING...PROCESSED PULSES: " + totalOfProcessedPulses);
            }
            catch (Exception e)
            {
                logger.Error("ERROR TIMER PROCESSING", e);
            }
        }
        /// <summary>
        /// Metodo que envia los pulsos seleccionados por Rest
        /// </summary>
        /// <param name="client">cliente </param>
        /// <param name="summarizedPulsesRQToSend"> pulsos a enviar</param>
        /// <param name="keysToDelete">lista de guis con los pulsos a eliminar del repositorio</param>
        /// <param name="tableIndex"></param>
        private void invokeBPulseRestService(BpulseSender client, PulsesRQ summarizedPulsesRQToSend,
                List<string> keysToDelete, int tableIndex)
        {
            client.executeBPulseRestService(summarizedPulsesRQToSend, keysToDelete, tableIndex, _dbMode);
        }
    }
}