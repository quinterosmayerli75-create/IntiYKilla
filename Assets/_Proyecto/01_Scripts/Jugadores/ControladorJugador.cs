// =====================================================================
// ControladorJugador.cs
// Proyecto: Inti y Killa
// Descripción: Mueve a un personaje (Inti o Killa) con el teclado usando
//              el Input System nuevo. El MISMO script sirve para los dos
//              jugadores: solo cambian las teclas y el elemento en el
//              Inspector.
// Mecánicas: caminar, saltar, coyote time, buffer de salto, morir.
// =====================================================================

using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class ControladorJugador : MonoBehaviour
{
    [Header("Identidad del personaje")]
    [SerializeField] private string nombreJugador = "Inti";
    [SerializeField] private TipoElemento elemento = TipoElemento.Fuego;

    [Header("Teclas (Jugador 1 = WASD, Jugador 2 = flechas)")]
    [SerializeField] private Key teclaIzquierda = Key.A;
    [SerializeField] private Key teclaDerecha = Key.D;
    [SerializeField] private Key teclaSaltar = Key.W;

    [Header("Movimiento")]
    [SerializeField] private float velocidadMovimiento = 6f;
    [SerializeField] private float fuerzaSalto = 15f;

    [Header("Ayudas de salto (hacen que el control se sienta justo)")]
    // Coyote time: un instante extra para saltar después de salir del borde,
    // como el coyote de los dibujos que corre en el aire antes de caer.
    [SerializeField] private float tiempoCoyote = 0.1f;
    // Buffer: si presionas saltar un poquito antes de tocar el suelo, igual salta.
    [SerializeField] private float tiempoBufferSalto = 0.1f;

    [Header("Caída al vacío")]
    [SerializeField] private float alturaLimiteCaida = -12f;

    // Referencias a los componentes del mismo objeto.
    private Rigidbody2D cuerpoFisico;
    private BoxCollider2D colisionador;
    private SpriteRenderer dibujo;

    // Estado interno del personaje.
    private float direccionHorizontal = 0f;
    private float contadorCoyote = 0f;
    private float contadorBufferSalto = 0f;
    private bool estaVivo = true;
    private bool puedeMoverse = true;

    // Propiedades de solo lectura para que otros scripts consulten datos.
    public TipoElemento Elemento
    {
        get { return elemento; }
    }

    public string NombreJugador
    {
        get { return nombreJugador; }
    }

    public bool EstaVivo
    {
        get { return estaVivo; }
    }

    private void Awake()
    {
        cuerpoFisico = GetComponent<Rigidbody2D>();
        colisionador = GetComponent<BoxCollider2D>();
        dibujo = GetComponent<SpriteRenderer>();

        // El personaje no debe girar como una pelota al chocar.
        cuerpoFisico.freezeRotation = true;
    }

    // Permite configurar al personaje desde el script que arma el nivel.
    public void ConfigurarJugador(string nuevoNombre, TipoElemento nuevoElemento, Key nuevaIzquierda, Key nuevaDerecha, Key nuevoSalto)
    {
        nombreJugador = nuevoNombre;
        elemento = nuevoElemento;
        teclaIzquierda = nuevaIzquierda;
        teclaDerecha = nuevaDerecha;
        teclaSaltar = nuevoSalto;
    }

    private void Update()
    {
        if (estaVivo == false || puedeMoverse == false)
        {
            direccionHorizontal = 0f;
            return;
        }

        LeerTeclado();
        ActualizarContadoresDeSalto();
        IntentarSaltar();
        RevisarCaidaAlVacio();
    }

    private void FixedUpdate()
    {
        // La física se aplica en FixedUpdate para que el movimiento sea estable.
        Vector2 velocidadActual = cuerpoFisico.linearVelocity;
        Vector2 velocidadNueva = new Vector2(direccionHorizontal * velocidadMovimiento, velocidadActual.y);
        cuerpoFisico.linearVelocity = velocidadNueva;
    }

    private void LeerTeclado()
    {
        Keyboard teclado = Keyboard.current;
        if (teclado == null)
        {
            return;
        }

        direccionHorizontal = 0f;

        if (teclado[teclaIzquierda].isPressed == true)
        {
            direccionHorizontal = direccionHorizontal - 1f;
        }

        if (teclado[teclaDerecha].isPressed == true)
        {
            direccionHorizontal = direccionHorizontal + 1f;
        }

        // Voltea el dibujo para que mire hacia donde camina.
        if (dibujo != null)
        {
            if (direccionHorizontal < 0f)
            {
                dibujo.flipX = true;
            }
            else if (direccionHorizontal > 0f)
            {
                dibujo.flipX = false;
            }
        }

        if (teclado[teclaSaltar].wasPressedThisFrame == true)
        {
            contadorBufferSalto = tiempoBufferSalto;
        }
    }

    private void ActualizarContadoresDeSalto()
    {
        if (EstaEnElSuelo() == true)
        {
            contadorCoyote = tiempoCoyote;
        }
        else
        {
            contadorCoyote = contadorCoyote - Time.deltaTime;
        }

        contadorBufferSalto = contadorBufferSalto - Time.deltaTime;
    }

    private void IntentarSaltar()
    {
        bool quiereSaltar = contadorBufferSalto > 0f;
        bool puedeSaltar = contadorCoyote > 0f;

        if (quiereSaltar == true && puedeSaltar == true)
        {
            Vector2 velocidadActual = cuerpoFisico.linearVelocity;
            cuerpoFisico.linearVelocity = new Vector2(velocidadActual.x, fuerzaSalto);

            // Se gastan las ayudas para no saltar dos veces seguidas.
            contadorBufferSalto = 0f;
            contadorCoyote = 0f;
        }
    }

    // Revisa si hay algo sólido justo debajo de los pies.
    private bool EstaEnElSuelo()
    {
        Bounds limites = colisionador.bounds;
        Vector2 centroCaja = new Vector2(limites.center.x, limites.min.y - 0.05f);
        Vector2 tamanoCaja = new Vector2(limites.size.x * 0.9f, 0.1f);

        Collider2D[] objetosDebajo = Physics2D.OverlapBoxAll(centroCaja, tamanoCaja, 0f);

        foreach (Collider2D objeto in objetosDebajo)
        {
            bool esElMismoJugador = objeto == colisionador;
            bool esZonaInvisible = objeto.isTrigger;

            if (esElMismoJugador == false && esZonaInvisible == false)
            {
                return true;
            }
        }

        return false;
    }

    private void RevisarCaidaAlVacio()
    {
        if (transform.position.y < alturaLimiteCaida)
        {
            Morir("cayó al vacío");
        }
    }

    // Lo llaman las zonas peligrosas (lava, agua, veneno) o la caída.
    public void Morir(string motivo)
    {
        if (estaVivo == false)
        {
            return;
        }

        estaVivo = false;
        direccionHorizontal = 0f;
        cuerpoFisico.linearVelocity = Vector2.zero;

        // Se oscurece para que se note que murió.
        if (dibujo != null)
        {
            dibujo.color = new Color(0.2f, 0.2f, 0.2f, 0.6f);
        }

        if (GestorNivel.Instancia != null)
        {
            GestorNivel.Instancia.JugadorMurio(this, motivo);
        }
    }

    // Lo usa el GestorNivel al ganar, para que los personajes se queden quietos.
    public void DetenerMovimiento()
    {
        puedeMoverse = false;
        direccionHorizontal = 0f;
    }

    // Dibuja la caja de "pies" en la vista Scene para poder revisarla.
    private void OnDrawGizmosSelected()
    {
        BoxCollider2D colisionadorDeDibujo = GetComponent<BoxCollider2D>();
        if (colisionadorDeDibujo == null)
        {
            return;
        }

        Bounds limites = colisionadorDeDibujo.bounds;
        Vector3 centroCaja = new Vector3(limites.center.x, limites.min.y - 0.05f, 0f);
        Vector3 tamanoCaja = new Vector3(limites.size.x * 0.9f, 0.1f, 0f);

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(centroCaja, tamanoCaja);
    }
}
