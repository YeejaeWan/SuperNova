using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gear : MonoBehaviour
{
    public ItemData.ItemType type;
    public float rate;

    public void Init(ItemData data)
    {
        //±âº» ¼Â
        name = "Gear" + data.itemId;
        transform.parent = GameManager.instance.player.transform;
        transform.localPosition = Vector3.zero;


        //Propery set
        type = data.itemType;
        rate = data.damages[0];
        ApplyGear();


    }

    public void LevelUp(float rate)
    {
        this.rate = rate;
        ApplyGear();
    }

    void ApplyGear()
    {
        switch(type)
        {
            case ItemData.ItemType.Glove:
                RateUp();
                break;
            case ItemData.ItemType.Shoe:
                SpeedUp();
                break;

        }
    }


    void RateUp()
    {
        Weapon[] weapons = transform.parent.GetComponentsInChildren<Weapon>();

        foreach(Weapon weapon in weapons)
        {
            switch(weapon.id)
            {
                case 0:
                    float speed = 150 * Character.WeaponSpeed;
                    weapon.speed = 150 + (150 * rate);
                    break;
                default:
                    speed = 0.5f * Character.WeaponRate;
                    weapon.speed = 0.5f * (1f - rate);
                    //ÃÑ¾ËÀ» ´õ »¡¸® ¹ß»çÇÏ°Ô µÊ.
                    break;
            }
        }
    }

    void SpeedUp()
    {
        float speed = 3 * Character.Speed;
        GameManager.instance.player.speed = speed + speed * rate;

    }
}
