using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeLogic : BuildingsLogic
{
    public List<int> residents = new List<int>();

    public void Initialize(HomeData data)
    {
        base.Initialize(data);
    }

    public bool CanRegister(VillagerLogic v)
    {
        if (residents.Count == 0) return true;

        foreach (int id in residents)
        {
            VillagerLogic other = FindVillagerByID(id);
            if (other == null) continue;
            if (other.relationship != null && other.relationship.inLove && other.relationship.LoverID == v.id) return true;

            if (v.age == Age.Niño)
            {
                if (v.fatherID == id || v.motherID == id) return true;
            }
            if (other.age == Age.Niño)
            {
                if (other.fatherID == v.id || other.motherID == v.id) return true;
            }
        }

        return false;
    }

    public void RegisterVillager(VillagerLogic v)
    {
        if (residents.Contains(v.id)) return;

        if (CanRegister(v))
        {
            residents.Add(v.id);
            v.currentHome = this;
        }
    }

    public void RemoveVillager(VillagerLogic v)
    {
        if (residents.Contains(v.id))
        {
            residents.Remove(v.id);
            v.currentHome = null;
        }
    }

    VillagerLogic FindVillagerByID(int id)
    {
        foreach (var v in FindObjectOfType<VillagerSpawner>().allVillagers)
        {
            if (v.id == id) return v;
        }
        return null;
    }
}