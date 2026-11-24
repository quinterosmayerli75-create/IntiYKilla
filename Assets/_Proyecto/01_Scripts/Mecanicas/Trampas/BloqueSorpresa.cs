// =====================================================================
// BloqueSorpresa.cs
// Proyecto: Inti y Killa
// Descripción: Bloque con un "?" que invita a golpearlo de cabeza, como en
//              los juegos clásicos. Al golpearlo desde abajo no da premio:
//              salta, cambia el "?" por un "!" y suelta un bichito que camina,
//              cae por los bordes y sigue avanzando un tramo.
//              El bichito es más lento que los jugadores y se puede saltar.
// Uso: bloque sólido con BoxCollider2D. El generador le pone un texto hijo
//      llamado "Signo" con el "?".
// =====================================================================

using System.Collections;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class BloqueSorpresa : TrampaActivable
{
    [Header("Bichito que sale del bloque")]
    [SerializeField] private float velocidadBichito = 2.6f;
    [SerializeField] private float distanciaARecorrer = 7f;
    // 1: camina hacia la derecha; -1: hacia la izquierda.
    [SerializeField] private float direccionInicial = 1f;
    [SerializeField] private float tiempoParaSalir = 0.4f;
    [SerializeField] private Color colorBichito = new Color(0.55f, 0.12f, 0.12f);
    [SerializeField] private Sprite spriteBichito;
    [SerializeField] private string mensajeDeMuerte = "{0} golpeó el bloque \"?\"... y el premio era un bichito.";

    private const float tamanoBichito = 0.75f;
    private const float gravedad = 22f;

    private BoxCollider2D colisionador;
    private TextMesh signo;
    private GameObject bichito;
    private bool bichitoAfuera = false;

    // El bot de pruebas lo usa para esperar a que el bichito se vaya.
    public bool BichitoAfuera
    {
        get { return bichitoAfuera; }
    }

    public void Configurar(float nuevaDireccion, float nuevaDistancia, Sprite nuevoSpriteBichito)
    {
        direccionInicial = nuevaDireccion;
        distanciaARecorrer = nuevaDistancia;
        spriteBichito = nuevoSpriteBichito;
    }

    protected override void Awake()
    {
        base.Awake();
        colisionador = GetComponent<BoxCollider2D>();
        Transform hijoSigno = transform.Find("Signo");
        if (hijoSigno != null)
        {
            signo = hijoSigno.GetComponent<TextMesh>();
        }
    }

    private void Update()
    {
        if (bichitoAfuera == true)
        {
            RevisarSiTocaAJugador();
        }

        if (SeActivaSola == false || YaActivada == true)
        {
            return;
        }

        // Zona finita justo debajo del bloque: ahí entra la cabeza al golpearlo.
        Bounds limites = colisionador.bounds;
        Vector2 centroZona = new Vector2(limites.center.x, limites.min.y - 0.1f);
        Vector2 tamanoZona = new Vector2(limites.size.x * 0.9f, 0.3f);
        foreach (ControladorJugador jugador in jugadores)
        {
            if (jugador == null || jugador.EstaVivo == false)
            {
                continue;
            }
            bool estaDebajo = jugador.LimitesCuerpo.center.y < limites.min.y;
            if (estaDebajo == true && jugador.VelocidadVertical > -0.5f && JugadorTocaRectangulo(jugador, centroZona, tamanoZona) == true)
            {
                Activar(jugador);
                return;
            }
        }
    }

    protected override void AlActivarse(ControladorJugador causante)
    {
        if (signo != null)
        {
            signo.text = "!";
            signo.color = new Color(1f, 0.25f, 0.2f);
        }
        StartCoroutine(SaltitoDelBloque());
        StartCoroutine(SoltarBichito());
    }

    // El bloque se levanta un poquito, como si lo hubieran golpeado.
    private IEnumerator SaltitoDelBloque()
    {
        Vector3 inicio = transform.position;
        float tiempo = 0f;
        while (tiempo < 0.16f)
        {
            tiempo = tiempo + Time.deltaTime;
            float altura = Mathf.Sin(tiempo / 0.16f * Mathf.PI) * 0.2f;
            transform.position = inicio + new Vector3(0f, altura, 0f);
            yield return null;
        }
        transform.position = inicio;
    }

    private IEnumerator SoltarBichito()
    {
        Bounds limites = colisionador.bounds;
        bichito = CrearDibujoBichito();

        // Sale despacio por arriba del bloque: ese momento es el aviso.
        Vector3 posicionEscondida = new Vector3(limites.center.x, limites.max.y - tamanoBichito / 2f, 0f);
        Vector3 posicionAfuera = new Vector3(limites.center.x, limites.max.y + tamanoBichito / 2f, 0f);
        float tiempo = 0f;
        while (tiempo < tiempoParaSalir)
        {
            tiempo = tiempo + Time.deltaTime;
            bichito.transform.position = Vector3.Lerp(posicionEscondida, posicionAfuera, tiempo / tiempoParaSalir);
            yield return null;
        }
        bichito.transform.position = posicionAfuera;
        bichitoAfuera = true;

        // Camina; si no hay suelo debajo, cae; si choca una pared, se da la vuelta.
        float direccion = direccionInicial;
        float velocidadCaida = 0f;
        float recorrido = 0f;
        float tiempoMaximo = 9f;
        while (recorrido < distanciaARecorrer && tiempoMaximo > 0f)
        {
            float paso = Time.deltaTime;
            tiempoMaximo = tiempoMaximo - paso;
            Vector2 posicion = bichito.transform.position;

            float distanciaAlSuelo = MedirDistancia(posicion, Vector2.down, 3f);
            if (distanciaAlSuelo > tamanoBichito / 2f + 0.02f)
            {
                velocidadCaida = velocidadCaida + gravedad * paso;
                float bajada = Mathf.Min(velocidadCaida * paso, distanciaAlSuelo - tamanoBichito / 2f);
                posicion.y = posicion.y - bajada;
            }
            else
            {
                velocidadCaida = 0f;
            }

            float distanciaAPared = MedirDistancia(posicion, new Vector2(direccion, 0f), 1f);
            if (distanciaAPared < tamanoBichito / 2f + 0.05f)
            {
                direccion = -direccion;
            }

            float avance = velocidadBichito * paso;
            posicion.x = posicion.x + direccion * avance;
            recorrido = recorrido + avance;
            bichito.transform.position = new Vector3(posicion.x, posicion.y, 0f);

            // Si cayó al vacío, se termina.
            if (posicion.y < -20f)
            {
                break;
            }
            yield return null;
        }

        bichitoAfuera = false;
        Destroy(bichito);
    }

    // Distancia a lo sólido más cercano en esa dirección (sin contar jugadores ni zonas).
    private float MedirDistancia(Vector2 origen, Vector2 direccion, float maximo)
    {
        RaycastHit2D[] choques = Physics2D.RaycastAll(origen, direccion, maximo);
        float menor = maximo;
        foreach (RaycastHit2D choque in choques)
        {
            if (choque.collider == null || choque.collider.isTrigger == true || choque.collider.enabled == false)
            {
                continue;
            }
            if (choque.collider.GetComponentInParent<ControladorJugador>() != null)
            {
                continue;
            }
            if (choque.distance < menor)
            {
                menor = choque.distance;
            }
        }
        return menor;
    }

    private void RevisarSiTocaAJugador()
    {
        if (bichito == null)
        {
            return;
        }
        Vector2 tamano = new Vector2(tamanoBichito * 0.85f, tamanoBichito * 0.85f);
        ControladorJugador atrapado = BuscarJugadorEnRectangulo(bichito.transform.position, tamano);
        if (atrapado != null)
        {
            atrapado.Morir(mensajeDeMuerte.Replace("{0}", atrapado.NombreJugador));
        }
    }

    // Cuerpo redondo oscuro con dos ojos: se reconoce como enemigo.
    private GameObject CrearDibujoBichito()
    {
        GameObject cuerpo = new GameObject("Bichito del bloque");
        cuerpo.transform.localScale = new Vector3(tamanoBichito, tamanoBichito, 1f);
        SpriteRenderer dibujo = cuerpo.AddComponent<SpriteRenderer>();
        dibujo.sprite = ObtenerSpriteRedondo();
        dibujo.color = colorBichito;
        dibujo.sortingOrder = 8;

        for (int indice = 0; indice < 2; indice++)
        {
            GameObject ojo = new GameObject("Ojo");
            ojo.transform.SetParent(cuerpo.transform);
            float lado = -0.18f;
            if (indice == 1)
            {
                lado = 0.18f;
            }
            ojo.transform.localPosition = new Vector3(lado, 0.12f, 0f);
            ojo.transform.localScale = new Vector3(0.2f, 0.28f, 1f);
            SpriteRenderer dibujoOjo = ojo.AddComponent<SpriteRenderer>();
            dibujoOjo.sprite = ObtenerSpriteRedondo();
            dibujoOjo.color = Color.white;
            dibujoOjo.sortingOrder = 9;
        }
        return cuerpo;
    }

    // Sin dibujo redondo configurado se usa el cuadrado base.
    private Sprite ObtenerSpriteRedondo()
    {
        if (spriteBichito != null)
        {
            return spriteBichito;
        }
        return SpriteBase;
    }
}
