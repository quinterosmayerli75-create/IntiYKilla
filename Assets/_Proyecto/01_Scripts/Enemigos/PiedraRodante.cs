// =====================================================================
// PiedraRodante.cs
// Proyecto: Inti y Killa
// Descripción: Piedra que rueda por un piso, en ciclo:
//              1) Aparece temblando en su punto de partida: es el aviso.
//              2) Rueda hasta el final del recorrido y mata a quien toque.
//              3) Desaparece, descansa y vuelve a empezar.
//              No se puede pisar, pero se puede saltar: es baja a propósito
//              para que también se salte bajo un techo (regla de justicia).
// Uso: objeto con SpriteRenderer. Mientras no haya arte se ve como una forma
//      de color; basta con poner una imagen en "imagenPropia".
// =====================================================================

using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class PiedraRodante : MonoBehaviour
{
    [Header("Dibujo")]
    [SerializeField] private Sprite imagenPropia;

    [Header("Recorrido")]
    // Positivo rueda a la derecha; negativo, a la izquierda.
    [SerializeField] private float distancia = -10f;
    [SerializeField] private float velocidad = 7f;
    [SerializeField] private float radio = 0.45f;

    [Header("Tiempos del ciclo")]
    [SerializeField] private float segundosDeEspera = 2f;
    [SerializeField] private float segundosDeAviso = 0.8f;
    [SerializeField] private float intensidadTemblor = 0.06f;

    [SerializeField] private string mensajeDeMuerte = "Una piedra de los Yungas aplanó a {0}.";

    private SpriteRenderer dibujo;
    private ControladorJugador[] jugadores;
    private Vector3 posicionInicial;
    private bool estaRodando = false;

    public bool EstaAvisando { get; private set; }

    public bool EstaRodando
    {
        get { return estaRodando; }
    }

    public void Configurar(float nuevaDistancia, float nuevaVelocidad, float nuevosSegundosDeEspera)
    {
        distancia = nuevaDistancia;
        velocidad = nuevaVelocidad;
        segundosDeEspera = nuevosSegundosDeEspera;
    }

    private void Awake()
    {
        dibujo = GetComponent<SpriteRenderer>();
        jugadores = FindObjectsByType<ControladorJugador>();
        posicionInicial = transform.position;
        dibujo.enabled = false;

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
        if (estaRodando == false)
        {
            return;
        }

        Vector2 centro = transform.position;
        foreach (ControladorJugador jugador in jugadores)
        {
            if (jugador != null && jugador.EstaVivo == true)
            {
                Bounds cuerpo = jugador.LimitesCuerpo;
                bool cruzaEnX = cuerpo.max.x > centro.x - radio && cuerpo.min.x < centro.x + radio;
                bool cruzaEnY = cuerpo.max.y > centro.y - radio && cuerpo.min.y < centro.y + radio;
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

            // Aviso: aparece y tiembla antes de soltarse.
            dibujo.enabled = true;
            EstaAvisando = true;
            float tiempo = 0f;
            while (tiempo < segundosDeAviso)
            {
                tiempo = tiempo + Time.deltaTime;
                Vector2 desplazamiento = Random.insideUnitCircle * intensidadTemblor;
                transform.position = posicionInicial + new Vector3(desplazamiento.x, desplazamiento.y, 0f);
                yield return null;
            }
            EstaAvisando = false;
            transform.position = posicionInicial;

            estaRodando = true;
            float direccion = Mathf.Sign(distancia);
            float recorrido = 0f;
            while (recorrido < Mathf.Abs(distancia))
            {
                float paso = velocidad * Time.deltaTime;
                recorrido = recorrido + paso;
                transform.position = transform.position + new Vector3(direccion * paso, 0f, 0f);

                // Gira según lo que avanza, como una rueda de verdad.
                float grados = -direccion * paso / radio * Mathf.Rad2Deg;
                transform.Rotate(0f, 0f, grados);
                yield return null;
            }
            estaRodando = false;

            dibujo.enabled = false;
            transform.position = posicionInicial;
        }
    }
}
