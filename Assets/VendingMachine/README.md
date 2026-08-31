# Distributeur VR

Ce module ajoute un distributeur autonome. Le prefab reste réutilisable et un exemplaire est installé dans la scène du restaurant `Assets/Scenes/MainScene.unity`.

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

## Scène de démonstration sans casque

1. Attendre la fin de la compilation dans Unity.
2. Utiliser `Rattrapage > Créer la scène de démonstration`.
3. Ouvrir `Assets/VendingMachine/Scenes/DistributeurDemo.unity` si Unity ne l'affiche pas automatiquement.
4. Appuyer sur Play.
5. Cliquer avec la souris sur `SODA`, `SNACK` ou `JUS`.

Le produit apparaît devant la machine et tombe sur le sol. Cette scène séparée permet de démontrer le distributeur sans casque indépendamment du restaurant.

## Installation dans le restaurant

1. Utiliser `Rattrapage > Installer le distributeur dans le restaurant`.
2. Le script ouvre `MainScene`, place une seule machine devant la caméra du joueur et l'oriente vers lui.
3. Le contrôle souris est ajouté à la caméra pour permettre le test sans casque.
4. Utiliser `Rattrapage > Valider le distributeur dans le restaurant` pour vérifier automatiquement la machine, les trois boutons et la distribution d'un produit.

Une nouvelle installation remplace uniquement l'ancien exemplaire du distributeur et évite ainsi les doublons.
