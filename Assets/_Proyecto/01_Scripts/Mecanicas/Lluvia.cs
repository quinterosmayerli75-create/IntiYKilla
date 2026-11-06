// =====================================================================
// Lluvia.cs
// Proyecto: Inti y Killa
// Descripción: Lluvia de los Yungas, que se repite en ciclo:
//              seco → aviso (se oscurece y caen las primeras gotas) → lluvia.
//              Mientras llueve, quien tiene el poder del fuego se va apagando
//              si no tiene un techo encima, y al poco tiempo muere. Se pone azul
//              para avisar, y se recupera apenas vuelve a estar a salvo.
//              Quien tiene el poder del agua puede mojarse sin problema.
// Uso: un solo objeto en la escena, creado por el generador.
// =====================================================================

using UnityEngine;

public enum EstadoLluvia
{
    Seco,
    Aviso,
    Lloviendo
}

public class Lluvia : MonoBehaviour
{
    [Header("Tiempos del ciclo")]
    [SerializeField] private float segundosSeco = 5.5f;
    [SerializeField] private float segundosDeAviso = 1.5f;
    [SerializeField] private float segundosLloviendo = 3.5f;

    [Header("Peligro")]
    [SerializeField] private float segundosParaApagarse = 1.2f;
    [SerializeField] private string mensajeDeMuerte = "La lluvia apagó a {0}. Pssss...";

    [Header("Aspecto")]
    [SerializeField] private Sprite spriteGota;
    [SerializeField] private Material materialSinLuces;
    [SerializeField] private int cantidadGotas = 90;
    [SerializeField] private float velocidadGotas = 24f;
    [SerializeField] private float oscuridadMaxima = 0.3f;
    [SerializeField] private Vector2 zona = new Vector2(38f, 24f);
    [SerializeField] private Vector2 centroZona = new Vector2(0f, 4f);

    private ControladorJugador[] jugadores;
    private float[] exposicionDeJugadores;
    private Transform[] gotas;
    private SpriteRenderer[] dibujosGotas;
    private SpriteRenderer capaOscura;
    private float tiempoEnEstado = 0f;
    private bool yaSeAviso = false;

    public EstadoLluvia Estado { get; private set; }

    public void Configurar(Sprite nuevoSpriteGota, Material nuevoMaterial)
    {
        spriteGota = nuevoSpriteGota;
        materialSinLuces = nuevoMaterial;
    }

    private void Awake()
    {
        jugadores = FindObjectsByType<ControladorJugador>();
        exposicionDeJugadores = new float[jugadores.Length];
        Estado = EstadoLluvia.Seco;
        CrearDibujos();
    }

    private void CrearDibujos()
    {
        capaOscura = CrearDibujo("Cielo oscuro", 14);
        capaOscura.transform.position = new Vector3(centroZona.x, centroZona.y, 0f);
        capaOscura.transform.localScale = new Vector3(zona.x, zona.y, 1f) / spriteGota.bounds.size.x;
        capaOscura.color = new Color(0.05f, 0.08f, 0.18f, 0f);

        gotas = new Transform[cantidadGotas];
        dibujosGotas = new SpriteRenderer[cantidadGotas];
        Vector3 escalaGota = new Vector3(0.05f, 0.6f, 1f) / spriteGota.bounds.size.x;

        for (int indice = 0; indice < cantidadGotas; indice++)
        {
            SpriteRenderer dibujoGota = CrearDibujo("Gota", 14);
            dibujoGota.color = new Color(0.72f, 0.85f, 1f, 0.75f);
            dibujoGota.transform.localScale = escalaGota;
            dibujoGota.transform.position = PosicionAlAzar(Random.Range(0f, 1f));
            dibujoGota.enabled = false;
            gotas[indice] = dibujoGota.transform;
            dibujosGotas[indice] = dibujoGota;
        }
    }

    private SpriteRenderer CrearDibujo(string nombre, int orden)
    {
        GameObject objeto = new GameObject(nombre);
        objeto.transform.SetParent(transform);
        SpriteRenderer dibujo = objeto.AddComponent<SpriteRenderer>();
        dibujo.sprite = spriteGota;
        dibujo.sortingOrder = orden;
        if (materialSinLuces != null)
        {
            dibujo.sharedMaterial = materialSinLuces;
        }
        return dibujo;
    }

    private Vector3 PosicionAlAzar(float alturaRelativa)
    {
        float posicionX = centroZona.x + Random.Range(-zona.x / 2f, zona.x / 2f);
        float posicionY = centroZona.y - zona.y / 2f + zona.y * alturaRelativa;
        return new Vector3(posicionX, posicionY, 0f);
    }

