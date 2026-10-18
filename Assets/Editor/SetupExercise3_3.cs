#if UNITY_EDITOR
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEditor.Events;
using TMPro;

/// <summary>
/// Automated Unity Editor Script to generate, build, and configure 100% of the specifications
/// for Slide 3.3: "Practice: UI Design Elements" in Unity 2021.3.
/// Menu: "Week 8 > Setup Exercise 3.3 - UI Popup"
/// </summary>
[InitializeOnLoad]
public static class SetupExercise3_3
{
    private const string SCENE_3_2_PATH = "Assets/Scenes/_Scene_3_2.unity";
    private const string SCENE_1_PATH = "Assets/Scenes/_Scene_1.unity";

    static SetupExercise3_3()
    {
        EditorApplication.delayCall += AutoRunIfFirstTime;
    }

    private static void AutoRunIfFirstTime()
    {
        if (SessionState.GetBool("Week8_Exercise3_3_AutoSetupRan", false)) return;
        SessionState.SetBool("Week8_Exercise3_3_AutoSetupRan", true);

        Debug.Log("[Week 8] Auto-generating Exercise 3.3 scenes and assets...");
        ExecuteSetup(false);
    }

    [MenuItem("Week 8/Setup Exercise 3.3 - UI Popup")]
    public static void RunManualSetup()
    {
        ExecuteSetup(true);
    }

    [MenuItem("Week 8/Setup All Practice Scenes")]
    public static void RunSetupAll()
    {
        ExecuteSetup(true);
    }

    /// <summary>
    /// Executes the full automated build for _Scene_3_2, _Scene_1, materials, UI hierarchy, and build settings.
    /// </summary>
    public static void ExecuteSetup(bool showDialog)
    {
        EnsureDirectories();
        CreateMaterials();
        BuildScene1();
        BuildScene3_2();
        ConfigureBuildSettings();
        EditorSceneManager.OpenScene(SCENE_3_2_PATH, OpenSceneMode.Single);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("[Week 8] Successfully configured Exercise 3.3 UI Popup scenes and references!");

        if (showDialog)
        {
            EditorUtility.DisplayDialog("Week 8 - Exercise 3.3 Setup Complete",
                "Successfully generated '_Scene_3_2' and '_Scene_1'!\n\n" +
                "• All 3D Objects (RunWay1, Airplane, Wall) are created.\n" +
                "• TextMeshPro popup UI hierarchy (Panel > Frame > Title, Message, Enter, Restart) configured.\n" +
                "• GameManager & EnterGame scripts attached and Inspector fields wired.\n" +
                "• Persistent OnClick() events wired.\n" +
                "• Registered in File > Build Settings.\n\n" +
                "You can now press Play in '_Scene_3_2' to test!", "OK");
        }
    }

    private static void EnsureDirectories()
    {
        string[] dirs = new string[]
        {
            "Assets/Scenes",
            "Assets/Scripts",
            "Assets/Materials",
            "Assets/Prefabs",
            "Assets/Editor"
        };

        foreach (var dir in dirs)
        {
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
        }
    }

