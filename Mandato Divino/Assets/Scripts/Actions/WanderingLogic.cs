using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WanderingLogic : MonoBehaviour //Sistema para que los aldeanos deambulen cuando no tienen nada que hacer
{
    public float moveSpeed = 0.5f; //Su velocidad
    public Vector2 decisionTime = new Vector2(1,3); //Tiempo aleatorio que tardan en cambiar de direccion
    internal float decisionTimer = 0; //Contador de tiempo
    internal Vector3[] moveDirection = new Vector3[] {Vector3.right, Vector3.left, Vector3.up, Vector3.down, Vector3.zero, Vector3.zero, Vector3.zero}; //Las posibles direcciones qeu pueden tomar, solo son rectas, bidimensionales y además se han añadido tres zero para que sea probable que el aldeano no se mueva
    internal int currentDirection; //La dirección que lleva
    Rigidbody2D rb; //La componente de Rigidbody, para que puedan detectar colisiones y no atraviesen objetos solidos

    void Start() //Bucle inicial para decidir l primra dirección que toman
    {
        rb = GetComponent<Rigidbody2D>();
        decisionTimer = Random.Range(decisionTime.x, decisionTime.y);
        ChooseDirection();
    }

    void Update() //Nuevos movimientos
    {
        if (decisionTimer > 0)
        {
            decisionTimer -= Time.deltaTime;
        }
        else
        {
            decisionTimer = Random.Range(decisionTime.x, decisionTime.y);
            ChooseDirection();
        }
    }

    void FixedUpdate() //Calcula las fisicas de los movimientos en intervalos similares a Update
    {
        rb.velocity = moveDirection[currentDirection] * moveSpeed;
    }

    void ChooseDirection() //Función para cambiar semialeatoriamente los movimientos
    {
        currentDirection = Mathf.FloorToInt(Random.Range(0, moveDirection.Length));
    }

    void OnCollisionEnter2D(Collision2D col) //Detecta las colisiones con los BoxCollidre de los objetos del mapa y fuerza a cambiar de dirección
    {
        rb.velocity = Vector2.zero;
        ChooseDirection();
    }
}