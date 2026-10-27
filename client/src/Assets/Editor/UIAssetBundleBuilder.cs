using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class UIAssetBundleBuilder
{
    private const string TempAtlasDir = "Assets/Editor/TempAtlas";

    [MenuItem("JX1M/Build UI AssetBundles")]
    public static void BuildAllUIBundles()
    {
        Debug.Log("[UIAssetBundleBuilder] Starting UI AssetBundles build...");

        string projectRoot = Path.GetFullPath(Path.Combine(Application.dataPath, "../.."));
        string resUIDir = Path.Combine(projectRoot, "res/UI");

        if (!Directory.Exists(resUIDir))
        {
            Debug.LogError("[UIAssetBundleBuilder] Resource directory not found: " + resUIDir);
            return;
        }

        if (!Directory.Exists(TempAtlasDir))
        {
            Directory.CreateDirectory(TempAtlasDir);
        }

        try
        {
            // 1. Build Login.unity3d
            BuildAtlasBundle(
                atlasName: "Login",
                sourceDir: Path.Combine(resUIDir, "Pregame/Login"),
                destDir: Path.Combine(Application.streamingAssetsPath, "Data/Resources/UI/Pregame"),
                bundleFileName: "Login.unity3d",
                fileFilter: (fileName) => !fileName.EndsWith(".tps") && !fileName.Equals("Login.jpg", StringComparison.OrdinalIgnoreCase) && (fileName.EndsWith(".png", StringComparison.OrdinalIgnoreCase) || fileName.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase))
            );

            // 2. Build Components.unity3d
            BuildAtlasBundle(
                atlasName: "Components",
                sourceDir: Path.Combine(resUIDir, "Components"),
                destDir: Path.Combine(Application.streamingAssetsPath, "Data/Resources/UI/Common"),
                bundleFileName: "Components.unity3d",
                fileFilter: (fileName) => !fileName.EndsWith(".tps") && (fileName.EndsWith(".png", StringComparison.OrdinalIgnoreCase) || fileName.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase))
            );

            // 3. Build LoginScene.unity3d
            BuildAtlasBundle(
                atlasName: "LoginScene",
                sourceDir: Path.Combine(resUIDir, "Pregame/LoginScene"),
                destDir: Path.Combine(Application.streamingAssetsPath, "Data/Resources/UI/Pregame"),
                bundleFileName: "LoginScene.unity3d",
                fileFilter: (fileName) => !fileName.EndsWith(".tps") && !fileName.Equals("Login-new.jpg", StringComparison.OrdinalIgnoreCase) && (fileName.EndsWith(".png", StringComparison.OrdinalIgnoreCase) || fileName.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase))
            );

            // 4. Build SelectServer.unity3d
            BuildAtlasBundle(
                atlasName: "SelectServer",
                sourceDir: Path.Combine(resUIDir, "Pregame/SelectServer"),
                destDir: Path.Combine(Application.streamingAssetsPath, "Data/Resources/UI/Pregame"),
                bundleFileName: "SelectServer.unity3d",
                fileFilter: (fileName) => !fileName.EndsWith(".tps") && !fileName.EndsWith(".rar", StringComparison.OrdinalIgnoreCase) && !fileName.Contains("đổi-máy-chủ") && (fileName.EndsWith(".png", StringComparison.OrdinalIgnoreCase) || fileName.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase))
            );

            // 5. Build CreateRole.unity3d
            BuildAtlasBundle(
                atlasName: "CreateRole",
                sourceDir: Path.Combine(resUIDir, "Pregame/CreateRole"),
                destDir: Path.Combine(Application.streamingAssetsPath, "Data/Resources/UI/Pregame"),
                bundleFileName: "CreateRole.unity3d",
                fileFilter: (fileName) => !fileName.EndsWith(".tps") && !fileName.EndsWith(".rar", StringComparison.OrdinalIgnoreCase) && !fileName.Contains("tạo-nhân-vật") && (fileName.EndsWith(".png", StringComparison.OrdinalIgnoreCase) || fileName.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase))
            );

            // 6. Build SelectRole.unity3d
            BuildAtlasBundle(
                atlasName: "SelectRole",
                sourceDir: Path.Combine(resUIDir, "Pregame/SelectRole"),
                destDir: Path.Combine(Application.streamingAssetsPath, "Data/Resources/UI/Pregame"),
                bundleFileName: "SelectRole.unity3d",
                fileFilter: (fileName) => !fileName.EndsWith(".tps") && !fileName.Contains("chọn-nhân-vật") && (fileName.EndsWith(".png", StringComparison.OrdinalIgnoreCase) || fileName.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase))
            );

            // 7. Build LoadingResources.unity3d
            BuildAtlasBundle(
                atlasName: "LoadingResources",
                sourceDir: Path.Combine(resUIDir, "Pregame/LoadingResources"),
                destDir: Path.Combine(Application.streamingAssetsPath, "Data/Resources/UI/Pregame"),
                bundleFileName: "LoadingResources.unity3d",
                fileFilter: (fileName) => !fileName.EndsWith(".tps") && (fileName.EndsWith(".png", StringComparison.OrdinalIgnoreCase) || fileName.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase))
            );

            // 8. Build Main.unity3d (PlayZone main UI)
            string mainDest = Path.Combine(Application.streamingAssetsPath, "Data/Resources/UI/Game/MainUI");
            BuildAtlasBundle(
                atlasName: "Main",
                sourceDir: Path.Combine(resUIDir, "Main"),
                destDir: mainDest,
                bundleFileName: "Main.unity3d",
                fileFilter: (fileName) => !fileName.EndsWith(".tps") && !fileName.EndsWith(".psd", StringComparison.OrdinalIgnoreCase) && (fileName.EndsWith(".png", StringComparison.OrdinalIgnoreCase) || fileName.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase))
            );
            DeployCopy(Path.Combine(mainDest, "Main.unity3d"), Path.Combine(Application.streamingAssetsPath, "Data/Resources/UI/MainGame/MainUI.unity3d"));
            DeployCopy(Path.Combine(mainDest, "Main.unity3d"), Path.Combine(Application.streamingAssetsPath, "Data/Resources/UI/MainGame/Main.unity3d"));
            DeployCopy(Path.Combine(mainDest, "Main.unity3d"), Path.Combine(Application.streamingAssetsPath, "Data/Resources/UI/Game/MainUI.unity3d"));

            // 9. Build ActivityIcons.unity3d
            BuildAtlasBundle(
                atlasName: "ActivityIcons",
                sourceDir: Path.Combine(resUIDir, "ActivityIcons"),
                destDir: Path.Combine(Application.streamingAssetsPath, "Data/Resources/UI/Game/MainUI"),
                bundleFileName: "ActivityIcons.unity3d",
                fileFilter: (fileName) => !fileName.EndsWith(".tps") && (fileName.EndsWith(".png", StringComparison.OrdinalIgnoreCase) || fileName.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase))
            );

            // 10. Build RoleAvarta.unity3d
            var avartaAliases = new Dictionary<string, string[]>
            {
                { "1", new[] { "Male_1", "1" } },
                { "2", new[] { "Male_2", "2" } },
                { "3", new[] { "Male_3", "3" } },
                { "4", new[] { "Male_4", "4" } },
                { "5", new[] { "Male_5", "5" } },
                { "6", new[] { "Female_1", "Male_6", "6" } },
                { "7", new[] { "Female_2", "7" } },
                { "8", new[] { "Female_3", "8" } },
                { "9", new[] { "Female_4", "9" } },
                { "10", new[] { "Female_5", "10" } },
            };
            string avartaDest = Path.Combine(Application.streamingAssetsPath, "Data/Resources/UI");
            BuildAtlasBundle(
                atlasName: "1",
                sourceDir: Path.Combine(resUIDir, "RoleAvarta"),
                destDir: avartaDest,
                bundleFileName: "RoleAvarta.unity3d",
                fileFilter: (fileName) => !fileName.EndsWith(".tps") && (fileName.EndsWith(".png", StringComparison.OrdinalIgnoreCase) || fileName.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase)),
                customAliases: avartaAliases
            );
            string builtAvarta = Path.Combine(avartaDest, "RoleAvarta.unity3d");
            DeployCopy(builtAvarta, Path.Combine(Application.streamingAssetsPath, "Data/RoleAvarta.unity3d"));
            DeployCopy(builtAvarta, Path.Combine(Application.streamingAssetsPath, "Data/RoleAvarta.unity3"));
            DeployCopy(builtAvarta, Path.Combine(Application.streamingAssetsPath, "Data/Resources/RoleAvarta.unity3d"));
            DeployCopy(builtAvarta, Path.Combine(Application.streamingAssetsPath, "Data/Resources/RoleAvarta.unity3"));
            DeployCopy(builtAvarta, Path.Combine(Application.streamingAssetsPath, "Data/Resources/UI/RoleAvarta.unity3"));

            // 11. Build SpecialEvents.unity3d
            BuildAtlasBundle(
                atlasName: "SpecialEvents",
                sourceDir: Path.Combine(resUIDir, "SpecialEvents"),
                destDir: Path.Combine(Application.streamingAssetsPath, "Data/Resources/UI/Game"),
                bundleFileName: "SpecialEvents.unity3d",
                fileFilter: (fileName) => !fileName.EndsWith(".tps") && (fileName.EndsWith(".png", StringComparison.OrdinalIgnoreCase) || fileName.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase))
            );

            // 12. Build LocalMap.unity3d
            BuildAtlasBundle(
                atlasName: "LocalMap",
                sourceDir: Path.Combine(resUIDir, "LocalMap"),
                destDir: Path.Combine(Application.streamingAssetsPath, "Data/Resources/UI/Game"),
                bundleFileName: "LocalMap.unity3d",
                fileFilter: (fileName) => !fileName.EndsWith(".tps") && (fileName.EndsWith(".png", StringComparison.OrdinalIgnoreCase) || fileName.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase))
            );

            // 13. Build ChatBox.unity3d
            string chatDest = Path.Combine(Application.streamingAssetsPath, "Data/Resources/UI/Game");
            BuildAtlasBundle(
                atlasName: "ChatBox",
                sourceDir: Path.Combine(resUIDir, "ChatBox"),
                destDir: chatDest,
                bundleFileName: "ChatBox.unity3d",
                fileFilter: (fileName) => !fileName.EndsWith(".tps") && (fileName.EndsWith(".png", StringComparison.OrdinalIgnoreCase) || fileName.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase))
            );
            DeployCopy(Path.Combine(chatDest, "ChatBox.unity3d"), Path.Combine(Application.streamingAssetsPath, "Data/Resources/UI/MainGame/UIChat.unity3d"));

            // 14. Build NPCDialog.unity3d
            BuildAtlasBundle(
                atlasName: "NPCDialog",
                sourceDir: Path.Combine(resUIDir, "NPCDialog"),
                destDir: Path.Combine(Application.streamingAssetsPath, "Data/Resources/UI/Game"),
                bundleFileName: "NPCDialog.unity3d",
                fileFilter: (fileName) => !fileName.EndsWith(".tps") && (fileName.EndsWith(".png", StringComparison.OrdinalIgnoreCase) || fileName.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase))
            );

            // 15. Build SkillTree.unity3d
            BuildAtlasBundle(
                atlasName: "SkillTree",
                sourceDir: Path.Combine(resUIDir, "SkillTree"),
                destDir: Path.Combine(Application.streamingAssetsPath, "Data/Resources/UI/Game"),
                bundleFileName: "SkillTree.unity3d",
                fileFilter: (fileName) => !fileName.EndsWith(".tps") && (fileName.EndsWith(".png", StringComparison.OrdinalIgnoreCase) || fileName.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase))
            );

            // 16. Build Bag.unity3d
            BuildAtlasBundle(
                atlasName: "Bag",
                sourceDir: Path.Combine(resUIDir, "Bag"),
                destDir: Path.Combine(Application.streamingAssetsPath, "Data/Resources/UI/Game"),
                bundleFileName: "Bag.unity3d",
                fileFilter: (fileName) => !fileName.EndsWith(".tps") && (fileName.EndsWith(".png", StringComparison.OrdinalIgnoreCase) || fileName.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase))
            );

            // 17. Build SystemSetting.unity3d
            BuildAtlasBundle(
                atlasName: "SystemSetting",
                sourceDir: Path.Combine(resUIDir, "SystemSetting"),
                destDir: Path.Combine(Application.streamingAssetsPath, "Data/Resources/UI/Game"),
                bundleFileName: "SystemSetting.unity3d",
                fileFilter: (fileName) => !fileName.EndsWith(".tps") && (fileName.EndsWith(".png", StringComparison.OrdinalIgnoreCase) || fileName.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase))
            );

            // 18. Build Welfare.unity3d
            string welfareDest = Path.Combine(Application.streamingAssetsPath, "Data/Resources/UI/Game");
            BuildAtlasBundle(
                atlasName: "Welfare",
                sourceDir: Path.Combine(resUIDir, "Welfare"),
                destDir: welfareDest,
                bundleFileName: "Welfare.unity3d",
                fileFilter: (fileName) => !fileName.EndsWith(".tps") && (fileName.EndsWith(".png", StringComparison.OrdinalIgnoreCase) || fileName.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase))
            );
            DeployCopy(Path.Combine(welfareDest, "Welfare.unity3d"), Path.Combine(Application.streamingAssetsPath, "Data/Resources/UI/MainGame/Welfare.unity3d"));

            // 19. Build TeamBox.unity3d
            string teamDest = Path.Combine(Application.streamingAssetsPath, "Data/Resources/UI/Game");
            BuildAtlasBundle(
                atlasName: "TeamBox",
                sourceDir: Path.Combine(resUIDir, "TeamBox"),
                destDir: teamDest,
                bundleFileName: "TeamBox.unity3d",
                fileFilter: (fileName) => !fileName.EndsWith(".tps") && (fileName.EndsWith(".png", StringComparison.OrdinalIgnoreCase) || fileName.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase))
            );
            DeployCopy(Path.Combine(teamDest, "TeamBox.unity3d"), Path.Combine(Application.streamingAssetsPath, "Data/Resources/UI/MainGame/TeamFrame.unity3d"));

            // 20. Build Shop.unity3d
            string shopDest = Path.Combine(Application.streamingAssetsPath, "Data/Resources/UI/Game");
            BuildAtlasBundle(
                atlasName: "Shop",
                sourceDir: Path.Combine(resUIDir, "Shop"),
                destDir: shopDest,
                bundleFileName: "Shop.unity3d",
                fileFilter: (fileName) => !fileName.EndsWith(".tps") && (fileName.EndsWith(".png", StringComparison.OrdinalIgnoreCase) || fileName.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase))
            );
            DeployCopy(Path.Combine(shopDest, "Shop.unity3d"), Path.Combine(Application.streamingAssetsPath, "Data/Resources/UI/MainGame/Shop.unity3d"));

            // 21. Build TokenShop.unity3d
            string tokenShopDest = Path.Combine(Application.streamingAssetsPath, "Data/Resources/UI/Game");
            BuildAtlasBundle(
                atlasName: "TokenShop",
                sourceDir: Path.Combine(resUIDir, "TokenShop"),
                destDir: tokenShopDest,
                bundleFileName: "TokenShop.unity3d",
                fileFilter: (fileName) => !fileName.EndsWith(".tps") && (fileName.EndsWith(".png", StringComparison.OrdinalIgnoreCase) || fileName.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase))
            );
            DeployCopy(Path.Combine(tokenShopDest, "TokenShop.unity3d"), Path.Combine(Application.streamingAssetsPath, "Data/Resources/UI/MainGame/TokenShop.unity3d"));

            // 22. Build ItemBoxUseEffect.unity3d
            BuildAtlasBundle(
                atlasName: "ItemBoxUseEffect",
                sourceDir: Path.Combine(resUIDir, "ItemBoxUseEffect"),
                destDir: Path.Combine(Application.streamingAssetsPath, "Data/Resources/UI/Common"),
                bundleFileName: "ItemBoxUseEffect.unity3d",
                fileFilter: (fileName) => !fileName.EndsWith(".tps") && (fileName.EndsWith(".png", StringComparison.OrdinalIgnoreCase) || fileName.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase))
            );

            // 23. Build RoleEquip.unity3d
            string roleEquipDest = Path.Combine(Application.streamingAssetsPath, "Data/Resources/UI/Game");
            BuildAtlasBundle(
                atlasName: "RoleEquip",
                sourceDir: Path.Combine(resUIDir, "RoleEquip"),
                destDir: roleEquipDest,
                bundleFileName: "RoleEquip.unity3d",
                fileFilter: (fileName) => !fileName.EndsWith(".tps") && (fileName.EndsWith(".png", StringComparison.OrdinalIgnoreCase) || fileName.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase))
            );
            DeployCopy(Path.Combine(roleEquipDest, "RoleEquip.unity3d"), Path.Combine(Application.streamingAssetsPath, "Data/Resources/UI/MainGame/RoleInfo.unity3d"));

            AssetDatabase.Refresh();
            Debug.Log("[UIAssetBundleBuilder] All UI AssetBundles built successfully!");
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
    }

    private static void DeployCopy(string srcFile, string destFile)
    {
        if (!File.Exists(srcFile)) return;
        string dir = Path.GetDirectoryName(destFile);
        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
        File.Copy(srcFile, destFile, true);
    }

    private static void BuildAtlasBundle(string atlasName, string sourceDir, string destDir, string bundleFileName, Func<string, bool> fileFilter, Dictionary<string, string[]> customAliases = null)
    {
        Debug.Log("[UIAssetBundleBuilder] Building " + bundleFileName + " from " + sourceDir);

        if (!Directory.Exists(sourceDir))
        {
            Debug.LogWarning("[UIAssetBundleBuilder] Source dir does not exist: " + sourceDir);
            return;
        }

        string[] allFiles = Directory.GetFiles(sourceDir);
        List<string> selectedFiles = new List<string>();
        foreach (string file in allFiles)
        {
            string fileName = Path.GetFileName(file);
            if (fileFilter(fileName))
            {
                selectedFiles.Add(file);
            }
        }

        if (selectedFiles.Count == 0)
        {
            Debug.LogWarning("[UIAssetBundleBuilder] No valid files in " + sourceDir);
            return;
        }

        List<Texture2D> textures = new List<Texture2D>();
        List<string> spriteNames = new List<string>();

        foreach (string filePath in selectedFiles)
        {
            byte[] bytes = File.ReadAllBytes(filePath);
            Texture2D tex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            if (tex.LoadImage(bytes))
            {
                textures.Add(tex);
                spriteNames.Add(Path.GetFileNameWithoutExtension(filePath));
            }
        }

        // Pack into atlas texture
        Texture2D atlas = new Texture2D(2048, 2048, TextureFormat.RGBA32, false);
        Rect[] uvRects = atlas.PackTextures(textures.ToArray(), 2, 4096);

        // Save atlas PNG
        string atlasAssetPath = Path.Combine(TempAtlasDir, atlasName + ".png");
        File.WriteAllBytes(atlasAssetPath, atlas.EncodeToPNG());

        AssetDatabase.ImportAsset(atlasAssetPath, ImportAssetOptions.ForceUpdate);

        // Setup TextureImporter for multiple sprites
        TextureImporter importer = AssetImporter.GetAtPath(atlasAssetPath) as TextureImporter;
        if (importer != null)
        {
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Multiple;
            importer.maxTextureSize = 4096;
            importer.isReadable = true;

            var defaultAliases = new Dictionary<string, string[]>
            {
                { "Btn_Class_ThieuLam", new[] { "icon_tl" } },
                { "Btn_Class_ThieuLam_Active", new[] { "icon_tl_act" } },
                { "Title_ThieuLam", new[] { "logo_tl" } },
                { "Btn_Class_ThienVuong", new[] { "icon_tv" } },
                { "Btn_Class_ThienVuong_Active", new[] { "icon_tv_act" } },
                { "Title_ThienVuong", new[] { "logo_tv" } },
                { "Btn_Class_DuongMon", new[] { "icon_dm" } },
                { "Btn_Class_DuongMon_Active", new[] { "icon_dm_act" } },
                { "Title_DuongMon", new[] { "logo_dm" } },
                { "Btn_Class_NguDoc", new[] { "icon_nd" } },
                { "Btn_Class_NguDoc_Active", new[] { "icon_nd_act" } },
                { "Title_NguDoc", new[] { "logo_nd" } },
                { "Btn_Class_NgaMy", new[] { "icon_nm" } },
                { "Btn_Class_NgaMy_Active", new[] { "icon_nm_act" } },
                { "Title_NgaMy", new[] { "logo_nm" } },
                { "Btn_Class_ThuyYen", new[] { "icon_ty" } },
                { "Btn_Class_ThuyYen_Active", new[] { "icon_ty_act" } },
                { "Title_ThuyYen", new[] { "logo_ty" } },
                { "Btn_Class_CaiBang", new[] { "icon_cb" } },
                { "Btn_Class_CaiBang_Active", new[] { "icon_cb_act" } },
                { "Title_CaiBang", new[] { "logo_cb" } },
                { "Btn_Class_ThienNhan", new[] { "icon_tn" } },
                { "Btn_Class_ThienNhan_Active", new[] { "icon_tn_act" } },
                { "Title_ThienNhan", new[] { "logo_tn" } },
                { "Btn_Class_VoDang", new[] { "icon_vd" } },
                { "Btn_Class_VoDang_Active", new[] { "icon_vd_act" } },
                { "Title_VoDang", new[] { "logo_vd" } },
                { "Btn_Class_ConLon", new[] { "icon_cl" } },
                { "Btn_Class_ConLon_Active", new[] { "icon_cl_act" } },
                { "Title_ConLon", new[] { "logo_cl" } },
                { "Btn_Class_HoaSon", new[] { "icon_hs" } },
                { "Btn_Class_HoaSon_Active", new[] { "icon_hs_act" } },
                { "Title_HoaSon", new[] { "logo_hs" } },
            };

            var aliases = customAliases ?? defaultAliases;

            List<SpriteMetaData> metas = new List<SpriteMetaData>();
            for (int i = 0; i < textures.Count; i++)
            {
                Rect uv = uvRects[i];
                SpriteMetaData meta = new SpriteMetaData();
                meta.name = spriteNames[i];
                meta.rect = new Rect(uv.x * atlas.width, uv.y * atlas.height, uv.width * atlas.width, uv.height * atlas.height);
                meta.pivot = new Vector2(0.5f, 0.5f);
                meta.alignment = (int)SpriteAlignment.Center;
                metas.Add(meta);

                if (aliases.TryGetValue(spriteNames[i], out string[] extraNames))
                {
                    foreach (string alias in extraNames)
                    {
                        SpriteMetaData aliasMeta = new SpriteMetaData();
                        aliasMeta.name = alias;
                        aliasMeta.rect = meta.rect;
                        aliasMeta.pivot = meta.pivot;
                        aliasMeta.alignment = meta.alignment;
                        metas.Add(aliasMeta);
                    }
                }
            }

            importer.spritesheet = metas.ToArray();
            EditorUtility.SetDirty(importer);
            importer.SaveAndReimport();
        }

        // Clean up memory
        foreach (Texture2D t in textures)
        {
            UnityEngine.Object.DestroyImmediate(t);
        }
        UnityEngine.Object.DestroyImmediate(atlas);

        // Build AssetBundle
        if (!Directory.Exists(destDir))
        {
            Directory.CreateDirectory(destDir);
        }

        string tempBundleOutput = Path.Combine(Path.GetTempPath(), "JX1M_Bundles_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tempBundleOutput);

        AssetBundleBuild[] builds = new AssetBundleBuild[1];
        builds[0].assetBundleName = bundleFileName;
        builds[0].assetNames = new string[] { atlasAssetPath };

        BuildPipeline.BuildAssetBundles(tempBundleOutput, builds, BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);

        string srcBundlePath = Path.Combine(tempBundleOutput, bundleFileName);
        string finalBundlePath = Path.Combine(destDir, bundleFileName);

        if (File.Exists(srcBundlePath))
        {
            if (File.Exists(finalBundlePath)) File.Delete(finalBundlePath);
            File.Copy(srcBundlePath, finalBundlePath, true);
            Debug.Log("[UIAssetBundleBuilder] Successfully built and deployed: " + finalBundlePath);
        }
        else
        {
            Debug.LogError("[UIAssetBundleBuilder] Bundle output not found: " + srcBundlePath);
        }

        Directory.Delete(tempBundleOutput, true);
    }

    [MenuItem("JX1M/Build Starter Character Bundles")]
    public static void BuildStarterCharacterBundles()
    {
        Debug.Log("[UIAssetBundleBuilder] Starting Character Bundles build...");

        string projectRoot = Path.GetFullPath(Path.Combine(Application.dataPath, "../.."));
        string playerResDir = Path.Combine(projectRoot, "res/PlayerRes");
        string streamingData = Path.Combine(Application.streamingAssetsPath, "Data/Resources/Character");
        string tempCharDir = "Assets/Editor/TempCharacter";

        if (!Directory.Exists(tempCharDir)) Directory.CreateDirectory(tempCharDir);

        var targets = new (string sourceSubDir, string destSubDir)[]
        {
            ("Man/Body/body20", "Man/Body/body20"),
            ("Woman/Body/body20", "Woman/Body/body20"),
            ("Man/Helm/helm20", "Man/Head/helm20"),
            ("Woman/Helm/helm20", "Woman/Head/helm20"),
            ("Man/Weapon/weapon02", "Man/Weapon/weapon02"),
        };

        string[] actions = new[] { "FreeStand1", "FreeRun", "FreeWalk" };

        List<AssetBundleBuild> allBuilds = new List<AssetBundleBuild>();
        List<(string bundleName, string destDir)> copyMap = new List<(string, string)>();

        foreach (var (sourceSubDir, destSubDir) in targets)
        {
            string srcPath = Path.Combine(playerResDir, sourceSubDir);
            string dstPath = Path.Combine(streamingData, destSubDir);
            if (!Directory.Exists(srcPath)) continue;
            if (!Directory.Exists(dstPath)) Directory.CreateDirectory(dstPath);

            string prefix = destSubDir.Replace("/", "_");
            string charTargetDir = Path.Combine(tempCharDir, destSubDir);
            if (!Directory.Exists(charTargetDir)) Directory.CreateDirectory(charTargetDir);

            foreach (string action in actions)
            {
                for (int dir = 0; dir < 8; dir++)
                {
                    string actionName = string.Format("{0}_{1}", action, dir);
                    string pngFile = Path.Combine(srcPath, string.Format("{0}.png", actionName));
                    string tpsheetFile = Path.Combine(srcPath, string.Format("{0}.tpsheet", actionName));
                    string bundleName = string.Format("{0}_{1}.unity3d", prefix, actionName);
                    string finalBundleName = string.Format("{0}.unity3d", actionName);

                    if (!File.Exists(pngFile) || !File.Exists(tpsheetFile)) continue;

                    string tempPngAsset = Path.Combine(charTargetDir, string.Format("{0}.png", actionName));
                    File.Copy(pngFile, tempPngAsset, true);
                    AssetDatabase.ImportAsset(tempPngAsset, ImportAssetOptions.ForceUpdate);

                    TextureImporter importer = AssetImporter.GetAtPath(tempPngAsset) as TextureImporter;
                    if (importer != null)
                    {
                        importer.textureType = TextureImporterType.Sprite;
                        importer.spriteImportMode = SpriteImportMode.Multiple;
                        importer.maxTextureSize = 2048;
                        importer.isReadable = true;

                        List<SpriteMetaData> metas = new List<SpriteMetaData>();
                        foreach (string line in File.ReadAllLines(tpsheetFile))
                        {
                            if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#") || line.StartsWith(":")) continue;
                            string[] parts = line.Split(';');
                            if (parts.Length < 7) continue;

                            string spriteName = parts[0].Trim();
                            float x = float.Parse(parts[1], System.Globalization.CultureInfo.InvariantCulture);
                            float y = float.Parse(parts[2], System.Globalization.CultureInfo.InvariantCulture);
                            float w = float.Parse(parts[3], System.Globalization.CultureInfo.InvariantCulture);
                            float h = float.Parse(parts[4], System.Globalization.CultureInfo.InvariantCulture);
                            float px = float.Parse(parts[5], System.Globalization.CultureInfo.InvariantCulture);
                            float py = float.Parse(parts[6], System.Globalization.CultureInfo.InvariantCulture);

                            SpriteMetaData meta = new SpriteMetaData
                            {
                                name = spriteName,
                                rect = new Rect(x, y, w, h),
                                pivot = new Vector2(px, py),
                                alignment = (Mathf.Approximately(px, 0.5f) && Mathf.Approximately(py, 0.5f)) ? (int)SpriteAlignment.Center : (int)SpriteAlignment.Custom
                            };
                            metas.Add(meta);
                        }

                        importer.spritesheet = metas.ToArray();
                        EditorUtility.SetDirty(importer);
                        importer.SaveAndReimport();
                    }

                    allBuilds.Add(new AssetBundleBuild
                    {
                        assetBundleName = bundleName,
                        assetNames = new[] { tempPngAsset },
                        addressableNames = new[] { actionName }
                    });
                    copyMap.Add((bundleName, Path.Combine(dstPath, finalBundleName)));
                }
            }
        }

        if (allBuilds.Count > 0)
        {
            string tempOutput = Path.Combine(Path.GetTempPath(), "JX1M_CharBundles_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempOutput);

            BuildPipeline.BuildAssetBundles(tempOutput, allBuilds.ToArray(), BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);

            foreach (var (bundleName, destFile) in copyMap)
            {
                string src = Path.Combine(tempOutput, bundleName);
                if (File.Exists(src))
                {
                    if (File.Exists(destFile)) File.Delete(destFile);
                    File.Copy(src, destFile, true);
                }
            }

            Directory.Delete(tempOutput, true);
            AssetDatabase.Refresh();
            Debug.Log(string.Format("[UIAssetBundleBuilder] Built {0} Character Bundles successfully!", copyMap.Count));
        }
    }


    [MenuItem("JX1M/Build Map Config Bundles")]
    public static void BuildMapConfigBundles()
    {
        Debug.Log("[UIAssetBundleBuilder] Starting Map Config Bundles build...");
        string projectRoot = Path.GetFullPath(Path.Combine(Application.dataPath, "../../.."));
        string serverMapConfig = Path.Combine(projectRoot, "server/bin/GameServer/Config/MapConfig");
        string streamingMapConfig = Path.Combine(Application.streamingAssetsPath, "Data/MapConfig");
        string tempMapDir = "Assets/Editor/TempMapConfig";

        if (!Directory.Exists(serverMapConfig))
        {
            Debug.LogError("[UIAssetBundleBuilder] Server MapConfig directory not found: " + serverMapConfig);
            return;
        }

        if (!Directory.Exists(streamingMapConfig)) Directory.CreateDirectory(streamingMapConfig);
        if (!Directory.Exists(tempMapDir)) Directory.CreateDirectory(tempMapDir);

        string[] starterMaps = new[] { "BaLangHuyen", "GiangTanThon", "BienKinh", "TuongDuong", "PhuongTuong", "DaiLy", "ThanhDo", "LamAn" };
        List<AssetBundleBuild> builds = new List<AssetBundleBuild>();

        foreach (string mapName in starterMaps)
        {
            string srcMapDir = Path.Combine(serverMapConfig, mapName);
            if (!Directory.Exists(srcMapDir)) continue;

            string tempTargetDir = Path.Combine(tempMapDir, mapName);
            if (!Directory.Exists(tempTargetDir)) Directory.CreateDirectory(tempTargetDir);

            List<string> assetPaths = new List<string>();
            List<string> addressableNames = new List<string>();

            foreach (string file in Directory.GetFiles(srcMapDir))
            {
                if (file.EndsWith(".meta", StringComparison.OrdinalIgnoreCase)) continue;
                string fileName = Path.GetFileName(file);
                string tempFile = Path.Combine(tempTargetDir, fileName);
                File.Copy(file, tempFile, true);
                AssetDatabase.ImportAsset(tempFile, ImportAssetOptions.ForceUpdate);

                string normName = fileName;
                if (normName.Equals("npcs.xml", StringComparison.OrdinalIgnoreCase)) normName = "NPCs.xml";
                else if (normName.Equals("teleports.xml", StringComparison.OrdinalIgnoreCase)) normName = "Teleport.xml";
                else if (normName.Equals("zones.xml", StringComparison.OrdinalIgnoreCase)) normName = "Zones.xml";

                assetPaths.Add(tempFile);
                addressableNames.Add(normName);
            }

            if (assetPaths.Count > 0)
            {
                builds.Add(new AssetBundleBuild
                {
                    assetBundleName = mapName + ".unity3d",
                    assetNames = assetPaths.ToArray(),
                    addressableNames = addressableNames.ToArray()
                });
            }
        }

        if (builds.Count > 0)
        {
            string tempOutput = Path.Combine(Path.GetTempPath(), "JX1M_MapBundles_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempOutput);

            BuildPipeline.BuildAssetBundles(tempOutput, builds.ToArray(), BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);

            foreach (var b in builds)
            {
                string src = Path.Combine(tempOutput, b.assetBundleName);
                string dst = Path.Combine(streamingMapConfig, b.assetBundleName);
                if (File.Exists(src))
                {
                    if (File.Exists(dst)) File.Delete(dst);
                    File.Copy(src, dst, true);
                }
            }

            Directory.Delete(tempOutput, true);
            AssetDatabase.Refresh();
            Debug.Log(string.Format("[UIAssetBundleBuilder] Built {0} Map Config Bundles successfully!", builds.Count));
        }
    }

    [MenuItem("JX1M/Build Starter NPC Bundles")]
    public static void BuildStarterNPCBundles()
    {
        Debug.Log("[UIAssetBundleBuilder] Starting NPC Bundles build for starter map...");

        string projectRoot = Path.GetFullPath(Path.Combine(Application.dataPath, "../.."));
        string npcResDir = Path.Combine(projectRoot, "res/Npc");
        string streamingNpcDir = Path.Combine(Application.streamingAssetsPath, "Data/Resources/NPC");
        string tempNpcDir = "Assets/Editor/TempNPC";

        if (!Directory.Exists(tempNpcDir)) Directory.CreateDirectory(tempNpcDir);
        if (!Directory.Exists(streamingNpcDir)) Directory.CreateDirectory(streamingNpcDir);

        string[] starterNpcs = new string[]
        {
            "passerby049", "enemy057", "enemy158", "enemy162", "enemy104", "enemy189",
            "passerby041", "passerby007", "passerby024", "passerby177", "passerby181",
            "enemy081", "enemy165", "enemy167", "enemy169", "teleport"
        };

        List<AssetBundleBuild> allBuilds = new List<AssetBundleBuild>();
        List<(string bundleName, string destPath)> copyMap = new List<(string, string)>();

        foreach (string npcName in starterNpcs)
        {
            string srcFolder = Path.Combine(npcResDir, npcName);
            if (!Directory.Exists(srcFolder)) continue;

            string dstFolder = Path.Combine(streamingNpcDir, npcName);
            if (!Directory.Exists(dstFolder)) Directory.CreateDirectory(dstFolder);

            string targetDir = Path.Combine(tempNpcDir, npcName);
            if (!Directory.Exists(targetDir)) Directory.CreateDirectory(targetDir);

            string[] pngFiles = Directory.GetFiles(srcFolder, "*.png");
            foreach (string pngPath in pngFiles)
            {
                string actionName = Path.GetFileNameWithoutExtension(pngPath);
                if (!actionName.StartsWith("NormalStand") && !actionName.StartsWith("FightStand"))
                    continue;

                string tpsheetPath = Path.Combine(srcFolder, actionName + ".tpsheet");
                string bundleName = string.Format("npc_{0}_{1}.unity3d", npcName, actionName);
                string finalBundleName = actionName + ".unity3d";

                string tempPngAsset = Path.Combine(targetDir, actionName + ".png");
                File.Copy(pngPath, tempPngAsset, true);
                AssetDatabase.ImportAsset(tempPngAsset, ImportAssetOptions.ForceUpdate);

                TextureImporter importer = AssetImporter.GetAtPath(tempPngAsset) as TextureImporter;
                if (importer != null)
                {
                    importer.textureType = TextureImporterType.Sprite;
                    importer.spriteImportMode = SpriteImportMode.Multiple;
                    importer.maxTextureSize = 2048;
                    importer.isReadable = true;

                    List<SpriteMetaData> metas = new List<SpriteMetaData>();
                    if (File.Exists(tpsheetPath))
                    {
                        foreach (string line in File.ReadAllLines(tpsheetPath))
                        {
                            if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#") || line.StartsWith(":")) continue;
                            string[] parts = line.Split(';');
                            if (parts.Length < 7) continue;

                            string spriteName = parts[0].Trim();
                            float x = float.Parse(parts[1], System.Globalization.CultureInfo.InvariantCulture);
                            float y = float.Parse(parts[2], System.Globalization.CultureInfo.InvariantCulture);
                            float w = float.Parse(parts[3], System.Globalization.CultureInfo.InvariantCulture);
                            float h = float.Parse(parts[4], System.Globalization.CultureInfo.InvariantCulture);
                            float px = float.Parse(parts[5], System.Globalization.CultureInfo.InvariantCulture);
                            float py = float.Parse(parts[6], System.Globalization.CultureInfo.InvariantCulture);

                            metas.Add(new SpriteMetaData
                            {
                                name = spriteName,
                                rect = new Rect(x, y, w, h),
                                pivot = new Vector2(px, py),
                                alignment = (Mathf.Approximately(px, 0.5f) && Mathf.Approximately(py, 0.5f)) ? (int)SpriteAlignment.Center : (int)SpriteAlignment.Custom
                            });
                        }
                    }

                    if (metas.Count == 0)
                    {
                        metas.Add(new SpriteMetaData
                        {
                            name = actionName,
                            rect = new Rect(0, 0, 128, 128),
                            pivot = new Vector2(0.5f, 0.5f),
                            alignment = (int)SpriteAlignment.Center
                        });
                    }

                    importer.spritesheet = metas.ToArray();
                    EditorUtility.SetDirty(importer);
                    importer.SaveAndReimport();
                }

                allBuilds.Add(new AssetBundleBuild
                {
                    assetBundleName = bundleName,
                    assetNames = new[] { tempPngAsset },
                    addressableNames = new[] { actionName }
                });
                copyMap.Add((bundleName, Path.Combine(dstFolder, finalBundleName)));
            }
        }

        string effectResFile = Path.Combine(projectRoot, "res/UI/Icons/Item/Unknown/hoicham.png");
        string streamingEffectDir = Path.Combine(Application.streamingAssetsPath, "Data/Resources/Effect");
        if (!Directory.Exists(streamingEffectDir)) Directory.CreateDirectory(streamingEffectDir);

        if (File.Exists(effectResFile))
        {
            string effectTargetDir = Path.Combine(tempNpcDir, "Effect");
            if (!Directory.Exists(effectTargetDir)) Directory.CreateDirectory(effectTargetDir);
            string tempEffectAsset = Path.Combine(effectTargetDir, "1012.png");
            File.Copy(effectResFile, tempEffectAsset, true);
            AssetDatabase.ImportAsset(tempEffectAsset, ImportAssetOptions.ForceUpdate);

            TextureImporter effImp = AssetImporter.GetAtPath(tempEffectAsset) as TextureImporter;
            if (effImp != null)
            {
                effImp.textureType = TextureImporterType.Sprite;
                effImp.spriteImportMode = SpriteImportMode.Multiple;
                effImp.isReadable = true;
                effImp.spritesheet = new SpriteMetaData[]
                {
                    new SpriteMetaData
                    {
                        name = "1012",
                        rect = new Rect(0, 0, 48, 48),
                        pivot = new Vector2(0.5f, 0.5f),
                        alignment = (int)SpriteAlignment.Center
                    }
                };
                EditorUtility.SetDirty(effImp);
                effImp.SaveAndReimport();
            }

            allBuilds.Add(new AssetBundleBuild
            {
                assetBundleName = "effect_1012.unity3d",
                assetNames = new[] { tempEffectAsset },
                addressableNames = new[] { "1012" }
            });
            copyMap.Add(("effect_1012.unity3d", Path.Combine(streamingEffectDir, "1012.unity3d")));
        }

        if (allBuilds.Count > 0)
        {
            string tempBundleOutput = Path.Combine(Path.GetTempPath(), "JX1M_NPCBundles_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempBundleOutput);

            BuildPipeline.BuildAssetBundles(tempBundleOutput, allBuilds.ToArray(), BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);

            foreach (var (bundleName, destPath) in copyMap)
            {
                string srcFile = Path.Combine(tempBundleOutput, bundleName);
                if (File.Exists(srcFile))
                {
                    if (File.Exists(destPath)) File.Delete(destPath);
                    File.Copy(srcFile, destPath, true);
                }
            }

            Directory.Delete(tempBundleOutput, true);
            AssetDatabase.Refresh();
            Debug.Log(string.Format("[UIAssetBundleBuilder] Successfully built {0} NPC and Effect bundles!", allBuilds.Count));
        }
    }

    [MenuItem("JX1M/Build BaLangHuyen Map and Music")]
    public static void BuildBaLangHuyenMapAndMusic()
    {
        Debug.Log("[UIAssetBundleBuilder] Building BaLangHuyen Map Tiles and Music...");

        string tempMapDir = "Assets/Editor/TempMap";
        string streamingMapDir = Path.Combine(Application.streamingAssetsPath, "Data/Resources/Map/BaLangHuyen");
        string streamingImageDir = Path.Combine(streamingMapDir, "Image");

        if (!Directory.Exists(streamingImageDir)) Directory.CreateDirectory(streamingImageDir);

        List<AssetBundleBuild> allBuilds = new List<AssetBundleBuild>();
        List<(string bundleName, string destPath)> copyMap = new List<(string, string)>();

        for (int layerID = 1; layerID <= 15; layerID++)
        {
            string pngAsset = Path.Combine(tempMapDir, string.Format("Image/{0}.png", layerID));
            if (!File.Exists(pngAsset)) continue;

            AssetDatabase.ImportAsset(pngAsset, ImportAssetOptions.ForceUpdate);
            TextureImporter importer = AssetImporter.GetAtPath(pngAsset) as TextureImporter;
            if (importer != null)
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.spriteImportMode = SpriteImportMode.Multiple;
                importer.maxTextureSize = 2048;
                importer.isReadable = true;
                importer.spritesheet = new SpriteMetaData[]
                {
                    new SpriteMetaData
                    {
                        name = layerID.ToString(),
                        rect = new Rect(0, 0, 512, 512),
                        pivot = new Vector2(0.5f, 0.5f),
                        alignment = (int)SpriteAlignment.Center
                    }
                };
                EditorUtility.SetDirty(importer);
                importer.SaveAndReimport();
            }

            string bundleName = string.Format("map_blh_{0}.unity3d", layerID);
            allBuilds.Add(new AssetBundleBuild
            {
                assetBundleName = bundleName,
                assetNames = new[] { pngAsset },
                addressableNames = new[] { layerID.ToString() }
            });
            copyMap.Add((bundleName, Path.Combine(streamingImageDir, string.Format("{0}.unity3d", layerID))));
        }

        string wavAsset = Path.Combine(tempMapDir, "Music.wav");
        if (File.Exists(wavAsset))
        {
            AssetDatabase.ImportAsset(wavAsset, ImportAssetOptions.ForceUpdate);
            allBuilds.Add(new AssetBundleBuild
            {
                assetBundleName = "map_blh_music.unity3d",
                assetNames = new[] { wavAsset },
                addressableNames = new[] { "Music" }
            });
            copyMap.Add(("map_blh_music.unity3d", Path.Combine(streamingMapDir, "Music.unity3d")));
        }

        if (allBuilds.Count > 0)
        {
            string tempBundleOutput = Path.Combine(Path.GetTempPath(), "JX1M_MapTiles_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempBundleOutput);

            BuildPipeline.BuildAssetBundles(tempBundleOutput, allBuilds.ToArray(), BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);

            foreach (var (bundleName, destPath) in copyMap)
            {
                string srcFile = Path.Combine(tempBundleOutput, bundleName);
                if (File.Exists(srcFile))
                {
                    if (File.Exists(destPath)) File.Delete(destPath);
                    File.Copy(srcFile, destPath, true);
                }
            }

            Directory.Delete(tempBundleOutput, true);
            AssetDatabase.Refresh();
            Debug.Log(string.Format("[UIAssetBundleBuilder] Successfully built {0} BaLangHuyen map tiles and music bundles!", allBuilds.Count));
        }
    }

}