    /// <summary>
    /// Creates and persists all required materials matching the slide aesthetic.
    /// </summary>
    private static void CreateMaterials()
    {
        // 1. Transparent Material for Wall (Slide 30 & 31)
        string wallMatPath = "Assets/Materials/Transparent.mat";
        Material wallMat = AssetDatabase.LoadAssetAtPath<Material>(wallMatPath);
        if (wallMat == null)
        {
            wallMat = new Material(Shader.Find("Standard"));
            wallMat.name = "Transparent";
            wallMat.color = new Color(0.20f, 0.65f, 0.95f, 0.35f); // Transparent blue-cyan
            wallMat.SetFloat("_Mode", 3); // Transparent mode
            wallMat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            wallMat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            wallMat.SetInt("_ZWrite", 0);
            wallMat.DisableKeyword("_ALPHATEST_ON");
            wallMat.EnableKeyword("_ALPHABLEND_ON");
            wallMat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            wallMat.renderQueue = 3000;
            AssetDatabase.CreateAsset(wallMat, wallMatPath);
        }

        // 2. Airplane Material (Glossy white)
        string planeMatPath = "Assets/Materials/Airplane_Mat.mat";
        Material planeMat = AssetDatabase.LoadAssetAtPath<Material>(planeMatPath);
        if (planeMat == null)
        {
            planeMat = new Material(Shader.Find("Standard"));
            planeMat.name = "Airplane_Mat";
            planeMat.color = new Color(0.96f, 0.96f, 0.98f, 1f);
            planeMat.SetFloat("_Glossiness", 0.65f);
            AssetDatabase.CreateAsset(planeMat, planeMatPath);
        }

        // 3. Runway Material (Dark tarmac asphalt)
        string runwayMatPath = "Assets/Materials/Runway_Mat.mat";
        Material runwayMat = AssetDatabase.LoadAssetAtPath<Material>(runwayMatPath);
        if (runwayMat == null)
        {
            runwayMat = new Material(Shader.Find("Standard"));
            runwayMat.name = "Runway_Mat";
            runwayMat.color = new Color(0.18f, 0.20f, 0.23f, 1f);
            runwayMat.SetFloat("_Glossiness", 0.15f);
            AssetDatabase.CreateAsset(runwayMat, runwayMatPath);
        }

        // 4. Runway Stripe Material (Bright white)
        string stripeMatPath = "Assets/Materials/Runway_Stripe_Mat.mat";
        Material stripeMat = AssetDatabase.LoadAssetAtPath<Material>(stripeMatPath);
        if (stripeMat == null)
        {
            stripeMat = new Material(Shader.Find("Standard"));
            stripeMat.name = "Runway_Stripe_Mat";
            stripeMat.color = new Color(0.95f, 0.95f, 0.95f, 1f);
            AssetDatabase.CreateAsset(stripeMat, stripeMatPath);
        }
    }

