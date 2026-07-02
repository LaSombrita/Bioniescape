using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class Barradevida : MonoBehaviour
{
    Animator Anim;
    [SerializeField] public float Cantdoshit;
    public Image BarraDaño;
    public float Daño;
    public float DañoMaximo;
    public bool IsOnDamage;
    [SerializeField] float DañoCaida = 1;

    private void Awake()
    {
        Anim = GetComponent<Animator>();

        DañoMaximo = 100;
        Daño = 0;
    }

    public void AyMeCai()
    {

      Daño += DañoCaida;
      Anim.SetTrigger("Hit");
      StartCoroutine(CDSt());
        Debug.Log("Ay Me caí");
    }

    public void DañoInfringido(float Auch) 
    {
        Daño += Auch;
        Anim.SetTrigger("Hit");
        StartCoroutine(CDSt());
    }

    public void DañoConKnock(float Auch, Vector3 posicion)
    {
        Daño += Auch;
        StartCoroutine(CDSt());
    }

    public void Whentemueres() 
    {
        if (Daño >= DañoMaximo)
        {
            SceneManager.LoadScene("Pantalla de Muerte");
            Debug.Log("Cargando Escena");
        }
    }

    void Update()
    {
        EfectoDaño();
        Whentemueres();

    }

    public void EfectoDaño()
    {
        BarraDaño.fillAmount = Daño / DañoMaximo;
    }

    private IEnumerator CDSt()
    {
        yield return new WaitForSeconds(Cantdoshit);

    }
}
