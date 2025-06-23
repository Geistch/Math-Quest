using UnityEngine;
using UnityEngine.UI; // Biblioteca para elementos de UI (como botões)
using System;
using System.Linq; // Para uso de métodos como .Skip() e .Take()
using TMPro; // Para usar TextMeshProUGUI
using JetBrains.Annotations;
using Unity.VisualScripting.Dependencies.NCalc;

public class Baralho : MonoBehaviour
{
    // Array que representa o baralho com 54 cartas (valores entre 0 e 9)
    private int[] cards = new int[54];

    // Referências aos componentes de UI no inspetor
    public Button button; // Botão para pegar cartas
    public TextMeshProUGUI lblResult; // Label que mostra as 6 novas cartas
    public TextMeshProUGUI vlr1; // Primeiro valor escolhido pelo jogador
    public TextMeshProUGUI vlr2; // Segundo valor escolhido pelo jogador
    public TextMeshProUGUI txtResult; // Resultado da operação matemática
    public TextMeshProUGUI operador; // Operador atual (+, -, x ou /)
    public TextMeshProUGUI NumeroTexto; // Valor do inimigo (ou jogador) que será subtraído
    public TextMeshProUGUI life; // Vida do jogador que será subtraído
    public Transform buttonContainer; // Local onde os botões das cartas serão criados
    public Transform enemyOptionsContainer;
    public GameObject buttonPrefab; // Prefab dos botões de cartas

    // Instância do gerador de números aleatórios
    private System.Random random = new System.Random();

    // Índice atual para controle de cartas já utilizadas
    private int currentIndex = 0;

    // Ao iniciar a cena
    void Start()
    {
        cardsForm(); // Gera os valores das cartas
        button.onClick.AddListener(PegarCartas); // Adiciona o evento de clique ao botão
        randOperator(); // Define o operador aleatório inicial
        PegarCartas(); // Gera as primeiras cartas logo no início
    }

    // Variável para guardar o operador atual (1=+, 2=-, 3=x, 4=/)
    private int operadorAtual;

    // Gera operador aleatório e atualiza o texto do operador
    public int randOperator() {
        operadorAtual = random.Next(1, 4); // De 1 a 3 (divisão nunca será escolhida)

        switch (operadorAtual) {
            case 1:
                operador.text = "+"; break;
            case 2:
                operador.text = "-"; break;
            case 3:
                operador.text = "x"; break;
            case 4:
                operador.text = "/"; break;
            default:
                operador.text = "operador inválido"; break;
        }
        return operadorAtual;
    }

    // Gera valores aleatórios entre 0 e 9 para as 54 cartas
    public void cardsForm() 
    {
        for (int i = 0; i < cards.Length; i++) 
        {
            cards[i] = random.Next(0, 10);
        }
        currentIndex = 0; // Reinicia o índice
    }

    // Método chamado ao clicar no botão principal
    private void PegarCartas()
    {
        // Pega até 6 cartas restantes do baralho
        int cartasRestantes = Math.Min(6, cards.Length - currentIndex);
        int[] newCards = cards.Skip(currentIndex).Take(cartasRestantes).ToArray(); 
        currentIndex += cartasRestantes;
        // Remove botões antigos
        foreach (Transform child in buttonContainer)
        {
            Destroy(child.gameObject);
        }
        // Cria novos botões com as cartas
        foreach (int card in newCards)
        {
            GameObject newButton = Instantiate(buttonPrefab, buttonContainer);
            newButton.GetComponentInChildren<TextMeshProUGUI>().text = card.ToString();

            // Quando o botão é clicado, chama o método com o valor da carta
            newButton.GetComponent<Button>().onClick.AddListener(() => OnCardButtonClick(newButton, card));
        }
    }

    // Controle de preenchimento dos valores
    public bool vlr1Full = false;
    public bool vlr2Full = false;

    // Quando um botão de carta é clicado
    private void OnCardButtonClick(GameObject button, int card)
    {
        if (!vlr1Full)
        {
            vlr1.text = "" + card;
            vlr1Full = true;
            Destroy(button); // Remove o botão da UI
        }
        else if (!vlr2Full)
        {
            vlr2.text = "" + card;
            vlr2Full = true;
            result(); // Calcula o resultado
            Destroy(button);
        }
    }

