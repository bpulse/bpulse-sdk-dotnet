using System;
using me.bpulse.domain.proto.collector;
using System.Text;
using System.IO;
using System.Collections.Generic;
using bpulse_sdk_csharp.compress;
using log4net;
using bpulse_sdk_csharp.dto;
using bpulse_sdk_csharp.bpulseSender;
using bpulse_sdk_csharp.pulseRepository;
using System.Configuration;
using bpulse_sdk_csharp.bpulsesConstants;
using System.Timers;
using bpulse_sdk_csharp.timer;
using System.Runtime.CompilerServices;

namespace bpulse_sdk_csharp.bpulseClient
{
    /// <summary>
    /// Clase donde se inicializa el timer y la logica del conector.
    /// </summary>
    public class BPulseCsharpClient
    {
        private static bool isStarted = false;
        private static readonly ILog logger = LogManager.GetLogger("bpulseLogger");
        private BpulseSender bpulseSender;
        private IRepository pulseRepository;
        private string propDBMode;
        private static BPulseCsharpClient instance;

        /// <summary>
        /// Constructor de la clase donde se obtiene la instancia de los repositorios y se inicia el proceso.
        /// </summary>
        public BPulseCsharpClient()
        {
            try
            {
                logger.Info("GET INSTANCE BpulseCsharpClient...");
                bpulseSender = BpulseSender.GetInstance();
                start();
                logger.Info("GET INSTANCE BpulseJavaClient SUCCESSFUL.");
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// Metodo para obtener la instancia del cliente del conector, a modo de Singleton.
        /// </summary>
        /// <returns>Retorna la instancia inicializada, o si no, instancia una nueva.</returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public BPulseCsharpClient GetInstance()
        {
            if (instance == null)
            {
                instance = new BPulseCsharpClient();
            }
            return instance;
        }

        /// <summary>
        /// metodo de inicio del cliente.
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void start()
        {
            try
            {
                logger.Info("INIT BPULSE TIMER...");

                if (!isStarted)
                {
                    propDBMode = BPulsesConstants.BPULSE_MEM_PULSES_REPOSITORY;

                    Timer tim = new Timer();

                    tim.Enabled = true;

                    string periodInMinutesNextExecutionTimer = ConfigurationManager.AppSettings[BPulsesConstants.BPULSE_PROPERTY_USER_TIMER_DELAY];
                    long periodInMillis = BPulsesConstants.COMMON_NUMBER_0;
                    if (periodInMinutesNextExecutionTimer == null)
                    {
                        periodInMillis = BPulsesConstants.DEFAULT_TIMER_MIN_DELAY;
                    }
                    else
                    {
                        periodInMillis = long.Parse(periodInMinutesNextExecutionTimer) * BPulsesConstants.COMMON_NUMBER_60 * BPulsesConstants.COMMON_NUMBER_1000;
                    }

                    tim.Interval = periodInMillis;
                    tim.Elapsed += BPulseRestSenderTask;

                    isStarted = true;
                }
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        /// <summary>
        /// Metodo de tipo Timer que se ejecutara cada cierto tiempo segun el tiempo en AppConfig periodInMinutesNextExecTimer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BPulseRestSenderTask(object sender, ElapsedEventArgs e)
        {
            BpulseRestSenderTimer bpST = new BpulseRestSenderTimer(bpulseSender, propDBMode);

        }

        /// <summary>
        /// Metodo para enviar pulsos. este metodo almacena los pulsos a enviar en memoria hasta llenar el maximo programado.
        /// </summary>
        /// <param name="pulse">Pulso construido por el cliente</param>
        /// <returns>regresa un string de exitoso o no exitoso.</returns>
        public string SendPulse(PulsesRQ pulse)
        {
            if (isStarted)
            {
                return bpulseSender.SendPulse(pulse);
            }
            else
            {
                throw new Exception("Error sending pulses: BPulseCsharpClient is not started yet. Please invoke the start() method.");
            }
        }

        /// <summary>
        /// metodo para enviar los pulsos con atributos attrLong, de tipo long
        /// </summary>
        /// <param name="pulse">Pulso construido por el client</param>
        /// <param name="listLong">lista de atributos a ser convertido.</param>
        public void SendPulseWithLong(PulsesRQ pulse, Dictionary<string, List<string>> listLong)
        {
            if (isStarted)
            {
                pulse = rebuildValue(pulse, listLong, false);
                SendPulse(pulse);
            }
            else
            {
                throw new Exception("Error sending pulses: BPulseJavaClient is not started yet. Please invoke the start() method.");
            }
        }

        /// <summary>
        /// Metodo que reconstruye el valor del pulso en base al atributo de tipo long 
        /// </summary>
        /// <param name="pulse">pulso</param>
        /// <param name="listAttr">lista de atributo</param>
        /// <param name="isTrace"></param>
        /// <returns></returns>
        private PulsesRQ rebuildValue(PulsesRQ pulse, Dictionary<string, List<string>> mapAttr, bool isTrace)
        {
            if (mapAttr == null || mapAttr.Count == 0)
            {
                return pulse;
            }
            //var mapAttr = listAttrtoMap(listAttr);
            var rqbuilder = pulse;

            foreach (var pulseValue in rqbuilder.Pulse)
            {
                //get the list of attributes from the map
                if (!mapAttr.ContainsKey(pulseValue.TypeId))
                    throw new Exception("El TypeId del pulso seleccionado no existe en la lista de pulsos de tipo Long");

                var listAttributes = mapAttr[pulseValue.TypeId];
                if (listAttributes != null && listAttributes.Count > 0)
                {

                    for (int i = 0; i < pulseValue.Values.Count; i++)
                    {
                        var value = pulseValue.Values[i];
                        if (listAttributes.Contains(value.Name) && value.Values.Count > 0)
                        {
                            for (int j = 0; j < value.Values.Count; j++)
                            {
                                value.Values[j] = Convert.ToBase64String(Encoding.UTF8.GetBytes(value.Values[j]));
                            }
                        }
                    }
                }
            }
            return rqbuilder;
        }

        /// <summary>
        /// lista de atributos 
        /// </summary>
        /// <param name="listLong">lista de atributos de tipo long.</param>
        /// <returns></returns>
        private Dictionary<string, List<string>> listAttrtoMap(List<AttributeDto> listLong)
        {
            Dictionary<string, List<string>> map = new Dictionary<string, List<string>>();
            if (listLong != null && listLong.Count > 0)
            {
                foreach (var attributeDto in listLong)
                {
                    map[attributeDto.TypeId] = attributeDto.ListAttr;
                }
            }
            return map;
        }
    }
}