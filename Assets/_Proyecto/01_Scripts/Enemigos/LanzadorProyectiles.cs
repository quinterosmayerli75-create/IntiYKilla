// =====================================================================
// LanzadorProyectiles.cs
// Proyecto: Inti y Killa
// Descripción: Desde un lugar fijo lanza objetos en arco cada cierto tiempo
//              (monos con cocos, llamas que escupen). Antes de lanzar tiembla
//              y se pone rojo (regla de justicia). Apunta a donde está el jugador
//              en ese momento: quien sigue moviéndose no recibe el golpe.
//              Se aturde si lo pisan, y mientras tanto no lanza nada.
// =====================================================================

using UnityEngine;

public class LanzadorProyectiles : EnemigoBase
{
    [Header("Lanzamiento")]
    [SerializeField] private float alcanceHorizontal = 7f;
    [SerializeField] private float segundosEntreLanzamientos = 3.5f;
    [SerializeField] private float segundosDeAviso = 0.6f;
    [SerializeField] private float segundosDeVuelo = 1.1f;
    [SerializeField] private float gravedadProyectil = 14f;
    [SerializeField] private float intensidadTemblor = 0.07f;
    [SerializeField] private Color colorDeAviso = new Color(1f, 0.3f, 0.2f);

    [Header("Proyectil")]
    [SerializeField] private Sprite spriteProyectil;
    [SerializeField] private Color colorProyectil = new Color(0.45f, 0.28f, 0.14f);
    [SerializeField] private float tamanoProyectil = 0.45f;
    [SerializeField] private string mensajeDelProyectil = "¡Un mono le tiró un coco a {0}!";

    private ControladorJugador[] todosLosJugadores;
    private PuertaSalida[] puertas;
    private Vector3 posicionOriginal;
    private float tiempoDeEspera = 0f;
    private float tiempoDeAviso = 0f;
    private bool estaAvisando = false;

    public void Configurar(float nuevoAlcance, float nuevosSegundos, Sprite nuevoSprite)
    {
        alcanceHorizontal = nuevoAlcance;
        segundosEntreLanzamientos = nuevosSegundos;
        spriteProyectil = nuevoSprite;
    }

    // Para que el mismo lanzador sirva para monos con cocos y llamas que escupen.
    public void ConfigurarProyectil(Color nuevoColor, float nuevoTamano, string nuevoMensaje)
    {
        colorProyectil = nuevoColor;
        tamanoProyectil = nuevoTamano;
        mensajeDelProyectil = nuevoMensaje;
    }

    protected override void Awake()
    {
        base.Awake();
        posicionOriginal = transform.position;
        todosLosJugadores = FindObjectsByType<ControladorJugador>();
        puertas = FindObjectsByType<PuertaSalida>();
    }

    protected override void MoverEnemigo()
    {
        if (estaAvisando == true)
        {
            Avisar();
            return;
        }

        tiempoDeEspera = tiempoDeEspera + Time.deltaTime;
        if (tiempoDeEspera >= segundosEntreLanzamientos && BuscarObjetivo() != null)
        {
            estaAvisando = true;
            tiempoDeAviso = 0f;
            PintarDibujo(colorDeAviso);
        }
    }

    private void Avisar()
    {
        tiempoDeAviso = tiempoDeAviso + Time.deltaTime;
        Vector2 temblor = Random.insideUnitCircle * intensidadTemblor;
        transform.position = posicionOriginal + new Vector3(temblor.x, temblor.y, 0f);

        if (tiempoDeAviso >= segundosDeAviso)
        {
            estaAvisando = false;
            tiempoDeEspera = 0f;
            transform.position = posicionOriginal;
            RestaurarColorDibujo();

            ControladorJugador objetivo = BuscarObjetivo();
            if (objetivo != null)
            {
                Lanzar(objetivo.transform.position);
            }
        }
    }

    private void Lanzar(Vector2 destino)
    {
        // Fórmula del tiro: la velocidad que hace caer el objeto justo en el
        // destino después de "segundosDeVuelo", sin importar la distancia.
        Vector2 origen = posicionOriginal;
        Vector2 distancia = destino - origen;
        float velocidadX = distancia.x / segundosDeVuelo;
        float velocidadY = distancia.y / segundosDeVuelo + gravedadProyectil * segundosDeVuelo / 2f;

        GameObject objeto = new GameObject("Proyectil");
        objeto.transform.position = posicionOriginal;
        objeto.transform.localScale = new Vector3(tamanoProyectil, tamanoProyectil, 1f);

        SpriteRenderer dibujo = objeto.AddComponent<SpriteRenderer>();
        dibujo.sprite = spriteProyectil;
        dibujo.color = colorProyectil;
        dibujo.sortingOrder = 9;

        ProyectilArco proyectil = objeto.AddComponent<ProyectilArco>();
        proyectil.Iniciar(new Vector2(velocidadX, velocidadY), gravedadProyectil, tamanoProyectil / 2f, mensajeDelProyectil);
    }

    // No se le lanza a quien espera en su puerta: sería injusto castigarlo por
    // lo que tarda su compañero (regla de justicia).
    private bool EstaEnSuPuerta(ControladorJugador jugador)
    {
        foreach (PuertaSalida puerta in puertas)
        {
            if (puerta.EstaEsperando(jugador) == true)
            {
                return true;
            }
        }
        return false;
    }

    private ControladorJugador BuscarObjetivo()
    {
        ControladorJugador masCercano = null;
        float menorDistancia = alcanceHorizontal;

        foreach (ControladorJugador jugador in todosLosJugadores)
        {
            if (jugador == null || jugador.EstaVivo == false || EstaEnSuPuerta(jugador) == true)
            {
                continue;
            }

            float distancia = Mathf.Abs(jugador.transform.position.x - posicionOriginal.x);
            if (distancia <= menorDistancia)
            {
                masCercano = jugador;
                menorDistancia = distancia;
            }
        }

        return masCercano;
    }
}