    // Calcula o resultado da operação entre vlr1 e vlr2
    private void result() {
        if (vlr1 == null || vlr2 == null || txtResult == null)
        {
            Debug.LogError("Um dos objetos de texto não está atribuído no Inspector!");
            return;
        }

        switch (operadorAtual) {
            case 1:
                txtResult.text = (int.Parse(vlr1.text) + int.Parse(vlr2.text)).ToString(); break;
            case 2:
                txtResult.text = (int.Parse(vlr1.text) - int.Parse(vlr2.text)).ToString(); break;
            case 3:
                txtResult.text = (int.Parse(vlr1.text) * int.Parse(vlr2.text)).ToString(); break;
            case 4:
                txtResult.text = (int.Parse(vlr1.text) / int.Parse(vlr2.text)).ToString(); break;
        }

        // Após 2 segundos, limpa tudo
        Invoke("clearAll", 2.0f);
    }

    // Limpa os valores após o cálculo
    private void clearAll() {
        // Subtrai o valor do resultado do valor atual de NumeroTexto
        NumeroTexto.text = (int.Parse(NumeroTexto.text) - int.Parse(txtResult.text)).ToString();
        
        // Limpa os textos
        vlr1.text = "";
        vlr2.text = "";
        txtResult.text = "";

        // Marca como disponíveis novamente
        vlr1Full = false;
        vlr2Full = false;

        // Gera novo operador
        randOperator();
        TurnoDoInimigo(); // Chama o turno do inimigo
    }

    public void TurnoDoInimigo()
{
    // 1. Gera valores da conta
    int a = random.Next(0, 10);
    int b = random.Next(1, 10); // evita divisão por 0
    int operadorRandom = random.Next(1, 4); // 1=+, 2=-, 3=x
    int resultadoCorreto = 0;
    string simbolo = "";

    switch (operadorRandom)
    {
        case 1: simbolo = "+"; resultadoCorreto = a + b; break;
        case 2: simbolo = "-"; resultadoCorreto = a - b; break;
        case 3: simbolo = "x"; resultadoCorreto = a * b; break;
        case 4: simbolo = "/"; resultadoCorreto = a / b; break;
    }

    // 2. Mostra a conta para o jogador
    lblResult.text = $"Resolva: {a} {simbolo} {b}";

    // 3. Gera 3 alternativas erradas
    var opcoes = new System.Collections.Generic.HashSet<int> { resultadoCorreto };
    while (opcoes.Count < 4)
    {
        int fake = resultadoCorreto + random.Next(-5, 6);
        if (fake != resultadoCorreto && fake >= 0)
            opcoes.Add(fake);
    }

    var listaOpcoes = opcoes.OrderBy(x => random.Next()).ToList();

    // Remove apenas botões de opções do inimigo
    foreach (Transform child in enemyOptionsContainer)
    {
        Destroy(child.gameObject);
    }

    // 5. Cria os botões de opção
    foreach (int opcao in listaOpcoes)
    {
        GameObject newButton = Instantiate(buttonPrefab, enemyOptionsContainer);
        newButton.GetComponentInChildren<TextMeshProUGUI>().text = opcao.ToString();

        // Quando o jogador clicar, verifica se acertou
        newButton.GetComponent<Button>().onClick.AddListener(() =>
        {
            VerificarRespostaJogador(opcao, resultadoCorreto);
        });
    }
}
private void VerificarRespostaJogador(int respostaJogador, int respostaCorreta)
{
     int vidaAtual = int.Parse(life.text);
    if (respostaJogador == respostaCorreta)
    {
        vidaAtual -= 10; // exemplo: dano menor
        Debug.Log("Jogador acertou! Sofre dano menor.");
    }
    else
    {
       vidaAtual -= 20; // exemplo: dano maior
        Debug.Log("Jogador errou! Sofre dano maior.");
    }
    life.text = vidaAtual.ToString();
    // Limpa após 2 segundos
Invoke("LimparBotoes", 2f);
}
private void LimparBotoes()
{
    foreach (Transform child in enemyOptionsContainer)
    {
        Destroy(child.gameObject);
    }

    lblResult.text = "";
}


}
