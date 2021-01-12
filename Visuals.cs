using System;
using System.Numerics;
using Raylib_cs;
using PaperSDL;

public class Visuals : Module {
    public Visuals(Pluto pluto) : base(pluto) {}

    // centered objects
    CenteredTexture[] stars;
    Color[] starColors;
    CenteredTexture symbol;
    CenteredText timeText;
    TextObject[] consoleText = new TextObject[28];

    private CenteredRectangle header;
    private CenteredRectangle volumeSlider;
    private Circle volumeHandle;
    private float brightness = 1f;
    private CenteredRectangle brightnessSlider;
    private Circle brightnessHandle;
    private CenteredRectangle brightnessPanel;
    private CenteredRectangle mainPanel;

    public static Color[] colors = new Color[]{Color.YELLOW, Color.GOLD, Color.ORANGE, Color.PINK, Color.RED, Color.MAROON, Color.GREEN, Color.LIME, Color.DARKGREEN, Color.SKYBLUE, Color.BLUE, Color.DARKBLUE, Color.PURPLE, Color.VIOLET, Color.DARKPURPLE};
    public static Color[] plutoColors = new Color[]{new Color(69,66,60,255), new Color(41,31,29,255), new Color(27,8,5,255), new Color(87,61,60,255), new Color(163,69,64,255)};

    // fonts
    FontData fontData;
    FontData timeFontData;


    public override void Start() {
        int width = pluto.width;
        int height = pluto.height;


        // stars
        stars = new CenteredTexture[25];
        starColors = new Color[stars.Length];

        GenerateStars();

        // fonts
        fontData = new FontData(Raylib.LoadFont("assets/Square.ttf"), 18f);
        timeFontData = new FontData(fontData.font, 24);

        // pluto image
        symbol = new CenteredTexture(new Vector2(width/2, height/2), Raylib.LoadTexture("assets/pluto.png"));

        // header bar
        header = new CenteredRectangle(new Vector2(width/2, 15), new Vector2(width, 30));
        volumeSlider = new CenteredRectangle(new Vector2(header.position.X+275, header.position.Y), new Vector2(100, 10));
        brightnessSlider = new CenteredRectangle(new Vector2(header.position.X-275, header.position.Y), new Vector2(100, 10));

        // handles
        volumeHandle = new Circle(new Vector2(volumeSlider.literalPosition.X, volumeSlider.position.Y), 8f);
        brightnessHandle = new Circle(new Vector2(brightnessSlider.literalPosition.X, brightnessSlider.position.Y), 8f);

        // panels
        brightnessPanel = new CenteredRectangle(new Vector2(width/2, height/2), new Vector2(width, height));
        mainPanel = new CenteredRectangle(new Vector2(width/2, height/2+15), new Vector2(width, height-30));

        // text objects
        timeText = new CenteredText(header.position, timeFontData, Color.WHITE, "TI:ME");

        for(int i = 0; i < consoleText.Length; i++) {
            consoleText[i] = new TextObject(new Vector2(10, 35+(i*20)), fontData, Color.WHITE);
        }

        UpdateVolumeSlider(pluto.musicSystem.volume);
        brightnessHandle.position = new Vector2(GetRelativePosition(brightnessSlider.literalPosition.X, brightnessSlider.literalPosition.X + brightnessSlider.size.X, 0.99f), volumeHandle.position.Y);
    }


    public override void Update()
    {
        timeText.SetText(DateTime.Now.ToString("MM-dd").Replace("-","/") + "  |  " + Pluto.GetTime().ToUpper());

        if(Raylib.IsKeyPressed(KeyboardKey.KEY_R)) {
            GenerateStars();
        }

        if(PaperUtils.RectClick(volumeSlider.GetRectangle())) {
            volumeHandle.position = new Vector2(Raylib.GetMouseX(), volumeHandle.position.Y);
            float newVolume = GetRelativePercentage(volumeSlider.literalPosition.X, volumeSlider.literalPosition.X + volumeSlider.size.X, Raylib.GetMouseX());
            // Console.WriteLine(newVolume);
            pluto.musicSystem.SetVolume(newVolume);
        }

        if(PaperUtils.RectClick(brightnessSlider.GetRectangle())) {
            brightnessHandle.position = new Vector2(Raylib.GetMouseX(), brightnessHandle.position.Y);
            brightness = GetRelativePercentage(brightnessSlider.literalPosition.X, brightnessSlider.literalPosition.X + brightnessSlider.size.X, Raylib.GetMouseX());
        
            // Console.WriteLine(brightness);
        }
    }

