using System.Collections;
using UnityEngine;

namespace GrazerCore.Tool
{
    public class CoroutineAgent : MonoBehaviour
    {
        private static CoroutineAgent agent = null;

        public static Coroutine StartEntrustCoroutine(IEnumerator enumerator)
        {
            if (agent == null)
            {
                var newGO = new GameObject("CoroutineAgent ");
                agent = newGO.AddComponent<CoroutineAgent>();
            }
            var newCoroutine = agent.StartCoroutine(enumerator);
            return newCoroutine;
        }

        public static void StopEntrustCoroutine(Coroutine _entrustCoroutine)
        {
            agent.StopCoroutine(_entrustCoroutine);
        }
    }
}