    /// <summary>
    /// Builds _Scene_3_2 matching 100% of Slide 3.3 specs.
    /// </summary>
    private static void BuildScene3_2()
    {
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

        // Fetch materials
        Material wallMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Transparent.mat");
        Material planeMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Airplane_Mat.mat");
        Material runwayMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Runway_Mat.mat");
        Material stripeMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Runway_Stripe_Mat.mat");

        // 1. Main Camera Setup
        Camera mainCam = Camera.main;
        if (mainCam == null)
        {
            GameObject camObj = new GameObject("Main Camera");
            mainCam = camObj.AddComponent<Camera>();
            camObj.tag = "MainCamera";
        }
        mainCam.name = "Main Camera";
        mainCam.transform.position = new Vector3(0f, 3.2f, -14f);
        mainCam.transform.rotation = Quaternion.Euler(8f, 0f, 0f);

        // Attach GameManager to Main Camera
        GameManager gm = mainCam.GetComponent<GameManager>();
        if (gm == null) gm = mainCam.gameObject.AddComponent<GameManager>();

        // 2. Directional Light
        Light dirLight = Object.FindObjectOfType<Light>();
        if (dirLight != null)
        {
            dirLight.name = "Directional Light";
            dirLight.transform.rotation = Quaternion.Euler(45f, -30f, 0f);
            dirLight.intensity = 1.15f;
            dirLight.color = new Color(1f, 0.98f, 0.92f);
        }

        // 3. Virtual Camera (as shown in slide hierarchy under Main Camera and Directional Light)
        GameObject vcamObj = new GameObject("Virtual Camera");
        vcamObj.transform.position = new Vector3(0f, 3.8f, -12f);
        vcamObj.transform.rotation = Quaternion.Euler(10f, 0f, 0f);

        // 4. Runway (RunWay1)
        GameObject runway = GameObject.CreatePrimitive(PrimitiveType.Cube);
        runway.name = "RunWay1";
        runway.transform.position = new Vector3(0f, -0.1f, 40f);
        runway.transform.localScale = new Vector3(14f, 0.2f, 130f);
        runway.GetComponent<MeshRenderer>().sharedMaterial = runwayMat;

        // Ground base plane
        GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
        ground.name = "Ground";
        ground.transform.position = new Vector3(0f, -0.2f, 40f);
        ground.transform.localScale = new Vector3(30f, 1f, 30f);
        Material groundMat = new Material(Shader.Find("Standard")) { color = new Color(0.30f, 0.38f, 0.30f) };
        ground.GetComponent<MeshRenderer>().sharedMaterial = groundMat;

        // Runway centerline stripes
        for (int z = -15; z <= 85; z += 10)
        {
            GameObject stripe = GameObject.CreatePrimitive(PrimitiveType.Cube);
            stripe.name = "Stripe_" + z;
            stripe.transform.SetParent(runway.transform, false);
            stripe.transform.localPosition = new Vector3(0f, 0.52f, (z - 40f) / 130f);
            stripe.transform.localScale = new Vector3(0.04f, 0.05f, 0.04f);
            stripe.GetComponent<MeshRenderer>().sharedMaterial = stripeMat;
            Object.DestroyImmediate(stripe.GetComponent<Collider>());
        }

        // 5. Airplane Model Setup (tagged "Player", Rigidbody, BoxCollider, AirplaneController)
        GameObject airplane = new GameObject("Airplane");
        airplane.tag = "Player";
        airplane.transform.position = new Vector3(0f, 0.5f, 0f);

        // Airplane Fuselage (Body)
        GameObject body = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        body.name = "Fuselage";
        body.transform.SetParent(airplane.transform, false);
        body.transform.localPosition = Vector3.zero;
        body.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
        body.transform.localScale = new Vector3(1.1f, 2.6f, 1.1f);
        body.GetComponent<MeshRenderer>().sharedMaterial = planeMat;
        Object.DestroyImmediate(body.GetComponent<Collider>());

        // Airplane Nose Cone
        GameObject nose = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        nose.name = "NoseCone";
        nose.transform.SetParent(airplane.transform, false);
        nose.transform.localPosition = new Vector3(0f, 0f, 2.5f);
        nose.transform.localScale = new Vector3(1.05f, 1.05f, 1.2f);
        nose.GetComponent<MeshRenderer>().sharedMaterial = planeMat;
        Object.DestroyImmediate(nose.GetComponent<Collider>());

        // Airplane Main Wings
        GameObject wings = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wings.name = "MainWings";
        wings.transform.SetParent(airplane.transform, false);
        wings.transform.localPosition = new Vector3(0f, 0.1f, 0.6f);
        wings.transform.localScale = new Vector3(8.5f, 0.12f, 1.6f);
        wings.GetComponent<MeshRenderer>().sharedMaterial = planeMat;
        Object.DestroyImmediate(wings.GetComponent<Collider>());

        // Airplane Horizontal Stabilizer (Tail Wings)
        GameObject tailWings = GameObject.CreatePrimitive(PrimitiveType.Cube);
        tailWings.name = "TailWings";
        tailWings.transform.SetParent(airplane.transform, false);
        tailWings.transform.localPosition = new Vector3(0f, 0.2f, -2.1f);
        tailWings.transform.localScale = new Vector3(3.2f, 0.1f, 0.9f);
        tailWings.GetComponent<MeshRenderer>().sharedMaterial = planeMat;
        Object.DestroyImmediate(tailWings.GetComponent<Collider>());

        // Airplane Vertical Tail Fin
        GameObject fin = GameObject.CreatePrimitive(PrimitiveType.Cube);
        fin.name = "VerticalFin";
        fin.transform.SetParent(airplane.transform, false);
        fin.transform.localPosition = new Vector3(0f, 0.9f, -2.0f);
        fin.transform.localRotation = Quaternion.Euler(15f, 0f, 0f);
        fin.transform.localScale = new Vector3(0.12f, 1.4f, 1.1f);
        fin.GetComponent<MeshRenderer>().sharedMaterial = planeMat;
        Object.DestroyImmediate(fin.GetComponent<Collider>());

        // Collider & Physics on Airplane Root
        BoxCollider planeCol = airplane.AddComponent<BoxCollider>();
        planeCol.center = new Vector3(0f, 0.35f, 0f);
        planeCol.size = new Vector3(8.5f, 1.8f, 5.5f);

        Rigidbody planeRb = airplane.AddComponent<Rigidbody>();
        planeRb.isKinematic = true;
        planeRb.useGravity = false;

        // Airplane flight movement logic
        AirplaneController planeCtrl = airplane.AddComponent<AirplaneController>();
        planeCtrl.forwardSpeed = 15f;
        planeCtrl.climbSpeed = 2.4f;
        planeCtrl.takeoffDistance = 12f;
        planeCtrl.maxAltitude = 4f;
        planeCtrl.autoFly = true;

        // 6. Wall Setup (Cube at the end of takeoff path, Transparent material, BoxCollider with isTrigger = true)
        GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wall.name = "Wall";
        wall.transform.position = new Vector3(0f, 3.5f, 55f);
        wall.transform.localScale = new Vector3(22f, 8f, 1.2f);
        wall.GetComponent<MeshRenderer>().sharedMaterial = wallMat;

        BoxCollider wallCol = wall.GetComponent<BoxCollider>();
        wallCol.isTrigger = true;

        EnterGame enterScript = wall.AddComponent<EnterGame>();

        // 7. UI Canvas & Hierarchy matching Pages 27, 28, 30, 34:
        // Canvas
        //  └── Panel
        //       └── Frame
        //            ├── Title
        //            ├── Message
        //            ├── Enter
        //            └── Restart
        GameObject canvasObj = new GameObject("Canvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObj.AddComponent<GraphicRaycaster>();

        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;

        // EventSystem
        if (Object.FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            GameObject esObj = new GameObject("EventSystem");
            esObj.AddComponent<UnityEngine.EventSystems.EventSystem>();
            esObj.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }

        // Builtin UI Sprite for rounded buttons and sliced panel
        Sprite uiSprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
        Sprite bgSprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Background.psd");

        // Panel (Semi-transparent blue popup panel centered on screen)
        GameObject panelObj = new GameObject("Panel");
        panelObj.transform.SetParent(canvasObj.transform, false);
        Image panelImg = panelObj.AddComponent<Image>();
        if (bgSprite != null)
        {
            panelImg.sprite = bgSprite;
            panelImg.type = Image.Type.Sliced;
        }
        // Curated semi-transparent blue matching Slide 28/30 preview:
        panelImg.color = new Color(0.22f, 0.38f, 0.65f, 0.82f);

        RectTransform panelRect = panelObj.GetComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.5f, 0.5f);
        panelRect.anchorMax = new Vector2(0.5f, 0.5f);
        panelRect.pivot = new Vector2(0.5f, 0.5f);
        panelRect.sizeDelta = new Vector2(720f, 400f);
        panelRect.anchoredPosition = Vector2.zero;

        // Frame (Empty GameObject container)
        GameObject frameObj = new GameObject("Frame");
        frameObj.transform.SetParent(panelObj.transform, false);
        RectTransform frameRect = frameObj.AddComponent<RectTransform>();
        frameRect.anchorMin = Vector2.zero;
        frameRect.anchorMax = Vector2.one;
        frameRect.sizeDelta = Vector2.zero;
        frameRect.anchoredPosition = Vector2.zero;

        // Title (TMP_Text: top-center, large white text, default editor text "Title")
        GameObject titleObj = new GameObject("Title");
        titleObj.transform.SetParent(frameObj.transform, false);
        TextMeshProUGUI titleTMP = titleObj.AddComponent<TextMeshProUGUI>();
        titleTMP.text = "Title"; // Default editor text per specifications
        titleTMP.fontSize = 46;
        titleTMP.fontStyle = FontStyles.Bold;
        titleTMP.alignment = TextAlignmentOptions.Center;
        titleTMP.color = Color.white;
        RectTransform titleRect = titleObj.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0.5f, 0.72f);
        titleRect.anchorMax = new Vector2(0.5f, 0.72f);
        titleRect.pivot = new Vector2(0.5f, 0.5f);
        titleRect.sizeDelta = new Vector2(660f, 70f);
        titleRect.anchoredPosition = Vector2.zero;

