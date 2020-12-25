using System;
using PaperSDL;
using Raylib_cs;
using System.Numerics;

public class Pluto : PaperApp
{
    // Modules
    public Rhasspy rhasspy;
    public MusicSystem musicSystem;
    public Visuals visuals;
    
    // UI
    public static int windowWidth = 1280;
    public static int windowHeight = 720;


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

        Raylib.SetWindowMinSize(width, height);
    }

    // overarching start code
    public override void Start() {
        rhasspy = new Rhasspy(this);
        musicSystem = new MusicSystem(this);
        visuals = new Visuals(this);
        
        musicSystem.Start();
        visuals.Start();

        rhasspy.Call();
        musicSystem.SetDirectory("/home/sammy/Music/renamer/"); // assign the initial directory
    }

    // update logic: for music
    public override void Update() {
        musicSystem.Update();
        
        if(!hasGui) return; // events that only happen if we have a window below
        visuals.Update();
    }

    // draw is only called when we have a window. i want to keep it to strictly drawing
    public override void Draw() {
        Raylib.ClearBackground(Color.BLACK);
        visuals.Draw();
        
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
        visuals.Log(log.ToUpper());
    }

    public static string GetTime() {
        return DateTime.Now.ToString("h:mm tt");
    }
}
