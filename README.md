# Monster Energy Flappy Bird

A small **C# + raylib-cs** Flappy Bird-style game using the included custom energy-can, obstacle, background, and game-over artwork.

## Download / play

Every push to `main` is built on a Windows GitHub Actions runner. Open **Actions → Build Monster Energy Flappy Bird → latest successful run**, then download the `MonsterEnergyFlappyBird-win-x64` artifact.

Tagged versions (`v1.0.0`, `v1.1.0`, etc.) also create a GitHub Release containing a clean Windows ZIP.

The Windows build uses **.NET Native AOT**, so the C# application code is compiled ahead of time to native machine code and the target PC does not need a separately installed .NET runtime.

## Controls

- **Space / Up / W / left mouse** — flap
- **Space / Enter after death** — retry
- **Escape** — quit

## Build locally

Requirements:

- Windows x64
- .NET 10 SDK
- Visual Studio 2022 Build Tools with **Desktop development with C++**

Then run `build-native-aot.bat`, or:

    dotnet publish MonsterEnergyFlappyBird.csproj -c Release -r win-x64 --self-contained true -o dist

The finished game is placed in `dist/`.

## Repository layout

- `Program.cs` — game loop, physics, collisions, score, retry screen
- `MonsterEnergyFlappyBird.csproj` — .NET / Native AOT project configuration
- `assets/` — game artwork
- `.github/workflows/build.yml` — Windows CI build + artifact + tagged release packaging
- `build-native-aot.bat` — local Native AOT build
- `build-normal-windows.bat` — fallback normal self-contained Windows build

## CI / releases

The workflow restores dependencies and publishes `win-x64` on `windows-latest`, verifies that the EXE exists, packages the complete runnable output, uploads it as an Actions artifact, and publishes the ZIP to GitHub Releases for `v*` tags.

## Note

This is a fan-made game/project and is not affiliated with or endorsed by Monster Energy Company.