using System.Collections;
using UnityEngine;

namespace GrazerCore.Tool
{
    public class SimpleFlash : MonoBehaviour
    {
        [SerializeField][Tooltip("Require textShader")]
        private Material m_FlashMaterial = null;
        [SerializeField]
        private SpriteRenderer[] m_TargetRenders = null;
        private Material[] m_TargetsOriginalMaterial = null;
        [SerializeField]
        private float m_FlashTime = 0.25f;
        private Coroutine flashRoutine = null;

        private void Start()
        {
            RecordOriginalMaterial();
        }

        private void OnDisable()
        {
            StopFlash();
            SetToOriginal();
        }

        private void RecordOriginalMaterial()
        {
            var targetCount = m_TargetRenders.Length;
            m_TargetsOriginalMaterial = new Material[targetCount];
            for (int index = 0; index < targetCount; ++index)
            {
                m_TargetsOriginalMaterial[index] = m_TargetRenders[index].material;
            }
        }

        private void SetFlash()
        {
            var targetCount = m_TargetRenders.Length;
            for (int index = 0; index < targetCount; ++index)
            {
                m_TargetRenders[index].material = m_FlashMaterial;
            }
        }

        private void SetToOriginal()
        {
            var targetCount = m_TargetRenders.Length;
            for (int index = 0; index < targetCount; ++index)
            {
                m_TargetRenders[index].material = m_TargetsOriginalMaterial[index];
            }
        }

        private void StopFlash()
        {
            if (flashRoutine != null)
            {
                StopCoroutine(flashRoutine);
            }
            flashRoutine = null;
        }

        /// <summary>
        /// If forceReplace are true, will stop last working coroutine
        /// and play new one.
        /// </summary>
        /// <param name="forceReplace"></param>
        public void DoFlash(bool forceReplace)
        {
            if (!this.gameObject.activeInHierarchy)
            {
                return;
            }
            else if (!forceReplace && flashRoutine != null)
            {
                return;
            } else if (forceReplace)
            {
                StopFlash();
            }
            flashRoutine = StartCoroutine(Flashing());
        }

        private IEnumerator Flashing()
        {
            SetFlash();
            yield return new WaitForSeconds(m_FlashTime);
            SetToOriginal();
            flashRoutine = null;
        }
    }
}
