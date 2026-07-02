using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;
public class PopUpMinijuego : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private RectTransform spawnArea;
    [SerializeField] private GameObject PopUps;
    public GameObject PopUpCanvas;

    [Header("Config")]
    [SerializeField] private int maxOnScreen = 3;
    [SerializeField] private int totalThoughts = 6;
    [SerializeField] private float timeLimit = 7f;

    private float timer;
    private int spawnedTotal;
    private int activeCount;
    private int clickedCount;
    private bool running;

    public bool IsRunning => running;

    private void Update()
    {
        if (!running) return;

        timer += Time.deltaTime;

        if (timer >= timeLimit)
        {
            Fail();
            return;
        }

        FillSlots();
    }

    public void StartGame()
    {
        timer = 0f;
        spawnedTotal = 0;
        activeCount = 0;
        clickedCount = 0;
        running = true;

        PopUpCanvas.SetActive(true);
        FillSlots();
    }

    private void FillSlots()
    {
        while (activeCount < maxOnScreen && spawnedTotal < totalThoughts)
        {
            SpawnThought();
        }
    }

    private void SpawnThought()
    {
        GameObject obj = Instantiate(PopUps, spawnArea);

        RectTransform rt = obj.GetComponent<RectTransform>();
        rt.anchoredPosition = GetRandomPosition();

        obj.GetComponent<PopUpsFuncion>().Init(this);

        obj.SetActive(true);

        spawnedTotal++;
        activeCount++;
    }

    private Vector2 GetRandomPosition()
    {
        Vector2 size = spawnArea.rect.size;

        float x = Random.Range(-size.x / 2f, size.x / 2f);
        float y = Random.Range(-size.y / 2f, size.y / 2f);

        return new Vector2(x, y);
    }

    public void OnThoughtClicked(GameObject obj)
    {
        activeCount--;
        clickedCount++;

        Destroy(obj);

        if (clickedCount >= totalThoughts)
        {
            Win();
        }
    }

    private void Win()
    {
        EndGame();
        MinijuegoEmpezar.Instance.victorias++;
        MinijuegoEmpezar.Instance.Randonautica();
    }

    private void Fail()
    {
        EndGame();
        MinijuegoEmpezar.Instance.vidas -= 1;
        MinijuegoEmpezar.Instance.Randonautica();
    }

    private void EndGame()
    {
        running = false;
        PopUpCanvas.SetActive(false);
    }

}
