using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.PostProcessing.AntialiasingModel;
using UnityEngine.UIElements;

namespace Schedule1Mod
{
    public class UltimateModMenuWrapper
    {
        private Rect menuRect;
        private bool isDragging = false;
        private Vector2 dragOffset;

        private Dictionary<string, List<IMenuItem>> tabItems = new Dictionary<string, List<IMenuItem>>();
        private List<string> tabNames = new List<string>();
        private int currentTab = 0;

        public bool IsVisible { get; set; } = true;
        public string Title { get; set; }

        public UltimateModMenuWrapper(string title, float x, float y, float width, float height)
        {
            Title = title;
            menuRect = new Rect(x, y, width, height);
        }

        public void AddTab(string tabName)
        {
            if (!tabItems.ContainsKey(tabName))
            {
                tabItems[tabName] = new List<IMenuItem>();
                tabNames.Add(tabName);
            }
        }

        private void EnsureTab(string tabName)
        {
            if (!tabItems.ContainsKey(tabName))
                AddTab(tabName);
        }

        public void AddLabel(string tab, string text) =>
            tabItems[tab].Add(new MenuLabel(text));

        public void AddButton(string tab, string label, Action onClick) =>
            tabItems[tab].Add(new MenuButton(label, onClick));

        public void AddToggle(string tab, string label, Func<bool> getter, Action<bool> setter) =>
            tabItems[tab].Add(new MenuToggle(label, getter, setter));

        public void AddSlider(string tab, string label, Func<float> getter, Action<float> setter, float min, float max) =>
            tabItems[tab].Add(new MenuSlider(label, getter, setter, min, max));

        public void AddTextField(string tab, string label, Func<string> getter, Action<string> setter) =>
            tabItems[tab].Add(new MenuTextField(label, getter, setter));

        public void AddDropdown(string tab, string label, string[] options, Func<int> getter, Action<int> setter) =>
            tabItems[tab].Add(new MenuDropdown(label, options, getter, setter));

        public void AddColorPicker(string tab, string label, Func<Color> getter, Action<Color> setter) =>
            tabItems[tab].Add(new MenuColorPicker(label, getter, setter));

        public void AddKeybind(string tab, string label, Func<KeyCode> getter, Action<KeyCode> setter) =>
            tabItems[tab].Add(new MenuKeybind(label, getter, setter));

        public void AddSeparator(string tab) =>
            tabItems[tab].Add(new MenuSeparator());


        public class Ref<T>
        {
            public T Value;
            public Ref(T value) => Value = value;
        }

        // 🚩 Surcharge ultra simplifiée → sans label(bouton = nom de la méthode)
        public void AddButton(string tab, Action onClick)
        {
            string methodName = onClick.Method.Name;
            AddButton(tab, methodName, onClick);
        }

        public void AddToggle(string tab, string label, Ref<bool> variable)
        {
            AddToggle(tab, label, () => variable.Value, v => variable.Value = v);
        }

        public void AddSlider(string tab, string label, Ref<float> variable, float min, float max)
        {
            AddSlider(tab, label, () => variable.Value, v => variable.Value = v, min, max);
        }

        public void AddDropdown(string tab, string label, string[] options, Ref<int> variable)
        {
            AddDropdown(tab, label, options, () => variable.Value, v => variable.Value = v);
        }

        public void AddColorPicker(string tab, string label, Ref<Color> variable)
        {
            AddColorPicker(tab, label, () => variable.Value, v => variable.Value = v);
        }


        public void Draw()
        {
            if (!IsVisible) return;

            // 🧠 Ne pas bloquer ici : laisse le menu recevoir les events d'abord (comme le drag ou les boutons)

            // Drag
            var dragArea = new Rect(menuRect.x, menuRect.y, menuRect.width, 20);

            if (Event.current.type == EventType.MouseDown && dragArea.Contains(Event.current.mousePosition))
            {
                isDragging = true;
                dragOffset = Event.current.mousePosition - new Vector2(menuRect.x, menuRect.y);
            }
            else if (Event.current.type == EventType.MouseUp)
            {
                isDragging = false;
            }
            if (isDragging)
            {
                var mousePos = Event.current.mousePosition;
                menuRect.x = mousePos.x - dragOffset.x;
                menuRect.y = mousePos.y - dragOffset.y;
            }

            // Box
            GUI.Box(menuRect, Title);

            // Tabs
            float tabBtnX = menuRect.x + 10;
            for (int i = 0; i < tabNames.Count; i++)
            {
                if (GUI.Button(new Rect(tabBtnX, menuRect.y + 25, 70, 25), tabNames[i]))
                {
                    currentTab = i;
                }
                tabBtnX += 75;
            }

            // Items
            if (tabNames.Count > 0)
            {
                string activeTab = tabNames[currentTab];
                float y = menuRect.y + 60;
                foreach (var item in tabItems[activeTab])
                {
                    y = item.Draw(menuRect.x + 10, y, menuRect.width - 20);
                }
            }

            // ✅ Après avoir traité tout le menu : bloquer la propagation si souris dessus
            if (menuRect.Contains(Event.current.mousePosition))
            {
                if (Event.current.type == EventType.MouseDown ||
                    Event.current.type == EventType.MouseDrag ||
                    Event.current.type == EventType.MouseUp)
                {
                    Event.current.Use();
                }
            }
        }


