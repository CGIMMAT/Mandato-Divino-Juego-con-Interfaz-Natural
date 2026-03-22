using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DragCamera : MonoBehaviour //Script sencillo para controlar el movimiento de camara por el jugador
{
    private Vector3 dragOrigin;
    private bool isDragging = false;
    public float dragSpeed = 5.0f; //Velocidad a la que se movera la camara al arrastrar
    public MapGenerator MG; //Referencia al generador del mapa para poder limitar el movimiento solo a las casillas generadas
    public Tilemap tilemap; //Referencia al tilemap en sí
    private float minX, maxX, minY, maxY;

    public void StartCamera()
    {
        BoundsInt bounds = tilemap.cellBounds;

        //Metemos un sistema para que la camara no pueda ver nada más alla de las casillas
        float camHeight = Camera.main.orthographicSize;
        float camWidth = camHeight * Camera.main.aspect;

        minX = bounds.xMin + camWidth;
        maxX = bounds.xMax - camWidth;
        minY = bounds.yMin + camHeight;
        maxY = bounds.yMax - camHeight;

        //Y otro para que al empezar la partida estemos en el centro del mapa
        //Quitar este cuando haya un sistem de spawneo de aldeanos
        float centerX = (bounds.xMin + bounds.xMax) / 2f;
        float centerY = (bounds.yMin + bounds.yMax) / 2f;
        transform.position = new Vector3(centerX, centerY, transform.position.z);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Comprueba si se ha hecho click
        {
            dragOrigin  = Input.mousePosition;
            isDragging = true;
        }

        if (Input.GetMouseButtonUp(0)) //Comprueba si ya no hay click 
        {
            isDragging = false;
        }

        if (isDragging) //Si hay un click, se calcula el movimiento de la camara en base al inverso del ratón
        {
            Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - dragOrigin);
            Vector3 move = new Vector3(-pos.x * dragSpeed, -pos.y * dragSpeed, 0);
            Vector3 newPos = transform.position + move;

            //Se limita el movimiento a los bordes del mapa
            newPos.x = Mathf.Clamp(newPos.x, minX, maxX);
            newPos.y = Mathf.Clamp(newPos.y, minY, maxY);

            transform.position = newPos;
            dragOrigin = Input.mousePosition;
        }
    }
}
