// =====================================================================
// ZonaElemental.cs
// Proyecto: Inti y Killa
// Descripción: Zonas peligrosas. Es un Trigger (zona que se atraviesa)
//              que decide quién muere al tocarla:
//              - Lava:         mata a quien tenga elemento Agua (Killa)
//              - Agua:         mata a quien tenga elemento Fuego (Inti)
//              - Veneno:       mata a los dos
//              - CristalesSal: púas de sal del Salar de Uyuni, matan a los dos
// Uso: agregar a un objeto con un Collider2D marcado como "Is Trigger".
// =====================================================================

using UnityEngine;

public enum TipoZona
{
    Lava,
    Agua,
    Veneno,
    CristalesSal
}

[RequireComponent(typeof(Collider2D))]
public class ZonaElemental : MonoBehaviour
{
    [SerializeField] private TipoZona tipoZona = TipoZona.Lava;

    public void ConfigurarZona(TipoZona nuevoTipo)
    {
        tipoZona = nuevoTipo;
    }

    private void Reset()
    {
        // Al agregar el script, el colisionador se vuelve trigger automáticamente.
        Collider2D colisionador = GetComponent<Collider2D>();
        colisionador.isTrigger = true;
    }

    // Se usa "Stay" y no solo "Enter" para que, cuando exista el eclipse
    // (cambio de elemento), un jugador parado en la zona también muera.
    private void OnTriggerStay2D(Collider2D otro)
    {
        ControladorJugador jugador = otro.GetComponent<ControladorJugador>();
        if (jugador == null)
        {
            return;
        }

        if (EsPeligrosoPara(jugador.Elemento) == true)
        {
            jugador.Morir(MensajesDeMuerte.DeZona(tipoZona, jugador.NombreJugador));
        }
    }

    private bool EsPeligrosoPara(TipoElemento elementoJugador)
    {
        if (tipoZona == TipoZona.Veneno)
        {
            return true;
        }

        if (tipoZona == TipoZona.CristalesSal)
        {
            return true;
        }

        // Durante el cambio de poderes del eclipse automático, la lava y el agua
        // dan un instante de gracia (ver CicloEclipse).
        if (CicloEclipse.EstaEnGracia == true)
        {
            return false;
        }

        if (tipoZona == TipoZona.Lava && elementoJugador == TipoElemento.Agua)
        {
            return true;
        }

        if (tipoZona == TipoZona.Agua && elementoJugador == TipoElemento.Fuego)
        {
            return true;
        }

        return false;
    }
}
