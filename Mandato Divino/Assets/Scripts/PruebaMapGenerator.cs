using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PruebaMapGenerator : MonoBehaviour
{
    float [,] map; //Almacena los valores del algoritmo
    public int SizeXY; //Determina el tamaño total del mapa, asumiendo que es un cuadrado dde dimensiones X^2
    public Tilemap tilemap; //Referencia al tilemap del grid
    public Tile[] tiles; //Almacena los distintos tipos de tiles
    public float[] levels; //Se usa para determinar los tiles asociados a cada valor del algoritmo
    public int InValues; //Valores iniciales para las esquinas del mapa

    private void Start()
    {
        map = new float[SizeXY, SizeXY];
        map[0,0] = InValues;
        map[SizeXY -1, SizeXY - 1] = InValues;
        map[0, SizeXY - 1] = InValues;
        map[SizeXY - 1, 0] = InValues;
        GenMap();
    }

    void GenMap()
    {
        float maxValue = 0;
        DiamondSquareAlgorithm(SizeXY);

        for (int y = 0; y < SizeXY; y++)
        {
            for (int x = 0; x < SizeXY; x++)
            {
                if (map[x,y] > maxValue)
                {
                    maxValue = map[x,y];
                }
            }
        }

        for (int y = 0; y < SizeXY; y++)
        {
            for (int x = 0; x < SizeXY; x++)
            {
                float v = 0;
                if (map[x,y] > 0)
                {
                    v = map[x,y]/maxValue;
                }
                tilemap.SetTile(new Vector3Int(x,y,0), SelectTile(v));
            }
        }
    }

    Tile SelectTile(float p)
    {
        for (int i = 0; i < levels.Length; i++)
        {
            if (p <= levels[i]) return tiles[i];
        }
        return null;
    }

    void DiamondSquareAlgorithm(int size) //Función que aplica el algoritmo
    {
        int half = size/2;
        if (half < 1) return;

        for (int y = half; y < SizeXY; y += size)
        {
            for (int x = half; x < SizeXY; x += size)
            {
                DiamondStep(x, y, half);
            }
        }

        DiamondSquareAlgorithm(half); // LLamamos a la función dentro de si misma para que sea recursiva, se aplicará hasta que el tamaño de half sea de menos de una casilla
    }

    void DiamondStep(int x, int y, int size) //Aplica los cambios de valores entre tiles para el algoritmo
    {
        float value = 0.0f;
        value += map[x-size, y-size]; //Esquina inferior izquierda
        value += map[x+size, y-size]; //Esquina inferior derecha
        value += map[x-size, y+size]; //Esquina superior izquierda
        value += map[x+size, y+size]; //Esquina superior derecha
        value += Random.Range(0, size * 2) - size; //Añade valores aleatorios para generar variedad en el diseño del mapa
        value /= 4.0f;
        map[x, y] = value;

        SquareStep(x - size, y, size);
        SquareStep(x + size, y, size);
        SquareStep(x, y - size, size);
        SquareStep(x, y + size, size);
    }

    void SquareStep(int x, int y, int size) //Aplica los cambios de valores en tiles de los puntos medios del cuadrado
    {
        float value = 0.0f;
        int count = 0;
        if (x - size >= 0)
        {
            value += map[x - size, y];
            count++;
        }
        if (x + size < SizeXY)
        {
            value += map[x + size, y];
            count++;
        }
        if (y - size >= 0)
        {
            value += map[x, y - size];
            count++;
        }
        if (y + size < SizeXY)
        {
            value += map[x, y + size];
            count++;
        }
        value += Random.Range(0, size * 2) - size;
        value /= count;
        map[x,y] = value;
    }
}
