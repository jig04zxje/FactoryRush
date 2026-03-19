using UnityEngine;

public class SFX_Tester : MonoBehaviour
{
    void Update()
    {
        // Nhấn phím để test từng clip
        if (Input.GetKeyDown(KeyCode.Alpha1))
            AudioManager.Instance.PlaySFX("sell_ding");

        if (Input.GetKeyDown(KeyCode.Alpha2))
            AudioManager.Instance.PlaySFX("timer_tick");

        if (Input.GetKeyDown(KeyCode.Alpha3))
            AudioManager.Instance.PlaySFX("record_broken");
    }
}