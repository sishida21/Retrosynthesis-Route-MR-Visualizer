using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using System;
using UnityEngine;

public class ReactionCreator : MonoBehaviour
{
    public GameObject nodeSphere;
    public GameObject textPrefab;
    public GameObject CreateTransparentSphere(DataNode node)
    {
        GameObject sphere = Instantiate(nodeSphere);
        sphere.tag = "RxnContainer";
        //sphere.transform.SetParent(transform);
        sphere.transform.Rotate(new Vector3(0, 0, 90));
        sphere.name = node.id;
        sphere.transform.localScale = Vector3.one * 0.2f;

        Rigidbody rb = sphere.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        sphere.AddComponent<BoxCollider>();
        sphere.AddComponent<ObjectManipulator>();
        sphere.AddComponent<NearInteractionGrabbable>();
        DisplayInteraction interaction = sphere.AddComponent<DisplayInteraction>();
        interaction.textPrefab = textPrefab;
        string text = String.Format("<u>Reaction information</u>\n\n" +
            $"<size=\"55\">{node.numExamples} <size=\"35\">examples\n" + 
            $"<size=\"35\">Plausibility: <size=\"55\">{node.ffScore:f2}\n" +
            $"<size=\"35\">Template Score: <size=\"55\">{node.templateScore:f2}");
        interaction.displayText = text;
        return sphere;
    }
}
