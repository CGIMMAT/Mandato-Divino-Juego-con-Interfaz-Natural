using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapGenerator : MonoBehaviour
{
    float [,] map; //Almacena los valores del algoritmo
    public int SizeXY; //Determina el tamaño total del mapa, asumiendo que es un cuadrado dde dimensiones X^2
    public Tilemap tilemap; //Referencia al tilemap del grid
    public PrefabTile[] tiles; //Almacena los distintos tipos de tiles
    public float[] levels; //Se usa para determinar los tiles asociados a cada valor del algoritmo
    public int InValues; //Valores iniciales para las esquinas del mapa
    int border = 10; //Numero de casillas alrdedor del mapa que siempre serán de agua

    public ResourceGenerator RG; // Invoca la función de generación de recursos
    public DragCamera DC; //Invoca la función para inicializar correctamente la camara

    private void Start()
    {
        map = new float[SizeXY, SizeXY];
        map[0,0] = InValues;
        map[SizeXY -1, SizeXY - 1] = InValues;
        map[0, SizeXY - 1] = InValues;
        map[SizeXY - 1, 0] = InValues;
        GenMap();
        RG.Initialize();
        DC.StartCamera();
    }

    void GenMap() //La función que genera el mappa en base al algoritmo
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

        for (int y = 0; y < SizeXY; y++) //Metodo empleado para que el mapa cuente con un borde de casillas de agua y que la distribución sea másvariada pero lógica
        {
            for (int x = 0; x < SizeXY; x++)
            {
                float v = SmoothValue(x, y) / maxValue;
                float noise = Mathf.PerlinNoise(x * 0.05f, y * 0.05f);
                v = v * 0.6f + noise * 0.4f; 

                float dx = (x - SizeXY / 2f) / (SizeXY / 2f);
                float dy = (y - SizeXY / 2f) / (SizeXY / 2f);
                float distance = Mathf.Sqrt(dx * dx + dy * dy);
                v -= distance * 0.3f; 

                int distBorder = Mathf.Min(x, y, SizeXY - 1 - x, SizeXY -1 - y); //Se calcula la distancia al borde del mapa

                if (distBorder < border)
                {
                    float factor = distBorder / (float)border;
                    v = Mathf.Lerp(0f, v, factor);
                }

                v = Mathf.Clamp01(v);
                PrefabTile tile = SelectTile(v);
                Vector3Int pos = new Vector3Int(x, y, 0);
                tilemap.SetTile(pos, tile);

                //Añadi aquí sistema de generación de prefabs en tiles 
            }
        }
    }

    float SmoothValue(int x, int y) //Esta función suaviza los valores que se asignan a las casillas para que esta sea más variad pero lógica
    {
        float total = 0f;
        int count = 0;

        for (int oy = -1; oy <= 1; oy++)
        {
            for (int ox = -1; ox <= 1; ox++)
            {
                int nx = x + ox;
                int ny = y + oy;

                if (nx >= 0 && nx < SizeXY && ny >= 0 && ny < SizeXY)
                {
                    total += map[nx, ny];
                    count++;
                }
            }
        }

        return total/count;
    }

    PrefabTile SelectTile(float p) //Se usa para asignar un tile a cada casilla del mapa
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
