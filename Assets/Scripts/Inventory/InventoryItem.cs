using UnityEngine;

public class InventoryItem : ConstrainedDraggableItem
{
    public Usable LinkedItem { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        LinkedItem = (Usable)GetComponent<Lootable>().item;
    }

    protected override void Start()
    {
        errosionDistance = GameConstants.Instance.inventoryItemsErrosionDistance;
        mouseDistance = GameConstants.Instance.inventoryItemsMouseDistance;
        containerBounds = InventoryManager.Instance.TableCollider.bounds.size;
        containerCenter = InventoryManager.Instance.TableCollider.bounds.center;
        base.Start();
    }

    protected override void OnMouseEnter()
    {
        if (ScreenManager.Instance.ActiveScreen != ScreenType.Inventory)
        {
            return;
        }
        base.OnMouseEnter();
        InventoryUI.Instance.showItemDetails(LinkedItem);
    }

    protected override void OnMouseExit()
    {
        if (ScreenManager.Instance.ActiveScreen != ScreenType.Inventory)
        {
            return;
        }
        base.OnMouseExit();
        InventoryUI.Instance.hideItemDetails();
    }

    protected override void OnMouseDrag()
    {
        if (ScreenManager.Instance.ActiveScreen != ScreenType.Inventory)
        {
            return;
        }

        base.OnMouseDrag();
        InventoryUI.Instance.showItemDetails(LinkedItem);
        if (Input.GetKeyDown(KeyCode.E))
        {
            use();
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            remove();
        }
    }

    private void use()
    {
        InventoryManager.Instance.useItem(this);
        InventoryUI.Instance.hideItemDetails();
    }

    private void remove()
    {

    }
}
