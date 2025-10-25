using System;
using UnityEngine;

namespace Unity3D
{
    [Obsolete]
    [CreateAssetMenu(menuName ="Game/Turn Panel Settings")]
    public class TurnPanelSettings : ScriptableObject
    {
        [System.Serializable]
        public class PlayerSettings
        {
            public Team Team;
            public string DisplayName = "Player";
            public Color TextColor = Color.white;
            public int FontSize = 36;
            public Sprite Icon => _createTextSprite();
            private Sprite _createTextSprite()
            {
                Texture2D texture = new Texture2D(1, 1);
                texture.SetPixel(0, 0, Color.clear);
                texture.Apply();
                return Sprite.Create(texture, new Rect(0, 0, 1, 1), Vector2.zero);
            }
        }
        public PlayerSettings Player1Settings;
        public PlayerSettings Player2Settings;
        public PlayerSettings this[Team team] => team == Team.Player1 ? Player1Settings : Player2Settings;
    }
}
