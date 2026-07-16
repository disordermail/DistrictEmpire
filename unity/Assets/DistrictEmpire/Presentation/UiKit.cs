using System;
using UnityEngine.UIElements;

namespace DistrictEmpire.Presentation
{
    public static class UiKit
    {
        public static readonly StyleColor Ink = new(new UnityEngine.Color(0.08f, 0.13f, 0.18f));
        public static readonly StyleColor Muted = new(new UnityEngine.Color(0.35f, 0.42f, 0.48f));
        public static readonly StyleColor Blue = new(new UnityEngine.Color(0.14f, 0.37f, 0.78f));
        public static readonly StyleColor Green = new(new UnityEngine.Color(0.03f, 0.53f, 0.37f));
        public static readonly StyleColor Amber = new(new UnityEngine.Color(0.71f, 0.40f, 0.04f));
        public static readonly StyleColor Panel = new(UnityEngine.Color.white);

        public static VisualElement Card(string tone = "neutral")
        {
            var card = new VisualElement();
            card.AddToClassList("de-card");
            if (tone != "neutral") card.AddToClassList($"de-card-{tone}");
            card.style.marginBottom = 10;
            card.style.paddingTop = card.style.paddingBottom = 14;
            card.style.paddingLeft = card.style.paddingRight = 14;
            card.style.borderTopLeftRadius = card.style.borderTopRightRadius = 10;
            card.style.borderBottomLeftRadius = card.style.borderBottomRightRadius = 10;
            card.style.backgroundColor = tone == "income" ? new UnityEngine.Color(0.90f, 0.97f, 0.94f) : tone == "waiting" ? new UnityEngine.Color(1f, 0.95f, 0.85f) : tone == "attention" ? new UnityEngine.Color(1f, 0.94f, 0.90f) : UnityEngine.Color.white;
            return card;
        }

        public static Label Text(string value, int size = 14, bool bold = false, StyleColor? color = null)
        {
            var label = new Label(value);
            label.AddToClassList("de-text");
            if (size >= 22) label.AddToClassList("de-text-amount");
            else if (size >= 16 && bold) label.AddToClassList("de-text-title");
            else if (size <= 12) label.AddToClassList("de-text-muted");
            label.style.fontSize = size;
            label.style.color = color ?? Ink;
            if (bold) label.style.unityFontStyleAndWeight = UnityEngine.FontStyle.Bold;
            label.style.whiteSpace = WhiteSpace.Normal;
            return label;
        }

        public static Button Button(string title, Action action, string kind = "primary")
        {
            var button = new Button(action) { text = title };
            button.AddToClassList("de-button");
            button.AddToClassList($"de-button-{kind}");
            button.style.minHeight = kind == "primary" ? 52 : 44;
            button.style.marginTop = 10;
            button.style.borderTopLeftRadius = button.style.borderTopRightRadius = 10;
            button.style.borderBottomLeftRadius = button.style.borderBottomRightRadius = 10;
            button.style.unityFontStyleAndWeight = UnityEngine.FontStyle.Bold;
            button.style.backgroundColor = kind == "income" ? Green.value : kind == "secondary" ? UnityEngine.Color.white : Blue.value;
            button.style.color = kind == "secondary" ? Blue : new StyleColor(UnityEngine.Color.white);
            return button;
        }

        public static VisualElement Row(string className = null)
        {
            var row = new VisualElement();
            row.style.flexDirection = FlexDirection.Row;
            row.style.alignItems = Align.Center;
            if (!string.IsNullOrEmpty(className)) row.AddToClassList(className);
            return row;
        }
    }
}
