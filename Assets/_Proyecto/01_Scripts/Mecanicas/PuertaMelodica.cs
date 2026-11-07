// =====================================================================
// PuertaMelodica.cs
// Proyecto: Inti y Killa
// Descripción: Puerta que se abre al tocar una melodía en orden con las cuerdas
//              de charango. Arriba tiene una luz por cada nota, del color de esa
//              nota: así se lee qué hay que tocar. Cada nota correcta enciende
//              su luz; una equivocada apaga todo y la melodía empieza de nuevo.
//              Repetir la última nota no cuenta como error: al rebotar sobre la
//              misma cuerda suena varias veces y no sería justo.
// Uso: se le pasan la melodía, las luces y las barras que se mueven al abrir.
// =====================================================================

using UnityEngine;

public class PuertaMelodica : MonoBehaviour
{
    [SerializeField] private int[] melodia = new int[] { 0, 1, 2 };
    [SerializeField] private SpriteRenderer[] luces;
    [SerializeField] private Rigidbody2D[] barras;
    [SerializeField] private Vector2 desplazamientoAlAbrir = new Vector2(0f, -7f);
    [SerializeField] private float velocidad = 5f;
    [SerializeField] private float transparenciaApagada = 0.25f;

    private Vector2[] posicionesCerradas;
    private int notasAcertadas = 0;
    private int ultimaNota = -1;
    private bool estaAbierta = false;

    public bool EstaAbierta
    {
        get { return estaAbierta; }
    }

    public int NotasAcertadas
    {
        get { return notasAcertadas; }
    }

    public void Configurar(int[] nuevaMelodia, SpriteRenderer[] nuevasLuces, Rigidbody2D[] nuevasBarras, Vector2 nuevoDesplazamiento)
    {
        melodia = nuevaMelodia;
        luces = nuevasLuces;
        barras = nuevasBarras;
        desplazamientoAlAbrir = nuevoDesplazamiento;
    }

    private void Awake()
    {
        posicionesCerradas = new Vector2[barras.Length];
        for (int indice = 0; indice < barras.Length; indice++)
        {
            barras[indice].bodyType = RigidbodyType2D.Kinematic;
            posicionesCerradas[indice] = barras[indice].position;
        }
        ActualizarLuces();
    }

    private void OnEnable()
    {
        CuerdaCharango.AlSonarNota += RecibirNota;
    }

    private void OnDisable()
    {
        CuerdaCharango.AlSonarNota -= RecibirNota;
    }

    private void RecibirNota(int nota)
    {
        if (estaAbierta == true)
        {
            return;
        }

        if (nota == melodia[notasAcertadas])
        {
            notasAcertadas = notasAcertadas + 1;
        }
        else if (nota != ultimaNota)
        {
            Equivocarse(nota);
        }

        ultimaNota = nota;

        if (notasAcertadas >= melodia.Length)
        {
            estaAbierta = true;
            if (GestorNivel.Instancia != null)
            {
                GestorNivel.Instancia.MostrarAviso("¡Qué linda melodía! La puerta se abrió.", 2.5f);
            }
        }

        ActualizarLuces();
    }

    private void Equivocarse(int nota)
    {
        // Si la nota equivocada es la primera de la melodía, ya cuenta como comienzo.
        notasAcertadas = 0;
        if (nota == melodia[0])
        {
            notasAcertadas = 1;
        }

        if (GestorNivel.Instancia != null)
        {
            GestorNivel.Instancia.MostrarAviso("Nota equivocada: la melodía empieza de nuevo. Miren las luces de la puerta.", 2.5f);
        }
    }

    private void ActualizarLuces()
    {
        for (int indice = 0; indice < luces.Length; indice++)
        {
            Color color = CuerdaCharango.ObtenerColorNota(melodia[indice]);
            if (indice >= notasAcertadas)
            {
                color.a = transparenciaApagada;
            }
            luces[indice].color = color;
        }
    }

    private void FixedUpdate()
    {
        for (int indice = 0; indice < barras.Length; indice++)
        {
            Vector2 destino = posicionesCerradas[indice];
            if (estaAbierta == true)
            {
                destino = posicionesCerradas[indice] + desplazamientoAlAbrir;
            }

            Vector2 nuevaPosicion = Vector2.MoveTowards(barras[indice].position, destino, velocidad * Time.fixedDeltaTime);
            barras[indice].MovePosition(nuevaPosicion);
        }
    }
}
