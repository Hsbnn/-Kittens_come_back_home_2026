using System;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public static class RoofsAndBellsSceneBuilder
{
    private const string ScenePath = "Assets/Scenes/RoofsAndBells.unity";
    private const string DownloadedTexturePath = "Assets/Textures/Downloaded/";
    private const string GeneratedTexturePath = "Assets/Textures/Generated/";
    private const string DownloadedModelPath = "Assets/Models/Downloaded/";
    private const string RainThunderAudioPath = "Assets/Audio/rain_thunder_mixkit_2390.wav";
    private const string LanternModelPath = DownloadedModelPath + "Lantern_01/Lantern_01_1k.fbx";
    private const string LanternTexturePath = DownloadedModelPath + "Lantern_01/textures/Lantern_01_brass_diff_1k.png";

    [MenuItem("Tools/Roofs and Bells/Generate Main Scene")]
    public static void GenerateMainScene()
    {
        AssetDatabase.Refresh();
        EnsureGeneratedTextures();
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        Material roof = CreateTexturedMaterial("M_Roof_Visible_Tiles", GeneratedTexturePath + "roof_tiles_visible.png", new Color(0.55f, 0.16f, 0.1f), false, new Vector2(5f, 5f));
        Material facade = CreateTexturedMaterial("M_Brick_Visible_Facade", GeneratedTexturePath + "brick_visible.png", new Color(0.35f, 0.28f, 0.31f), false, new Vector2(4f, 4f));
        Material darkBrick = CreateTexturedMaterial("M_Dark_Brick_Visible", GeneratedTexturePath + "brick_visible.png", new Color(0.16f, 0.14f, 0.17f), false, new Vector2(3f, 3f));
        Material asphalt = CreateTexturedMaterial("M_Wet_Asphalt_Visible", GeneratedTexturePath + "wet_asphalt_visible.png", new Color(0.08f, 0.085f, 0.09f), false, new Vector2(8f, 8f));
        Material sky = CreateTexturedMaterial("M_Storm_Sky_Visible", GeneratedTexturePath + "storm_sky_visible.png", new Color(0.02f, 0.025f, 0.045f), true, Vector2.one);

        Material trim = CreateMaterial("M_Roof_Trim_Wet_Stone", new Color(0.13f, 0.13f, 0.16f));
        Material platform = CreateMaterial("M_Resonant_Platform_Glow", new Color(0.18f, 0.22f, 0.32f), true);
        Material cacheProp = CreateMaterial("M_Cacheable_Wood_Props", new Color(0.58f, 0.36f, 0.19f));
        Material metal = CreateMaterial("M_Bell_Warm_Gold", new Color(0.95f, 0.76f, 0.25f), true);
        Material gate = CreateMaterial("M_Blue_Gate_Glow", new Color(0.12f, 0.32f, 0.62f), true);
        Material window = CreateMaterial("M_Warm_Lit_Windows", new Color(1f, 0.68f, 0.28f), true);
        Material puddle = CreateMaterial("M_Rain_Puddles", new Color(0.09f, 0.13f, 0.18f));
        Material neonPink = CreateMaterial("M_Rainy_Neon_Pink", new Color(1f, 0.18f, 0.72f), true);
        Material neonCyan = CreateMaterial("M_Rainy_Neon_Cyan", new Color(0.2f, 0.82f, 1f), true);
        Material lantern = CreateTexturedMaterial("M_CC0_Lantern_Brass", LanternTexturePath, new Color(0.72f, 0.48f, 0.22f), false, Vector2.one);
        Material cat = CreateMaterial("M_Cat_Fur_Black", new Color(0.025f, 0.025f, 0.03f));
        Material kitten = CreateMaterial("M_Kitten_Fur_Cream", new Color(1f, 0.78f, 0.5f));
        Material whiteFur = CreateMaterial("M_Cat_White_Muzzle", new Color(0.92f, 0.86f, 0.76f));
        Material tailTip = CreateMaterial("M_Cat_Tail_Tip_White", new Color(0.96f, 0.9f, 0.78f));
        Material rescueGlow = CreateMaterial("M_Kitten_Rescue_Glow", new Color(1f, 0.58f, 0.22f), true);
        Material eye = CreateMaterial("M_Cat_Green_Eyes", new Color(0.35f, 0.95f, 0.55f), true);
        Material nose = CreateMaterial("M_Cat_Pink_Nose", new Color(0.95f, 0.38f, 0.48f));
        Material collar = CreateMaterial("M_Red_Collar", new Color(0.74f, 0.06f, 0.08f));
        Material goal = CreateMaterial("M_Window_Goal_Glow", new Color(1f, 0.76f, 0.38f), true);

        ConfigureLighting();
        CreateSkyBackdrop(sky);
        CreateStreetLevel(asphalt, puddle, facade, darkBrick, trim, window);

        CreateRooftop("Roof_Start", new Vector3(0f, 0f, 0f), new Vector3(8f, 0.5f, 8f), roof, facade, trim, window);
        CreateRooftop("Roof_Melody", new Vector3(9f, 0.3f, 2f), new Vector3(7f, 0.5f, 6f), roof, facade, trim, window, eastPassageCenterZ: 0.5f, eastPassageWidth: 2.8f);
        CreateRooftop("Roof_Garden", new Vector3(17f, 0.7f, -1f), new Vector3(7f, 0.5f, 8f), roof, facade, trim, window, westPassageCenterZ: 0.5f, westPassageWidth: 2.8f);
        CreateRooftop("Roof_Final", new Vector3(26f, 1.2f, 2f), new Vector3(8f, 0.5f, 7f), roof, facade, trim, window, westPassageCenterZ: 0.6f, westPassageWidth: 3f);

        CreateRooftopBridge("Bridge_First", new Vector3(4.6f, 0.15f, 1f), new Vector3(2.6f, 0.22f, 2.4f), trim, roof, window);
        CreateRooftopBridge("Bridge_Melody", new Vector3(13f, 0.45f, 0.5f), new Vector3(1.2f, 0.22f, 2.6f), trim, roof, window);
        CreateRooftopBridge("Bridge_Final", new Vector3(21.7f, 0.95f, 0.6f), new Vector3(4.2f, 0.22f, 2.4f), trim, roof, window);
        CreateAcousticMaze(darkBrick, trim, window);

        CreateCacheableProp("Cacheable_Milk_Crate_A", PrimitiveType.Cube, new Vector3(3.8f, 0.65f, 1.05f), new Vector3(1.1f, 0.8f, 1.15f), "молочный ящик", 1.2f, cacheProp);
        CreateCacheableProp("Cacheable_Flowerpot_Blocker", PrimitiveType.Cylinder, new Vector3(7.6f, 0.95f, 2.8f), new Vector3(0.85f, 0.65f, 0.85f), "цветочный горшок", 0.85f, cacheProp);
        CreateCacheableProp("Cacheable_Rain_Barrel_Acoustic_Blocker", PrimitiveType.Cylinder, new Vector3(8.75f, 1.05f, 3.28f), new Vector3(1.05f, 1.15f, 1.05f), "дождевую бочку", 0.75f, cacheProp);
        CreateCacheableProp("Cacheable_Rooftop_Box", PrimitiveType.Cube, new Vector3(15.8f, 1.35f, -0.7f), new Vector3(0.9f, 0.65f, 0.9f), "крышная коробка", 1.5f, cacheProp);
        CreateCacheableProp("Cacheable_Fallen_Sign_Blocker", PrimitiveType.Cube, new Vector3(14.2f, 1.25f, -2.2f), new Vector3(1.65f, 0.55f, 1.15f), "упавшую вывеску", 1.1f, cacheProp);
        CreateCacheableProp("Cacheable_Tin_Can", PrimitiveType.Capsule, new Vector3(19.2f, 1.55f, 1.4f), new Vector3(0.35f, 0.45f, 0.35f), "жестяная банка", 1.8f, metal);

        GameObject player = CreateCatPlayer(cat, whiteFur, tailTip, eye, nose, collar, metal);
        CreateCameraRig(player.transform);
        CreateRain();
        AmbienceAudio ambience = CreateGameObjectWithComponent<AmbienceAudio>("Rain_And_Thunder_Ambience");
        ambience.Configure(AssetDatabase.LoadAssetAtPath<AudioClip>(RainThunderAudioPath));

        CreateBell("Bell_C", new Vector3(1.7f, 0.9f, 2.4f), 0, metal, trim);
        CreateBell("Bell_E", new Vector3(8.4f, 1.2f, 4.2f), 1, metal, trim);
        CreateBell("Bell_G", new Vector3(10.8f, 1.2f, -0.8f), 2, metal, trim);
        CreateResonantPlatform("Resonant_Bridge_E", new Vector3(13.1f, 0.55f, -2.25f), new Vector3(3.1f, 0.25f, 1.35f), 1, new Vector3(0f, 0f, 2.75f), Vector3.zero, platform, metal);
        CreateResonantPlatform("Rotating_Chimney_Plank_G", new Vector3(18.2f, 1.35f, 4.65f), new Vector3(4f, 0.22f, 1.1f), 2, new Vector3(0f, 0f, -2.3f), new Vector3(0f, 30f, 0f), platform, metal);

        GameObject melodyGate = CreateGate("Melody_Gate", new Vector3(13.2f, 1.25f, 0.5f), new Vector3(0.42f, 2.45f, 5.4f), gate, metal);
        MelodyGate melody = melodyGate.AddComponent<MelodyGate>();
        melody.Melody = new[] { 0, 1, 2 };

        CreateKitten("Kitten_RainPipe", new Vector3(16.2f, 1.18f, -3.4f), kitten, whiteFur, tailTip, eye, nose, collar, metal, rescueGlow);
        CreateKitten("Kitten_Chimney", new Vector3(18.8f, 1.18f, 2.2f), kitten, whiteFur, tailTip, eye, nose, collar, metal, rescueGlow);

        GameObject harmonyGate = CreateGate("Harmony_Gate", new Vector3(21.8f, 1.9f, 0.6f), new Vector3(0.5f, 2.85f, 5.2f), gate, metal);
        HarmonyGate harmony = harmonyGate.AddComponent<HarmonyGate>();
        harmony.RequiredNote = 2;
        harmony.RequiredKittens = 2;

        CreateBell("Bell_Final_G", new Vector3(24.3f, 2.1f, 4.2f), 2, metal, trim);
        CreateGoal(new Vector3(28.3f, 2.1f, 2f), goal, trim, window);
        CreateDecorativeCity(goal, facade, darkBrick, trim, window, roof, neonPink, neonCyan);
        CreateDownloadedLanternDecor(lantern, window);
        CreateStoryTriggers();
        CreateGameObjectWithComponent<StoryDirector>("Story_Director");
        CreateGameObjectWithComponent<GameUI>("Game_UI");

        EditorSceneManager.SaveScene(scene, ScenePath);
        EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(ScenePath, true) };
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("Generated scene: " + ScenePath);
    }

    private static Material CreateMaterial(string name, Color color, bool emission = false)
    {
        string path = "Assets/Materials/" + name + ".mat";
        Material material = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (material == null)
        {
            material = new Material(GetReliableSceneShader()) { name = name };
            AssetDatabase.CreateAsset(material, path);
        }

        ConfigureMaterial(material, color, emission);
        return material;
    }

    private static Material CreateTexturedMaterial(string name, string texturePath, Color fallbackColor, bool emission = false, Vector2? tiling = null)
    {
        Material material = CreateMaterial(name, fallbackColor, emission);
        Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(texturePath);
        if (texture == null)
        {
            texture = AssetDatabase.LoadAssetAtPath<Texture2D>(texturePath.Replace(GeneratedTexturePath, DownloadedTexturePath));
        }

        if (texture == null)
        {
            return material;
        }

        Color textureTint = emission ? Color.white * 0.92f : Color.white;
        material.color = textureTint;
        if (material.HasProperty("_Color"))
        {
            material.SetColor("_Color", textureTint);
        }

        material.mainTexture = texture;
        material.mainTextureScale = tiling ?? Vector2.one;
        if (material.HasProperty("_MainTex"))
        {
            material.SetTexture("_MainTex", texture);
            material.SetTextureScale("_MainTex", tiling ?? Vector2.one);
        }

        EditorUtility.SetDirty(material);
        return material;
    }

    private static Material CreateParticleMaterial(string name, Color color)
    {
        string path = "Assets/Materials/" + name + ".mat";
        Shader shader = Shader.Find("Particles/Standard Unlit")
            ?? Shader.Find("Legacy Shaders/Particles/Alpha Blended")
            ?? Shader.Find("Sprites/Default")
            ?? GetReliableSceneShader();
        Material material = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (material == null)
        {
            material = new Material(shader) { name = name };
            AssetDatabase.CreateAsset(material, path);
        }
        else if (shader != null && material.shader != shader)
        {
            material.shader = shader;
        }

        if (material.HasProperty("_Mode"))
        {
            material.SetFloat("_Mode", 2f);
        }

        material.color = color;
        if (material.HasProperty("_Color"))
        {
            material.SetColor("_Color", color);
        }

        if (material.HasProperty("_TintColor"))
        {
            material.SetColor("_TintColor", color);
        }

        if (material.HasProperty("_EmissionColor"))
        {
            material.SetColor("_EmissionColor", Color.black);
        }

        material.renderQueue = (int)RenderQueue.Transparent;
        EditorUtility.SetDirty(material);
        return material;
    }

    private static Shader GetReliableSceneShader()
    {
        Shader shader = Shader.Find("Standard");
        if (shader != null)
        {
            return shader;
        }

        return Shader.Find("Unlit/Color");
    }

    private static void EnsureGeneratedTextures()
    {
        Directory.CreateDirectory(GeneratedTexturePath);
        WriteTextureIfMissing(GeneratedTexturePath + "roof_tiles_visible.png", GenerateRoofTexture(256));
        WriteTextureIfMissing(GeneratedTexturePath + "brick_visible.png", GenerateBrickTexture(256));
        WriteTextureIfMissing(GeneratedTexturePath + "wet_asphalt_visible.png", GenerateAsphaltTexture(256));
        WriteTextureIfMissing(GeneratedTexturePath + "storm_sky_visible.png", GenerateStormSkyTexture(512, 256));
        AssetDatabase.Refresh();
        ConfigureTextureImporter(GeneratedTexturePath + "roof_tiles_visible.png");
        ConfigureTextureImporter(GeneratedTexturePath + "brick_visible.png");
        ConfigureTextureImporter(GeneratedTexturePath + "wet_asphalt_visible.png");
        ConfigureTextureImporter(GeneratedTexturePath + "storm_sky_visible.png");
    }

    private static void WriteTextureIfMissing(string assetPath, Texture2D texture)
    {
        string fullPath = Path.Combine(Directory.GetCurrentDirectory(), assetPath);
        if (!File.Exists(fullPath))
        {
            File.WriteAllBytes(fullPath, texture.EncodeToPNG());
        }

        UnityEngine.Object.DestroyImmediate(texture);
    }

    private static void ConfigureTextureImporter(string assetPath)
    {
        TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
        if (importer == null)
        {
            return;
        }

        importer.wrapMode = TextureWrapMode.Repeat;
        importer.filterMode = FilterMode.Point;
        importer.textureCompression = TextureImporterCompression.Uncompressed;
        importer.SaveAndReimport();
    }

    private static Texture2D GenerateRoofTexture(int size)
    {
        Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                int tileX = x / 32;
                int tileY = y / 18;
                bool mortar = x % 32 < 3 || y % 18 < 2;
                float wave = Mathf.Sin((x + tileY * 13) * 0.22f) * 0.04f;
                float shade = ((tileX + tileY) % 2 == 0 ? 0.08f : -0.03f) + wave;
                Color color = mortar
                    ? new Color(0.12f, 0.06f, 0.045f)
                    : new Color(0.62f + shade, 0.16f + shade * 0.45f, 0.08f);
                texture.SetPixel(x, y, color);
            }
        }

        texture.Apply();
        return texture;
    }

    private static Texture2D GenerateBrickTexture(int size)
    {
        Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                int row = y / 20;
                int offset = row % 2 == 0 ? 0 : 28;
                int localX = (x + offset) % 56;
                bool mortar = localX < 3 || y % 20 < 3;
                float grime = Hash01(x / 4, y / 4) * 0.08f;
                Color color = mortar
                    ? new Color(0.07f, 0.07f, 0.08f)
                    : new Color(0.28f + grime, 0.18f + grime * 0.6f, 0.16f + grime * 0.45f);
                texture.SetPixel(x, y, color);
            }
        }

        texture.Apply();
        return texture;
    }

    private static Texture2D GenerateAsphaltTexture(int size)
    {
        Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float noise = Hash01(x, y);
                bool wetStreak = (x + y * 2) % 47 < 4;
                float v = 0.055f + noise * 0.08f + (wetStreak ? 0.08f : 0f);
                texture.SetPixel(x, y, new Color(v, v + 0.01f, v + 0.018f));
            }
        }

        texture.Apply();
        return texture;
    }

    private static Texture2D GenerateStormSkyTexture(int width, int height)
    {
        Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        for (int y = 0; y < height; y++)
        {
            float t = y / (float)(height - 1);
            for (int x = 0; x < width; x++)
            {
                float cloud = Mathf.Sin(x * 0.035f + y * 0.08f) * 0.5f + Mathf.Sin(x * 0.017f - y * 0.04f) * 0.5f;
                cloud = Mathf.Clamp01(cloud * 0.5f + 0.5f);
                Color bottom = new Color(0.018f, 0.021f, 0.032f);
                Color top = new Color(0.035f, 0.042f, 0.075f);
                Color color = Color.Lerp(bottom, top, t);
                color += new Color(0.035f, 0.038f, 0.045f) * Mathf.SmoothStep(0.45f, 0.95f, cloud) * (1f - t * 0.35f);
                if (Hash01(x / 3, y / 3) > 0.997f && t > 0.45f)
                {
                    color += new Color(0.65f, 0.68f, 0.78f) * 0.75f;
                }

                texture.SetPixel(x, y, color);
            }
        }

        texture.Apply();
        return texture;
    }

    private static float Hash01(int x, int y)
    {
        unchecked
        {
            int n = x * 73856093 ^ y * 19349663;
            n = (n << 13) ^ n;
            return ((n * (n * n * 15731 + 789221) + 1376312589) & 0x7fffffff) / 2147483647f;
        }
    }

    private static void ConfigureMaterial(Material material, Color color, bool emission)
    {
        Shader shader = GetReliableSceneShader();
        if (shader != null && material.shader != shader)
        {
            material.shader = shader;
        }

        material.color = color;
        if (material.HasProperty("_Color"))
        {
            material.SetColor("_Color", color);
        }

        if (material.HasProperty("_Glossiness"))
        {
            material.SetFloat("_Glossiness", emission ? 0.65f : 0.28f);
        }

        if (material.HasProperty("_Metallic"))
        {
            material.SetFloat("_Metallic", emission ? 0.08f : 0f);
        }

        if (emission && material.HasProperty("_EmissionColor"))
        {
            material.EnableKeyword("_EMISSION");
            material.SetColor("_EmissionColor", color * 1.45f);
        }
        else if (material.HasProperty("_EmissionColor"))
        {
            material.DisableKeyword("_EMISSION");
            material.SetColor("_EmissionColor", Color.black);
        }

        EditorUtility.SetDirty(material);
    }

    private static void ConfigureLighting()
    {
        RenderSettings.ambientMode = AmbientMode.Trilight;
        RenderSettings.ambientSkyColor = new Color(0.025f, 0.035f, 0.07f);
        RenderSettings.ambientEquatorColor = new Color(0.055f, 0.06f, 0.085f);
        RenderSettings.ambientGroundColor = new Color(0.018f, 0.016f, 0.022f);
        RenderSettings.fog = true;
        RenderSettings.fogMode = FogMode.ExponentialSquared;
        RenderSettings.fogColor = new Color(0.025f, 0.03f, 0.055f);
        RenderSettings.fogDensity = 0.018f;
        RenderSettings.skybox = CreateStormSkyboxMaterial();

        GameObject moon = new GameObject("Moon_Key_Light");
        Light moonLight = moon.AddComponent<Light>();
        moonLight.type = LightType.Directional;
        moonLight.intensity = 0.72f;
        moonLight.color = new Color(0.64f, 0.72f, 1f);
        moon.transform.rotation = Quaternion.Euler(48f, -35f, 0f);

        GameObject window = new GameObject("Warm_Home_Window_Light");
        Light point = window.AddComponent<Light>();
        point.type = LightType.Point;
        point.intensity = 7.5f;
        point.range = 14f;
        point.color = new Color(1f, 0.64f, 0.32f);
        window.transform.position = new Vector3(28f, 4f, 2f);
    }

    private static Material CreateStormSkyboxMaterial()
    {
        string path = "Assets/Materials/M_Storm_Procedural_Skybox.mat";
        Material material = AssetDatabase.LoadAssetAtPath<Material>(path);
        Shader shader = Shader.Find("Skybox/Procedural");
        if (shader == null)
        {
            shader = Shader.Find("Skybox/6 Sided");
        }

        if (material == null && shader != null)
        {
            material = new Material(shader) { name = "M_Storm_Procedural_Skybox" };
            AssetDatabase.CreateAsset(material, path);
        }

        if (material == null)
        {
            return null;
        }

        if (shader != null && material.shader != shader)
        {
            material.shader = shader;
        }

        if (material.HasProperty("_SkyTint"))
        {
            material.SetColor("_SkyTint", new Color(0.025f, 0.03f, 0.06f));
        }

        if (material.HasProperty("_GroundColor"))
        {
            material.SetColor("_GroundColor", new Color(0.012f, 0.014f, 0.02f));
        }

        if (material.HasProperty("_Exposure"))
        {
            material.SetFloat("_Exposure", 0.18f);
        }

        if (material.HasProperty("_AtmosphereThickness"))
        {
            material.SetFloat("_AtmosphereThickness", 0.18f);
        }

        EditorUtility.SetDirty(material);
        return material;
    }

    private static void CreateStreetLevel(Material asphalt, Material puddle, Material facade, Material darkBrick, Material trim, Material window)
    {
        CreateCube("Wet_Asphalt_Street", new Vector3(13f, -4.35f, 0f), new Vector3(58f, 0.16f, 24f), asphalt, true);
        CreateCube("Left_Sidewalk", new Vector3(13f, -4.22f, -10.2f), new Vector3(58f, 0.18f, 2f), trim, true);
        CreateCube("Right_Sidewalk", new Vector3(13f, -4.22f, 10.2f), new Vector3(58f, 0.18f, 2f), trim, true);

        for (int i = 0; i < 9; i++)
        {
            float x = -8f + i * 5.2f;
            CreateCube("Crosswalk_Stripe_" + i, new Vector3(x, -4.13f, -0.2f), new Vector3(2.4f, 0.04f, 0.28f), window, false);
        }

        for (int i = 0; i < 11; i++)
        {
            float x = -12f + i * 5.2f;
            float z = i % 2 == 0 ? -7.1f : 6.6f;
            CreatePuddle("Street_Puddle_" + i, new Vector3(x, -4.1f, z), new Vector3(1.3f + (i % 3) * 0.4f, 0.035f, 0.6f + (i % 2) * 0.28f), puddle);
        }

        CreateStreetLamp("Street_Lamp_A", new Vector3(-4f, -2.4f, -8.6f), trim, window);
        CreateStreetLamp("Street_Lamp_B", new Vector3(12.5f, -2.4f, 8.4f), trim, window);
        CreateStreetLamp("Street_Lamp_C", new Vector3(29f, -2.4f, -8.7f), trim, window);

        for (int i = 0; i < 7; i++)
        {
            float x = -10f + i * 7f;
            float height = 4.8f + (i % 4) * 1.25f;
            CreateFacadeBuilding("Street_Facade_" + i, new Vector3(x, -4.28f + height * 0.5f, -13.2f), new Vector3(4.7f, height, 2.4f), i % 2 == 0 ? facade : darkBrick, trim, window, true);
        }
    }

    private static void CreateSkyBackdrop(Material sky)
    {
        CreateBackdropQuad("Night_Sky_North_CC0", new Vector3(13f, 8f, 19f), new Vector3(46f, 18f, 1f), Quaternion.Euler(0f, 180f, 0f), sky);
        CreateBackdropQuad("Night_Sky_South_CC0", new Vector3(13f, 8f, -18f), new Vector3(46f, 18f, 1f), Quaternion.identity, sky);
        CreateBackdropQuad("Night_Sky_East_CC0", new Vector3(36f, 8f, 1f), new Vector3(38f, 18f, 1f), Quaternion.Euler(0f, -90f, 0f), sky);
        CreateMoonDisc(new Vector3(27f, 11.5f, 17.8f));
    }

    private static void CreateBackdropQuad(string name, Vector3 position, Vector3 scale, Quaternion rotation, Material material)
    {
        GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
        quad.name = name;
        quad.transform.position = position;
        quad.transform.rotation = rotation;
        quad.transform.localScale = scale;
        ApplyMaterial(quad, material);
        DestroyCollider(quad);
        GameObjectUtility.SetStaticEditorFlags(quad, StaticEditorFlags.ContributeGI | StaticEditorFlags.BatchingStatic);
    }

    private static void CreateMoonDisc(Vector3 position)
    {
        Material moon = CreateMaterial("M_Moon_Disc", new Color(0.78f, 0.82f, 0.9f), true);
        GameObject disc = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        disc.name = "Cloudy_Moon_Disc";
        disc.transform.position = position;
        disc.transform.localScale = new Vector3(1.8f, 1.8f, 0.12f);
        ApplyMaterial(disc, moon);
        DestroyCollider(disc);
    }

    private static void CreateRooftop(string name, Vector3 position, Vector3 scale, Material roof, Material facade, Material trim, Material window, float? westPassageCenterZ = null, float westPassageWidth = 0f, float? eastPassageCenterZ = null, float eastPassageWidth = 0f)
    {
        GameObject roofObject = CreateCube(name, position, scale, roof, true);
        Vector3 houseBlockPosition = position + Vector3.down * 2.25f;
        Vector3 houseBlockScale = new Vector3(scale.x, 4.1f, scale.z);
        float? housePassageCenterZ = westPassageCenterZ ?? eastPassageCenterZ;
        float housePassageWidth = westPassageWidth > 0f ? westPassageWidth : eastPassageWidth;
        CreateSplitBlockAlongZ(name + "_House_Block", houseBlockPosition, houseBlockScale, facade, housePassageCenterZ, housePassageWidth, true);
        CreateParapets(name, position, scale, trim, westPassageCenterZ, westPassageWidth, eastPassageCenterZ, eastPassageWidth);
        CreateRoofDetails(name, position, scale, trim, window);
        CreateFacadeWindows(name, houseBlockPosition, houseBlockScale, window);
        GameObjectUtility.SetStaticEditorFlags(roofObject, StaticEditorFlags.ContributeGI | StaticEditorFlags.BatchingStatic | StaticEditorFlags.OccluderStatic);
    }

    private static void CreateRooftopBridge(string name, Vector3 position, Vector3 scale, Material trim, Material roof, Material glow)
    {
        CreateCube(name, position, scale, roof, true);
        CreateCube(name + "_Wet_Rail_L", position + new Vector3(0f, 0.28f, scale.z * 0.52f), new Vector3(scale.x, 0.18f, 0.12f), trim, true);
        CreateCube(name + "_Wet_Rail_R", position + new Vector3(0f, 0.28f, -scale.z * 0.52f), new Vector3(scale.x, 0.18f, 0.12f), trim, true);
        for (int i = 0; i < 5; i++)
        {
            float t = i / 4f - 0.5f;
            Vector3 left = position + new Vector3(scale.x * t, 0.52f + Mathf.Sin(i * 0.9f) * 0.05f, scale.z * 0.6f);
            Vector3 right = position + new Vector3(scale.x * t, 0.52f + Mathf.Cos(i * 0.7f) * 0.05f, -scale.z * 0.6f);
            CreateVisualWorldPrimitive(name + "_Warm_Raindrop_Light_L_" + i, PrimitiveType.Sphere, left, Vector3.one * 0.12f, glow, false);
            CreateVisualWorldPrimitive(name + "_Warm_Raindrop_Light_R_" + i, PrimitiveType.Sphere, right, Vector3.one * 0.12f, glow, false);
        }
    }

    private static void CreateParapets(string name, Vector3 position, Vector3 scale, Material trim, float? westPassageCenterZ = null, float westPassageWidth = 0f, float? eastPassageCenterZ = null, float eastPassageWidth = 0f)
    {
        float topY = position.y + scale.y * 0.5f + 0.22f;
        CreateCube(name + "_Parapet_N", new Vector3(position.x, topY, position.z + scale.z * 0.5f), new Vector3(scale.x + 0.35f, 0.42f, 0.22f), trim, true);
        CreateCube(name + "_Parapet_S", new Vector3(position.x, topY, position.z - scale.z * 0.5f), new Vector3(scale.x + 0.35f, 0.42f, 0.22f), trim, true);

        float eastX = position.x + scale.x * 0.5f;
        Vector3 sideParapetScale = new Vector3(0.22f, 0.42f, scale.z + 0.35f);
        CreateSplitBlockAlongZ(name + "_Parapet_E", new Vector3(eastX, topY, position.z), sideParapetScale, trim, eastPassageCenterZ, eastPassageWidth, true);

        float westX = position.x - scale.x * 0.5f;
        CreateSplitBlockAlongZ(name + "_Parapet_W", new Vector3(westX, topY, position.z), sideParapetScale, trim, westPassageCenterZ, westPassageWidth, true);
    }

    private static void CreateSplitBlockAlongZ(string name, Vector3 position, Vector3 scale, Material material, float? passageCenterZ, float passageWidth, bool keepCollider)
    {
        if (!passageCenterZ.HasValue || passageWidth <= 0f)
        {
            CreateCube(name, position, scale, material, keepCollider);
            return;
        }

        float minZ = position.z - scale.z * 0.5f;
        float maxZ = position.z + scale.z * 0.5f;
        float passageMin = passageCenterZ.Value - passageWidth * 0.5f;
        float passageMax = passageCenterZ.Value + passageWidth * 0.5f;

        if (passageMin > minZ + 0.05f)
        {
            float southDepth = passageMin - minZ;
            CreateCube(name + "_South", new Vector3(position.x, position.y, minZ + southDepth * 0.5f), new Vector3(scale.x, scale.y, southDepth), material, keepCollider);
        }

        if (passageMax < maxZ - 0.05f)
        {
            float northDepth = maxZ - passageMax;
            CreateCube(name + "_North", new Vector3(position.x, position.y, maxZ - northDepth * 0.5f), new Vector3(scale.x, scale.y, northDepth), material, keepCollider);
        }
    }

    private static void CreateRoofDetails(string name, Vector3 position, Vector3 scale, Material trim, Material window)
    {
        float top = position.y + scale.y * 0.5f;
        CreateCube(name + "_Chimney", position + new Vector3(-scale.x * 0.24f, 0.7f, scale.z * 0.2f), new Vector3(0.55f, 1.15f, 0.55f), trim, true);
        CreateCube(name + "_Chimney_Glow", position + new Vector3(-scale.x * 0.24f, 1.32f, scale.z * 0.2f), new Vector3(0.46f, 0.06f, 0.46f), window, false);
        CreateCube(name + "_Vent_A", new Vector3(position.x + scale.x * 0.24f, top + 0.25f, position.z - scale.z * 0.2f), new Vector3(0.8f, 0.22f, 0.5f), trim, true);
        CreateCylinderBetween(name + "_Antenna", new Vector3(position.x + scale.x * 0.31f, top + 0.15f, position.z + scale.z * 0.23f), new Vector3(position.x + scale.x * 0.31f, top + 1.6f, position.z + scale.z * 0.23f), 0.025f, trim, false);
        CreateVisualWorldPrimitive(name + "_Warm_Roof_Lantern", PrimitiveType.Sphere, new Vector3(position.x + scale.x * 0.35f, top + 0.45f, position.z + scale.z * 0.28f), Vector3.one * 0.18f, window, false);
    }

    private static void CreateFacadeWindows(string prefix, Vector3 center, Vector3 scale, Material window)
    {
        int columns = Mathf.Max(2, Mathf.RoundToInt(scale.x / 2f));
        int rows = 3;
        for (int side = 0; side < 2; side++)
        {
            float z = center.z + (side == 0 ? -scale.z * 0.505f : scale.z * 0.505f);
            for (int c = 0; c < columns; c++)
            {
                for (int r = 0; r < rows; r++)
                {
                    if ((c + r + prefix.Length) % 4 == 0)
                    {
                        continue;
                    }

                    float x = center.x - scale.x * 0.38f + c * (scale.x * 0.76f / Mathf.Max(1, columns - 1));
                    float y = center.y - scale.y * 0.28f + r * 1.05f;
                    CreateCube(prefix + "_Window_" + side + "_" + c + "_" + r, new Vector3(x, y, z), new Vector3(0.55f, 0.5f, 0.06f), window, false);
                }
            }
        }
    }

    private static void CreateAcousticMaze(Material material, Material trim, Material window)
    {
        CreateCube("Acoustic_Wall_Blocking_Direct_C", new Vector3(5.9f, 1.2f, 2.35f), new Vector3(0.45f, 2.2f, 3.4f), material, true);
        CreateCube("Acoustic_Wall_Forcing_Angle_E", new Vector3(8.5f, 1.25f, 0.55f), new Vector3(3.2f, 2.1f, 0.45f), material, true);
        CreateCube("Chimney_Sound_Shadow", new Vector3(16.8f, 2.1f, 1.15f), new Vector3(1f, 2.6f, 1f), material, true);
        CreateCube("Chimney_Relay_Tower", new Vector3(24.1f, 2.2f, 3.45f), new Vector3(0.8f, 2.2f, 0.8f), material, true);
        CreateCube("Acoustic_Wall_Blocking_Direct_C_Glow", new Vector3(5.66f, 1.55f, 2.35f), new Vector3(0.04f, 0.4f, 1.1f), window, false);
        CreateCube("Chimney_Relay_Tower_Cap", new Vector3(24.1f, 3.38f, 3.45f), new Vector3(1.05f, 0.18f, 1.05f), trim, true);
    }

    private static void CreateDecorativeCity(Material glow, Material facade, Material darkBrick, Material trim, Material window, Material roof, Material neonPink, Material neonCyan)
    {
        CreateCube("Background_City_Ground_Block", new Vector3(13f, -4.35f, 13.2f), new Vector3(64f, 0.22f, 8.5f), trim, true);
        for (int i = 0; i < 14; i++)
        {
            float x = -12f + i * 4.3f;
            float z = 11.5f + (i % 3) * 2.2f;
            float height = 4.2f + (i % 5) * 1.1f;
            float groundY = -4.25f;
            Material wall = i % 2 == 0 ? facade : darkBrick;
            CreateFacadeBuilding("Background_Building_" + i, new Vector3(x, groundY + height * 0.5f, z), new Vector3(2.9f, height, 2.2f), wall, trim, window, true);
            CreateCube("Background_Building_" + i + "_Foundation", new Vector3(x, groundY + 0.1f, z), new Vector3(3.15f, 0.26f, 2.45f), trim, true);
            CreateCube("Background_Roof_" + i, new Vector3(x, groundY + height + 0.16f, z), new Vector3(3.2f, 0.28f, 2.5f), roof, true);
            if (i % 3 == 0)
            {
                Material neon = i % 2 == 0 ? neonPink : neonCyan;
                CreateCube("Background_Building_" + i + "_Neon_Sign", new Vector3(x, groundY + height * 0.72f, z - 1.16f), new Vector3(1.45f, 0.42f, 0.06f), neon, false);
                CreateCube("Background_Building_" + i + "_Neon_Sign_Stripe", new Vector3(x, groundY + height * 0.72f - 0.36f, z - 1.18f), new Vector3(1.05f, 0.08f, 0.08f), neon, false);
            }
        }

        CreateCube("Far_Home_Glow_Backing", new Vector3(28.45f, 2.1f, 2.18f), new Vector3(2.3f, 2.45f, 0.18f), glow, false);
    }

    private static void CreateDownloadedLanternDecor(Material lanternMaterial, Material glow)
    {
        Vector3[] positions =
        {
            new Vector3(1.3f, 0.55f, -3.0f),
            new Vector3(9.9f, 0.86f, 4.35f),
            new Vector3(16.0f, 1.25f, 3.1f),
            new Vector3(25.6f, 1.75f, -0.75f)
        };

        for (int i = 0; i < positions.Length; i++)
        {
            if (!TryCreateDownloadedLantern("CC0_Lantern_" + i, positions[i], 0.55f, lanternMaterial))
            {
                CreateProceduralLantern("Procedural_Lantern_" + i, positions[i], lanternMaterial, glow);
            }

            GameObject lightObject = new GameObject("Lantern_Warm_Point_Light_" + i);
            lightObject.transform.position = positions[i] + Vector3.up * 0.6f;
            Light light = lightObject.AddComponent<Light>();
            light.type = LightType.Point;
            light.color = new Color(1f, 0.58f, 0.25f);
            light.range = 4.5f;
            light.intensity = 1.5f;
        }
    }

    private static bool TryCreateDownloadedLantern(string name, Vector3 position, float scale, Material fallbackMaterial)
    {
        GameObject asset = AssetDatabase.LoadAssetAtPath<GameObject>(LanternModelPath);
        if (asset == null)
        {
            return false;
        }

        GameObject instance = PrefabUtility.InstantiatePrefab(asset) as GameObject;
        if (instance == null)
        {
            return false;
        }

        instance.name = name;
        instance.transform.position = position;
        instance.transform.localScale = Vector3.one * scale;
        ApplyMaterialRecursive(instance, fallbackMaterial);
        DestroyCollidersInChildren(instance);
        return true;
    }

    private static void CreateProceduralLantern(string name, Vector3 position, Material metal, Material glow)
    {
        CreateVisualWorldPrimitive(name + "_Body", PrimitiveType.Cylinder, position + Vector3.up * 0.25f, new Vector3(0.28f, 0.34f, 0.28f), metal, false);
        CreateVisualWorldPrimitive(name + "_Glass", PrimitiveType.Cube, position + Vector3.up * 0.56f, new Vector3(0.42f, 0.45f, 0.42f), glow, false);
        CreateCylinderBetween(name + "_Handle", position + new Vector3(-0.2f, 0.92f, 0f), position + new Vector3(0.2f, 0.92f, 0f), 0.025f, metal, false);
    }

    private static void CreateFacadeBuilding(string name, Vector3 position, Vector3 scale, Material wall, Material trim, Material window, bool staticForLighting)
    {
        CreateCube(name, position, scale, wall, staticForLighting);
        CreateCube(name + "_TopTrim", position + new Vector3(0f, scale.y * 0.5f + 0.12f, 0f), new Vector3(scale.x + 0.2f, 0.22f, scale.z + 0.2f), trim, staticForLighting);

        int columns = Mathf.Max(2, Mathf.RoundToInt(scale.x / 1.2f));
        int rows = Mathf.Max(2, Mathf.RoundToInt(scale.y / 1.25f));
        for (int c = 0; c < columns; c++)
        {
            for (int r = 0; r < rows; r++)
            {
                if ((c * 7 + r * 3 + name.Length) % 5 == 0)
                {
                    continue;
                }

                float x = position.x - scale.x * 0.34f + c * (scale.x * 0.68f / Mathf.Max(1, columns - 1));
                float y = position.y - scale.y * 0.33f + r * (scale.y * 0.66f / Mathf.Max(1, rows - 1));
                float z = position.z - scale.z * 0.515f;
                CreateCube(name + "_Lit_Window_" + c + "_" + r, new Vector3(x, y, z), new Vector3(0.42f, 0.36f, 0.06f), window, false);
            }
        }
    }

    private static GameObject CreateCatPlayer(Material fur, Material muzzle, Material tailTip, Material eye, Material nose, Material collar, Material bell)
    {
        GameObject root = new GameObject("Player_Cat");
        root.transform.position = new Vector3(-2.8f, 0.62f, -1.6f);
        CharacterController controller = root.AddComponent<CharacterController>();
        controller.height = 1.15f;
        controller.radius = 0.34f;
        controller.center = new Vector3(0f, 0.57f, 0f);
        controller.stepOffset = 0.55f;
        controller.slopeLimit = 60f;
        controller.skinWidth = 0.04f;
        controller.minMoveDistance = 0f;
        root.AddComponent<CatController>();
        root.AddComponent<CatObjectInteractor>();
        BuildCatVisual(root.transform, fur, muzzle, tailTip, eye, nose, collar, bell, 1f);
        return root;
    }

    private static void CreateKitten(string name, Vector3 position, Material fur, Material muzzle, Material tailTip, Material eye, Material nose, Material collar, Material bell, Material glow)
    {
        GameObject root = new GameObject(name);
        root.transform.position = position;
        BuildCatVisual(root.transform, fur, muzzle, tailTip, eye, nose, collar, bell, 0.65f);
        CreateVisualPrimitive("Rescue_Glow_Halo", PrimitiveType.Cylinder, root.transform, new Vector3(0f, 0.035f, 0f), new Vector3(0.72f, 0.012f, 0.72f), glow);
        CreateVisualPrimitive("Tiny_Hope_Light", PrimitiveType.Sphere, root.transform, new Vector3(0f, 1.18f, 0f), Vector3.one * 0.09f, glow);
        GameObject lightObject = new GameObject(name + "_Soft_Rescue_Light");
        lightObject.transform.position = position + new Vector3(0f, 1.2f, 0f);
        Light light = lightObject.AddComponent<Light>();
        light.type = LightType.Point;
        light.color = new Color(1f, 0.62f, 0.3f);
        light.range = 3.2f;
        light.intensity = 1.15f;
        root.AddComponent<KittenRescue>();
    }

    private static void BuildCatVisual(Transform root, Material fur, Material muzzle, Material tailTip, Material eye, Material nose, Material collar, Material bell, float scale)
    {
        GameObject body = CreateVisualPrimitive("Soft_Capsule_Body", PrimitiveType.Capsule, root, new Vector3(0f, 0.48f * scale, 0f), new Vector3(0.42f, 0.58f, 0.42f) * scale, fur);
        body.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
        GameObject chest = CreateVisualPrimitive("Round_Chest", PrimitiveType.Sphere, root, new Vector3(0f, 0.49f * scale, 0.26f * scale), new Vector3(0.5f, 0.43f, 0.38f) * scale, muzzle);
        GameObject head = CreateVisualPrimitive("Expressive_Head", PrimitiveType.Sphere, root, new Vector3(0f, 0.86f * scale, 0.56f * scale), new Vector3(0.46f, 0.42f, 0.44f) * scale, fur);
        GameObject muzzleObj = CreateVisualPrimitive("Soft_Muzzle", PrimitiveType.Sphere, head.transform, new Vector3(0f, -0.05f, 0.34f) * scale, new Vector3(0.22f, 0.13f, 0.13f) * scale, muzzle);
        CreateTailSegment(root, "Simple_Tail", new Vector3(0f, 0.64f, -0.74f) * scale, new Vector3(0.12f, 0.58f, 0.12f) * scale, new Vector3(52f, 0f, 0f), fur);

        CreateVisualPrimitive("Ear_Left", PrimitiveType.Cube, head.transform, new Vector3(-0.2f, 0.31f, 0.02f) * scale, new Vector3(0.19f, 0.29f, 0.08f) * scale, fur).transform.localRotation = Quaternion.Euler(0f, 0f, 34f);
        CreateVisualPrimitive("Ear_Right", PrimitiveType.Cube, head.transform, new Vector3(0.2f, 0.31f, 0.02f) * scale, new Vector3(0.19f, 0.29f, 0.08f) * scale, fur).transform.localRotation = Quaternion.Euler(0f, 0f, -34f);
        CreateVisualPrimitive("Ear_Left_Inner", PrimitiveType.Cube, head.transform, new Vector3(-0.2f, 0.3f, 0.05f) * scale, new Vector3(0.1f, 0.18f, 0.035f) * scale, nose).transform.localRotation = Quaternion.Euler(0f, 0f, 34f);
        CreateVisualPrimitive("Ear_Right_Inner", PrimitiveType.Cube, head.transform, new Vector3(0.2f, 0.3f, 0.05f) * scale, new Vector3(0.1f, 0.18f, 0.035f) * scale, nose).transform.localRotation = Quaternion.Euler(0f, 0f, -34f);
        CreateVisualPrimitive("Eye_Left_Glow", PrimitiveType.Sphere, head.transform, new Vector3(-0.13f, 0.04f, 0.36f) * scale, Vector3.one * 0.065f * scale, eye);
        CreateVisualPrimitive("Eye_Right_Glow", PrimitiveType.Sphere, head.transform, new Vector3(0.13f, 0.04f, 0.36f) * scale, Vector3.one * 0.065f * scale, eye);
        CreateVisualPrimitive("Eye_Left_Spark", PrimitiveType.Sphere, head.transform, new Vector3(-0.112f, 0.062f, 0.405f) * scale, Vector3.one * 0.022f * scale, muzzle);
        CreateVisualPrimitive("Eye_Right_Spark", PrimitiveType.Sphere, head.transform, new Vector3(0.148f, 0.062f, 0.405f) * scale, Vector3.one * 0.022f * scale, muzzle);
        CreateVisualPrimitive("Tiny_Nose", PrimitiveType.Sphere, head.transform, new Vector3(0f, -0.04f, 0.46f) * scale, new Vector3(0.06f, 0.04f, 0.04f) * scale, nose);
        CreateVisualPrimitive("Left_Cheek", PrimitiveType.Sphere, head.transform, new Vector3(-0.19f, -0.08f, 0.39f) * scale, new Vector3(0.055f, 0.028f, 0.025f) * scale, nose);
        CreateVisualPrimitive("Right_Cheek", PrimitiveType.Sphere, head.transform, new Vector3(0.19f, -0.08f, 0.39f) * scale, new Vector3(0.055f, 0.028f, 0.025f) * scale, nose);
        CreateVisualPrimitive("Collar", PrimitiveType.Cube, root, new Vector3(0f, 0.72f * scale, 0.35f * scale), new Vector3(0.52f, 0.07f, 0.08f) * scale, collar);
        CreateVisualPrimitive("Collar_Bell", PrimitiveType.Sphere, root, new Vector3(0f, 0.62f * scale, 0.58f * scale), Vector3.one * 0.08f * scale, bell);

        for (int i = 0; i < 4; i++)
        {
            float x = i < 2 ? -0.22f : 0.22f;
            float z = i % 2 == 0 ? 0.28f : -0.28f;
            CreateVisualPrimitive("Leg_" + i, PrimitiveType.Capsule, root, new Vector3(x * scale, 0.22f * scale, z * scale), new Vector3(0.1f, 0.22f, 0.1f) * scale, fur);
            CreateVisualPrimitive("Paw_" + i, PrimitiveType.Sphere, root, new Vector3(x * scale, 0.06f * scale, (z + 0.04f) * scale), new Vector3(0.14f, 0.07f, 0.18f) * scale, muzzle);
        }

        CreateWhisker(head.transform, "Whisker_L_0", new Vector3(-0.13f, -0.04f, 0.43f) * scale, new Vector3(-0.48f, 0.02f, 0.52f) * scale, muzzle);
        CreateWhisker(head.transform, "Whisker_L_1", new Vector3(-0.13f, -0.08f, 0.43f) * scale, new Vector3(-0.46f, -0.12f, 0.5f) * scale, muzzle);
        CreateWhisker(head.transform, "Whisker_R_0", new Vector3(0.13f, -0.04f, 0.43f) * scale, new Vector3(0.48f, 0.02f, 0.52f) * scale, muzzle);
        CreateWhisker(head.transform, "Whisker_R_1", new Vector3(0.13f, -0.08f, 0.43f) * scale, new Vector3(0.46f, -0.12f, 0.5f) * scale, muzzle);

        body.name = root.name + "_Body";
        chest.name = root.name + "_Chest";
        muzzleObj.name = root.name + "_Muzzle";
    }

    private static void CreateTailSegment(Transform root, string name, Vector3 localPosition, Vector3 localScale, Vector3 euler, Material material)
    {
        GameObject segment = CreateVisualPrimitive(name, PrimitiveType.Capsule, root, localPosition, localScale, material);
        segment.transform.localRotation = Quaternion.Euler(euler);
    }

    private static void CreateWhisker(Transform parent, string name, Vector3 start, Vector3 end, Material material)
    {
        Vector3 midpoint = (start + end) * 0.5f;
        Vector3 direction = end - start;
        GameObject whisker = CreateVisualPrimitive(name, PrimitiveType.Cylinder, parent, midpoint, new Vector3(0.012f, direction.magnitude * 0.5f, 0.012f), material);
        whisker.transform.localRotation = Quaternion.FromToRotation(Vector3.up, direction.normalized);
    }

    private static GameObject CreateVisualPrimitive(string name, PrimitiveType type, Transform parent, Vector3 localPosition, Vector3 localScale, Material material)
    {
        GameObject gameObject = GameObject.CreatePrimitive(type);
        gameObject.name = name;
        gameObject.transform.SetParent(parent, false);
        gameObject.transform.localPosition = localPosition;
        gameObject.transform.localScale = localScale;
        DestroyCollider(gameObject);
        ApplyMaterial(gameObject, material);
        return gameObject;
    }

    private static void CreateBell(string name, Vector3 position, int note, Material material, Material trim)
    {
        GameObject root = new GameObject(name);
        root.transform.position = position;
        CreateVisualPrimitive("Bell_Arch_L", PrimitiveType.Cylinder, root.transform, new Vector3(-0.38f, 0.42f, 0f), new Vector3(0.045f, 0.55f, 0.045f), trim);
        CreateVisualPrimitive("Bell_Arch_R", PrimitiveType.Cylinder, root.transform, new Vector3(0.38f, 0.42f, 0f), new Vector3(0.045f, 0.55f, 0.045f), trim);
        CreateVisualPrimitive("Bell_Crossbar", PrimitiveType.Cube, root.transform, new Vector3(0f, 0.96f, 0f), new Vector3(0.9f, 0.08f, 0.08f), trim);
        GameObject bell = CreateVisualPrimitive("Bell_" + ProceduralAudio.GetNoteName(note), PrimitiveType.Sphere, root.transform, new Vector3(0f, 0.35f, 0f), new Vector3(0.55f, 0.42f, 0.55f), material);
        bell.AddComponent<SphereCollider>().isTrigger = true;
        ResonanceBell resonanceBell = root.AddComponent<ResonanceBell>();
        resonanceBell.BellNote = note;
        CreateBellParticles(root.transform);
    }

    private static void CreateResonantPlatform(string name, Vector3 position, Vector3 scale, int note, Vector3 activeOffset, Vector3 activeRotation, Material material, Material accent)
    {
        GameObject platform = CreateCube(name, position, scale, material, false);
        CreateCube(name + "_Glow_Stripe", position + new Vector3(0f, scale.y * 0.55f, 0f), new Vector3(scale.x * 0.86f, 0.035f, scale.z * 0.2f), accent, false);
        ResonantPlatform resonantPlatform = platform.AddComponent<ResonantPlatform>();
        resonantPlatform.TriggerNote = note;
        resonantPlatform.ActiveOffset = activeOffset;
        resonantPlatform.ActiveRotation = activeRotation;
    }

    private static GameObject CreateGate(string name, Vector3 position, Vector3 scale, Material gate, Material accent)
    {
        GameObject root = CreateCube(name, position, scale, gate, false);
        GameObject top = CreateCube(name + "_Gold_Edge_Top", position + new Vector3(0f, scale.y * 0.52f, 0f), new Vector3(scale.x * 1.25f, 0.12f, scale.z * 1.1f), accent, false);
        GameObject bottom = CreateCube(name + "_Gold_Edge_Bottom", position + new Vector3(0f, -scale.y * 0.52f, 0f), new Vector3(scale.x * 1.25f, 0.12f, scale.z * 1.1f), accent, false);
        top.transform.SetParent(root.transform, true);
        bottom.transform.SetParent(root.transform, true);
        DestroyCollider(top);
        DestroyCollider(bottom);
        return root;
    }

    private static void CreateCacheableProp(string name, PrimitiveType primitiveType, Vector3 position, Vector3 scale, string displayName, float throwMultiplier, Material material)
    {
        GameObject prop = GameObject.CreatePrimitive(primitiveType);
        prop.name = name;
        prop.transform.position = position;
        prop.transform.localScale = scale;
        ApplyMaterial(prop, material);

        Rigidbody body = prop.AddComponent<Rigidbody>();
        body.mass = Mathf.Clamp(scale.x * scale.y * scale.z * 2.5f, 0.4f, 4f);
        body.linearDamping = 0.2f;
        body.angularDamping = 0.05f;
        body.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        body.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        CacheableObject cacheable = prop.AddComponent<CacheableObject>();
        cacheable.Configure(displayName, throwMultiplier);
    }

    private static void CreateBellParticles(Transform parent)
    {
        GameObject particleObject = new GameObject("Sound_Ring_Particles");
        particleObject.transform.SetParent(parent, false);
        ParticleSystem particles = particleObject.AddComponent<ParticleSystem>();
        ParticleSystem.MainModule main = particles.main;
        main.duration = 0.5f;
        main.startLifetime = 0.55f;
        main.startSpeed = 2.6f;
        main.startSize = 0.08f;
        main.loop = false;
        main.playOnAwake = false;
        main.startColor = new Color(1f, 0.92f, 0.45f, 0.8f);

        ParticleSystem.EmissionModule emission = particles.emission;
        emission.rateOverTime = 0f;
        emission.SetBursts(new[] { new ParticleSystem.Burst(0f, (short)34) });

        ParticleSystem.ShapeModule shape = particles.shape;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 0.25f;
    }

    private static void CreateGoal(Vector3 position, Material material, Material trim, Material window)
    {
        GameObject goal = CreateCube("Glowing_Home_Window_Goal", position, new Vector3(1.6f, 1.8f, 0.25f), material, true);
        CreateCube("Home_Window_Frame_Top", position + new Vector3(0f, 1f, -0.03f), new Vector3(1.95f, 0.15f, 0.34f), trim, false);
        CreateCube("Home_Window_Frame_Bottom", position + new Vector3(0f, -1f, -0.03f), new Vector3(1.95f, 0.15f, 0.34f), trim, false);
        CreateCube("Home_Window_Cross", position + new Vector3(0f, 0f, -0.06f), new Vector3(0.08f, 1.72f, 0.34f), window, false);
        CreateVisualWorldPrimitive("Home_Goal_Golden_Halo", PrimitiveType.Sphere, position + new Vector3(0f, 0f, -0.18f), new Vector3(2.25f, 2.45f, 0.18f), material, false);
        CreateCube("Home_Goal_Arrow_Body", position + new Vector3(-2.45f, -1.25f, -0.08f), new Vector3(1.25f, 0.12f, 0.22f), window, false);
        CreateCube("Home_Goal_Arrow_Head", position + new Vector3(-1.75f, -1.25f, -0.08f), new Vector3(0.32f, 0.32f, 0.24f), material, false);
        GameObject lightObject = new GameObject("Home_Goal_Strong_Warm_Light");
        lightObject.transform.position = position + new Vector3(0f, 0f, -0.85f);
        Light light = lightObject.AddComponent<Light>();
        light.type = LightType.Point;
        light.color = new Color(1f, 0.63f, 0.25f);
        light.range = 7f;
        light.intensity = 2.8f;
        BoxCollider collider = goal.GetComponent<BoxCollider>();
        collider.isTrigger = true;
        collider.size = new Vector3(2.4f, 2.5f, 1.2f);
        goal.AddComponent<RooftopGoal>();
    }

    private static void CreateRain()
    {
        Material rainMaterial = CreateParticleMaterial("M_Grey_Rain_Particles_Unlit", new Color(0.58f, 0.6f, 0.62f, 0.68f));
        Material splashMaterial = CreateParticleMaterial("M_Grey_Rain_Splash_Particles_Unlit", new Color(0.62f, 0.63f, 0.65f, 0.36f));

        GameObject rain = new GameObject("Heavy_Rain_Volume_Around_Rooftops");
        rain.transform.position = new Vector3(13f, 5.2f, 1f);
        ParticleSystem particles = rain.AddComponent<ParticleSystem>();
        ParticleSystem.MainModule main = particles.main;
        main.loop = true;
        main.startLifetime = 1.55f;
        main.startSpeed = 0.25f;
        main.startSize = 0.028f;
        main.maxParticles = 3600;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.startColor = new Color(0.58f, 0.6f, 0.62f, 0.58f);

        ParticleSystem.EmissionModule emission = particles.emission;
        emission.rateOverTime = 760f;

        ParticleSystem.ShapeModule shape = particles.shape;
        shape.shapeType = ParticleSystemShapeType.Box;
        shape.scale = new Vector3(42f, 1.2f, 24f);

        ParticleSystem.VelocityOverLifetimeModule velocity = particles.velocityOverLifetime;
        velocity.enabled = true;
        velocity.x = new ParticleSystem.MinMaxCurve(-0.65f);
        velocity.y = new ParticleSystem.MinMaxCurve(-9.5f);
        velocity.z = new ParticleSystem.MinMaxCurve(0.25f);

        ParticleSystemRenderer renderer = particles.GetComponent<ParticleSystemRenderer>();
        renderer.renderMode = ParticleSystemRenderMode.Stretch;
        renderer.lengthScale = 1.8f;
        renderer.velocityScale = 0.12f;
        renderer.material = rainMaterial;
        renderer.sharedMaterial = rainMaterial;

        GameObject splashes = new GameObject("Rain_Splash_Mist_On_Roofs");
        splashes.transform.position = new Vector3(13f, 1.2f, 1f);
        ParticleSystem splashParticles = splashes.AddComponent<ParticleSystem>();
        ParticleSystem.MainModule splashMain = splashParticles.main;
        splashMain.loop = true;
        splashMain.startLifetime = 0.42f;
        splashMain.startSpeed = 0.8f;
        splashMain.startSize = 0.045f;
        splashMain.maxParticles = 1200;
        splashMain.simulationSpace = ParticleSystemSimulationSpace.World;
        splashMain.startColor = new Color(0.62f, 0.63f, 0.65f, 0.28f);
        ParticleSystem.EmissionModule splashEmission = splashParticles.emission;
        splashEmission.rateOverTime = 260f;
        ParticleSystem.ShapeModule splashShape = splashParticles.shape;
        splashShape.shapeType = ParticleSystemShapeType.Box;
        splashShape.scale = new Vector3(36f, 1f, 18f);
        ParticleSystemRenderer splashRenderer = splashParticles.GetComponent<ParticleSystemRenderer>();
        splashRenderer.renderMode = ParticleSystemRenderMode.Billboard;
        splashRenderer.material = splashMaterial;
        splashRenderer.sharedMaterial = splashMaterial;
    }

    private static void CreateStreetLamp(string name, Vector3 position, Material pole, Material glow)
    {
        CreateCylinderBetween(name + "_Pole", position + Vector3.down * 1.7f, position + Vector3.up * 1.7f, 0.055f, pole, true);
        CreateCube(name + "_Arm", position + new Vector3(0.38f, 1.55f, 0f), new Vector3(0.85f, 0.08f, 0.08f), pole, true);
        CreateVisualWorldPrimitive(name + "_Lamp_Glow", PrimitiveType.Sphere, position + new Vector3(0.78f, 1.42f, 0f), new Vector3(0.34f, 0.26f, 0.34f), glow, false);
        GameObject lightObject = new GameObject(name + "_PointLight");
        lightObject.transform.position = position + new Vector3(0.78f, 1.35f, 0f);
        Light light = lightObject.AddComponent<Light>();
        light.type = LightType.Point;
        light.color = new Color(1f, 0.66f, 0.34f);
        light.range = 6f;
        light.intensity = 1.7f;
    }

    private static void CreatePuddle(string name, Vector3 position, Vector3 scale, Material material)
    {
        CreateVisualWorldPrimitive(name, PrimitiveType.Cylinder, position, scale, material, false);
    }

    private static GameObject CreateCube(string name, Vector3 position, Vector3 scale, Material material, bool staticForLighting)
    {
        GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
        gameObject.name = name;
        gameObject.transform.position = position;
        gameObject.transform.localScale = scale;
        ApplyMaterial(gameObject, material);

        if (staticForLighting)
        {
            GameObjectUtility.SetStaticEditorFlags(gameObject, StaticEditorFlags.ContributeGI | StaticEditorFlags.BatchingStatic | StaticEditorFlags.OccluderStatic);
        }

        return gameObject;
    }

    private static GameObject CreateVisualWorldPrimitive(string name, PrimitiveType type, Vector3 position, Vector3 scale, Material material, bool keepCollider)
    {
        GameObject gameObject = GameObject.CreatePrimitive(type);
        gameObject.name = name;
        gameObject.transform.position = position;
        gameObject.transform.localScale = scale;
        ApplyMaterial(gameObject, material);
        if (!keepCollider)
        {
            DestroyCollider(gameObject);
        }

        return gameObject;
    }

    private static void CreateCylinderBetween(string name, Vector3 start, Vector3 end, float radius, Material material, bool keepCollider)
    {
        Vector3 direction = end - start;
        GameObject cylinder = CreateVisualWorldPrimitive(name, PrimitiveType.Cylinder, (start + end) * 0.5f, new Vector3(radius, direction.magnitude * 0.5f, radius), material, keepCollider);
        cylinder.transform.rotation = Quaternion.FromToRotation(Vector3.up, direction.normalized);
    }

    private static void CreateStoryTriggers()
    {
        CreateStoryTrigger(
            "Story_Start",
            new Vector3(-2.8f, 1.2f, -1.6f),
            new Vector3(4.5f, 2.4f, 4.5f),
            "Найди 1-й колокольчик: выбери первую ноту (нажми 1), подойди к нему и нажми F.",
            "Котята потерялись во время грозы. Чтобы открыть путь, сыграй мелодию из 3-х колокольчиков.");

        CreateStoryTrigger(
            "Story_Blocking_Crate",
            new Vector3(3.9f, 1.2f, 1.05f),
            new Vector3(3.2f, 2.4f, 2.7f),
            "Путь завален ящиком.",
            "Убери их.");

        CreateStoryTrigger(
            "Story_First_Bell",
            new Vector3(1.7f, 1.4f, 2.4f),
            new Vector3(3.2f, 2.8f, 3.2f),
            "Это 1-й колокольчик.",
            "Колокольчики реагируют только на правильные ноты. Подсказка: 1 -> 2 -> 3.");

        CreateStoryTrigger(
            "Story_Cache_Lesson",
            new Vector3(8.2f, 1.35f, 3.1f),
            new Vector3(4.4f, 2.8f, 3.8f),
            "Коробка и бочка глушат ноту.",
            "Убери их.");

        CreateStoryTrigger(
            "Story_Kittens",
            new Vector3(17.4f, 1.7f, -0.8f),
            new Vector3(5.2f, 2.8f, 6.8f),
            "Котят не ловят, их успокаивают. Подойди почти вплотную и держи Shift...",
            "Они боятся грозы. Им нужно спокойное мурчание рядом, после этого они сами пойдут за тобой.");

        CreateStoryTrigger(
            "Story_Final_Gate",
            new Vector3(21.8f, 2.2f, 0.6f),
            new Vector3(4.5f, 3.2f, 4.5f),
            "Финал: если спасены 2 котёнка, нажми 3 = G и F у синих ворот. Потом иди к оранжевому окну.",
            "Оранжевое окно за воротами — это дом.");
    }

    private static void CreateStoryTrigger(string name, Vector3 position, Vector3 scale, string objective, string story)
    {
        GameObject trigger = new GameObject(name);
        trigger.transform.position = position;
        BoxCollider collider = trigger.AddComponent<BoxCollider>();
        collider.isTrigger = true;
        collider.size = scale;
        StoryTrigger storyTrigger = trigger.AddComponent<StoryTrigger>();
        storyTrigger.Configure(objective, story);
    }

    private static void CreateCameraRig(Transform player)
    {
        GameObject cameraObject = new GameObject("Main Camera");
        Camera camera = cameraObject.AddComponent<Camera>();
        cameraObject.AddComponent<AudioListener>();
        cameraObject.tag = "MainCamera";
        cameraObject.transform.position = player.position + new Vector3(0f, 6.7f, -8.2f);
        cameraObject.transform.LookAt(player.position + Vector3.up);
        camera.fieldOfView = 50f;
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = new Color(0.02f, 0.025f, 0.045f);
        FollowCamera fallbackFollow = cameraObject.AddComponent<FollowCamera>();
        fallbackFollow.Target = player;
        fallbackFollow.SnapToTarget();
    }

    private static T CreateGameObjectWithComponent<T>(string name) where T : Component
    {
        GameObject gameObject = new GameObject(name);
        return gameObject.AddComponent<T>();
    }

    private static void ApplyMaterial(GameObject gameObject, Material material)
    {
        Renderer renderer = gameObject.GetComponent<Renderer>();
        if (renderer != null && material != null)
        {
            renderer.sharedMaterial = material;
        }
    }

    private static void ApplyMaterialRecursive(GameObject gameObject, Material material)
    {
        Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer>();
        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i] != null && material != null)
            {
                renderers[i].sharedMaterial = material;
            }
        }
    }

    private static void DestroyCollider(GameObject gameObject)
    {
        Collider collider = gameObject.GetComponent<Collider>();
        if (collider != null)
        {
            UnityEngine.Object.DestroyImmediate(collider);
        }
    }

    private static void DestroyCollidersInChildren(GameObject gameObject)
    {
        Collider[] colliders = gameObject.GetComponentsInChildren<Collider>();
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i] != null)
            {
                UnityEngine.Object.DestroyImmediate(colliders[i]);
            }
        }
    }
}
