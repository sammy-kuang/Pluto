using System;
using System.Net.Http;
using System.Net;
using Newtonsoft.Json.Linq;
using Websocket.Client;
using System.Threading;

public class Rhasspy : Module{

    Uri url = new Uri("ws://localhost:12101/api/events/intent");
    string tts = "http://localhost:12101/api/text-to-speech";
    HttpClient client = new HttpClient();

    WebClient wb = new WebClient();
    WebsocketClient websocketClient;
    ManualResetEvent exitEvent = new ManualResetEvent(false);

    Thread listeningThread;

    public Rhasspy(Pluto pluto) : base(pluto) {}

    public void Call() {
        ThreadStart ts = new ThreadStart(Run);
        listeningThread = new Thread(ts);
        listeningThread.Start();
    }

    public void Log(string text) {
        pluto.Log(text);
    }

    public void Run() {
        using (websocketClient = new WebsocketClient(url)) {
            websocketClient.MessageReceived.Subscribe(msg => MessageReceived(msg));
            websocketClient.Start();
            Log("opened a websocket");
            exitEvent.WaitOne();
        }
    }

    public void MessageReceived(object text) {
        dynamic parsed = JObject.Parse(text.ToString());
        string intent = parsed.intent.name;

        LogIntent(intent);
        ParseIntent(intent, parsed);
    }

    public void ParseIntent(string intent, dynamic json) {
        switch(intent) {
            case "GetTime":
                Say(Pluto.GetTime());
                break;
            case "PlaySong":
                pluto.musicSystem.PlayIndex(int.Parse(json.slots.index.ToString())-1);
                break;
            case "StopSong":
                pluto.musicSystem.StopSong();
                break;
            case "Volume":
                string state = json.slots.state.ToString();
                if(state == "down") {
                    pluto.Log("has turned the volume down");
                    pluto.musicSystem.ModifyVolume(-0.05f);
                }
                else if (state == "up") {
                    pluto.Log("has turned the volume up");
                    pluto.musicSystem.ModifyVolume(0.05f);
                }
                break;
            case "Shutdown":
                pluto.shouldClose = true;
                break;
            default:
                pluto.Log("failed to catch an intent");
                break;
        }
    }

    public void LogIntent(string intent) {
        Log(String.Format("caught {0} intent", intent));
    }

    public void Say(string text) {
        wb.UploadString(tts, text);
    }

    public override void Close() {
        Log("preparing to close Rhasspy thread, ignore core dumped!");
        exitEvent.Set();
        websocketClient.Dispose();
        listeningThread.Interrupt();
    }
}
