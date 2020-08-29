using UnityEngine;

public abstract class Highlightable : Interactable
{
    [Header("Settings")]
    [SerializeField]
    private LootableOutlineType outlineType;

    private Material[] baseMaterials;
    private Material[] outlinedMaterials;
    private Renderer renderer;

    bool outline = true;

    protected bool OutlineEnabled
    {
        get
        {
            return outline;
        }
        set
        {
            outline = value;
            if (!outline)
            {
                toggleOutline(false);
            }
        }
    }

    protected virtual void Awake()
    {
        renderer = GetComponent<Renderer>();
        baseMaterials = renderer.materials;
    }

    protected virtual void Start()
    {
        initializeOutlinedMaterials();
    }

    protected override void OnMouseOver()
    {
        base.OnMouseOver();
    }

    protected override void mouseEntered()
    {
        if (!OutlineEnabled)
        {
            return;
        }
        toggleOutline(true);
    }

    protected override void mouseExited()
    {
        if (!OutlineEnabled)
        {
            return;
        }
        toggleOutline(false);
    }

    protected void toggleOutline(bool on)
    {
        if (on)
        {
            renderer.materials = outlinedMaterials;
        }
        else
        {
            renderer.materials = baseMaterials;
        }
    }

    private void initializeOutlinedMaterials()
    {
        outlinedMaterials = new Material[baseMaterials.Length];
        Material baseOutline = GameConstants.Instance.getLootableOutlineMaterialByType(outlineType);
        for (int i = 0; i < outlinedMaterials.Length; i++)
        {
            Material material = new Material(baseOutline);
            material.color = baseMaterials[i].color;
            outlinedMaterials[i] = material;
        }
    }
}
