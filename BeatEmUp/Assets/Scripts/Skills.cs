﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Skills : MonoBehaviour
{

    public int str;
    public float agi;
    public int sta;
    public float lck;
    public int skillpoints;
    public int Exp;
    public int PlayerLevel;
    public int KickUpgrade;
    public int PunchUpgrade;
    public int PunchSkill;
    public int KickSkill;
    //public int currentStageLevel;
    public int Gold;

    public int BasepunchDamage = 10;
    public int BasekickDamage = 20;

    public int punchDamage;
    public int kickDamage;

    public float agiRatio;
    public int extraHealth;
    public float luckRatio;
    public float strRatio;

    public SaveData saveData;


    private void Awake()
    {
        saveData = GameObject.Find("Main Camera").GetComponent<SaveData>();
       
             
    }

    private void Start()
    {
        CalculateStats();
    }

    public void CalculateStats()
    {
        strRatio = str + str / 7;
        agiRatio = agi / 100;
        luckRatio = lck / 4; 
        punchDamage = BasepunchDamage + (int)strRatio;
        kickDamage = BasekickDamage + (int)strRatio;
        extraHealth = sta * 20;
    }

  

   
    

    
}
