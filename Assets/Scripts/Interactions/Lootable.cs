using System.Collections;
using UnityEngine;

namespace Djinde.Quest
{
    public class Lootable : ItemPhysics
    {
        [Header("Parameters")]
        public Item item;

        private Material[] baseMaterials;
        private Material[] outlinedMaterials;
        private Renderer renderer;

        protected override void Awake()
        {
            base.Awake();
            renderer = GetComponent<Renderer>();
            baseMaterials = renderer.materials;
            item.fetchType();
        }

        protected void Start()
        {
            initializeOutlinedMaterials();
        }

        protected override void mouseEntered()
        {
            MainUI.Instance.displayDescription(item.description);
            toggleOutline(!grabbed);
        }

        protected override void mouseExited()
        {
            MainUI.Instance.hideDescription();
            toggleOutline(false);
        }

        private void loot()
        {
            toggleOutline(false);
            InventoryManager.Instance.loot(this);
        }

        protected override void activate()
        {
            base.activate();
            toggleOutline(false);
            StartCoroutine(checkIfGrabbedOrClicked());
        }

        private IEnumerator checkIfGrabbedOrClicked()
        {
            if (CameraManager.Instance.FirstPersonViewActive)
            {
                yield return new WaitForSecondsRealtime(0.1f);
                if (Input.GetMouseButton(0))
                {
                    grabbed = true;
                }
                else
                {
                    loot();
                }
            }
            else
            {
                loot();
            }
        }

        private void toggleOutline(bool on)
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
            Material baseOutline = GameConstants.Instance.getLootableOutlineMaterialByType(item.rarity);
            for (int i = 0; i < outlinedMaterials.Length; i++)
            {
                Material material = new Material(baseOutline);
                material.color = baseMaterials[i].color;
                outlinedMaterials[i] = material;
            }
        }
    }
}