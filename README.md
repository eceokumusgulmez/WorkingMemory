# WorkingMemory 🧠

A small Unity mini-game that trains and assesses working memory using audio-visual stimuli, a concentration bar, and dynamic rules. The game alternates turns between the player and the AI, plays a short sound, shows symbols, and applies penalties for specific mistakes.

---

## 🚀 Features

- Audio-visual matching gameplay with 4 symbols (ball, cat, crown, drop)
- Concentration bar that slowly depletes and can be replenished (Space / clicks)
- Dynamic forbidden symbol that changes every 10 rounds
- Button shuffling every 10 rounds
- Timer (default 6 minutes) and final score panel
- Configurable in the Unity Inspector via `MollymawkManager` component

---

## 🔧 Requirements

- Unity Editor: **2022.3.62f3** (project settings use this version)
- Recommended to run in the Unity Editor for development and testing

---

## ▶️ How to run

1. Open the project in Unity (recommended version shown above).
2. Open `Assets/Scenes/SampleScene.unity` in the Editor.
3. Locate the `MollymawkManager` component (attached to a GameObject in the scene).
4. Assign sprites and audio clips in the Inspector (see the configuration section below).
5. Press **Play** to start the scene. The game will show a loading panel before gameplay begins.

---

## 🛠️ Configuration (Inspector)

Attach appropriate assets to these public fields on `MollymawkManager`:

- **Audio**: `sesKaynagi`, `ballSes`, `catSes`, `crownSes`, `dropSes`, or `sembolSesleri` array
- **Sprites**: `ballSprite`, `catSprite`, `crownSprite`, `dropSprite`, and `sembolResimleri` array
- **UI**: `solKutuImage`, `sagKutuImage`, `timerText`, `finalPanel`, `finalPuanText`
- **Concentration**: `konsantrasyonSlider`, `dususHizi`, `yukselisMiktari`
- **Warnings**: `kirmiziPanel`, `bipSesi`
- **Rules**: `yasakliGostergeImage`
- **Buttons**: `oyunButonlari` (drag Button `RectTransform`s here to allow shuffling)
- **Loading**: `loadingPaneli`, `loadingBarImage`

Ensure all referenced UI elements and audio sources are present in the scene.

---

## 🎮 Controls & Gameplay

- Click the appropriate symbol button when it's your turn.
- Press **Space** or click on an empty area to increase the concentration bar.
- Scoring rules (implemented in `ButonaBasildi`):
  - **-2 points** if you press the symbol that is shown in the AI's target (`sagKutu`) or press the one that matches the sound you heard
  - **-2 points** if you press the current forbidden symbol
  - **+1 point** for a correct non-penalized choice
- The forbidden symbol changes every 10 rounds, and the button positions are shuffled every 10 rounds.
- Game length defaults to **6 minutes (360 seconds)**. After the time runs out, the final panel is shown.
- Final score threshold: **>= 90** considered successful (message text color is set accordingly).

---

## 📂 Notable files

- `Assets/MollymawkManager.cs` — Core game logic and rules
- `Assets/Scenes/SampleScene.unity` — Example scene to run the game
- `Assets/*.png` and `Assets/*.wav` — Example assets (ball, cat, crown, drop)
- `ProjectSettings/ProjectVersion.txt` — Unity editor version used

---

## 🧩 Extending & Customizing

- Add more symbols by extending the `semboller` array and `sembolResimleri`/`sembolSesleri` arrays.
- Adjust `dususHizi` and `yukselisMiktari` to tweak the concentration mechanic difficulty.
- Modify scoring and rule frequency in `MollymawkManager.cs` for experiment-specific behavior.

---

## ✅ Contributing

Contributions and improvements are welcome. Please open an issue or a pull request with a clear description of what you'd like to change.

---

## 📜 License

This repository does not include a license file. If you intend to share or distribute the project, consider adding a suitable license (e.g., MIT).

---

If you'd like, I can:
- Add an example `BUILD.md` or `CONTRIBUTING.md` ✅
- Add a license file (e.g., MIT) ✅
- Add screenshots and a short demo GIF to the README ✅

ENJOY!