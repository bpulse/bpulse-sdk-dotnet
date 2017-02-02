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
using System.Timers;

namespace bpulse_sdk_csharp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            testSendPulse();
        }
        public static void testSendPulse()
        {
            Timer timer = new Timer();
            timer.Interval = 80000;
            timer.Elapsed += WavesofPulses;
            timer.Start();
            Console.WriteLine("Done");
            Console.ReadLine();
        }

        private static void WavesofPulses(object sender, ElapsedEventArgs e)
        {
            for (int i = 0; i < 750000; i++)
            {
                PulsesRQ request;

                PulsesRQ pulses = new PulsesRQ();

                //var demoProcessedPulses = DemoProcessedPulses(pulses);
                var demoPruebas = DemoPruebasCLientes(pulses);

                List<AttributeDto> attributedto = new List<AttributeDto>();
                List<string> listAttrb = new List<string>();
                listAttrb.Add("attrLong");
                listAttrb.Add("newattrLong");
                AttributeDto adto = new AttributeDto("bpulse_demo_PruebasClientes", listAttrb);

                attributedto.Add(adto);

                List<string> listAttrb2 = new List<string>();
                listAttrb2.Add("Long");
                listAttrb2.Add("newLong");
                AttributeDto adto2 = new AttributeDto("bpulse_bpulse_processedPulses", listAttrb2);
                attributedto.Add(adto2);

                request = demoPruebas;
                BPulseCsharpClient client = new BPulseCsharpClient().GetInstance();

                client.SendPulseWithLong(pulses, attributedto);

            }
        }

        public static PulsesRQ DemoPruebasCLientes(PulsesRQ pulses)
        {


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