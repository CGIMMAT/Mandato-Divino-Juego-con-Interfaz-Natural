using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Windows.Speech;


public class WordRegister : MonoBehaviour //Nuevo sistema para leer comandos
{
    KeywordRecognizer keywordRecognizer;

    Dictionary<string, Action> Actions = new Dictionary<string, Action>();
    public CommandList CL;

    void Start()
    {
        Actions = new Dictionary<string, Action>();

        // Asociamos palabras con funciones de CommandList
        //Actions.Add("palabra", CL.funcion)

        keywordRecognizer = new KeywordRecognizer(Actions.Keys.ToArray());
        keywordRecognizer.OnPhraseRecognized += WordRecognized;
        keywordRecognizer.Start();
    }

    private void WordRecognized(PhraseRecognizedEventArgs word)
    {
        Debug.Log("Palabra detectada: " + word.text);

        // Solo actúa si la palabra está en el diccionario
        if (Actions.ContainsKey(word.text))
        {
            Actions[word.text].Invoke();
        }
        // Si no está, no hace nada
    }
}