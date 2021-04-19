using System;
using System.Collections.Generic;
using AudioSwitcher.AudioApi;
using AudioSwitcher.AudioApi.CoreAudio;
using System.Linq;
using Newtonsoft.Json;
using System.Drawing;
using System.IO;
using System.Threading;
using Newtonsoft.Json.Linq;
using System.Diagnostics;


namespace VolumeController
{

    class Program
    {
        static public CoreAudioController controller=new CoreAudioController();
        static public CoreAudioDevice mainDevice = controller.DefaultPlaybackDevice;
        static public Boolean isLog=false;
        static public Boolean isJson=false;
        static public Boolean isFade=false;
        static public IDictionary<string, object> getDeviceData(CoreAudioDevice device=null){
            if (device==null){
                device=mainDevice;
            }
            IDictionary<string, object> data = new Dictionary<string, object>();
            data.Add("name", mainDevice.FullName); //Device Name + Interface
            data.Add("type", mainDevice.Name); //Name Only
            data.Add("interface", mainDevice.InterfaceName); //Interface Only
            data.Add("device", mainDevice.DeviceType);

            data.Add("icon", mainDevice.Icon);
            data.Add("iconPath", mainDevice.IconPath);
            data.Add("id", mainDevice.Id);
            data.Add("realID", mainDevice.RealId);

            data.Add("isMuted", mainDevice.IsMuted);
            data.Add("volume", mainDevice.Volume); //Master Volume
            data.Add("state", mainDevice.State); //Current State (Active/Disable)

            if(isJson)
                System.Console.WriteLine(JsonConvert.SerializeObject(data,Formatting.Indented));
            else
                foreach (KeyValuePair<string, object> kvp in data)
                    Console.WriteLine("{0}: {1}", kvp.Key, kvp.Value);
            return data;
        }        
        static public JObject getDeviceData(bool json,CoreAudioDevice device=null){
            if (device==null){
                mainDevice=controller.GetDefaultDevice(DeviceType.Playback,Role.Console);
                device=mainDevice;
            }
            IDictionary<string, object> data = new Dictionary<string, object>();
            data.Add("name", mainDevice.FullName); //Device Name + Interface
            data.Add("type", mainDevice.Name); //Name Only
            data.Add("interface", mainDevice.InterfaceName); //Interface Only
            data.Add("device", mainDevice.DeviceType);

            data.Add("icon", mainDevice.Icon);
            data.Add("iconPath", mainDevice.IconPath);
            data.Add("id", mainDevice.Id);
            data.Add("realID", mainDevice.RealId);

            data.Add("isMuted", mainDevice.IsMuted);
            data.Add("volume", mainDevice.Volume); //Master Volume
            data.Add("state", mainDevice.State); //Current State (Active/Disable)
            return (JObject)JsonConvert.DeserializeObject(JsonConvert.SerializeObject(data,Formatting.Indented));
        }

        // static public IDictionary<string, object> getSourceData(AudioSwitcher.AudioApi.Session.IAudioSession item){
        //     IDictionary<string, object> source = new Dictionary<string, object>();
        //     source.Add("name", item.DisplayName); //Device Name + Interface
        //     source.Add("device", item.Device.FullName);
        //     source.Add("processID", item.ProcessId);
        //     source.Add("isSystem", item.IsSystemSession);
        //     source.Add("isMuted", item.IsMuted);
        //     source.Add("state", item.SessionState);
        //     source.Add("volume", item.Volume);
        //     source.Add("iconPath", item.IconPath);
        //     source.Add("exePath", item.ExecutablePath);



        //     if(isJson)
        //         System.Console.WriteLine(JsonConvert.SerializeObject(source,Formatting.Indented));
        //     else
        //         foreach (KeyValuePair<string, object> kvp in source)
        //             Console.WriteLine("{0}: {1}", kvp.Key, kvp.Value);

        //     return source;
        // }