        // Message (TMP_Text: below Title, medium white text, default editor text "Message")
        GameObject msgObj = new GameObject("Message");
        msgObj.transform.SetParent(frameObj.transform, false);
        TextMeshProUGUI msgTMP = msgObj.AddComponent<TextMeshProUGUI>();
        msgTMP.text = "Message"; // Default editor text per specifications
        msgTMP.fontSize = 26;
        msgTMP.alignment = TextAlignmentOptions.Center;
        msgTMP.color = Color.white;
        RectTransform msgRect = msgObj.GetComponent<RectTransform>();
        msgRect.anchorMin = new Vector2(0.5f, 0.52f);
        msgRect.anchorMax = new Vector2(0.5f, 0.52f);
        msgRect.pivot = new Vector2(0.5f, 0.5f);
        msgRect.sizeDelta = new Vector2(660f, 50f);
        msgRect.anchoredPosition = Vector2.zero;

        // Color Tint block for buttons matching Page 34:
        // Normal Color: Orange-brown (#D47A39)
        // Highlighted Color: Dark brown (#965321)
        // Pressed Color: Bright green (#78D637)
        // Selected Color: White (#FFFFFF)
        // Disabled Color: Grey (#C8C8C8)
        // Fade Duration: 0.4
        ColorBlock btnColorBlock = ColorBlock.defaultColorBlock;
        btnColorBlock.normalColor = new Color32(0xD4, 0x7A, 0x39, 0xFF);     // #D47A39
        btnColorBlock.highlightedColor = new Color32(0x96, 0x53, 0x21, 0xFF); // #965321
        btnColorBlock.pressedColor = new Color32(0x78, 0xD6, 0x37, 0xFF);     // #78D637
        btnColorBlock.selectedColor = new Color32(0xFF, 0xFF, 0xFF, 0xFF);    // #FFFFFF
        btnColorBlock.disabledColor = new Color32(0xC8, 0xC8, 0xC8, 0x80);    // #C8C8C8
        btnColorBlock.colorMultiplier = 1f;
        btnColorBlock.fadeDuration = 0.4f;

