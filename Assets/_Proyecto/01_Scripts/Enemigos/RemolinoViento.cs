// =====================================================================
// RemolinoViento.cs
// Proyecto: Inti y Killa
// Descripción: Zona de viento que empuja a los jugadores hacia un lado sin
//              matarlos. El empuje es menor que la velocidad al caminar, así
//              que siempre se puede avanzar contra el viento.
//              Siempre está a la vista (fondo y rayas que se mueven hacia
//              donde empuja): ese es su aviso, según la regla de justicia.
// Uso: objeto con SpriteRenderer de fondo y rayas hijas. Mientras no haya
//      arte se ve como formas de color; basta con poner una imagen en
//      "imagenPropia" para reemplazar el fondo.
// =====================================================================

using UnityEngine;

public class RemolinoViento : MonoBehaviour
{
    [Header("Zona y fuerza")]
    [SerializeField] private Vector2 tamanoZona = new Vector2(4f, 4f);
    // Positivo empuja a la derecha, negativo a la izquierda (el jugador camina a 6).
    [SerializeField] private float velocidadEmpuje = 3f;

    [Header("Dibujo")]
    // Si tiene imagen, se usa en lugar del rectángulo de color.
    [SerializeField] private Sprite imagenPropia;
    [SerializeField] private SpriteRenderer dibujoFondo;
    [SerializeField] private Transform[] rayas;
    [SerializeField] private float velocidadRayas = 4f;

    private ControladorJugador[] jugadores;

    public void Configurar(Vector2 nuevoTamano, float nuevaVelocidad, SpriteRenderer nuevoFondo, Transform[] nuevasRayas)
    {
        tamanoZona = nuevoTamano;
        velocidadEmpuje = nuevaVelocidad;
        dibujoFondo = nuevoFondo;
        rayas = nuevasRayas;
    }

    private void Awake()
    {
        jugadores = FindObjectsByType<ControladorJugador>();

        if (imagenPropia != null && dibujoFondo != null)
        {
            dibujoFondo.sprite = imagenPropia;
            dibujoFondo.color = Color.white;
        }
    }

    // Se usa FixedUpdate porque el empuje se suma a la física del jugador.
    private void FixedUpdate()
    {
        foreach (ControladorJugador jugador in jugadores)
        {
            if (jugador != null && jugador.EstaVivo == true && EstaDentro(jugador) == true)
            {
                jugador.AplicarEmpuje(velocidadEmpuje);
            }
        }
    }

    private bool EstaDentro(ControladorJugador jugador)
    {
        Bounds cuerpo = jugador.LimitesCuerpo;
        Vector2 centro = transform.position;
        bool cruzaEnX = cuerpo.max.x > centro.x - tamanoZona.x / 2f && cuerpo.min.x < centro.x + tamanoZona.x / 2f;
        bool cruzaEnY = cuerpo.max.y > centro.y - tamanoZona.y / 2f && cuerpo.min.y < centro.y + tamanoZona.y / 2f;
        return cruzaEnX == true && cruzaEnY == true;
    }

    // Las rayas avanzan en la dirección del viento y vuelven a entrar por el otro lado.
    private void Update()
    {
        if (rayas == null)
        {
            return;
        }

        float direccion = Mathf.Sign(velocidadEmpuje);
        float limite = tamanoZona.x / 2f;

        foreach (Transform raya in rayas)
        {
            Vector3 posicion = raya.localPosition;
            posicion.x = posicion.x + direccion * velocidadRayas * Time.deltaTime;

            if (posicion.x > limite)
            {
                posicion.x = -limite;
            }
            else if (posicion.x < -limite)
            {
                posicion.x = limite;
            }

            raya.localPosition = posicion;
        }
    }
}
