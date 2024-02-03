using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering.HighDefinition;

namespace XLWeather.Utils
{
    /*
    public class MaterialUtil : MonoBehaviour
    {
        public List<Material> hdrpMaterials;
        public Material TestMat;
        public Texture blankTexture;

        // ----------- get map materials -----------------

        private void Start()
        {
            //CreateTexture();
        }
        private void CreateTexture()
        {
            int width = 2048; // Adjust the width of the texture
            int height = 2048; // Adjust the height of the texture

            // Create a new RenderTexture with transparent pixels
            RenderTexture renderTexture = new RenderTexture(width, height, 0);
            renderTexture.enableRandomWrite = true;
            renderTexture.Create();

            // Assign the RenderTexture to the blankTexture variable
            blankTexture = renderTexture;

            // You can use the blankTexture as needed.
        }

        private void GetMapMaterials()
        {
            hdrpMaterials = new List<Material>();

            Renderer[] renderers = FindObjectsOfType<Renderer>();

            if (renderers == null)
                return;

            foreach (Renderer renderer in renderers)
            {
                Material[] materials = renderer.sharedMaterials;

                foreach (Material material in materials)
                {
                    if (material != null && material.HasProperty("_MaskMap"))
                    {
                        hdrpMaterials.Add(material);
                    }
                }
            }
            Main.Logger.Log($"{hdrpMaterials.Count} materials found");
        }
        public void GetTestMat()
        {
            if (hdrpMaterials == null) return;

            foreach (Material mat in hdrpMaterials)
            {
                if (TestMat != null) return;

                if (mat.name.Contains("metal_fence-1"))
                {
                    TestMat = mat;
                }
            }

        }
        public void UpdateMaterials()
        {
            if (hdrpMaterials == null) return;

            foreach (Material mat in hdrpMaterials)
            {
                mat.SetTexture("_MaskMap", blankTexture);

                Color color = mat.GetColor("_Color");
                color.a = 0.5f; // 50% transparent
                mat.SetColor("_Color", color);

                mat.SetFloat("_Alpha", 0.5f); // Set to 50% transparency

                // Set to Opaque mode
                mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
            }
        }
       
        public void ListMatProperties()
        {
            if (TestMat == null)
            {
                Main.Logger.Log("TestMat Null");
                return;
            }

            int index = TestMat.shader.GetPropertyCount();

            for (int i = 0; i < index; i++)
            {
                string name = TestMat.shader.GetPropertyName(i);
                Main.Logger.Log($"{name}");
            }
        }
    }
    */
}
