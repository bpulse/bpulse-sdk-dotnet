using bpulse_sdk_csharp.bpulseClient;
using bpulse_sdk_csharp.bpulsesConstants;
using bpulse_sdk_csharp.dto;
using bpulse_sdk_csharp.pulseRepository;
using me.bpulse.domain.proto.collector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace bpulse_sdk_csharp
{
    public class Program
    {
        private InMemoryRepository MemRepository;

        public static void Main(string[] args)
        {
            testSendPulse();
        }
        public static void testSendPulse()
        {
            Console.WriteLine("Construyendo el Pulso");




            var numPulseToSend = BPulsesConstants.DEFAULT_TIMER_MAX_NUMBER_GROUPED_PULSES;
            for (int i = 0; i < 750000; i++)
            {



                //var demoProcessedPulses = DemoProcessedPulses(pulses);
                var demoPruebas = DemoPruebasCLientes();

                //request = pulses;
                //  request = demoPruebas;
                //request = demoProcessedPulses;

                // Console.WriteLine(request);
                //Console.WriteLine("Enviando el pulso");
                // BPulseCsharpClient client = new BPulseCsharpClient().GetInstance();
                // client.SendPulse(request);
            }


            //bpulseSender.SendPulse(repository);


            //client.SendPulseWithLong(request, attrdto);

            Console.WriteLine("Done");
            Console.ReadLine();
        }

        public static PulsesRQ DemoPruebasCLientes()
        {
            PulsesRQ request;

            PulsesRQ pulses = new PulsesRQ();

            pulses.Version = "0.1";
            Pulse pulse = new Pulse();
            pulse.InstanceId = "1";
            pulse.TypeId = "bpulse_demo_PruebasClientes";


            //  var epoch = Convert.ToInt64((DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds);
            pulse.Time = Calendar.EpochinMilis;

            Value value = new Value();
            value.Name = "dateAttr";
            value.Values.Add("2017-01-23T00:01:25");
            pulse.Values.Add(value);

            value = new Value();
            value.Name = "numericAttr";
            value.Values.Add("1500");
            pulse.Values.Add(value);

            value = new Value();
            value.Name = "listStringAttr";
            value.Values.Add("Code01");
            value.Values.Add("Code02");
            value.Values.Add("Code03");

            pulse.Values.Add(value);

            value = new Value();
            value.Name = "client";
            value.Values.Add("client_dotnet_demo");
            pulse.Values.Add(value);

            value = new Value();
            value.Name = "stringAttr";
            value.Values.Add("gerardo");
            pulse.Values.Add(value);

            value = new Value();
            value.Name = ("attrLong");
            value.Values.Add("[ { \"_id\": \"576c0e769ae931f5\", \"index\": 0, \"product\": \"HT206\", \"isActive\": true, \"price\": \"221\" } ]");
            pulse.Values.Add(value);

            value = new Value();
            value.Name = ("attrLong");
            value.Values.Add("[ { \"_id\": \"576c0e769ae931f5\", \"index\": 0, \"product\": \"HT206\", \"isActive\": true, \"price\": \"221\" } ]");
            pulse.Values.Add(value);

            value = new Value();
            value.Name = ("newattrLong");
            value.Values.Add("[ { \"_id\": \"576c0e769ae931f5\", \"index\": 0, \"product\": \"HT206\", \"isActive\": true, \"price\": \"221\" } ]");
            pulse.Values.Add(value);
            //string json = """{ ""widget"":{ ""debug"":""on"",""window"":{ ""title"":""Sample Konfabulator Widget"",""name"":""main_window"",""width"":500,""height"":500},""image"":{ ""src"":""Images/Sun.png"",""name"":""sun1"",""hOffset"":250,""vOffset"":250,""alignment"":""center""},""text"":{ ""data':'Click Here','size':36,'style':'bold','name':'text1','hOffset':250,'vOffset':100,'alignment':'center','onMouseUp':'sun1.opacity = (sun1.opacity / 100) * 90;'} } }'";


            pulses.Pulse.Add(pulse);

            Pulse pulse2 = new Pulse();
            pulse2.InstanceId = "1";
            pulse2.TypeId = "bpulse_bpulse_processedPulses";
            pulse2.Time = Calendar.EpochinMilis;

            Value value2 = new Value();
            value2.Name = "dateAttr";
            value2.Values.Add("2017-01-23T00:01:25");
            pulse2.Values.Add(value2);

            value2 = new Value();
            value2.Name = "numericAttr";
            value2.Values.Add("1500");
            pulse2.Values.Add(value2);

            value2 = new Value();
            value2.Name = "listStringAttr";
            value2.Values.Add("Code01");
            value2.Values.Add("Code02");
            value2.Values.Add("Code03");

            pulse2.Values.Add(value2);

            value2 = new Value();
            value2.Name = "client";
            value2.Values.Add("client_dotnet_demo");
            pulse2.Values.Add(value2);

            value2 = new Value();
            value2.Name = "stringAttr";
            value2.Values.Add("gerardo");
            pulse2.Values.Add(value2);

            value2 = new Value();
            value2.Name = ("Long");
            value2.Values.Add("[ { \"_id\": \"576c0e769ae931f5\", \"index\": 0, \"product\": \"HT206\", \"isActive\": true, \"price\": \"221\" } ]");
            pulse2.Values.Add(value2);

            value2 = new Value();
            value2.Name = ("newLong");
            value2.Values.Add("[ { \"_id\": \"576c0e769ae931f5\", \"index\": 0, \"product\": \"HT206\", \"isActive\": true, \"price\": \"221\" } ]");
            pulse2.Values.Add(value2);

            pulses.Pulse.Add(pulse2);

            BPulseCsharpClient client = new BPulseCsharpClient().GetInstance();
            Dictionary<string, List<string>> attributedto = new Dictionary<string, List<string>>();
            List<string> listAttrb = new List<string>();
            listAttrb.Add("attrLong");
            listAttrb.Add("newattrLong");

            attributedto.Add("bpulse_demo_PruebasClientes", listAttrb);

            List<string> listAttrb2 = new List<string>();
            listAttrb2.Add("Long");
            listAttrb2.Add("newLong");

            attributedto.Add("bpulse_bpulse_processedPulses", listAttrb);

            client.SendPulseWithLong(pulses, attributedto);

            return pulses;

        }

        public static PulsesRQ DemoProcessedPulses(PulsesRQ pulses)
        {
            pulses.Version = "0.1";

            Pulse pulse = new Pulse();

            pulse.InstanceId = "1";
            pulse.TypeId = "bpulse_bpulse_processedPulses";

            var epoch = Convert.ToInt64((DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds);
            pulse.Time = Calendar.EpochinMilis;

            Value value = new Value();
            value.Name = "nErrors";
            value.Values.Add("0");
            pulse.Values.Add(value);

            value = new Value();
            value.Name = "nPulses";
            value.Values.Add("1");
            pulse.Values.Add(value);

            value = new Value();
            value.Name = "rsInstance";
            value.Values.Add("gerardo");
            pulse.Values.Add(value);

            value = new Value();
            value.Name = "clientId";
            value.Values.Add("demo");
            pulse.Values.Add(value);

            value = new Value();
            value.Name = "rsTime";
            value.Values.Add("1200");
            pulse.Values.Add(value);


            pulses.Pulse.Add(pulse);
            return pulses;
        }
    }
}