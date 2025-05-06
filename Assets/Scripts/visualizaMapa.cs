using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class visualiazador : MonoBehaviour
{
    [Header("Referências")]
    public RNG rNG;
    public GameObject NodeButton;
    public Transform nodeParent;

    [Header("Sprites")]
    public Sprite batalhaSprite;
    public Sprite lojaSprite;
    public Sprite eventoSprite;
    public Sprite bauSprite;
    public Sprite chefeSprite;
    public Sprite inicioSprite;

    [Header("Visual")]
    public Vector2 espacocasa = new Vector2(200,150); //espaçamento entre as casas

    void Start()
    {
        GerarMapaVisual();
    }

    void GerarMapaVisual()
    {
        var dadosDoMapa = rNG.GetMapa();


        for(int camada = 0; camada < dadosDoMapa.Count; camada++)
        {
            List<Node> nodes = dadosDoMapa[camada];
            for(int i = 0; i < nodes.Count; i++)
            {
                Node node = nodes[i];

                GameObject newNode = Instantiate(NodeButton, nodeParent);

                float x = camada * espacocasa.x;
                float y = -i * espacocasa.y;

                newNode.GetComponent<RectTransform>().anchoredPosition = new Vector2(x,y);

                Image icon = newNode.GetComponent<Image>();
                icon.sprite = GetSpriteForType(node.tipo);


            }
        }
    }

    Sprite GetSpriteForType(string tipo)
    {
        switch(tipo)
        {
            case "Batalha": return batalhaSprite;
            case "Loja": return lojaSprite;
            case "Evento": return eventoSprite;
            case "Baú": return bauSprite;
            case "Boss": return chefeSprite;
            case "Inicio": return inicioSprite;
            default: return null;
        }
    }
}