        // Interfaces & Items
        private interface IMenuItem
        {
            float Draw(float x, float y, float width);
        }

        private class MenuLabel : IMenuItem
        {
            private string text;
            public MenuLabel(string text) => this.text = text;
            public float Draw(float x, float y, float width)
            {
                GUI.Label(new Rect(x, y, width, 20), text);
                return y + 25;
            }
        }

        private class MenuButton : IMenuItem
        {
            private string label;
            private Action onClick;
            public MenuButton(string label, Action onClick)
            {
                this.label = label; this.onClick = onClick;
            }
            public float Draw(float x, float y, float width)
            {
                if (GUI.Button(new Rect(x, y, width, 30), label)) onClick?.Invoke();
                return y + 40;
            }
        }

        private class MenuToggle : IMenuItem
        {
            private string label;
            private Func<bool> getter;
            private Action<bool> setter;
            public MenuToggle(string label, Func<bool> getter, Action<bool> setter)
            {
                this.label = label; this.getter = getter; this.setter = setter;
            }
            public float Draw(float x, float y, float width)
            {
                bool val = getter();
                bool newVal = GUI.Toggle(new Rect(x, y, width, 20), val, label);
                if (newVal != val) setter(newVal);
                return y + 25;
            }
        }

        private class MenuSlider : IMenuItem
        {
            private string label;
            private Func<float> getter;
            private Action<float> setter;
            private float min, max;
            public MenuSlider(string label, Func<float> getter, Action<float> setter, float min, float max)
            {
                this.label = label; this.getter = getter; this.setter = setter; this.min = min; this.max = max;
            }
            public float Draw(float x, float y, float width)
            {
                GUI.Label(new Rect(x, y, width, 20), $"{label}: {getter():0.00}");
                float newVal = GUI.HorizontalSlider(new Rect(x, y + 20, width, 20), getter(), min, max);
                setter(newVal);
                return y + 50;
            }
        }

        private class MenuTextField : IMenuItem
        {
            private string label;
            private Func<string> getter;
            private Action<string> setter;
            public MenuTextField(string label, Func<string> getter, Action<string> setter)
            {
                this.label = label; this.getter = getter; this.setter = setter;
            }
            public float Draw(float x, float y, float width)
            {
                GUI.Label(new Rect(x, y, width, 20), label);
                string newVal = GUI.TextField(new Rect(x, y + 20, width, 25), getter());
                if (newVal != getter()) setter(newVal);
                return y + 55;
            }
        }

        private class MenuDropdown : IMenuItem
        {
            private string label;
            private string[] options;
            private Func<int> getter;
            private Action<int> setter;
            private bool open = false;

            public MenuDropdown(string label, string[] options, Func<int> getter, Action<int> setter)
            {
                this.label = label; this.options = options; this.getter = getter; this.setter = setter;
            }

            public float Draw(float x, float y, float width)
            {
                GUI.Label(new Rect(x, y, width, 20), $"{label}: {options[getter()]}");
                if (GUI.Button(new Rect(x, y + 20, width, 25), "Changer"))
                {
                    open = !open;
                }
                if (open)
                {
                    for (int i = 0; i < options.Length; i++)
                    {
                        if (GUI.Button(new Rect(x, y + 50 + i * 25, width, 25), options[i]))
                        {
                            setter(i);
                            open = false;
                        }
                    }
                    return y + 50 + options.Length * 25;
                }
                return y + 55;
            }
        }

        private class MenuColorPicker : IMenuItem
        {
            private string label;
            private Func<Color> getter;
            private Action<Color> setter;
            public MenuColorPicker(string label, Func<Color> getter, Action<Color> setter)
            {
                this.label = label; this.getter = getter; this.setter = setter;
            }
            public float Draw(float x, float y, float width)
            {
                Color col = getter();
                GUI.Label(new Rect(x, y, width, 20), $"{label}: R:{(int)(col.r * 255)} G:{(int)(col.g * 255)} B:{(int)(col.b * 255)}");
                float r = GUI.HorizontalSlider(new Rect(x, y + 20, width, 20), col.r, 0f, 1f);
                float g = GUI.HorizontalSlider(new Rect(x, y + 45, width, 20), col.g, 0f, 1f);
                float b = GUI.HorizontalSlider(new Rect(x, y + 70, width, 20), col.b, 0f, 1f);
                setter(new Color(r, g, b));
                return y + 95;
            }
        }

        private class MenuKeybind : IMenuItem
        {
            private string label;
            private Func<KeyCode> getter;
            private Action<KeyCode> setter;
            private bool waiting = false;

            public MenuKeybind(string label, Func<KeyCode> getter, Action<KeyCode> setter)
            {
                this.label = label; this.getter = getter; this.setter = setter;
            }

            public float Draw(float x, float y, float width)
            {
                if (waiting && Event.current.isKey)
                {
                    setter(Event.current.keyCode);
                    waiting = false;
                }

                string display = waiting ? "[Press key...]" : getter().ToString();
                if (GUI.Button(new Rect(x, y, width, 25), $"{label}: {display}"))
                {
                    waiting = true;
                }

                return y + 30;
            }
        }

        private class MenuSeparator : IMenuItem
        {
            public float Draw(float x, float y, float width)
            {
                GUI.Box(new Rect(x, y + 10, width, 2), "");
                return y + 15;
            }
        }
    }
}
