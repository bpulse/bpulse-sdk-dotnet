using bpulse_sdk_csharp.bpulsesConstants;
using log4net;
using me.bpulse.domain.proto.collector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace bpulse_sdk_csharp.pulseRepository
{
    /// <summary>
    ///      Clase que manejara el repositorio en memoria. y hereda de la interfaz IRepository.
    /// </summary>
    public class InMemoryRepository : IRepository
    {
        #region Public Constructors

        /// <summary>
        ///      Constructor del repositorio en memoria.
        /// </summary>
        /// <param name="maxNumberPulsesToProcessByTimer">
        ///      maximo de pulsos a procesar por tiempo.
        /// </param>
        public InMemoryRepository(int maxNumberPulsesToProcessByTimer)
        {
            _limitNumberPulsesToReadFromDb = maxNumberPulsesToProcessByTimer;

            bpulseRQInProgressMap = new Dictionary<string, PulsesRQ>();

            ConvertAllBpulseKeyInProgressToPending();
        }

        #endregion Public Constructors

        #region Private Fields

        private static readonly ILog Logger = LogManager.GetLogger("bpulseLogger");
        private static InMemoryRepository _instance;
        private readonly int _limitNumberPulsesToReadFromDb;
        private readonly Dictionary<string, PulsesRQ> bpulseRQInProgressMap;
        private readonly Dictionary<string, PulsesRQ> bpulseRQMap = new Dictionary<string, PulsesRQ>();
        private long _deleteTimeMillisAverage = BPulsesConstants.COMMON_NUMBER_0;
        private long _getTimeMillisAverage = BPulsesConstants.COMMON_NUMBER_0;
        private int _insertedRecords = BPulsesConstants.COMMON_NUMBER_0;
        private long _insertTimeMillisAverage = BPulsesConstants.COMMON_NUMBER_0;
        private long _sortedKeysTimeMillisAverage = BPulsesConstants.COMMON_NUMBER_0;

        #endregion Private Fields

        #region Public Methods

        /// <summary>
        ///      intancia el repositorio.
        /// </summary>
        /// <param name="currentMinute">minuto actual</param>
        /// <returns>retorna la instancia en el minuto actual.</returns>
        public static InMemoryRepository GetInstance(int currentMinute)
        {
            if (_instance == null)
                _instance = new InMemoryRepository(currentMinute);

            return _instance;
        }

        /// <summary>
        ///      metodo que convierte todas las claves en progreso a pendiente.
        /// </summary>
        public void ConvertAllBpulseKeyInProgressToPending()
        {
            try
            {
                bpulseRQInProgressMap.Clear();
            }
            catch (Exception e)
            {
                Logger.Error("FAILED TO MASSIVE UPDATE THE PULSES STATE FROM INPROGRESS TO PENDING: ", e);
                throw;
            }
        }

        /// <summary>
        ///      conteo de los Pulsos
        /// </summary>
        /// <returns></returns>
        public int CountBpulsesRq()
        {
            return 0;
        }

        /// <summary>
        ///      cuenta los pulsos marcados como en progreso
        /// </summary>
        /// <returns></returns>
        public int CountMarkBpulseKeyInProgress()
        {
            var resp = BPulsesConstants.COMMON_NUMBER_0;
            try
            {
                resp = bpulseRQInProgressMap.Count();
            }
            catch (Exception e)
            {
                Logger.Error("FAILED TO GET PULSES INPROGRESS COUNT: ", e);
                throw;
            }

            return resp;
        }

        /// <summary>
        ///      elimina los pulsos por su GUID
        /// </summary>
        /// <param name="pKey">Id unico del pulso asociado</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void DeleteBpulseRqByKey(string pKey)
        {
            var initTime = Calendar.EpochinMilis;
            try
            {
                bpulseRQMap.Remove(pKey);
                bpulseRQInProgressMap.Remove(pKey);
                _deleteTimeMillisAverage = _deleteTimeMillisAverage + (Calendar.EpochinMilis - initTime);
            }
            catch (Exception e)
            {
                Logger.Error("FAILED TO DELETE PULSE: ", e);
                throw e;
            }
        }

        /// <summary>
        ///      obtiene los pulsos por su clave unica dela asociado en memoria
        /// </summary>
        /// <param name="pKey">clave unica</param>
        /// <returns>retorna el pulso</returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public PulsesRQ GetBpulseRqByKey(string pKey)
        {
            PulsesRQ resp = null;
            var initTime = Calendar.EpochinMilis;
            try
            {
                resp = bpulseRQMap[pKey];
                _getTimeMillisAverage = _getTimeMillisAverage + (Calendar.EpochinMilis - initTime);
            }
            catch (Exception e)
            {
                Logger.Error("FAILED TO GET THE PULSE BY KEY: ", e);
                throw e;
            }
            return resp;
        }

        /// <summary>
        ///      obtiene la cantidad de pulsos almacenados en memoria
        /// </summary>
        /// <returns>retorna un entero con el numero de pulsos en memoria.</returns>
        public long GetDbSize()
        {
            return bpulseRQMap.Count();
        }

        /// <summary>
        ///      obtiene los pulsos ordenados. para su envio.
        /// </summary>
        /// <returns>retorna los pulsos.</returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public object[] GetSortedbpulseRqMapKeys()
        {
            var initTime = Calendar.EpochinMilis;
            var resp = new List<object>();
            try
            {
                var sortedKeys = new List<string>(bpulseRQMap.Keys);
                sortedKeys.Sort();

                var i = 0;
                while (resp.Count() < _limitNumberPulsesToReadFromDb && i < sortedKeys.Count())
                {
                    resp.Add(sortedKeys[i]);
                    i++;
                }

                _sortedKeysTimeMillisAverage = _sortedKeysTimeMillisAverage + (Calendar.EpochinMilis - initTime);
            }
            catch (Exception e)
            {
                Logger.Error("FAILED TO GET THE PENDING PULSES LIST: ", e);
                throw e;
            }

            return resp.ToArray();
        }

        /// <summary>
        ///      marca los pulsos almacenados en memoria en progreso.
        /// </summary>
        /// <param name="pKey">id unico del pulso a pasar.</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void MarkBpulseKeyInProgress(string pKey)
        {
            try
            {
                bpulseRQInProgressMap.Add(pKey, bpulseRQMap[pKey]);
            }
            catch (Exception e)
            {
                Logger.Error("FAILED TO UPDATE THE PULSE STATE FROM PENDING TO INPROGRESS: " + e);
                throw e;
            }
        }

        /// <summary>
        ///      libera el repositorio de los pulsos en progreso por su clave.
        /// </summary>
        /// <param name="pKey">guid del pulso almacenado en el repositorio de datos.</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void ReleaseBpulseKeyInProgressByKey(string pKey)
        {
            try
            {
                bpulseRQMap.Remove(pKey);
            }
            catch (Exception e)
            {
                Logger.Error("FAILED TO UPDATE THE PULSE STATE FROM INPROGRESS TO PENDING: " + e);
                throw e;
            }
        }

        /// <summary>
        ///      guardar el pulso en memoria, en un Dictionary
        /// </summary>
        /// <param name="pPulsesRQ">Pulso</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void SavePulse(PulsesRQ pPulsesRQ)
        {
            var initTime = Calendar.EpochinMilis;
            var key = Guid.NewGuid().ToString();

            bpulseRQMap.Add(key, pPulsesRQ);
            _insertedRecords++;

            var epoch = Calendar.EpochinMilis;

            _insertTimeMillisAverage = _insertTimeMillisAverage + (epoch - initTime);
        }

        #endregion Public Methods
    }
}