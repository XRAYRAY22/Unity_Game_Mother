using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Cell
{
    public Vector2 position;
    public float A;
    public float B;
}

public class StarlightController : MonoBehaviour
{
    [SerializeField] private ComputeShader shader;
    [SerializeField] private Texture2D initialState;
    private Texture2D initialStateEdit;
    [SerializeField] private bool randomise;
    [HideInInspector] public RenderTexture texture;
    public RenderTexture output;
    private RenderTexture map;
    [Space(4)] [SerializeField] private Material material;
    [SerializeField] private Vector2 dimensions;
    //private ComputeBuffer buffer;
    //private Cell[] cells;

    [SerializeField] private Color colourA;
    [SerializeField] private Color colourB;

    [Range(0.0f, 1.0f)] [SerializeField] private float diffA;
    [Range(0.0f, 1.0f)] [SerializeField] private float diffB;
    [Range(1, 5)] [SerializeField] private int sampleRadius;

    [Range(0.0f, 0.1f)] [SerializeField] private float feed;
    [Range(0.0f, 0.1f)] [SerializeField] private float kill;
    [Range(0.0f, 1.0f)] [SerializeField] private float speed;

    void Awake()
    {
        if (texture != null) texture.Release();
        if (initialState == null) initialState = Texture2D.linearGrayTexture;

        texture = new RenderTexture((int) dimensions.x, (int) dimensions.y, 24);
        texture.enableRandomWrite = true;
        texture.Create();

        map = new RenderTexture((int) dimensions.x, (int) dimensions.y, 24);
        map.enableRandomWrite = true;
        map.Create();

        texture.filterMode = FilterMode.Point;
        map.filterMode = FilterMode.Point;

        initialStateEdit = new Texture2D((int) dimensions.x, (int) dimensions.y);
        Graphics.CopyTexture(initialState, initialStateEdit);

        if (randomise)
        {
            for (int x = 0; x < dimensions.x; x++)
            {
                for (int y = 0; y < dimensions.y; y++)
                {
                    if (initialStateEdit.GetPixel(x, y).g > 0.1f)
                    {
                        initialStateEdit.SetPixel(x, y, new Color(Random.Range(0.1f, 1.0f), Random.Range(0.1f, 1.0f), 0.0f, 0.0f));
                    }
                }
            }
        }
        initialStateEdit.Apply();

        Graphics.Blit(initialStateEdit, map);

        material.mainTexture = texture;
        material.EnableKeyword("_EMISSION");
        material.SetColor("_EmissionColor", colourB * 2);
        material.SetTexture("_EmissionMap", texture);
        material.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;


        // initialise cells
        //cells = new Cell[texture.width * texture.height];

        //for (int i = 0; i < texture.width; i++)
        //{
        //    for (int j = 0; j < texture.height; j++)
        //    {
        //        cells[i + j * texture.width].position = new Vector2(i, j);
        //        cells[i + j * texture.width].A = initialState.GetPixel(i, j).r;
        //        cells[i + j * texture.width].B = 1.0f - initialState.GetPixel(i, j).r;
        //    }
        //}

        // initialise setting values on GPU
        shader.SetVector("colourA", colourA);
        shader.SetVector("colourB", colourB);

        shader.SetFloat("diffA", diffA);
        shader.SetFloat("diffB", diffB);

        shader.SetFloat("feed", feed);
        shader.SetFloat("kill", kill);
        shader.SetFloat("speed", speed);
        shader.SetInt("sampleRadius", sampleRadius);

        shader.SetTexture(0, "display", texture);
        shader.SetTexture(0, "map", map);
    }

    // Update is called once per frame
    void Update()
    {
        this.UpdateGPU();
        Graphics.Blit(texture, output);
    }

    void UpdateGPU()
    {
        //int entitySize = 2 * sizeof(float) + 2 * sizeof(int);
        //buffer = new ComputeBuffer(texture.height * texture.width, entitySize);
        //buffer.SetData(cells);
        //shader.SetBuffer(0, "cells", buffer);

        shader.SetFloat("time", Time.deltaTime);

        shader.Dispatch(shader.FindKernel("Update"), texture.width / 8, texture.height / 8, 1);

        //buffer.GetData(cells);
        //buffer.Release();
    }
}
