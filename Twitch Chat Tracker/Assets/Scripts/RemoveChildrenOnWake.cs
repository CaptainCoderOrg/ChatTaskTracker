using System.Collections.Generic;
using UnityEngine;

public class RemoveChildrenOnWake : MonoBehaviour
{
    [field: SerializeField]
    public List<GameObject> Skip { get; private set; }
    void Awake()
    {
        for (int ix = transform.childCount - 1; ix >= 0; ix--)
        {
            var go = transform.GetChild(ix).gameObject;
            if (Skip.Contains(go)) { continue; }
            Object.Destroy(go);
        }
    }
}
