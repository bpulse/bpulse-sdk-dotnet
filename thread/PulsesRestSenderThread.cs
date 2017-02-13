using bpulse_sdk_csharp.bpulsesConstants;
using bpulse_sdk_csharp.pulseRepository;
using bpulse_sdk_csharp.rest;
using log4net;
using me.bpulse.domain.proto.collector;
using System.Collections.Generic;
using System.Configuration;
using System.Runtime.CompilerServices;

namespace bpulse_sdk_csharp.thread
{
    /// <summary>
    ///      clase que maneja el envio
    /// </summary>
    public class PulsesRestSenderThread : ISyncService
    {
        #region Public Constructors

        /// <summary>
        ///      Constructor de la clase para iniciar el proceso de envio
        /// </summary>
        /// <param name="pulseToSendByRest">Pulso a enviar</param>
        /// <param name="pulseRepository">repositorio de pulsos</param>
        /// <param name="pKeysToDelete">lista de guid de los pulsos a eliminar</param>
        /// <param name="dbMode">modo de bd por defecto MEMoria</param>
        public PulsesRestSenderThread(PulsesRQ pulseToSendByRest, IRepository pulseRepository,
            List<string> pKeysToDelete, string dbMode)
        {
            _pulseToSendByRest = pulseToSendByRest;
            _pulsesRepository = pulseRepository;
            _keysToDelete = pKeysToDelete;
            _dbMode = dbMode;
            Run();
        }

        #endregion Public Constructors

        #region Public Methods

        /// <summary>
        ///      Metodo que inicializa los procesos
        /// </summary>
        public void Run()
        {
            var sendByRestServices = new RestInvoker();

            var sended = sendByRestServices.SendByRestService(_username, _pass, _pulseToSendByRest);
            if (sended)
            {
                Logger.Info("Enviado Exitosamente " + _pulsesRepository.GetDbSize());
                DeletePulseKeysProcessedByRest();
            }
            else
            {
                Logger.Error("Falla al Enviar " + _pulsesRepository.GetDbSize() + "los siguientes pulsos ");
            }
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        ///      Metodo para eliminar los pulsos que ya han sido procesados del repositorio de pulsos
        ///      en memoria.
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        private void DeletePulseKeysProcessedByRest()
        {
            if (!_dbMode.Equals(BPulsesConstants.BPULSE_MEM_PULSES_REPOSITORY)) return;
            foreach (var keyToDelete in _keysToDelete)
                _pulsesRepository.DeleteBpulseRqByKey(keyToDelete);
        }

        #endregion Private Methods

        #region Private Fields

        private static readonly ILog Logger = LogManager.GetLogger("bpulseLogger");
        private readonly string _dbMode;
        private readonly List<string> _keysToDelete;

        private readonly string _pass =
            ConfigurationManager.AppSettings[BPulsesConstants.BPULSE_PROPERTY_USER_CREDENTIALS_PASSWORD];

        private readonly IRepository _pulsesRepository;
        private readonly PulsesRQ _pulseToSendByRest;

        private readonly string _username =
            ConfigurationManager.AppSettings[BPulsesConstants.BPULSE_PROPERTY_USER_CREDENTIALS_USERNAME];

        #endregion Private Fields
    }
}