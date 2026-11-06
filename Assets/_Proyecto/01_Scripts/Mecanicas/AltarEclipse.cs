// =====================================================================
// AltarEclipse.cs
// Proyecto: Inti y Killa
// Descripción: El "plus" del juego. Un altar tiene dos placas (pueden
//              estar lejos una de la otra). Cuando Inti y Killa están
//              parados cada uno en una placa AL MISMO TIEMPO, ocurre un
//              eclipse: intercambian sus elementos (el fuego pasa a ser
//              agua y el agua, fuego).
//              Para volver a usarlo, alguno debe bajarse de su placa.
// =====================================================================

using UnityEngine;

public class AltarEclipse : MonoBehaviour
{
    [SerializeField] private PlacaAltar placaUno;
    [SerializeField] private PlacaAltar placaDos;

    [Header("Dibujos del efecto de eclipse")]
    [SerializeField] private Sprite spriteCirculo;
    [SerializeField] private Sprite spriteResplandor;
    // Opcional: la imagen de la luna. Sin ella se usa un círculo.
    [SerializeField] private Sprite imagenLuna;

    // Evita que el eclipse se repita en cada frame mientras siguen parados.
    private bool estaListo = true;

    public PlacaAltar PlacaUno
    {
        get { return placaUno; }
    }

    public PlacaAltar PlacaDos
    {
        get { return placaDos; }
    }

    public void Configurar(PlacaAltar nuevaPlacaUno, PlacaAltar nuevaPlacaDos)
    {
        placaUno = nuevaPlacaUno;
        placaDos = nuevaPlacaDos;
    }

    public void ConfigurarEfecto(Sprite nuevoCirculo, Sprite nuevoResplandor)
    {
        spriteCirculo = nuevoCirculo;
        spriteResplandor = nuevoResplandor;
    }

    private void Update()
    {
        if (placaUno == null || placaDos == null)
        {
            return;
        }

        ControladorJugador jugadorUno = placaUno.Ocupante;
        ControladorJugador jugadorDos = placaDos.Ocupante;

        bool alguienSeBajo = jugadorUno == null || jugadorDos == null;
        if (alguienSeBajo == true)
        {
            estaListo = true;
            return;
        }

        bool sonPersonajesDistintos = jugadorUno != jugadorDos;
        if (sonPersonajesDistintos == true && estaListo == true)
        {
            ProvocarEclipse(jugadorUno, jugadorDos);
            estaListo = false;
        }
    }

    private void ProvocarEclipse(ControladorJugador jugadorUno, ControladorJugador jugadorDos)
    {
        TipoElemento elementoDelUno = jugadorUno.Elemento;
        TipoElemento elementoDelDos = jugadorDos.Elemento;

        jugadorUno.CambiarElemento(elementoDelDos);
        jugadorDos.CambiarElemento(elementoDelUno);

        EfectoEclipse.Reproducir(spriteCirculo, spriteResplandor, imagenLuna);

        if (GestorNivel.Instancia != null)
        {
            GestorNivel.Instancia.MostrarAviso("¡ECLIPSE! " + jugadorUno.NombreJugador + " y " + jugadorDos.NombreJugador + " intercambiaron poderes", 2.5f);
        }
    }
}
