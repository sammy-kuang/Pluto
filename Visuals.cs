using System;
using System.Numerics;
using Raylib_cs;
using PaperSDL;

public class Visuals : Module {
    public Visuals(Pluto pluto) : base(pluto) {}

    // centered objects
    private CenteredTexture[] stars;
    private Color[] starColors;
    public CenteredTexture symbol;
    public CenteredText logText;
    public CenteredText timeText;
    public CenteredObject[] centereds;

    private CenteredRectangle header;

    public static Color[] colors = new Color[]{Color.YELLOW, Color.GOLD, Color.ORANGE, Color.PINK, Color.RED, Color.MAROON, Color.GREEN, Color.LIME, Color.DARKGREEN, Color.SKYBLUE, Color.BLUE, Color.DARKBLUE, Color.PURPLE, Color.VIOLET, Color.DARKPURPLE};
    public static Color[] plutoColors = new Color[]{new Color(69,66,60,255), new Color(41,31,29,255), new Color(27,8,5,255), new Color(87,61,60,255), new Color(163,69,64,255)};

    // fonts
    FontData fontData;
    FontData timeFontData;


    public override void Start() {
        int width = pluto.width;
        int height = pluto.height;


        // stars
        stars = new CenteredTexture[50];
        starColors = new Color[stars.Length];

        GenerateStars();

        // fonts
        fontData = new FontData(Raylib.LoadFont("assets/Square.ttf"), 18f);
        timeFontData = new FontData(Raylib.GetFontDefault(), 56, 5f);

        // pluto image
        symbol = new CenteredTexture(new Vector2(width/2, height/2), Raylib.LoadTexture("assets/pluto.png"));

        // header bar
        header = new CenteredRectangle(new Vector2(width/2, 15), new Vector2(width, 30));


        // text objects
        logText = new CenteredText(new Vector2(width/2, height/4*3.5f), fontData, Color.WHITE, "Pluto initialization");
        timeText = new CenteredText(new Vector2(width/2, height/7), timeFontData, Color.WHITE, "TI:ME");
        centereds = new CenteredObject[3] {logText, timeText, symbol};
   
   
    }


    public override void Update()
    {
        timeText.SetText(Pluto.GetTime().ToUpper());

        if(Raylib.IsKeyPressed(KeyboardKey.KEY_R)) {
            GenerateStars();
        }

    }

    public override void Draw() {
        // foreach(Circle star in stars) {
        //     PaperUtils.DrawCircle(star, Color.WHITE);
        // }

        PaperUtils.DrawCenteredObject(symbol);
        logText.Draw();
        timeText.Draw();

        Raylib.DrawRectangle((int)header.literalPosition.X, (int)header.literalPosition.Y, (int)header.size.X, (int)header.size.Y, new Color(50,50,50,255));

        // foreach(CenteredTexture tex in stars) {
        //     Raylib.DrawTexture(tex.GetTexture(), (int)tex.literalPosition.X, (int)tex.literalPosition.Y, starColors[stars.]);
        // }

        for(int i = 0; i < stars.Length; i++) {
            Raylib.DrawTexture(stars[i].GetTexture(), (int)stars[i].literalPosition.X, (int)stars[i].literalPosition.Y, starColors[i]);
        }
    }

    
    public void RecenterAllObjects() {
        for(int i = 0; i < centereds.Length; i++) {
            centereds[i].Center();
        }
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

    public static Color GetRandomColor() {
        return colors[Raylib.GetRandomValue(0, colors.Length-1)];
    }

    public static Object GetRandomObject(Array objects) {
        return objects.GetValue(Raylib.GetRandomValue(0, objects.Length-1));
    }

    public void Log(string text) {
        logText.SetText(text);
    }
}