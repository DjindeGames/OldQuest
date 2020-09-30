using System.Collections.Generic;
using UnityEngine;

public class SelectableList : MonoBehaviour
{
    #region Events

    public delegate void ItemSelectedEvent(SelectableListItem listItem);
    public event ItemSelectedEvent OnItemSelectedEvent;

    #endregion

    #region Exposed Attributes

    [Header("References")]
    [SerializeField]
    private Transform _itemsContainer;
    [SerializeField]
    private SelectableListItem _listItemPrefab;
    [Header("Parameters")]
    [SerializeField]
    private ESFXType _onItemSelectedSFX;

    #endregion

    #region Attributes

    public int ItemCount
    {
        get
        {
            return _listContent.Count;
        }
    }

    private SelectableListItem _currentSelectedItem;
    private List<SelectableListItem> _listContent = new List<SelectableListItem>();

    #endregion

    #region MonoBehaviour Methods

    private void Awake()
    {
        InitList();
    }

    #endregion

    #region Private Methods

    private void InitList()
    {
        //Fill list with container's content
        for (int i = 0; i < _itemsContainer.childCount; i++)
        {
            SelectableListItem item = _itemsContainer.GetChild(i).GetComponent<SelectableListItem>();
            if (item != null)
            {
                item.SetParentList(this);
                _listContent.Add(item);
            }
        }
    }

    #endregion

    #region Public Methods

    public SelectableListItem AddItem(SelectableListItemData data)
    {
        SelectableListItem listItem = null;
        GameObject addedItem = Instantiate(_listItemPrefab.gameObject, _itemsContainer);
        addedItem.transform.SetAsFirstSibling();
        listItem = addedItem.GetComponent<SelectableListItem>();
        listItem.SetData(data);
        listItem.SetParentList(this);
        _listContent.Add(listItem);
        return listItem;
    }

    public void RemoveItem(SelectableListItem item)
    {
        _listContent.Remove(item);
        Destroy(item.gameObject);
    }

    public SelectableListItem GetSelectedItem()
    {
        return _currentSelectedItem;
    }

    public SelectableListItem GetItemAfter(SelectableListItem item)
    {
        int index = _listContent.IndexOf(item);
        return (index < ItemCount - 1) ? _listContent[index + 1] : null;
    }

    public SelectableListItem GetItemBefore(SelectableListItem item)
    {
        int index = _listContent.IndexOf(item);
        return (index > 0) ? _listContent[index - 1] : null;
    }

    public void ClearList()
    {
        for(int i = 0; i < _listContent.Count; i++)
        {
            Destroy(_itemsContainer.GetChild(i).gameObject);
        }
        _listContent.Clear();
    }

    public void OnItemSelected(SelectableListItem item)
    {
        SoundManager.Instance.playSFX(_onItemSelectedSFX);
        if (_currentSelectedItem != null)
        {
            _currentSelectedItem.Unselect();
        }
        _currentSelectedItem = item;
        OnItemSelectedEvent?.Invoke(_currentSelectedItem);
    }

    #endregion
}
