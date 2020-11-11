using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;
using Djinde.UI;

namespace Djinde.Quest
{
    public class SelectableListItem_Loot : SelectableListItem
    {
        #region Exposed Attributes

        [BoxGroup("SelectableListItem_Loot")]
        [Header("References")]
        [SerializeField]
        private Outline _outline;

        #endregion

        #region Attributes

        private bool _revealed = false;

        #endregion

        #region Public Methods

        public override void SetData(SelectableListItemData data)
        {
            _data = data;
            _label.text = "?";
        }

        public override void OnSelect()
        {
            if (!_revealed)
            {
                _revealed = true;
                _outline.effectColor = ((SelectableListItem_LootData)_Data)._outlineColor;
                _label.text = _Data._label;
                base.OnSelect();
            }
        }

        #endregion
    }

    public class SelectableListItem_LootData : SelectableListItemData
    {
        #region Attributes

        public Color _outlineColor;

        #endregion
    }
}