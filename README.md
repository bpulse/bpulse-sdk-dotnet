# bpulse-sdk-dotnet

Bpulse SDK donet or BPulse CSharp Client is a conector between any donet based application subscribed to BPULSE Service and the PULSES COLLECTOR REST SERVICE.
This README explains how to integrate the conector with the target client application, configuration parameters and how to use it.

# Requirements

* [bpulse-protobuf-donet](https://github.com/bpulse/bpulse-protobuf-dotnet)
* [NetFrameWork 4.5.2 +](https://www.microsoft.com/en-us/download/details.aspx?id=42642)

# Build dependencies
The following dependencies are required to build the sdk and are also required add to project by Nuget Package Manager:

* **BPulse dependencies**
 * bpulse.protobuf-csharp\[latest].dll
* **Google Protobuf dependencies**
 * Google.Protobuf version="3.1.0"
* **Http dependencies**
 * Microsoft.AspNet.WebApi.Client version="5.2.3"
 * Newtonsoft.Json" version="6.0.4"
* **log4net dependencies**
 * log4net version="2.0.7"
 * Corresponding Binding for used logging framework (See **Binding with a logging framework at deployment time** at [https://logging.apache.org/log4net/release/manual/introduction.html](https://logging.apache.org/log4net/release/manual/introduction.html))

This is a Visual Studio 2015 Solution, so all of this dependencies are already added in the dll of solution packager.config but you must add them on you Visual Studio 2015 if you want build your application;

# Build
Just clone the repository and make sure all the dependencies are satisfied, then in Visual Studio go to the Build menu and execute:

## Importing to a Visual Studio project
After building the project use the bpulse_sdk_csharp.dll file in project bin folder and add that reference to the new project.

Remember the dependencies mentioned above incase your current classpath doesn't have them at runtime, also keep in mind the version of this project you are building.

## Starting your application
The SDK needs some configuration properties that indicate the client how should work and where to connect so you must provide a properties file when you start your application or the application server where your application run, so to do that, simply edit the **App.config** on you project, for example:

This is an example of a basic configuration file content:
```properties
#BPULSE DONET CLIENT CONFIGURATION APP.CONFIG EXAMPLE
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
  <startup>
    <supportedRuntime version="v4.0" sku=".NETFramework,Version=v4.5.2" />
  </startup>
  <appSettings>
    <add key="bpulse.client.periodInMinutesNextExecTimer" value="1"/>
    <add key="bpulse.client.maxNumberPulsesReadFromTimer" value="180000"/>
    <add key="bpulse.client.bpulseUsername" value="usernameCredential"/>
    <add key="bpulse.client.bpulsePassword" value="passwordCredential"/>
    <add key="bpulse.client.bpulseRestURL" value="http://URL/app.collector/collector/pulses"/>
    <add key="bpulse.client.pulsesRepositoryMode" value="MEM"/>
    <add key="bpulse.client.pulsesRepositoryMemMaxNumberPulses" value="600000"/>
  </appSettings>
</configuration>
```

# Usage

The starting point is the BPulseCsharpClient class. It implements two methods: getInstance() and sendPulse(PulsesRQ) to publish them via BPULSE COLLECTOR REST SERVICE.

```csharp
//get the BPulseCsharpClient instance. It manages the pulses repository and begins the pulses notification timer.
BPulseCsharpClient client = new BPulseCsharpClient.getInstance();
```

Then use a combination of *me.bpulse.domain.proto.collector.CollectorMessageRQ.PulsesRQ*, *me.bpulse.domain.proto.collector.CollectorMessageRQ.Value* and *me.bpulse.domain.proto.collector.CollectorMessageRQ.Pulse* in order to build the pulses you want to send according to the Pulse Definition made in BPULSE, for example:

```csharp
//Request instance
  PulsesRQ request;
//create instances of PulsesRQ
  PulsesRQ pulses = new PulsesRQ();
//Pulse version, send 1.0 always, we will use this field later.
  pulses.Version = "1.0";

//Instance a new Pulse to create each pulse individually
  Pulse pulse = new Pulse();

//Name of the pulse definition, the same as defined using the BPULSE web app
  pulse.TypeId = "bpulse_client_hotelavail";
//Time of the pulse, usually should be the current time but you can set whatever time you need, or can reference with TimeSpan Epoch
//or you can use the property use Calendar.EpochinMilis; on Calendar Class of bpulse_sdk_csharp.bpulsesConstants;
  var epoch = Convert.ToInt64((DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds);
  pulse.Time = epoch;
//pulse.Time = Calendar.EpochinMilis;
//
  pulse.InstanceId = "1";

//instance Value to assing the different pulse values to each pulse
  Value value = new Value();
//Name of the pulse attribute
  value.Name = "attribute_name";
//Value of the current attribute
  value.Values.Add("attribute_value");
//Add the created value to the pulse instance
  pulse.Values.Add(value);

//Same as before but for a time value TODO Joda time
  Value value = new Value();
  value.Name = "dateAttr";
  value.Values.Add("2017-01-23T00:01:25");
  pulse.Values.Add(value);

//Same as before but for a numeric value
  value = new Value();
  value.Name = "numericAttr";
  value.Values.Add("1500");
  pulse.Values.Add(value);

//Add the pulse to the pulses collection
 pulses.Pulse.Add(pulse);

//Then build the pulses request
   request = pulses;

```

Finally send the pulse created with:

```csharp
//invoke the operation for inserting the pulse into pulses repository.
   client.SendPulse(request);
```
When you need to send longData values, you have to build the pulses you want to send according to the Pulse Definition made in BPULSE similar to example above

```csharp
//Request instance
  PulsesRQ request;
//create instances of PulsesRQ
  PulsesRQ pulses = new PulsesRQ();
//Pulse version, send 1.0 always, we will use this field later.
  pulses.Version = "1.0";

//Instance a new Pulse to create each pulse individually
  Pulse pulse = new Pulse();

//Name of the pulse definition, the same as defined using the BPULSE web app
  pulse.TypeId = "bpulse_client_hotelavail";
//Time of the pulse, usually should be the current time but you can set whatever time you need, or can reference with TimeSpan Epoch
//or you can use the property use Calendar.EpochinMilis; on Calendar Class of bpulse_sdk_csharp.bpulsesConstants;
  var epoch = Convert.ToInt64((DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds);
  pulse.Time = epoch;
//pulse.Time = Calendar.EpochinMilis;
//
  pulse.InstanceId = "1";

//instance Value to assing the different pulse values to each pulse
  Value value = new Value();
//Name of the pulse attribute
  value.Name = "attribute_name";
//Value of the current attribute
  value.Values.Add("attribute_value");
//Add the created value to the pulse instance
  pulse.Values.Add(value);

//Same as before but for a time value TODO Joda time
  Value value = new Value();
  value.Name = "dateAttr";
  value.Values.Add("2017-01-23T00:01:25");
  pulse.Values.Add(value);

//Same as before but for a numeric value
  value = new Value();
  value.Name = "numericAttr";
  value.Values.Add("1500");
  pulse.Values.Add(value);
  
//Same as before but for a LongData value
value = new Value();
value.Name=("attrLong");
value.Values.Add("[ { \"_id\": \"576c0e769ae931f5\", \"index\": 0, \"product\": \"HT206\", \"isActive\": true, \"price\": \"221\" } ]");

//Add the pulse to the pulses collection
pulses.addPulse(pulse);

//Then build the pulses request
request = pulses.build();

```

And for each attribute defined as LongData type you must provide their names in a list for each pulse. 
```
//Configure LongData attributes.
  List<AttributeDto> AttributeDtoList = new List<AttributeDto>();
  List<string> listAttrb = new List<string>();
// Add your attributes name to the List
  listAttrb.Add("attrLong");
  listAttrb.Add("newattrLong");
// add the List and Pulse TypeId 
  AttributeDto adto = new AttributeDto("bpulse_demo_PruebasClientes", listAttrb);
          
  AttributeDtoList.Add(adto);
```
Use the appropriate method to send it
```
client.sendPulseWithLong(request, AttributeDtoList);
```

And that's it!, now you are sending pulses to BPULSE and can see them using the web dashboard.

## Available Configuration Parameters

BPulse Csharp client has a configuration file to define the main parameters for sending and processing pulses (pulses repository path, number of threads for notifying pulses via BPULSE COLLECTOR REST SERVICE, etc.). It's definition is expected through you project App.config File **App.config** 

All properties are defined below:

| Variable name        | Description           |
|:------------- |:------------- |
|bpulse.client.periodInMinutesNextExecTimer|Delay time in minutes between timer executions for pulses notification (default value = 1). |
|bpulse.client.periodInMinutesNextExecTimer|Delay time in minutes between timer executions for pulses notification (default value = 1). |
|bpulse.client.maxNumberPulsesReadFromTimer|Max number of read pulses for each timer execution from pulsesRepositoryDB for sending to BPULSE COLLECTOR REST SERVICE (default value = 180000). |
|bpulse.client.bpulseUsername|Client's Username for sending pulses to BPULSE COLLECTOR SERVICE. |
|bpulse.client.bpulsePassword|Client's Password  for sending pulses to BPULSE COLLECTOR SERVICE. |
|bpulse.client.bpulseRestURL| BPULSE COLLECTOR REST SERVICE URL. |
|bpulse.client.pulsesRepositoryMode|Pulses Repositories' Mode:  MEM=PULSES IN MEMORY |
|bpulse.client.pulsesRepositoryMemMaxNumberPulses|When the pulses repositories' mode is MEM, it's necessary define the maximum number of pulses in memory(default value = 600000). |

An example of configuration file is shown:


## About Logging
BPulse Csharp Client uses Apache log4net API for register logs from pulse processing sending via BPULSE REST SERVICE. 
log4net version="2.0.7

## Logging Configuration Parameters

After selecting the logging api, it's necessary to add a Csharp option according to the used logging framework:


In case of the target system has its own logging properties file, it's necessary to add the corresponding lines mentioned above to it.

# Contact us

You can reach the Developer Platform team at jtenganan@bpulse.io

# License

The Bpulse Protobuf Java is licensed under the Apache License 2.0. Details can be found in the LICENSE file.
