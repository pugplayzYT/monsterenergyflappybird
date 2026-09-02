using System.Numerics;
using Raylib_cs;
using static Raylib_cs.Raylib;

namespace MonsterEnergyFlappyBird;

internal static class Program
{
    private const int ScreenWidth = 1000;
    private const int ScreenHeight = 700;
    private const float GroundY = 642f;
    private const float PlayerX = 225f;
    private const float PlayerW = 67f;
    private const float PlayerH = 105f;
    private const float Gravity = 1500f;
    private const float FlapVelocity = -515f;
    private const float PipeWidth = 112f;
    private const float PipeGap = 220f;
    private const float PipeSpeed = 245f;
    private const float PipeSpacing = 365f;

    private sealed class PipePair { public float X; public float GapCenter; public bool Scored; }

    [STAThread]
    public static void Main()
    {
        SetConfigFlags(ConfigFlags.VSyncHint | ConfigFlags.Msaa4xHint);
        InitWindow(ScreenWidth, ScreenHeight, "Monster Energy Flappy Bird");
        SetTargetFPS(144);
        string assets = Path.Combine(AppContext.BaseDirectory, "assets");
        Texture2D background = LoadTexture(Path.Combine(assets, "background.png"));
        Texture2D can = LoadTexture(Path.Combine(assets, "can.png"));
        Texture2D pipe = LoadTexture(Path.Combine(assets, "pipe.png"));
        Texture2D gameOver = LoadTexture(Path.Combine(assets, "gameover.png"));
        Image icon = LoadImage(Path.Combine(assets, "can.png")); SetWindowIcon(icon); UnloadImage(icon);
        var rng = new Random(); var pipes = new List<PipePair>(4); ResetPipes(pipes, rng);
        float playerY = 265f, velocityY = 0f; int score = 0, best = 0; bool started = false, dead = false;
        while (!WindowShouldClose())
        {
            float dt = Math.Clamp(GetFrameTime(), 0f, 1f / 30f);
            bool flap = IsKeyPressed(KeyboardKey.Space) || IsKeyPressed(KeyboardKey.Up) || IsKeyPressed(KeyboardKey.W) || IsMouseButtonPressed(MouseButton.Left);
            if (!dead)
            {
                if (flap) { started = true; velocityY = FlapVelocity; }
                if (started)
                {
                    velocityY += Gravity * dt; playerY += velocityY * dt;
                    foreach (PipePair p in pipes) { p.X -= PipeSpeed * dt; if (!p.Scored && p.X + PipeWidth < PlayerX) { p.Scored = true; score++; if (score > best) best = score; } }
                    PipePair leftmost = pipes.OrderBy(p => p.X).First();
                    if (leftmost.X + PipeWidth < -10f) { float rightmostX = pipes.Max(p => p.X); leftmost.X = rightmostX + PipeSpacing; leftmost.GapCenter = NextGapCenter(rng); leftmost.Scored = false; }
                    Rectangle playerRect = PlayerCollisionRect(playerY);
                    if (playerY < -45f || playerY + PlayerH > GroundY) dead = true;
                    else foreach (PipePair p in pipes) { float gapTop = p.GapCenter - PipeGap / 2f, gapBottom = p.GapCenter + PipeGap / 2f; Rectangle topPipe = new(p.X, 0f, PipeWidth, gapTop), bottomPipe = new(p.X, gapBottom, PipeWidth, GroundY - gapBottom); if (CheckCollisionRecs(playerRect, topPipe) || CheckCollisionRecs(playerRect, bottomPipe)) { dead = true; break; } }
                }
            }
            else if (flap || IsKeyPressed(KeyboardKey.Enter)) { playerY = 265f; velocityY = 0f; score = 0; started = false; dead = false; ResetPipes(pipes, rng); }
            BeginDrawing(); ClearBackground(Color.Black);
            if (!dead)
            {
                DrawFullscreen(background); foreach (PipePair p in pipes) DrawPipePair(pipe, p); DrawCan(can, playerY, velocityY);
                DrawTextWithShadow(score.ToString(), ScreenWidth / 2 - MeasureText(score.ToString(), 48) / 2, 32, 48, Color.White);
                if (!started) { DrawCenteredPanel("MONSTER FLAPPY", 270, 44); DrawCenteredPanel("SPACE / CLICK TO FLAP", 330, 24); }
            }
            else { DrawFullscreen(gameOver); string scoreText = $"SCORE  {score}", bestText = $"BEST   {best}"; DrawTextWithShadow(scoreText, ScreenWidth / 2 - MeasureText(scoreText, 42) / 2, 350, 42, new Color(167, 255, 28, 255)); DrawTextWithShadow(bestText, ScreenWidth / 2 - MeasureText(bestText, 26) / 2, 405, 26, Color.White); }
            EndDrawing();
        }
        UnloadTexture(background); UnloadTexture(can); UnloadTexture(pipe); UnloadTexture(gameOver); CloseWindow();
    }
    private static Rectangle PlayerCollisionRect(float y) => new(PlayerX + 11f, y + 8f, PlayerW - 22f, PlayerH - 16f);
    private static void ResetPipes(List<PipePair> pipes, Random rng) { pipes.Clear(); for (int i = 0; i < 4; i++) pipes.Add(new PipePair { X = 720f + i * PipeSpacing, GapCenter = NextGapCenter(rng), Scored = false }); }
    private static float NextGapCenter(Random rng) => rng.Next(210, 485);
    private static void DrawFullscreen(Texture2D tex) => DrawTexturePro(tex, new Rectangle(0, 0, tex.Width, tex.Height), new Rectangle(0, 0, ScreenWidth, ScreenHeight), Vector2.Zero, 0f, Color.White);
    private static void DrawCan(Texture2D tex, float y, float velocityY) { float tilt = Math.Clamp(velocityY / 28f, -16f, 32f); DrawTexturePro(tex, new Rectangle(0, 0, tex.Width, tex.Height), new Rectangle(PlayerX + PlayerW / 2f, y + PlayerH / 2f, PlayerW, PlayerH), new Vector2(PlayerW / 2f, PlayerH / 2f), tilt, Color.White); }
    private static void DrawPipePair(Texture2D tex, PipePair p) { float gapTop = p.GapCenter - PipeGap / 2f, gapBottom = p.GapCenter + PipeGap / 2f, bottomHeight = GroundY - gapBottom; DrawTexturePro(tex, new Rectangle(0, tex.Height, tex.Width, -tex.Height), new Rectangle(p.X, 0, PipeWidth, gapTop), Vector2.Zero, 0f, Color.White); DrawTexturePro(tex, new Rectangle(0, 0, tex.Width, tex.Height), new Rectangle(p.X, gapBottom, PipeWidth, bottomHeight), Vector2.Zero, 0f, Color.White); }
    private static void DrawCenteredPanel(string text, int y, int size) { int width = MeasureText(text, size), x = ScreenWidth / 2 - width / 2; DrawRectangleRounded(new Rectangle(x - 18, y - 8, width + 36, size + 18), 0.35f, 12, new Color(0, 0, 0, 180)); DrawTextWithShadow(text, x, y, size, new Color(171, 255, 20, 255)); }
    private static void DrawTextWithShadow(string text, int x, int y, int size, Color color) { DrawText(text, x + 3, y + 3, size, new Color(0, 0, 0, 190)); DrawText(text, x, y, size, color); }
}
