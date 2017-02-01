using System;
using System.Collections.Generic;
using System.Linq;
using me.bpulse.domain.proto.collector;
using bpulse_sdk_csharp.bpulsesConstants;
using log4net;
using System.Runtime.CompilerServices;

namespace bpulse_sdk_csharp.pulseRepository
{
    /// <summary>
    /// Clase que manejara el repositorio en memoria. y hereda de la interfaz IRepository.
    /// </summary>
    public class InMemoryRepository : IRepository
    {
        private long getTimeMillisAverage = BPulsesConstants.COMMON_NUMBER_0;
        private long deleteTimeMillisAverage = BPulsesConstants.COMMON_NUMBER_0;

        private long insertTimeMillisAverage = BPulsesConstants.COMMON_NUMBER_0;
        private int insertedRecords = BPulsesConstants.COMMON_NUMBER_0;
        private long sortedKeysTimeMillisAverage = BPulsesConstants.COMMON_NUMBER_0;
        private int limitNumberPulsesToReadFromDb = BPulsesConstants.COMMON_NUMBER_0;
        private Dictionary<string, PulsesRQ> bpulseRQInProgressMap;
        private Dictionary<string, PulsesRQ> bpulseRQMap = new Dictionary<string, PulsesRQ>();
        private static readonly ILog logger = LogManager.GetLogger("bpulseLogger");
        private static InMemoryRepository instance;

        /// <summary>
        /// Constructor del repositorio en memoria.
        /// </summary>
        /// <param name="maxNumberPulsesToProcessByTimer">maximo de pulsos a procesar por tiempo.</param>
        public InMemoryRepository(int maxNumberPulsesToProcessByTimer)
        {
            limitNumberPulsesToReadFromDb = maxNumberPulsesToProcessByTimer;

            bpulseRQInProgressMap = new Dictionary<string, PulsesRQ>();

            convertAllBpulseKeyInProgressToPending();
        }

        /// <summary>
        /// metodo que convierte todas las claves en progreso a pendiente.
        /// </summary>
        public void convertAllBpulseKeyInProgressToPending()
        {
            try
            {
                bpulseRQInProgressMap.Clear();

            }
            catch (Exception e)
            {
                logger.Error("FAILED TO MASSIVE UPDATE THE PULSES STATE FROM INPROGRESS TO PENDING: ", e);
                throw e;
            }
        }

        /// <summary>
        /// conteo de los Pulsos
        /// </summary>
        /// <returns></returns>
        public int countBpulsesRQ()
        {
            return 0;
        }

        /// <summary>
        /// cuenta los pulsos marcados como en progreso
        /// </summary>
        /// <returns></returns>
        public int countMarkBpulseKeyInProgress()
        {
            int resp = BPulsesConstants.COMMON_NUMBER_0;
            try
            {

                resp = bpulseRQInProgressMap.Count();

            }
            catch (Exception e)
            {
                logger.Error("FAILED TO GET PULSES INPROGRESS COUNT: ", e);
                throw e;
            }

            return resp;

        }

        /// <summary>
        /// elimina los pulsos por su GUID
        /// </summary>
        /// <param name="pKey">Id unico del pulso asociado</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void deleteBpulseRQByKey(string pKey)
        {
            long initTime = Calendar.EpochinMilis;
            try
            {
                bpulseRQMap.Remove(pKey);
                bpulseRQInProgressMap.Remove(pKey);
                deleteTimeMillisAverage = this.deleteTimeMillisAverage + (Calendar.EpochinMilis - initTime);
            }
            catch (Exception e)
            {
                logger.Error("FAILED TO DELETE PULSE: ", e);
                throw e;
            }
        }

