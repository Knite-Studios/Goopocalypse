using Attributes;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    /// <summary>
    /// Custom property drawer for the <see cref="TitleHeaderAttribute"/>.
    /// </summary>
    [CustomPropertyDrawer(typeof(TitleHeaderAttribute))]
    public class TitleHeaderDrawer : DecoratorDrawer
    {
        public override void OnGUI(Rect position)
        {
            var header = (TitleHeaderAttribute) attribute;
        
            // Set the position for the header.
            position.y += EditorGUIUtility.singleLineHeight / 2.0f;
            position.height = EditorGUIUtility.singleLineHeight;
        
            // Set the style for the header.
            var headerStyle = new GUIStyle(EditorStyles.label)
            {
                fontSize = 13,
                fontStyle = FontStyle.Bold,
                normal = {
                    textColor = Color.white,
                    background = Texture2D.blackTexture
                },
                alignment = TextAnchor.MiddleCenter
            };
        
            // Set the background color for the header.
            var backgroundColor = new Color(0.177f, 0.177f, 0.177f, 1.0f);
            EditorGUI.DrawRect(position, backgroundColor);
            // Draw the header.
            EditorGUI.LabelField(position, header.Header, headerStyle);
        }

        public override float GetHeight() => EditorGUIUtility.singleLineHeight * 2.0f;
    }
}