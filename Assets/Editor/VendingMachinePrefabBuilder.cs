#if UNITY_EDITOR
using System;
using System.IO;
using Rattrapage.VendingMachine;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public static class VendingMachinePrefabBuilder
{
    private const string Root = "Assets/VendingMachine";
    private const string PrefabFolder = Root + "/Prefabs";
    private const string MaterialFolder = Root + "/Materials";

    [MenuItem("Rattrapage/Creer le distributeur VR")]
    public static void Build()
    {
        EnsureFolders();

        Material blue = CreateMaterial("MachineBlue", new Color(0.02f, 0.20f, 0.42f));
        Material cyan = CreateMaterial("ScreenCyan", new Color(0.02f, 0.75f, 0.85f));
        Material dark = CreateMaterial("MachineDark", new Color(0.025f, 0.035f, 0.06f));
        Material red = CreateMaterial("ProductRed", new Color(0.85f, 0.12f, 0.16f));
        Material green = CreateMaterial("ProductGreen", new Color(0.12f, 0.72f, 0.30f));
        Material orange = CreateMaterial("ProductOrange", new Color(1f, 0.42f, 0.05f));

        GameObject soda = CreateProductPrefab("Soda", PrimitiveType.Cylinder, red, new Vector3(0.08f, 0.18f, 0.08f));
        GameObject snack = CreateProductPrefab("Snack", PrimitiveType.Cube, green, new Vector3(0.14f, 0.20f, 0.05f));
        GameObject juice = CreateProductPrefab("Jus", PrimitiveType.Cylinder, orange, new Vector3(0.07f, 0.20f, 0.07f));

        GameObject machine = new GameObject("DistributeurVR");
        BoxCollider bodyCollider = machine.AddComponent<BoxCollider>();
        bodyCollider.center = new Vector3(0f, 1f, 0f);
        bodyCollider.size = new Vector3(1.2f, 2f, 0.6f);

        VendingMachineController controller = machine.AddComponent<VendingMachineController>();

        CreatePart(machine.transform, "Corps", PrimitiveType.Cube, new Vector3(0f, 1f, 0f), new Vector3(1.2f, 2f, 0.6f), blue, false);
        CreatePart(machine.transform, "Ecran", PrimitiveType.Cube, new Vector3(0f, 1.38f, -0.315f), new Vector3(0.92f, 0.55f, 0.035f), dark, false);
        CreatePart(machine.transform, "Bandeau", PrimitiveType.Cube, new Vector3(0f, 1.82f, -0.325f), new Vector3(0.92f, 0.20f, 0.04f), cyan, false);
        CreateText(machine.transform, "DISTRIBUTEUR VR", new Vector3(0f, 1.82f, -0.355f), 0.095f);
        CreateText(machine.transform, "CHOISISSEZ UN PRODUIT", new Vector3(0f, 1.55f, -0.355f), 0.055f);

        Transform fallback = new GameObject("PointApparitionSecours").transform;
        fallback.SetParent(machine.transform, false);
        fallback.localPosition = new Vector3(0f, 0.75f, -0.65f);

        CreateButton(machine.transform, controller, 0, "SODA", new Vector3(-0.32f, 1.18f, -0.36f), red);
        CreateButton(machine.transform, controller, 1, "SNACK", new Vector3(0f, 1.18f, -0.36f), green);
        CreateButton(machine.transform, controller, 2, "JUS", new Vector3(0.32f, 1.18f, -0.36f), orange);

        SerializedObject serialized = new SerializedObject(controller);
        SerializedProperty products = serialized.FindProperty("products");
        products.arraySize = 3;
        SetProduct(products.GetArrayElementAtIndex(0), "Soda", soda);
        SetProduct(products.GetArrayElementAtIndex(1), "Snack", snack);
        SetProduct(products.GetArrayElementAtIndex(2), "Jus", juice);
        serialized.FindProperty("fallbackSpawnPoint").objectReferenceValue = fallback;
        serialized.ApplyModifiedPropertiesWithoutUndo();

        PrefabUtility.SaveAsPrefabAsset(machine, PrefabFolder + "/DistributeurVR.prefab");
        UnityEngine.Object.DestroyImmediate(machine);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Distributeur cree dans " + PrefabFolder + "/DistributeurVR.prefab");
    }

    private static void EnsureFolders()
    {
        CreateFolderIfMissing("Assets", "VendingMachine");
        CreateFolderIfMissing(Root, "Prefabs");
        CreateFolderIfMissing(Root, "Materials");
    }

    private static void CreateFolderIfMissing(string parent, string name)
    {
        string path = parent + "/" + name;
        if (!AssetDatabase.IsValidFolder(path))
            AssetDatabase.CreateFolder(parent, name);
    }

    private static Material CreateMaterial(string name, Color color)
    {
        string path = MaterialFolder + "/" + name + ".mat";
        Material material = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (material != null)
            return material;

        Shader shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
        material = new Material(shader) { name = name, color = color };
        AssetDatabase.CreateAsset(material, path);
        return material;
    }

    private static GameObject CreateProductPrefab(string name, PrimitiveType type, Material material, Vector3 scale)
    {
        GameObject product = GameObject.CreatePrimitive(type);
        product.name = name;
        product.transform.localScale = scale;
        product.GetComponent<Renderer>().sharedMaterial = material;
        product.AddComponent<Rigidbody>().mass = 0.2f;
        product.AddComponent<XRGrabInteractable>();

        string path = PrefabFolder + "/Produit" + name + ".prefab";
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(product, path);
        UnityEngine.Object.DestroyImmediate(product);
        return prefab;
    }

    private static GameObject CreatePart(Transform parent, string name, PrimitiveType type, Vector3 position, Vector3 scale, Material material, bool keepCollider)
    {
        GameObject part = GameObject.CreatePrimitive(type);
        part.name = name;
        part.transform.SetParent(parent, false);
        part.transform.localPosition = position;
        part.transform.localScale = scale;
        part.GetComponent<Renderer>().sharedMaterial = material;
        if (!keepCollider)
            UnityEngine.Object.DestroyImmediate(part.GetComponent<Collider>());
        return part;
    }

    private static void CreateButton(Transform parent, VendingMachineController controller, int index, string label, Vector3 position, Material material)
    {
        GameObject button = CreatePart(parent, "Bouton_" + label, PrimitiveType.Cube, position, new Vector3(0.22f, 0.17f, 0.08f), material, true);
        button.AddComponent<XRSimpleInteractable>();
        VendingProductButton vendingButton = button.AddComponent<VendingProductButton>();

        SerializedObject serialized = new SerializedObject(vendingButton);
        serialized.FindProperty("vendingMachine").objectReferenceValue = controller;
        serialized.FindProperty("productIndex").intValue = index;
        serialized.FindProperty("movingPart").objectReferenceValue = button.transform;
        serialized.ApplyModifiedPropertiesWithoutUndo();

        CreateText(button.transform, label, new Vector3(0f, 0f, -0.56f), 0.26f);
    }

    private static void CreateText(Transform parent, string value, Vector3 position, float size)
    {
        GameObject textObject = new GameObject("Texte_" + value);
        textObject.transform.SetParent(parent, false);
        textObject.transform.localPosition = position;
        textObject.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
        TextMeshPro text = textObject.AddComponent<TextMeshPro>();
        text.text = value;
        text.fontSize = size * 10f;
        text.alignment = TextAlignmentOptions.Center;
        text.color = Color.white;
        text.rectTransform.sizeDelta = new Vector2(4f, 1f);
    }

    private static void SetProduct(SerializedProperty property, string productName, GameObject prefab)
    {
        property.FindPropertyRelative("name").stringValue = productName;
        property.FindPropertyRelative("prefab").objectReferenceValue = prefab;
    }
}
#endif
