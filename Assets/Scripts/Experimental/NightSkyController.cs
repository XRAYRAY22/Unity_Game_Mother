using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public struct Entity
{
    public Vector2 position;
    public Vector2 direction;
}


public class NightSkyController : MonoBehaviour
{
    public int width = 800;
    public int height = 800;

    public float speed = 5;
    public float disperseSpeed = 5;
    public float turnAngle = 15;

    public Color background;
    public Color foreground;
    public ComputeShader shader;
    public RenderTexture texture;
    private ComputeBuffer buffer;

    [Range(8, 1000)]
    public int entityCountRoot = 8;
    private int entityCount;

    private Entity[] entities;

    void Awake()
    {
        turnAngle *= Mathf.PI/180;
        entityCount = entityCountRoot * entityCountRoot;

        if (texture == null)
        {
            texture = new RenderTexture(width, height, 24);
            texture.enableRandomWrite = true;
            texture.Create();
        }

        else
        {
            this.width = texture.width;
            this.height = texture.height;
            texture.enableRandomWrite = true;
        }

        entities = new Entity[entityCount];

        int i;
        for (i = 0; i < entityCount; i++)
        {
            entities[i].position.x = (Random.Range(-1.0f, 1.0f) * width / 16) + width / 2;
            entities[i].position.y = (Random.Range(0.0f, 1.0f) * height / 10) + height / 10;

            Vector2 direction = new Vector2();
            direction.x = entities[i].position.x - (width / 2);
            direction.y = entities[i].position.y;

            direction = direction.normalized;
            entities[i].direction = direction;

            //Debug.Log(entities[i].position.x + ", " + entities[i].position.y);
        }

        this.StartOnGPU();
    }

    public void StartOnGPU()
    {
        int entitySize = sizeof(float) * 4;
        Debug.Log(entitySize);
        buffer = new ComputeBuffer(entityCount, entitySize);
        buffer.SetData(entities);

        shader.SetBuffer(0, "entities", buffer);

        shader.SetFloat("width", (float) width);
        shader.SetFloat("height", (float) height);
        shader.SetFloat("speed", speed);
        shader.SetFloat("disperseSpeed", disperseSpeed);
        shader.SetFloat("turnAngle", turnAngle);
        shader.SetInt("entityCountRoot", entityCountRoot);
        Vector4 back = new Vector4(background.r, background.g, background.b, 1);
        shader.SetVector("background", back);
        Vector4 fore = new Vector4(foreground.r, foreground.g, foreground.b, 1);
        shader.SetVector("foreground", fore);


        shader.SetTexture(0, "display", texture);
        shader.SetTexture(1, "display", texture);

        buffer.GetData(entities);
        buffer.Release();
    }

    public void UpdateGPU()
    {
        int entitySize = sizeof(float) * 4;
        buffer = new ComputeBuffer(entityCount, (entitySize));
        buffer.SetData(entities);

        shader.SetBuffer(shader.FindKernel("UpdateEntities"), "entities", buffer);
        shader.SetFloat("time", Time.deltaTime);
        //shader.SetTexture(0, "display", texture);

        // only dispatch the number of entities, then each id will be one entity
        shader.Dispatch(shader.FindKernel("UpdateTexture"), width / 8, height / 8, 1);
        shader.Dispatch(shader.FindKernel("UpdateEntities"), entityCountRoot / 8, entityCountRoot / 8, 1);

        buffer.GetData(entities);
        buffer.Release();
    }

    void Update()
    {
        this.UpdateGPU();
    }

    //void OnDisable()
    //{
    //    // free memory / allow garbage collection
    //    buffer.Release();
    //    buffer = null;
    //}
}
