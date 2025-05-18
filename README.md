

[![Inspired by UnityPCModMenu](https://img.shields.io/badge/Inspired%20by-Francois284Modz%2FUnityPCModMenu-blue?logo=unity)](https://github.com/Francois284Modz/UnityPCModMenu)

> Ce projet rend hommage et s'inspire de [UnityPCModMenu](https://github.com/Francois284Modz/UnityPCModMenu), en l'adaptant aux jeux Unity IL2CPP récents, de plus en plus nombreux.

# 🎮 UltimateModMenuWrapper
...


Un **wrapper ultra-complet pour créer facilement des menus GUI** dans vos mods MelonLoader Unity.

✅ Gère :
- Menu draggable
- Onglets (tabs)
- Labels
- Boutons
- Toggles (booléens)
- Sliders (float)
- Dropdowns (int)
- Color Pickers
- Keybinds
- Séparateurs
- Gestion simplifiée via `Ref<T>`

✨ Conçu pour être **facile à utiliser et rapide à intégrer** dans vos mods.

---

## 🚀 **Installation**

1. Copiez `UltimateModMenuWrapper.cs` dans votre projet MelonLoader
2. Dans votre classe `MelonMod`, créez une instance et ajoutez vos éléments
3. Affichez le menu dans `OnGUI()`

---

## 📝 **Utilisation**

### ✅ **Exemple minimal complet**

```csharp
// Déclaration des variables via Ref<T>
Ref<bool> godMode = new Ref<bool>(false);
Ref<float> speed = new Ref<float>(5f);
Ref<int> classe = new Ref<int>(0);
Ref<Color> playerColor = new Ref<Color>(Color.white);
KeyCode cheatKey = KeyCode.F5;

// Création du menu
UltimateModMenuWrapper menu = new UltimateModMenuWrapper("Ultimate Menu", 100, 100, 350, 500);

// Ajout des onglets
menu.AddTab("Main");
menu.AddTab("Settings");

// ➡️ Onglet Main
menu.AddLabel("Main", "Bienvenue sur le Mod Menu !");
menu.AddToggle("Main", "GodMode", godMode);
menu.AddSlider("Main", "Speed", speed, 1f, 20f);
menu.AddDropdown("Main", "Classe", new[] { "Warrior", "Mage", "Rogue" }, classe);
menu.AddColorPicker("Main", "Couleur", playerColor);
menu.AddKeybind("Main", "Bind Cheat", () => cheatKey, (v) => cheatKey = v);
menu.AddButton("Main", () => ResetStats());

// ➡️ Onglet Settings
menu.AddLabel("Settings", "Paramètres avancés");
menu.AddToggle("Settings", "Debug Mode", new Ref<bool>(false)); // variable temporaire inline
menu.AddSeparator("Settings");
menu.AddButton("Settings", "Afficher Position", () => MelonLogger.Msg("Pos: " + Camera.main.transform.position));

// Toggle visibilité (exemple F1)
public override void OnUpdate()
{
    if (Input.GetKeyDown(KeyCode.F1))
        menu.IsVisible = !menu.IsVisible;
}

// Dessiner le menu
public override void OnGUI()
{
    menu.Draw();
}
```

---

## ✨ **Pourquoi `Ref<T>` ?**

C# ne permet **PAS d'utiliser `ref` directement dans une lambda**.

Le wrapper utilise **une classe `Ref<T>`** pour stocker une référence modifiable et la passer facilement dans les getter/setter.

```csharp
// Exemple
Ref<bool> godMode = new Ref<bool>(false);

menu.AddToggle("Main", "GodMode", godMode);

if (godMode.Value) { /* GodMode activé */ }
```

---

## 🧩 **Liste des méthodes**

| Méthode                              | Description                                       |
|-------------------------------------|--------------------------------------------------|
| `AddTab(string tab)`                 | Ajoute un onglet                                 |
| `AddLabel(string tab, string text)`  | Ajoute un label                                  |
| `AddButton(string tab, Action)`      | Bouton (nom automatique = nom méthode)           |
| `AddButton(string tab, string label, Action)` | Bouton avec label personnalisé       |
| `AddToggle(string tab, string label, Ref<bool>)` | Toggle booléen lié à variable       |
| `AddSlider(string tab, string label, Ref<float>, float min, float max)` | Slider float   |
| `AddDropdown(string tab, string label, string[] options, Ref<int>)` | Dropdown index |
| `AddColorPicker(string tab, string label, Ref<Color>)` | Color Picker    |
| `AddKeybind(string tab, string label, Func<KeyCode>, Action<KeyCode>)` | Keybind        |
| `AddSeparator(string tab)`           | Ligne de séparation                             |

---

## 🏆 **Exemple complet de variable avec Ref<T>**

```csharp
Ref<bool> debugMode = new Ref<bool>(true);
menu.AddToggle("Settings", "Mode Debug", debugMode);

if (debugMode.Value)
{
    // faire des logs
}
```

---

## 🤝 **Contributions**

N’hésitez pas à adapter, forker ou améliorer le wrapper !

Merci d’utiliser UltimateModMenuWrapper 🎉
