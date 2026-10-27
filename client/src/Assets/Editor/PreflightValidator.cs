using System;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class PreflightValidator
{
    [MenuItem("JX1M/Run Preflight Verification")]
    public static void RunChecks()
    {
        Debug.Log("==================================================");
        Debug.Log("  JX1M CLIENT PRE-FLIGHT ASSET & FLOW VALIDATOR   ");
        Debug.Log("==================================================");

        int errors = 0;

        // 1. Check Essential Config XMLs
        Debug.Log("\n[Check 1/3] Checking Essential Config Files...");
        string[] essentialConfigs = new string[]
        {
            "Assets/Config/MapConfig.xml",
            "Assets/Config/Faction.xml",
            "Assets/Config/CharacterActionSet.xml"
        };

        foreach (string cfg in essentialConfigs)
        {
            if (File.Exists(cfg))
            {
                Debug.Log($"  [OK] Found config: {cfg}");
            }
            else
            {
                Debug.LogError($"  [FAIL] Missing config file: {cfg}");
                errors++;
            }
        }

        // 2. Check Map Bundles & Data Decompression
        Debug.Log("\n[Check 2/3] Checking Starter Map Bundles Decompression...");
        string[] mapBundles = new string[]
        {
            "Assets/StreamingAssets/Data/MapConfig/BaLangHuyen.unity3d",
            "Assets/StreamingAssets/Data/MapConfig/GiangTanThon.unity3d",
            "Assets/StreamingAssets/Data/MapConfig/BienKinh.unity3d"
        };

        foreach (string mapPath in mapBundles)
        {
            if (!File.Exists(mapPath))
            {
                Debug.LogError($"  [FAIL] Map bundle not found on disk: {mapPath}");
                errors++;
                continue;
            }

            try
            {
                byte[] rawBytes = File.ReadAllBytes(mapPath);
                AssetBundle bundle = AssetBundle.LoadFromMemory(rawBytes);
                if (bundle == null)
                {
                    Debug.LogError($"  [FAIL] Unable to decompress AssetBundle: {mapPath}");
                    errors++;
                }
                else
                {
                    TextAsset mapSetting = bundle.LoadAsset<TextAsset>("MapSetting") ?? bundle.LoadAsset<TextAsset>("MapSetting.xml");
                    TextAsset obs = bundle.LoadAsset<TextAsset>("Obs") ?? bundle.LoadAsset<TextAsset>("Obs.txt");
                    
                    if (mapSetting != null && obs != null)
                    {
                        Debug.Log($"  [OK] Bundle '{Path.GetFileName(mapPath)}' loaded with MapSetting ({mapSetting.bytes.Length} bytes) and Obs ({obs.bytes.Length} bytes)");
                    }
                    else
                    {
                        Debug.LogWarning($"  [WARN] Bundle '{Path.GetFileName(mapPath)}' loaded but MapSetting={mapSetting != null}, Obs={obs != null}");
                    }
                    bundle.Unload(true);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"  [FAIL] Exception loading {mapPath}: {ex.Message}");
                errors++;
            }
        }

        // Check BaLangHuyen Music & Ground Tiles
        string musicPath = "Assets/StreamingAssets/Data/Resources/Map/BaLangHuyen/Music.unity3d";
        if (File.Exists(musicPath))
        {
            AssetBundle mb = AssetBundle.LoadFromFile(musicPath);
            if (mb != null)
            {
                AudioClip clip = mb.LoadAsset<AudioClip>("Music");
                if (clip != null) Debug.Log("  [OK] BaLangHuyen Music.unity3d successfully loaded AudioClip 'Music'!");
                else { Debug.LogError("  [FAIL] Music.unity3d loaded but AudioClip 'Music' is missing!"); errors++; }
                mb.Unload(true);
            }
            else { Debug.LogError("  [FAIL] Could not load Music.unity3d bundle!"); errors++; }
        }
        else { Debug.LogError("  [FAIL] Music.unity3d not found!"); errors++; }

        string tile1Path = "Assets/StreamingAssets/Data/Resources/Map/BaLangHuyen/Image/1.unity3d";
        if (File.Exists(tile1Path))
        {
            AssetBundle tb = AssetBundle.LoadFromFile(tile1Path);
            if (tb != null)
            {
                Sprite[] sp = tb.LoadAllAssets<Sprite>();
                if (sp != null && sp.Length > 0) Debug.Log($"  [OK] BaLangHuyen Image/1.unity3d successfully loaded Sprite '{sp[0].name}'!");
                else { Debug.LogError("  [FAIL] Image/1.unity3d loaded but no Sprite found!"); errors++; }
                tb.Unload(true);
            }
            else { Debug.LogError("  [FAIL] Could not load Image/1.unity3d bundle!"); errors++; }
        }
        else { Debug.LogError("  [FAIL] Image/1.unity3d not found!"); errors++; }

        // 3. Check Starter Character Sprites & SubAssets
        Debug.Log("\n[Check 3/3] Checking Starter Character Animation Bundles...");
        string[] charBundles = new string[]
        {
            "Assets/StreamingAssets/Data/Resources/Character/Woman/Body/body20/FreeStand1_0.unity3d",
            "Assets/StreamingAssets/Data/Resources/Character/Woman/Head/helm20/FreeStand1_0.unity3d",
            "Assets/StreamingAssets/Data/Resources/Character/Man/Body/body20/FreeStand1_0.unity3d",
            "Assets/StreamingAssets/Data/Resources/Character/Man/Head/helm20/FreeStand1_0.unity3d",
            "Assets/StreamingAssets/Data/Resources/Character/Man/Weapon/weapon02/FreeStand1_0.unity3d"
        };

        foreach (string charPath in charBundles)
        {
            if (!File.Exists(charPath))
            {
                Debug.LogError($"  [FAIL] Character bundle missing: {charPath}");
                errors++;
                continue;
            }

            try
            {
                AssetBundle bundle = AssetBundle.LoadFromFile(charPath);
                if (bundle == null)
                {
                    Debug.LogError($"  [FAIL] Failed to load character bundle: {charPath}");
                    errors++;
                    continue;
                }

                Sprite[] sprites = bundle.LoadAllAssets<Sprite>();
                if (sprites == null || sprites.Length == 0)
                {
                    Debug.LogError($"  [FAIL] No Sprite assets found inside bundle: {charPath}");
                    errors++;
                }
                else
                {
                    Debug.Log($"  [OK] Bundle '{Path.GetFileName(Path.GetDirectoryName(charPath))}/{Path.GetFileName(charPath)}' has {sprites.Length} Sprite sub-assets (First: {sprites[0].name})");
                }

                bundle.Unload(true);
            }
            catch (Exception ex)
            {
                Debug.LogError($"  [FAIL] Exception loading {charPath}: {ex.Message}");
                errors++;
            }
        }

        Debug.Log("==================================================");
        if (errors == 0)
        {
            Debug.Log("  PREFLIGHT RESULT: ALL CLIENT ASSETS VERIFIED (PASS)!  ");
            Debug.Log("==================================================");
            if (Application.isBatchMode)
            {
                EditorApplication.Exit(0);
            }
        }
        else
        {
            Debug.LogError($"  PREFLIGHT RESULT: FAILED WITH {errors} ERRORS!  ");
            Debug.Log("==================================================");
            if (Application.isBatchMode)
            {
                EditorApplication.Exit(1);
            }
        }
    }
}
