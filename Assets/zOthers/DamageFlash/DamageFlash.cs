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
        [SerializeField] private Material flashMat;
        [SerializeField] private MeshRenderer meshRenderer;
        [SerializeField] private float blinkDuration = 0.075f;

        private List<Material> materials = new List<Material>();
        private List<Material> flashMats = new List<Material>();

        private void Start()
        {
            for (int i = 0; i < meshRenderer.materials.Length; i++)
            {
                materials.Add(meshRenderer.materials[i]);
                flashMats.Add(flashMat);
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Flash();
            }
        }

        public void Flash()
        {
            meshRenderer.materials = flashMats.ToArray();

            StartCoroutine(FadeDelay());
        }

        private IEnumerator FadeDelay()
        {
            yield return new WaitForSeconds(blinkDuration);
            for (int i = 0; i < meshRenderer.materials.Length; i++)
            {
                meshRenderer.materials = materials.ToArray();
            }
        }
    }
}