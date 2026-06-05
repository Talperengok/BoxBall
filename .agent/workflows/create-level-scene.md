---
description: Level sahnesi oluşturma ve düzenleme adımları
---

# Level Sahnesi Oluşturma Workflow

Bu workflow, Box Ball oyunu için yeni level sahneleri oluşturmayı açıklar.

## Ön Koşullar
- Unity Editor açık olmalı
- SampleScene template olarak kullanılacak

---

## Adım 1: Level Klasörü Oluştur
1. Project panelinde `Assets/Scenes` klasörüne git
2. Sağ tık → Create → Folder → "Levels" adını ver

---

## Adım 2: Template Sahneyi Kopyala
1. `SampleScene.unity` dosyasını seç
2. Ctrl+D ile kopyala
3. Kopyayı "Levels" klasörüne taşı
4. İsmi `Level_01` olarak değiştir
5. Bu işlemi Level_02, Level_03... Level_10 için tekrarla

---

## Adım 3: Her Level İçin Sahne Yapısı

Hierarchy'de şu yapı olmalı:

```
Level_XX
├── GameManager (GameManager.cs)
├── Main Camera (Camera, URP settings)
├── EventSystem (UI için)
├── --- ENVIRONMENT ---
│   ├── Background (SpriteRenderer)
│   ├── Ground (BoxCollider2D)
│   └── Walls (Boundary colliders)
├── --- GAMEPLAY ---
│   ├── SpawnZone (BoxCollider2D - IsTrigger)
│   ├── Ball (Prefab instance - seçilen top)
│   ├── Bat (Player control)
│   └── GoalBox (Level complete trigger)
├── --- LEVEL ITEMS ---
│   ├── [Item 1]
│   ├── [Item 2]
│   └── ...
├── --- COLLECTIBLES ---
│   ├── Star_1 (CollectibleStar prefab)
│   ├── Star_2
│   └── Star_3
└── --- UI ---
    ├── Canvas
    │   ├── StarCounter
    │   ├── Timer (opsiyonel)
    │   └── PauseButton
    └── ...
```

---

## Adım 4: Level Specific Ayarlar

### Level 1 - İlk Vuruş
- Items: YOK
- Ball: Basketball
- Layout: Düz zemin, sol başlangıç, sağ hedef
- Stars: 3 (kolay erişilebilir)

### Level 2 - Zıpla!
- Items: 
  - 1x Trampoline (Prefabs/LevelItems/MovementModifiers/Trampoline)
- Ball: Basketball
- Layout: Hedef yüksekte, trampoline ile ulaşılmalı
- Stars: 3

### Level 3 - Duvarları Kır
- Items:
  - 2x BreakableWall (Prefabs/LevelItems/Obstacles/BreakableWall)
  - 1x Trampoline
- Ball: Football (ağır)
- Layout: Duvarlar yolu kapatıyor

### Level 4 - Işınlan
- Items:
  - 2x Portal (Prefabs/LevelItems/SpecialEffects/Portal)
  - 1x Trampoline
- Ball: Tennis Ball
- Layout: Portal A'dan gir, B'den çık

### Level 5 - Hareketli Engel
- Items:
  - 2x MovingPlatform (Prefabs/LevelItems/Obstacles/MovingPlatform)
  - 1x Trampoline
- Ball: Basketball
- Layout: Platformlar yukarı-aşağı hareket ediyor

### Level 6 - Rüzgar Gülü
- Items:
  - 2x WindArea (Prefabs/LevelItems/MovementModifiers/WindArea)
  - 1x Trampoline
- Ball: Volleyball (hafif)
- Layout: Rüzgar alanları topu yönlendiriyor

### Level 7 - Buz Pisti
- Items:
  - 2x IceZone (Prefabs/LevelItems/MovementModifiers/IceZone)
  - 2x Trampoline
- Ball: Golf Ball (küçük)
- Layout: Buzlu zemin, kaygan

### Level 8 - Hız Treni
- Items:
  - 3x SpeedBoost (Prefabs/LevelItems/SpecialEffects/SpeedBoost)
  - 1x Trampoline
  - 1x BreakableWall
- Ball: Basketball
- Layout: Speed boost'lar sıralı

### Level 9 - Buton Bulmacası
- Items:
  - 2x PressureButton (Prefabs/LevelItems/TargetMechanics/PressureButton)
  - 2x ToggleDoor (Prefabs/LevelItems/Obstacles/ToggleDoor)
  - 1x Trampoline
- Ball: Football
- Layout: Butona basınca kapı açılır

### Level 10 - Karışık Parkur
- Items:
  - 2x Trampoline
  - 2x Portal
  - 1x MovingPlatform
  - 1x SpeedBoost
  - 1x BreakableWall
- Ball: Oyuncu seçer
- Layout: Tüm mekanikler

---

## Adım 5: Build Settings'e Ekle

1. File → Build Settings
2. "Add Open Scenes" ile her level'ı ekle
3. Sıralama:
   - 0: MainMenu
   - 1: Level_01
   - 2: Level_02
   - ...
   - 10: Level_10

---

## Adım 6: Test Et

Her level için:
1. Play mode'a gir
2. Topu vur, level'ı tamamla
3. Tüm yıldızları topla
4. Edge case'leri test et (top sıkışma, vs.)

---

## İpuçları

### Item Yerleştirme
- Prefab'ları Project panelinden Hierarchy'ye sürükle
- Scene view'da pozisyonla
- Inspector'da item-specific ayarları yap

### Collider Ayarları
- Ground: IsTrigger = false
- SpawnZone: IsTrigger = true
- GoalBox: IsTrigger = true

### Katman (Sorting Layer) Sırası
1. Background (-10)
2. Ground (0)
3. Items (5)
4. Ball (10)
5. UI (100)
