using OBSWebsocketDotNet;
using OBSWebsocketDotNet.Types;
using System;
using System.Threading;

namespace OBS_Manejador_dotnet
{
    internal class Program
    {
        protected static OBSWebsocket obs;
        static OutputState recorderStatus;
        static void Main(string[] args)
        {
            

            obs = new OBSWebsocket();
            obs.RecordingStateChanged += onRecordingStateChange;

            Console.WriteLine("Hello World! " + recorderStatus);
            if (!obs.IsConnected)
            {
                try
                {
                    obs.Connect("ws://127.0.0.1:4444", "yourPassword");
                }
                catch (AuthFailureException)
                {
                   Console.WriteLine("Authentication failed.");
                    return;
                }
                catch (ErrorResponseException ex)
                {
                    Console.WriteLine("Connect failed : " + ex.Message);
                    return;
                }
            }
            else
            {
                obs.Disconnect();
            }

            var streamStatus = obs.GetStreamingStatus();

            if (streamStatus.IsRecording)
                onRecordingStateChange(obs, OutputState.Started);
            else
                onRecordingStateChange(obs, OutputState.Stopped);


            if (recorderStatus == OutputState.Stopped || recorderStatus == OutputState.Stopping)
            {
                Console.WriteLine("Orden de iniciar..");
                obs.StartRecording();
            }
            Thread.Sleep(10000);

            if(recorderStatus==OutputState.Started || recorderStatus==OutputState.Starting)
            {
                Console.WriteLine("orden de parar..");
                obs.StopRecording();
            }
            else
            {
                Console.WriteLine("ya está detenido");
            }
            
            //
            Console.ReadKey();
            obs.Disconnect();
        }

        private static void onRecordingStateChange(OBSWebsocket sender, OutputState newState)
        {
            string state = "";
            switch (newState)
            {
                case OutputState.Starting:
                    state = "Recording starting...";
                    break;

                case OutputState.Started:
                    state = "Recording STARTED";
                    break;

                case OutputState.Stopping:
                    state = "Recording stopping...";
                    break;

                case OutputState.Stopped:
                    state = "Recording STOPPED";
                    break;

                default:
                    state = "State unknown";
                    break;
            }
            recorderStatus = newState;
            Console.WriteLine(state);
        }
    }
}
