using System.Collections;
using UnityEngine;

namespace External.DamageFlash
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

        public void Flash()
        {
            StopAllCoroutines();
            for (int i = 0; i < meshRenderers.Length; i++)
            {
                StartCoroutine(FadeDelay(meshRenderers[i], i));
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