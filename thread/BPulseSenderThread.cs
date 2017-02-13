using bpulse_sdk_csharp.pulseRepository;
using log4net;
using me.bpulse.domain.proto.collector;
using System;

namespace bpulse_sdk_csharp.thread
{
    /// <summary>
    ///      Clase que administra los envios de los pulsos.
    /// </summary>
    public class BPulseSenderThread : ISyncService
    {
        #region Public Constructors

        /// <summary>
        ///      contructor de la clase
        /// </summary>
        /// <param name="pulseToPersist">pulsos a enviar.</param>
        /// <param name="pulsesRepository">repositorio de pulsos en memoria.</param>
        public BPulseSenderThread(PulsesRQ pulseToPersist, IRepository pulsesRepository)
        {
            _pulseRQToPersist = pulseToPersist;
            _dbPulsesRepository = pulsesRepository;
            Run();
        }

        #endregion Public Constructors

        #region Private Fields

        private static readonly ILog Logger = LogManager.GetLogger("bpulseLogger");
        private readonly IRepository _dbPulsesRepository;
        private readonly PulsesRQ _pulseRQToPersist;

        #endregion Private Fields

        #region Public Methods

        /// <summary>
        ///      metodo que inicializa el proceso.
        /// </summary>
        public void Run()
        {
            try
            {
                SendPulseToRepository();
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
            }
        }

        /// <summary>
        ///      metodo que llama al repositorio para que guarde los pulsos en memoria.
        /// </summary>
        public void SendPulseToRepository()
        {
            //obtain the current minute.
            _dbPulsesRepository.SavePulse(_pulseRQToPersist);
        }

        #endregion Public Methods
    }
}