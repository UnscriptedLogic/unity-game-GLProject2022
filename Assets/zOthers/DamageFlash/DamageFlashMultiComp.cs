using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace External.DamageFlash
{
    public class DamageFlashMultiComp : DamageFlash
    {
        [SerializeField] private MeshRenderer[] meshRenderers;

        private class MeshRendererMat
        {
            public Material[] materials;
            public MeshRendererMat(Material[] materials) => this.materials = materials;
        }

        private MeshRendererMat[] meshRendererMats;

        protected override void Start()
        {
            meshRendererMats = new MeshRendererMat[meshRenderers.Length];
            for (int i = 0; i < meshRenderers.Length; i++)
            {
                meshRendererMats[i] = new MeshRendererMat(meshRenderers[i].materials);
            }
        }

        protected override void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Flash();
            }
        }

        public override void Flash()
        {
            StopAllCoroutines();
            for (int i = 0; i < meshRenderers.Length; i++)
            {
                StartCoroutine(FadeDelay(meshRenderers[i], i));
            }
        }

        private void MeshRendererMethod()
        {
            for (int i = 0; i < meshRenderers.Length; i++)
            {
                int count = meshRenderers[i].materials.Length;
                Material[] flashmaterials = new Material[count];
                for (int x = 0; x < flashmaterials.Length; x++)
                {
                    flashmaterials[x] = flashMat;
                }
                meshRenderers[i].materials = flashmaterials;
            }
        }

        protected IEnumerator FadeDelay(MeshRenderer meshRenderer, int index)
        {
            int length = meshRenderer.materials.Length;
            Material[] flashMats = new Material[length];
            for (int i = 0; i < length; i++)
            {
                flashMats[i] = flashMat;
            }

            meshRenderer.materials = flashMats;
            yield return new WaitForSeconds(blinkDuration);
            meshRenderers[index].materials = meshRendererMats[index].materials;
        }
    }
}