using bpulse_sdk_csharp.pulseRepository;
using log4net;
using me.bpulse.domain.proto.collector;
using System;

namespace bpulse_sdk_csharp.thread
{
    /// <summary>
    /// Clase que administra los envios de los pulsos.
    /// </summary>
    public class BPulseSenderThread : ISyncService
    {

        private PulsesRQ _pulseRQToPersist;
        private IRepository _dbPulsesRepository;
        private static readonly ILog logger = LogManager.GetLogger("bpulseLogger");

        /// <summary>
        /// contructor de la clase
        /// </summary>
        /// <param name="pulseToPersist">pulsos a enviar.</param>
        /// <param name="pulsesRepository">repositorio de pulsos en memoria.</param>
        public BPulseSenderThread(PulsesRQ pulseToPersist, IRepository pulsesRepository)
        {
            _pulseRQToPersist = pulseToPersist;
            _dbPulsesRepository = pulsesRepository;
            Run();
        }

        /// <summary>
        /// metodo que inicializa el proceso.
        /// </summary>
        public void Run()
        {
            try
            {
                sendPulseToRepository();
            }
            catch (Exception ex)
            {

                logger.Error(ex.Message);
            }

        }

        /// <summary>
        /// metodo que llama al repositorio para que guarde los pulsos en memoria.
        /// </summary>
        public void sendPulseToRepository()
        {
            //obtain the current minute.
            _dbPulsesRepository.SavePulse(_pulseRQToPersist);
        }
    }
}