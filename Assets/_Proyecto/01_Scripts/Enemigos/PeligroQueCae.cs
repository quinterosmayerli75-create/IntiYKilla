// =====================================================================
// PeligroQueCae.cs
// Proyecto: Inti y Killa
// Descripción: Roca colgada que se repite en ciclo:
//              1) Espera en su lugar.
//              2) Tiembla 0.5 segundos: es el aviso (regla de justicia).
//              3) Cae y mata a quien toque.
//              4) Desaparece al llegar al piso y reaparece después de un rato.
// Uso: objeto con SpriteRenderer. Mientras no haya arte se ve como una forma
//      de color; basta con poner una imagen en "imagenPropia".
// =====================================================================

using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class PeligroQueCae : MonoBehaviour
{
    [Header("Dibujo")]
    [SerializeField] private Sprite imagenPropia;

    [Header("Cuerpo y caída")]
    [SerializeField] private Vector2 tamano = new Vector2(1f, 1f);
    [SerializeField] private float alturaDelPiso = -3.5f;
    [SerializeField] private float velocidadCaida = 12f;

    [Header("Tiempos del ciclo")]
    [SerializeField] private float segundosDeEspera = 2.5f;
    [SerializeField] private float segundosDeTemblor = 0.5f;
    [SerializeField] private float segundosParaReaparecer = 1.5f;
    [SerializeField] private float intensidadTemblor = 0.08f;

    [SerializeField] private string mensajeDeMuerte = "Una roca de Potosí le cayó encima a {0}.";

    private SpriteRenderer dibujo;
    private ControladorJugador[] jugadores;
    private Vector3 posicionOriginal;
    private bool estaCayendo = false;

    public bool EstaTemblando { get; private set; }

    public void Configurar(Vector2 nuevoTamano, float nuevaAlturaDelPiso, float nuevosSegundosDeEspera)
    {
        tamano = nuevoTamano;
        alturaDelPiso = nuevaAlturaDelPiso;
        segundosDeEspera = nuevosSegundosDeEspera;
    }

    private void Awake()
    {
        dibujo = GetComponent<SpriteRenderer>();
        jugadores = FindObjectsByType<ControladorJugador>();
        posicionOriginal = transform.position;

        if (imagenPropia != null)
        {
            dibujo.sprite = imagenPropia;
            dibujo.color = Color.white;
        }
    }

    private void Start()
    {
        StartCoroutine(Ciclo());
    }

    private void Update()
    {
        if (estaCayendo == false)
        {
            return;
        }

        Vector2 centro = transform.position;
        foreach (ControladorJugador jugador in jugadores)
        {
            if (jugador != null && jugador.EstaVivo == true)
            {
                Bounds cuerpo = jugador.LimitesCuerpo;
                bool cruzaEnX = cuerpo.max.x > centro.x - tamano.x / 2f && cuerpo.min.x < centro.x + tamano.x / 2f;
                bool cruzaEnY = cuerpo.max.y > centro.y - tamano.y / 2f && cuerpo.min.y < centro.y + tamano.y / 2f;
                if (cruzaEnX == true && cruzaEnY == true)
                {
                    jugador.Morir(mensajeDeMuerte.Replace("{0}", jugador.NombreJugador));
                }
            }
        }
    }

    private IEnumerator Ciclo()
    {
        while (true)
        {
            yield return new WaitForSeconds(segundosDeEspera);

            // Aviso: tiembla en su lugar antes de soltarse.
            EstaTemblando = true;
            float tiempo = 0f;
            while (tiempo < segundosDeTemblor)
            {
                tiempo = tiempo + Time.deltaTime;
                Vector2 desplazamiento = Random.insideUnitCircle * intensidadTemblor;
                transform.position = posicionOriginal + new Vector3(desplazamiento.x, desplazamiento.y, 0f);
                yield return null;
            }
            transform.position = posicionOriginal;
            EstaTemblando = false;

            // Caída hasta el piso.
            estaCayendo = true;
            float alturaFinal = alturaDelPiso + tamano.y / 2f;
            while (transform.position.y > alturaFinal)
            {
                transform.position = transform.position + new Vector3(0f, -velocidadCaida * Time.deltaTime, 0f);
                yield return null;
            }
            estaCayendo = false;

            // Desaparece y vuelve a su lugar para repetir el ciclo.
            dibujo.enabled = false;
            yield return new WaitForSeconds(segundosParaReaparecer);
            transform.position = posicionOriginal;
            dibujo.enabled = true;
        }
    }
}
