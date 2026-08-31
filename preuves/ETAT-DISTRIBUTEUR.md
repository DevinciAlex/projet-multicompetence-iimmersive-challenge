# État du distributeur VR

Date : 31 août 2026

## Réalisé

- Copie indépendante de l'ancien projet dans le dossier du rattrapage.
- Historique Git absent de cette copie.
- Deux scripts de fonctionnement du distributeur ajoutés.
- Outil Unity de génération du prefab ajouté.
- Trois produits prévus : soda, snack et jus.
- Remise directe du produit à la main qui actionne le bouton.
- Aucune scène modifiée.
- Prefab généré dans `Assets/VendingMachine/Prefabs/DistributeurVR.prefab`.
- Scène séparée générée dans `Assets/VendingMachine/Scenes/DistributeurDemo.unity`.
- Scène ajoutée aux Build Settings sans modifier `MainScene`.
- Mode de démonstration sans casque ajouté : clic gauche sur les boutons avec la souris.
- Validation Unity automatisée réussie : contrôleur, trois boutons, caméra souris et création d'un produit vérifiés.

## Test visuel restant

1. Ouvrir `Assets/VendingMachine/Scenes/DistributeurDemo.unity`.
2. Appuyer sur Play.
3. Cliquer sur `SODA`, `SNACK` ou `JUS`.
4. Vérifier que le produit apparaît devant la machine et tombe sur le sol.

Ce test utilise uniquement la souris et ne nécessite pas de casque VR.
