// =====================================================================
// ObjetoActivable.cs
// Proyecto: Inti y Killa
// Descripción: Clase base (abstracta) para todo lo que se activa con un
//              botón o una palanca: compuertas, elevadores, etc.
//              Funciona como un "enchufe": cada botón presionado le manda
//              una señal encendida, y cuando se suelta, una apagada.
//              Si recibe suficientes señales a la vez, se activa.
//              Ejemplo: una compuerta con senalesNecesarias = 2 solo se
//              abre cuando Inti y Killa pisan sus botones al mismo tiempo.
// =====================================================================

using UnityEngine;

public abstract class ObjetoActivable : MonoBehaviour
{
    [SerializeField] private int senalesNecesarias = 1;

    private int senalesActivas = 0;
    private bool estaActivo = false;

    public bool EstaActivo
    {
        get { return estaActivo; }
    }

    public void ConfigurarSenalesNecesarias(int cantidad)
    {
        senalesNecesarias = cantidad;
    }

    // Lo llaman los botones y palancas conectados a este objeto.
    public void RecibirSenal(bool senalEncendida)
    {
        if (senalEncendida == true)
        {
            senalesActivas = senalesActivas + 1;
        }
        else
        {
            senalesActivas = senalesActivas - 1;
        }

        if (senalesActivas < 0)
        {
            senalesActivas = 0;
        }

        bool deberiaEstarActivo = senalesActivas >= senalesNecesarias;
        if (deberiaEstarActivo != estaActivo)
        {
            estaActivo = deberiaEstarActivo;
            CambiarEstado(estaActivo);
        }
    }

    // Cada objeto hijo decide qué hacer al activarse o desactivarse.
    protected abstract void CambiarEstado(bool activo);
}