        /// <summary>
        /// obtiene los pulsos por su clave unica dela asociado en memoria
        /// </summary>
        /// <param name="pKey">clave unica</param>
        /// <returns>retorna el pulso</returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public PulsesRQ getBpulseRQByKey(string pKey)
        {
            PulsesRQ resp = null;
            long initTime = Calendar.EpochinMilis;
            try
            {
                resp = bpulseRQMap[pKey];
                getTimeMillisAverage = getTimeMillisAverage + (Calendar.EpochinMilis - initTime);

            }
            catch (Exception e)
            {
                logger.Error("FAILED TO GET THE PULSE BY KEY: ", e);
                throw e;
            }
            return resp;
        }

        /// <summary>
        /// obtiene la cantidad de pulsos almacenados en memoria
        /// </summary>
        /// <returns>retorna un entero con el numero de pulsos en memoria.</returns>
        public long getDBSize()
        {
            return bpulseRQMap.Count();

        }

        /// <summary>
        /// obtiene los pulsos ordenados. para su envio.
        /// </summary>
        /// <returns>retorna los pulsos.</returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public object[] getSortedbpulseRQMapKeys()
        {
            long initTime = Calendar.EpochinMilis;
            List<object> resp = new List<object>();
            try
            {
                List<string> sortedKeys = new List<string>(bpulseRQMap.Keys);
                sortedKeys.Sort();

                int i = 0;
                while (resp.Count() < limitNumberPulsesToReadFromDb && i < sortedKeys.Count())
                {
                    resp.Add(sortedKeys[i]);
                    i++;
                }

                sortedKeysTimeMillisAverage = sortedKeysTimeMillisAverage + (Calendar.EpochinMilis - initTime);

            }
            catch (Exception e)
            {
                logger.Error("FAILED TO GET THE PENDING PULSES LIST: ", e);
                throw e;
            }


            return resp.ToArray();
        }

        /// <summary>
        /// intancia el repositorio.
        /// </summary>
        /// <param name="currentMinute">minuto actual</param>
        /// <returns>retorna la instancia en el minuto actual.</returns>
        public static InMemoryRepository GetInstance(int currentMinute)
        {
            if ((instance == null))
            {
                instance = new InMemoryRepository(currentMinute);
            }

            return instance;
        }

        /// <summary>
        /// marca los pulsos almacenados en memoria en progreso.
        /// </summary>
        /// <param name="pKey">id unico del pulso a pasar.</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void markBpulseKeyInProgress(string pKey)
        {
            try
            {
                bpulseRQInProgressMap.Add(pKey, bpulseRQMap[pKey]);

            }
            catch (Exception e)
            {
                logger.Error("FAILED TO UPDATE THE PULSE STATE FROM PENDING TO INPROGRESS: " + e);
                throw e;
            }
        }

        /// <summary>
        /// libera el repositorio de los pulsos en progreso por su clave.
        /// </summary>
        /// <param name="pKey">guid del pulso almacenado en el repositorio de datos.</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void releaseBpulseKeyInProgressByKey(string pKey)
        {
            try
            {
                bpulseRQMap.Remove(pKey);
            }
            catch (Exception e)
            {
                logger.Error("FAILED TO UPDATE THE PULSE STATE FROM INPROGRESS TO PENDING: " + e);
                throw e;
            }
        }

        /// <summary>
        /// guardar el pulso en memoria, en un Dictionary
        /// </summary>
        /// <param name="pPulsesRQ">Pulso</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void SavePulse(PulsesRQ pPulsesRQ)
        {
            var initTime = Calendar.EpochinMilis;
            string key = Guid.NewGuid().ToString();

            bpulseRQMap.Add(key, pPulsesRQ);
            insertedRecords++;

            if (bpulseRQMap.Count == 1000)
            {
                var example = getBpulseRQByKey(key);
                example = bpulseRQMap[key];
                PulsesRQ pul = new PulsesRQ();
                pul.Pulse.Add(example.Pulse);
                Console.WriteLine("llegue a 1000");
            }

            var epoch = Calendar.EpochinMilis;

            insertTimeMillisAverage = insertTimeMillisAverage + (epoch - initTime);
        }
    }
}