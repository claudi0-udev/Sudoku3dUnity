using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GenerateBoard : MonoBehaviour
{
    public int size = 9;
    public GameObject quadPrefab;

    public Casilla[] casilla;

    public Color[] color;
    public Color errorColor;

    public static Casilla[,] row, column, block;

    public Font arialFont;

    void Awake()
    {
        casilla = new Casilla[size * size];


        row = new Casilla[size, size];
        column = new Casilla[size, size];
        block = new Casilla[size, size];        

        PositionQuads();
        GetRows();
        GetColumns();
        GetBlocks();

        SetColors();


        SetValues();
    }

    void PositionQuads()
    {
        int c = 0;
        for(int y = 0; y < size; y++)
        {
            for(int x = 0; x < size; x++)
            {
                GameObject q = Instantiate(quadPrefab, new Vector3(x - size/2, y - size/2, transform.position.z), Quaternion.identity, this.transform);
                q.name = $"{x}{y}";
                casilla[c] = q.AddComponent<Casilla>();
                c++;
            }
        }
    }

    public void SetColors()// aplicable solo a 9 filas * columnas
    {
        int c = 0;
        int rootSqrt = (int)(size / Mathf.Sqrt(size));
        
        for (int i = 0; i < (size*size)-size * 2 ; i += size/rootSqrt)
        {
            if (i == size) i += size*2;
            if (i == size * (rootSqrt + 1)) i += size * 2;
            casilla[i].color = color[c];
            casilla[i + 1].color = color[c];
            casilla[i + 2].color = color[c];
            casilla[i + size].color = color[c];
            casilla[i + size + 1].color = color[c];
            casilla[i + size + 2].color = color[c];
            casilla[i + (size * 2)].color = color[c];
            casilla[i + (size * 2) + 1].color = color[c];
            casilla[i + (size * 2) + 2].color = color[c];
            c++;
        }                
            
        

        foreach(Casilla ca in casilla)
        {
            if(!ca.error)
                ca.GetComponent<MeshRenderer>().material.SetColor("_BaseColor", ca.color);
            else
                ca.GetComponent<MeshRenderer>().material.SetColor("_BaseColor", errorColor);
        }
    }

    void GetRows()
    {
        int count = 0;
        for(int x = 0; x < size; x++)
        {
            //print($"ROW {count/9}");
            for (int y = 0; y < size; y++)
            {
                row[x, y] = casilla[count];
                //Debug.Log(casilla[count].name);
                count += 1;
            }
            //print("----------");
        }
    }

    void GetColumns()
    {
        for (int x = 0; x < size; x++)
        {
            //print($"COLUMN {x}");
            for (int y = 0; y < size; y++)
            {
                column[x, y] = casilla[x + (y * size)];
                //Debug.Log(casilla[x + (y * size)].name);
                
            }
            //print("----------");
        }
    }

    void GetBlocks()
    {
        int c = 0;
        int rootSqrt = (int)(size / Mathf.Sqrt(size));

        for (int i = 0; i < (size * size) - size * 2; i += size / rootSqrt)
        {
            if (i == size) i += size * 2;
            if (i == size * (rootSqrt + 1)) i += size * 2;
            block[c, 0] = casilla[i];
            block[c, 1] = casilla[i + 1];
            block[c, 2] = casilla[i + 2];
            block[c, 3] = casilla[i + size];
            block[c, 4] = casilla[i + size + 1];
            block[c, 5] = casilla[i + size + 2];
            block[c, 6] = casilla[i + (size * 2)];
            block[c, 7] = casilla[i + (size * 2) + 1];
            block[c, 8] = casilla[i + (size * 2) + 2];
            c++;
        }

        /*for(int x = 0; x < size; x++)
        {
            print($"BLOCK {x}");
            for(int y = 0; y < size; y++)
            {
                Debug.Log(block[x, y].name);
            }
            print("-----------");
        }*/
    }

    public void SetValues()
    {
        /*foreach (Casilla ca in GenerateBoard.row)
        {
            ca.SetValue(Random.Range(1, 10));
        }*/

        //verificar si los numeros random estan bien, rows, blocks, columns
        //VerifyNumbersAtStart();
        NumberGenerator();//Prueba de sudoku con numeros controlados
        for(int x = 0; x < 9; x++)
        {
            for(int y = 0; y < 9; y++)
            {
                //row[x, y].SetValue(grid[x, y]);
                int n = UnityEngine.Random.Range(1, 10);
                if(n+1 >= grid[x, y] && n-1 <= grid[x, y])
                {
                    row[x, y].SetValue(grid[x, y]);
                }
            }
        }
        SetCasillasNoWritable();

    }

    void VerifyNumbersAtStart()
    {
        VerifyRows();
        VerifyColumns();
        VerifyBlocks();

        SetCasillasNoWritable();
    }

    void VerifyRows()
    {
        for (int x = 0; x < size; x++)
        {
            List<Casilla> r = new List<Casilla>();
            for (int y = 0; y < size; y++)
            {
                if (GenerateBoard.row[x, y].value != 0)
                    r.Add(GenerateBoard.row[x, y]);
            }

            r = r.OrderBy(n => n.GetComponent<Casilla>().value).ToList();

            for (int i = 1; i < r.Count; i++)
            {

                if (r[i].value == r[i - 1].value)
                {
                    r[i].SetValue(0);
                    r[i - 1].SetValue(0);
                }
            }

        }
    }

    void VerifyColumns()
    {
        for (int x = 0; x < size; x++)
        {
            List<Casilla> r = new List<Casilla>();
            for (int y = 0; y < size; y++)
            {
                if (GenerateBoard.column[x, y].value != 0)
                    r.Add(GenerateBoard.column[x, y]);
            }

            r = r.OrderBy(n => n.GetComponent<Casilla>().value).ToList();

            for (int i = 1; i < r.Count; i++)
            {

                if (r[i].value == r[i - 1].value)
                {
                    r[i].SetValue(0);
                    r[i - 1].SetValue(0);
                }
            }

        }
    }

    void VerifyBlocks()
    {
        for (int x = 0; x < size; x++)
        {
            List<Casilla> r = new List<Casilla>();
            for (int y = 0; y < size; y++)
            {
                if (GenerateBoard.block[x, y].value != 0)
                    r.Add(GenerateBoard.block[x, y]);
            }

            r = r.OrderBy(n => n.GetComponent<Casilla>().value).ToList();

            for (int i = 1; i < r.Count; i++)
            {

                if (r[i].value == r[i - 1].value)
                {
                    r[i].SetValue(0);
                    r[i - 1].SetValue(0);
                }
            }

        }
    }

    void SetCasillasNoWritable()
    {
        for (int x = 0; x < size; x++)
        {
            List<Casilla> r = new List<Casilla>();
            for (int y = 0; y < size; y++)
            {
                if (GenerateBoard.row[x, y].value != 0)
                    r.Add(GenerateBoard.row[x, y]);
            }

            r = r.OrderBy(n => n.GetComponent<Casilla>().value).ToList();

            for (int i = 0; i < r.Count; i++)
            {
                r[i].isWritable = false;
                //set font arial
                r[i].SetFeatures(arialFont);
            }

        }
    }

    //Test generador de sudoku automatico casillas OK

    static int[,] grid = new int[9,9];
    static string s;
    static void Init(ref int[,] grid)
    {
        for(int i = 0; i < 9; i++)
        {
            for(int j = 0; j < 9; j++)
            {
                grid[i, j] = (i * 3 + i / 3 + j) % 9 + 1;
            }
        }
    }

    static void Draw(ref int[,] grid, out string _s)
    {
        for(int x = 0; x < 9; x++)
        {
            for(int y = 0; y < 9; y++)
            {
                s += grid[x, y].ToString() + " ";
            }
        }
        s += Environment.NewLine;
        //Console.WriteLine(s);
        Debug.Log(s);
        _s = s;
        s = "";
    }

    static void ChangeTwoCell(ref int[,] grid, int findValue1, int findValue2)
    {
        int xParam1, yParam1, xParam2, yParam2;
        xParam1 = xParam2 = yParam1 = yParam2 = 0;

        for(int i = 0; i < 9; i+=3)
        {
            for(int k = 0; k < 9; k+=3)
            {
                for(int j = 0; j < 3; j++)
                {
                    for(int z = 0; z < 3; z++)
                    {
                        if(grid[i + j, k + z] == findValue1)
                        {
                            xParam1 = i + j;
                            yParam1 = k + z;
                        }
                        if(grid[i + j, k + z] == findValue2)
                        {
                            xParam2 = i + j;
                            yParam2 = k + z;
                        }

                    }
                }
                grid[xParam1, yParam1] = findValue2;
                grid[xParam2, yParam2] = findValue1;
            }
        }
    }

    static void UpToDate(ref int[,] grid, int shuffleLevel)
    {
        for(int repeat = 0; repeat < shuffleLevel; repeat++)
        {
            System.Random rand = new System.Random(Guid.NewGuid().GetHashCode());
            System.Random rand2 = new System.Random(Guid.NewGuid().GetHashCode());
            ChangeTwoCell(ref grid, rand.Next(1, 9), rand2.Next(1, 9));
        }
    }
    void NumberGenerator()
    {
        string c;
        Init(ref grid);
        UpToDate(ref grid, 10);
        Draw(ref grid, out c);        
    }
}
