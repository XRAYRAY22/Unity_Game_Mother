using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//https://www.youtube.com/watch?v=bG0uEXV6aHQ
public class PerlinNoise: MonoBehaviour{
    public int width = 256;
    public int height = 256;
    public float scale = 20f;
    public float offsetX;
    public float offsetY;

    void Start(){
        Renderer renderer = GetComponent<Renderer>();
        renderer.material.mainTexture= GenerateTexture();
        offsetX = Random.Range(0f,100f);
        offsetY = Random.Range(0f,100f);
    }

    Texture2D GenerateTexture(){
        Texture2D texture = new Texture2D(width, height);

        for (int i = 0; i < width; i++){
            for (int j = 0; j < height; j++){
                Color color = CalculateColor(i,j);
                texture.SetPixel(i,j,color);
                
            }
        }
        texture.Apply();
        return texture;

    }

    public Color CalculateColor(int x, int y){
        float xCoord = (float)x /width * scale + offsetX;
        float yCoord = (float)y / height * scale + offsetY;
        float sample = Mathf.PerlinNoise(xCoord,yCoord);
        return new Color(sample,sample,sample);
    }
}
