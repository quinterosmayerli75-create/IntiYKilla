// =====================================================================
// CicloEclipse.cs
// Proyecto: Inti y Killa
// Descripción: Eclipse automático de la Isla del Sol: cada cierto tiempo los
//              dos personajes intercambian poderes, estén donde estén. Antes
//              aparece una cuenta regresiva (regla de justicia): hay tiempo para
//              salir de un charco que con el poder nuevo sería mortal.
// Uso: un solo objeto en la escena, creado por el generador.
// =====================================================================

using UnityEngine;

public class CicloEclipse : MonoBehaviour
{
    [SerializeField] private float segundosEntreEclipses = 9f;
    [SerializeField] private float segundosDeCuentaRegresiva = 3f;

    [Header("Dibujos del efecto")]
    [SerializeField] private Sprite spriteCirculo;
    [SerializeField] private Sprite spriteResplandor;

    // Instante alrededor del cambio en que la lava y el agua no dañan: así se
    // puede saltar a un charco del poder nuevo cuando la cuenta llega a 1.
    [SerializeField] private float segundosDeGracia = 0.7f;

    private static CicloEclipse instancia;

    private ControladorJugador[] jugadores;
    private float tiempoDesdeElCambio = 100f;

    public static bool EstaEnGracia
    {
        get
        {
            if (instancia == null)
            {
                return false;
            }
            return instancia.SegundosParaElEclipse <= instancia.segundosDeGracia || instancia.tiempoDesdeElCambio <= instancia.segundosDeGracia;
        }
    }
    private float tiempo = 0f;
    private int ultimoNumeroMostrado = -1;

    public float SegundosParaElEclipse
    {
        get { return segundosEntreEclipses - tiempo; }
    }

    public void Configurar(float nuevosSegundos, Sprite nuevoCirculo, Sprite nuevoResplandor)
    {
        segundosEntreEclipses = nuevosSegundos;
        spriteCirculo = nuevoCirculo;
        spriteResplandor = nuevoResplandor;
    }

    private void Awake()
    {
        instancia = this;
        jugadores = FindObjectsByType<ControladorJugador>();
    }

    private void OnDestroy()
    {
        if (instancia == this)
        {
            instancia = null;
        }
    }

    private void Update()
    {
        tiempo = tiempo + Time.deltaTime;
        tiempoDesdeElCambio = tiempoDesdeElCambio + Time.deltaTime;
        MostrarCuentaRegresiva();

        if (tiempo >= segundosEntreEclipses)
        {
            tiempo = 0f;
            tiempoDesdeElCambio = 0f;
            ultimoNumeroMostrado = -1;
            IntercambiarPoderes();
        }
    }

    private void MostrarCuentaRegresiva()
    {
        float restante = SegundosParaElEclipse;
        if (restante > segundosDeCuentaRegresiva || GestorNivel.Instancia == null)
        {
            return;
        }

        int numero = Mathf.CeilToInt(restante);
        if (numero != ultimoNumeroMostrado)
        {
            ultimoNumeroMostrado = numero;
            GestorNivel.Instancia.MostrarAviso("Eclipse en " + numero + "...", 1f);
        }
    }

    private void IntercambiarPoderes()
    {
        if (jugadores.Length < 2)
        {
            return;
        }

        TipoElemento elementoDelUno = jugadores[0].Elemento;
        TipoElemento elementoDelDos = jugadores[1].Elemento;
        jugadores[0].CambiarElemento(elementoDelDos);
        jugadores[1].CambiarElemento(elementoDelUno);

        EfectoEclipse.Reproducir(spriteCirculo, spriteResplandor, null);
        if (GestorNivel.Instancia != null)
        {
            GestorNivel.Instancia.MostrarAviso("¡ECLIPSE! Cambiaron los poderes.", 1.5f);
        }
    }
}
