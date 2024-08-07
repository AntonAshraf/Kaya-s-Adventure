using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TimeCountdown : MonoBehaviour
{
    public TMPro.TextMeshProUGUI text;
    float endTime;
    public int timeLeft;
    public Animator anim;
    void Start()
    {
        endTime = Time.time + 360;
        text = GetComponent<TextMeshProUGUI>();
        text.text = "360";
        timeLeft = (int)(endTime - Time.time);
    }

    void Update()
    {
        if (anim.GetBool("Win"))
        {
            text.text = timeLeft.ToString();
            return;
        }
        timeLeft = (int)(endTime - Time.time);
        if (timeLeft < 0) {
            timeLeft = 0;
        }
        text.text = timeLeft.ToString();
    }

}
