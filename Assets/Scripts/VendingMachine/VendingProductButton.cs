using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace Rattrapage.VendingMachine
{
    /// <summary>
    /// Relie un bouton physique XR a un produit du distributeur.
    /// </summary>
    [RequireComponent(typeof(XRSimpleInteractable))]
    public sealed class VendingProductButton : MonoBehaviour
    {
        [SerializeField] private VendingMachineController vendingMachine;
        [SerializeField, Min(0)] private int productIndex;
        [SerializeField] private Transform movingPart;
        [SerializeField, Min(0f)] private float pressedDepth = 0.015f;

        private XRSimpleInteractable interactable;
        private Vector3 initialLocalPosition;

        private void Awake()
        {
            interactable = GetComponent<XRSimpleInteractable>();
            if (movingPart == null)
                movingPart = transform;

            initialLocalPosition = movingPart.localPosition;
        }

        private void OnEnable()
        {
            if (interactable == null)
                interactable = GetComponent<XRSimpleInteractable>();

            interactable.selectEntered.AddListener(OnSelected);
            interactable.selectExited.AddListener(OnReleased);
        }

        private void OnDisable()
        {
            if (interactable == null)
                return;

            interactable.selectEntered.RemoveListener(OnSelected);
            interactable.selectExited.RemoveListener(OnReleased);
        }

        private void OnSelected(SelectEnterEventArgs args)
        {
            movingPart.localPosition = initialLocalPosition + Vector3.back * pressedDepth;

            if (vendingMachine == null)
            {
                Debug.LogWarning("Ce bouton n'est relie a aucun distributeur.", this);
                return;
            }

            // Le bouton cede immediatement la selection au produit cree.
            args.manager.SelectExit(args.interactorObject, args.interactableObject);
            vendingMachine.Dispense(productIndex, args.interactorObject);
        }

        private void OnReleased(SelectExitEventArgs args)
        {
            movingPart.localPosition = initialLocalPosition;
        }

        /// <summary>
        /// Active le bouton sans interactor XR. Cette methode est utilisee
        /// par la scene de demonstration clavier/souris.
        /// </summary>
        public void ActivateForDesktop()
        {
            if (movingPart == null)
                movingPart = transform;

            movingPart.localPosition = initialLocalPosition + Vector3.back * pressedDepth;

            if (vendingMachine == null)
            {
                Debug.LogWarning("Ce bouton n'est relie a aucun distributeur.", this);
                return;
            }

            vendingMachine.Dispense(productIndex, null);
            CancelInvoke(nameof(ResetButton));
            Invoke(nameof(ResetButton), 0.15f);
        }

        private void ResetButton()
        {
            if (movingPart != null)
                movingPart.localPosition = initialLocalPosition;
        }
    }
}
