// =====================================================================
// AuraElemento.cs
// Proyecto: Inti y Killa
// Descripción: Resplandor con chispas alrededor del personaje, del color de
//              su elemento actual. Sirve para saber de un vistazo qué poder
//              tiene cada uno, sobre todo después de un eclipse. Al cambiar
//              de elemento el resplandor crece un momento para llamar la atención.
// Uso: en el jugador, con un hijo que tenga el SpriteRenderer del resplandor.
// =====================================================================

using UnityEngine;

[RequireComponent(typeof(ControladorJugador))]
public class AuraElemento : MonoBehaviour
{
    [Header("Dibujos")]
    [SerializeField] private SpriteRenderer dibujoResplandor;
    [SerializeField] private SpriteRenderer[] chispas;

    [Header("Resplandor")]
    [SerializeField] private float transparenciaMinima = 0.7f;
    [SerializeField] private float transparenciaMaxima = 1f;
    [SerializeField] private float velocidadPulso = 4f;
    [SerializeField] private float crecimientoAlCambiar = 0.8f;
    [SerializeField] private float segundosDelCambio = 0.6f;

    [Header("Chispas")]
    // Altura que suben las chispas, en unidades del juego.
    [SerializeField] private float alturaChispas = 1.5f;
    [SerializeField] private float anchoChispas = 0.45f;
    [SerializeField] private float velocidadChispas = 0.9f;

    private ControladorJugador jugador;
    private Vector3 escalaResplandor;
    private TipoElemento ultimoElemento;
    private float tiempoDesdeElCambio = 100f;

    public void Configurar(SpriteRenderer nuevoResplandor, SpriteRenderer[] nuevasChispas)
    {
        dibujoResplandor = nuevoResplandor;
        chispas = nuevasChispas;
    }

    private void Awake()
    {
        jugador = GetComponent<ControladorJugador>();
    }

    private void Start()
    {
        ultimoElemento = jugador.Elemento;
        escalaResplandor = dibujoResplandor.transform.localScale;
    }

    private void Update()
    {
        // Al morir desaparece: el personaje ya se oscurece para indicarlo.
        bool estaVivo = jugador.EstaVivo;
        dibujoResplandor.enabled = estaVivo;
        foreach (SpriteRenderer chispa in chispas)
        {
            chispa.enabled = estaVivo;
        }

        if (estaVivo == false)
        {
            return;
        }

        if (jugador.Elemento != ultimoElemento)
        {
            ultimoElemento = jugador.Elemento;
            tiempoDesdeElCambio = 0f;
        }
        tiempoDesdeElCambio = tiempoDesdeElCambio + Time.deltaTime;

        Color colorElemento = ColoresElemento.ObtenerColor(jugador.Elemento);
        ActualizarResplandor(colorElemento);
        ActualizarChispas(colorElemento);
    }

    private void ActualizarResplandor(Color colorElemento)
    {
        float pulso = (Mathf.Sin(Time.time * velocidadPulso) + 1f) / 2f;
        float transparencia = Mathf.Lerp(transparenciaMinima, transparenciaMaxima, pulso);
        dibujoResplandor.color = new Color(colorElemento.r, colorElemento.g, colorElemento.b, transparencia);

        // Después de un eclipse crece y vuelve poco a poco a su tamaño.
        float progreso = Mathf.Clamp01(tiempoDesdeElCambio / segundosDelCambio);
        float escala = 1f + crecimientoAlCambiar * (1f - progreso);
        dibujoResplandor.transform.localScale = escalaResplandor * escala;
    }

    // Las chispas suben ondulando y se desvanecen, como una llamita.
    private void ActualizarChispas(Color colorElemento)
    {
        Vector3 escalaJugador = transform.localScale;

        for (int indice = 0; indice < chispas.Length; indice++)
        {
            float fase = (float)indice / chispas.Length;
            float avance = Mathf.Repeat(Time.time * velocidadChispas + fase, 1f);

            float posicionX = Mathf.Sin((avance + fase) * Mathf.PI * 4f) * anchoChispas;
            float posicionY = -0.5f + avance * alturaChispas;
            chispas[indice].transform.localPosition = new Vector3(posicionX / escalaJugador.x, posicionY / escalaJugador.y, 0f);
            chispas[indice].color = new Color(colorElemento.r, colorElemento.g, colorElemento.b, 1f - avance);
        }
    }
}