        static public AudioSwitcher.AudioApi.Session.IAudioSession[] getCurrentSources(Boolean system=false){
            var actives=mainDevice.SessionController.All();
            List<AudioSwitcher.AudioApi.Session.IAudioSession> sessions = new List<AudioSwitcher.AudioApi.Session.IAudioSession>();
            List<Dictionary<string, object>> allSrcs= new List<Dictionary<string, object>>();
            foreach (var item in actives)
            {
                if (item.IsSystemSession && !system){
                    continue;
                }
                sessions.Add(item);

                Dictionary<string, object> source = new Dictionary<string, object>();
                source.Add("name", item.DisplayName); //Device Name + Interface
                source.Add("device", item.Device.FullName);
                source.Add("processID", item.ProcessId);
                source.Add("isSystem", item.IsSystemSession);
                source.Add("isMuted", item.IsMuted);
                source.Add("state", item.SessionState);
                source.Add("volume", item.Volume);
                source.Add("iconPath", item.IconPath);
                source.Add("exePath", item.ExecutablePath);
                try{
                    var icon=Icon.ExtractAssociatedIcon(item.ExecutablePath);
                    using (var st = new MemoryStream()){
                        icon.ToBitmap().Save(st, System.Drawing.Imaging.ImageFormat.Png);
                        byte[] iconBytes=st.ToArray();
                        source.Add("iconBytes", Convert.ToBase64String(iconBytes));
                    }
                }catch (System.Exception){System.Console.WriteLine("error");};

                allSrcs.Add(source);
            }

            if(isJson)
                System.Console.WriteLine(JsonConvert.SerializeObject(allSrcs,Formatting.Indented));
            else
                foreach (var src in allSrcs){
                    foreach (KeyValuePair<string, object> kvp in src){
                            Console.WriteLine("{0}: {1}", kvp.Key, kvp.Value);
                    }
                    System.Console.WriteLine(" ");
                }
            AudioSwitcher.AudioApi.Session.IAudioSession[] values = sessions.ToArray();
            return values;
        }

        static public JArray getCurrentSources(Boolean json,Boolean system=false){
            var actives=mainDevice.SessionController.All();
            List<AudioSwitcher.AudioApi.Session.IAudioSession> sessions = new List<AudioSwitcher.AudioApi.Session.IAudioSession>();
            List<Dictionary<string, object>> allSrcs= new List<Dictionary<string, object>>();
            foreach (var item in actives)
            {
                if (item.IsSystemSession && !system){
                    continue;
                }
                sessions.Add(item);

                Dictionary<string, object> source = new Dictionary<string, object>();
                source.Add("name", item.DisplayName); //Device Name + Interface
                source.Add("device", item.Device.FullName);
                source.Add("processID", item.ProcessId);
                source.Add("isSystem", item.IsSystemSession);
                source.Add("isMuted", item.IsMuted);
                source.Add("state", item.SessionState);
                source.Add("volume", item.Volume);
                source.Add("iconPath", item.IconPath);
                source.Add("exePath", item.ExecutablePath);
                try{
                    var icon=Icon.ExtractAssociatedIcon(item.ExecutablePath);
                    using (var st = new MemoryStream()){
                        icon.ToBitmap().Save(st, System.Drawing.Imaging.ImageFormat.Png);
                        byte[] iconBytes=st.ToArray();
                        source.Add("iconBytes", Convert.ToBase64String(iconBytes));
                    }
                }catch (System.Exception){System.Console.WriteLine("error");};

                allSrcs.Add(source);
            }
            return (JArray)JsonConvert.DeserializeObject(JsonConvert.SerializeObject(allSrcs,Formatting.Indented));
        }

