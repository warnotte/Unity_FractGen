using System;
using UnityEngine;

namespace UnityStandardAssets.ImageEffects
{
    [ExecuteInEditMode]
    [AddComponentMenu("PostProd/VintageScreen")]
    public class VintageScreen : ImageEffectBase
    {
        public Texture textureRamp;

        [Range(-1.0f, 1.0f)]
        public float rampOffset;
        [Range(-0.0f, 1.0f)]
        public float curveMix = 1.0f;
        [Range(-0.0f, 1.0f)]
        public float vignetteMix = 1.0f;
        [Range(-0.0f, 8.0f)]
        public float distortAmp = 1.0f;
        [Range(-0.0f, 1.0f)]
        public float chromaticAmp = 1.0f;
        [Range(-0.0f, 1.0f)]
        public float scanLineMix = 1.0f;

        [Range(-0.0f, 1.0f)]
        public float rMix = 1.0f;
        [Range(-0.0f, 1.0f)]
        public float gMix = 1.0f;
        [Range(-0.0f, 1.0f)]
        public float bMix = 1.0f;

        [Range(-0.0f, 1.0f)]
        public float dryMix = 1.0f;




        // Called by camera to apply image effect
        void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            material.SetTexture("_RampTex", textureRamp);
            material.SetFloat("_RampOffset", rampOffset);
            material.SetFloat("_DryMix", dryMix);
            material.SetFloat("_CurveMix", curveMix);
            material.SetFloat("_VignetteMix", vignetteMix);
            material.SetFloat("_ChromaticAmp", chromaticAmp);
            material.SetFloat("_DistortAmp", distortAmp);
            material.SetFloat("_ScanLineMix", scanLineMix);
            material.SetFloat("_RMix", rMix);
            material.SetFloat("_GMix", gMix);
            material.SetFloat("_BMix", bMix);
           Graphics.Blit(source, destination, material);
        }
    }
}
