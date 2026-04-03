using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NameManager : MonoBehaviour
{
    public static NameManager Instance;

    private List<string> maleNames = new List<string>();
    private List<string> femaleNames = new List<string>();

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        LoadNames();
    }

    void LoadNames()
    {
        TextAsset maleFile = UnityEngine.Resources.Load<TextAsset>("maleName");
        TextAsset femaleFile = UnityEngine.Resources.Load<TextAsset>("femaleName");

        if (maleFile != null)
        {
            maleNames = new List<string>(maleFile.text.Split('\n'));
            maleNames.RemoveAll(name => string.IsNullOrWhiteSpace(name));
        }
        else Debug.LogWarning("male_names.txt no encontrado en Resources");

        if (femaleFile != null)
        {
            femaleNames = new List<string>(femaleFile.text.Split('\n'));
            femaleNames.RemoveAll(name => string.IsNullOrWhiteSpace(name));
        }
        else Debug.LogWarning("female_names.txt no encontrado en Resources");
    }

    public string GetRandomName(Gender gender)
    {
        if (gender == Gender.Male && maleNames.Count > 0)
            return maleNames[Random.Range(0, maleNames.Count)];
        if (gender == Gender.Female && femaleNames.Count > 0)
            return femaleNames[Random.Range(0, femaleNames.Count)];
        return "Villager";
    }
}
