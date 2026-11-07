// =====================================================================
// CrecidaRio.cs
// Proyecto: Inti y Killa
// Descripción: Crecida del río (mazamorra de barro): después de un aviso, el
//              agua sube sin parar y se lleva a quien alcance, tenga el poder
//              que tenga. Obliga a escalar el nivel sin detenerse, y como todo
//              cabe en una pantalla se ve venir desde el principio.
// Uso: un solo objeto en la escena, con un SpriteRenderer que crece. Se detiene
//      antes de llegar al piso de las puertas, para que siempre se pueda ganar.
// =====================================================================

using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class CrecidaRio : MonoBehaviour
{
    [SerializeField] private float alturaDeAbajo = -6f;
    [SerializeField] private float alturaInicial = -3.6f;
    [SerializeField] private float alturaFinal = 9f;
    [SerializeField] private float velocidad = 0.75f;
    [SerializeField] private float segundosDeAviso = 3f;
    [SerializeField] private float anchoZona = 40f;
    // Lo que deben hundirse los pies para que se lo lleve: un roce no mata.
    [SerializeField] private float margenDeLosPies = 0.15f;
    [SerializeField] private string mensajeDeMuerte = "La crecida se llevó a {0} río abajo.";

    private ControladorJugador[] jugadores;
    private float tiempo = 0f;
    private bool yaAviso = false;

    public float NivelActual { get; private set; }

    public void Configurar(float nuevaAlturaFinal, float nuevaVelocidad, float nuevosSegundosDeAviso)
    {
        alturaFinal = nuevaAlturaFinal;
        velocidad = nuevaVelocidad;
        segundosDeAviso = nuevosSegundosDeAviso;
    }

    private void Awake()
    {
        jugadores = FindObjectsByType<ControladorJugador>();
        NivelActual = alturaInicial;
        ActualizarDibujo();
    }

    private void Update()
    {
        tiempo = tiempo + Time.deltaTime;

        if (yaAviso == false && GestorNivel.Instancia != null)
        {
            GestorNivel.Instancia.MostrarAviso("¡Viene la crecida! ¡Suban sin parar!", segundosDeAviso);
            yaAviso = true;
        }

        if (tiempo >= segundosDeAviso && NivelActual < alturaFinal)
        {
            NivelActual = Mathf.Min(alturaFinal, NivelActual + velocidad * Time.deltaTime);
            ActualizarDibujo();
        }

        foreach (ControladorJugador jugador in jugadores)
        {
            if (jugador.EstaVivo == true && jugador.LimitesCuerpo.min.y < NivelActual - margenDeLosPies)
            {
                jugador.Morir(mensajeDeMuerte.Replace("{0}", jugador.NombreJugador));
            }
        }
    }

    // El dibujo se estira desde abajo hasta el nivel actual del agua.
    private void ActualizarDibujo()
    {
        float alto = NivelActual - alturaDeAbajo;
        transform.position = new Vector3(0f, alturaDeAbajo + alto / 2f, 0f);
        transform.localScale = new Vector3(anchoZona, alto, 1f);
    }
}
