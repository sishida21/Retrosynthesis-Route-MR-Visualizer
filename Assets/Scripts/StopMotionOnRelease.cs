using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine;


[RequireComponent(typeof(ObjectManipulator))]
public class StopMotionOnRelease : MonoBehaviour
{
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        ObjectManipulator manipulator = GetComponent<ObjectManipulator>();

        manipulator.OnManipulationEnded.AddListener((_) => StopMotion());
    }

    private void StopMotion()
    {
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }
}

