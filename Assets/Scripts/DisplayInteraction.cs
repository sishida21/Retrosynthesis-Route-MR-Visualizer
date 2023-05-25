using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;
using TMPro;

public class DisplayInteraction : MonoBehaviour, IMixedRealityPointerHandler
{
    public GameObject textPrefab; // Assign in Inspector
    public string displayText;
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
        Vector3 textPosition = transform.position + -2.0f * transform.up * offset + -1.2f * transform.forward * offset;

        currentTextObject = Instantiate(textPrefab, textPosition, Quaternion.identity);
        currentTextObject.transform.SetParent(this.transform);
        TextMeshProUGUI tmPro = currentTextObject.transform.GetChild(0).transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        if (tmPro != null)
        {
            tmPro.text = displayText;
        }
    }
}
