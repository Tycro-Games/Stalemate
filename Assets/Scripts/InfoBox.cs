using TMPro;
using UnityEngine;

public class InfoBox : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI title;

    [SerializeField] private TextMeshProUGUI description;

    public void SetInfo(UnitBoardInfo unitSettings)
    {
        title.text = unitSettings.unitSettings.title;
        description.text = unitSettings.unitSettings.description;
    }

    public void SetToDefault()
    {
        title.text = "title";
        description.text = "default description";
    }
}