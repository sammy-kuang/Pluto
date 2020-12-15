using System;
using PaperSDL;
using Raylib_cs;
using System.Numerics;
using System.Threading;

public class Pluto : PaperApp
{
    // Modules
    public Rhasspy rhasspy;
    public MusicSystem musicSystem;
    
    // UI
    public static int windowWidth = 480;
    public static int windowHeight = 320;
    public CenteredTexture symbol;
    public CenteredText logText;
    FontData fontData;


    // Start
    public Pluto() : base(windowWidth, windowHeight, "Pluto") {}
    public static string[] arguments;
    public bool shouldClose = false;
    public bool hasGui = false;

    static void Main(string[] args) {
        arguments = args;
        new Pluto();
    }

    // we need to override the paperstart to decide whether to open OpenGL context or not
    public override void PaperStart() {
        if(Array.Exists<string>(arguments, element => element.Equals("--no-gui"))) {
            hasGui = false;
            Awake();
            Console.WriteLine("PSDL: Ignoring OpenGL context");
            Start();

            while(true) {
                Update();

                if(shouldClose) break;
            }
            Close();
        }
        else {
            hasGui = true;
            Awake();
            Raylib.InitWindow(width, height, title);
            Console.WriteLine("PSDL: OpenGL context opened");
            WindowStart();
            Start();

            while(!Raylib.WindowShouldClose() && !shouldClose) {
                Update();
                PaperDraw();
            }

            Raylib.CloseWindow();
            Close(); 
        }
    }

    // if we need a window:
    public void WindowStart() {
        Raylib.SetTargetFPS(24);
        symbol = new CenteredTexture(new Vector2(windowWidth/2, windowHeight/2), Raylib.LoadTexture("assets/pluto.png"));
        fontData = new FontData(Raylib.LoadFont("assets/EHSMB.TTF"), 15f);
        logText = new CenteredText(new Vector2(width/2, 20), fontData, Color.WHITE, "Pluto initialization");
    }

    // overarching start code
    public override void Start() {
        rhasspy = new Rhasspy(this);
        musicSystem = new MusicSystem(this);
        
        
        musicSystem.Start();

        rhasspy.Call();
        musicSystem.SetDirectory("/home/sammy/Music/renamer/"); // assign the initial directory

    }

    // update logic: for music
    public override void Update()
    {
        musicSystem.Update();
    }

    // draw is only called when we have a window
    public override void Draw() {
        Raylib.ClearBackground(Color.BLACK);
        PaperUtils.DrawCenteredObject(symbol);
        logText.Draw();
    }

    public override void Close() {
        Raylib.UnloadMusicStream(musicSystem.music);
        Raylib.CloseAudioDevice();
        rhasspy.Close();
    }

    public void Log(string text) {
        string log = String.Format("PLUTO: -- pluto {0} --", text);
        Console.WriteLine(log);
        if(!hasGui) return;
        logText.SetText(log);
    }
}
