using System.Collections;
using UnityEngine;

namespace Djinde.Quest
{
    public class GridLever : Highlightable
    {
        [Header("References")]
        [SerializeField]
        private GameObject grid;
        [Header("Parameters")]
        [SerializeField]
        [Range(0, 1)]
        private float translateSpeed = 0.1f;
        [SerializeField]
        private bool open = false;

        private AudioSource leverSource;
        private AudioSource gridSource;

        private float gridHeight;
        private float position = 0;
        private bool animationInProgress = false;

        override protected void Awake()
        {
            base.Awake();
            leverSource = GetComponent<AudioSource>();
            gridSource = grid.GetComponent<AudioSource>();
            gridHeight = grid.GetComponent<BoxCollider>().bounds.size.y;
            if (open)
            {
                StartCoroutine(translateGrid());
            }
        }

        protected override void activate()
        {
            if (!animationInProgress)
            {
                StartCoroutine(translateGrid());
                transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, -transform.localScale.z);
                gridSource.volume = SettingsManager.Instance.EffectsVolume;
                leverSource.volume = SettingsManager.Instance.EffectsVolume;
                gridSource.Play();
                leverSource.Play();
            }
        }

        IEnumerator translateGrid()
        {
            animationInProgress = true;
            do
            {
                grid.transform.Translate(new Vector3(0, 0, translateSpeed));
                position += translateSpeed;
                yield return null;
            } while (Mathf.Abs(position) <= gridHeight);
            position = 0;
            translateSpeed = -translateSpeed;
            animationInProgress = false;
        }
    }
}