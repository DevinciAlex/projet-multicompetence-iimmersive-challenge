# Distributeur VR

Ce module ajoute un distributeur autonome sans modifier les scènes existantes.

## Fonctionnement

- Trois boutons XR proposent un soda, un snack et un jus.
- La main qui sélectionne un bouton reçoit immédiatement le produit.
- Chaque produit possède un `Rigidbody`, un collider et un `XRGrabInteractable`.
- Si aucun interactor n'est disponible, le produit apparaît au point de secours devant la machine.

## Génération du prefab

1. Ouvrir le projet dans Unity 6000.4.2f1.
2. Attendre la fin de l'importation et de la compilation.
3. Utiliser `Rattrapage > Créer le distributeur VR`.
4. Le prefab est généré dans `Assets/VendingMachine/Prefabs/DistributeurVR.prefab`.

Cette opération crée uniquement les ressources du distributeur. Elle ne place rien dans une scène.

## Ajout ultérieur

Une fois le prefab généré, il pourra être glissé plusieurs fois dans `MainScene`. Tous les exemplaires utiliseront le même code et pourront distribuer les mêmes produits.
