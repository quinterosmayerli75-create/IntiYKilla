// =====================================================================
// CastigoTraicion.cs
// Proyecto: Inti y Killa
// Descripción: El castigo que cae sobre el compañero cuando alguien usa una
//              trampa de traición. Se crea mientras el juego corre:
//              1) Aparece una marca roja sobre el lugar donde está la víctima.
//              2) Después del aviso, el castigo cae en ESE lugar.
//              Regla de justicia: la víctima siempre tiene tiempo de moverse.
// =====================================================================

using System.Collections;
using UnityEngine;

public enum TipoCastigo
{
    CaeLava,
    SalenPuas,
    SeAbreElSuelo
}

public class CastigoTraicion : MonoBehaviour
{
    private const float anchoCastigo = 1.8f;
    private const float altoPuas = 0.6f;
    private const float duracionPuas = 1f;
    private const float duracionAgujero = 1.2f;
    private const float alturaInicialLava = 7f;
    private const float velocidadLava = 18f;

    private static readonly Color colorMarca = new Color(1f, 0.1f, 0.1f, 0.9f);
    private static readonly Color colorLava = new Color(1f, 0.30f, 0.05f);
    private static readonly Color colorPuas = new Color(0.90f, 0.95f, 1f);
    private static readonly Color colorAgujero = new Color(0.05f, 0.03f, 0.08f);

    private ControladorJugador victima;
    private ControladorJugador[] jugadores;
    private TipoCastigo tipo;
    private float tiempoDeAviso;
    private Sprite spriteBase;
    private string mensaje;

    // Punto de entrada: todas las trampas de traición llaman a este método.
    public static void Aplicar(ControladorJugador causante, ControladorJugador nuevaVictima, TipoCastigo nuevoTipo, float nuevoTiempoDeAviso, Sprite nuevoSprite)
    {
        if (causante == null || nuevaVictima == null || nuevaVictima.EstaVivo == false)
        {
            return;
        }

        GameObject objeto = new GameObject("Castigo de traicion");
        CastigoTraicion castigo = objeto.AddComponent<CastigoTraicion>();
        castigo.victima = nuevaVictima;
        castigo.tipo = nuevoTipo;
        castigo.tiempoDeAviso = nuevoTiempoDeAviso;
        castigo.spriteBase = nuevoSprite;
        castigo.mensaje = MensajesDeMuerte.DeTraicion(causante.NombreJugador, nuevaVictima.NombreJugador);
    }

    private void Start()
    {
        jugadores = FindObjectsByType<ControladorJugador>();
        StartCoroutine(Ejecutar());
    }

    private IEnumerator Ejecutar()
    {
        // El castigo apunta a donde estaba la víctima al empezar el aviso.
        Vector2 objetivo = victima.LimitesCuerpo.center;
        float pisoBajoVictima = victima.LimitesCuerpo.min.y;
        Collider2D piso = BuscarPisoDebajo(objetivo);

        GameObject marca = CrearRectangulo("Aviso", new Vector2(objetivo.x, objetivo.y + 1.3f), new Vector2(0.3f, 0.6f), colorMarca, 20);
        yield return new WaitForSeconds(tiempoDeAviso);
        Destroy(marca);

        if (tipo == TipoCastigo.CaeLava)
        {
            yield return CaerLava(objetivo);
        }
        else if (tipo == TipoCastigo.SeAbreElSuelo && piso != null)
        {
            yield return AbrirSuelo(objetivo, pisoBajoVictima, piso);
        }
        else
        {
            yield return SacarPuas(objetivo, pisoBajoVictima);
        }

        Destroy(gameObject);
    }

    private IEnumerator CaerLava(Vector2 objetivo)
    {
        Vector2 tamano = new Vector2(anchoCastigo, 0.8f);
        GameObject lava = CrearRectangulo("Lava que cae", new Vector2(objetivo.x, objetivo.y + alturaInicialLava), tamano, colorLava, 15);

        float alturaFinal = objetivo.y - 3f;
        while (lava.transform.position.y > alturaFinal)
        {
            lava.transform.position = lava.transform.position + new Vector3(0f, -velocidadLava * Time.deltaTime, 0f);
            MatarAQuienToque(lava.transform.position, tamano);
            yield return null;
        }

        Destroy(lava);
    }