        static public Boolean setSourceVolume(int processID,int vol){
            var actives=mainDevice.SessionController.All();
            Dictionary<string, object> response = new Dictionary<string, object>();
            bool result=false;
            if (vol>100 || vol<0){
                response.Add("msg",$"Volume must be between 0 - 100");
                response.Add("result",result);
            }
            else{
                foreach (var src in actives)
                {
                    if (src.ProcessId==processID){
                        System.Console.WriteLine(src.Volume);
                        System.Console.WriteLine(vol);
                        var cVolume=src.Volume;
                        while(Math.Round(src.Volume)!=vol){
                            //-setVol 13148,0
                            if(!isFade){
                                src.Volume=vol;
                                break;
                            }
                            if(src.Volume>vol){
                                src.Volume-=1;
                            }else if(src.Volume<vol){
                                src.Volume+=1;
                            }
                            System.Threading.Thread.Sleep(1);
                        }
                        result=true;
                        response.Add("msg",$"Changed Volume to {vol} for, ID: {processID} | Name: {src.DisplayName}");
                        response.Add("result",result);
                    }
                }

                if (!result){
                    response.Add("msg",$"No Source for, ID: {processID}");
                    response.Add("result",result);
                }
            }
            if(isJson)
                System.Console.WriteLine(JsonConvert.SerializeObject(response,Formatting.Indented));
            else
                foreach (KeyValuePair<string, object> kvp in response)
                    Console.WriteLine("{0}: {1}", kvp.Key, kvp.Value);
            return result;
        }

        static public Boolean setSourceMute(int processID, int type){
            var actives=mainDevice.SessionController.All();
            Dictionary<string, object> response = new Dictionary<string, object>();
            bool result=false;
            foreach (var src in actives)
                {
                    if (src.ProcessId==processID){
                        src.IsMuted=(0 == type);
                        result=true;
                        response.Add("msg",$"Set Mute to {(0 == type)} for, ID: {processID} | Name: {src.DisplayName}");
                        response.Add("result",result);
                    }
                }

                if (!result){
                    response.Add("msg",$"No Source for, ID: {processID}");
                    response.Add("result",result);
            }
            if(isJson)
                System.Console.WriteLine(JsonConvert.SerializeObject(response,Formatting.Indented));
            else
                foreach (KeyValuePair<string, object> kvp in response)
                    Console.WriteLine("{0}: {1}", kvp.Key, kvp.Value);
            return result;
        }

