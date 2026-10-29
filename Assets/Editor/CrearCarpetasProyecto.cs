// =====================================================================
// CrearCarpetasProyecto.cs
// Proyecto: Inti y Killa
// Descripción: Script de EDITOR que crea toda la estructura de carpetas
//              del proyecto con un solo clic desde el menú de Unity.
// Ubicación obligatoria: Assets/Editor/CrearCarpetasProyecto.cs
// Uso: Menú superior de Unity -> Inti y Killa -> Crear carpetas del proyecto
// =====================================================================

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CrearCarpetasProyecto
{
    // Carpeta principal donde vivirá todo el contenido propio del juego.
    // El guion bajo "_" hace que aparezca primera en la ventana Project.
    private const string carpetaRaiz = "Assets/_Proyecto";

    // Este atributo agrega una opción nueva al menú superior de Unity.
    [MenuItem("Inti y Killa/Crear carpetas del proyecto")]
    public static void CrearCarpetas()
    {
        // Lista de subcarpetas que queremos crear dentro de la carpeta raíz.
        List<string> listaSubcarpetas = new List<string>();
        listaSubcarpetas.Add("Scripts");
        listaSubcarpetas.Add("Scripts/Jugadores");
        listaSubcarpetas.Add("Scripts/Mecanicas");
        listaSubcarpetas.Add("Scripts/Interfaz");
        listaSubcarpetas.Add("Scripts/Sistema");
        listaSubcarpetas.Add("Scenes");
        listaSubcarpetas.Add("Scenes/Niveles");
        listaSubcarpetas.Add("Sprites");
        listaSubcarpetas.Add("Sprites/Personajes");
        listaSubcarpetas.Add("Sprites/Fondos");
        listaSubcarpetas.Add("Sprites/Interfaz");
        listaSubcarpetas.Add("Tiles");
        listaSubcarpetas.Add("Prefabs");
        listaSubcarpetas.Add("Prefabs/Jugadores");
        listaSubcarpetas.Add("Prefabs/Mecanicas");
        listaSubcarpetas.Add("Audio");
        listaSubcarpetas.Add("Audio/Musica");
        listaSubcarpetas.Add("Audio/Efectos");
        listaSubcarpetas.Add("Animations");
        listaSubcarpetas.Add("UI");
        listaSubcarpetas.Add("Materiales");
        listaSubcarpetas.Add("Input");

        // Primero nos aseguramos de que exista la carpeta raíz.
        CrearCarpetaSiNoExiste("Assets", "_Proyecto");

        int cantidadCreadas = 0;

        // Recorremos cada subcarpeta de la lista.
        foreach (string rutaSubcarpeta in listaSubcarpetas)
        {
            // Una ruta como "Scripts/Jugadores" se separa en partes
            // para crear primero "Scripts" y luego "Jugadores" dentro.
            string[] partesDeLaRuta = rutaSubcarpeta.Split('/');
            string rutaPadre = carpetaRaiz;

            foreach (string nombreCarpeta in partesDeLaRuta)
            {
                bool fueCreada = CrearCarpetaSiNoExiste(rutaPadre, nombreCarpeta);
                if (fueCreada == true)
                {
                    cantidadCreadas = cantidadCreadas + 1;
                }

                // La carpeta actual pasa a ser la carpeta padre de la siguiente.
                rutaPadre = rutaPadre + "/" + nombreCarpeta;
            }
        }

        // Le pedimos a Unity que actualice la ventana Project.
        AssetDatabase.Refresh();

        Debug.Log("Inti y Killa: estructura lista. Carpetas nuevas creadas: " + cantidadCreadas);
    }

    // Crea una carpeta solo si todavía no existe.
    // Devuelve true si la creó y false si ya existía.
    private static bool CrearCarpetaSiNoExiste(string rutaPadre, string nombreCarpeta)
    {
        string rutaCompleta = rutaPadre + "/" + nombreCarpeta;

        if (AssetDatabase.IsValidFolder(rutaCompleta) == true)
        {
            return false;
        }

        AssetDatabase.CreateFolder(rutaPadre, nombreCarpeta);
        return true;
    }
}
