// =====================================================================
// GuardianEclipse.cs
// Proyecto: Inti y Killa
// Descripción: Jefe final (nivel 6-5). Vuela de lado a lado y lanza bolas de
//              fuego y de agua, que solo dañan al elemento contrario: el fuego
//              al que tiene el agua, y el agua al que tiene el fuego. Tiene 3
//              vidas y pierde una cuando los DOS jugadores pisan a la vez los
//              dos botones del escenario. Después de cada golpe parpadea un
//              momento (no se le puede volver a golpear) y se vuelve más rápido.
//              Al vencerlo activa lo que lleva a la salida (en 6-5, un escalón).
// =====================================================================

using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class GuardianEclipse : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float limiteIzquierdo = -10f;
    [SerializeField] private float limiteDerecho = 10f;
    [SerializeField] private float velocidad = 3f;
    [SerializeField] private float aumentoDeVelocidadPorGolpe = 1.3f;

    [Header("Ataque")]
    [SerializeField] private float segundosEntreDisparos = 1.8f;
    [SerializeField] private float segundosDeVuelo = 1.2f;
    // Antes de cada disparo brilla del color de la bola (regla de justicia).
    [SerializeField] private float segundosDeAviso = 0.5f;
    [SerializeField] private float gravedadProyectil = 12f;
    [SerializeField] private Sprite spriteProyectil;

    [Header("Vidas")]
    [SerializeField] private int vidas = 3;
    [SerializeField] private float segundosInvulnerable = 2.5f;
    [SerializeField] private BotonPresion botonUno;
    [SerializeField] private BotonPresion botonDos;
    [SerializeField] private ObjetoActivable[] alSerVencido;
    [SerializeField] private Vector2 tamanoCuerpo = new Vector2(2f, 2f);

    private SpriteRenderer dibujo;
    private ControladorJugador[] jugadores;
    private Color colorNormal;
    private float direccion = 1f;
    private float tiempoDesdeDisparo = 0f;
    private float tiempoInvulnerable = 0f;
    private bool siguienteEsFuego = true;
    private bool botonesPisadosAntes = false;
    private bool estaVencido = false;

    public int Vidas
    {
        get { return vidas; }
    }

    public float SegundosDesdeDisparo
    {
        get { return tiempoDesdeDisparo; }
    }

    public void Configurar(BotonPresion nuevoBotonUno, BotonPresion nuevoBotonDos, ObjetoActivable[] nuevosObjetivos, Sprite nuevoSprite)
    {
        botonUno = nuevoBotonUno;
        botonDos = nuevoBotonDos;
        alSerVencido = nuevosObjetivos;
        spriteProyectil = nuevoSprite;
    }

    private void Awake()
    {
        dibujo = GetComponent<SpriteRenderer>();
        jugadores = FindObjectsByType<ControladorJugador>();
        colorNormal = dibujo.color;
    }

    private void Update()
    {
        if (estaVencido == true)
        {
            return;
        }

        Mover();
        RevisarBotones();
        Parpadear();
        MatarAlQueToque();

        tiempoDesdeDisparo = tiempoDesdeDisparo + Time.deltaTime;
        if (tiempoDesdeDisparo >= segundosEntreDisparos)
        {
            tiempoDesdeDisparo = 0f;
            Disparar();
        }
    }

    private void Mover()
    {
        float posicionX = transform.position.x + direccion * velocidad * Time.deltaTime;
        if (posicionX >= limiteDerecho)
        {
            posicionX = limiteDerecho;
            direccion = -1f;
        }
        else if (posicionX <= limiteIzquierdo)
        {
            posicionX = limiteIzquierdo;
            direccion = 1f;
        }
        transform.position = new Vector3(posicionX, transform.position.y, transform.position.z);
    }

    // Cuenta solo el momento en que los dos botones quedan pisados a la vez.
    private void RevisarBotones()
    {
        bool ambosPisados = botonUno.EstaPresionado == true && botonDos.EstaPresionado == true;
        if (ambosPisados == true && botonesPisadosAntes == false && tiempoInvulnerable <= 0f)
        {
            RecibirGolpe();
        }
        botonesPisadosAntes = ambosPisados;
    }

    private void RecibirGolpe()
    {
        vidas = vidas - 1;
        tiempoInvulnerable = segundosInvulnerable;
        velocidad = velocidad * aumentoDeVelocidadPorGolpe;

        if (vidas <= 0)
        {
            Vencer();
            return;
        }

        if (GestorNivel.Instancia != null)
        {
            GestorNivel.Instancia.MostrarAviso("¡Golpe al Guardián! Le quedan " + vidas + " vidas.", 2f);
        }
    }

    private void Vencer()
    {
        estaVencido = true;
        dibujo.enabled = false;
        foreach (Transform hijo in transform)
        {
            hijo.gameObject.SetActive(false);
        }

        foreach (ObjetoActivable objetivo in alSerVencido)
        {
            objetivo.RecibirSenal(true);
        }

        if (GestorNivel.Instancia != null)
        {
            GestorNivel.Instancia.MostrarAviso("¡Vencieron al Guardián del Eclipse! Suban a sus puertas.", 4f);
        }
    }

    private void Parpadear()
    {
        tiempoInvulnerable = Mathf.Max(0f, tiempoInvulnerable - Time.deltaTime);
        bool mostrarTenue = tiempoInvulnerable > 0f && Mathf.Repeat(tiempoInvulnerable * 8f, 1f) < 0.5f;
        bool vaADisparar = tiempoDesdeDisparo >= segundosEntreDisparos - segundosDeAviso;
        if (mostrarTenue == true)
        {
            dibujo.color = new Color(colorNormal.r, colorNormal.g, colorNormal.b, 0.35f);
        }
        else if (vaADisparar == true)
        {
            TipoElemento proximo = TipoElemento.Agua;
            if (siguienteEsFuego == true)
            {
                proximo = TipoElemento.Fuego;
            }
            dibujo.color = ColoresElemento.ObtenerColor(proximo);
        }
        else
        {
            dibujo.color = colorNormal;
        }
    }

    private void MatarAlQueToque()
    {
        Vector2 centro = transform.position;
        foreach (ControladorJugador jugador in jugadores)
        {
            if (jugador.EstaVivo == false)
            {
                continue;
            }

            Bounds cuerpo = jugador.LimitesCuerpo;
            bool cruzaEnX = cuerpo.max.x > centro.x - tamanoCuerpo.x / 2f && cuerpo.min.x < centro.x + tamanoCuerpo.x / 2f;
            bool cruzaEnY = cuerpo.max.y > centro.y - tamanoCuerpo.y / 2f && cuerpo.min.y < centro.y + tamanoCuerpo.y / 2f;
            if (cruzaEnX == true && cruzaEnY == true)
            {
                jugador.Morir("El Guardián del Eclipse atrapó a " + jugador.NombreJugador + ".");
            }
        }
    }

    // Alterna fuego y agua. El fuego busca a quien tiene el agua, y al revés.
    private void Disparar()
    {
        TipoElemento elementoDelDisparo = TipoElemento.Agua;
        if (siguienteEsFuego == true)
        {
            elementoDelDisparo = TipoElemento.Fuego;
        }
        siguienteEsFuego = !siguienteEsFuego;

        ControladorJugador objetivo = null;
        foreach (ControladorJugador jugador in jugadores)
        {
            if (jugador.EstaVivo == true && jugador.Elemento != elementoDelDisparo)
            {
                objetivo = jugador;
            }
        }

        if (objetivo == null)
        {
            return;
        }

        Vector2 origen = transform.position;
        Vector2 distancia = (Vector2)objetivo.transform.position - origen;
        float velocidadX = distancia.x / segundosDeVuelo;
        float velocidadY = distancia.y / segundosDeVuelo + gravedadProyectil * segundosDeVuelo / 2f;

        GameObject objeto = new GameObject("Proyectil del Guardian");
        objeto.transform.position = transform.position;
        objeto.transform.localScale = new Vector3(0.6f, 0.6f, 1f);
        SpriteRenderer dibujoProyectil = objeto.AddComponent<SpriteRenderer>();
        dibujoProyectil.sprite = spriteProyectil;
        dibujoProyectil.color = ColoresElemento.ObtenerColor(elementoDelDisparo);
        dibujoProyectil.sortingOrder = 9;

        string mensaje = "Una bola de agua apagó a {0}.";
        if (elementoDelDisparo == TipoElemento.Fuego)
        {
            mensaje = "Una bola de fuego alcanzó a {0}.";
        }

        ProyectilArco proyectil = objeto.AddComponent<ProyectilArco>();
        proyectil.Iniciar(new Vector2(velocidadX, velocidadY), gravedadProyectil, 0.3f, mensaje);
        proyectil.DaniarSoloAlContrarioDe(elementoDelDisparo);
    }
}
