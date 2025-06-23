using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VidaUIController : MonoBehaviour
{
    public TextMeshProUGUI NumeroTexto;
    public Image BarraVida;
    public Sprite life100;
    public Sprite life75;
    public Sprite life50;
    public Sprite life25;
    public Sprite life0;

    public void AtualizarBarraVida()
    {
        int vida = int.Parse(NumeroTexto.text);

        if (vida >= 100)
            BarraVida.sprite = life100;
        else if (vida >= 75)
            BarraVida.sprite = life75;
        else if (vida >= 50)
            BarraVida.sprite = life50;
        else if (vida >= 25)
            BarraVida.sprite = life25;
        else
            BarraVida.sprite = life0;
    }
}