    private void Update()
    {
        AvanzarCiclo();
        ActualizarDibujos();
        RevisarJugadores();
    }

    private void AvanzarCiclo()
    {
        tiempoEnEstado = tiempoEnEstado + Time.deltaTime;

        if (Estado == EstadoLluvia.Seco && tiempoEnEstado >= segundosSeco)
        {
            CambiarEstado(EstadoLluvia.Aviso);

            // Solo la primera vez: después el cielo oscuro ya se reconoce.
            if (yaSeAviso == false && GestorNivel.Instancia != null)
            {
                GestorNivel.Instancia.MostrarAviso("¡Se viene la lluvia! Quien tiene el fuego debe buscar un techo.", 2.5f);
                yaSeAviso = true;
            }
        }
        else if (Estado == EstadoLluvia.Aviso && tiempoEnEstado >= segundosDeAviso)
        {
            CambiarEstado(EstadoLluvia.Lloviendo);
        }
        else if (Estado == EstadoLluvia.Lloviendo && tiempoEnEstado >= segundosLloviendo)
        {
            CambiarEstado(EstadoLluvia.Seco);
        }
    }

    private void CambiarEstado(EstadoLluvia nuevoEstado)
    {
        Estado = nuevoEstado;
        tiempoEnEstado = 0f;
    }

    private void ActualizarDibujos()
    {
        float oscuridadBuscada = 0f;
        // En el aviso caen pocas gotas; con la lluvia, todas.
        int gotasVisibles = 0;

        if (Estado == EstadoLluvia.Aviso)
        {
            oscuridadBuscada = oscuridadMaxima * 0.7f;
            gotasVisibles = cantidadGotas / 6;
        }
        else if (Estado == EstadoLluvia.Lloviendo)
        {
            oscuridadBuscada = oscuridadMaxima;
            gotasVisibles = cantidadGotas;
        }

        Color color = capaOscura.color;
        color.a = Mathf.MoveTowards(color.a, oscuridadBuscada, Time.deltaTime);
        capaOscura.color = color;

        float piso = centroZona.y - zona.y / 2f;
        for (int indice = 0; indice < cantidadGotas; indice++)
        {
            dibujosGotas[indice].enabled = indice < gotasVisibles;
            gotas[indice].position = gotas[indice].position + new Vector3(-1.5f, -velocidadGotas, 0f) * Time.deltaTime;
            if (gotas[indice].position.y < piso)
            {
                gotas[indice].position = PosicionAlAzar(1f);
            }
        }
    }

    private void RevisarJugadores()
    {
        for (int indice = 0; indice < jugadores.Length; indice++)
        {
            ControladorJugador jugador = jugadores[indice];
            if (jugador.EstaVivo == false)
            {
                continue;
            }

            bool seEstaMojando = Estado == EstadoLluvia.Lloviendo && jugador.Elemento == TipoElemento.Fuego && TieneTechoEncima(jugador) == false;
            if (seEstaMojando == true)
            {
                exposicionDeJugadores[indice] = exposicionDeJugadores[indice] + Time.deltaTime;
            }
            else
            {
                // Se recupera el doble de rápido: un descuido corto no se paga caro.
                exposicionDeJugadores[indice] = Mathf.Max(0f, exposicionDeJugadores[indice] - Time.deltaTime * 2f);
            }

            jugador.MostrarApagandose(exposicionDeJugadores[indice] / segundosParaApagarse);

            if (exposicionDeJugadores[indice] >= segundosParaApagarse)
            {
                jugador.Morir(mensajeDeMuerte.Replace("{0}", jugador.NombreJugador));
            }
        }
    }

    // Hay techo si algo sólido está por encima de la cabeza. Se ignoran los
    // charcos y zonas (triggers) y al compañero, que no sirve de paraguas.
    private bool TieneTechoEncima(ControladorJugador jugador)
    {
        Bounds cuerpo = jugador.LimitesCuerpo;
        Vector2 inicio = new Vector2(cuerpo.center.x, cuerpo.max.y + 0.05f);
        RaycastHit2D[] golpes = Physics2D.RaycastAll(inicio, Vector2.up, 30f);

        foreach (RaycastHit2D golpe in golpes)
        {
            bool esUnJugador = golpe.collider.GetComponent<ControladorJugador>() != null;
            if (golpe.collider.isTrigger == false && esUnJugador == false)
            {
                return true;
            }
        }

        return false;
    }
}
