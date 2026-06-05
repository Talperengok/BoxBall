# Ball & Equipment UI Setup Checklist

## ❌ Problem
Ball Select'te toplar gözükmüyor, preview image görünmüyor.

## ✅ Çözüm - Unity Inspector'da Yapılacaklar

### Adım 1: MainMenu Sahnesini Aç
1. `Assets/Scenes/MainMenu.unity` sahnesini aç

---

### Adım 2: BallSelectPanel'i Bul ve Ayarla

1. Hierarchy'de **BallSelectPanel** objesini bul
2. **BallSelectUI** component'ini seç
3. Inspector'da şu alanları doldur:

#### UI References:
| Alan | Bağlanacak Obje |
|------|-----------------|
| `Back Button` | BallSelectPanel altındaki BackButton |
| `Ball Grid Container` | Topların yerleşeceği Grid/Panel |
| `Ball Item Prefab` | `Assets/Prefabs/UI/BallItemPrefab` |
| `Preview Image` | Seçili topun büyük görüntüsü için Image |
| `Ball Name Text` | Top adı için TextMeshPro |
| `Ball Description Text` | Açıklama için TextMeshPro |
| `Select Button` | "Seç" butonu |

#### Available Balls Array (5 eleman):
```
[0] Basketball
    - Ball Name: "Basketbol"
    - Description: "Standart top, dengeli fizik"
    - Icon: Assets/Materials/Balls/basketball.png
    - Ball Color: Turuncu (FF6B00)
    - Is Unlocked: ✓
    - Unlock Cost: 0

[1] Tennis Ball
    - Ball Name: "Tenis Topu"
    - Description: "Hafif ve yüksek zıplayıcı"
    - Icon: Assets/Materials/Balls/tennisball.png
    - Ball Color: Sarı-yeşil
    - Is Unlocked: ✓
    - Unlock Cost: 0

[2] Football
    - Ball Name: "Futbol Topu"
    - Description: "Ağır, güçlü vuruş için ideal"
    - Icon: Assets/Materials/Balls/Football.png
    - Ball Color: Beyaz
    - Is Unlocked: ✓
    - Unlock Cost: 0

[3] Golf Ball
    - Ball Name: "Golf Topu"
    - Description: "Küçük ve hassas kontrol"
    - Icon: Assets/Materials/Balls/GolfBall.png
    - Ball Color: Beyaz
    - Is Unlocked: ✓
    - Unlock Cost: 0

[4] Volleyball
    - Ball Name: "Voleybol Topu"
    - Description: "Hafif, rüzgardan etkilenir"
    - Icon: Assets/Materials/Balls/Volleyball.png
    - Ball Color: Beyaz/Sarı
    - Is Unlocked: ✓
    - Unlock Cost: 0
```

---

### Adım 3: EquipmentPanel'i Bul ve Ayarla

1. Hierarchy'de **EquipmentPanel** objesini bul
2. **EquipmentUI** component'ini seç
3. Inspector'da şu alanları doldur:

#### UI References:
| Alan | Bağlanacak Obje |
|------|-----------------|
| `Back Button` | EquipmentPanel altındaki BackButton |
| `Equipment Grid Container` | Item'ların yerleşeceği Grid/Panel |
| `Equipment Item Prefab` | `Assets/Prefabs/UI/EquipmentItemPrefab` |
| `Preview Image` | Seçili item'ın büyük görüntüsü için Image |
| `Equipment Name Text` | Item adı için TextMeshPro |
| `Equipment Description Text` | Açıklama için TextMeshPro |
| `Stats Text` | İstatistikler için TextMeshPro |
| `Equip Button` | "Donat" butonu |

#### Category Tabs:
| Alan | Bağlanacak Obje |
|------|-----------------|
| `Bats Tab Button` | Sopalar sekmesi butonu |
| `Power Ups Tab Button` | Güçlendiriciler sekmesi |
| `Skins Tab Button` | Skinler sekmesi |

#### Bats Array (2 eleman):
```
[0] Default Bat
    - Equipment Name: "Beyzbol Sopası"
    - Description: "Standart sopa, dengeli güç"
    - Icon: Assets/Materials/Bats/bat.png
    - Power Bonus: 0
    - Speed Bonus: 0
    - Is Unlocked: ✓
    - Unlock Cost: 0

[1] Golf Bat
    - Equipment Name: "Golf Sopası"
    - Description: "Hassas kontrol, düşük güç"
    - Icon: Assets/Materials/Bats/GolfBat.png
    - Power Bonus: -10
    - Speed Bonus: 20
    - Is Unlocked: ✓
    - Unlock Cost: 0
```

---

### Adım 4: Sprite Import Ayarları

Her sprite için (Materials/Balls ve Materials/Bats klasörlerinde):

1. Sprite dosyasını seç
2. Inspector'da:
   - **Texture Type:** `Sprite (2D and UI)`
   - **Pixels Per Unit:** `100`
3. **Apply** butonuna tıkla

---

### Adım 5: Test Et

1. Play moduna gir
2. Main Menu'den "Balls" butonuna tıkla
3. Toplar grid'de görünmeli
4. Bir topa tıkla → Preview güncellenmelii
5. "Equipment" butonuna tıkla
6. Bats sekmesinde sopalar görünmeli

---

## 🔍 Hata Ayıklama

### Toplar gözükmüyorsa:
- `availableBalls` array'i boş olabilir
- `ballGridContainer` referansı eksik olabilir
- `ballItemPrefab` referansı eksik olabilir

### Preview gözükmüyorsa:
- `previewImage` referansı eksik
- Sprite'lar Texture Type = Sprite olarak ayarlanmamış olabilir

### Console'da hata varsa:
- "Missing required references" → Yukarıdaki referansları kontrol et