    private IEnumerator SacarPuas(Vector2 objetivo, float alturaPiso)
    {
        Vector2 tamano = new Vector2(anchoCastigo, altoPuas);
        Vector2 centro = new Vector2(objetivo.x, alturaPiso + altoPuas / 2f);
        GameObject puas = CrearRectangulo("Puas de traicion", centro, tamano, colorPuas, 15);

        float tiempo = 0f;
        while (tiempo < duracionPuas)
        {
            tiempo = tiempo + Time.deltaTime;
            MatarAQuienToque(centro, tamano);
            yield return null;
        }

        Destroy(puas);
    }

    // El piso sigue ahí, pero deja de chocar con quien esté sobre el agujero.
    private IEnumerator AbrirSuelo(Vector2 objetivo, float alturaPiso, Collider2D piso)
    {
        GameObject agujero = CrearRectangulo("Agujero", new Vector2(objetivo.x, alturaPiso - 0.25f), new Vector2(anchoCastigo, 0.5f), colorAgujero, 15);
        float mitadAncho = anchoCastigo / 2f;

        float tiempo = 0f;
        while (tiempo < duracionAgujero)
        {
            tiempo = tiempo + Time.deltaTime;
            foreach (ControladorJugador jugador in jugadores)
            {
                if (jugador != null)
                {
                    Bounds cuerpo = jugador.LimitesCuerpo;
                    bool cabeEnElAgujero = cuerpo.min.x > objetivo.x - mitadAncho && cuerpo.max.x < objetivo.x + mitadAncho;
                    Physics2D.IgnoreCollision(jugador.GetComponent<Collider2D>(), piso, cabeEnElAgujero);
                    if (cabeEnElAgujero == true)
                    {
                        jugador.RecordarTrampa(mensaje);
                    }
                }
            }
            yield return null;
        }

        foreach (ControladorJugador jugador in jugadores)
        {
            if (jugador != null)
            {
                Physics2D.IgnoreCollision(jugador.GetComponent<Collider2D>(), piso, false);
            }
        }

        Destroy(agujero);
    }

    private Collider2D BuscarPisoDebajo(Vector2 desde)
    {
        RaycastHit2D[] golpes = Physics2D.RaycastAll(desde, Vector2.down, 3f);
        foreach (RaycastHit2D golpe in golpes)
        {
            bool esSolido = golpe.collider.isTrigger == false;
            bool esJugador = golpe.collider.GetComponent<ControladorJugador>() != null;
            if (esSolido == true && esJugador == false)
            {
                return golpe.collider;
            }
        }

        return null;
    }

    private void MatarAQuienToque(Vector2 centro, Vector2 tamano)
    {
        foreach (ControladorJugador jugador in jugadores)
        {
            if (jugador != null && jugador.EstaVivo == true)
            {
                Bounds cuerpo = jugador.LimitesCuerpo;
                bool cruzaEnX = cuerpo.max.x > centro.x - tamano.x / 2f && cuerpo.min.x < centro.x + tamano.x / 2f;
                bool cruzaEnY = cuerpo.max.y > centro.y - tamano.y / 2f && cuerpo.min.y < centro.y + tamano.y / 2f;
                if (cruzaEnX == true && cruzaEnY == true)
                {
                    jugador.Morir(mensaje);
                }
            }
        }
    }

    private GameObject CrearRectangulo(string nombre, Vector2 centro, Vector2 tamano, Color color, int ordenDeDibujo)
    {
        GameObject objeto = new GameObject(nombre);
        objeto.transform.SetParent(transform);
        objeto.transform.position = new Vector3(centro.x, centro.y, 0f);
        objeto.transform.localScale = new Vector3(tamano.x, tamano.y, 1f);

        SpriteRenderer dibujo = objeto.AddComponent<SpriteRenderer>();
        dibujo.sprite = spriteBase;
        dibujo.color = color;
        dibujo.sortingOrder = ordenDeDibujo;
        return objeto;
    }
}
