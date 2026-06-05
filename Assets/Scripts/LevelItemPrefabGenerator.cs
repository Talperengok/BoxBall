#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

/// <summary>
/// Level item prefab'larını otomatik oluşturan Editor script.
/// Unity Editor menüsünden: Tools > Level Items > Generate All Prefabs
/// </summary>
public class LevelItemPrefabGenerator : EditorWindow
{
    private static string prefabPath = "Assets/Prefabs/LevelItems";
    
    [MenuItem("Tools/Level Items/Generate All Prefabs")]
    public static void GenerateAllPrefabs()
    {
        CreateFolderStructure();
        
        // Movement Modifiers
        CreatePrefab("WindArea", "MovementModifiers", typeof(BoxCollider2D), true, new Vector2(3f, 3f), new Color(0.5f, 0.8f, 1f, 0.3f));
        CreatePrefab("Trampoline", "MovementModifiers", typeof(BoxCollider2D), false, new Vector2(2f, 0.3f), new Color(1f, 0.5f, 0f, 1f));
        CreatePrefab("MagneticField", "MovementModifiers", typeof(CircleCollider2D), true, 3f, new Color(0.5f, 0f, 1f, 0.3f));
        CreatePrefab("GravityModifier", "MovementModifiers", typeof(BoxCollider2D), true, new Vector2(4f, 4f), new Color(0.5f, 0f, 0.5f, 0.3f));
        CreatePrefab("IceZone", "MovementModifiers", typeof(BoxCollider2D), true, new Vector2(5f, 1f), new Color(0.7f, 0.9f, 1f, 0.5f));
        CreatePrefab("StickyZone", "MovementModifiers", typeof(BoxCollider2D), true, new Vector2(3f, 1f), new Color(0.4f, 0.2f, 0f, 0.5f));
        
        // Obstacles
        CreatePrefab("RotatingWheel", "Obstacles", typeof(CircleCollider2D), false, 1f, new Color(0.6f, 0.6f, 0.6f, 1f), true);
        CreatePrefab("ToggleDoor", "Obstacles", typeof(BoxCollider2D), false, new Vector2(0.5f, 3f), Color.red);
        CreatePrefab("BreakableWall", "Obstacles", typeof(BoxCollider2D), false, new Vector2(2f, 2f), new Color(0.6f, 0.4f, 0.2f, 1f));
        CreatePrefab("LaserBarrier", "Obstacles", typeof(BoxCollider2D), true, new Vector2(5f, 0.2f), Color.red);
        CreatePrefab("MovingPlatform", "Obstacles", typeof(BoxCollider2D), false, new Vector2(3f, 0.5f), new Color(0.4f, 0.4f, 0.8f, 1f), true);
        
        // Special Effects
        CreatePrefab("Portal", "SpecialEffects", typeof(CircleCollider2D), true, 0.8f, new Color(0.2f, 0.2f, 1f, 0.8f));
        CreatePrefab("SpeedBoost", "SpecialEffects", typeof(BoxCollider2D), true, new Vector2(2f, 2f), new Color(1f, 1f, 0f, 0.5f));
        CreatePrefab("SlowMotionZone", "SpecialEffects", typeof(BoxCollider2D), true, new Vector2(3f, 3f), new Color(0.5f, 0.5f, 1f, 0.3f));
        CreatePrefab("SizeChanger", "SpecialEffects", typeof(BoxCollider2D), true, new Vector2(2f, 2f), new Color(1f, 0.5f, 0f, 0.5f));
        CreatePrefab("BouncinessModifier", "SpecialEffects", typeof(BoxCollider2D), true, new Vector2(2f, 2f), new Color(0f, 1f, 0.5f, 0.5f));
        
        // Target Mechanics
        CreatePrefab("PressureButton", "TargetMechanics", typeof(BoxCollider2D), true, new Vector2(1.5f, 0.3f), Color.red);
        CreatePrefab("CollectibleStar", "TargetMechanics", typeof(CircleCollider2D), true, 0.5f, Color.yellow);
        CreatePrefab("SequentialTarget", "TargetMechanics", typeof(CircleCollider2D), true, 0.6f, Color.gray);
        
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        Debug.Log("✅ Tüm Level Item Prefab'ları oluşturuldu: " + prefabPath);
        EditorUtility.DisplayDialog("Başarılı!", "18 Level Item Prefab'ı oluşturuldu!\n\nKonum: " + prefabPath, "Tamam");
    }

    private static void CreateFolderStructure()
    {
        if (!AssetDatabase.IsValidFolder("Assets/Prefabs"))
            AssetDatabase.CreateFolder("Assets", "Prefabs");
            
        if (!AssetDatabase.IsValidFolder(prefabPath))
            AssetDatabase.CreateFolder("Assets/Prefabs", "LevelItems");
            
        string[] subfolders = { "MovementModifiers", "Obstacles", "SpecialEffects", "TargetMechanics" };
        foreach (var folder in subfolders)
        {
            if (!AssetDatabase.IsValidFolder(prefabPath + "/" + folder))
                AssetDatabase.CreateFolder(prefabPath, folder);
        }
    }

    // BoxCollider2D version
    private static void CreatePrefab(string name, string subfolder, Type colliderType, bool isTrigger, Vector2 size, Color color, bool addRigidbody = false)
    {
        GameObject go = new GameObject(name);
        
        if (colliderType == typeof(BoxCollider2D))
        {
            var col = go.AddComponent<BoxCollider2D>();
            col.isTrigger = isTrigger;
            col.size = size;
        }
        
        var sr = go.AddComponent<SpriteRenderer>();
        sr.color = color;
        
        if (addRigidbody)
        {
            var rb = go.AddComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Kinematic;
        }
        
        // Script'i isimle ekle
        var scriptType = Type.GetType(name + ", Assembly-CSharp");
        if (scriptType != null)
        {
            go.AddComponent(scriptType);
        }
        else
        {
            Debug.LogWarning($"Script bulunamadı: {name}");
        }
        
        SavePrefab(go, $"{prefabPath}/{subfolder}/{name}.prefab");
    }
    
    // CircleCollider2D version
    private static void CreatePrefab(string name, string subfolder, Type colliderType, bool isTrigger, float radius, Color color, bool addRigidbody = false)
    {
        GameObject go = new GameObject(name);
        
        if (colliderType == typeof(CircleCollider2D))
        {
            var col = go.AddComponent<CircleCollider2D>();
            col.isTrigger = isTrigger;
            col.radius = radius;
        }
        
        var sr = go.AddComponent<SpriteRenderer>();
        sr.color = color;
        
        if (addRigidbody)
        {
            var rb = go.AddComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Kinematic;
        }
        
        // Script'i isimle ekle
        var scriptType = Type.GetType(name + ", Assembly-CSharp");
        if (scriptType != null)
        {
            go.AddComponent(scriptType);
        }
        else
        {
            Debug.LogWarning($"Script bulunamadı: {name}");
        }
        
        SavePrefab(go, $"{prefabPath}/{subfolder}/{name}.prefab");
    }
    
    private static void SavePrefab(GameObject go, string path)
    {
        PrefabUtility.SaveAsPrefabAsset(go, path);
        UnityEngine.Object.DestroyImmediate(go);
    }
    
    [MenuItem("Tools/Level Items/Open Prefabs Folder")]
    public static void OpenPrefabsFolder()
    {
        if (AssetDatabase.IsValidFolder(prefabPath))
        {
            var folder = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(prefabPath);
            Selection.activeObject = folder;
            EditorGUIUtility.PingObject(folder);
        }
    }
}
#endif
