using UnityEngine;
using UnityEngine.UI;

namespace Abiogenesis3d.UPixelator_Demo
{
    public class HandleUI_PixelArtEdgeHighlights : MonoBehaviour
    {
        [HideInInspector]
        public PixelArtEdgeHighlights paeh;
        public Toggle effectEnabled;
        public Slider convexHighlight;
        public Slider outlineShadow;
        public Slider concaveShadow;
        public Slider depthSensitivity;
        public Slider debugEffect;
        public Text debugEffectName;

        void Start()
        {
            paeh = FindObjectOfType<PixelArtEdgeHighlights>();
            if (!paeh) return;
            effectEnabled.isOn = paeh.gameObject.activeInHierarchy;
            convexHighlight.value = paeh.convexHighlight;
            outlineShadow.value = paeh.outlineShadow;
            concaveShadow.value = paeh.concaveShadow;
            depthSensitivity.value = paeh.depthSensitivity;
            debugEffect.value = (int)paeh.debugEffect;
        }

        void Update()
        {
            if (!paeh) return;
            paeh.gameObject.SetActive(effectEnabled.isOn);
            paeh.convexHighlight = convexHighlight.value;
            paeh.outlineShadow = outlineShadow.value;
            paeh.concaveShadow = concaveShadow.value;
            paeh.depthSensitivity = depthSensitivity.value;
            paeh.debugEffect = (PixelArtEdgeHighlightsDebugEffect)debugEffect.value;
            var d = paeh.debugEffect.ToString();
            if (d == "None") d = "";
            debugEffectName.text = d;
        }
    }
}