        // Enter (Button - TextMeshPro: bottom-right button, orange rounded style, white text "Enter")
        GameObject enterBtnObj = new GameObject("Enter");
        enterBtnObj.transform.SetParent(frameObj.transform, false);
        Image enterImg = enterBtnObj.AddComponent<Image>();
        if (uiSprite != null)
        {
            enterImg.sprite = uiSprite;
            enterImg.type = Image.Type.Sliced;
        }
        enterImg.color = new Color32(0xD4, 0x7A, 0x39, 0xFF);

        Button enterBtn = enterBtnObj.AddComponent<Button>();
        enterBtn.transition = Selectable.Transition.ColorTint;
        enterBtn.targetGraphic = enterImg;
        enterBtn.colors = btnColorBlock;

        RectTransform enterRect = enterBtnObj.GetComponent<RectTransform>();
        enterRect.anchorMin = new Vector2(0.5f, 0.22f);
        enterRect.anchorMax = new Vector2(0.5f, 0.22f);
        enterRect.pivot = new Vector2(0.5f, 0.5f);
        enterRect.sizeDelta = new Vector2(180f, 54f);
        enterRect.anchoredPosition = new Vector2(115f, 0f); // Bottom-right position

        GameObject enterTextObj = new GameObject("Text (TMP)");
        enterTextObj.transform.SetParent(enterBtnObj.transform, false);
        TextMeshProUGUI enterTMP = enterTextObj.AddComponent<TextMeshProUGUI>();
        enterTMP.text = "Enter";
        enterTMP.fontSize = 22;
        enterTMP.fontStyle = FontStyles.Bold;
        enterTMP.alignment = TextAlignmentOptions.Center;
        enterTMP.color = Color.white;
        RectTransform enterTextRect = enterTextObj.GetComponent<RectTransform>();
        enterTextRect.anchorMin = Vector2.zero;
        enterTextRect.anchorMax = Vector2.one;
        enterTextRect.sizeDelta = Vector2.zero;
        enterTextRect.anchoredPosition = Vector2.zero;

        // Restart (Button - TextMeshPro: bottom-left button, orange rounded style, white text "Restart")
        GameObject restartBtnObj = new GameObject("Restart");
        restartBtnObj.transform.SetParent(frameObj.transform, false);
        Image restartImg = restartBtnObj.AddComponent<Image>();
        if (uiSprite != null)
        {
            restartImg.sprite = uiSprite;
            restartImg.type = Image.Type.Sliced;
        }
        restartImg.color = new Color32(0xD4, 0x7A, 0x39, 0xFF);

