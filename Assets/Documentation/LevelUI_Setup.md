# Level UI Kurulum Rehberi

Bu döküman, her level sahnesinde olması gereken UI elementlerinin nasıl kurulacağını açıklar.

---

## 📋 UI Hiyerarşisi

```
Canvas (Screen Space - Overlay)
├── LevelUI (LevelUI.cs script burada)
│
├── HUD (Üst Panel - Her zaman görünür)
│   ├── StarPanel
│   │   ├── StarIcon_1 (Image)
│   │   ├── StarIcon_2 (Image)
│   │   ├── StarIcon_3 (Image)
│   │   └── StarCountText (TextMeshPro) → "0/3"
│   │
│   ├── PauseButton (Button)
│   │   └── PauseIcon (Image)
│   │
│   └── RestartButton (Button) [Opsiyonel]
│       └── RestartIcon (Image)
│
├── PausePanel (Başlangıçta Gizli)
│   ├── Background (Image - yarı saydam siyah)
│   ├── PauseTitle (TextMeshPro) → "PAUSE"
│   ├── ResumeButton (Button) → "Devam Et"
│   ├── RestartButton (Button) → "Yeniden Başla"
│   └── MainMenuButton (Button) → "Ana Menü"
│
└── LevelCompletePanel (Başlangıçta Gizli)
    ├── Background (Image - yarı saydam)
    ├── Title (TextMeshPro) → "LEVEL TAMAMLANDI!"
    │
    ├── StarsPanel
    │   ├── Star_1 (Image)
    │   ├── Star_2 (Image)
    │   └── Star_3 (Image)
    │
    ├── StatsPanel
    │   ├── ScoreText (TextMeshPro) → "Skor: 300"
    │   └── TimeText (TextMeshPro) → "Süre: 12.5s"
    │
    ├── NextLevelButton (Button) → "Sonraki Level"
    ├── ReplayButton (Button) → "Tekrar Oyna"
    └── MenuButton (Button) → "Ana Menü"
```

---

## 🔧 Adım Adım Kurulum

### 1. Canvas Oluştur
1. Hierarchy → Sağ tık → UI → Canvas
2. Canvas Scaler ayarları:
   - UI Scale Mode: `Scale With Screen Size`
   - Reference Resolution: `1920 x 1080`
   - Match: `0.5` (Width ve Height arası)

### 2. LevelUI Script Ekle
1. Canvas altında boş GameObject oluştur → "LevelUI"
2. `LevelUI.cs` script'ini ekle
3. Tüm UI referanslarını Inspector'da bağla

### 3. HUD (Üst Panel) Oluştur

#### Star Panel
```
Pozisyon: Üst-sol köşe
Anchor: Top-Left
```

1. Canvas altında Panel oluştur → "HUD"
2. HUD altında Panel oluştur → "StarPanel"
3. Horizontal Layout Group ekle
4. 3 adet Image ekle (yıldız sprite'ı)
5. TextMeshPro ekle → StarCountText

#### Pause Button
```
Pozisyon: Üst-sağ köşe
Anchor: Top-Right
Size: 60x60
```

### 4. Pause Panel Oluştur
```
Anchor: Stretch (tüm ekranı kapla)
Başlangıçta: Deaktif
```

1. Canvas altında Panel oluştur → "PausePanel"
2. Image component'i yarı saydam siyah yap (0,0,0,0.8)
3. Ortasına butonları ekle:
   - ResumeButton → "Devam Et"
   - RestartButton → "Yeniden Başla"
   - MainMenuButton → "Ana Menü"
4. **GameObject'i deaktif et** (Inspector'da checkbox'ı kaldır)

### 5. Level Complete Panel Oluştur
```
Anchor: Stretch (tüm ekranı kapla)
Başlangıçta: Deaktif
```

1. Canvas altında Panel oluştur → "LevelCompletePanel"
2. Celebratory tasarım (gradient arka plan, parıltılar)
3. 3 yıldız Image'ı (büyük, ortada)
4. Skor ve süre text'leri
5. 3 buton: Next, Replay, Menu
6. **GameObject'i deaktif et**

---

## 🎨 Önerilen Renk Paleti

| Element | Renk (Hex) |
|---------|------------|
| Boş yıldız | `#808080` (Gri, %50 alpha) |
| Dolu yıldız | `#FFD700` (Altın sarısı) |
| Pause arka plan | `#000000` (%80 alpha) |
| Level Complete arka plan | `#1a1a2e` (%90 alpha) |
| Buton - Normal | `#4a4a8a` |
| Buton - Hover | `#6a6aaa` |
| Buton - Text | `#FFFFFF` |

---

## 📐 Anchor Presets Referansı

```
┌─────────────────────────────┐
│  TL        TC        TR    │  TL = Top-Left
│                             │  TC = Top-Center
│  ML        MC        MR    │  TR = Top-Right
│                             │
│  BL        BC        BR    │  Butonlar için:
└─────────────────────────────┘  Pause → TR
                                  Stars → TL
                                  Complete Panel → MC (Stretch)
```

---

## ✅ LevelUI Inspector Bağlantıları

LevelUI script'indeki her slot'u doldur:

### HUD - Üst Panel
- `starCountText` → StarCountText (TextMeshPro)
- `starIcons` → [StarIcon_1, StarIcon_2, StarIcon_3]
- `pauseButton` → PauseButton
- `restartButton` → RestartButton (opsiyonel)

### Pause Menü
- `pausePanel` → PausePanel
- `resumeButton` → ResumeButton
- `mainMenuButton` → MainMenuButton

### Level Complete Panel
- `levelCompletePanel` → LevelCompletePanel
- `levelCompleteStars` → [Star_1, Star_2, Star_3]
- `scoreText` → ScoreText
- `timeText` → TimeText
- `nextLevelButton` → NextLevelButton
- `replayButton` → ReplayButton
- `levelCompleteMenuButton` → MenuButton

---

## 💡 Prefab Olarak Kaydet

Kurulumu tamamladıktan sonra:

1. Canvas'ı seç
2. Project paneline sürükle → `Assets/Prefabs/UI/LevelCanvas.prefab`
3. Artık her yeni level sahnesine bu prefab'ı koyabilirsin!

---

## 🎮 Test Checklist

- [ ] Oyun başladığında HUD görünüyor
- [ ] Yıldız toplandığında sayı güncelleniyor
- [ ] Yıldız ikonları renk değiştiriyor
- [ ] ESC tuşu pause menüyü açıyor
- [ ] Pause butonuna tıklayınca pause oluyor
- [ ] Resume butonu oyunu devam ettiriyor
- [ ] Level tamamlandığında panel açılıyor
- [ ] Next Level butonu çalışıyor
- [ ] Replay butonu level'ı yeniden başlatıyor
- [ ] Menu butonu ana menüye götürüyor
