using Djinde.UI;

namespace Djinde.Quest
{
    public class SelectableList_Loot : SelectableList
    {
        #region Events

        public delegate void LootsFullyRevealedEvent();
        public event LootsFullyRevealedEvent OnLootsFullyRevealedEvent;

        #endregion

        #region Attributes

        private int _lootsRevealedCount = 0;

        #endregion

        #region Public Methods

        public override void ClearList()
        {
            base.ClearList();
            _lootsRevealedCount = 0;
        }

        public override void OnItemSelected(SelectableListItem item)
        {
            base.OnItemSelected(item);
            _lootsRevealedCount++;
            if (_lootsRevealedCount == _listContent.Count)
            {
                OnLootsFullyRevealedEvent?.Invoke();
            }
        }

        #endregion
    }
}