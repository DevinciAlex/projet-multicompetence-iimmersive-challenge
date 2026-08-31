using UnityEngine;
using UnityEngine.InputSystem;

namespace Rattrapage.VendingMachine
{
    /// <summary>
    /// Permet de tester le distributeur dans l'editeur sans casque VR.
    /// Un clic gauche lance un rayon depuis la camera vers le bouton vise.
    /// </summary>
    [RequireComponent(typeof(Camera))]
    public sealed class VendingMachineDesktopInput : MonoBehaviour
    {
        [SerializeField] private float maximumDistance = 20f;

        private Camera sceneCamera;

        private void Awake()
        {
            sceneCamera = GetComponent<Camera>();
        }

        private void Update()
        {
            Mouse mouse = Mouse.current;
            if (mouse == null || !mouse.leftButton.wasPressedThisFrame)
                return;

            Ray ray = sceneCamera.ScreenPointToRay(mouse.position.ReadValue());
            if (!Physics.Raycast(ray, out RaycastHit hit, maximumDistance))
                return;

            VendingProductButton button = hit.collider.GetComponentInParent<VendingProductButton>();
            if (button != null)
                button.ActivateForDesktop();
        }
    }
}
