using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;
public class MinijuegoEmpezar : MonoBehaviour
{
    public static MinijuegoEmpezar Instance;

    [Header("Interaccion con el trigger")]
    public TMP_Text text;
    public string PresioneMinijuegos = "Presione la Rueda del Mouse para Interactuar con la Tableta";
    public bool InTableta;

    [Header("Menciones")]
    QTEMiniJuego Captcha;
    PopUpMinijuego PopUps;

    [Header("Vidas y victoria")]

    public int vidas = 3;
    public int victorias;
    public int MaxVictorias = 4;
    [Header("Abrir el Minijuego")]
    public GameObject panelActual;


    private void Update()
    {
        if (vidas <= 0) 
        {
            SceneManager.LoadScene("Pantalla de muerte");
        }

        Efectotableta();

    }


    private void OnTriggerEnter(Collider other)
    {


        if (other.gameObject.CompareTag("Player"))
        {
            text.text = PresioneMinijuegos;
            InTableta = true;

        }

    }

    void Efectotableta()
    {
        if (Input.GetKey(KeyCode.Mouse2) && InTableta)
        {
            text.text = string.Empty;
            Randonautica();
            Debug.Log("Empezo el minijuego");
            Time.timeScale = 0f;
            InTableta = false;
        }
    }

    public void Randonautica()
    {
        int Chanceint = Random.Range(1, 2); // el numero depende de cuantos minijuegos tengas :p
        switch (Chanceint)
        {
            case 1:
                Captchajuego();
                break;
            case 2:
                PopUpJuego();
                break;
        }
    }

    void Captchajuego() 
    {

    
        if (Captcha == null)
            return;

        if (Captcha.MinigamePanel.activeSelf)
            return;

        Captcha.MinigamePanel.SetActive(true);

        Captcha.currentRound = 0;

        Captcha.qteText.text = "";
        Captcha.timerText.text = "";

        Captcha.StartQTE();
    
    }
    void PopUpJuego() 
    { 
        PopUps.StartGame();
        
    }

    public void AbrirMinijuego(GameObject pantallaMinijuego)
    {
        if (pantallaMinijuego == null)
        {
            Debug.LogWarning("MinigameManager: se intento abrir un minijuego con panel null.");
            return;
        }

        Debug.Log(
            $"Minijuego Empezar: AbrirMinijuego -> {pantallaMinijuego.name}, " +
            $"ActivoSelfAntes={pantallaMinijuego.activeSelf}, " +
            $"ActivoJerarquiaAntes={pantallaMinijuego.activeInHierarchy}");

        if (panelActual != null && panelActual != pantallaMinijuego)
            panelActual.SetActive(false);

        if (pantallaMinijuego.activeSelf)
            pantallaMinijuego.SetActive(false);

        panelActual = pantallaMinijuego;
        Time.timeScale = 0f;
        pantallaMinijuego.SetActive(true);

        Debug.Log(
            $"MinigameManager: panel activado -> {pantallaMinijuego.name}, " +
            $"ActivoSelfDespues={pantallaMinijuego.activeSelf}, " +
            $"ActivoJerarquiaDespues={pantallaMinijuego.activeInHierarchy}");

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }



}
