using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SelectableListItem : MonoBehaviour
{
    #region Exposed Attributes

    [Header("References")]
    [SerializeField]
    private Image _background;
    [SerializeField]
    protected TMP_Text _label;
    [Header("Parameters")]
    [SerializeField]
    private Color _selectedColor;
    [SerializeField]
    private Color _idleColor;
    [Header("Data")]
    [SerializeField]
    protected SelectableListItemData _data;

    #endregion

    #region Attributes

    public SelectableListItemData _Data
    {
        get
        {
            return _data;
        }
    }
    private SelectableList _parentList;
    private bool _isSelected = false;

    #endregion

    #region MonoBehaviour Methods

    private void Awake()
    {
        Unselect();
    }

    #endregion

    #region Public Methods

    public void SetParentList(SelectableList parentList)
    {
        _parentList = parentList;
    }

    public virtual void SetData(SelectableListItemData data)
    {
        _data = data;
        _label.text = data._label;
    }

    public virtual void OnSelect()
    {
        if (!_isSelected)
        {
            _parentList.OnItemSelected(this);
            _isSelected = true;
            _background.color = _selectedColor;
        }
    }

    public void Unselect()
    {
        _background.color = _idleColor;
        _isSelected = false;
    }

    #endregion
}

[System.Serializable]
public class SelectableListItemData
{
    #region Attributes

    public string _label;

    #endregion
}