        static public Boolean setMasterVolume(int vol){
            bool result=false;
            Dictionary<string, object> response = new Dictionary<string, object>();
            if (vol>100 || vol<0){
                response.Add("msg",$"Volume must be between 0 - 100");
                response.Add("result",result);
            }
            else{
                try{
                    while(Math.Round(mainDevice.Volume)!=vol){
                        if(!isFade){
                            mainDevice.Volume=vol;
                            break;
                        }
                        if(mainDevice.Volume>vol){
                            mainDevice.Volume-=1;
                        }else if(mainDevice.Volume<vol){
                            mainDevice.Volume+=1;
                        }
                        System.Threading.Thread.Sleep(1);
                    }
                    result=true;
                    response.Add("msg",$"Changed master volume to {vol}");
                    response.Add("result",result);
                }
                catch (System.Exception){
                    result=false;
                    response.Add("msg",$"Error in changing master volume to {vol}");
                    response.Add("result",result);
                }
            }
            if(isJson)
                System.Console.WriteLine(JsonConvert.SerializeObject(response,Formatting.Indented));
            else
                foreach (KeyValuePair<string, object> kvp in response)
                    Console.WriteLine("{0}: {1}", kvp.Key, kvp.Value);
            return result;
        }
        static public Boolean setMasterMute(int type){
            bool result=false;
            Dictionary<string, object> response = new Dictionary<string, object>();
            try{
                    mainDevice.Mute((0 == type));
                    result=true;
                    response.Add("msg",$"Set Master mute to {(0 == type)}");
                    response.Add("result",result);
                }
            catch (System.Exception){
                result=false;
                response.Add("msg",$"Error in changing master mute to {(0 == type)}");
                response.Add("result",result);
            }
            if(isJson)
                System.Console.WriteLine(JsonConvert.SerializeObject(response,Formatting.Indented));
            else
                foreach (KeyValuePair<string, object> kvp in response)
                    Console.WriteLine("{0}: {1}", kvp.Key, kvp.Value);
            return result;
        }
        static void writeJson(JObject json){
            using (FileStream data = new FileStream("callbacks.json",FileMode.Create,FileAccess.Write,FileShare.ReadWrite)){
                using (StreamWriter file = new StreamWriter(data)){
                    using (JsonTextWriter writer = new JsonTextWriter(file))
                    {
                        writer.Formatting = Formatting.Indented;
                        json.WriteTo(writer);
                    }
                }
            }            
        }
        static JObject readJson(){
            try{
                using (FileStream data = new FileStream("callbacks.json",FileMode.Open,FileAccess.Read,FileShare.ReadWrite)){
                    using (StreamReader file = new StreamReader(data)){
                        using (JsonTextReader reader = new JsonTextReader(file))
                        {
                            return (JObject)JToken.ReadFrom(reader);
                        }
                    }
                }    
            }catch(System.Exception){return null;} //Needed incasing reading and writing overlap with external usage    
        }
        static void updateServiceJson(){
            JObject data=readJson();
            if(Int32.Parse(data["serviceID"].ToString())!=0){
                try{
                    Process old=Process.GetProcessById(Int32.Parse(data["serviceID"].ToString()));
                    if(!old.HasExited && old.ProcessName==Process.GetCurrentProcess().ProcessName){                        
                        old.Kill();
                        System.Threading.Thread.Sleep(100);//Breathing room after killing old process
                    }
                }catch(System.Exception){} 
            }
            data["serviceID"]=Process.GetCurrentProcess().Id;
            writeJson(data);
        }
        static void cleanup(){
            GC.Collect();
        }
        static void Warn(string warning){
            if(isLog) return;
            Console.ForegroundColor=ConsoleColor.Yellow;
            System.Console.WriteLine(warning);
            Console.ResetColor();
        }
        static void Error(string error){
            //if(isLog) return;
            Console.ForegroundColor=ConsoleColor.DarkRed;
            System.Console.WriteLine(error);
            Console.ResetColor();
        }
        static void Prompt(string prompt){
            if(isLog) return;
            Console.ForegroundColor=ConsoleColor.DarkGreen;
            System.Console.WriteLine(prompt);
            Console.ResetColor();
        }
        static void main_loop(string[] args){
            //Try to parse argument or print out error with valid options if failed
            string[] fmtArg={"-silent","-q","-json","-c","-clear","-startService","-stopService"};
            string[] optArg={"-device","-sources"};
            string[] valueArg={"-setVol","-setMute","-setMaster","-setMuteMaster"};
            string[] values=valueArg;

            bool validity=false;
            bool nxtValue=false;
            int itr=0;
            try
            {
                if (args.Contains("-clear") || args.Contains("-c")){
                    Console.Clear(); //Clear the annoying package warnings
                }
                if (args.Contains("-silent") || args.Contains("-q")){
                    isLog=true;
                }
                if (args.Contains("-json")){
                    isJson=true;
                }
                foreach (var opt in args)
                {
                    if(nxtValue){
                        try
                        {
                            if(values[itr]=="-setVol" || values[itr]=="-setMute"){
                                var split=opt.Split(',');
                                var test="";
                                test=Int32.Parse(split[0]).ToString();
                                test=Int32.Parse(split[1]).ToString();
                                values[itr]=opt;
                            }
                            else{
                                values[itr]=Int32.Parse(opt).ToString();
                            }
                            validity=true;
                        }
                        catch (System.Exception)
                        {
                            Warn($"Warn: Invalid Value For [{valueArg[itr]}] \"{opt}\" ..Skipping");
                        }
                        nxtValue=false;
                        continue;
                    }

                    if (optArg.Contains(opt)){
                        validity=true;
                    }
                    else if(valueArg.Contains(opt)){
                        nxtValue=true;
                        for (int i = 0; i < valueArg.Length; i++)
                            if(valueArg[i]==opt) itr=i;
                    }
                    else{
                        if(!fmtArg.Contains(opt)){
                            Warn($"Warn: Invalid Option [{opt}] ..Skipping");
                        }
                    }

                }

                if(args.Length<1 || !validity){
                    throw new SystemException();
                }
                for (int i = 0; i < values.Length; i++){
                    if(values[i].StartsWith('-')){
                        values[i]="null";
                    }
                }
            }
            catch (System.Exception)
            {
                Error("Err: No Valid Options Given\n");
                System.Console.WriteLine("\tList of Options");
                string[] validOpts={"-silent/-q","-clear/-c","-json","-device","-sources","-setVol <ProcessID>,<Value>","-setMaster <Value>","-setMuteMaster <State 0/1>","-setMute <State 0/1>","-startService/-stopService"};
                foreach (var item in validOpts)
                    System.Console.WriteLine(item);
                return;
            }

            //Run methods based on input opts

            if(args.Contains("-device")){
                Prompt("Processing Default Audio Device Details");
                getDeviceData();
            }

            if(args.Contains("-sources")){
                Prompt("Processing Default Audio Device Sources");
                var srcs=getCurrentSources();
            }

            if(args.Contains("-setVol") && values[0]!="null"){
                var split=values[0].Split(",");
                Prompt($"Setting Source Volume for Prompt {split[0]} to {split[1]}");
                setSourceVolume(Int32.Parse(split[0]),Int32.Parse(split[1]));
            }

            if(args.Contains("-setMute") && values[1]!="null"){
                var split=values[1].Split(",");
                Prompt($"Setting Source Mute for Prompt {split[0]} to {split[1]}");
                setSourceMute(Int32.Parse(split[0]),Int32.Parse(split[1]));
            }

            if(args.Contains("-setMaster") && values[2]!="null"){
                Prompt($"Setting Master Volume to {values[2]}");
                setMasterVolume(Int32.Parse(values[2]));
            }

            if(args.Contains("-setMuteMaster") && values[3]!="null"){
                Prompt($"Setting Master Mute to {values[3]}");
                setMasterMute(Int32.Parse(values[3]));
            }
        }

