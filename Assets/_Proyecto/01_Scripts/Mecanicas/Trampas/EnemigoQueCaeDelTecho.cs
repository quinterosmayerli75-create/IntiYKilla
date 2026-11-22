// =====================================================================
// EnemigoQueCaeDelTecho.cs
// Proyecto: Inti y Killa
// Descripción: Un bicho escondido en el techo. Cuando alguien pasa debajo,
//              se asoma temblando (aviso), se deja caer y, ya en el piso,
//              corre un tramo corto hacia donde iba el jugador y desaparece.
//              Quien sigue caminando sin detenerse se salva; quien frena
//              justo debajo, no.
// Uso: objeto con SpriteRenderer pegado al techo. Se activa solo o con un
//      SensorTrampa.
// =====================================================================

using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class EnemigoQueCaeDelTecho : TrampaActivable
{
    [SerializeField] private float alturaDelPiso = -3.5f;
    [SerializeField] private Vector2 tamano = new Vector2(0.8f, 0.8f);
    [SerializeField] private float anchoDeDeteccion = 1.2f;
    [SerializeField] private float segundosDeAviso = 0.4f;
    [SerializeField] private float velocidadCaida = 24f;
    [SerializeField] private float velocidadCarrera = 3f;
    [SerializeField] private float distanciaARecorrer = 2.5f;
    [SerializeField] private string mensajeDeMuerte = "Algo cayó del techo encima de {0}.";

    private SpriteRenderer dibujo;
    private Vector3 posicionOriginal;
    private bool esPeligroso = false;

    public void Configurar(float nuevaAlturaDelPiso)
    {
        alturaDelPiso = nuevaAlturaDelPiso;
    }

    protected override void Awake()
    {
        base.Awake();
        dibujo = GetComponent<SpriteRenderer>();
        posicionOriginal = transform.position;
        // Escondido: apenas un poquito visible, como una mancha en el techo.
        Color color = dibujo.color;
        dibujo.color = new Color(color.r, color.g, color.b, 0.25f);
    }

    private void Update()
    {
        if (esPeligroso == true)
        {
            ControladorJugador tocado = BuscarJugadorEnRectangulo(transform.position, tamano * 0.85f);
            if (tocado != null)
            {
                tocado.Morir(mensajeDeMuerte.Replace("{0}", tocado.NombreJugador));
            }
        }

        if (SeActivaSola == false || YaActivada == true)
        {
            return;
        }

        // Columna bajo el bicho, desde el piso hasta él.
        float alto = posicionOriginal.y - alturaDelPiso;
        Vector2 centroColumna = new Vector2(posicionOriginal.x, alturaDelPiso + alto / 2f);
        ControladorJugador jugador = BuscarJugadorEnRectangulo(centroColumna, new Vector2(anchoDeDeteccion, alto));
        if (jugador != null)
        {
            Activar(jugador);
        }
    }

    protected override void AlActivarse(ControladorJugador causante)
    {
        float direccion = 1f;
        if (causante != null && causante.transform.position.x < posicionOriginal.x)
        {
            direccion = -1f;
        }
        StartCoroutine(CaerYCorrer(direccion));
    }

    private IEnumerator CaerYCorrer(float direccion)
    {
        // 1) Aviso: se vuelve visible y tiembla.
        Color color = dibujo.color;
        dibujo.color = new Color(color.r, color.g, color.b, 1f);
        float tiempo = 0f;
        while (tiempo < segundosDeAviso)
        {
            tiempo = tiempo + Time.deltaTime;
            float temblor = Mathf.Sin(tiempo * 60f) * 0.06f;
            transform.position = posicionOriginal + new Vector3(temblor, 0f, 0f);
            yield return null;
        }

        // 2) Cae hasta el piso.
        esPeligroso = true;
        float alturaFinal = alturaDelPiso + tamano.y / 2f;
        transform.position = posicionOriginal;
        while (transform.position.y > alturaFinal)
        {
            transform.position = transform.position + new Vector3(0f, -velocidadCaida * Time.deltaTime, 0f);
            yield return null;
        }
        transform.position = new Vector3(transform.position.x, alturaFinal, transform.position.z);

        // 3) Corre un tramo corto y desaparece: nunca se queda tapando el camino.
        float recorrido = 0f;
        while (recorrido < distanciaARecorrer)
        {
            float paso = velocidadCarrera * Time.deltaTime;
            recorrido = recorrido + paso;
            transform.position = transform.position + new Vector3(paso * direccion, 0f, 0f);
            yield return null;
        }

        esPeligroso = false;
        dibujo.enabled = false;
    }
}
