using System;
using System.Collections.Generic;
using UnityEngine;


public class RNG : MonoBehaviour
{
    public int qtdlvl = 10; //Quantidade de niveís
    public int minEsc = 2; // Mininmo de caminhos possiveis
    public int maxEsc = 3; // Maximo de caminhos possiveis

    private List<List<Node>> camadas = new List<List<Node>>();

    // limites de qntas casas de cada tipo pode ter
    private Dictionary<string, int> limitesDoMapa = new Dictionary<string, int>
    {
        {"Loja", 2},
        {"Baú", 2},
        {"Evento", 6},
        {"Batalha", 6}
    };

    // quantas casas ja foram usadas
    private Dictionary<string, int> tiposUsados = new Dictionary<string, int>();

    // funçao em desuso devido a uma função melhor e mais balanceada
    /*string TipoAleatorio()
    {
        string[] tipos = {"Batalha", "Loja", "Evento", "Baú"};
        return tipos[UnityEngine.Random.Range(0, tipos.Length)];
    }*/

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
        camadas.Clear();
        tiposUsados.Clear();

        Node inicial = new Node("Inicio"); // Casa inicial
        List<Node> camadaAnterior = new List<Node>{ inicial }; // A primeira casa é o inicio (óbvio ne poha kk)
        camadas.Add(camadaAnterior); // Adiciona no mapa

        for(int i = 0; i < qtdlvl; i++)
        {
            int caminhos = UnityEngine.Random.Range(minEsc, maxEsc + 1); //Numero aleatório de caminhos
            List<Node> camadaatual = new List<Node>(); // Lista pra armazenar esses caminhos

            for(int j = 0; j < caminhos; j++)
            {
                string tipo;
                int tentativa = 0;

                do{
                    tipo = TipoAleatorioMelhor();
                    tentativa++;

                    if(tentativa > 10)
                    {
                        tipo = "Batalha";
                        break;
                    }
                }while(limitesDoMapa.ContainsKey(tipo)&& tiposUsados.ContainsKey(tipo) && tiposUsados[tipo] >= limitesDoMapa[tipo]);

                if(!tiposUsados.ContainsKey(tipo))
                    tiposUsados[tipo] = 0;

                tiposUsados[tipo]++;

                Node novoNo = new Node(tipo);
                camadaatual.Add(novoNo);
            }

            if(i == 0)
            {
                foreach(Node noAtual in camadaatual)
                {
                    camadaAnterior[0].conexoes.Add(noAtual);
                }
            }else {
            // conequita com com as casa anteriores
            foreach(Node noAnterior in camadaAnterior)
                {
                    //Quantas conexoes vai ter \/
                    int conexoes = UnityEngine.Random.Range(1, camadaatual.Count + 1);

                    HashSet<int> usados = new HashSet<int>(); // Evita ter mais de uma conexao

                    for(int c = 0; c < conexoes; c++)
                    {
                        int index;
                        do{
                            index = UnityEngine.Random.Range(0, camadaatual.Count); //Escolhe uma casa aleatoria pra seguir
                        }while(!usados.Add(index)); //assim ela não vai repetir

                        noAnterior.conexoes.Add(camadaatual[index]); // cria a conexao
                    }
                }
            }

            foreach(Node noAtual in camadaatual)
            {
                bool temCaminho = false;
                foreach(Node noAnte in camadaAnterior)
                {
                    if(noAnte.conexoes.Contains(noAtual))
                    {
                        temCaminho = true;
                        break;
                    }
                }

                if(!temCaminho)
                {
                    Node aleatorioAnterior = camadaAnterior[UnityEngine.Random.Range(0, camadaAnterior.Count)];
                    aleatorioAnterior.conexoes.Add(noAtual);
                }
            }

            

            camadas.Add(camadaatual); //adiciono ela no mapa
            camadaAnterior = camadaatual; // Transformando ela na camada anterior
        }

        Node chefe = new Node("Boss"); // criando a casa final
        foreach(Node nofinal in camadaAnterior)
        {
            nofinal.conexoes.Add(chefe);
        }       
        camadas.Add(new List<Node>{chefe}); // adiciona a camada final que é o boss

        Debug.Log("Mapa Gerado");
        mostraMapa();// mostra no console todas as camdas geradas
    }
    
    void Start()
    {
        GeraMapa(); // gera o mapa
    }

    // Update é chamdo uma vez por frme
    void Update()
    {
        
    }

    string TipoAleatorioMelhor()
    {
        string[] baseTipos = {"Batalha", "Evento", "Loja", "Baú"};

        List<string> rand = new List<string>
        {
            "Batalha", "Batalha", "Batalha", "Evento", "Loja", "Baú"
        };

        return rand[UnityEngine.Random.Range(0, rand.Count)];
    }

    // a funçao que o outro script usa para pegar o mapa
    public List<List<Node>> GetMapa()
    {
        return camadas;
    }
}

//classe que é a Casa
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