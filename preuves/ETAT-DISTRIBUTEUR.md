# État du distributeur VR

Date : 31 août 2026

## Réalisé

- Reconstruction depuis une copie fraîche du dépôt d'origine `IlanDeVinci/UnityARVR` au commit `cb08944`.
- Le projet source OneDrive et ses modifications locales sont restés intacts.
- Scripts de fonctionnement du distributeur ajoutés.
- Outil Unity de génération du prefab ajouté.
- Trois produits prévus : soda, snack et jus.
- Remise directe du produit à la main qui actionne le bouton.
- Prefab généré dans `Assets/VendingMachine/Prefabs/DistributeurVR.prefab`.
- Scène séparée générée dans `Assets/VendingMachine/Scenes/DistributeurDemo.unity`.
- Distributeur intégré directement dans le restaurant de `Assets/Scenes/MainScene.unity`.
- Placement automatique devant la caméra du joueur et orientation vers celui-ci.
- Mode de démonstration sans casque ajouté : clic gauche sur les boutons avec la souris.
- Validation Unity automatisée réussie : contrôleur, trois boutons, caméra souris et création d'un produit vérifiés.
- Validation spécifique de `MainScene` réussie avec un code retour Unity `0`.

## Test visuel restant

1. Ouvrir `Assets/Scenes/MainScene.unity`.
2. Appuyer sur Play puis sur **JOUER** dans le menu du restaurant.
3. Repérer le distributeur placé devant le joueur.
4. Cliquer sur `SODA`, `SNACK` ou `JUS`.
5. Vérifier que le produit apparaît devant la machine et tombe sur le sol.

La scène indépendante `Assets/VendingMachine/Scenes/DistributeurDemo.unity` reste disponible comme test de secours plus simple.

Ce test utilise uniquement la souris et ne nécessite pas de casque VR.
