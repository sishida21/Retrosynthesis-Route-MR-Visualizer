using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine;


[RequireComponent(typeof(ObjectManipulator))]
public class FreezeRotationOnRelease : MonoBehaviour
{
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        ObjectManipulator manipulator = GetComponent<ObjectManipulator>();

        manipulator.OnManipulationEnded.AddListener((_) => FreezeRotation());
        manipulator.OnManipulationStarted.AddListener((_) => UnfreezeRotation());
    }

    private void FreezeRotation()
    {
        if (rb != null)
        {
            rb.constraints = RigidbodyConstraints.FreezeRotation;
        }
    }

    private void UnfreezeRotation()
    {
        if (rb != null)
        {
            rb.constraints = RigidbodyConstraints.None;
        }
    }
}
