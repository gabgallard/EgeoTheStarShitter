using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(TMP_Text))]
public class LinkOpener : MonoBehaviour, IPointerClickHandler {

    public void OnPointerClick(PointerEventData eventData) {
        TMP_Text pTextMeshPro = GetComponent<TMP_Text>();
        int linkIndex = TMP_TextUtilities.FindIntersectingLink(pTextMeshPro, eventData.position, null);  // If you are not in a Canvas using Screen Overlay, put your camera instead of null
        if (linkIndex != -1) { // was a link clicked?
            TMP_LinkInfo linkInfo = pTextMeshPro.textInfo.linkInfo[linkIndex];
            string url = linkInfo.GetLinkID();
            // Application.OpenURL(url);
            Application.ExternalEval("window.open(\"" + url + "\")");
            // OpenURLInExternalWindow(url);
        }
    }

    // [DllImport("__Internal")]
    //  private static extern void OpenURLInExternalWindow(string url);

    //  public void OpenMyUrl()
    //  {
    //      OpenURLInExternalWindow(url);
    //  }

}
