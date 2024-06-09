using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public enum InfoType { Exp, Level, Kill, Time, Health }
    public InfoType type;

    Text myText;
    Slider mySlider;

    void Awake()
    {
       myText = GetComponentInChildren<Text>();
        mySlider = GetComponentInChildren<Slider>();
    }

    void LateUpdate()
    {
        switch(type)
        {
            case InfoType.Exp:
                float curExp = GameManager.instance.exp;
                float maxExp = GameManager.instance.nextExp[Mathf.Min(GameManager.instance.level, GameManager.instance.nextExp.Length - 1)];
                mySlider.value = curExp / maxExp;
                break;
            case InfoType.Level:
                //{0}은 오는 인덱스 넘버
                myText.text = string.Format("Lv.{0:F0}",GameManager.instance.level); //문자열로 변환
                break;
            case InfoType.Kill:
                myText.text = string.Format("{0:F0}", GameManager.instance.kill); //문자열로 변환
                break;
            case InfoType.Time:
                float remainTime = GameManager.instance.maxGameTime - GameManager.instance.gameTime;
                int min = Mathf.FloorToInt(remainTime / 60);
                int sec = Mathf.FloorToInt(remainTime % 60);
                // {D}는 자리수 고정
                myText.text = string.Format("{0:D2}:{1:D2}", min, sec); //문자열로 변환
                break;
            case InfoType.Health:
                float curHealth = GameManager.instance.health;
                float maxHealth = GameManager.instance.maxHealth;
                mySlider.value = curHealth / maxHealth;
                break;
        }    
    }

}
