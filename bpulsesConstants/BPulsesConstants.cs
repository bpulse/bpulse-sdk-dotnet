using System;

namespace bpulse_sdk_csharp.bpulsesConstants
{
    /// <summary>
    /// Clase de atributos constantes.
    /// </summary>
    public class BPulsesConstants
    {
        public const long DEFAULT_TIMER_MIN_DELAY = 60000;
        public const long DEFAULT_TIMER_MAX_NUMBER_GROUPED_PULSES = 1000;
        public const int DEFAULT_REST_INVOKER_TIMEOUT = 30000;
        public const long DEFAULT_MAX_PULSESDB_SIZE_BYTES = 1073741824;
        public const int COMMON_NUMBER_60 = 60;
        public const int COMMON_NUMBER_1000 = 1000;
        public const int COMMON_NUMBER_0 = 0;
        public const int COMMON_NUMBER_1 = 1;
        public const int COMMON_NUMBER_2 = 2;
        public const int COMMON_NUMBER_3 = 3;
        public const int COMMON_NUMBER_5 = 5;
        public const int COMMON_NUMBER_MINUS_5 = -5;
        public const int COMMON_NUMBER_180000 = 180000;
        public const int BPULSE_REST_HTTP_CREATED = 201;
        public const long BPULSE_MAX_MEM_NUM_PULSES = 600000L;

        public const String BPULSE_SUCCESSFUL_RESPONSE = "OK";
        public const string BPULSE_FAILED_RESPONSE = "ERROR";
        public const string BPULSE_STATUS_PENDING = "P";
        public const string BPULSE_STATUS_INPROGRESS = "I";
        public const string BPULSE_REPOSITORY_NAME = "BPULSEDB";
        public const string BPULSE_REPOSITORY_USER = "admin";
        public const string BPULSE_PROPERTY_CONFIG_FILE = "bpulse.client.config";
        public const string BPULSE_PROPERTY_USER_TIMER_DELAY = "bpulse.client.periodInMinutesNextExecTimer";
        public const string BPULSE_PROPERTY_NUMBER_THREADS_SEND_PULSES = "bpulse.client.initNumThreadsSendPulses";
        public const string BPULSE_PROPERTY_NUMBER_THREADS_REST_INVOKER = "bpulse.client.initNumThreadsRestInvoker";
        public const string BPULSE_PROPERTY_USER_CREDENTIALS_USERNAME = "bpulse.client.bpulseUsername";
        public const string BPULSE_PROPERTY_USER_CREDENTIALS_PASSWORD = "bpulse.client.bpulsePassword";
        public const string BPULSE_PROPERTY_URL_REST_SERVICE = "bpulse.client.bpulseRestURL";
        public const string BPULSE_PROPERTY_MAX_NUMBER_PULSES_TO_PROCESS_TIMER = "bpulse.client.maxNumberPulsesReadFromTimer";
        public const string BPULSE_PROPERTY_MAX_PULSESDB_SIZE_BYTES = "bpulse.client.pulsesRepositoryDBMaxSizeBytes";
        public const string BPULSE_PROPERTY_PULSES_REPOSITORY_MODE = "bpulse.client.pulsesRepositoryMode";
        public const string BPULSE_PROPERTY_MEM_PULSES_REPOSITORY_MAX_NUM_PULSES = "bpulse.client.pulsesRepositoryMemMaxNumberPulses";
        public const string BPULSE_MEM_PULSES_REPOSITORY = "MEM";
        public const string BPULSE_DB_PULSES_REPOSITORY = "DB";
        public const string CHARSET_UTF8 = "UTF-8";
        public const string CHARSET_ISO88591 = "ISO-8859-1";

    }

    /// <summary>
    /// Clase Calendario
    /// </summary>
    public class Calendar
    {
        /// <summary>
        /// campo para obtener el tiempo de epoca en milisegundos.
        /// </summary>
        public static long EpochinMilis
        {
            get
            {
                long epochmilis = Convert.ToInt64((DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds);
                return epochmilis;
            }
        }
    }
}