        static void service_loop(string[] args){
            bool isService=false;
            if (args.Contains("-startService") && !args.Contains("-stopService")){
                if (!File.Exists("callbacks.json")){
                   JObject callbacksFile = new JObject(
                    new JProperty("serviceID", 0),
                    new JProperty("stop", false),
                    new JProperty("fade", false),
                    new JProperty("calls", ""),
                    new JProperty("device", new JObject()),
                    new JProperty("sources", new JArray())
                    );
                    writeJson(callbacksFile);
                }
                updateServiceJson();
                if (args.Contains("-json")){
                    System.Console.WriteLine("{\"service\":true}");
                    isJson=true;
                } else {
                    Prompt("Starting Service | Reading Commands from callbacks.json | Use -stopService to close service");
                }
                isService=true;
            }else{
                main_loop(args);
            }
            while(isService){
                JObject data=readJson();
                if(data==null){
                    continue;
                }
                //Check If stop call has been made (Intended to stop service externally)
                if((Boolean)data["stop"]){
                    data["stop"]=false;
                    writeJson(data);
                    break;
                }

                isFade=(Boolean)data["fade"];

                //Check If any external calls are made
                if((String)data["calls"]!=""){
                    // String cmd=data["calls"][0].ToString();
                    // main_loop(cmd.Split(" "));
                    // data["calls"][0].Remove();
                    String cmd=data["calls"].ToString();
                    main_loop(cmd.Split(" "));
                    data["calls"]="";
                    writeJson(data);
                }

                bool updated=false;
                //Update Device Data
                JObject DEVICE=getDeviceData(json:true);
                //Update Sources Data
                JArray SOURCES=getCurrentSources(json:true);
                if(data["device"].ToString()!=DEVICE.ToString()){
                    data["device"]=DEVICE;
                    updated=true;
                }
                if(data["sources"].ToString()!=SOURCES.ToString()){
                    data["sources"]=SOURCES;
                    updated=true;
                }

                //Update data to callbacks.json
                if (updated){
                    writeJson(data);
                }
                cleanup();
                Thread.Sleep(10);
            }
        }
        static void Main(string[] args)
        {
            //To Public/Compile Run: dotnet publish -c Release
            service_loop(args);
        }
    }
}
