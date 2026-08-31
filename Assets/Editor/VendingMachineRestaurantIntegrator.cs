#if UNITY_EDITOR
using System;
using System.IO;
using Rattrapage.VendingMachine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class VendingMachineRestaurantIntegrator
{
    private const string ScenePath = "Assets/Scenes/MainScene.unity";
    private const string PrefabPath = "Assets/VendingMachine/Prefabs/DistributeurVR.prefab";
    private const string InstanceName = "Distributeur automatique - Rattrapage";

    [MenuItem("Rattrapage/Installer le distributeur dans le restaurant")]
    public static void Install()
    {
        VendingMachinePrefabBuilder.Build();

        if (!File.Exists(ScenePath))
            throw new FileNotFoundException("La scene principale du restaurant est introuvable.", ScenePath);

        Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        RemovePreviousInstance();

        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath);
        if (prefab == null)
            throw new FileNotFoundException("Le prefab du distributeur est introuvable.", PrefabPath);

        Camera camera = FindSceneCamera();
        GameObject machine = (GameObject)PrefabUtility.InstantiatePrefab(prefab, scene);
        machine.name = InstanceName;
        PlaceInFrontOfPlayer(machine.transform, camera);

        if (camera != null && camera.GetComponent<VendingMachineDesktopInput>() == null)
            camera.gameObject.AddComponent<VendingMachineDesktopInput>();

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        AssetDatabase.SaveAssets();
        Selection.activeGameObject = machine;

        Debug.Log("Distributeur installe dans MainScene devant le joueur.");
    }

    [MenuItem("Rattrapage/Valider le distributeur dans le restaurant")]
    public static void Validate()
    {
        if (!File.Exists(ScenePath))
            throw new FileNotFoundException("La scene principale du restaurant est introuvable.", ScenePath);

        EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);

        VendingMachineController[] machines = UnityEngine.Object.FindObjectsByType<VendingMachineController>(FindObjectsSortMode.None);
        VendingProductButton[] buttons = UnityEngine.Object.FindObjectsByType<VendingProductButton>(FindObjectsSortMode.None);
        Camera camera = FindSceneCamera();

        if (machines.Length != 1)
            throw new InvalidOperationException($"Un seul distributeur est attendu dans MainScene, {machines.Length} trouves.");
        if (buttons.Length != 3)
            throw new InvalidOperationException($"Trois boutons sont attendus, {buttons.Length} trouves.");
        if (camera == null)
            throw new MissingComponentException("Aucune camera n'a ete trouvee dans MainScene.");
        if (camera.GetComponent<VendingMachineDesktopInput>() == null)
            throw new MissingComponentException("Le controle souris sans casque est absent de la camera.");

        GameObject product = machines[0].Dispense(0, null);
        if (product == null || product.GetComponent<Rigidbody>() == null)
            throw new MissingComponentException("Le produit de validation n'a pas ete cree correctement.");

        UnityEngine.Object.DestroyImmediate(product);
        Debug.Log("Validation reussie : distributeur present dans MainScene, trois boutons et test souris operationnels.");
    }

    private static void RemovePreviousInstance()
    {
        VendingMachineController[] machines = UnityEngine.Object.FindObjectsByType<VendingMachineController>(FindObjectsSortMode.None);
        foreach (VendingMachineController machine in machines)
            UnityEngine.Object.DestroyImmediate(machine.gameObject);
    }

    private static Camera FindSceneCamera()
    {
        if (Camera.main != null)
            return Camera.main;

        Camera[] cameras = UnityEngine.Object.FindObjectsByType<Camera>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        return cameras.Length > 0 ? cameras[0] : null;
    }

    private static void PlaceInFrontOfPlayer(Transform machine, Camera camera)
    {
        if (camera == null)
        {
            machine.SetPositionAndRotation(new Vector3(0f, 0f, 2f), Quaternion.identity);
            return;
        }

        Vector3 forward = Vector3.ProjectOnPlane(camera.transform.forward, Vector3.up).normalized;
        if (forward.sqrMagnitude < 0.01f)
            forward = Vector3.forward;

        Vector3 position = camera.transform.position + forward * 2.5f;
        position.y = FindFloorHeight(position, camera.transform.position.y - 1.6f);

        Vector3 backDirection = Vector3.ProjectOnPlane(position - camera.transform.position, Vector3.up).normalized;
        machine.SetPositionAndRotation(position, Quaternion.LookRotation(backDirection, Vector3.up));
    }

    private static float FindFloorHeight(Vector3 position, float fallbackHeight)
    {
        Physics.SyncTransforms();
        Vector3 origin = new Vector3(position.x, position.y + 5f, position.z);
        return Physics.Raycast(origin, Vector3.down, out RaycastHit hit, 20f)
            ? hit.point.y
            : fallbackHeight;
    }
}
#endif
