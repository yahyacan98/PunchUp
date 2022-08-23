﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveData : MonoBehaviour
{
    //skills
    public int str;
    public int sta;
    public float agi;
    public float lck;
    public int skillpoints;
    public int Exp;
    public int PlayerLevel;

    public bool NewPlayer;

    //store
    public int Gold;
    public int PunchUpgrade;
    public int KickUpgrade;
    public int PunchSkill;
    public int KickSkill;

    //LevelManager
    public int currentStageLevel;

    public Skills skills;
    public LevelManager levelManager;

    public void Awake()
    {
        LoadInfo();

        if (GameObject.Find("Fighter"))
        {
            skills = GameObject.Find("Fighter").GetComponent<Skills>();
            LoadPlayer();
            skills.CalculateStats();
        }
        else if (GameObject.Find("SkillMenuController"))
        {
            skills = GameObject.Find("SkillMenuController").GetComponent<Skills>();
            LoadPlayer();
            skills.CalculateStats();
        }
        else if (GameObject.Find("Store"))
        {
            skills = GameObject.Find("Skills").GetComponent<Skills>();
            LoadPlayer();
            skills.CalculateStats();
        }


        levelManager = GameObject.Find("LevelManager").GetComponent<LevelManager>();
        LoadLevelManager();

    }
    public void UpdateSaveData()
    {
        str = skills.str;
        sta = skills.sta;
        agi = skills.agi;
        lck = skills.lck;
        skillpoints = skills.skillpoints;
        Exp = skills.Exp;
        PlayerLevel = skills.PlayerLevel;
        Gold = skills.Gold;
        currentStageLevel = levelManager.Level;
        PunchUpgrade = skills.PunchUpgrade;
        KickUpgrade = skills.KickUpgrade;
        PunchSkill = skills.PunchSkill;
        KickSkill = skills.KickSkill;
    }

    public void save()
    {
        UpdateSaveData();
        SaveSystem.SavePlayerSkills(this);
        skills.CalculateStats();
    }



    public void SaveNewPlayer()
    {
        SaveSystem.SavePlayerSkills(this);
    }

    public void LoadInfo()
    {
        Debug.Log("Player Info Loaded");
        PlayerData data = SaveSystem.LoadPlayerSkills();
        if (data != null)
        {
            str = data.Str;
            agi = data.Agi;
            sta = data.Sta;
            lck = data.Lck;
            skillpoints = data.SkillPoint;
            Exp = data.Exp;
            Gold = data.Gold;
            PlayerLevel = data.PlayerLevel;
            KickSkill = data.KickSkill;
            PunchSkill = data.PunchSkill;
            KickUpgrade = data.KickUpgrade;
            PunchUpgrade = data.PunchUpgrade;

            NewPlayer = false;
        }
        else
        {
            NewPlayer = true;
        }
        if (GameObject.Find("LevelText"))
        {
            GameObject.Find("LevelText").GetComponent<Text>().text = "Level : " + PlayerLevel;
        }
        if (GameObject.Find("GoldText"))
        {
            GameObject.Find("GoldText").GetComponent<Text>().text = "x " + Gold;
        }

    }

    public void LoadPlayer()
    {
        Debug.Log("Player Loaded");
        PlayerData data = SaveSystem.LoadPlayerSkills();
        if (data != null)
        {
            skills.str = data.Str;
            skills.agi = data.Agi;
            skills.sta = data.Sta;
            skills.lck = data.Lck;
            skills.skillpoints = data.SkillPoint;
            skills.Exp = data.Exp;
            skills.Gold = data.Gold;
            skills.PlayerLevel = data.PlayerLevel;
            skills.KickSkill = data.KickSkill;
            skills.PunchSkill = data.PunchSkill;
            skills.KickUpgrade = data.KickUpgrade;
            skills.PunchUpgrade = data.PunchUpgrade;
        }
        else
        {
            newPlayer();
        }
    }

    public void LoadLevelManager()
    {
        Debug.Log("Level Manager Loaded");
        PlayerData data = SaveSystem.LoadPlayerSkills();
        if (data != null)
        {
            levelManager.Level = data.currentStageLevel;
        }
        else
        {
            newPlayer();
        }
    }

    public void newPlayer()
    {
        Debug.Log("Yeni Oyuncu Oluşturuldu");
        str = 0;
        agi = 0;
        sta = 0;
        lck = 0;
        skillpoints = 0;
        Exp = 0;
        Gold = 0;
        PlayerLevel = 1;
        currentStageLevel = 1;
        KickSkill = 0;
        PunchSkill = 0;
        KickUpgrade = 0;
        PunchUpgrade = 0;

        SaveNewPlayer();
        Debug.Log("Yeni Oyuncu Kaydedildi");
    }
}
