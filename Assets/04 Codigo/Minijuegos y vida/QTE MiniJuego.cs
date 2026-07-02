using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.EventSystems.EventTrigger;

public class QTEMiniJuego : MonoBehaviour
{
    [Header("Menciones")]



    [Header("UI")]
    public TextMeshProUGUI qteText;
    public TextMeshProUGUI timerText;
    public GameObject MinigamePanel;

    [Header("Configuración QTE")]
    public int minSequenceLength = 5;
    public int maxSequenceLength = 10;
    public float timeLimit = 7f;

    [Header("Progreso")]
    public int maxRounds = 3;

    private KeyCode[] possibleKeys =
    {
        KeyCode.A, KeyCode.B, KeyCode.C, KeyCode.D,
        KeyCode.E, KeyCode.F, KeyCode.G, KeyCode.H,
        KeyCode.I, KeyCode.J, KeyCode.K, KeyCode.L,
        KeyCode.M, KeyCode.N, KeyCode.O, KeyCode.P,
        KeyCode.Q, KeyCode.R, KeyCode.S, KeyCode.T,
        KeyCode.U, KeyCode.V, KeyCode.W, KeyCode.X,
        KeyCode.Y, KeyCode.Z
    };

    public KeyCode[] sequence;
    public int currentIndex;
    public float timer;
    public bool qteActive;

    public int currentRound;



    private void Start()
    {
        if (MinigamePanel != null)
        {
            MinigamePanel.SetActive(false);
        }

        StartMinigame();
    }



    private void Update()
    {
        if (!qteActive)
            return;

        timer -= Time.deltaTime;

        timerText.text = $"Tiempo: {timer:F1}";

        if (timer <= 0f)
        {
            Fail();
            return;
        }

        if (Input.anyKeyDown)
        {
            if (Input.GetKeyDown(sequence[currentIndex]))
            {
                currentIndex++;

                if (currentIndex >= sequence.Length)
                {
                    Success();
                }
                else
                {
                    UpdateDisplay();
                }
            }
            else
            {
                Fail();
            }
        }
    }

    public void StartMinigame()
    {
        if (MinigamePanel.activeSelf)
            return;

        MinigamePanel.SetActive(true);

        currentRound = 0;

        qteText.text = "";
        timerText.text = "";

        StartQTE();

    }

    public void StartQTE()
    {
        int sequenceLength =
            Random.Range(minSequenceLength, maxSequenceLength + 1);

        sequence = new KeyCode[sequenceLength];

        for (int i = 0; i < sequenceLength; i++)
        {
            sequence[i] =
                possibleKeys[Random.Range(0, possibleKeys.Length)];
        }

        currentIndex = 0;
        timer = timeLimit;
        qteActive = true;

        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        string display = "";

        for (int i = 0; i < sequence.Length; i++)
        {
            if (i == currentIndex)
            {
                display += $"<color=yellow>{sequence[i]}</color> ";
            }
            else
            {
                display += sequence[i] + " ";
            }
        }

        qteText.text = display;
    }

    private void Success()
    {
        qteActive = false;

        currentRound++;

        if (currentRound >= maxRounds)
        {
            EndMinigame();
        }
        else
        {
            Invoke(nameof(RestartRound), 0.5f);
        }
    }

    private void RestartRound()
    {
        StartQTE();
    }

    private void Fail()
    {
        qteActive = false;
        CloseMinigame();
        MinijuegoEmpezar.Instance.vidas -= 1;
        MinijuegoEmpezar.Instance.Randonautica();
    }

    private void EndMinigame()
    {
        qteActive = false;
        CloseMinigame();
        MinijuegoEmpezar.Instance.victorias++;
        MinijuegoEmpezar.Instance.Randonautica();
    }

    private void CloseMinigame()
    {
        qteText.text = "";
        timerText.text = "";

        MinigamePanel.SetActive(false);
    }


    



}
