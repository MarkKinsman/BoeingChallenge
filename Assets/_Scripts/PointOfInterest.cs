using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[ExecuteInEditMode]
public class PointOfInterest : MonoBehaviour
{
    [Header("POI Information")]
    [SerializeField]
    private  string poiTitle;
    [SerializeField]
    private Texture poiImage;
    [SerializeField]
    private string info1Title;
    [SerializeField]
    private string info1Desc;
    [SerializeField]
    private string info2Title;
    [SerializeField]
    private string info2Desc;
    [SerializeField]
    private string info3Title;
    [SerializeField]
    private string info3Desc;

    private GameObject locationMarker;
    private GameObject moreInfo;

    // Use this for initialization
    void Start()
    {
        locationMarker = gameObject.transform.Find("LocationMarker").gameObject;
        moreInfo = gameObject.transform.Find("MoreInfo").gameObject;

        UpdateInfo();
    }

    private void OnSelect()
    {
        locationMarker.SetActive(!locationMarker.activeInHierarchy);
        moreInfo.SetActive(!moreInfo.activeInHierarchy);
    }

    private void OnRenderObject()
    {
        UpdateInfo();
    }

    private void UpdateInfo()
    {
        locationMarker.transform.Find("TitleText").gameObject.GetComponent<TextMeshProUGUI>().SetText(poiTitle);
        moreInfo.transform.Find("MiddlePanel/InfoPanel/TitlePanel/InfoTitleText").GetComponent<TextMeshProUGUI>().SetText(poiTitle);
        moreInfo.transform.Find("MiddlePanel/InfoPanel/Info1Panel/Info1CategoryText").GetComponent<TextMeshProUGUI>().SetText(info1Title + ":");
        moreInfo.transform.Find("MiddlePanel/InfoPanel/Info1Panel/Info1DescText").GetComponent<TextMeshProUGUI>().SetText(info1Desc);
        moreInfo.transform.Find("MiddlePanel/InfoPanel/Info2Panel/Info2CategoryText").GetComponent<TextMeshProUGUI>().SetText(info2Title + ":");
        moreInfo.transform.Find("MiddlePanel/InfoPanel/Info2Panel/Info2DescText").GetComponent<TextMeshProUGUI>().SetText(info2Desc);
        moreInfo.transform.Find("MiddlePanel/InfoPanel/Info3Panel/Info3CategoryText").GetComponent<TextMeshProUGUI>().SetText(info3Title + ":");
        moreInfo.transform.Find("MiddlePanel/InfoPanel/Info3Panel/Info3DescText").GetComponent<TextMeshProUGUI>().SetText(info3Desc);

        Material picture = new Material(Shader.Find("Unlit/Texture"));
        picture.name = poiTitle;
        picture.mainTexture = poiImage;
        moreInfo.transform.Find("MiddlePanel/PicturePanel").GetComponent<Image>().material = picture;
    }
}
