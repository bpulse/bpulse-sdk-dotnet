using System;
using System.Collections.Generic;
using System.Configuration;
using System.Runtime.CompilerServices;
using bpulse_sdk_csharp.bpulsesConstants;
using bpulse_sdk_csharp.pulseRepository;
using bpulse_sdk_csharp.thread;
using log4net;
using me.bpulse.domain.proto.collector;

namespace bpulse_sdk_csharp.bpulseSender
{
    /// <summary>
    ///      Clase que prerapa los pulsos para almacenarlos.
    /// </summary>
    public class BpulseSender
    {
        #region Internal Constructors

        /// <summary>
        ///      Constructor de la clase que incializa los parametros del App.config
        /// </summary>
        internal BpulseSender()
        {
            var propMaxNumberRQsToReadFromDb =
                ConfigurationManager.AppSettings[BPulsesConstants.BPULSE_PROPERTY_MAX_NUMBER_PULSES_TO_PROCESS_TIMER];

            var maxNumberRQsToReadFromDb = propMaxNumberRQsToReadFromDb != null
                ? int.Parse(propMaxNumberRQsToReadFromDb)
                : BPulsesConstants.COMMON_NUMBER_180000;

            _propDbMode = ConfigurationManager.AppSettings[BPulsesConstants.BPULSE_PROPERTY_PULSES_REPOSITORY_MODE];
            var propMemMaxNumPulses =
                ConfigurationManager.AppSettings[BPulsesConstants.BPULSE_PROPERTY_MEM_PULSES_REPOSITORY_MAX_NUM_PULSES];

            if (_propDbMode == null)
                _propDbMode = BPulsesConstants.BPULSE_MEM_PULSES_REPOSITORY;
            _maxMemNumPulses = propMemMaxNumPulses != null
                ? long.Parse(propMemMaxNumPulses)
                : BPulsesConstants.BPULSE_MAX_MEM_NUM_PULSES;

            if (_propDbMode.Equals(BPulsesConstants.BPULSE_MEM_PULSES_REPOSITORY))
            {
                Logger.Info("InMemoryPulsesRepository instance");
                _pulsesRepository = new InMemoryRepository(maxNumberRQsToReadFromDb);
            }
        }

        #endregion Internal Constructors

        #region Private Fields

        private static readonly ILog Logger = LogManager.GetLogger("bpulseLogger");
        private static BpulseSender _instance;
        private readonly long _maxMemNumPulses;
        private readonly string _propDbMode;
        private readonly IRepository _pulsesRepository;
        private PulsesRQ _pulses = new PulsesRQ();

        #endregion Private Fields

        #region Internal Methods

        /// <summary>
        ///      metodo que retorna la intancia de BpulseSender a modo de singleton.
        /// </summary>
        /// <returns>retorna la instancia</returns>
        internal static BpulseSender GetInstance()
        {
            return _instance ?? (_instance = new BpulseSender());
        }

        /// <summary>
        ///      MEtodo que ejecuta el serivico REST del pulso
        /// </summary>
        /// <param name="summarizedPulsesRQToSend"></param>
        /// <param name="keysToDelete"></param>
        /// <param name="tableIndex"></param>
        /// <param name="dbMode"></param>
        internal void ExecuteBPulseRestService(PulsesRQ summarizedPulsesRQToSend, List<string> keysToDelete,
            int tableIndex, string dbMode)
        {
            try
            {
                var pulsesRestSenderThread = new PulsesRestSenderThread(summarizedPulsesRQToSend, _pulsesRepository,
                    keysToDelete, dbMode);
            }
            catch (Exception ex)
            {
                Logger.Error("No se pudo realizar el envio de Pulso " + ex.Message);
            }
        }

        /// <summary>
        ///      Metodo para obtener los pulsos del repositorio.
        /// </summary>
        /// <returns>retorna el repositorio de pulsos.</returns>
        internal IRepository GetPulsesRepository()
        {
            return _pulsesRepository;
        }

        /// <summary>
        ///      metodo que envia los pulsos a ser procesados
        /// </summary>
        /// <param name="pulse"></param>
        /// <returns>retorna un string si es o no exitosa</returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        internal string SendPulse(PulsesRQ pulse)
        {
            long maxRepositorySize = 0;

            if (_propDbMode.Equals(BPulsesConstants.BPULSE_MEM_PULSES_REPOSITORY))
                maxRepositorySize = _maxMemNumPulses;

            if (maxRepositorySize > _pulsesRepository.GetDbSize())
            {
                var thread = new BPulseSenderThread(pulse, _pulsesRepository);

                return BPulsesConstants.BPULSE_SUCCESSFUL_RESPONSE;
            }
            Logger.Error("FAILED SEND PULSE: THE MAX PULSESDB SIZE WAS REACHED, MAXIMUM: " + maxRepositorySize +
                         ", CURRENT SIZE: " + _pulsesRepository.GetDbSize());
            return BPulsesConstants.BPULSE_FAILED_RESPONSE;
        }

        #endregion Internal Methods
    }
}