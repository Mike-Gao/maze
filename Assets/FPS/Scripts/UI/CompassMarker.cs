using UnityEngine;
using UnityEngine.UI;

public class CompassMarker : MonoBehaviour
{
    [Tooltip("Main marker image")]
    public Image mainImage;
    [Tooltip("Canvas group for the marker")]
    public CanvasGroup canvasGroup;

    [Header("Enemy element")]
    [Tooltip("Default color for the marker")]
    public Color defaultColor;
    [Tooltip("Alternative color for the marker")]
    public Color altColor;

    [Header("Direction element")]
    [Tooltip("Use this marker as a magnetic direction")]
    public bool isDirection;
    [Tooltip("Text content for the direction")]
    public TMPro.TextMeshProUGUI textContent;


    public void Initialize(CompassElement compassElement, string textDirection)
    {
        if (isDirection && textContent)
        {
            textContent.text = textDirection;
        }
    }

    public void DetectTarget()
    {
        mainImage.color = altColor;
    }

    public void LostTarget()
    {
        mainImage.color = defaultColor;
    }
}
