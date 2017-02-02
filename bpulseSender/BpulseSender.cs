using bpulse_sdk_csharp.bpulsesConstants;
using bpulse_sdk_csharp.pulseRepository;
using bpulse_sdk_csharp.thread;
using log4net;
using me.bpulse.domain.proto.collector;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Runtime.CompilerServices;

namespace bpulse_sdk_csharp.bpulseSender
{
    /// <summary>
    /// Clase que prerapa los pulsos para almacenarlos.
    /// </summary>
    public class BpulseSender
    {
        private static BpulseSender instance = null;
        private static readonly ILog logger = LogManager.GetLogger("bpulseLogger");
        private string propDBMode;
        private long maxMemNumPulses;
        private IRepository pulsesRepository;

        PulsesRQ pulses = new PulsesRQ();

        /// <summary>
        /// Constructor de la clase que incializa los parametros del App.config
        /// </summary>
        internal BpulseSender()
        {
            string propMaxNumberRQsToReadFromDB = ConfigurationManager.AppSettings[BPulsesConstants.BPULSE_PROPERTY_MAX_NUMBER_PULSES_TO_PROCESS_TIMER];

            int maxNumberRQsToReadFromDB = BPulsesConstants.COMMON_NUMBER_0;

            if (propMaxNumberRQsToReadFromDB != null)
            {
                maxNumberRQsToReadFromDB = int.Parse(propMaxNumberRQsToReadFromDB);
            }
            else
            {
                maxNumberRQsToReadFromDB = BPulsesConstants.COMMON_NUMBER_180000;
            }

            propDBMode = ConfigurationManager.AppSettings[BPulsesConstants.BPULSE_PROPERTY_PULSES_REPOSITORY_MODE];
            string propMemMaxNumPulses = ConfigurationManager.AppSettings[BPulsesConstants.BPULSE_PROPERTY_MEM_PULSES_REPOSITORY_MAX_NUM_PULSES];

            if (propDBMode == null)
            {
                propDBMode = BPulsesConstants.BPULSE_MEM_PULSES_REPOSITORY;
            }
            if (propMemMaxNumPulses != null)
            {
                maxMemNumPulses = long.Parse(propMemMaxNumPulses);
            }
            else
            {
                maxMemNumPulses = BPulsesConstants.BPULSE_MAX_MEM_NUM_PULSES;
            }

            if (propDBMode.Equals(BPulsesConstants.BPULSE_MEM_PULSES_REPOSITORY))
            {
                logger.Info("InMemoryPulsesRepository instance");
                pulsesRepository = new InMemoryRepository(maxNumberRQsToReadFromDB);
            }
        }

        /// <summary>
        /// metodo que retorna la intancia de BpulseSender a modo de singleton.
        /// </summary>
        /// <returns>retorna la instancia</returns>
        internal static BpulseSender GetInstance()
        {
            if (instance == null)
            {
                instance = new BpulseSender();
            }

            return instance;
        }

        /// <summary>
        /// metodo que envia los pulsos a ser procesados
        /// </summary>
        /// <param name="pulse"></param>
        /// <returns>retorna un string si es o no exitosa</returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        internal string SendPulse(PulsesRQ pulse)
        {
            Random random = new Random();
            long additionalPulseId = random.Next();
            long maxRepositorySize = 0;

            if (propDBMode.Equals(BPulsesConstants.BPULSE_MEM_PULSES_REPOSITORY))
            {
                maxRepositorySize = maxMemNumPulses;
            }

            if (maxRepositorySize > pulsesRepository.getDBSize())
            {
                BPulseSenderThread thread = new BPulseSenderThread(pulse, pulsesRepository);

                return BPulsesConstants.BPULSE_SUCCESSFUL_RESPONSE;
            }
            else
            {
                logger.Error("FAILED SEND PULSE: THE MAX PULSESDB SIZE WAS REACHED, MAXIMUM: " + maxRepositorySize + ", CURRENT SIZE: " + this.pulsesRepository.getDBSize());
                return BPulsesConstants.BPULSE_FAILED_RESPONSE;
            }
        }

        /// <summary>
        /// MEtodo que ejecuta el serivico REST del pulso
        /// </summary>
        /// <param name="summarizedPulsesRQToSend"></param>
        /// <param name="keysToDelete"></param>
        /// <param name="tableIndex"></param>
        /// <param name="_dbMode"></param>
        internal void executeBPulseRestService(PulsesRQ summarizedPulsesRQToSend, List<string> keysToDelete, int tableIndex, string _dbMode)
        {
            try
            {
                var restInvoker = new PulsesRestSenderThread(summarizedPulsesRQToSend, pulsesRepository, keysToDelete, tableIndex, _dbMode);
            }
            catch (Exception ex)
            {
                logger.Error("No se pudo realizar el envio de Pulso " + ex.Message);
            }

        }

        /// <summary>
        /// Metodo para obtener los pulsos del repositorio.
        /// </summary>
        /// <returns>retorna el repositorio de pulsos.</returns>
        internal IRepository getPulsesRepository()
        {
            return pulsesRepository;
        }
    }
}