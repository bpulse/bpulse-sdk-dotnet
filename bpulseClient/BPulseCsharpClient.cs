using System;
using System.Collections.Generic;
using System.Configuration;
using System.Runtime.CompilerServices;
using System.Text;
using System.Timers;
using bpulse_sdk_csharp.bpulsesConstants;
using bpulse_sdk_csharp.bpulseSender;
using bpulse_sdk_csharp.dto;
using bpulse_sdk_csharp.timer;
using log4net;
using me.bpulse.domain.proto.collector;

namespace bpulse_sdk_csharp.bpulseClient
{
    /// <summary>
    ///      Clase donde se inicializa el timer y la logica del conector.
    /// </summary>
    public class BPulseCsharpClient
    {
        #region Public Constructors

        /// <summary>
        ///      Constructor de la clase donde se obtiene la instancia de los repositorios y se
        ///      inicia el proceso.
        /// </summary>
        public BPulseCsharpClient()
        {
            Logger.Info("GET INSTANCE BpulseCsharpClient...");
            _bpulseSender = BpulseSender.GetInstance();
            Start();
            Logger.Info("GET INSTANCE BpulseJavaClient SUCCESSFUL.");
        }

        #endregion Public Constructors

        #region Private Fields

        private static readonly ILog Logger = LogManager.GetLogger("bpulseLogger");
        private static BPulseCsharpClient _instance;
        private static bool _isStarted;
        private readonly BpulseSender _bpulseSender;
        private string _propDbMode;

        #endregion Private Fields

        #region Public Methods

        /// <summary>
        ///      Metodo para obtener la instancia del cliente del conector, a modo de Singleton.
        /// </summary>
        /// <returns>Retorna la instancia inicializada, o si no, instancia una nueva.</returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public BPulseCsharpClient GetInstance()
        {
            if (_instance == null)
                _instance = new BPulseCsharpClient();
            return _instance;
        }

        /// <summary>
        ///      Metodo para enviar pulsos. este metodo almacena los pulsos a enviar en memoria hasta
        ///      llenar el maximo programado.
        /// </summary>
        /// <param name="pulse">Pulso construido por el cliente</param>
        /// <returns>regresa un string de exitoso o no exitoso.</returns>
        public string SendPulse(PulsesRQ pulse)
        {
            if (_isStarted)
                return _bpulseSender.SendPulse(pulse);
            throw new Exception(
                "Error sending pulses: BPulseCsharpClient is not started yet. Please invoke the start() method.");
        }

        /// <summary>
        ///      metodo para enviar los pulsos con atributos attrLong, de tipo long
        /// </summary>
        /// <param name="pulse">Pulso construido por el client</param>
        /// <param name="listLong">lista de atributos a ser convertido.</param>
        public void SendPulseWithLong(PulsesRQ pulse, List<AttributeDto> listLong)
        {
            if (_isStarted)
            {
                pulse = RebuildValue(pulse, listLong);
                SendPulse(pulse);
            }
            else
            {
                throw new Exception(
                    "Error sending pulses: BPulseJavaClient is not started yet. Please invoke the start() method.");
            }
        }

        /// <summary>
        ///      metodo de inicio del cliente.
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Start()
        {
            try
            {
                Logger.Info("INIT BPULSE TIMER...");

                if (!_isStarted)
                {
                    _propDbMode = BPulsesConstants.BPULSE_MEM_PULSES_REPOSITORY;

                    var tim = new Timer();

                    tim.Enabled = true;

                    var periodInMinutesNextExecutionTimer =
                        ConfigurationManager.AppSettings[BPulsesConstants.BPULSE_PROPERTY_USER_TIMER_DELAY];
                    long periodInMillis;
                    if (periodInMinutesNextExecutionTimer == null)
                        periodInMillis = BPulsesConstants.DEFAULT_TIMER_MIN_DELAY;
                    else
                        periodInMillis = long.Parse(periodInMinutesNextExecutionTimer) *
                                         BPulsesConstants.COMMON_NUMBER_60 * BPulsesConstants.COMMON_NUMBER_1000;

                    tim.Interval = periodInMillis;
                    tim.Elapsed += BPulseRestSenderTask;

                    _isStarted = true;
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Error al iniciar el Cliente " + ex.Message);
            }
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        ///      Metodo de tipo Timer que se ejecutara cada cierto tiempo segun el tiempo en
        ///      AppConfig periodInMinutesNextExecTimer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BPulseRestSenderTask(object sender, ElapsedEventArgs e)
        {
            var bpSt = new BpulseRestSenderTimer(_bpulseSender, _propDbMode);
        }

        /// <summary>
        ///      lista de atributos
        /// </summary>
        /// <param name="listLong">lista de atributos de tipo long.</param>
        /// <returns></returns>
        private Dictionary<string, List<string>> listAttrtoMap(List<AttributeDto> listLong)
        {
            var map = new Dictionary<string, List<string>>();
            if (listLong != null && listLong.Count > 0)
                foreach (var attributeDto in listLong)
                    map.Add(attributeDto.TypeId, attributeDto.ListAttr);
            return map;
        }

        /// <summary>
        ///      Metodo que reconstruye el valor del pulso en base al atributo de tipo long
        /// </summary>
        /// <param name="pulse">pulso</param>
        /// <param name="listAttr">lista de atributo</param>
        /// <returns></returns>
        private PulsesRQ RebuildValue(PulsesRQ pulse, List<AttributeDto> listAttr)
        {
            if (listAttr == null || listAttr.Count == 0)
                return pulse;

            var rqbuilder = pulse;
            var mapAttr = listAttrtoMap(listAttr);
            foreach (var pulseValue in rqbuilder.Pulse)
            {
                //get the list of attributes from the map
                if (!mapAttr.ContainsKey(pulseValue.TypeId))
                    throw new Exception("El TypeId del pulso seleccionado no existe en la lista de pulsos de tipo Long");

                var listAttributes = mapAttr[pulseValue.TypeId];
                if (listAttributes != null && listAttributes.Count > 0)
                    for (var i = 0; i < pulseValue.Values.Count; i++)
                    {
                        var value = pulseValue.Values[i];
                        if (listAttributes.Contains(value.Name) && value.Values.Count > 0)
                            for (var j = 0; j < value.Values.Count; j++)
                            {
                                var newValue = Convert.ToBase64String(Encoding.UTF8.GetBytes(value.Values[j]));
                                value.Values[j] = newValue;
                            }
                    }
            }
            return rqbuilder;
        }

        #endregion Private Methods
    }
}