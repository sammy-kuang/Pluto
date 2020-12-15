using System;
using System.IO;
using Raylib_cs;
using PaperSDL;

public class MusicSystem : Module {
    
    public Music music = new Music();
    public float volume = 0.05f;
    public string[] musicFiles;

    public MusicSystem(Pluto pluto) : base(pluto) {}

    public void PlaySong(string path) {
        Raylib.StopMusicStream(music);
        music = Raylib.LoadMusicStream(path);
        Raylib.PlayMusicStream(music);
        Raylib.SetMusicVolume(music, volume);
    }

    public void StopSong() {
        Raylib.StopMusicStream(music);
    }

    public override void Start() {
        Raylib.InitAudioDevice();
        Raylib.SetMusicLoopCount(music, 0);
    }

    public override void Update()
    {
        Raylib.UpdateMusicStream(music);
    }

    public void PlayIndex(int index) {
        if(index > musicFiles.Length) {
            Console.WriteLine("Specified index out of range. Length of array is: " + musicFiles.Length.ToString());
            return;
        }

        PlaySong(musicFiles[index]);
    }

    public void SetDirectory(string path) {
        musicFiles = Directory.GetFiles(path);
        Array.Sort(musicFiles);
        // for(int i = 0; i < musicFiles.Length; i++) {Console.WriteLine(musicFiles[i]);}
    }

    public void ModifyVolume(float mod) {
        volume += mod;
        Raylib.SetMusicVolume(music, volume);
    }
}