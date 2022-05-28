using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts
{
    public class DamageFlashMultiComp : MonoBehaviour
    {
        [SerializeField] private Material flashMat;
        [SerializeField] private MeshRenderer[] meshRenderers;
        [SerializeField] private float blinkDuration = 0.075f;

        private class MeshRendererMat
        {
            public Material[] materials;
            public MeshRendererMat(Material[] materials) => this.materials = materials;
        }

        private MeshRendererMat[] meshRendererMats;

        private void Start()
        {
            meshRendererMats = new MeshRendererMat[meshRenderers.Length];
            for (int i = 0; i < meshRenderers.Length; i++)
            {
                meshRendererMats[i] = new MeshRendererMat(meshRenderers[i].materials);
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

        private IEnumerator FadeDelay(MeshRenderer meshRenderer, int index)
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