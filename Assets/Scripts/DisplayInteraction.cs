using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;
using UnityEngine.UI;

public class DisplayInteraction : MonoBehaviour, IMixedRealityPointerHandler
{
    public GameObject textPrefab; // Assign in Inspector
    private GameObject currentTextObject = null;

    public void OnPointerClicked(MixedRealityPointerEventData eventData)
    {
        if (currentTextObject != null)
        {
            Destroy(currentTextObject);
            currentTextObject = null;
        } else
        {
            ShowText();
        }
    }

    public void OnPointerDown(MixedRealityPointerEventData eventData) { }

    public void OnPointerDragged(MixedRealityPointerEventData eventData) { }

    public void OnPointerUp(MixedRealityPointerEventData eventData) { }

    private void ShowText()
    {
        Collider collider = this.gameObject.GetComponent<Collider>();
        if (collider == null)
        {
            Debug.LogError("Cannot find Renderer component");
            return;
        }

        // Calculate the position to place the text
        float offset = collider.bounds.extents.magnitude; // half the "size" of the object
        Vector3 textPosition = transform.position + transform.forward * offset;

        //GameObject textObject = Instantiate(textPrefab, transform.position + new Vector3(0.2f, 0, 0), Quaternion.identity);
        currentTextObject = Instantiate(textPrefab, textPosition, Quaternion.identity);
        currentTextObject.transform.SetParent(this.transform);
        Text text = currentTextObject.transform.GetChild(0).transform.GetChild(0).GetComponent<Text>();
        if (text != null)
        {
            text.text = this.gameObject.name;
        }
    }
}
