using System;
using System.Collections.Generic;
using UnityEngine;


public class RNG : MonoBehaviour
{
    public int qtdlvl = 10; //Quantidade de niveís
    public int minEsc = 2; // Mininmo de caminhos possiveis
    public int maxEsc = 3; // Maximo de caminhos possiveis

    private List<List<Node>> camadas = new List<List<Node>>();

    string TipoAleatorio()
    {
        string[] tipos = {"Batalha", "Loja", "Evento", "Baú"};
        return tipos[UnityEngine.Random.Range(0, tipos.Length)];
    }

    void mostraMapa()
    {
        for(int i = 0; i < camadas.Count; i++)
        {
            string camada = "Camada" + i + ":";
            foreach(Node node in camadas[i])
            {
                camada += node.tipo + "->";
            }
            Debug.Log(camada);
        }
    }

    void GeraMapa()
    {
        Node inicial = new Node("Inicio"); // Casa inicial
        camadas.Add(new List<Node>{inicial}); // coloco a casa inicial como primeira no mapa

        Node nodacamadaanterior = inicial;

        for(int i = 0; i < qtdlvl; i++)
        {
            int caminhos = UnityEngine.Random.Range(minEsc, maxEsc + 1); //Numero aleatório de caminhos
            List<Node> camadaatual = new List<Node>(); // Lista pra armazenar esses caminhos

            for(int j = 0; j < caminhos; j++)
            {
                string tipo = TipoAleatorio(); // Gero um tipo aleaotorio
                Node novono = new Node(tipo); // Crio uma casa com esse tipo
                camadaatual.Add(novono); //Adiciono essa casa
                nodacamadaanterior.conexoes.Add(novono); // ligo ela com a casa anterior
            }

            camadas.Add(camadaatual); //adiciono ela no mapa
            nodacamadaanterior = camadaatual[UnityEngine.Random.Range(0, camadaatual.Count)]; // Pega e escolhe um caminho aleotorio para seguir
        }

        Node chefe = new Node("Boss"); // criando a casa final
        nodacamadaanterior.conexoes.Add(chefe); // conecto a casa do chefe a ultima casa
        camadas.Add(new List<Node>{chefe});

        Debug.Log("Mapa Gerado");
        mostraMapa();
    }
    
    void Start()
    {
        GeraMapa(); // gera o mapa
    }

    // Update é chamdo uma vez por frme
    void Update()
    {
        
    }

    public List<List<Node>> GetMapa()
    {
        return camadas;
    }
}

public class Node
{
    public string tipo; //Tipo da casa (batalha, loja, etc.)
    public List<Node> conexoes; //lista das próximas casas conectadas

    public Node(String tipo)
    {
        this.tipo = tipo;
        conexoes = new List<Node>();
    }
}