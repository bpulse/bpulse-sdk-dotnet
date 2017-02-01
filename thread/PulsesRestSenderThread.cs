using bpulse_sdk_csharp.pulseRepository;
using log4net;
using me.bpulse.domain.proto.collector;
using System;
using System.Collections.Generic;
using bpulse_sdk_csharp.rest;
using System.Configuration;
using bpulse_sdk_csharp.bpulsesConstants;
using System.Runtime.CompilerServices;

namespace bpulse_sdk_csharp.thread
{
    /// <summary>
    /// clase que maneja el envio
    /// </summary>
    public class PulsesRestSenderThread : ISyncService
    {
        private PulsesRQ _pulseToSendByRest;
        private IRepository _PulsesRepository;
        private List<string> _keysToDelete;
        private int _tableIndex;
        private string _dbMode;
        private string _username = ConfigurationManager.AppSettings[BPulsesConstants.BPULSE_PROPERTY_USER_CREDENTIALS_USERNAME];
        private string _pass = ConfigurationManager.AppSettings[BPulsesConstants.BPULSE_PROPERTY_USER_CREDENTIALS_PASSWORD];

        private static readonly ILog logger = LogManager.GetLogger("bpulseLogger");

        /// <summary>
        /// Constructor de la clase para iniciar el proceso de envio
        /// </summary>
        /// <param name="pulseToSendByRest">Pulso a enviar</param>
        /// <param name="pulseRepository">repositorio de pulsos</param>
        /// <param name="pKeysToDelete">lista de guid de los pulsos a eliminar</param>
        /// <param name="tabindex"></param>
        /// <param name="dbMode">modo de bd por defecto MEMoria</param>
        public PulsesRestSenderThread(PulsesRQ pulseToSendByRest, IRepository pulseRepository, List<string> pKeysToDelete, int tabindex, string dbMode)
        {
            _pulseToSendByRest = pulseToSendByRest;
            _PulsesRepository = pulseRepository;
            _keysToDelete = pKeysToDelete;
            _tableIndex = tabindex;
            _dbMode = dbMode;
            Run();
        }

        /// <summary>
        /// Metodo que inicializa los procesos 
        /// </summary>
        public void Run()
        {
            RestInvoker sendByRestServices = new RestInvoker();

            bool sended = sendByRestServices.SendByRestService(_username, _pass, _pulseToSendByRest);
            if (sended)
            {
                logger.Info("Enviado Exitosamente " + _PulsesRepository.getDBSize());
                deletePulseKeysProcessedByRest();
                Console.WriteLine("Quedan " + _PulsesRepository.getDBSize() + " en repositorio " + "y se han enviado " + _pulseToSendByRest.Pulse.Count);
            }
            else
            {
                logger.Error("Falla al Enviar " + _PulsesRepository.getDBSize() + "los siguientes pulsos ");
            }


        }

        /// <summary>
        ///  Metodo para eliminar los pulsos que ya han sido procesados del repositorio de pulsos en memoria.
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        private void deletePulseKeysProcessedByRest()
        {
            if (_dbMode.Equals(BPulsesConstants.BPULSE_MEM_PULSES_REPOSITORY))
            {
                foreach (string keyToDelete in _keysToDelete)
                {
                    _PulsesRepository.deleteBpulseRQByKey(keyToDelete);
                }
            }
        }
    }
}