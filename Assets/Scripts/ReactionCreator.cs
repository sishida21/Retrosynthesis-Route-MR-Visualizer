using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine;

public class ReactionCreator : MonoBehaviour
{
    public GameObject nodeSphere;
    public GameObject CreateTransparentSphere(string smilesString)
    {
        GameObject sphere = Instantiate(nodeSphere);
        sphere.tag = "RxnContainer";
        sphere.transform.SetParent(transform);
        sphere.name = smilesString;
        sphere.transform.localScale = Vector3.one * 0.3f;

        Rigidbody rb = sphere.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        sphere.AddComponent<BoxCollider>();
        sphere.AddComponent<ObjectManipulator>();
        sphere.AddComponent<NearInteractionGrabbable>();
        return sphere;
    }
}
