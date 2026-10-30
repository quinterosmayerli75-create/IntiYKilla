// =====================================================================
// ZonaElemental.cs
// Proyecto: Inti y Killa
// Descripción: Charcos de lava, agua o veneno. Es un Trigger (zona
//              invisible al choque) que decide quién muere al tocarlo:
//              - Lava:   mata a quien tenga elemento Agua (Killa)
//              - Agua:   mata a quien tenga elemento Fuego (Inti)
//              - Veneno: mata a los dos
// Uso: agregar a un objeto con BoxCollider2D marcado como "Is Trigger".
// =====================================================================

using UnityEngine;

public enum TipoZona
{
    Lava,
    Agua,
    Veneno
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
    // (cambio de elemento), un jugador parado en el charco también muera.
    private void OnTriggerStay2D(Collider2D otro)
    {
        ControladorJugador jugador = otro.GetComponent<ControladorJugador>();
        if (jugador == null)
        {
            return;
        }

        if (EsPeligrosoPara(jugador.Elemento) == true)
        {
            jugador.Morir("tocó " + tipoZona.ToString().ToLower());
        }
    }

    private bool EsPeligrosoPara(TipoElemento elementoJugador)
    {
        if (tipoZona == TipoZona.Veneno)
        {
            return true;
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
