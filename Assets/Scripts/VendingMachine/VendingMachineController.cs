using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace Rattrapage.VendingMachine
{
    /// <summary>
    /// Instancie le produit choisi et le place directement dans la main VR
    /// ayant actionne le bouton du distributeur.
    /// </summary>
    public sealed class VendingMachineController : MonoBehaviour
    {
        [Serializable]
        public struct Product
        {
            public string name;
            public GameObject prefab;
        }

        [SerializeField] private Product[] products;
        [SerializeField] private Transform fallbackSpawnPoint;

        public int ProductCount => products?.Length ?? 0;

        public string GetProductName(int index)
        {
            return IsValidIndex(index) ? products[index].name : "Produit inconnu";
        }

        /// <summary>
        /// Distribue un produit. Si une main VR est fournie, le produit est
        /// immediatement selectionne par cette main. Sinon il apparait devant
        /// le distributeur, ce qui facilite aussi les essais dans l'editeur.
        /// </summary>
        public GameObject Dispense(int productIndex, IXRSelectInteractor hand)
        {
            if (!IsValidIndex(productIndex) || products[productIndex].prefab == null)
            {
                Debug.LogWarning($"Produit {productIndex} non configure sur {name}.", this);
                return null;
            }

            Transform spawn = hand?.transform ?? fallbackSpawnPoint ?? transform;
            GameObject instance = Instantiate(
                products[productIndex].prefab,
                spawn.position,
                spawn.rotation);

            instance.name = products[productIndex].name;
            XRGrabInteractable grab = instance.GetComponent<XRGrabInteractable>();

            if (hand != null && grab != null)
            {
                XRInteractionManager manager = grab.interactionManager;
                if (manager == null && hand is Component handComponent)
                    manager = handComponent.GetComponentInParent<XRInteractionManager>();

                if (manager != null)
                    manager.SelectEnter(hand, grab);
                else
                    Debug.LogWarning("Aucun XRInteractionManager trouve : le produit a ete cree sans etre saisi.", instance);
            }

            return instance;
        }

        private bool IsValidIndex(int index)
        {
            return products != null && index >= 0 && index < products.Length;
        }
    }
}
