// =====================================================================
// EnemigoEnPicada.cs
// Proyecto: Inti y Killa
// Descripción: Espera arriba (el cóndor) y se lanza contra el jugador más cercano:
//              1) Espera hasta que alguien pasa debajo.
//              2) Aviso: tiembla y se pone rojo (regla de justicia).
//              3) Se lanza en línea recta hacia donde estaba el jugador.
//              4) Vuelve despacio a su lugar y descansa.
//              Quien sigue caminando se salva; quien se queda quieto, no.
//              Como todo enemigo, se aturde si lo pisan.
// =====================================================================

using UnityEngine;

public class EnemigoEnPicada : EnemigoBase
{
    private enum EstadoPicada
    {
        Esperando,
        Avisando,
        Lanzandose,
        Volviendo,
        Descansando
    }

    [Header("Detección")]
    [SerializeField] private float rangoHorizontal = 4f;
    [SerializeField] private float rangoVertical = 8f;

    [Header("Ataque")]
    [SerializeField] private float segundosDeAviso = 0.7f;
    [SerializeField] private float velocidadPicada = 13f;
    [SerializeField] private float velocidadRegreso = 4f;
    [SerializeField] private float segundosDeDescanso = 2.5f;
    [SerializeField] private float intensidadTemblor = 0.1f;
    [SerializeField] private Color colorDeAviso = new Color(1f, 0.25f, 0.2f);

    private EstadoPicada estado = EstadoPicada.Esperando;
    private ControladorJugador[] todosLosJugadores;
    private Vector3 posicionDeEspera;
    private Vector3 destino;
    private float tiempoEnEstado = 0f;

    public void Configurar(float nuevoRangoHorizontal, float nuevoRangoVertical)
    {
        rangoHorizontal = nuevoRangoHorizontal;
        rangoVertical = nuevoRangoVertical;
    }

    protected override void Awake()
    {
        base.Awake();
        posicionDeEspera = transform.position;
        todosLosJugadores = FindObjectsByType<ControladorJugador>();
    }

    protected override void MoverEnemigo()
    {
        tiempoEnEstado = tiempoEnEstado + Time.deltaTime;

        if (estado == EstadoPicada.Esperando)
        {
            if (BuscarJugadorCercano() != null)
            {
                CambiarEstado(EstadoPicada.Avisando);
                PintarDibujo(colorDeAviso);
            }
        }
        else if (estado == EstadoPicada.Avisando)
        {
            Vector2 temblor = Random.insideUnitCircle * intensidadTemblor;
            transform.position = posicionDeEspera + new Vector3(temblor.x, temblor.y, 0f);

            if (tiempoEnEstado >= segundosDeAviso)
            {
                Lanzarse();
            }
        }
        else if (estado == EstadoPicada.Lanzandose)
        {
            MoverHacia(destino, velocidadPicada, EstadoPicada.Volviendo);
        }
        else if (estado == EstadoPicada.Volviendo)
        {
            MoverHacia(posicionDeEspera, velocidadRegreso, EstadoPicada.Descansando);
        }
        else if (tiempoEnEstado >= segundosDeDescanso)
        {
            CambiarEstado(EstadoPicada.Esperando);
        }
    }

    private void Lanzarse()
    {
        transform.position = posicionDeEspera;
        RestaurarColorDibujo();

        ControladorJugador objetivo = BuscarJugadorCercano();
        if (objetivo == null)
        {
            // Se alejó durante el aviso: se queda sin atacar.
            CambiarEstado(EstadoPicada.Descansando);
            return;
        }

        destino = objetivo.transform.position;
        CambiarEstado(EstadoPicada.Lanzandose);
    }

    private void MoverHacia(Vector3 punto, float velocidad, EstadoPicada estadoAlLlegar)
    {
        transform.position = Vector3.MoveTowards(transform.position, punto, velocidad * Time.deltaTime);
        if (Vector3.Distance(transform.position, punto) < 0.01f)
        {
            CambiarEstado(estadoAlLlegar);
        }
    }

    private void CambiarEstado(EstadoPicada nuevoEstado)
    {
        estado = nuevoEstado;
        tiempoEnEstado = 0f;
    }

    private ControladorJugador BuscarJugadorCercano()
    {
        ControladorJugador masCercano = null;
        float menorDistancia = float.MaxValue;

        foreach (ControladorJugador jugador in todosLosJugadores)
        {
            if (jugador == null || jugador.EstaVivo == false)
            {
                continue;
            }

            Vector3 posicion = jugador.transform.position;
            float distanciaX = Mathf.Abs(posicion.x - posicionDeEspera.x);
            float distanciaY = posicionDeEspera.y - posicion.y;
            bool estaEnRango = distanciaX <= rangoHorizontal && distanciaY >= 0f && distanciaY <= rangoVertical;

            if (estaEnRango == true && distanciaX < menorDistancia)
            {
                masCercano = jugador;
                menorDistancia = distanciaX;
            }
        }

        return masCercano;
    }
}
