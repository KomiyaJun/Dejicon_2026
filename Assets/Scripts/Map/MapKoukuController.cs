using UnityEngine;

public class MapKoukuController : MonoBehaviour
{
    public static MapKoukuController ActiveInstance { get; private set; }

    [Header("校区線なしマップ")]
    [SerializeField] private GameObject mapKoukuOff;

    [Header("校区線ありマップ")]
    [SerializeField] private GameObject mapKoukuOn;

    [Header("開始時に校区線を表示するか")]
    [SerializeField] private bool showKoukuOnStart = false;

    [Header("校区線ありを表示中、校区線なしを非表示にする")]
    [SerializeField] private bool hideOffMapWhenOn = true;

    private bool isKoukuOn;

    private void Awake()
    {
        ActiveInstance = this;
        SetKouku(showKoukuOnStart);
    }

    private void OnEnable()
    {
        ActiveInstance = this;
    }

    private void OnDestroy()
    {
        if (ActiveInstance == this)
        {
            ActiveInstance = null;
        }
    }

    public void ShowKoukuOn()
    {
        SetKouku(true);
    }

    public void ShowKoukuOff()
    {
        SetKouku(false);
    }

    public void ToggleKouku()
    {
        SetKouku(!isKoukuOn);
    }

    private void SetKouku(bool show)
    {
        isKoukuOn = show;

        if (mapKoukuOn != null)
        {
            mapKoukuOn.SetActive(show);
        }

        if (mapKoukuOff != null && hideOffMapWhenOn)
        {
            mapKoukuOff.SetActive(!show);
        }
        else if (mapKoukuOff != null)
        {
            mapKoukuOff.SetActive(true);
        }
    }
}