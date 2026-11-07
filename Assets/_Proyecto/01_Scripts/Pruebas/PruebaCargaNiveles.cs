// =====================================================================
// PruebaCargaNiveles.cs
// Proyecto: Inti y Killa
// Descripción: Prueba automática en Play Mode. Abre cada nivel terminado y
//              lo deja correr unos segundos sin tocar ninguna tecla. Sirve
//              para detectar a tiempo escenas rotas, errores en consola y
//              peligros que matan a los jugadores aunque estén quietos.
//              NO demuestra que un nivel se pueda ganar: eso hay que jugarlo.
// Uso: Window > General > Test Runner > PlayMode, o Unity en modo batch.
// =====================================================================

using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class PruebaCargaNiveles
{
    // Dura más que un ciclo completo de lluvia (10.5 s) para que también se
    // pruebe el momento en que la lluvia empieza a hacer daño.
    private const float segundosPorNivel = 12f;
    private const int jugadoresEsperados = 2;
    private const int puertasEsperadas = 2;

    private int vecesCargada = 0;

    // Los niveles que ya existen. Al terminar un mundo nuevo se agregan aquí.
    private static IEnumerable<string> NivelesTerminados()
    {
        for (int numeroMundo = 1; numeroMundo <= 3; numeroMundo++)
        {
            for (int numeroNivel = 1; numeroNivel <= 5; numeroNivel++)
            {
                yield return "Mundo" + numeroMundo + "_Nivel" + numeroNivel;
            }
        }
    }

    [UnitySetUp]
    public IEnumerator Preparar()
    {
        vecesCargada = 0;
        SceneManager.sceneLoaded += ContarCarga;
        yield return null;
    }

    [UnityTearDown]
    public IEnumerator Limpiar()
    {
        SceneManager.sceneLoaded -= ContarCarga;
        yield return null;
    }

    // Cada carga después de la primera significa que alguien murió y el
    // nivel se reinició, porque el reinicio vuelve a cargar la escena.
    private void ContarCarga(Scene escena, LoadSceneMode modo)
    {
        vecesCargada = vecesCargada + 1;
    }

    // Se buscan por nombre de clase porque esta prueba vive en su propio
    // ensamblado y no puede usar directamente los tipos del juego.
    private int ContarComponentes(string nombreClase)
    {
        MonoBehaviour[] componentes = Object.FindObjectsByType<MonoBehaviour>();
        int cantidad = 0;

        foreach (MonoBehaviour componente in componentes)
        {
            if (componente.GetType().Name == nombreClase)
            {
                cantidad = cantidad + 1;
            }
        }

        return cantidad;
    }

    [UnityTest]
    public IEnumerator ElNivelCargaYCorreSinErrores([ValueSource(nameof(NivelesTerminados))] string nombreEscena)
    {
        SceneManager.LoadScene(nombreEscena);
        yield return null;
        yield return null;

        Assert.AreEqual(nombreEscena, SceneManager.GetActiveScene().name, "No se abrió la escena pedida.");
        Assert.AreEqual(jugadoresEsperados, ContarComponentes("ControladorJugador"), "Cantidad de jugadores incorrecta.");
        Assert.AreEqual(puertasEsperadas, ContarComponentes("PuertaSalida"), "Cantidad de puertas incorrecta.");
        Assert.AreEqual(1, ContarComponentes("GestorNivel"), "Debe haber exactamente un GestorNivel.");

        // Se usa tiempo real para que el resultado no dependa de los cuadros
        // por segundo de la máquina donde corre la prueba.
        float tiempoFinal = Time.realtimeSinceStartup + segundosPorNivel;
        while (Time.realtimeSinceStartup < tiempoFinal)
        {
            yield return null;
        }

        // Las muertes no fallan la prueba: algunos peligros pueden alcanzar a
        // un jugador quieto a propósito. Se informan para revisarlas a mano.
        int muertesEstandoQuietos = vecesCargada - 1;
        Debug.Log("[PRUEBA] " + nombreEscena + ": muertes estando quietos = " + muertesEstandoQuietos);
    }
}