        Button restartBtn = restartBtnObj.AddComponent<Button>();
        restartBtn.transition = Selectable.Transition.ColorTint;
        restartBtn.targetGraphic = restartImg;
        restartBtn.colors = btnColorBlock;

        RectTransform restartRect = restartBtnObj.GetComponent<RectTransform>();
        restartRect.anchorMin = new Vector2(0.5f, 0.22f);
        restartRect.anchorMax = new Vector2(0.5f, 0.22f);
        restartRect.pivot = new Vector2(0.5f, 0.5f);
        restartRect.sizeDelta = new Vector2(180f, 54f);
        restartRect.anchoredPosition = new Vector2(-115f, 0f); // Bottom-left position

        GameObject restartTextObj = new GameObject("Text (TMP)");
        restartTextObj.transform.SetParent(restartBtnObj.transform, false);
        TextMeshProUGUI restartTMP = restartTextObj.AddComponent<TextMeshProUGUI>();
        restartTMP.text = "Restart";
        restartTMP.fontSize = 22;
        restartTMP.fontStyle = FontStyles.Bold;
        restartTMP.alignment = TextAlignmentOptions.Center;
        restartTMP.color = Color.white;
        RectTransform restartTextRect = restartTextObj.GetComponent<RectTransform>();
        restartTextRect.anchorMin = Vector2.zero;
        restartTextRect.anchorMax = Vector2.one;
        restartTextRect.sizeDelta = Vector2.zero;
        restartTextRect.anchoredPosition = Vector2.zero;

        // 8. Wire Inspector References on GameManager (Slide 30):
        gm.GameMenu = canvas;
        gm.Title = titleTMP;
        gm.Message = msgTMP;
        gm.ContinueButton = enterBtn;
        gm.RestartButton = restartBtn;

        // 9. Wire Inspector Reference on EnterGame (Slide 31):
        enterScript.GameMenu = canvas;

        // 10. Persistent OnClick() Events (Slide 34):
        // On "Enter" button -> On Click () references "Main Camera" -> "GameManager.NextScene"
        while (enterBtn.onClick.GetPersistentEventCount() > 0)
        {
            UnityEventTools.RemovePersistentListener(enterBtn.onClick, 0);
        }
        UnityEventTools.AddPersistentListener(enterBtn.onClick, gm.NextScene);

        // On "Restart" button -> On Click () references "Main Camera" -> "GameManager.RestartScene"
        while (restartBtn.onClick.GetPersistentEventCount() > 0)
        {
            UnityEventTools.RemovePersistentListener(restartBtn.onClick, 0);
        }
        UnityEventTools.AddPersistentListener(restartBtn.onClick, gm.RestartScene);

        // Initially disable Canvas per Slide 29 ("Disable Canvas by default")
        canvas.enabled = false;

