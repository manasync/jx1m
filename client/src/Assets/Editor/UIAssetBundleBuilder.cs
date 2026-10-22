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
}
