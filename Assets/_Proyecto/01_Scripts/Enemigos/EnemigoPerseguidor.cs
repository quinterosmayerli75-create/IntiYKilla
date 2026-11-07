// =====================================================================
// EnemigoPerseguidor.cs
// Proyecto: Inti y Killa
// Descripción: Persigue al jugador más cercano mientras esté dentro de su zona
//              (avispas del panal) y vuelve a su lugar cuando nadie está ahí.
//              Es más lento que los jugadores: quien sigue moviéndose escapa.
//              No persigue a quien ya espera en su puerta (regla de justicia).
//              Se aturde si lo pisan.
// =====================================================================

using UnityEngine;

public class EnemigoPerseguidor : EnemigoBase
{
    [SerializeField] private Vector2 centroZona = Vector2.zero;
    [SerializeField] private Vector2 tamanoZona = new Vector2(8f, 6f);
    [SerializeField] private float velocidad = 3f;
    [SerializeField] private float amplitudZumbido = 0.12f;

    private ControladorJugador[] todosLosJugadores;
    private PuertaSalida[] puertas;
    private Vector3 posicionDeCasa;
    private Vector3 posicionSinZumbido;

    public void Configurar(Vector2 nuevoCentro, Vector2 nuevoTamano, float nuevaVelocidad)
    {
        centroZona = nuevoCentro;
        tamanoZona = nuevoTamano;
        velocidad = nuevaVelocidad;
    }

    protected override void Awake()
    {
        base.Awake();
        posicionDeCasa = transform.position;
        posicionSinZumbido = transform.position;
        todosLosJugadores = FindObjectsByType<ControladorJugador>();
        puertas = FindObjectsByType<PuertaSalida>();
    }

    protected override void MoverEnemigo()
    {
        Vector3 destino = posicionDeCasa;
        ControladorJugador objetivo = BuscarObjetivo();
        if (objetivo != null)
        {
            destino = objetivo.LimitesCuerpo.center;
        }

        posicionSinZumbido = Vector3.MoveTowards(posicionSinZumbido, destino, velocidad * Time.deltaTime);

        // Un temblor pequeño: así se ve como una avispa y no como una pelota.
        float zumbido = Mathf.Sin(Time.time * 25f) * amplitudZumbido;
        transform.position = posicionSinZumbido + new Vector3(0f, zumbido, 0f);
    }

    private ControladorJugador BuscarObjetivo()
    {
        Rect zona = new Rect(centroZona - tamanoZona / 2f, tamanoZona);
        ControladorJugador masCercano = null;
        float menorDistancia = float.MaxValue;

        foreach (ControladorJugador jugador in todosLosJugadores)
        {
            if (jugador == null || jugador.EstaVivo == false || EstaEnSuPuerta(jugador) == true)
            {
                continue;
            }

            Vector2 posicion = jugador.LimitesCuerpo.center;
            if (zona.Contains(posicion) == false)
            {
                continue;
            }

            float distancia = Vector2.Distance(posicion, posicionSinZumbido);
            if (distancia < menorDistancia)
            {
                masCercano = jugador;
                menorDistancia = distancia;
            }
        }

        return masCercano;
    }

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
}
