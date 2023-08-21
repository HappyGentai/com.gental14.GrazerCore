using UnityEngine;
using GrazerCore.Interfaces;

namespace GrazerCore.Fields
{
    /// <summary>
    /// Any object which have collider(2D) enter this area will set to disable.
    /// </summary>
    public class RecycleArea : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D collision)
        {
            var recycleable = collision.gameObject.GetComponent<IRecycleable>();
            if (recycleable != null)
            {
                recycleable.Recycle();
            }
        }
    }
}
