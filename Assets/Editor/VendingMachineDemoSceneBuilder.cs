#if UNITY_EDITOR
using System.IO;
using System.Linq;
using Rattrapage.VendingMachine;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class VendingMachineDemoSceneBuilder
{
    private const string SceneFolder = "Assets/VendingMachine/Scenes";
    private const string ScenePath = SceneFolder + "/DistributeurDemo.unity";
    private const string PrefabPath = "Assets/VendingMachine/Prefabs/DistributeurVR.prefab";
    private const string FloorMaterialPath = "Assets/VendingMachine/Materials/DemoFloor.mat";

    [InitializeOnLoadMethod]
    private static void ScheduleInitialBuild()
    {
        if (!File.Exists(ScenePath))
            EditorApplication.delayCall += BuildWhenEditorIsReady;
    }

    private static void BuildWhenEditorIsReady()
    {
        if (EditorApplication.isCompiling || EditorApplication.isUpdating)
        {
            EditorApplication.delayCall += BuildWhenEditorIsReady;
            return;
        }

        if (!File.Exists(ScenePath))
            Build();
    }

    [MenuItem("Rattrapage/Creer la scene de demonstration")]
    public static void Build()
    {
        VendingMachinePrefabBuilder.Build();
        EnsureSceneFolder();

        Scene previousScene = SceneManager.GetActiveScene();
        bool hasSavedPreviousScene = previousScene.IsValid() && !string.IsNullOrEmpty(previousScene.path);
        if (!hasSavedPreviousScene && previousScene.IsValid() && previousScene.isDirty)
        {
            Debug.LogWarning("Enregistrez ou fermez la scene actuelle avant de creer la demonstration.");
            return;
        }

        NewSceneMode mode = hasSavedPreviousScene ? NewSceneMode.Additive : NewSceneMode.Single;
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, mode);
        SceneManager.SetActiveScene(scene);

        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath);
        GameObject machine = (GameObject)PrefabUtility.InstantiatePrefab(prefab, scene);
        machine.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);

        CreateFloor();
        CreateLighting();
        CreateCamera();
        CreateInstructions();
        new GameObject("GameManager").AddComponent<GameManager>();

        EditorSceneManager.SaveScene(scene, ScenePath);
        if (hasSavedPreviousScene)
        {
            EditorSceneManager.CloseScene(scene, true);
            SceneManager.SetActiveScene(previousScene);
        }
        AddSceneToBuildSettings();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Selection.activeGameObject = machine;
        Debug.Log("Scene de demonstration creee dans " + ScenePath);
    }

    [MenuItem("Rattrapage/Valider la scene de demonstration")]
    public static void Validate()
    {
        if (!File.Exists(ScenePath))
            throw new FileNotFoundException("La scene de demonstration n'existe pas.", ScenePath);

        EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);

        VendingMachineController controller = Object.FindFirstObjectByType<VendingMachineController>();
        VendingProductButton[] buttons = Object.FindObjectsByType<VendingProductButton>(FindObjectsSortMode.None);
        VendingMachineDesktopInput desktopInput = Object.FindFirstObjectByType<VendingMachineDesktopInput>();

        if (controller == null)
            throw new MissingComponentException("VendingMachineController absent de la scene.");
        if (buttons.Length != 3)
            throw new MissingComponentException($"Trois boutons sont attendus, {buttons.Length} trouves.");
        if (desktopInput == null)
            throw new MissingComponentException("Le controle souris est absent de la camera.");

        GameObject product = controller.Dispense(0, null);
        if (product == null || product.GetComponent<Rigidbody>() == null)
            throw new MissingComponentException("Le produit de test n'a pas ete cree correctement.");

        Object.DestroyImmediate(product);
        Debug.Log("Validation reussie : distributeur, 3 boutons, controle souris et produit de test presents.");
    }

    private static void EnsureSceneFolder()
    {
        if (!AssetDatabase.IsValidFolder("Assets/VendingMachine"))
            AssetDatabase.CreateFolder("Assets", "VendingMachine");
        if (!AssetDatabase.IsValidFolder(SceneFolder))
            AssetDatabase.CreateFolder("Assets/VendingMachine", "Scenes");
    }

    private static void CreateFloor()
    {
        GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
        floor.name = "Sol";
        floor.transform.position = Vector3.zero;
        floor.transform.localScale = new Vector3(1.2f, 1f, 1.2f);

        floor.GetComponent<Renderer>().sharedMaterial = GetFloorMaterial();
    }

    private static Material GetFloorMaterial()
    {
        Material material = AssetDatabase.LoadAssetAtPath<Material>(FloorMaterialPath);
        if (material != null)
            return material;

        Shader shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
        material = new Material(shader)
        {
            name = "DemoFloor",
            color = new Color(0.08f, 0.10f, 0.14f)
        };
        AssetDatabase.CreateAsset(material, FloorMaterialPath);
        return material;
    }

    private static void CreateLighting()
    {
        GameObject lightObject = new GameObject("Lumiere principale");
        Light light = lightObject.AddComponent<Light>();
        light.type = LightType.Directional;
        light.intensity = 1.2f;
        lightObject.transform.rotation = Quaternion.Euler(45f, -35f, 0f);

        RenderSettings.ambientLight = new Color(0.28f, 0.32f, 0.40f);
    }

    private static void CreateCamera()
    {
        GameObject cameraObject = new GameObject("Camera de test");
        Camera camera = cameraObject.AddComponent<Camera>();
        cameraObject.tag = "MainCamera";
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = new Color(0.015f, 0.025f, 0.055f);
        cameraObject.transform.position = new Vector3(0f, 1.45f, -4f);
        cameraObject.transform.rotation = Quaternion.LookRotation(new Vector3(0f, 1.1f, 0f) - cameraObject.transform.position);
        cameraObject.AddComponent<AudioListener>();
        cameraObject.AddComponent<VendingMachineDesktopInput>();
    }

    private static void CreateInstructions()
    {
        GameObject textObject = new GameObject("Instructions");
        textObject.transform.position = new Vector3(0f, 2.45f, -0.15f);
        textObject.transform.rotation = Quaternion.Euler(0f, 180f, 0f);

        TextMeshPro text = textObject.AddComponent<TextMeshPro>();
        text.text = "TEST SANS CASQUE\nCliquez sur SODA, SNACK ou JUS";
        text.fontSize = 3.5f;
        text.alignment = TextAlignmentOptions.Center;
        text.color = new Color(0.65f, 0.95f, 1f);
        text.rectTransform.sizeDelta = new Vector2(5.5f, 1.2f);
    }

    private static void AddSceneToBuildSettings()
    {
        EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;
        if (scenes.Any(item => item.path == ScenePath))
            return;

        EditorBuildSettings.scenes = scenes
            .Concat(new[] { new EditorBuildSettingsScene(ScenePath, true) })
            .ToArray();
    }
}
#endif
