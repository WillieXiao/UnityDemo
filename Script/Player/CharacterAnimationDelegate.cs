using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimationDelegate : MonoBehaviour
{
    public GameObject leftHand_Attack_Point, rightHand_Attack_Point, leftFoot_Attack_Point,rightFoot_Attack_Point, Sword_Attack_Point, pakourRayPoint,WillPowerPoint;

    public void LeftHand_Attack_On()
    {        
        leftHand_Attack_Point.SetActive(true);
        leftHand_Attack_Point.GetComponent<AttackUniversal>().startPoint = leftHand_Attack_Point.transform.position;
    }
    public void LeftHand_Attack_Off()
    {
        leftHand_Attack_Point.GetComponent<AttackUniversal>().endPoint = leftHand_Attack_Point.transform.position;
        if (leftHand_Attack_Point.activeInHierarchy)
        {          
            leftHand_Attack_Point.SetActive(false);
        }
    }
    public void RightHand_Attack_On()
    {
        rightHand_Attack_Point.SetActive(true);
        rightHand_Attack_Point.GetComponent<AttackUniversal>().startPoint = rightHand_Attack_Point.transform.position;
    }
    public void RightHand_Attack_Off()
    {
        rightHand_Attack_Point.GetComponent<AttackUniversal>().endPoint = rightHand_Attack_Point.transform.position;
        if (rightHand_Attack_Point.activeInHierarchy)
        {           
            rightHand_Attack_Point.SetActive(false);
        }
    }

    public void RightFoot_Attack_On()
    {
        rightFoot_Attack_Point.SetActive(true);
        rightFoot_Attack_Point.GetComponent<AttackUniversal>().startPoint = rightFoot_Attack_Point.transform.position;
    }
    public void RightFoot_Attack_Off()
    {
        if (rightFoot_Attack_Point.activeInHierarchy)
        {
            rightFoot_Attack_Point.SetActive(false);
        }
    }

    public void LeftFoot_Attack_On()
    {
        leftFoot_Attack_Point.SetActive(true);
        leftFoot_Attack_Point.GetComponent<AttackUniversal>().startPoint = leftFoot_Attack_Point.transform.position;
    }
    public void LeftFoot_Attack_Off()
    {
        if (leftFoot_Attack_Point.activeInHierarchy)
        {
            leftFoot_Attack_Point.SetActive(false);
        }
    }

    public void Sword_Attack_On()
    {
        Sword_Attack_Point.SetActive(true);
        Sword_Attack_Point.GetComponent<AttackUniversal>().startPoint = Sword_Attack_Point.transform.position;
    }
    public void Sword_Attack_Off()
    {
        Sword_Attack_Point.GetComponent<AttackUniversal>().endPoint = Sword_Attack_Point.transform.position;
        if (Sword_Attack_Point.activeInHierarchy)
        {
            Sword_Attack_Point.SetActive(false);
        }
    }

    public void RayPoint_On()
    {
        pakourRayPoint.SetActive(true);
    }
    public void RayPoint_Off()
    {
        if (pakourRayPoint.activeInHierarchy)
        {
            pakourRayPoint.SetActive(false);
        }
    }

    public void WillPowerPoint_On()
    {
        WillPowerPoint.SetActive(true);
        WillPowerPoint.GetComponent<AttackUniversal>().startPoint = WillPowerPoint.transform.position;
    }
    public void WillPowerPoint_Off()
    {
        
        if (WillPowerPoint.activeInHierarchy)
        {
            WillPowerPoint.SetActive(false);
        }
    }
    //擊退
    public void TagLeft_Arm()
    {
        leftHand_Attack_Point.tag = "Left_Arm";
    }
    public void UnTagLeft_Arm()
    {
        leftHand_Attack_Point.tag = "Untagged";
    }
    public void TagRight_Arm()
    {
        rightHand_Attack_Point.tag = "Right_Arm";
    }
    public void UnTagRight_Arm()
    {
        rightHand_Attack_Point.tag = "Untagged";
    }
    //擊飛
    public void HitFly_TagArm()
    {
        leftHand_Attack_Point.tag = "HitFly_Arm";
        
    }
    public void HitFly_UnTagArm()
    {
        leftHand_Attack_Point.tag = "Untagged";
    }


}