        // Save Scene _Scene_3_2
        EditorSceneManager.SaveScene(scene, SCENE_3_2_PATH);
        Debug.Log("[Week 8] _Scene_3_2 created and saved successfully to: " + SCENE_3_2_PATH);
    }

    /// <summary>
    /// Builds _Scene_1 so GameManager.NextScene() transitions smoothly.
    /// </summary>
    private static void BuildScene1()
    {
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

        Camera mainCam = Camera.main;
        if (mainCam != null)
        {
            mainCam.backgroundColor = new Color(0.10f, 0.14f, 0.22f);
            mainCam.clearFlags = CameraClearFlags.SolidColor;
            mainCam.transform.position = new Vector3(0f, 1f, -10f);
        }

        // Canvas UI for Scene 1
        GameObject canvasObj = new GameObject("Canvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObj.AddComponent<GraphicRaycaster>();

        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;

        if (Object.FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            GameObject esObj = new GameObject("EventSystem");
            esObj.AddComponent<UnityEngine.EventSystems.EventSystem>();
            esObj.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }

        // Card Panel
        GameObject cardObj = new GameObject("Card");
        cardObj.transform.SetParent(canvasObj.transform, false);
        Image cardImg = cardObj.AddComponent<Image>();
        Sprite bgSprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Background.psd");
        if (bgSprite != null)
        {
            cardImg.sprite = bgSprite;
            cardImg.type = Image.Type.Sliced;
        }
        cardImg.color = new Color(0.12f, 0.20f, 0.35f, 0.92f);
        RectTransform cardRect = cardObj.GetComponent<RectTransform>();
        cardRect.sizeDelta = new Vector2(760f, 420f);
        cardRect.anchoredPosition = Vector2.zero;

        // Title
        GameObject titleObj = new GameObject("Title");
        titleObj.transform.SetParent(cardObj.transform, false);
        TextMeshProUGUI titleTMP = titleObj.AddComponent<TextMeshProUGUI>();
        titleTMP.text = "WELCOME TO SCENE 1";
        titleTMP.fontSize = 44;
        titleTMP.fontStyle = FontStyles.Bold;
        titleTMP.alignment = TextAlignmentOptions.Center;
        titleTMP.color = new Color(0.30f, 0.85f, 1f);
        RectTransform titleRect = titleObj.GetComponent<RectTransform>();
        titleRect.sizeDelta = new Vector2(700f, 70f);
        titleRect.anchoredPosition = new Vector2(0f, 110f);

        // Description
        GameObject descObj = new GameObject("Description");
        descObj.transform.SetParent(cardObj.transform, false);
        TextMeshProUGUI descTMP = descObj.AddComponent<TextMeshProUGUI>();
        descTMP.text = "You have successfully transitioned to the next scene\nafter clicking 'Enter' in the UI popup!";
        descTMP.fontSize = 24;
        descTMP.alignment = TextAlignmentOptions.Center;
        descTMP.color = Color.white;
        RectTransform descRect = descObj.GetComponent<RectTransform>();
        descRect.sizeDelta = new Vector2(700f, 80f);
        descRect.anchoredPosition = new Vector2(0f, 20f);

        // Return button back to _Scene_3_2
        GameObject returnBtnObj = new GameObject("ReturnButton");
        returnBtnObj.transform.SetParent(cardObj.transform, false);
        Image btnImg = returnBtnObj.AddComponent<Image>();
        Sprite uiSprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
        if (uiSprite != null)
        {
            btnImg.sprite = uiSprite;
            btnImg.type = Image.Type.Sliced;
        }
        btnImg.color = new Color(0.20f, 0.65f, 0.35f, 1f);
        Button returnBtn = returnBtnObj.AddComponent<Button>();
        RectTransform returnRect = returnBtnObj.GetComponent<RectTransform>();
        returnRect.sizeDelta = new Vector2(280f, 56f);
        returnRect.anchoredPosition = new Vector2(0f, -90f);

        GameObject btnTextObj = new GameObject("Text (TMP)");
        btnTextObj.transform.SetParent(returnBtnObj.transform, false);
        TextMeshProUGUI btnTMP = btnTextObj.AddComponent<TextMeshProUGUI>();
        btnTMP.text = "Back to UI Practice";
        btnTMP.fontSize = 20;
        btnTMP.fontStyle = FontStyles.Bold;
        btnTMP.alignment = TextAlignmentOptions.Center;
        btnTMP.color = Color.white;
        RectTransform btnTextRect = btnTextObj.GetComponent<RectTransform>();
        btnTextRect.anchorMin = Vector2.zero;
        btnTextRect.anchorMax = Vector2.one;
        btnTextRect.sizeDelta = Vector2.zero;

        SceneReturnHelper returnHelper = returnBtnObj.AddComponent<SceneReturnHelper>();
        UnityEventTools.AddPersistentListener(returnBtn.onClick, returnHelper.ReturnToPractice);

        EditorSceneManager.SaveScene(scene, SCENE_1_PATH);
        Debug.Log("[Week 8] _Scene_1 created and saved successfully to: " + SCENE_1_PATH);
    }

    /// <summary>
    /// Registers both _Scene_3_2 and _Scene_1 in File > Build Settings.
    /// </summary>
    private static void ConfigureBuildSettings()
    {
        EditorBuildSettingsScene[] scenes = new EditorBuildSettingsScene[]
        {
            new EditorBuildSettingsScene(SCENE_3_2_PATH, true),
            new EditorBuildSettingsScene(SCENE_1_PATH, true)
        };

        EditorBuildSettings.scenes = scenes;
        Debug.Log("[Week 8] Registered scenes in Build Settings: " + SCENE_3_2_PATH + ", " + SCENE_1_PATH);
    }
}
#endif
