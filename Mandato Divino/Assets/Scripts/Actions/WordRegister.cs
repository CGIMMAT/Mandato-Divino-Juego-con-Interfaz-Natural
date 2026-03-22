using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Windows.Speech;
using TMPro; //solo para la prueba, luego se eliminará si no es necesario

public class WordRegister : MonoBehaviour
{
    //Código importado para recibir inpust del microfono y actuar en base a las palabras dichas

    KeywordRecognizer KeywordRecognizer;
    public TextMeshProUGUI text;
    int wordCounter = 0;

    //Listado que usaremos para almacenar las palabras clave con su respuesta correspondiente
    Dictionary<string, Action> wordToAction;
    void Start()
    {
        wordToAction = new Dictionary<string, Action>(); //Se instancia el diccionario y se añaden las palabras clave
        wordToAction.Add("hola", Register);

        KeywordRecognizer = new KeywordRecognizer(wordToAction.Keys.ToArray()); //Se crea el reconocedor para esas palabras clave
        KeywordRecognizer.OnPhraseRecognized += WordRecognized; //Se le asigna la función para que actue al reconocer las palabras
        KeywordRecognizer.Start(); //Se inicializa
    }

    private void WordRecognized(PhraseRecognizedEventArgs word)
    {
        Debug.Log("Se ha registrado la palabbra: " + word.text); //Registramos que se ha recibido bien la palabra
        wordToAction[word.text].Invoke(); //Se invoca la acción asociada a la palabra dentro del diccionario
    }

    private void Register()
    {
        wordCounter++;
        text.text = "Has dicho Hola " + wordCounter + " veces";
    }
}
