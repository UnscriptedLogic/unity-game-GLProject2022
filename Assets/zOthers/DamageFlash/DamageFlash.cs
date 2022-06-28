using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

namespace External.DamageFlash
{
    public class DamageFlash : MonoBehaviour
    {
        [SerializeField] protected Material flashMat;
        [SerializeField] protected MeshRenderer meshRenderer;
        [SerializeField] protected float blinkDuration = 0.075f;

        private List<Material> materials = new List<Material>();
        private List<Material> flashMats = new List<Material>();

        protected virtual void Start()
        {
            for (int i = 0; i < meshRenderer.materials.Length; i++)
            {
                materials.Add(meshRenderer.materials[i]);
                flashMats.Add(flashMat);
            }
        }

        protected virtual void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Flash();
            }
        }

        public virtual void Flash()
        {
            meshRenderer.materials = flashMats.ToArray();

            StartCoroutine(FadeDelay());
        }

        protected virtual IEnumerator FadeDelay()
        {
            yield return new WaitForSeconds(blinkDuration);
            for (int i = 0; i < meshRenderer.materials.Length; i++)
            {
                meshRenderer.materials = materials.ToArray();
            }
        }
    }
}