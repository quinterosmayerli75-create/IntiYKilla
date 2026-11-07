// =====================================================================
// Liana.cs
// Proyecto: Inti y Killa
// Descripción: Liana del Trópico que se balancea sola, como un péndulo.
//              Quien la toca en el aire se agarra y se balancea con ella;
//              con la tecla de salto se suelta y sale impulsado. Su balanceo
//              siempre se repite igual: quien lo observa aprende cuándo saltar.
// Uso: el objeto es el punto de donde cuelga; gira, y arrastra a su hijo que
//      dibuja la cuerda. El ControladorJugador se encarga de seguirla.
// =====================================================================

using UnityEngine;

public class Liana : MonoBehaviour
{
    [SerializeField] private float largo = 6.5f;
    [SerializeField] private float anguloMaximo = 32f;
    [SerializeField] private float segundosPorVuelta = 3.6f;
    // 0 y 0.5 hacen que dos lianas se balanceen en sentidos contrarios.
    [SerializeField] private float fase = 0f;
    // Solo se puede agarrar por la parte de abajo.
    [SerializeField] private float parteQueNoSeAgarra = 0.5f;
    [SerializeField] private float distanciaParaAgarrar = 0.6f;
    // Siempre se queda colgado a esta distancia de la punta: así el recorrido
    // es el mismo cada vez y se puede aprender dónde soltarse.
    [SerializeField] private float distanciaDesdeLaPunta = 0.7f;

    private ControladorJugador[] jugadores;
    private float tiempo = 0f;

    // Dónde está la punta de la liana en este momento.
    public Vector2 Punta
    {
        get { return ObtenerPunto(largo); }
    }

    public void Configurar(float nuevoLargo, float nuevosSegundos, float nuevaFase)
    {
        largo = nuevoLargo;
        segundosPorVuelta = nuevosSegundos;
        fase = nuevaFase;
    }

    private void Awake()
    {
        jugadores = FindObjectsByType<ControladorJugador>();
    }

    private void Update()
    {
        tiempo = tiempo + Time.deltaTime;
        float vueltas = tiempo / segundosPorVuelta + fase;
        float angulo = Mathf.Sin(vueltas * Mathf.PI * 2f) * anguloMaximo;
        transform.rotation = Quaternion.Euler(0f, 0f, angulo);

        foreach (ControladorJugador jugador in jugadores)
        {
            if (jugador.PuedeAgarrarLiana() == true)
            {
                IntentarAgarrar(jugador);
            }
        }
    }

    private void IntentarAgarrar(ControladorJugador jugador)
    {
        Vector2 anclaje = transform.position;
        Vector2 direccionCuerda = transform.rotation * Vector2.down;
        Vector2 haciaJugador = (Vector2)jugador.LimitesCuerpo.center - anclaje;

        // Proyección sobre la cuerda: a qué altura de ella queda el jugador.
        float distanciaEnCuerda = Vector2.Dot(haciaJugador, direccionCuerda);
        if (distanciaEnCuerda < largo * parteQueNoSeAgarra || distanciaEnCuerda > largo)
        {
            return;
        }

        Vector2 puntoEnCuerda = anclaje + direccionCuerda * distanciaEnCuerda;
        if (Vector2.Distance(puntoEnCuerda, anclaje + haciaJugador) <= distanciaParaAgarrar)
        {
            jugador.AgarrarLiana(this, largo - distanciaDesdeLaPunta);
        }
    }

    // El punto de la cuerda que está a esa distancia del anclaje.
    public Vector2 ObtenerPunto(float distancia)
    {
        Vector2 direccionCuerda = transform.rotation * Vector2.down;
        return (Vector2)transform.position + direccionCuerda * distancia;
    }
}
