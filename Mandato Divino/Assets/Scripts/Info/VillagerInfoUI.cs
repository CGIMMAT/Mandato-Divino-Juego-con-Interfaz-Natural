using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class VillagerInfoUI : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI idText;
    public TextMeshProUGUI ageText;
    public TextMeshProUGUI genderText;
    public TextMeshProUGUI lifeText;
    public TextMeshProUGUI energyText;
    public TextMeshProUGUI relationshipText;

    public void Show(VillagerLogic v)
    {
        gameObject.SetActive(true);

        nameText.text = v.villagerName;
        idText.text = "ID: " + v.id;
        ageText.text = "Edad: " + v.age.ToString();
        genderText.text = "Género: " + v.gender.ToString();
        lifeText.text = "Vida: " + v.lifePoints;
        energyText.text = "Energía: " + v.energyPoints;

        if (v.relationship.inLove)
        {
            relationshipText.text = "Enamorado de ID: " + v.relationship.LoverID;
        }
        else
        {
            relationshipText.text = "Sin pareja";
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}