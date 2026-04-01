# Génération Procédurale et Analyse de Terrain 3D sur Unity

Ce projet a été développé dans le cadre de mon TIPE (Travail d'Initiative Personnelle Encadré) sous Unity. Il s'agit d'une suite complète comprenant la génération de terrain procédural potentiellement infini, la simulation d'érosion, l'évaluation algorithmique de la jouabilité, ainsi que des outils personnalisés intégrés à l'éditeur Unity.

![tipe-clip1](https://github.com/user-attachments/assets/29814d27-35d0-4394-b96b-ad949e7cc689)

## Technologies
* **Moteur :** Unity 3D (Runtime & Editor Scripting)
* **Langage :** C#
* **Concepts clés :** Génération procédurale, Mathématiques appliquées, Algorithmique (Graphes), Programmation d'Outils.

## Fonctionnalités Principales
* **Génération par Bruit Fractal & Chunking :** Création de *heightmaps* via Bruit de Perlin et système d'instanciation dynamique divisant le monde en "chunks" (`EndlessTerrain.cs`) avec niveaux de détails (lod) dynamiques pour optimiser les performances d'affichage.
* **Érosion Thermique :** Simulation de l'éboulement naturel de la matière (`Erosion.cs`) pour lisser les falaises abruptes et donner un aspect géologique réaliste.
* **Outil d'Analyse de Jouabilité :** Algorithme mesurant à la fois le réalisme et la jouabilité du terrain pour le Level Design (calcul des pentes, détection des zones accessibles via parcours de graphe BFS) pour attribuer un "Playability Score".
* **Exploration Interactive :** Implémentation d'un contrôleur physique (FPS) custom pour explorer le monde généré infiniment.

## Guide d'utilisation

Le cœur du projet repose sur le script `MapGenerator.cs`. Pour l'essayer, ouvrez la scène principale dans Unity, sélectionnez l'objet contenant le **Map Generator** et modifiez ses paramètres directement dans l'inspecteur. 

Si la case **Auto Update** est cochée, le terrain se mettra à jour en temps réel à chaque modification !

### 1. Les modes d'affichage (`Draw Mode`)
Vous pouvez visualiser différentes étapes de l'algorithme via le menu déroulant :
* **ClassicNoise :** Affiche un bruit classique où chaque valeur est aléatoire et indépendante des autres. Non utilisé pour la génération procédurale.
* **NoiseMap / PerlinNoise :** Affiche une texture 2D en noir et blanc représentant la carte des hauteurs brute. Le mode NoiseMap combine plusieurs bruits de Perlin (PerlinNoise) de différentes fréquences et amplitudes.
* **ColorMap :** Affiche une texture 2D plate avec les couleurs des biomes assignés selon la hauteur.
* **FalloffMap :** Affiche le masque d'atténuation circulaire utilisé pour transformer le terrain infini en une île isolée.
* **Mesh :** Génère et affiche le modèle 3D final avec ses couleurs et son relief.
* **Slopes :** Affiche la carte des pentes utilisée par l'algorithme d'évaluation de la jouabilité.
* **A / U / F / B / E :** Affiche des cartes secondaires utilisées dans le calcul du score de jouabilité.
* **P Curve /  L Curve :** Affiche une courbe du score de jouabilité en fonction de la persistance (resp. lacunarité), avec la lacunarité (resp. persistance) fixée à la valeur de l'inspecteur.
* **PL Curve :** Affiche une courbe 3D du score de jouabilité en fonction de la persistance et de la lacunarité (prend généralement du temps, une dizaine de minutes sur mon pc)

### 2. Paramètres de sculpture du terrain
* **Noise Settings (Octaves, Persistance, Lacunarité) :** Modifiez ces valeurs pour contrôler le niveau de détail fractal du bruit de Perlin. La persistance controle la diminution de l'amplitude selon les octaves et la lacunarité controle l'augmentation de la fréquence. 
* **Use Falloff :** Cochez cette case pour contraindre la génération sous forme d'île.
* **Apply Erosion :** Active la simulation d'érosion thermique. Ajustez les `Iterations`, le `Talus` (angle critique) et la `Fraction` pour simuler le vieillissement de la roche.

### 3. Outils d'Éditeur Custom
En plus du générateur principal, des outils sur mesure sont disponibles dans la barre de menu supérieure d'Unity :
* **`Terrain > Object to Terrain` :** Permet de convertir n'importe quel Mesh 3D sélectionné en un objet Terrain natif d'Unity grâce à un système de Raycasting massif.
* **`Tools > Plant Placement` :** Ouvre une fenêtre permettant de générer une *Noise Map* et d'évaluer la "Fitness" du terrain pour y placer de la végétation selon la pente et la hauteur.

## Exemples de Scripts
* **Génération par Chunking (`EndlessTerrain.cs`) :** Système d'instanciation dynamique divisant le monde en "chunks" s'affichant en fonction de la distance du joueur pour optimiser les performances.
* **Génération de Mesh Custom (`MeshGenerator.cs`) :** L'algorithme convertit la *heightmap* 2D en un tableau de *Vertices* et calcule les *Triangles* via les index, tout en appliquant une courbe de hauteur (`AnimationCurve`).
* **Analyse de Jouabilité (`PlayabilityScore.cs`) :** Algorithme qui permet de quantifier la "jouabilité" du terrain généré, en mesurant le réalisme et l'accessibilité du terrain.
Il utilise notamment un parcours en largeur (BFS) pour détecter les composantes connexes "marchables", évitant ainsi que le joueur ne reste bloqué dans un cratère.

## Crédits & Attributions
* **Object2Terrain :** Le script de conversion de Mesh en Terrain (`Object2Terrain.cs`) n'a pas été développé par nos soins. Il s'agit d'un outil open-source intégré à ce projet pour optimiser la pipeline de création. 
* Les autres algorithmes (Génération, Érosion, PlayabilityScore...) sont des développements personnels.

---
*Projet développé par Vassili NAKOV et Alexandre SELY.*
