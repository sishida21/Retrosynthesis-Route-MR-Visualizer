using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;
using TMPro;

public class DisplayInteraction : MonoBehaviour, IMixedRealityPointerHandler
{
    public GameObject textPrefab; // Assign in Inspector
    public string displayText;
    private GameObject currentTextObject = null;
    public float rightOffsetScale = -3.5f;
    public float forwardOffsetScale = -1.2f;
    public Texture2D texture;
    public GameObject pngObj;
    //public float boardScale = 0.5f;

    public void OnPointerClicked(MixedRealityPointerEventData eventData)
    {
        if (currentTextObject != null)
        {
            Destroy(currentTextObject);
            Destroy(pngObj);
            currentTextObject = null;
            pngObj = null;
        } else
        {
            ShowText();
            drawReaction();
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
        Vector3 textPosition = transform.position + rightOffsetScale * transform.up * offset + forwardOffsetScale * transform.forward * offset;

        currentTextObject = Instantiate(textPrefab, textPosition, Quaternion.identity);
        currentTextObject.transform.SetParent(this.transform);
        currentTextObject.transform.localScale = Vector3.one * 6.0f;
        TextMeshProUGUI tmPro = currentTextObject.transform.GetChild(0).transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        if (tmPro != null)
        {
            tmPro.text = displayText;
        }
    }

    private void drawReaction()
    {
        Collider collider = this.gameObject.GetComponent<Collider>();
        string nodeId = this.gameObject.name;
        if (collider == null)
        {
            Debug.LogError("Cannot find Renderer component");
            return;
        }
        float offset = collider.bounds.extents.magnitude; // half the "size" of the object
        Vector3 textPosition = transform.position - rightOffsetScale * transform.up * offset + forwardOffsetScale * transform.forward * offset;

        texture = Resources.Load<Texture2D>("images/" + nodeId);
        pngObj = new GameObject("2D_molecule");
        pngObj.transform.localScale = Vector3.one * 0.06f;
        pngObj.transform.position = textPosition;

        pngObj.AddComponent<SpriteRenderer>();
        Sprite sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);
        SpriteRenderer renderer = pngObj.GetComponent<SpriteRenderer>();
        renderer.sprite = sprite;
    }
}
