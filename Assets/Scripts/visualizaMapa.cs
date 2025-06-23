using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;



public class visualiazador : MonoBehaviour
{
    private Node noSelecionadoAtual = null;
    private Dictionary<Node, Button> noToButton = new Dictionary<Node, Button>();

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
    public Vector2 espacocasa = new Vector2(100, 100); //espaçamento entre as casas

    [Header("Linha")]
    public GameObject LinhaPrefab;
    public Transform linhaParent;
    

    // todas as declarações acima são pra referenciar o que ele vai usar(ex: quai script, qual prefab, a distancia entre as casas, etc.)

    //associa a logica ao visual
    private Dictionary<Node, GameObject> nodeToObject = new Dictionary<Node, GameObject>();


    void Start()
    {
        GerarMapaVisual();
    }

    void GerarMapaVisual()
    {
        var dadosDoMapa = rNG.GetMapa(); //pega os dados do mapa gerado

        // Cria o visual baseado em cada camada 
        for (int camada = 0; camada < dadosDoMapa.Count; camada++)
        {
            List<Node> nodes = dadosDoMapa[camada];
            for (int i = 0; i < nodes.Count; i++)
            {
                Node node = nodes[i];

                GameObject newNode = Instantiate(NodeButton, nodeParent);

                nodeToObject[node] = newNode;

                float x = camada * espacocasa.x;
                float y;
                if (node.tipo == "Inicio")
                {
                    int nostotais = dadosDoMapa[1].Count;
                    y = -((nostotais - 1) * espacocasa.y) / 2f;
                }
                else if (node.tipo == "Boss")
                {
                    int nostotaisAnte = dadosDoMapa[dadosDoMapa.Count - 2].Count;
                    y = -((nostotaisAnte - 1) * espacocasa.y) / 2f;
                }
                else
                {
                    y = -i * espacocasa.y;
                }

                newNode.GetComponent<RectTransform>().anchoredPosition = new Vector2(x, y);

                Image icon = newNode.GetComponent<Image>();
                icon.sprite = GetSpriteForType(node.tipo);

                Button botao = newNode.GetComponent<Button>();
                if (botao != null)
                {
                    noToButton[node] = botao;
                    botao.onClick.RemoveAllListeners();

                    Node capturaNode = node;
                    botao.onClick.AddListener(() => onNodeClick(capturaNode));

                    botao.interactable = false;
                }

                nodeToObject[node] = newNode;
            }
        }

        // conecta as casas com linhas
        foreach (var camada in rNG.GetMapa())
        {
            foreach (var node in camada)
            {
                GameObject origemGO;
                if (!nodeToObject.TryGetValue(node, out origemGO)) continue;

                foreach (Node destino in node.conexoes)
                {
                    GameObject destinoGO;
                    if (!nodeToObject.TryGetValue(destino, out destinoGO)) continue;

                    CriarLinhaEntre(origemGO.GetComponent<RectTransform>(), destinoGO.GetComponent<RectTransform>());
                }
            }
        }
        Node inicio = dadosDoMapa[0][0];
        AttNoClicavel(inicio);
    }

    void AttNoClicavel(Node noBase)
    {
        foreach (var par in noToButton)
        {
            par.Value.interactable = false;
        }

        foreach (Node conexao in noBase.conexoes)
        {
            if (noToButton.TryGetValue(conexao, out Button botao))
            {
                botao.interactable = true;
            }
        }
    }

    void CriarLinhaEntre(RectTransform origem, RectTransform destino)
    {
        GameObject Linha = Instantiate(LinhaPrefab, linhaParent);
        RectTransform rt = Linha.GetComponent<RectTransform>();

        Vector2 posOrigem = origem.anchoredPosition;
        Vector2 posDestino = destino.anchoredPosition;
        Vector2 direcao = posDestino - posOrigem;
        float comprimento = direcao.magnitude;

        rt.sizeDelta = new Vector2(5, comprimento); // largura e altura da linha
        rt.anchoredPosition = posOrigem + direcao / 2; // posição central entre os nós
        rt.rotation = Quaternion.FromToRotation(Vector3.up, direcao); // rotaciona para apontar pro destino

    }

    void onNodeClick(Node node)
    {
        Debug.Log("Clicou em:" + node.tipo);
        noSelecionadoAtual = node;
        AttNoClicavel(node);

        switch (node.tipo)
        {
            case "Batalha":
                SceneManager.LoadScene("battleScene");
                break;
            case "Loja":
                SceneManager.LoadScene("shopScene");
                break;
            case "Evento":
                SceneManager.LoadScene("eventScene");
                break;
            case "Baú":
                SceneManager.LoadScene("chestScene");
                break;
            case "Boss":
                SceneManager.LoadScene("bossScene");
                break;
            default:
                Debug.LogWarning("Tipo de Casa Inválida");
                break;


        }
    }

    Sprite GetSpriteForType(string tipo)
    {
        switch (tipo)
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
