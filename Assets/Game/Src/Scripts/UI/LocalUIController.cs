using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LocalUIController : MonoBehaviour
{
    [SerializeField] LocalUIEventChanelScriptableObject eventChanel;
    [SerializeField] Image fillImageHealth;
    [SerializeField] Image fillImageAmmo;
    [SerializeField] TextMeshProUGUI magazineNumber;
    [SerializeField] Image weaponIcon;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        eventChanel.AddListeners(this);
    }

    private void OnDestroy()
    {
        eventChanel.RemoveListeners(this);
    }

    public void UpdateHealthUI(float health)
    {
        fillImageHealth.fillAmount = health ;
    }

    public void UpdateAmmoUIEvent(float remainingAmmo)
    {
        fillImageAmmo.fillAmount = remainingAmmo;
    }


    public void UpdateMagazineNumberUI(int number)
    {
        magazineNumber.text = number.ToString();
    }

    public void UpdateWeaponIcon(Sprite newIcon)
    {
        weaponIcon.sprite = newIcon;
    }


}
