using bpulse_sdk_csharp.bpulsesConstants;
using Google.Protobuf;
using log4net;
using me.bpulse.domain.proto.collector;
using System;
using System.Configuration;
using System.Net;
using System.Text;

namespace bpulse_sdk_csharp.rest
{
    /// <summary>
    ///      clase para invocar el servicio Rest
    /// </summary>
    public class RestInvoker
    {
        #region Private Fields

        private static readonly ILog Logger = LogManager.GetLogger("bpulseLogger");

        #endregion Private Fields

        #region Public Methods

        /// <summary>
        ///      Metodo para enviar los pulsos al serivico rest
        /// </summary>
        /// <param name="user">usuario configurado en app.config</param>
        /// <param name="pass">password configurado en app.config</param>
        /// <param name="pulse">pulso a enviar.</param>
        /// <returns>retorna un string de respuesta en arreglo de bytes.</returns>
        public bool SendByRestService(string user, string pass, PulsesRQ pulse)
        {
            var url = ConfigurationManager.AppSettings[BPulsesConstants.BPULSE_PROPERTY_URL_REST_SERVICE];
            var basicAuth = Convert.ToBase64String(Encoding.UTF8.GetBytes(user + ":" + pass));
            var data = pulse.ToByteArray();

            #region metodo actual que funciona

            using (var client = new WebClient())
            {
                // Request configuration
                client.Headers.Add("Authorization", "Basic " + basicAuth);
                client.Headers[HttpRequestHeader.ContentType] = "application/x-protobuf";
                client.Headers[HttpRequestHeader.Accept] = "application/x-protobuf";

                // Request execution
                try
                {
                    var response = client.UploadData(url, data);

                    var stringResponse = client.Encoding.GetString(response);
                    // TODO Probar que esto devuelva algo legible

                    return true;
                }
                catch (Exception e)
                {
                    Logger.Error(e.Message);
                    return false;
                }
            }

            #endregion metodo actual que funciona
        }

        #endregion Public Methods
    }
}