    public override void Draw() {

        if(brightness <= 0.05f)
            return;

        for(int i = 0; i < stars.Length; i++) {
            Raylib.DrawTexture(stars[i].GetTexture(), (int)stars[i].literalPosition.X, (int)stars[i].literalPosition.Y, starColors[i]);
        }

        PaperUtils.DrawCenteredObject(symbol);

        Raylib.DrawRectangle((int)header.literalPosition.X, (int)header.literalPosition.Y, (int)header.size.X, (int)header.size.Y, new Color(25,25,25,255));
        
        // sliders
        Raylib.DrawRectangle((int)volumeSlider.literalPosition.X, (int)volumeSlider.literalPosition.Y, (int)volumeSlider.size.X, (int)volumeSlider.size.Y, Color.GRAY);
        Raylib.DrawRectangle((int)brightnessSlider.literalPosition.X, (int)brightnessSlider.literalPosition.Y, (int)brightnessSlider.size.X, (int)brightnessSlider.size.Y, Color.GRAY);

        // handles
        PaperUtils.DrawCircle(volumeHandle, Color.DARKGRAY);
        PaperUtils.DrawCircle(brightnessHandle, Color.DARKGRAY);

        Raylib.DrawRectangle((int)mainPanel.literalPosition.X, (int)mainPanel.literalPosition.Y, (int)mainPanel.size.X, (int)mainPanel.size.Y, new Color(0,0,0,140));

        timeText.Draw();

        for(int i = 0; i < consoleText.Length; i++) {
            consoleText[i].Draw();
        }

        Raylib.DrawRectangle((int)brightnessPanel.literalPosition.X, (int)brightnessPanel.literalPosition.Y, (int)brightnessPanel.size.X, (int)brightnessPanel.size.Y, new Color(0,0,0,(int)(255-(brightness*255))));

    }

    public Vector2 GetStarPos() {
        Vector2 toReturn = new Vector2(new Random().Next(10, pluto.width-10), new Random().Next(100, pluto.height-100));

        if(Raylib.CheckCollisionPointCircle(toReturn, new Vector2(pluto.width/2, pluto.height/2), 140)) {
            return GetStarPos();
        }

        for(int i = 0; i < stars.Length; i++) {
            if(stars[i] == null)
                break;

            if(Vector2.Distance(toReturn, stars[i].literalPosition) < 65)
                return GetStarPos();
        }

        return toReturn;
    }

    public void GenerateStars() {
        Texture2D tex = Raylib.LoadTexture("assets/star.png");
        for(int i = 0; i < stars.Length; i++) {
            stars[i] = new CenteredTexture(GetStarPos(), tex);
            starColors[i] = (Color)GetRandomObject(plutoColors);
        }
    }

    public static Object GetRandomObject(Array objects) {
        return objects.GetValue(Raylib.GetRandomValue(0, objects.Length-1));
    }

    public TextObject GetEmptyText() {
        foreach(TextObject text in consoleText) {
            if(String.IsNullOrEmpty(text.text)) {
                return text;
            }
        }

        // we could not find a text object, shifting
        for(int i = 0; i < consoleText.Length; i++) {
            if(i == consoleText.Length-1)
                return consoleText[i];
            
            consoleText[i].SetText(consoleText[i+1].text);
        }

        return null;

    }

    public void Log(string text) {
        GetEmptyText()?.SetText(String.Format("[{0}] {1}", DateTime.Now.ToString("HH:mm:ss"), text));
    }

    private float GetRelativePercentage(float min, float max, float value) {
        return (value-min) / (max-min);
    }

    private float GetRelativePosition(float min, float max, float percentage) {
        return ((max - min) * percentage) + min;
    }

    public void UpdateVolumeSlider(float newValue) {
        volumeHandle.position = new Vector2(GetRelativePosition(volumeSlider.literalPosition.X, volumeSlider.literalPosition.X + volumeSlider.size.X, newValue), volumeHandle.position.Y);
    }
}