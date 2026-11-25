// =====================================================================
// BotPruebas.cs
// Proyecto: Inti y Killa
// Descripción: Bot de pruebas automáticas. Juega los 30 niveles apretando
//              teclas simuladas (como un jugador de verdad: nunca se
//              teletransporta durante un recorrido) y comprueba que cada
//              nivel se gana. También prueba las trampas una por una
//              (ahí sí puede mover a los personajes, para aislar cada pieza)
//              y el sistema: pausa, pista, eclipse, melodía, Intro y Final.
//              Solo existe en el editor: nunca entra en la versión publicada.
// Uso: menú "Inti y Killa > Pruebas" o Herramientas/correr_bot.ps1.
//      Variables: PRUEBA_BOT=1 lo enciende; SOLO_NIVEL elige qué probar
//      ("" sistema y Mundos 5-6, "todos" Mundos 1-4, "desafio" piezas,
//      o nombres de escenas separados por coma).
// =====================================================================

#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.SceneManagement;

public class BotPruebas : MonoBehaviour
{
    private class Personaje
    {
        public ControladorJugador control;
        public Rigidbody2D cuerpo;
        public Key izquierda;
        public Key derecha;
        public Key salto;
        public Key habilidad;
    }

    private Keyboard teclado;
    private HashSet<Key> teclas = new HashSet<Key>();
    private Personaje inti;
    private Personaje killa;
    private string falloRuta = "";
    private bool rutaTerminada = false;
    private int fallos = 0;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Iniciar()
    {
        if (System.Environment.GetEnvironmentVariable("PRUEBA_BOT") != "1")
        {
            return;
        }
        GameObject objeto = new GameObject("BotPruebas");
        DontDestroyOnLoad(objeto);
        objeto.AddComponent<BotPruebas>();
    }

    private EjecutorDeRutasBot ejecutor;

    // Todo lo que las rutas ponen en marcha corre en el ejecutor.
    private new Coroutine StartCoroutine(IEnumerator rutina)
    {
        return ejecutor.StartCoroutine(rutina);
    }

    private new void StopCoroutine(Coroutine rutina)
    {
        if (rutina != null)
        {
            ejecutor.StopCoroutine(rutina);
        }
    }

    private void Start()
    {
        Application.runInBackground = true;
        InputSystem.settings.backgroundBehavior = InputSettings.BackgroundBehavior.IgnoreFocus;
        InputSystem.settings.editorInputBehaviorInPlayMode = InputSettings.EditorInputBehaviorInPlayMode.AllDeviceInputAlwaysGoesToGameView;
        teclado = InputSystem.AddDevice<Keyboard>();
        teclado.MakeCurrent();
        // Las rutas corren en otro componente: así se pueden cortar todas juntas
        // (al repetirse la sala o al terminar un nivel) sin cortar la prueba entera.
        ejecutor = gameObject.AddComponent<EjecutorDeRutasBot>();
        base.StartCoroutine(TodoMundo56());
    }

    // ===== Prueba de reinicio =====
    // En cada uno de los 30 niveles: se anota el estado de todas las trampas,
    // gemas y puertas (dónde están, si están activas y si ya se dispararon),
    // muere Inti y, cuando el nivel se reinicia solo, se comprueba que todo
    // volvió a quedar igual, que se cuenta la muerte y que las gemas vuelven a 0.
    private IEnumerator ProbarReinicios()
    {
        for (int numeroMundo = 1; numeroMundo <= 6; numeroMundo++)
        {
            for (int numeroNivel = 1; numeroNivel <= 5; numeroNivel++)
            {
                string escena = "Mundo" + numeroMundo + "_Nivel" + numeroNivel;
                yield return Cargar(escena);
                yield return new WaitForSeconds(0.3f);

                string estadoAntes = FotoDelEscenario();
                int muertesAntes = (int)DatoEstatico("muertesDelIntento");
                GestorNivel gestorAnterior = GestorNivel.Instancia;

                // Se disparan todas las trampas antes de morir: así se comprueba
                // que ninguna queda disparada después del reinicio.
                int trampasDisparadas = 0;
                foreach (TrampaActivable trampa in Object.FindObjectsByType<TrampaActivable>())
                {
                    trampa.Activar(inti.control);
                    trampasDisparadas = trampasDisparadas + 1;
                }
                yield return new WaitForSeconds(0.6f);
                Log(escena + ": trampas disparadas antes de morir = " + trampasDisparadas);

                if (inti.control != null && inti.control.EstaVivo == true)
                {
                    inti.control.Morir("Prueba de reinicio");
                }

                // Se espera a que exista un GestorNivel nuevo: eso es el reinicio.
                float limite = Time.time + 5f;
                while (Time.time < limite && (GestorNivel.Instancia == null || GestorNivel.Instancia == gestorAnterior))
                {
                    yield return null;
                }
                bool seReinicio = GestorNivel.Instancia != null && GestorNivel.Instancia != gestorAnterior;
                Verificar(escena + ": el nivel se reinicia al morir", seReinicio);
                if (seReinicio == false)
                {
                    continue;
                }

                yield return null;
                yield return null;

                inti = Crear("Inti", Key.A, Key.D, Key.W, Key.S);
                killa = Crear("Killa", Key.LeftArrow, Key.RightArrow, Key.UpArrow, Key.DownArrow);
                yield return new WaitForSeconds(0.3f);

                string estadoDespues = FotoDelEscenario();
                int muertesDespues = (int)DatoEstatico("muertesDelIntento");
                int gemasRecogidas = (int)Dato("gemasFuegoRecogidas") + (int)Dato("gemasAguaRecogidas");

                Verificar(escena + ": trampas, gemas y puertas vuelven a su estado inicial", estadoAntes == estadoDespues);
                if (estadoAntes != estadoDespues)
                {
                    Log("Antes:   " + estadoAntes);
                    Log("Después: " + estadoDespues);
                }
                Verificar(escena + ": la muerte se cuenta (" + muertesAntes + " -> " + muertesDespues + ")", muertesDespues == muertesAntes + 1);
                Verificar(escena + ": las gemas recogidas vuelven a 0", gemasRecogidas == 0);
                Verificar(escena + ": los dos personajes están vivos", inti.control.EstaVivo == true && killa.control.EstaVivo == true);
            }
        }
    }

    // Texto con el estado de cada trampa, gema y puerta, ordenado para poder
    // comparar dos momentos. Las gemas flotan, por eso de ellas solo se anota la X.
    private string FotoDelEscenario()
    {
        List<string> partes = new List<string>();
        FieldInfo campoActivada = typeof(TrampaActivable).GetField("yaActivada", BindingFlags.Instance | BindingFlags.NonPublic);

        foreach (TrampaBase trampa in Object.FindObjectsByType<TrampaBase>())
        {
            string activada = "";
            TrampaActivable activable = trampa as TrampaActivable;
            if (activable != null && campoActivada != null)
            {
                activada = " disparada=" + campoActivada.GetValue(activable);
            }

            bool esGema = trampa.GetComponent<Gema>() != null || trampa is GemaTrampa;
            partes.Add(trampa.GetType().Name + ":" + trampa.name + " " + DescribirPosicion(trampa.transform.position, esGema) + " activa=" + trampa.gameObject.activeInHierarchy + activada);
        }

        foreach (Gema gema in Object.FindObjectsByType<Gema>())
        {
            partes.Add("Gema:" + gema.name + " " + DescribirPosicion(gema.transform.position, true));
        }

        foreach (PuertaSalida puerta in Object.FindObjectsByType<PuertaSalida>())
        {
            partes.Add("Puerta:" + puerta.name + " " + DescribirPosicion(puerta.transform.position, false));
        }

        partes.Sort();
        return string.Join(" | ", partes.ToArray());
    }

    private string DescribirPosicion(Vector3 posicion, bool soloX)
    {
        if (soloX == true)
        {
            return "x=" + posicion.x.ToString("F2");
        }
        return "x=" + posicion.x.ToString("F2") + " y=" + posicion.y.ToString("F2");
    }

    // En modo batch (pruebas desde la consola) se cierra Unity al terminar.
    // Abierto desde el menú, solo se detiene el Play: nunca cierra el editor.
    private void TerminarPruebas()
    {
        if (Application.isBatchMode == true)
        {
            UnityEditor.EditorApplication.Exit(0);
            return;
        }

        UnityEditor.EditorApplication.isPlaying = false;
    }

    private void Update()
    {
        Key[] lista = new Key[teclas.Count];
        teclas.CopyTo(lista);
        InputSystem.QueueStateEvent(teclado, new KeyboardState(lista));
    }

    private IEnumerator Todo()
    {
        GestorProgreso.BorrarProgreso();
        yield return ProbarTrampas();
        yield return ProbarSistemaDeMuertes();
        Log("FALLOS EN TRAMPAS Y MUERTES: " + fallos);
        yield return Nivel("Mundo1_Nivel1", RutaN1());
        yield return Nivel("Mundo1_Nivel2", RutaN2());
        yield return Nivel("Mundo1_Nivel3", RutaN3());
        yield return Nivel("Mundo1_Nivel4", RutaN4());
        yield return Nivel("Mundo1_Nivel5", RutaN5());
        yield return Nivel("Mundo2_Nivel1", RutaM2N1());
        GestorProgreso.BorrarProgreso();
        Log("FIN DE LAS PRUEBAS");
        yield return null;
#if UNITY_EDITOR
        TerminarPruebas();
#endif
    }

    // ---------- Infraestructura ----------

    private object Dato(string nombre)
    {
        FieldInfo campo = typeof(GestorNivel).GetField(nombre, BindingFlags.Instance | BindingFlags.NonPublic);
        return campo.GetValue(GestorNivel.Instancia);
    }

    private object DatoEstatico(string nombre)
    {
        FieldInfo campo = typeof(GestorNivel).GetField(nombre, BindingFlags.Static | BindingFlags.NonPublic);
        return campo.GetValue(null);
    }

    private void Log(string texto)
    {
        Debug.Log("BOT " + texto);
    }

    private void Verificar(string nombre, bool correcto)
    {
        if (correcto == false)
        {
            fallos = fallos + 1;
        }
        Log((correcto ? "OK    " : "FALLO ") + nombre);
    }

    private Personaje Crear(string nombre, Key izq, Key der, Key sal, Key hab)
    {
        Personaje p = new Personaje();
        GameObject objeto = GameObject.Find(nombre);
        p.control = objeto.GetComponent<ControladorJugador>();
        p.cuerpo = objeto.GetComponent<Rigidbody2D>();
        p.izquierda = izq;
        p.derecha = der;
        p.salto = sal;
        p.habilidad = hab;
        return p;
    }

    private IEnumerator Cargar(string escena)
    {
        teclas.Clear();
        SceneManager.LoadScene(escena);
        yield return null;
        yield return null;
        inti = Crear("Inti", Key.A, Key.D, Key.W, Key.S);
        killa = Crear("Killa", Key.LeftArrow, Key.RightArrow, Key.UpArrow, Key.DownArrow);
    }

    private IEnumerator Nivel(string escena, IEnumerator ruta)
    {
        Log("===== " + escena + " =====");
        falloRuta = "";
        rutaTerminada = false;
        yield return Cargar(escena);
        yield return new WaitForSeconds(0.5f);

        bool buscoPuertas = false;
        GestorNivel gestorActual = GestorNivel.Instancia;
        Coroutine rutaActiva = StartCoroutine(Envolver(ruta));
        float limite = Time.time + 150f;
        string resultado = "TIEMPO AGOTADO";

        while (Time.time < limite)
        {
            // Sala que se repite: la escena se recargó sin que nadie muriera.
            if (GestorNivel.Instancia == null)
            {
                yield return null;
                continue;
            }
            if (GestorNivel.Instancia != gestorActual)
            {
                yield return null;
                yield return null;
                ejecutor.StopAllCoroutines();
                teclas.Clear();
                inti = Crear("Inti", Key.A, Key.D, Key.W, Key.S);
                killa = Crear("Killa", Key.LeftArrow, Key.RightArrow, Key.UpArrow, Key.DownArrow);
                gestorActual = GestorNivel.Instancia;
                rutaTerminada = false;
                buscoPuertas = false;
                limite = limite + 150f;
                Log(escena + " vuelta " + (SalaQueSeRepite.VueltaActual + 1));
                yield return new WaitForSeconds(0.5f);
                rutaActiva = StartCoroutine(Envolver(RutaPorNombre(escena)));
                continue;
            }
            if ((bool)Dato("nivelTerminado"))
            {
                resultado = "GANADO estrellas=" + Dato("estrellasGanadas") + " muertes=" + DatoEstatico("muertesDelIntento") + " tiempo=" + ((float)Dato("cronometro")).ToString("0.0") + " gemasFuego=" + Dato("gemasFuegoRecogidas") + "/" + Dato("totalGemasFuego") + " gemasAgua=" + Dato("gemasAguaRecogidas") + "/" + Dato("totalGemasAgua");
                resultado = resultado + " perfecto=" + Dato("nivelPerfecto") + " logros=" + GestorLogros.CantidadDesbloqueados();
                break;
            }
            if ((bool)Dato("reinicioEnCurso"))
            {
                resultado = "MUERTE: " + Dato("mensajeCentral") + " Inti=" + inti.cuerpo.position.ToString("F2") + " Killa=" + killa.cuerpo.position.ToString("F2");
                break;
            }
            if (falloRuta != "")
            {
                resultado = "RUTA FALLO: " + falloRuta + " Inti=" + inti.cuerpo.position.ToString("F2") + " Killa=" + killa.cuerpo.position.ToString("F2");
                break;
            }
            if (rutaTerminada == true && buscoPuertas == false)
            {
                // Las puertas pudieron cambiarse de lugar: cada uno va a la suya.
                buscoPuertas = true;
                yield return new WaitForSeconds(0.5f);
                if ((bool)Dato("nivelTerminado") == false && (bool)Dato("reinicioEnCurso") == false)
                {
                    rutaActiva = StartCoroutine(IrALasPuertasActuales());
                    float limitePuertas = Time.time + 12f;
                    while (Time.time < limitePuertas && GestorNivel.Instancia == gestorActual && (bool)Dato("nivelTerminado") == false && (bool)Dato("reinicioEnCurso") == false) { yield return null; }
                }
                continue;
            }
            if (rutaTerminada == true)
            {
                yield return new WaitForSeconds(2.5f);
                // Si la sala se repitió, la escena es otra: el bucle de arriba lo atiende.
                if (GestorNivel.Instancia == null || GestorNivel.Instancia != gestorActual) { continue; }
                if ((bool)Dato("nivelTerminado") == false)
                {
                    resultado = "RUTA COMPLETA PERO SIN VICTORIA Inti=" + inti.cuerpo.position.ToString("F2") + " Killa=" + killa.cuerpo.position.ToString("F2");
                    break;
                }
                continue;
            }
            yield return null;
        }

        // Corta la ruta y cualquier paso suelto que siga apretando teclas.
        ejecutor.StopAllCoroutines();
        teclas.Clear();
        Log(escena + " RESULTADO: " + resultado);
        yield return new WaitForSeconds(0.8f);
    }

    private IEnumerator Envolver(IEnumerator ruta)
    {
        yield return ruta;
        rutaTerminada = true;
    }

    private IEnumerator Caminar(Personaje p, float x)
    {
        float limite = Time.time + 12f;
        while (Mathf.Abs(p.cuerpo.position.x - x) > 0.12f)
        {
            Soltar(p);
            teclas.Add(TeclaHacia(p, x));
            if (Time.time > limite)
            {
                falloRuta = p.control.NombreJugador + " no pudo caminar hasta x=" + x;
                yield break;
            }
            yield return null;
        }
        Soltar(p);
        yield return new WaitForSeconds(0.1f);
    }

    // Dentro de la neblina engañosa los controles van al revés: el bot, como un
    // jugador que ya aprendió la trampa, aprieta la tecla contraria.
    private Key TeclaHacia(Personaje p, float x)
    {
        FieldInfo campo = typeof(ControladorJugador).GetField("controlesInvertidos", BindingFlags.Instance | BindingFlags.NonPublic);
        bool invertido = campo != null && (bool)campo.GetValue(p.control) == true;
        bool haciaDerecha = p.cuerpo.position.x < x;
        if (invertido == true) { haciaDerecha = !haciaDerecha; }
        return haciaDerecha ? p.derecha : p.izquierda;
    }

    private void Soltar(Personaje p)
    {
        teclas.Remove(p.izquierda);
        teclas.Remove(p.derecha);
    }

    private IEnumerator Salto(Personaje p, float x)
    {
        float limite = Time.time + 5f;
        while (p.control.EstaEnElSuelo() == false)
        {
            if (Time.time > limite)
            {
                falloRuta = p.control.NombreJugador + " nunca tocó suelo para saltar hacia x=" + x;
                yield break;
            }
            yield return null;
        }

        teclas.Add(p.salto);
        float finSalto = Time.time + 0.1f;
        float minimoEnAire = Time.time + 0.25f;
        limite = Time.time + 5f;

        while (true)
        {
            if (Time.time > finSalto)
            {
                teclas.Remove(p.salto);
            }
            Soltar(p);
            if (Mathf.Abs(p.cuerpo.position.x - x) > 0.08f)
            {
                teclas.Add(TeclaHacia(p, x));
            }
            if (Time.time > minimoEnAire && p.control.EstaEnElSuelo() == true && Mathf.Abs(p.cuerpo.position.x - x) <= 0.2f)
            {
                break;
            }
            if (Time.time > limite)
            {
                falloRuta = p.control.NombreJugador + " no aterrizó en x=" + x + " y=" + p.cuerpo.position.y.ToString("F2");
                yield break;
            }
            yield return null;
        }
        Soltar(p);
        yield return new WaitForSeconds(0.1f);
    }

    private IEnumerator TocarHabilidad(Personaje p)
    {
        teclas.Add(p.habilidad);
        yield return new WaitForSeconds(0.1f);
        teclas.Remove(p.habilidad);
        yield return new WaitForSeconds(0.1f);
    }

    private void Teletransportar(Personaje p, float x, float piso)
    {
        Vector3 posicion = new Vector3(x, piso + 0.65f, 0f);
        p.cuerpo.position = posicion;
        p.control.transform.position = posicion;
        p.cuerpo.linearVelocity = Vector2.zero;
        Physics2D.SyncTransforms();
    }

    private bool Vivo(Personaje p)
    {
        return p.control.EstaVivo;
    }

    private bool ColliderActivo(string nombre)
    {
        return GameObject.Find(nombre).GetComponent<Collider2D>().enabled;
    }

    // ---------- Prueba de cada trampa ----------

    private const string escenaPrueba = "PruebaTrampas";
    private const float piso = 3f;
    private const float suelo = -3.5f;

    private IEnumerator ProbarTrampas()
    {
        Log("===== TRAMPAS =====");

        // SueloFalso
        yield return Cargar(escenaPrueba);
        Vector3 inicioSuelo = GameObject.Find("SueloFalso").transform.position;
        Teletransportar(inti, -12f, -0.75f);
        Verificar("SueloFalso sólido al empezar", ColliderActivo("SueloFalso"));
        yield return new WaitForSeconds(0.4f);
        Verificar("SueloFalso se cae al pisarlo", ColliderActivo("SueloFalso") == false);
        yield return new WaitForSeconds(0.6f);
        Verificar("Inti cayó al piso de abajo", inti.cuerpo.position.y < -2f);
        yield return new WaitForSeconds(3.2f);
        Verificar("SueloFalso vuelve a su lugar", ColliderActivo("SueloFalso") && GameObject.Find("SueloFalso").transform.position == inicioSuelo);

        // BloqueInvisible
        yield return Cargar(escenaPrueba);
        Verificar("BloqueInvisible no choca al empezar", ColliderActivo("BloqueInvisible") == false);
        Teletransportar(inti, -8f, suelo);
        yield return new WaitForSeconds(0.2f);
        teclas.Add(inti.salto);
        yield return new WaitForSeconds(0.1f);
        teclas.Remove(inti.salto);
        yield return new WaitForSeconds(0.5f);
        Verificar("BloqueInvisible aparece al saltar debajo", ColliderActivo("BloqueInvisible"));
        Verificar("El salto se cortó (no atravesó el bloque)", inti.cuerpo.position.y < -1f);

        // PuasOcultas: quedarse muere; saltarlas no las dispara.
        yield return Cargar(escenaPrueba);
        Teletransportar(inti, -4f, suelo);
        yield return new WaitForSeconds(0.5f);
        Verificar("PuasOcultas matan a quien se queda", Vivo(inti) == false);
        Log("   mensaje: " + Dato("mensajeCentral"));
        yield return new WaitForSeconds(0.2f);
        yield return Cargar(escenaPrueba);
        Teletransportar(inti, -5.4f, suelo);
        yield return new WaitForSeconds(0.2f);
        yield return Salto(inti, -2.6f);
        yield return new WaitForSeconds(0.5f);
        Verificar("Quien salta las púas sigue vivo", Vivo(inti));

        // TechoQueCae: quedarse muere; correr salva.
        yield return Cargar(escenaPrueba);
        Teletransportar(inti, 0f, suelo);
        yield return new WaitForSeconds(0.8f);
        Verificar("TechoQueCae mata a quien se queda debajo", Vivo(inti) == false);
        Log("   mensaje: " + Dato("mensajeCentral"));
        yield return new WaitForSeconds(0.2f);
        yield return Cargar(escenaPrueba);
        Teletransportar(inti, -2f, suelo);
        yield return new WaitForSeconds(0.2f);
        yield return Caminar(inti, 2.2f);
        yield return new WaitForSeconds(0.6f);
        Verificar("Quien corre debajo del techo se salva", Vivo(inti));

        // GemaTrampa
        yield return Cargar(escenaPrueba);
        int gemasAntes = (int)Dato("totalGemasFuego");
        Teletransportar(inti, 4f, suelo);
        yield return new WaitForSeconds(0.6f);
        Verificar("GemaTrampa desaparece y no cuenta como gema", GameObject.Find("GemaTrampa") == null && gemasAntes == 0);
        Verificar("GemaTrampa activa su trampa", Vivo(inti) == false);
        Log("   mensaje: " + Dato("mensajeCentral"));
        yield return new WaitForSeconds(0.2f);

        // PuertaFalsa
        yield return Cargar(escenaPrueba);
        float yPuerta = GameObject.Find("PuertaFalsa").transform.position.y;
        Teletransportar(inti, 7f, suelo);
        yield return new WaitForSeconds(0.8f);
        Verificar("PuertaFalsa se escapa", GameObject.Find("PuertaFalsa").transform.position.y > yPuerta + 2f);
        Verificar("PuertaFalsa no cuenta como salida", GameObject.Find("PuertaFalsa").GetComponent<PuertaSalida>() == null);

        // PlataformaQueHuye
        yield return Cargar(escenaPrueba);
        float yPlataforma = GameObject.Find("PlataformaQueHuye").transform.position.y;
        Teletransportar(inti, 11f, suelo);
        yield return new WaitForSeconds(0.2f);
        Verificar("La plataforma no huye si no saltas", Mathf.Abs(GameObject.Find("PlataformaQueHuye").transform.position.y - yPlataforma) < 0.01f);
        teclas.Add(inti.salto);
        yield return new WaitForSeconds(0.1f);
        teclas.Remove(inti.salto);
        yield return new WaitForSeconds(0.6f);
        Verificar("PlataformaQueHuye huye al saltar cerca", GameObject.Find("PlataformaQueHuye").transform.position.y > yPlataforma + 1f);

        // BotonEnganoso
        yield return Cargar(escenaPrueba);
        float yTecho = GameObject.Find("TechoDeBoton").transform.position.y;
        Teletransportar(inti, -12f, piso);
        yield return new WaitForSeconds(0.2f);
        yield return Caminar(inti, -9.8f);
        yield return new WaitForSeconds(0.5f);
        Verificar("BotonEnganoso suelta su trampa", GameObject.Find("TechoDeBoton").transform.position.y < yTecho - 0.5f);
        Verificar("Quien sigue corriendo se salva", Vivo(inti));

        // EnemigoSorpresa: quedarse muere; correr salva.
        yield return Cargar(escenaPrueba);
        Teletransportar(inti, -9f, piso);
        yield return new WaitForSeconds(1.3f);
        Verificar("EnemigoSorpresa atrapa a quien se queda", Vivo(inti) == false);
        Log("   mensaje: " + Dato("mensajeCentral"));
        yield return new WaitForSeconds(0.2f);
        yield return Cargar(escenaPrueba);
        Teletransportar(inti, -9.6f, piso);
        yield return new WaitForSeconds(0.1f);
        yield return Caminar(inti, -12.5f);
        yield return new WaitForSeconds(1.2f);
        Verificar("Quien huye del enemigo se salva", Vivo(inti));

        // Traición: quedarse muere; moverse a tiempo salva. Inti activa y Killa recibe.
        yield return ProbarTraicion("TraicionLava", -4f);
        yield return ProbarTraicion("TraicionPuas", -1.5f);
        yield return ProbarTraicionSuelo();
        yield return ProbarPalanca();
        yield return ProbarPlacas();
    }

    private IEnumerator ProbarTraicion(string nombre, float xBoton)
    {
        yield return Cargar(escenaPrueba);
        Teletransportar(killa, 10f, suelo);
        yield return new WaitForSeconds(0.2f);
        Teletransportar(inti, xBoton, piso);
        yield return new WaitForSeconds(1.5f);
        Verificar(nombre + " castiga a Killa, no a Inti", Vivo(killa) == false && Vivo(inti) == true);
        Log("   mensaje: " + Dato("mensajeCentral"));
        yield return new WaitForSeconds(0.2f);

        yield return Cargar(escenaPrueba);
        Teletransportar(killa, 10f, suelo);
        yield return new WaitForSeconds(0.2f);
        Teletransportar(inti, xBoton, piso);
        yield return new WaitForSeconds(0.3f);
        yield return Caminar(killa, 12.5f);
        yield return new WaitForSeconds(1.5f);
        Verificar(nombre + ": Killa se salva si se mueve a tiempo", Vivo(killa));
    }

    private IEnumerator ProbarTraicionSuelo()
    {
        yield return Cargar(escenaPrueba);
        Teletransportar(killa, 10f, suelo);
        yield return new WaitForSeconds(0.2f);
        Teletransportar(inti, 1f, piso);
        yield return new WaitForSeconds(2.5f);
        Verificar("TraicionSuelo hace caer a Killa", Vivo(killa) == false);
        Log("   mensaje: " + Dato("mensajeCentral"));
        yield return new WaitForSeconds(0.2f);

        yield return Cargar(escenaPrueba);
        Teletransportar(killa, 10f, suelo);
        yield return new WaitForSeconds(0.2f);
        Teletransportar(inti, 1f, piso);
        yield return new WaitForSeconds(0.3f);
        yield return Caminar(killa, 12.5f);
        yield return new WaitForSeconds(2f);
        Verificar("TraicionSuelo: Killa se salva si se mueve", Vivo(killa));
        yield return Caminar(killa, 10f);
        yield return new WaitForSeconds(0.3f);
        Verificar("El suelo vuelve a ser sólido después", Vivo(killa) && killa.cuerpo.position.y > -3.5f);
    }

    private IEnumerator ProbarPalanca()
    {
        yield return Cargar(escenaPrueba);
        Teletransportar(killa, 10f, suelo);
        Teletransportar(inti, 4f, piso);
        yield return new WaitForSeconds(0.3f);
        yield return TocarHabilidad(inti);
        yield return new WaitForSeconds(1.6f);
        Verificar("PalancaSospechosa castiga a Killa, no a Inti", Vivo(killa) == false && Vivo(inti) == true);
        Log("   mensaje: " + Dato("mensajeCentral"));
        yield return new WaitForSeconds(0.2f);
    }

    private IEnumerator ProbarPlacas()
    {
        yield return Cargar(escenaPrueba);
        Teletransportar(inti, 8f, piso);
        Teletransportar(killa, 12f, piso);
        yield return new WaitForSeconds(0.4f);
        Log("   Eclipse al pararse: Inti=" + inti.control.Elemento + " Killa=" + killa.control.Elemento);
        yield return Caminar(inti, 6f);
        yield return new WaitForSeconds(1.5f);
        Verificar("PlacaVinculada castiga a quien se queda", Vivo(killa) == false && Vivo(inti) == true);
        Log("   mensaje: " + Dato("mensajeCentral"));
        yield return new WaitForSeconds(0.2f);

        yield return Cargar(escenaPrueba);
        Teletransportar(inti, 8f, piso);
        Teletransportar(killa, 12f, piso);
        yield return new WaitForSeconds(0.4f);
        yield return Caminar(inti, 6f);
        yield return Caminar(killa, 13.5f);
        yield return new WaitForSeconds(1.5f);
        Verificar("PlacaVinculada: si el otro se baja a tiempo se salva", Vivo(killa));
    }

    // ---------- Sistema de muertes ----------

    private IEnumerator ProbarSistemaDeMuertes()
    {
        Log("===== MUERTES =====");
        MethodInfo terminar = typeof(GestorNivel).GetMethod("TerminarNivel", BindingFlags.Instance | BindingFlags.NonPublic);

        // Se entra desde otra escena: el contador empieza en cero.
        SceneManager.LoadScene("MenuPrincipal");
        yield return new WaitForSeconds(0.2f);
        yield return Cargar("Mundo1_Nivel1");
        yield return new WaitForSeconds(0.2f);
        Verificar("Al entrar, 0 muertes", (int)DatoEstatico("muertesDelIntento") == 0);

        float inicio = Time.realtimeSinceStartup;
        inti.control.Morir("Mensaje de prueba");
        GestorNivel gestorAntes = GestorNivel.Instancia;
        while (GestorNivel.Instancia == gestorAntes)
        {
            yield return null;
        }
        float demora = Time.realtimeSinceStartup - inicio;
        Log("   Reinició en " + demora.ToString("0.00") + " s");
        Verificar("Reinicio entre 0.4 y 0.65 s", demora >= 0.4f && demora <= 0.65f);
        yield return new WaitForSeconds(0.1f);
        Verificar("La muerte se contó y sobrevivió al reinicio", (int)DatoEstatico("muertesDelIntento") == 1);
        Verificar("El mensaje se muestra en el nuevo intento", (string)Dato("avisoTemporal") == "Mensaje de prueba");

        // Ganar con 1 muerte (el nivel permite 3): la tercera estrella se gana.
        terminar.Invoke(GestorNivel.Instancia, null);
        Verificar("Con 1 muerte (máx. 3) y sin gemas: 2 estrellas", (int)Dato("estrellasGanadas") == 2);
        Verificar("El récord queda en 1", GestorProgreso.ObtenerRecordMuertes(1, 1) == 1);

        // Repetir: empieza en cero. Morir 4 veces: se pierde la estrella de muertes.
        yield return Cargar("Mundo1_Nivel1");
        yield return new WaitForSeconds(0.2f);
        Verificar("Al repetir tras ganar, 0 muertes", (int)DatoEstatico("muertesDelIntento") == 0);
        for (int vez = 0; vez < 4; vez++)
        {
            inti = Crear("Inti", Key.A, Key.D, Key.W, Key.S);
            inti.control.Morir("Otra");
            yield return new WaitForSeconds(0.75f);
        }
        Verificar("Cuatro muertes contadas", (int)DatoEstatico("muertesDelIntento") == 4);
        terminar.Invoke(GestorNivel.Instancia, null);
        Verificar("Con 4 muertes (máx. 3) y sin gemas: 1 estrella", (int)Dato("estrellasGanadas") == 1);
        Verificar("Un récord peor no reemplaza al mejor", GestorProgreso.ObtenerRecordMuertes(1, 1) == 1);
        Verificar("Tampoco bajan las estrellas guardadas", GestorProgreso.ObtenerEstrellas(1, 1) == 2);
        GestorProgreso.BorrarProgreso();
        Verificar("Borrar progreso borra el récord", GestorProgreso.ObtenerRecordMuertes(1, 1) == -1);
        yield return new WaitForSeconds(0.1f);
    }

    // ---------- Rutas de los niveles (ya conociendo las trampas) ----------

    private IEnumerator RutaN1()
    {
        yield return Caminar(inti, -5.5f);
        yield return Caminar(inti, -13f);
        yield return Salto(inti, -10f);
        yield return Caminar(inti, -9.6f);
        yield return EsperarCriatura("Bicho del agua");
        // Aterriza junto al borde y salta enseguida: las púas tardan en salir.
        yield return Salto(inti, -5.25f);
        yield return Salto(inti, -2.2f);
        yield return Salto(inti, -0.6f);
        yield return Salto(inti, -2.5f);
        yield return Caminar(inti, -1.6f);
        yield return Salto(inti, 1.8f);
        yield return Caminar(inti, 2.7f);
        yield return Salto(inti, 6f);
        yield return Caminar(inti, 6.6f);
        // Salta más lejos para no pisar el tramo falso.
        yield return Salto(inti, 11.5f);
        yield return new WaitForSeconds(3.5f);

        // Salta la gema regalada.
        yield return Salto(killa, 4.2f);
        yield return Caminar(killa, 5.5f);
        yield return Caminar(killa, 13f);
        yield return Salto(killa, 10f);
        yield return Caminar(killa, 9.6f);
        yield return EsperarCriatura("Bicho de la lava");
        yield return Salto(killa, 6.2f);
        yield return Caminar(killa, 5.3f);
        yield return Salto(killa, 2.2f);
        yield return Salto(killa, 0.6f);
        yield return Salto(killa, 2.5f);
        yield return Caminar(killa, 1.6f);
        yield return Salto(killa, -1.8f);
        yield return Caminar(killa, -2.7f);
        yield return Salto(killa, -6f);
        yield return Caminar(killa, -6.6f);
        yield return Salto(killa, -10.2f);
        yield return Caminar(killa, -11.5f);
    }

    private IEnumerator RutaN2()
    {
        yield return Caminar(killa, 11.5f);
        yield return new WaitForSeconds(0.7f);
        // Inti pisa el botón falso corriendo: el bloque cae detrás de él.
        yield return Caminar(inti, -7.2f);
        Log("Inti pasó el botón falso y sigue vivo");
        yield return Salto(inti, -4.8f);
        yield return Caminar(inti, -3f);
        yield return Salto(inti, -7f);
        yield return Caminar(inti, -10f);
        yield return new WaitForSeconds(0.7f);
        yield return Caminar(killa, 7.2f);
        yield return Salto(killa, 4.8f);
        yield return Caminar(killa, 3f);
        yield return Salto(killa, 7f);
        yield return Caminar(inti, -5.3f);
        yield return Salto(inti, -2f);
        yield return Caminar(inti, -3.2f);
        yield return Salto(inti, -6.5f);
        yield return Caminar(inti, -7.3f);
        yield return EsperarCriatura("Bicho de la lava");
        yield return Caminar(inti, -11.5f);
        yield return TocarHabilidad(inti);
        yield return Caminar(killa, 5.3f);
        yield return Salto(killa, 2f);
        yield return Caminar(killa, 3.2f);
        yield return Salto(killa, 6.5f);
        yield return Caminar(killa, 7.3f);
        yield return EsperarCriatura("Bicho del agua");
        yield return Caminar(killa, 11.5f);
        yield return TocarHabilidad(killa);
        yield return new WaitForSeconds(1f);
        yield return EsperarCriatura("Bicho de la lava");
        yield return Caminar(inti, -5.3f);
        yield return Salto(inti, -2f);
        yield return Caminar(inti, 1.2f);
        yield return EsperarCriatura("Bicho del agua");
        yield return Caminar(killa, 5.3f);
        yield return Salto(killa, 2.4f);
        // En el piso de la salida no se salta: se camina hasta la puerta.
        yield return Caminar(killa, -1.2f);
    }

    private IEnumerator RutaN3()
    {
        yield return Caminar(inti, 0f);
        Log("Inti pasó bajo el techo que cae y sigue vivo");
        yield return Caminar(inti, 12f);
        yield return Caminar(killa, -12f);
        yield return new WaitForSeconds(0.3f);
        yield return Caminar(inti, 13.6f);
        yield return Salto(inti, 11f);
        yield return Caminar(inti, 0f);
        yield return Caminar(inti, -10f);
        yield return Caminar(killa, 10f);
        yield return Caminar(killa, 13.6f);
        yield return Salto(killa, 10f);
        yield return new WaitForSeconds(0.3f);
        yield return Caminar(inti, -13f);
        yield return Salto(inti, -9.5f);
        yield return Caminar(inti, 0f);
        yield return Caminar(inti, 12.3f);
        yield return new WaitForSeconds(1.2f);
        Log("El bloque sobre la placa ya cayó");
        yield return Caminar(inti, 10f);
        yield return Caminar(killa, -13f);
        yield return Salto(killa, -9.5f);
        yield return new WaitForSeconds(0.3f);
        yield return Caminar(inti, 13.5f);
        yield return Salto(inti, 9.5f);
        yield return Caminar(inti, 3.5f);
        yield return Salto(inti, 0f);
        yield return Caminar(inti, -0.5f);
        yield return EsperarCriatura("Bicho del veneno");
        yield return Salto(inti, -3.9f);
        yield return Caminar(inti, -4.3f);
        yield return Salto(inti, -7.6f);
        // Salta las púas ocultas.
        yield return Salto(inti, -10.2f);
        yield return Caminar(inti, -12.5f);
        yield return Caminar(killa, 13.5f);
        yield return Salto(killa, 9.5f);
        yield return Caminar(killa, 7.7f);
        yield return Salto(killa, 4f);
        yield return Caminar(killa, -0.4f);
        yield return EsperarCriatura("Bicho del veneno");
        yield return Salto(killa, -3.9f);
        yield return Caminar(killa, -4.3f);
        yield return Salto(killa, -7.6f);
        yield return Salto(killa, -10f);
    }

    private IEnumerator RutaN4()
    {
        yield return Caminar(inti, -2.7f);
        yield return Caminar(killa, 2.7f);
        yield return new WaitForSeconds(0.3f);
        yield return Caminar(inti, -6.5f);
        yield return Caminar(inti, -10f);
        yield return new WaitForSeconds(0.7f);
        yield return Caminar(killa, 6.5f);
        yield return Caminar(killa, 13.5f);
        yield return new WaitForSeconds(0.7f);
        yield return Caminar(inti, -14.2f);
        yield return Salto(inti, -11.9f);
        yield return Caminar(killa, 14.2f);
        yield return Salto(killa, 11.9f);
        // Killa pisa el botón de traición camino a su gema: a Inti le cae lava y se quita.
        // El enemigo sorpresa: Inti lo hace salir, retrocede al borde y espera
        // a que corra su tramo y desaparezca.
        yield return Caminar(inti, -11.3f);
        yield return Caminar(inti, -14.2f);
        yield return new WaitForSeconds(1.3f);
        yield return Salto(inti, -11.9f);
        Log("Inti esquivó al enemigo sorpresa");
        StartCoroutine(Caminar(killa, 8.5f));
        yield return Caminar(inti, -8.5f);
        yield return new WaitForSeconds(1.2f);
        Log("Inti esquivó la lava del botón de traición");
        yield return EsperarCriatura("Bicho del veneno izquierdo");
        yield return Salto(inti, -6.1f);
        yield return Caminar(inti, -5f);
        yield return Caminar(inti, -6.1f);
        yield return EsperarCriatura("Bicho del veneno izquierdo");
        yield return Salto(inti, -8.5f);
        yield return Caminar(inti, -11.9f);
        yield return Salto(inti, -10f);
        yield return EsperarCriatura("Bicho del veneno derecho");
        yield return Salto(killa, 6.1f);
        yield return Caminar(killa, 5f);
        yield return Caminar(killa, 6.1f);
        yield return EsperarCriatura("Bicho del veneno derecho");
        yield return Salto(killa, 8.5f);
        yield return Caminar(killa, 11.9f);
        yield return Salto(killa, 10f);
        yield return Caminar(killa, 6f);
        yield return Caminar(killa, 5.4f);
        yield return Salto(killa, 2.5f);
        yield return Caminar(killa, 0f);
        yield return new WaitForSeconds(0.4f);
        // Placa vinculada: Killa se baja e Inti tiene que irse enseguida.
        yield return Caminar(killa, 2.4f);
        yield return Caminar(inti, -5.4f);
        Log("Inti escapó de las púas de la placa vinculada");
        yield return Salto(inti, -2.5f);
        yield return Caminar(inti, -2.4f);
    }

    private IEnumerator CruzarBloques(Personaje p)
    {
        yield return Caminar(p, -6.45f);
        yield return Salto(p, -4.5f);
        yield return Caminar(p, -3.7f);
        yield return Salto(p, -1.5f);
        yield return Caminar(p, -0.7f);
        yield return Salto(p, 1.5f);
        yield return Caminar(p, 2.3f);
        yield return Salto(p, 4.5f);
        yield return Caminar(p, 5.3f);
        yield return Salto(p, 6.6f);
    }

    private IEnumerator RutaN5()
    {
        yield return CruzarBloques(inti);
        yield return Caminar(inti, 13.5f);
        yield return new WaitForSeconds(3.5f);
        yield return CruzarBloques(killa);
        yield return Caminar(killa, 8.5f);
        yield return new WaitForSeconds(0.7f);
        yield return Salto(inti, 9.5f);
        yield return Caminar(inti, 5.7f);
        yield return TocarHabilidad(inti);
        // La palanca le abre el suelo a Killa: se mueve enseguida.
        yield return Caminar(killa, 13.5f);
        Log("Killa escapó del suelo que se abrió");
        yield return Caminar(inti, -3f);
        yield return Caminar(inti, -10f);
        yield return Salto(killa, 9.5f);
        yield return Caminar(killa, 3.5f);
        yield return new WaitForSeconds(0.3f);
        yield return Caminar(inti, -14f);
        yield return Salto(inti, -10.5f);
        yield return Caminar(inti, 0f);
        yield return Caminar(inti, 8f);
        yield return Caminar(killa, -14f);
        yield return Salto(killa, -9f);
        yield return new WaitForSeconds(0.3f);
        yield return Caminar(inti, 13.5f);
        yield return Salto(inti, 10.6f);
        yield return new WaitForSeconds(0.7f);
        yield return Caminar(killa, 13.5f);
        yield return Salto(killa, 11.2f);
        // Salta a Inti, y después el botón de traición y el veneno de un solo salto.
        yield return Salto(killa, 9.3f);
        yield return EsperarCriatura("Bicho del veneno");
        yield return Salto(killa, 4.3f);
        Log("Killa saltó el botón de traición");
        yield return Caminar(killa, 2.5f);
        yield return Salto(killa, -0.5f);
        yield return Caminar(killa, -6.5f);
        yield return TocarHabilidad(killa);
        yield return Caminar(killa, -11f);
        yield return Caminar(inti, 9.3f);
        yield return EsperarCriatura("Bicho del veneno");
        yield return Salto(inti, 4.3f);
        yield return Caminar(inti, 2.6f);
        yield return Salto(inti, -0.5f);
        yield return Caminar(inti, -9f);
    }

    private IEnumerator RutaM2N1()
    {
        yield return Caminar(inti, -10.6f);
        yield return Salto(inti, -8.5f);
        yield return Salto(inti, -6f);
        yield return Salto(inti, -3.5f);
        teclas.Add(inti.habilidad);
        yield return Caminar(killa, -10.6f);
        yield return new WaitForSeconds(0.5f);
        yield return Salto(killa, -8.5f);
        yield return Salto(killa, -6f);
        yield return Caminar(inti, -1.5f);
        yield return new WaitForSeconds(0.3f);
        yield return Salto(killa, -3.5f);
        yield return Caminar(inti, 0.2f);
        yield return Salto(killa, -1f);
        teclas.Remove(inti.habilidad);

        // Inti ya sabe que algo sale del suelo: lo salta y sigue.
        yield return Caminar(inti, 0.6f);
        yield return Salto(inti, 3.9f);
        yield return Caminar(inti, 6.5f);
        Log("Inti saltó al enemigo sorpresa");
        yield return Caminar(inti, 4f);
        yield return new WaitForSeconds(0.5f);
        yield return Caminar(killa, 2.6f);
        yield return new WaitForSeconds(0.4f);
        yield return Salto(killa, 4.5f);
        yield return Caminar(inti, 8.5f);
        yield return new WaitForSeconds(0.2f);
        yield return Salto(killa, 6.5f);
        yield return new WaitForSeconds(0.7f);
        teclas.Add(inti.habilidad);
        yield return new WaitForSeconds(0.6f);
        yield return Salto(killa, 8.5f);
        yield return Salto(killa, 11.9f);
        teclas.Remove(inti.habilidad);
        yield return Caminar(killa, 11f);
        yield return TocarHabilidad(killa);
        yield return new WaitForSeconds(0.8f);

        // Saltan desde lejos para pasar por encima del bloque invisible.
        yield return Caminar(killa, 11.9f);
        yield return Salto(killa, 6f);
        Log("Bloque invisible sin activar: " + (ColliderActivo("Bloque escondido") == false));
        yield return Caminar(inti, 12.1f);
        yield return Salto(inti, 14f);
        yield return Salto(inti, 11.9f);
        yield return Salto(inti, 8f);
        yield return Salto(inti, 3f);

        yield return Salto(inti, 0f);
        yield return EsperarCriatura("Bicho del veneno");
        yield return Salto(inti, -2f);
        yield return Salto(inti, -4f);
        // El derrumbe: Inti lo provoca, retrocede y deja caer las tres rocas.
        yield return Caminar(inti, -4.7f);
        yield return Caminar(inti, -3.4f);
        yield return new WaitForSeconds(2.3f);
        Log("Inti esperó que cayera el derrumbe");
        teclas.Add(inti.habilidad);
        yield return Caminar(killa, 3f);
        yield return new WaitForSeconds(0.5f);
        yield return Salto(killa, 0f);
        yield return EsperarCriatura("Bicho del veneno");
        yield return Salto(killa, -2f);
        yield return Salto(inti, -7.5f);
        yield return Salto(killa, -4f);
        yield return Caminar(inti, -8.2f);
        yield return new WaitForSeconds(0.2f);
        yield return Salto(killa, -6.9f);
        teclas.Remove(inti.habilidad);

        // Los dos saltan la gema regalada.
        yield return Salto(inti, -11.8f);
        yield return Caminar(inti, -13.2f);
        yield return Caminar(killa, -8.4f);
        yield return Salto(killa, -10.5f);
    }

    // ---------- Mundo 2 ----------

    private IEnumerator TodoMundo2()
    {
        GestorProgreso.BorrarProgreso();
        yield return ProbarPiezasMundo2();
        Log("FALLOS EN PIEZAS: " + fallos);
        yield return NivelM2("Mundo2_Nivel1", RutaM2N1());
        yield return NivelM2("Mundo2_Nivel2", RutaM2N2());
        yield return NivelM2("Mundo2_Nivel3", RutaM2N3());
        yield return NivelM2("Mundo2_Nivel4", RutaM2N4());
        yield return NivelM2("Mundo2_Nivel5", RutaM2N5());
        GestorProgreso.BorrarProgreso();
        Log("FIN DE LAS PRUEBAS");
        yield return null;
#if UNITY_EDITOR
        TerminarPruebas();
#endif
    }

    private IEnumerator NivelM2(string escena, IEnumerator ruta)
    {
        yield return Nivel(escena, ruta);
    }

    // El bicho del charco salta con ritmo fijo: se espera a que vuelva a caer y
    // se cruza en ese momento (tiene casi 2 s de calma hasta el próximo salto).
    private IEnumerator EsperarCriatura(string nombre)
    {
        GameObject objeto = GameObject.Find(nombre);
        if (objeto == null) { Log("No existe la criatura " + nombre); yield break; }
        CriaturaQueSalta criatura = objeto.GetComponent<CriaturaQueSalta>();
        float limite = Time.time + 7f;
        while (Time.time < limite)
        {
            if (criatura.EstaEnElAire == false && criatura.SegundosDesdeLaUltimaCaida < 0.15f) { break; }
            yield return null;
        }
        if (Time.time >= limite) { Log("La criatura " + nombre + " no saltó (nadie cerca)"); }
    }

    private IEnumerator EsperarRocaCaida(string nombre)
    {
        SpriteRenderer dibujo = GameObject.Find(nombre).GetComponent<SpriteRenderer>();
        PeligroQueCae roca = dibujo.GetComponent<PeligroQueCae>();
        float limite = Time.time + 8f;
        // Espera el temblor y después que desaparezca: empieza un ciclo completo.
        while (roca != null && roca.EstaTemblando == false && Time.time < limite) { yield return null; }
        while (dibujo.enabled == true && Time.time < limite)
        {
            yield return null;
        }
    }

    // El murciélago vuela alto en el centro: se pasa cuando viene del extremo contrario.
    private IEnumerator EsperarMurcielagoEnExtremo(string nombre, float xExtremo)
    {
        Transform murcielago = GameObject.Find(nombre).transform;
        float limite = Time.time + 8f;
        while (Mathf.Abs(murcielago.position.x - xExtremo) > 0.3f && Time.time < limite)
        {
            yield return null;
        }
    }

    // Salta derecho hacia arriba y recién cuando el centro pasa "alturaParaMoverse"
    // se mueve hacia x: así rodea por arriba un bloque escondido.
    private IEnumerator SaltoRodeando(Personaje p, float x, float alturaParaMoverse)
    {
        teclas.Add(p.salto);
        float limite = Time.time + 3f;
        float alturaMaxima = p.cuerpo.position.y;
        bool estabaEnSuelo = p.control.EstaEnElSuelo();
        while (p.cuerpo.position.y < alturaParaMoverse && Time.time < limite) { alturaMaxima = Mathf.Max(alturaMaxima, p.cuerpo.position.y); yield return null; }
        Log("SaltoRodeando " + p.control.NombreJugador + " llegó a y=" + p.cuerpo.position.y.ToString("F2") + " máx=" + alturaMaxima.ToString("F2") + " suelo=" + estabaEnSuelo + " en x=" + p.cuerpo.position.x.ToString("F2"));
        // El salto se mantiene apretado para que llegue a su altura máxima.
        yield return Caminar(p, x);
        teclas.Remove(p.salto);
        float limiteSuelo = Time.time + 3f;
        while (p.control.EstaEnElSuelo() == false && Time.time < limiteSuelo) { yield return null; }
    }

    // La vagoneta va y viene sin parar: se espera a que llegue a su extremo y se
    // salta siguiéndola, hasta quedar parado encima.
    private IEnumerator SubirAVagonetaEnMovimiento(Personaje p, float xExtremo)
    {
        Transform vagoneta = GameObject.Find("Vagoneta").transform;
        float limite = Time.time + 20f;
        while (Mathf.Abs(vagoneta.position.x - xExtremo) > 0.6f && Time.time < limite) { yield return null; }
        teclas.Add(p.salto);
        float finSalto = Time.time + 0.15f;
        float minimoEnAire = Time.time + 0.3f;
        while (Time.time < limite)
        {
            if (Time.time > finSalto) { teclas.Remove(p.salto); }
            Soltar(p);
            float diferencia = vagoneta.position.x - p.cuerpo.position.x;
            if (Mathf.Abs(diferencia) > 0.15f) { teclas.Add(diferencia > 0f ? p.derecha : p.izquierda); }
            bool encima = p.cuerpo.position.y > vagoneta.position.y && Mathf.Abs(diferencia) < 0.6f;
            if (Time.time > minimoEnAire && p.control.EstaEnElSuelo() == true && encima == true) { break; }
            yield return null;
        }
        Soltar(p);
        teclas.Remove(p.salto);
        Log(p.control.NombreJugador + " subió a la vagoneta en movimiento");
    }

    private IEnumerator EsperarVagoneta(float x)
    {
        Transform vagoneta = GameObject.Find("Vagoneta").transform;
        float limite = Time.time + 12f;
        float siguienteRegistro = Time.time;
        while (Mathf.Abs(vagoneta.position.x - x) > 0.3f)
        {
            if (Time.time >= siguienteRegistro)
            {
                siguienteRegistro = Time.time + 0.5f;
                Log("vagoneta=" + vagoneta.position.ToString("F2") + " Inti=" + inti.cuerpo.position.ToString("F2") + " Killa=" + killa.cuerpo.position.ToString("F2"));
            }
            if (Time.time > limite)
            {
                falloRuta = "la vagoneta no llegó a x=" + x + " (está en " + vagoneta.position.x.ToString("F2") + ")";
                yield break;
            }
            yield return null;
        }
    }

    private IEnumerator EmpujarCaja(float xFinal)
    {
        Transform caja = GameObject.Find("Caja").transform;
        float limite = Time.time + 15f;
        teclas.Add(inti.derecha);
        teclas.Add(killa.derecha);
        while (caja.position.x < xFinal && Time.time < limite)
        {
            yield return null;
        }
        Soltar(inti);
        Soltar(killa);
        Log("Caja empujada hasta x=" + caja.position.x.ToString("F2"));
        if (caja.position.x < xFinal)
        {
            falloRuta = "la caja no llegó a x=" + xFinal;
        }
        yield return new WaitForSeconds(0.2f);
    }

    private IEnumerator ProbarPiezasMundo2()
    {
        Log("===== PIEZAS =====");

        // Murciélago: de costado mata, pisarlo lo aturde.
        yield return Cargar("Mundo2_Nivel2");
        EnemigoVolador murcielago = GameObject.Find("Murcielago").GetComponent<EnemigoVolador>();
        Vector2 pos = murcielago.transform.position;
        inti.cuerpo.position = new Vector2(pos.x, pos.y + 1.4f);
        inti.control.transform.position = new Vector3(pos.x, pos.y + 1.4f, 0f);
        inti.cuerpo.linearVelocity = new Vector2(0f, -8f);
        Physics2D.SyncTransforms();
        float limite = Time.time + 1f;
        while (murcielago.EstaAturdido == false && inti.control.EstaVivo == true && Time.time < limite)
        {
            // El murciélago se mueve: Inti cae siempre sobre él.
            Vector2 actual = murcielago.transform.position;
            if (inti.cuerpo.position.y > actual.y + 1f)
            {
                inti.cuerpo.position = new Vector2(actual.x, inti.cuerpo.position.y);
            }
            yield return null;
        }
        Verificar("Pisar al murciélago lo aturde", murcielago.EstaAturdido);
        Verificar("Quien lo pisa sigue vivo", Vivo(inti));
        yield return new WaitForSeconds(0.05f);
        Verificar("Quien lo pisa rebota hacia arriba", inti.control.VelocidadVertical > 3f);
        Vector3 quieto = murcielago.transform.position;
        yield return new WaitForSeconds(1.5f);
        Verificar("Aturdido no se mueve", murcielago.transform.position == quieto);
        yield return new WaitForSeconds(2f);
        Verificar("Después de 3 s despierta", murcielago.EstaAturdido == false && murcielago.transform.position != quieto);

        yield return Cargar("Mundo2_Nivel2");
        EnemigoVolador murcielagoDos = GameObject.Find("Murcielago").GetComponent<EnemigoVolador>();
        yield return new WaitForSeconds(0.1f);
        Vector2 posDos = murcielagoDos.transform.position;
        Teletransportar(inti, posDos.x, posDos.y - 0.65f);
        yield return new WaitForSeconds(0.2f);
        Verificar("Tocarlo de costado mata", Vivo(inti) == false);
        Log("   mensaje: " + Dato("mensajeCentral") + " | aviso: " + Dato("avisoTemporal"));
        yield return new WaitForSeconds(0.6f);

        // Roca: tiembla antes de caer, mata debajo y reaparece.
        yield return Cargar("Mundo2_Nivel2");
        PeligroQueCae roca = GameObject.Find("Roca del piso 1").GetComponent<PeligroQueCae>();
        Vector3 inicioRoca = roca.transform.position;
        float limiteRoca = Time.time + 5f;
        while (roca.EstaTemblando == false && Time.time < limiteRoca)
        {
            yield return null;
        }
        Verificar("La roca tiembla antes de caer", roca.EstaTemblando);
        float inicioTemblor = Time.time;
        while (roca.EstaTemblando == true)
        {
            yield return null;
        }
        Log("   Tembló " + (Time.time - inicioTemblor).ToString("0.00") + " s");
        Verificar("El temblor dura unos 0.5 s", Time.time - inicioTemblor > 0.4f && Time.time - inicioTemblor < 0.7f);
        Teletransportar(inti, inicioRoca.x, -0.25f);
        yield return new WaitForSeconds(0.5f);
        Verificar("La roca mata a quien está debajo", Vivo(inti) == false);
        yield return new WaitForSeconds(0.7f);

        yield return Cargar("Mundo2_Nivel2");
        PeligroQueCae rocaDos = GameObject.Find("Roca del piso 1").GetComponent<PeligroQueCae>();
        SpriteRenderer dibujoRoca = rocaDos.GetComponent<SpriteRenderer>();
        Vector3 inicioRocaDos = rocaDos.transform.position;
        yield return new WaitForSeconds(3.5f);
        Verificar("Al caer desaparece", dibujoRoca.enabled == false);
        yield return new WaitForSeconds(1.5f);
        Verificar("Después reaparece en su lugar", dibujoRoca.enabled == true);

        // Caja: uno solo no la mueve; los dos sí.
        yield return Cargar("Mundo2_Nivel3");
        Transform caja = GameObject.Find("Caja").transform;
        float xCaja = caja.position.x;
        Teletransportar(killa, xCaja - 3f, suelo);
        yield return Caminar(inti, xCaja - 1.15f);
        teclas.Add(inti.derecha);
        yield return new WaitForSeconds(0.6f);
        Soltar(inti);
        Verificar("Uno solo no mueve la caja", Mathf.Abs(caja.position.x - xCaja) < 0.05f);
        teclas.Add(inti.derecha);
        teclas.Add(killa.derecha);
        yield return new WaitForSeconds(1f);
        Soltar(inti);
        Soltar(killa);
        Log("   La caja avanzó " + (caja.position.x - xCaja).ToString("F2"));
        Verificar("Los dos juntos sí la mueven", caja.position.x > xCaja + 0.5f);

        // Vagoneta: lleva al jugador y se detiene al soltar el botón.
        yield return Cargar("Mundo2_Nivel2");
        Transform vagoneta = GameObject.Find("Vagoneta").transform;
        float xVagoneta = vagoneta.position.x;
        yield return Salto(killa, xVagoneta);
        yield return Caminar(inti, -13.5f);
        yield return new WaitForSeconds(1.5f);
        Verificar("El botón mueve la vagoneta", vagoneta.position.x > xVagoneta + 1f);
        Verificar("La vagoneta lleva al jugador", Mathf.Abs(killa.cuerpo.position.x - vagoneta.position.x) < 0.5f);
        yield return Caminar(inti, -11.5f);
        float detenida = vagoneta.position.x;
        yield return new WaitForSeconds(0.5f);
        Verificar("Al soltar el botón se detiene", Mathf.Abs(vagoneta.position.x - detenida) < 0.05f);
    }

    private IEnumerator RutaM2N2()
    {
        // Killa sube a la vagoneta e Inti la lleva desde su botón.
        yield return Salto(killa, -8.8f);
        yield return Caminar(inti, -13.5f);
        yield return EsperarVagoneta(8.8f);
        yield return Salto(killa, 11.2f);
        yield return EsperarVagoneta(-8.8f);
        yield return Caminar(inti, -10.6f);
        yield return Salto(inti, -8.8f);

        // Ahora Killa la lleva a ella desde el otro botón.
        yield return Caminar(killa, 13.5f);
        yield return EsperarVagoneta(8.8f);
        yield return Salto(inti, 11.2f);
        yield return new WaitForSeconds(0.2f);

        // Subida: pasan cuando la roca ya cayó.
        yield return Caminar(killa, 12.9f);
        yield return Salto(killa, 10.4f);
        yield return EsperarRocaCaida("Roca del piso 1");
        yield return Caminar(killa, 7f);
        yield return SaltoRodeando(killa, 3.8f, 3.05f);
        yield return Caminar(inti, 12.9f);
        yield return Salto(inti, 10.4f);
        yield return EsperarRocaCaida("Roca del piso 1");
        yield return Caminar(inti, 7f);
        yield return SaltoRodeando(inti, 3.8f, 3.05f);
        Log("Los dos pasaron la roca");

        // Inti primero, porque su puerta está más lejos.
        yield return EsperarMurcielagoEnExtremo("Murcielago", -7.5f);
        yield return Caminar(inti, -0.5f);
        yield return Salto(inti, -3.3f);
        yield return Caminar(inti, -12.2f);
        Log("Inti pasó al murciélago");

        yield return EsperarMurcielagoEnExtremo("Murcielago", -7.5f);
        yield return Caminar(killa, -6.6f);
        yield return Salto(killa, -10.4f);
    }

    private IEnumerator RutaM2N3()
    {
        // Los dos juntos empujan la caja hasta el borde del piso alto.
        yield return Caminar(inti, -5.9f);
        yield return Caminar(killa, -7f);
        yield return EmpujarCaja(6.35f);
        yield return Salto(inti, 6.2f);
        yield return Salto(inti, 8.3f);
        Log("Inti subió por la caja");
        yield return Salto(killa, 6.2f);
        yield return new WaitForSeconds(0.2f);

        // Inti: pasa la roca, salta las púas y sube.
        yield return EsperarRocaCaida("Roca del piso alto");
        // Bajo el piso final el techo es bajo: se camina rápido bajo la roca y se
        // saltan las púas con un salto corto.
        yield return Caminar(inti, 10.9f);
        Log("Inti pasó la roca: " + inti.cuerpo.position.ToString("F2"));
        yield return Salto(inti, 13.3f);
        Log("Inti pasó las púas: " + inti.cuerpo.position.ToString("F2"));
        // Al piso final se sube desde fuera de su borde, saltando primero hacia arriba.
        yield return Caminar(inti, 14.5f);
        yield return SaltoRodeando(inti, 11.3f, 5.2f);
        yield return Caminar(inti, 9.6f);
        yield return Salto(inti, 6.9f);
        // El murciélago vuela bajo en sus extremos y alto en el centro: se cruza
        // cuando sale del extremo izquierdo, para pasar bajo él cuando va alto.
        yield return EsperarMurcielagoEnExtremo("Murcielago", -3f);
        yield return Caminar(inti, -2f);
        yield return Caminar(inti, -11f);
        Log("Inti cruzó el río");

        // Killa recoge su gema y llega a la placa: eclipse.
        yield return Salto(killa, 8.3f);
        yield return EsperarRocaCaida("Roca del piso alto");
        yield return Caminar(killa, 10.9f);
        yield return Salto(killa, 13.3f);
        yield return Caminar(killa, 14.5f);
        yield return SaltoRodeando(killa, 11.3f, 5.2f);
        yield return Caminar(killa, 9.6f);
        yield return Salto(killa, 6.2f);
        yield return new WaitForSeconds(0.4f);
        Log("Eclipse: Inti=" + inti.control.Elemento + " Killa=" + killa.control.Elemento);

        // Con los poderes cambiados, cada uno entra por la puerta del poder que tiene.
        yield return Caminar(inti, -13f);
        yield return Caminar(killa, -2f);
        yield return Caminar(killa, -9.6f);
    }

    private IEnumerator RutaM2N4()
    {
        // Killa sube a la vagoneta antes de que Inti use la palanca traicionera.
        yield return Salto(killa, -7.8f);
        yield return Caminar(inti, -11f);
        yield return TocarHabilidad(inti);
        Log("Palanca usada con Killa a salvo en la vagoneta");
        yield return EsperarVagoneta(7.8f);
        yield return Salto(killa, 10f);
        yield return Caminar(inti, -9.5f);
        yield return SubirAVagonetaEnMovimiento(inti, -7.8f);
        yield return EsperarVagoneta(7.8f);
        yield return Salto(inti, 11.5f);

        // Inti alumbra el cristal desde abajo para Killa.
        yield return Caminar(killa, 11.4f);
        yield return new WaitForSeconds(0.4f);
        yield return Salto(killa, 13.5f);
        yield return Salto(killa, 11.5f);
        yield return Caminar(inti, 11.5f);
        yield return Salto(inti, 13.5f);
        yield return Salto(inti, 11.9f);
        yield return EsperarRocaCaida("Roca del piso 1");
        yield return Caminar(inti, 9.7f);
        yield return Salto(inti, 7f);
        Log("Inti llegó al piso 2");
        // Inti salta enseguida el botón de traición: así el piso que calentó al
        // aterrizar se enfría antes de que llegue Killa.
        yield return Salto(inti, 3.5f);
        // Espera que se enfríe el piso que Inti calentó al aterrizar.
        yield return new WaitForSeconds(3.2f);
        yield return EsperarRocaCaida("Roca del piso 1");
        yield return Caminar(killa, 9.7f);
        yield return SaltoRodeando(killa, 8.1f, 5.2f);

        // Saltan el botón de traición.

        yield return Salto(inti, 2f);
        yield return Salto(inti, -1f);
        yield return EsperarCriatura("Bicho del veneno");
        yield return Salto(inti, -4f);
        teclas.Add(inti.habilidad);
        yield return new WaitForSeconds(0.5f);
        yield return Caminar(killa, 8.1f);
        yield return Salto(killa, 3.5f);
        yield return Salto(killa, 2f);
        yield return Salto(killa, -1f);
        yield return Salto(inti, -6.5f);
        yield return Caminar(inti, -6.8f);
        yield return EsperarCriatura("Bicho del veneno");
        yield return Salto(killa, -4f);
        yield return Caminar(inti, -8f);
        yield return Salto(killa, -6f);
        teclas.Remove(inti.habilidad);
        yield return Caminar(inti, -13f);
        yield return Caminar(killa, -11f);
    }

    private IEnumerator RutaM2N5()
    {
        yield return Caminar(inti, -7f);
        yield return Caminar(killa, -8f);
        yield return EmpujarCaja(5.1f);
        yield return Salto(inti, 5f);
        yield return Salto(inti, 8.5f);
        yield return Salto(killa, 5f);
        yield return Salto(killa, 8f);
        Log("Los dos subieron por la caja");

        // Saltan las púas ocultas y pasan bajo la roca.
        yield return Caminar(inti, 9.7f);
        yield return Salto(inti, 12.3f);
        yield return EsperarRocaCaida("Roca del piso 1");
        yield return Caminar(inti, 13.4f);
        yield return Salto(inti, 10f);
        Log("Inti llegó a la palanca");
        yield return Caminar(killa, 9.7f);
        yield return Salto(killa, 12.3f);
        yield return EsperarRocaCaida("Roca del piso 1");
        yield return Caminar(killa, 13.4f);
        yield return Salto(killa, 11.4f);

        // La palanca le abre el suelo a Killa: se mueve enseguida.
        yield return TocarHabilidad(inti);
        yield return Caminar(killa, 8.7f);
        Log("Killa escapó del suelo que se abrió");
        yield return new WaitForSeconds(1.2f);

        // Viajan en la vagoneta, uno detrás del otro.
        yield return EsperarVagoneta(6.8f);
        yield return Salto(killa, 6.8f);
        yield return EsperarVagoneta(-7.8f);
        yield return Salto(killa, -11f);
        yield return EsperarVagoneta(6.8f);
        yield return Caminar(inti, 8.5f);
        yield return Salto(inti, 6.8f);
        yield return EsperarVagoneta(-7.8f);
        yield return Salto(inti, -10.2f);
        Log("Los dos cruzaron en la vagoneta");

        // Roca del piso 2: se sube desde afuera de su columna.
        // Suben al piso 3 cuando el murciélago de arriba está en su extremo derecho.
        yield return EsperarMurcielagoEnExtremo("Murcielago de arriba", -5f);
        yield return Caminar(inti, -10.9f);
        yield return Salto(inti, -7.5f);
        yield return EsperarMurcielagoEnExtremo("Murcielago de arriba", -5f);
        yield return Caminar(killa, -10.9f);
        yield return Salto(killa, -8.5f);

        // Cristales: Inti va delante alumbrando.
        yield return Caminar(inti, -5.4f);
        yield return Salto(inti, -2.8f);
        yield return EsperarCriatura("Bicho del hueco");
        yield return Salto(inti, 0f);
        teclas.Add(inti.habilidad);
        yield return new WaitForSeconds(0.5f);
        // Killa sale cuando el murciélago de arriba está en su otro extremo.
        yield return EsperarMurcielagoEnExtremo("Murcielago de arriba", -9f);
        yield return Caminar(killa, -5.4f);
        yield return Salto(killa, -2.8f);
        yield return Salto(inti, 2.6f);
        yield return EsperarCriatura("Bicho del hueco");
        yield return Salto(killa, 0f);
        yield return Salto(inti, 4.6f);
        yield return Salto(killa, 2.6f);
        yield return Caminar(inti, 5.2f);
        yield return new WaitForSeconds(0.2f);
        yield return Salto(killa, 4.6f);
        teclas.Remove(inti.habilidad);

        // Saltan el botón de traición y la gema regalada.
        yield return Salto(inti, 8.4f);
        yield return Salto(inti, 12f);
        Log("Inti llegó a su puerta");
        yield return Caminar(killa, 5.2f);
        yield return Salto(killa, 8.4f);
        yield return Salto(killa, 10.9f);
    }

    // ---------- Mundo 3 ----------

    private IEnumerator TodoMundo3()
    {
        GestorProgreso.BorrarProgreso();
        string soloNivel = System.Environment.GetEnvironmentVariable("SOLO_NIVEL");
        if (soloNivel == null || soloNivel == "" || soloNivel == "piezas")
        {
            yield return ProbarPiezasMundo3();
            Log("FALLOS EN PIEZAS: " + fallos);
        }
        if (soloNivel == null || soloNivel == "" || soloNivel == "1") { yield return Nivel("Mundo3_Nivel1", RutaM3N1()); }
        if (soloNivel == null || soloNivel == "" || soloNivel == "2") { yield return Nivel("Mundo3_Nivel2", RutaM3N2()); }
        if (soloNivel == null || soloNivel == "" || soloNivel == "3") { yield return Nivel("Mundo3_Nivel3", RutaM3N3()); }
        if (soloNivel == null || soloNivel == "" || soloNivel == "4") { yield return Nivel("Mundo3_Nivel4", RutaM3N4()); }
        if (soloNivel == null || soloNivel == "" || soloNivel == "5") { yield return Nivel("Mundo3_Nivel5", RutaM3N5()); }
        GestorProgreso.BorrarProgreso();
        Log("FIN DE LAS PRUEBAS");
        yield return null;
#if UNITY_EDITOR
        TerminarPruebas();
#endif
    }

    private IEnumerator EsperarFase(bool deDia)
    {
        float limite = Time.time + 15f;
        while (CicloDiaNoche.Instancia.EsDeDia != deDia || CicloDiaNoche.Instancia.SegundosRestantes < 3.2f)
        {
            if (Time.time > limite) { falloRuta = "no llegó la fase"; yield break; }
            yield return null;
        }
    }

    // Termina justo cuando empieza un tiempo seco.
    private IEnumerator EsperarInicioSeco()
    {
        Lluvia lluvia = FindFirstObjectByType<Lluvia>();
        float limite = Time.time + 25f;
        while (lluvia.Estado != EstadoLluvia.Lloviendo)
        {
            if (Time.time > limite) { falloRuta = "no llovió"; yield break; }
            yield return null;
        }
        while (lluvia.Estado != EstadoLluvia.Seco)
        {
            yield return null;
        }
    }

    // Termina justo cuando la piedra acaba de terminar su recorrido.
    // Espera a que la primera piedra termine de rodar y, poco después, también la
    // segunda: así hay un rato en que ninguna de las dos viene.
    private IEnumerator EsperarDosPiedras(string primera, string segunda)
    {
        PiedraRodante piedraUno = GameObject.Find(primera).GetComponent<PiedraRodante>();
        PiedraRodante piedraDos = GameObject.Find(segunda).GetComponent<PiedraRodante>();
        float limite = Time.time + 40f;
        while (Time.time < limite)
        {
            yield return EsperarPiedraQuieta(primera);
            float finUno = Time.time;
            while (piedraDos.EstaRodando == false && Time.time < finUno + 2.2f) { yield return null; }
            if (piedraDos.EstaRodando == false) { continue; }
            while (piedraDos.EstaRodando == true) { yield return null; }
            if (Time.time < finUno + 2.4f && piedraUno.EstaRodando == false) { yield break; }
        }
    }

    private IEnumerator EsperarPiedraQuieta(string nombre)
    {
        PiedraRodante piedra = GameObject.Find(nombre).GetComponent<PiedraRodante>();
        float limite = Time.time + 12f;
        while (piedra.EstaRodando == false)
        {
            if (Time.time > limite) { falloRuta = "la piedra no rodó: " + nombre; yield break; }
            yield return null;
        }
        while (piedra.EstaRodando == true)
        {
            yield return null;
        }
    }

    private IEnumerator ProbarPiezasMundo3()
    {
        Log("===== PIEZAS MUNDO 3 =====");

        // Niebla
        yield return Cargar("Mundo3_Nivel1");
        yield return new WaitForSeconds(0.2f);
        Texture2D texturaNiebla = FindFirstObjectByType<Niebla>().GetComponent<SpriteRenderer>().sprite.texture;
        Vector2 esquinaNiebla = new Vector2(-19f, -7f);
        Vector3 posInti = inti.control.transform.position;
        Color cercaDeInti = texturaNiebla.GetPixel(Mathf.RoundToInt((posInti.x - esquinaNiebla.x) * 2f), Mathf.RoundToInt((posInti.y - esquinaNiebla.y) * 2f));
        Color lejos = texturaNiebla.GetPixel(Mathf.RoundToInt((12f - esquinaNiebla.x) * 2f), Mathf.RoundToInt((8f - esquinaNiebla.y) * 2f));
        Verificar("Cerca del jugador no hay niebla (" + cercaDeInti.a.ToString("F2") + ")", cercaDeInti.a < 0.1f);
        Verificar("Lejos la niebla es espesa (" + lejos.a.ToString("F2") + ")", lejos.a > 0.8f);

        // Lluvia: apaga al fuego sin techo; bajo techo y con agua no pasa nada.
        yield return Cargar("Mundo3_Nivel2");
        yield return new WaitForSeconds(0.1f);
        Teletransportar(killa, 12f, suelo);
        Lluvia lluvia = FindFirstObjectByType<Lluvia>();
        float limite = Time.time + 15f;
        while (lluvia.Estado != EstadoLluvia.Lloviendo && Time.time < limite) { yield return null; }
        yield return new WaitForSeconds(2.2f);
        Verificar("Bajo techo Inti sigue vivo con lluvia", Vivo(inti));
        Verificar("Killa sin techo sigue viva con lluvia", Vivo(killa));
        Teletransportar(inti, 13f, suelo);
        while (lluvia.Estado != EstadoLluvia.Seco && Vivo(inti)) { yield return null; }
        if (Vivo(inti))
        {
            limite = Time.time + 15f;
            while (lluvia.Estado != EstadoLluvia.Lloviendo && Time.time < limite) { yield return null; }
            yield return new WaitForSeconds(1.4f);
        }
        Verificar("Sin techo Inti se apaga", Vivo(inti) == false);
        yield return new WaitForSeconds(1f);

        // Charco: Killa lo congela, Inti lo cruza y después se derrite.
        yield return Cargar("Mundo3_Nivel2");
        yield return new WaitForSeconds(0.2f);
        AguaCongelable charco = FindFirstObjectByType<AguaCongelable>();
        yield return Caminar(inti, -9.9f);
        yield return TocarHabilidad(killa);
        Verificar("Killa congela el charco", charco.EstaCongelada);
        yield return Caminar(inti, -6f);
        Verificar("Inti camina sobre el hielo", Vivo(inti));
        yield return Caminar(inti, -9.9f);
        yield return new WaitForSeconds(4.6f);
        Verificar("El hielo se derrite solo", charco.EstaCongelada == false);
        yield return TocarHabilidad(killa);
        yield return TocarHabilidad(inti);
        Verificar("Inti derrite el hielo", charco.EstaCongelada == false);

        // Día y noche
        yield return Cargar("Mundo3_Nivel3");
        yield return new WaitForSeconds(0.2f);
        CicloDiaNoche ciclo = CicloDiaNoche.Instancia;
        Verificar("Empieza de día", ciclo.EsDeDia);
        Verificar("Bloque de día sólido", ColliderActivo("Bloque de dia"));
        Verificar("Bloque de noche sin colisión", ColliderActivo("Bloque de noche") == false);
        yield return new WaitForSeconds(ciclo.SegundosRestantes + 0.2f);
        Verificar("Cambia a noche", ciclo.EsDeDia == false);
        Verificar("Bloque de noche sólido", ColliderActivo("Bloque de noche"));
        Verificar("Bloque de día sin colisión", ColliderActivo("Bloque de dia") == false);

        // Cóndor: aviso y picada; quien se queda quieto muere, quien camina no.
        yield return Cargar("Mundo3_Nivel3");
        yield return new WaitForSeconds(0.1f);
        Teletransportar(inti, -4f, pisoDosYungasBot);
        Teletransportar(killa, 14f, pisoUnoYungasBot);
        Transform condor = GameObject.Find("Condor").transform;
        float alturaCondor = condor.position.y;
        // Inti sobre el aire no: se le da un piso de prueba quieto bajando la gravedad.
        inti.cuerpo.gravityScale = 0f;
        limite = Time.time + 3f;
        while (Mathf.Abs(condor.position.y - alturaCondor) < 0.5f && Vivo(inti) && Time.time < limite) { yield return null; }
        Verificar("El cóndor se lanza", Mathf.Abs(condor.position.y - alturaCondor) >= 0.5f || Vivo(inti) == false);
        yield return new WaitForSeconds(1.2f);
        Verificar("El cóndor alcanza al que no se mueve", Vivo(inti) == false);
        yield return new WaitForSeconds(1f);

        // Piedra: rueda y mata a quien toca.
        yield return Cargar("Mundo3_Nivel3");
        yield return new WaitForSeconds(0.1f);
        limite = Time.time + 8f;
        while (Vivo(inti) && Vivo(killa) && Time.time < limite) { yield return null; }
        Verificar("La piedra mata a quien se queda abajo", Vivo(inti) == false || Vivo(killa) == false);
        yield return new WaitForSeconds(1f);
    }

    private const float pisoUnoYungasBot = -0.25f;
    private const float pisoDosYungasBot = 3f;

    // Termina justo cuando la fase cambia a la pedida: en ese instante existen
    // las plataformas de las dos fases.
    private IEnumerator EsperarCambioA(bool deDia)
    {
        float limite = Time.time + 20f;
        while (CicloDiaNoche.Instancia.EsDeDia == deDia)
        {
            if (Time.time > limite) { falloRuta = "no cambió la fase (1)"; yield break; }
            yield return null;
        }
        while (CicloDiaNoche.Instancia.EsDeDia != deDia)
        {
            if (Time.time > limite) { falloRuta = "no cambió la fase (2)"; yield break; }
            yield return null;
        }
    }

    private IEnumerator SaltarJuntos(float xInti, float xKilla)
    {
        Coroutine saltoInti = StartCoroutine(Salto(inti, xInti));
        Coroutine saltoKilla = StartCoroutine(Salto(killa, xKilla));
        yield return saltoInti;
        yield return saltoKilla;
    }

    private IEnumerator CaminarJuntos(float xInti, float xKilla)
    {
        Coroutine pasoInti = StartCoroutine(Caminar(inti, xInti));
        Coroutine pasoKilla = StartCoroutine(Caminar(killa, xKilla));
        yield return pasoInti;
        yield return pasoKilla;
    }

    private IEnumerator RutaM3N1()
    {
        yield return Caminar(inti, -9.7f);
        yield return SaltoRodeando(inti, -7f, 0.5f);
        yield return Caminar(inti, -6.1f);
        yield return Caminar(killa, -9.7f);
        yield return SaltoRodeando(killa, -7f, 0.5f);
        Log("Subieron al primer tramo");

        // Inti pisa la lava y salta el agua; Killa al revés.
        yield return Caminar(inti, -5.7f);
        yield return Salto(inti, -1.75f);
        yield return Caminar(inti, 0.3f);
        yield return Salto(inti, 5.6f);
        yield return Caminar(killa, -5.7f);
        yield return Salto(killa, -0.2f);
        yield return Salto(killa, 4.25f);
        yield return Caminar(killa, 6.3f);
        Log("Cruzaron los charcos");

        yield return Caminar(inti, 6.3f);
        // Derrumbe: Inti aterriza lejos de la primera roca y espera que caigan.
        yield return Salto(inti, 10.9f);
        yield return new WaitForSeconds(1.6f);
        Log("Inti esperó el derrumbe");
        yield return Caminar(inti, 12.4f);
        yield return SaltoRodeando(inti, 13.9f, 3.7f);
        yield return Caminar(inti, 14.2f);
        yield return SaltoRodeando(inti, 10.5f, 6.95f);
        yield return Salto(killa, 10.9f);
        Log("K1 " + killa.cuerpo.position.ToString("F2"));
        yield return Caminar(killa, 12.4f);
        yield return SaltoRodeando(killa, 13.9f, 3.7f);
        Log("K2 " + killa.cuerpo.position.ToString("F2"));
        yield return Caminar(killa, 14.2f);
        yield return SaltoRodeando(killa, 11f, 6.95f);
        Log("K3 " + killa.cuerpo.position.ToString("F2"));
        Log("Llegaron arriba");

        // Saltan por encima del tramo falso.
        yield return Caminar(inti, 9.6f);
        yield return Caminar(inti, 7.8f);
        yield return Salto(inti, 3.4f);
        yield return Salto(inti, -1.9f);
        yield return Caminar(killa, 9.6f);
        Log("K35 " + killa.cuerpo.position.ToString("F2") + " voltea=" + GameObject.Find("Vuelta 1").transform.position.ToString("F2") + " activo=" + GameObject.Find("Vuelta 1").GetComponent<Collider2D>().enabled);
        yield return Caminar(killa, 7.8f);
        // Espera que la plataforma que se voltea vuelva a su lugar.
        Log("K4 " + killa.cuerpo.position.ToString("F2"));
        yield return new WaitForSeconds(3f);
        yield return Salto(killa, 3.4f);
        yield return Salto(killa, -2.1f);
        yield return Caminar(inti, -2.9f);
        yield return Salto(inti, -7f);
        yield return Salto(inti, -10.5f);
        yield return Caminar(inti, -13.5f);
        yield return Caminar(killa, -2.9f);
        yield return Salto(killa, -7f);
        yield return Salto(killa, -10.5f);
        yield return Caminar(killa, -11f);
    }

    private IEnumerator RutaM3N2()
    {
        // Espera a que acabe una lluvia y empieza con tiempo seco por delante.
        yield return EsperarInicioSeco();
        yield return Caminar(killa, -7f);
        yield return TocarHabilidad(killa);
        yield return Caminar(inti, -6.25f);
        yield return Caminar(inti, -1.9f);
        Log("Inti cruzó el hielo");

        yield return Caminar(killa, -4.2f);
        yield return Salto(killa, -1.5f);
        yield return Caminar(killa, 0.3f);
        yield return Caminar(inti, 0.7f);
        yield return new WaitForSeconds(0.4f);
        Log("Compuerta abierta por Inti");
        yield return Caminar(killa, 3.4f);
        yield return new WaitForSeconds(0.4f);
        Log("Compuerta abierta por Killa");

        yield return EsperarInicioSeco();
        yield return Caminar(inti, 7f);
        yield return Caminar(killa, 10.2f);
        yield return Salto(killa, 8.5f);
        yield return Caminar(killa, 7.1f);
        yield return Salto(killa, 5f);
        yield return Salto(killa, 0f);
        yield return Caminar(killa, -1f);
        yield return Caminar(killa, -3.8f);
        yield return Salto(killa, -6.5f);
        yield return Caminar(killa, -8.5f);
        Log("Killa llegó a su puerta");

        yield return EsperarInicioSeco();
        yield return Caminar(inti, 10.2f);
        yield return Salto(inti, 8f);
        yield return Caminar(inti, 7.1f);
        yield return Salto(inti, 4.5f);
        yield return Caminar(inti, 3.2f);
        yield return Salto(inti, -1f);
        yield return Caminar(inti, -3.8f);
        yield return Salto(inti, -6f);
        yield return Caminar(inti, -11.5f);
    }

    private IEnumerator RutaM3N3()
    {
        // Cada uno recoge su gema y se aparta del camino de la piedra.
        yield return CaminarJuntos(-9f, 9f);
        yield return CaminarJuntos(-4.9f, 4.9f);

        yield return EsperarFase(true);
        yield return Salto(inti, -7.25f);
        // La gema está sobre el piso que se envenena: se toma y se sale enseguida.
        yield return Salto(inti, -2f);
        Log("Inti en el mirador");
        yield return Caminar(inti, -3.15f);
        yield return EsperarFase(false);
        yield return Salto(killa, 7.25f);
        yield return Salto(killa, 2f);
        Log("Killa en el mirador");
        yield return EsperarFase(true);
        yield return Salto(inti, -7.25f);
        // Bajo la ladera el techo es bajo: salto corto y después caminar.
        yield return Caminar(inti, -8.6f);
        yield return Salto(inti, -11.4f);
        yield return Caminar(inti, -13f);
        Log("Inti en su placa");
        yield return Caminar(killa, 2.7f);
        yield return EsperarFase(false);
        yield return Salto(killa, 7.25f);
        yield return Caminar(killa, 8.6f);
        yield return Salto(killa, 11.4f);
        yield return Caminar(killa, 13f);
        yield return new WaitForSeconds(0.3f);
        Log("Antes del eclipse: Inti=" + inti.cuerpo.position.ToString("F2") + " Killa=" + killa.cuerpo.position.ToString("F2"));
        Verificar("Hubo eclipse", inti.control.Elemento == TipoElemento.Agua && killa.control.Elemento == TipoElemento.Fuego);

        yield return Caminar(inti, -12.6f);
        yield return Salto(inti, -9.8f);
        yield return Caminar(inti, -7.7f);
        yield return Caminar(killa, 12.6f);
        yield return Salto(killa, 9.8f);
        yield return Caminar(killa, 7.7f);
        yield return EsperarFase(false);
        yield return Salto(inti, -5.5f);
        yield return Salto(inti, -1.5f);
        Log("Inti en la salida");
        yield return EsperarFase(true);
        yield return Salto(killa, 5.5f);
        yield return Salto(killa, 1.5f);
    }

    private IEnumerator RutaM3N4()
    {
        yield return CaminarJuntos(-13.3f, -13.3f);
        yield return SaltarJuntos(-11f, -11.3f);
        yield return EsperarFase(true);
        yield return SaltarJuntos(-5.5f, -5.8f);
        yield return CaminarJuntos(-4.5f, -4.5f);

        // Saltan justo cuando cambia la luz: hay un instante en que existen los dos bloques.
        yield return EsperarCambioA(false);
        yield return SaltarJuntos(0f, 0f);
        yield return CaminarJuntos(1f, 1f);
        yield return EsperarCambioA(true);
        yield return SaltarJuntos(5.5f, 5.5f);
        yield return CaminarJuntos(6.5f, 6.5f);
        // Saltan el botón de traición del descanso.
        yield return SaltarJuntos(12f, 12f);
        Log("Cruzaron el puente de abajo");

        yield return CaminarJuntos(11.9f, 12.3f);
        yield return Caminar(inti, 12.4f);
        yield return SaltoRodeando(inti, 13.6f, 3.75f);
        yield return Caminar(inti, 13.25f);
        yield return SaltoRodeando(inti, 11.2f, 7f);
        Log("I-mirador " + inti.cuerpo.position.ToString("F2"));
        yield return Caminar(inti, 10f);
        yield return TocarHabilidad(inti);
        Log("Palanca usada; Killa escapa");
        // Killa sale de la marca roja hacia la derecha y espera que pasen las púas.
        yield return Caminar(killa, 13.9f);
        yield return new WaitForSeconds(2.5f);
        yield return Caminar(inti, 7.9f);
        yield return Caminar(killa, 12.4f);
        yield return SaltoRodeando(killa, 13.6f, 3.75f);
        yield return Caminar(killa, 13.25f);
        yield return SaltoRodeando(killa, 11.2f, 7f);
        yield return Caminar(killa, 7.9f);

        yield return EsperarFase(false);
        yield return SaltarJuntos(3.5f, 3.5f);
        yield return CaminarJuntos(2.5f, 2.5f);
        yield return EsperarCambioA(true);
        yield return SaltarJuntos(-2f, -2f);
        yield return CaminarJuntos(-3f, -3f);
        yield return EsperarCambioA(false);
        yield return SaltarJuntos(-7.5f, -7.5f);
        yield return CaminarJuntos(-8.6f, -8.6f);
        yield return SaltarJuntos(-10.5f, -10.5f);
        yield return CaminarJuntos(-13.5f, -11.5f);
    }

    // Cruza los tres bloques que están bajo el camino de arriba.
    private IEnumerator CruzarBloquesM3(Personaje p)
    {
        yield return Caminar(p, -7.75f);
        yield return EsperarFase(true);
        yield return SaltoRodeando(p, -4.5f, 3.7f);
        yield return Caminar(p, -3.1f);
        yield return EsperarCambioA(false);
        yield return Salto(p, 0.5f);
        yield return Caminar(p, 1.9f);
        yield return EsperarCambioA(true);
        yield return Salto(p, 5.5f);
        yield return Caminar(p, 6.9f);
        yield return Salto(p, 8.9f);
    }

    private IEnumerator RutaM3N5()
    {
        // Killa congela y los dos cruzan el charco.
        yield return EsperarInicioSeco();
        yield return Caminar(killa, 6f);
        yield return TocarHabilidad(killa);
        yield return Caminar(inti, 3.75f);
        yield return Caminar(inti, -1.5f);
        Log("Inti cruzó el charco");
        yield return Caminar(killa, -1f);

        // Saltan el botón y suben a la terraza cuando la piedra ya pasó.
        yield return EsperarPiedraQuieta("Piedra de abajo");
        yield return Caminar(inti, -3.4f);
        yield return Salto(inti, -6.05f);
        yield return Salto(inti, -8.5f);
        Log("Inti en la terraza");
        yield return EsperarPiedraQuieta("Piedra de abajo");
        yield return Caminar(killa, -3.4f);
        yield return Salto(killa, -6.05f);
        yield return Salto(killa, -10f);
        Log("Killa en la terraza");

        // Inti cruza primero, baja la palanca y Killa escapa de las púas.
        yield return CruzarBloquesM3(inti);
        Log("Inti cruzó los bloques");
        yield return EsperarPiedraQuieta("Piedra del medio");
        yield return Caminar(inti, 11.5f);
        yield return Caminar(inti, 13f);
        yield return TocarHabilidad(inti);
        Log("Palanca usada");
        // Vuelve enseguida, antes de que la piedra ruede otra vez.
        Coroutine vuelveInti = StartCoroutine(Caminar(inti, 8.9f));
        yield return Caminar(killa, -8.5f);
        yield return vuelveInti;
        Log("Inti volvió de la palanca");

        yield return CruzarBloquesM3(killa);
        Log("Killa cruzó los bloques");

        // Suben al camino alto cuando no llueve y las piedras no vienen.
        yield return EsperarInicioSeco();
        yield return EsperarPiedraQuieta("Piedra de arriba");
        yield return Caminar(inti, 10.2f);
        yield return Salto(inti, 8f);
        yield return Caminar(inti, 0.5f);
        yield return Salto(inti, -3f);
        yield return Caminar(inti, -13f);
        Log("Inti llegó a su puerta");
        yield return EsperarDosPiedras("Piedra del medio", "Piedra de arriba");
        yield return Caminar(killa, 10.2f);
        yield return Salto(killa, 8f);
        yield return Caminar(killa, 3.75f);
        yield return Caminar(killa, 0.5f);
        yield return Salto(killa, -3f);
        yield return Caminar(killa, -11f);
    }

    // ---------- Mundo 4 ----------

    private IEnumerator TodoMundo4()
    {
        GestorProgreso.BorrarProgreso();
        string modo = System.Environment.GetEnvironmentVariable("SOLO_NIVEL");
        if (modo == null) { modo = ""; }

        if (modo == "" || modo == "piezas")
        {
            yield return ProbarPiezasMundo4();
            Log("FALLOS EN PIEZAS: " + fallos);
        }
        if (modo == "" || modo == "1") { yield return Nivel("Mundo4_Nivel1", RutaM4N1()); }
        if (modo == "" || modo == "2") { yield return Nivel("Mundo4_Nivel2", RutaM4N2()); }
        if (modo == "" || modo == "3") { yield return Nivel("Mundo4_Nivel3", RutaM4N3()); }
        if (modo == "" || modo == "4") { yield return Nivel("Mundo4_Nivel4", RutaM4N4()); }
        if (modo == "" || modo == "5") { yield return Nivel("Mundo4_Nivel5", RutaM4N5()); }
        if (modo == "fotos") { yield return FotosMundo4(); }
        if (modo == "antiguos")
        {
            yield return Nivel("Mundo1_Nivel1", RutaN1());
            yield return Nivel("Mundo1_Nivel2", RutaN2());
            yield return Nivel("Mundo1_Nivel3", RutaN3());
            yield return Nivel("Mundo1_Nivel4", RutaN4());
            yield return Nivel("Mundo1_Nivel5", RutaN5());
            yield return Nivel("Mundo2_Nivel1", RutaM2N1());
            yield return Nivel("Mundo2_Nivel2", RutaM2N2());
            yield return Nivel("Mundo2_Nivel3", RutaM2N3());
            yield return Nivel("Mundo2_Nivel4", RutaM2N4());
            yield return Nivel("Mundo2_Nivel5", RutaM2N5());
            yield return Nivel("Mundo3_Nivel1", RutaM3N1());
            yield return Nivel("Mundo3_Nivel2", RutaM3N2());
            yield return Nivel("Mundo3_Nivel3", RutaM3N3());
            yield return Nivel("Mundo3_Nivel4", RutaM3N4());
            yield return Nivel("Mundo3_Nivel5", RutaM3N5());
        }
        GestorProgreso.BorrarProgreso();
        Log("FIN DE LAS PRUEBAS");
        yield return null;
#if UNITY_EDITOR
        TerminarPruebas();
#endif
    }

    private IEnumerator FotosMundo4()
    {
        for (int numero = 1; numero <= 5; numero++)
        {
            yield return Cargar("Mundo4_Nivel" + numero);
            yield return new WaitForSeconds(numero == 4 ? 9f : 1f);
            SacarFoto("m4n" + numero);
        }
        yield return Cargar("Mundo2_Nivel3");
        yield return new WaitForSeconds(0.5f);
        SacarFoto("m2n3");
    }

    // Salta hacia la liana y sigue apuntando a su punta hasta quedar colgado.
    private IEnumerator AgarrarLiana(Personaje p, string nombre, float distanciaAlSaltar)
    {
        Liana liana = GameObject.Find(nombre).GetComponent<Liana>();
        float limite = Time.time + 8f;
        // Espera a que la punta venga acercándose.
        float distanciaAnterior = Mathf.Abs(liana.Punta.x - p.cuerpo.position.x);
        while (Time.time < limite)
        {
            float distancia = Mathf.Abs(liana.Punta.x - p.cuerpo.position.x);
            bool seAcerca = distancia < distanciaAnterior;
            distanciaAnterior = distancia;
            if (seAcerca == true && distancia <= distanciaAlSaltar && p.control.EstaEnElSuelo() == true)
            {
                break;
            }
            yield return null;
        }

        teclas.Add(p.salto);
        float finSalto = Time.time + 0.1f;
        limite = Time.time + 2.5f;
        while (p.control.EstaEnLiana == false)
        {
            if (Time.time > finSalto) { teclas.Remove(p.salto); }
            Soltar(p);
            teclas.Add(TeclaHacia(p, liana.Punta.x));
            if (Time.time > limite)
            {
                falloRuta = p.control.NombreJugador + " no alcanzó " + nombre;
                yield break;
            }
            yield return null;
        }
        Soltar(p);
        teclas.Remove(p.salto);
    }

    // Colgado, espera el extremo del balanceo hacia ese lado y se suelta hacia x.
    private IEnumerator SoltarLianaHacia(Personaje p, string nombre, float direccion, float x, float fraccionExtremo = 0.424f)
    {
        Liana liana = GameObject.Find(nombre).GetComponent<Liana>();
        float centro = liana.transform.position.x;
        float largo = Vector2.Distance(liana.transform.position, liana.Punta);
        float extremo = largo * fraccionExtremo;
        float limite = Time.time + 8f;
        while ((liana.Punta.x - centro) * direccion < extremo)
        {
            if (Time.time > limite) { falloRuta = "la liana no llegó al extremo"; yield break; }
            yield return null;
        }

        teclas.Add(p.salto);
        float finSalto = Time.time + 0.1f;
        limite = Time.time + 4f;
        while (true)
        {
            if (Time.time > finSalto) { teclas.Remove(p.salto); }
            Soltar(p);
            if (Mathf.Abs(p.cuerpo.position.x - x) > 0.08f)
            {
                teclas.Add(TeclaHacia(p, x));
            }
            bool yaLlego = p.control.EstaEnElSuelo() == true && Mathf.Abs(p.cuerpo.position.x - x) <= 0.25f;
            if (Time.time > finSalto + 0.2f && (yaLlego == true || p.control.EstaEnLiana == true))
            {
                break;
            }
            if (Time.time > limite)
            {
                falloRuta = p.control.NombreJugador + " no llegó a x=" + x + " al soltarse";
                yield break;
            }
            yield return null;
        }
        Soltar(p);
        yield return new WaitForSeconds(0.1f);
    }

    // Camina hasta la hoja y, ya en lo alto, se dirige al destino.
    private IEnumerator RebotarHacia(Personaje p, float xHoja, float xDestino, float alturaPiso)
    {
        yield return CaminarHastaRebote(p, xHoja);
        float limite = Time.time + 3f;
        while (p.cuerpo.position.y - 0.65f < alturaPiso + 0.2f)
        {
            if (Time.time > limite) { falloRuta = p.control.NombreJugador + " no subió con la hoja"; yield break; }
            yield return null;
        }
        limite = Time.time + 4f;
        while (true)
        {
            Soltar(p);
            if (Mathf.Abs(p.cuerpo.position.x - xDestino) > 0.08f)
            {
                teclas.Add(TeclaHacia(p, xDestino));
            }
            if (p.control.EstaEnElSuelo() == true && Mathf.Abs(p.cuerpo.position.x - xDestino) <= 0.2f)
            {
                break;
            }
            if (Time.time > limite) { falloRuta = p.control.NombreJugador + " no llegó tras rebotar a x=" + xDestino; yield break; }
            yield return null;
        }
        Soltar(p);
        yield return new WaitForSeconds(0.1f);
    }

    private IEnumerator CaminarHastaRebote(Personaje p, float xHoja)
    {
        float limite = Time.time + 8f;
        while (p.control.VelocidadVertical < 10f)
        {
            Soltar(p);
            if (Mathf.Abs(p.cuerpo.position.x - xHoja) > 0.1f)
            {
                teclas.Add(TeclaHacia(p, xHoja));
            }
            if (Time.time > limite) { falloRuta = p.control.NombreJugador + " no llegó a la hoja"; yield break; }
            yield return null;
        }
        Soltar(p);
    }

    private IEnumerator ProbarPiezasMundo4()
    {
        Log("===== PIEZAS MUNDO 4 =====");

        // Liana: agarrar, seguirla y soltarse hacia arriba.
        yield return Cargar("Mundo4_Nivel1");
        yield return new WaitForSeconds(0.2f);
        yield return Caminar(inti, -9.7f);
        yield return AgarrarLiana(inti, "Liana 1", 3.5f);
        Verificar("Inti se agarra a la liana", inti.control.EstaEnLiana);
        Vector2 antes = inti.cuerpo.position;
        yield return new WaitForSeconds(0.5f);
        Verificar("La liana lo lleva", Vector2.Distance(antes, inti.cuerpo.position) > 0.5f);
        Verificar("Colgado no se cae", Vivo(inti));
        teclas.Add(inti.salto);
        yield return new WaitForSeconds(0.1f);
        teclas.Remove(inti.salto);
        Verificar("El salto lo suelta", inti.control.EstaEnLiana == false);
        Verificar("Sale impulsado hacia arriba", inti.control.VelocidadVertical > 5f);

        // Hoja: lanza más alto que el salto normal.
        yield return Cargar("Mundo4_Nivel2");
        yield return new WaitForSeconds(0.2f);
        float alturaInicial = inti.cuerpo.position.y;
        yield return CaminarHastaRebote(inti, -4f);
        float mayorAltura = alturaInicial;
        float limite = Time.time + 1.2f;
        while (Time.time < limite)
        {
            mayorAltura = Mathf.Max(mayorAltura, inti.cuerpo.position.y);
            yield return null;
        }
        Log("   La hoja subió " + (mayorAltura - alturaInicial).ToString("F2"));
        Verificar("La hoja lanza más alto que un salto", mayorAltura - alturaInicial > 5f);

        // Serpiente: va y viene, y mata de costado.
        yield return Cargar("Mundo4_Nivel3");
        yield return new WaitForSeconds(0.1f);
        Transform serpiente = GameObject.Find("Serpiente").transform;
        float x1 = serpiente.position.x;
        yield return new WaitForSeconds(0.6f);
        Verificar("La serpiente camina", Mathf.Abs(serpiente.position.x - x1) > 0.5f);
        Teletransportar(killa, 0f, suelo);
        limite = Time.time + 3f;
        while (Vivo(killa) && Time.time < limite) { yield return null; }
        Verificar("La serpiente mata de costado", Vivo(killa) == false);
        yield return new WaitForSeconds(1f);

        // Mono: avisa, lanza un coco y mata al que se queda quieto bajo el arco.
        yield return Cargar("Mundo4_Nivel2");
        yield return new WaitForSeconds(0.1f);
        Teletransportar(inti, 3f, 9.25f);
        Teletransportar(killa, 8.5f, 3f);
        limite = Time.time + 6f;
        bool huboCoco = false;
        while (Vivo(inti) && Time.time < limite)
        {
            if (GameObject.Find("Proyectil") != null) { huboCoco = true; }
            yield return null;
        }
        Verificar("El mono lanza cocos", huboCoco);
        Verificar("El coco mata al que no se mueve", Vivo(inti) == false);
        yield return new WaitForSeconds(1f);

        // Crecida: sube y se lleva al que se queda abajo.
        yield return Cargar("Mundo4_Nivel4");
        yield return new WaitForSeconds(0.1f);
        CrecidaRio crecida = FindFirstObjectByType<CrecidaRio>();
        float nivel1 = crecida.NivelActual;
        yield return new WaitForSeconds(3.6f);
        Verificar("La crecida sube", crecida.NivelActual > nivel1);
        limite = Time.time + 5f;
        while (Vivo(inti) && Time.time < limite) { yield return null; }
        Verificar("La crecida se lleva al que se queda", Vivo(inti) == false);
        yield return new WaitForSeconds(1f);

        // Roca: el ciclo debe ser corto.
        yield return Cargar("Mundo2_Nivel2");
        SpriteRenderer roca = GameObject.Find("Roca del piso 1").GetComponent<SpriteRenderer>();
        limite = Time.time + 5f;
        while (roca.enabled == true && Time.time < limite) { yield return null; }
        while (roca.enabled == false && Time.time < limite) { yield return null; }
        float inicioCiclo = Time.time;
        while (roca.enabled == true && Time.time < inicioCiclo + 5f) { yield return null; }
        float tiempoColgada = Time.time - inicioCiclo;
        Log("   La roca pasa colgada " + tiempoColgada.ToString("F2") + " s");
        Verificar("La roca cae seguido (menos de 1.5 s colgada)", tiempoColgada < 1.5f);

        // Caja: empujar solo muestra el aviso.
        yield return Cargar("Mundo2_Nivel3");
        yield return new WaitForSeconds(0.2f);
        yield return Caminar(inti, -6f);
        teclas.Add(inti.derecha);
        yield return new WaitForSeconds(1.2f);
        Soltar(inti);
        string aviso = (string)Dato("avisoTemporal");
        Verificar("Empujar solo explica qué falta: " + aviso, aviso.Contains("juntos"));
    }

    private IEnumerator RutaM4N1()
    {
        // Killa sube a la rama por su gema y cruza por el río.
        yield return Caminar(killa, -8.9f);
        yield return Salto(killa, -12f);
        yield return Caminar(killa, -9.9f);
        yield return Caminar(killa, 0f);
        yield return Caminar(killa, 9.9f);
        yield return Salto(killa, 14f);
        Log("Killa llegó a su puerta");

        // Inti cruza por las lianas.
        yield return Caminar(inti, -9.7f);
        yield return AgarrarLiana(inti, "Liana 1", 3.5f);
        Log("Inti en la liana 1");
        yield return SoltarLianaHacia(inti, "Liana 1", 1f, 1.5f);
        if (inti.control.EstaEnLiana == false) { falloRuta = "Inti no llegó a la liana 2"; yield break; }
        Log("Inti en la liana 2");
        yield return SoltarLianaHacia(inti, "Liana 2", 1f, 9.8f);
        yield return Salto(inti, 12f);
    }

    private IEnumerator RutaM4N2()
    {
        Coroutine subeInti = StartCoroutine(RebotarHacia(inti, -4f, -7.5f, 3f));
        yield return RebotarHacia(killa, 4f, 7f, 3f);
        yield return subeInti;
        Log("Subieron con las hojas");

        // Inti aprieta su botón: Killa pasa y aprieta el de Inti.
        yield return new WaitForSeconds(0.4f);
        yield return Caminar(killa, 10.5f);
        yield return new WaitForSeconds(0.4f);
        Log("Killa pasó su reja");
        yield return Caminar(inti, -10.8f);
        yield return Salto(inti, -14.3f);
        Log("Inti pasó su reja");
        yield return Salto(killa, 14.3f);

        Coroutine copaInti = StartCoroutine(RebotarHacia(inti, -12f, -8f, 9.25f));
        yield return RebotarHacia(killa, 12f, 8f, 9.25f);
        yield return copaInti;
        Log("Llegaron a la copa");
        yield return CaminarJuntos(-2f, 2f);
    }

    private IEnumerator RutaM4N3()
    {
        yield return CaminarJuntos(-9f, 9f);
        yield return CaminarJuntos(-9.9f, 9.9f);
        yield return SaltarJuntos(-13f, 13f);
        yield return new WaitForSeconds(0.3f);
        Verificar("Hubo eclipse", inti.control.Elemento == TipoElemento.Agua && killa.control.Elemento == TipoElemento.Fuego);

        yield return CaminarJuntos(-11.3f, 11.3f);
        Coroutine lianaInti = StartCoroutine(CruzarLianaM4N3(inti, "Liana izquierda", 1f, -3f));
        yield return CruzarLianaM4N3(killa, "Liana derecha", -1f, 3f);
        yield return lianaInti;
        Log("Llegaron a las ramas: Inti=" + inti.cuerpo.position.ToString("F2") + " Killa=" + killa.cuerpo.position.ToString("F2"));
        // La salida está justo encima de las ramas: se salta derecho y se entra por arriba.
        yield return CaminarJuntos(-3.2f, 3.2f);
        // Cada uno sube cuando su liana se alejó hacia afuera: si no, la liana
        // lo vuelve a agarrar en medio del salto.
        yield return EsperarLianaHaciaElCentro("Liana derecha", 1f);
        yield return SaltoRodeando(killa, 1f, 7f);
        yield return EsperarLianaHaciaElCentro("Liana izquierda", -1f);
        yield return SaltoRodeando(inti, -1f, 7f);
    }

    private IEnumerator CruzarLianaM4N3(Personaje p, string nombre, float haciaAdentro, float xRama)
    {
        yield return AgarrarLiana(p, nombre, 2.5f);
        Log(p.control.NombreJugador + " en la liana");
        float direccion = haciaAdentro;
        Log(p.control.NombreJugador + " colgado en " + p.cuerpo.position.ToString("F2"));
        // En el 4-3 la rama queda justo bajo el extremo: se suelta casi al final.
        yield return SoltarLianaHacia(p, nombre, direccion, xRama, 0.5f);
        Log(p.control.NombreJugador + " soltado en " + p.cuerpo.position.ToString("F2") + " suelo=" + p.control.EstaEnElSuelo());
        yield return Caminar(p, xRama * 1.05f);
    }

    private IEnumerator RutaM4N4()
    {
        // Suben juntos, casi sin detenerse.
        yield return CaminarJuntos(-6.2f, -6.2f);
        yield return SaltarJuntos(-3f, -2f);
        yield return CaminarJuntos(2.6f, 2.8f);
        // Saltan el botón de traición, bajo el techo.
        yield return SaltarJuntos(5f, 5f);
        yield return CaminarJuntos(6.3f, 6.4f);
        yield return Caminar(killa, 6.3f);
        Log("Pasaron el piso 1");
        // Suben al piso 2 cuando la serpiente sale de su extremo izquierdo y, sin
        // pararse en el piso que se envenena, la saltan por encima.
        yield return EsperarSerpienteYendoALaIzquierda();
        Coroutine subeInti = StartCoroutine(SaltoRodeando(inti, 2.8f, 3.75f));
        yield return SaltoRodeando(killa, 2.8f, 3.75f);
        yield return subeInti;
        // Bajo el piso 3 el salto es corto: se salta cuando la serpiente ya está
        // casi encima, para pasar por arriba de ella mientras viene.
        yield return EsperarSerpienteLlegandoAlaDerecha(1.3f);
        yield return SaltarJuntos(-0.8f, -0.8f);
        yield return CaminarJuntos(-5f, -5f);
        yield return CaminarJuntos(-6.2f, -6.2f);
        yield return SaltarJuntos(-3.5f, -3.5f);
        Log("Pasaron el piso 2");
        yield return CaminarJuntos(0f, 3.5f);
        yield return Caminar(inti, 5f);
        yield return Caminar(inti, 9f);
        yield return TocarHabilidad(inti);
        Log("Palanca usada");
        // Saltan desde más a la derecha: sobre x=5.8 hay un bloque invisible.
        Coroutine intiVuelve = StartCoroutine(Caminar(inti, 7f));
        yield return Caminar(killa, 7.1f);
        yield return intiVuelve;
        // Al piso 4 se sube desde fuera de su borde: primero hacia arriba.
        Coroutine arribaInti = StartCoroutine(SaltoRodeando(inti, 3.8f, 10.2f));
        yield return SaltoRodeando(killa, 3.5f, 10.2f);
        yield return arribaInti;
        yield return CaminarJuntos(-9.5f, -7f);
    }

    // Espera a que el mono lance un coco y a que ese coco termine de caer.
    private IEnumerator EsperarCocoCaido()
    {
        float limite = Time.time + 8f;
        while (GameObject.Find("Proyectil") == null && Time.time < limite) { yield return null; }
        while (GameObject.Find("Proyectil") != null && Time.time < limite) { yield return null; }
    }

    private IEnumerator EsperarSerpienteLlegandoAlaDerecha(float xMinimo)
    {
        Transform serpiente = GameObject.Find("Serpiente").transform;
        float limite = Time.time + 8f;
        float xAnterior = serpiente.position.x;
        yield return null;
        while (Time.time < limite)
        {
            bool vaAlaDerecha = serpiente.position.x > xAnterior;
            xAnterior = serpiente.position.x;
            if (vaAlaDerecha == true && serpiente.position.x >= xMinimo) { break; }
            yield return null;
        }
    }

    private IEnumerator EsperarSerpienteYendoALaIzquierda()
    {
        Transform serpiente = GameObject.Find("Serpiente").transform;
        float limite = Time.time + 8f;
        float xAnterior = serpiente.position.x;
        yield return null;
        while (Time.time < limite)
        {
            bool vaAlaIzquierda = serpiente.position.x < xAnterior;
            xAnterior = serpiente.position.x;
            if (vaAlaIzquierda == true && serpiente.position.x < 1f) { break; }
            yield return null;
        }
    }

    private IEnumerator EsperarSerpienteSaliendoDeLaIzquierda()
    {
        Transform serpiente = GameObject.Find("Serpiente").transform;
        float limite = Time.time + 8f;
        float xAnterior = serpiente.position.x;
        yield return null;
        while (Time.time < limite)
        {
            bool vaAlaDerecha = serpiente.position.x > xAnterior;
            xAnterior = serpiente.position.x;
            if (vaAlaDerecha == true && serpiente.position.x < -1.6f) { break; }
            yield return null;
        }
    }

    private IEnumerator EsperarSerpienteLejos(float xJugador)
    {
        Transform serpiente = GameObject.Find("Serpiente").transform;
        float limite = Time.time + 6f;
        // Se espera a que venga de vuelta hacia la izquierda, para ir detrás de ella.
        float xAnterior = serpiente.position.x;
        yield return null;
        while (Time.time < limite)
        {
            bool vaAlaIzquierda = serpiente.position.x < xAnterior;
            xAnterior = serpiente.position.x;
            if (vaAlaIzquierda == true && serpiente.position.x < 0f)
            {
                break;
            }
            yield return null;
        }
    }

    private IEnumerator RutaM4N5()
    {
        // Inti por la lava y Killa por el agua, hasta las hojas.
        Coroutine subeInti = StartCoroutine(RebotarHacia(inti, -5f, -6.4f, 3f));
        yield return RebotarHacia(killa, 5f, 6.4f, 3f);
        yield return subeInti;
        Log("Subieron con las hojas");

        // Pasan la rama saltando la serpiente y el botón de traición.
        // Saltan al enemigo sorpresa cuando las lianas están del lado del centro.
        yield return CaminarJuntos(-8.4f, 8.4f);
        yield return EsperarLianaHaciaElCentro("Liana derecha", -1f);
        yield return SaltarJuntos(-13.6f, 13.6f);
        yield return CaminarJuntos(-14f, 14f);
        yield return CaminarJuntos(-13f, 13f);
        yield return new WaitForSeconds(0.3f);
        Verificar("Hubo eclipse", inti.control.Elemento == TipoElemento.Agua && killa.control.Elemento == TipoElemento.Fuego);

        yield return CaminarJuntos(-11.2f, 11.2f);
        Coroutine lianaInti = StartCoroutine(CruzarLianaM4N5(inti, "Liana izquierda", 1f, -1.2f));
        yield return CruzarLianaM4N5(killa, "Liana derecha", -1f, 1.2f);
        yield return lianaInti;
    }

    // Espera a que la punta de la liana esté casi en su extremo del lado pedido.
    private IEnumerator EsperarLianaHaciaElCentro(string nombre, float lado)
    {
        Liana liana = GameObject.Find(nombre).GetComponent<Liana>();
        float largo = Vector2.Distance(liana.transform.position, liana.Punta);
        float limite = Time.time + 8f;
        while ((liana.Punta.x - liana.transform.position.x) * lado < largo * 0.45f && Time.time < limite) { yield return null; }
    }

    private IEnumerator CruzarLianaM4N5(Personaje p, string nombre, float direccion, float xPuerta)
    {
        yield return AgarrarLiana(p, nombre, 2.5f);
        Log(p.control.NombreJugador + " en la liana");
        yield return SoltarLianaHacia(p, nombre, direccion, xPuerta);
        Log(p.control.NombreJugador + " en la salida");
    }

    // ---------- Mundos 5 y 6, pausa e historia ----------

    private IEnumerator TodoMundo56()
    {
        GestorProgreso.BorrarProgreso();
        string modo = System.Environment.GetEnvironmentVariable("SOLO_NIVEL");
        if (modo == null) { modo = ""; }

        if (modo == "" || modo == "sistema") { yield return ProbarSistema(); Log("FALLOS EN SISTEMA: " + fallos); }
        for (int numero = 1; numero <= 5; numero++)
        {
            if (modo == "" || modo == "5" + numero) { yield return Nivel("Mundo5_Nivel" + numero, RutaMundo5(numero)); }
        }
        for (int numero = 1; numero <= 5; numero++)
        {
            if (modo == "" || modo == "6" + numero) { yield return Nivel("Mundo6_Nivel" + numero, RutaMundo6(numero)); }
        }
        if (modo == "" || modo == "65") { if (esperoFinal == true) { yield return new WaitForSeconds(5f); } Verificar("Después del jefe llega el Final", SceneManager.GetActiveScene().name == "Final" || esperoFinal == false); }
        if (modo == "fotos") { yield return FotosMundo56(); }
        if (modo == "diag")
        {
            foreach (string escena in new string[] { "Mundo6_Nivel5", "Mundo6_Nivel4" })
            {
                yield return Cargar(escena);
                yield return new WaitForSeconds(0.5f);
                teclas.Add(killa.derecha);
                teclas.Add(inti.izquierda);
                yield return new WaitForSeconds(1.5f);
                teclas.Clear();
                Log(escena + " Inti=" + inti.cuerpo.position.ToString("F2") + " Killa=" + killa.cuerpo.position.ToString("F2"));
                foreach (Collider2D choque in Physics2D.OverlapBoxAll(killa.cuerpo.position, new Vector2(2.5f, 1.6f), 0f))
                {
                    Log("  cerca de Killa: " + choque.name + " trigger=" + choque.isTrigger + " caja=" + choque.bounds.ToString());
                }
            }
        }
        if (modo == "psicologicas") { yield return ProbarPiezasPsicologicas(); Log("FALLOS EN PIEZAS PSICOLÓGICAS: " + fallos); }
        if (modo == "desafio") { yield return ProbarPiezasDesafio(); Log("FALLOS EN PIEZAS DEL DESAFÍO: " + fallos); }
        if (modo == "reinicio") { yield return ProbarReinicios(); Log("FALLOS EN REINICIO: " + fallos); }
        if (modo == "todos")
        {
            yield return TodoLoAnterior();
        }
        // Varios niveles por nombre, separados por coma: "Mundo1_Nivel1,Mundo1_Nivel2".
        if (modo.StartsWith("Mundo"))
        {
            foreach (string escena in modo.Split(','))
            {
                yield return Nivel(escena, RutaPorNombre(escena));
            }
        }
        GestorProgreso.BorrarProgreso();
        Log("FIN DE LAS PRUEBAS");
        yield return null;
#if UNITY_EDITOR
        TerminarPruebas();
#endif
    }

    private bool esperoFinal = false;

    // Espera hasta "segundos" y dice si en ese tiempo alguien murió.
    private IEnumerator EsperarMuerte(float segundos, bool[] resultado)
    {
        float limite = Time.time + segundos;
        resultado[0] = false;
        while (Time.time < limite)
        {
            if (GestorNivel.Instancia != null && (bool)Dato("reinicioEnCurso") == true) { resultado[0] = true; yield break; }
            yield return null;
        }
    }

    private IEnumerator ProbarPiezasPsicologicas()
    {
        Log("===== PIEZAS PSICOLÓGICAS =====");
        bool[] murio = new bool[1];

        // Cartel que cae (4-4): quien se para bajo las letras, muere.
        yield return Cargar("Mundo4_Nivel4");
        Teletransportar(inti, -1f, 9.5f);
        yield return EsperarMuerte(4f, murio);
        Verificar("Cartel: las letras caen sobre quien se queda debajo", murio[0]);

        // Nube asesina (1-5): quien se queda bajo la nube recibe el rayo.
        yield return Cargar("Mundo1_Nivel5");
        Teletransportar(inti, 6f, 6.25f);
        yield return EsperarMuerte(3f, murio);
        Verificar("Nube: el rayo alcanza a quien se queda debajo", murio[0]);

        // Gema imán (2-1): quien la toma y se queda quieto, recibe las piedras.
        yield return Cargar("Mundo2_Nivel1");
        // La gema está en el aire: se toma con un salto y salen las piedras hacia ese punto.
        Teletransportar(inti, 5.5f, 5.65f);
        bool salieronPiedras = false;
        float limitePiedras = Time.time + 1f;
        while (Time.time < limitePiedras && salieronPiedras == false) { salieronPiedras = GameObject.Find("Piedra atraida") != null; yield return null; }
        Verificar("Gema imán: al tomarla salen las piedras hacia ese lugar", salieronPiedras);

        // Arbusto trampa (4-1): quien se acerca y se queda, es mordido.
        yield return Cargar("Mundo4_Nivel1");
        Teletransportar(inti, -13.3f, -0.25f);
        yield return EsperarMuerte(3f, murio);
        Verificar("Arbusto: muerde a quien se queda al lado", murio[0]);

        // Trampolín catapulta (4-2): lanza contra las púas del techo.
        yield return Cargar("Mundo4_Nivel2");
        yield return Caminar(inti, 0f);
        yield return EsperarMuerte(3f, murio);
        Verificar("Catapulta: la hoja del centro lanza contra las púas", murio[0]);

        // Arena movediza (5-3): quien se queda adentro, se hunde.
        yield return Cargar("Mundo5_Nivel3");
        Teletransportar(killa, 12.5f, -3.5f);
        yield return EsperarMuerte(4f, murio);
        Verificar("Arena movediza: se traga a quien se queda", murio[0]);

        // Poción de peso (5-4): el piso se rompe y se cae, sin morir.
        yield return Cargar("Mundo5_Nivel4");
        Teletransportar(inti, -12.25f, 1.5f);
        yield return new WaitForSeconds(0.4f);
        yield return Caminar(inti, -11.5f);
        yield return new WaitForSeconds(1.2f);
        Verificar("Poción de peso: el piso se rompe y cae al suelo", inti.cuerpo.position.y < 0f && Vivo(inti));

        // Púas de goma (6-3): rebotan hacia arriba sin matar.
        yield return Cargar("Mundo6_Nivel3");
        Teletransportar(inti, 13.5f, 1.5f);
        float alturaMaxima = -10f;
        float limiteGoma = Time.time + 1.5f;
        while (Time.time < limiteGoma) { alturaMaxima = Mathf.Max(alturaMaxima, inti.cuerpo.position.y); yield return null; }
        Verificar("Púas de goma: rebotan alto y no matan", alturaMaxima > 3f && Vivo(inti));

        // Vidas (6-5): las dos primeras muertes repiten el nivel; con la tercera
        // se vuelve al comienzo del mundo (6-1).
        yield return Cargar("Mundo6_Nivel5");
        for (int muerte = 1; muerte <= 3; muerte++)
        {
            yield return new WaitForSeconds(0.5f);
            GameObject.Find("Inti").GetComponent<ControladorJugador>().Morir("Prueba de vidas");
            yield return new WaitForSeconds(2f);
            if (muerte < 3)
            {
                Verificar("Vidas: la muerte " + muerte + " en el final repite el 6-5", SceneManager.GetActiveScene().name == "Mundo6_Nivel5");
            }
        }
        Verificar("Vidas: al perder las 3 en el final se vuelve al 6-1", SceneManager.GetActiveScene().name == "Mundo6_Nivel1");

        // Vidas (1-2): al perder las 3 se vuelve al nivel 1 del mundo.
        yield return Cargar("Mundo1_Nivel2");
        for (int muerte = 1; muerte <= 3; muerte++)
        {
            yield return new WaitForSeconds(0.5f);
            GameObject.Find("Inti").GetComponent<ControladorJugador>().Morir("Prueba de vidas");
            yield return new WaitForSeconds(2f);
        }
        Verificar("Vidas: al perder las 3 en el 1-2 se vuelve al 1-1", SceneManager.GetActiveScene().name == "Mundo1_Nivel1");

        // Sala que se repite (5-1): la misma escena vuelve a empezar en la vuelta 2.
        yield return Cargar("Mundo5_Nivel1");
        Verificar("Bucle: empieza en la vuelta 1", SalaQueSeRepite.VueltaActual == 0);
    }

    private IEnumerator ProbarPiezasDesafio()
    {
        Log("===== PIEZAS DEL DESAFÍO =====");
        // Suelo que desaparece al saltar (1-2): caminar encima no hace nada; saltar sí.
        yield return Cargar("Mundo1_Nivel2");
        Collider2D pisoSalida = GameObject.Find("Piso de salida").GetComponent<Collider2D>();
        Teletransportar(inti, -2.5f, 9.5f);
        yield return new WaitForSeconds(0.6f);
        yield return Caminar(inti, -0.5f);
        Verificar("Caminar sobre el suelo agrietado no lo hace desaparecer", pisoSalida.enabled == true && inti.cuerpo.position.y > 9.5f);
        teclas.Add(inti.salto);
        yield return new WaitForSeconds(0.15f);
        teclas.Remove(inti.salto);
        yield return new WaitForSeconds(1.2f);
        Verificar("Saltar encima lo hace desaparecer y se cae", inti.cuerpo.position.y < 9f && Vivo(inti));
        yield return new WaitForSeconds(2.5f);
        Verificar("El suelo vuelve a aparecer", pisoSalida.enabled == true);

        // Sensor y bloque que cierra el regreso (1-1).
        yield return Cargar("Mundo1_Nivel1");
        Collider2D bloque = GameObject.Find("Bloque del regreso").GetComponent<Collider2D>();
        Verificar("El bloque empieza escondido", bloque.enabled == false);
        Teletransportar(killa, -12.3f, 6.25f);
        yield return new WaitForSeconds(0.4f);
        Verificar("Sin pasar por el sensor no sube", bloque.enabled == false);
        yield return Caminar(killa, -10.9f);
        yield return new WaitForSeconds(1f);
        Verificar("Al pasar por el sensor el bloque sube y cierra el regreso", bloque.enabled == true && Vivo(killa));
        // Bloque invisible (2-1): aparece al saltar debajo y se esconde a los 3 s.
        yield return Cargar("Mundo2_Nivel1");
        Teletransportar(inti, 10.2f, 1.7f);
        yield return new WaitForSeconds(0.5f);
        teclas.Add(inti.salto);
        yield return new WaitForSeconds(0.1f);
        teclas.Remove(inti.salto);
        yield return new WaitForSeconds(0.4f);
        Verificar("El bloque invisible aparece al saltar debajo", ColliderActivo("Bloque escondido"));
        yield return new WaitForSeconds(3f);
        Verificar("El bloque invisible se vuelve a esconder (nadie queda atascado)", ColliderActivo("Bloque escondido") == false);
    }

    private IEnumerator RutaPorNombre(string escena)
    {
        string[] partes = escena.Replace("Mundo", "").Replace("Nivel", "").Split('_');
        int mundo = int.Parse(partes[0]);
        int numero = int.Parse(partes[1]);
        if (mundo == 1)
        {
            IEnumerator[] rutas = new IEnumerator[] { RutaN1(), RutaN2(), RutaN3(), RutaN4(), RutaN5() };
            return rutas[numero - 1];
        }
        if (mundo == 2)
        {
            IEnumerator[] rutas = new IEnumerator[] { RutaM2N1(), RutaM2N2(), RutaM2N3(), RutaM2N4(), RutaM2N5() };
            return rutas[numero - 1];
        }
        if (mundo == 3)
        {
            IEnumerator[] rutas = new IEnumerator[] { RutaM3N1(), RutaM3N2(), RutaM3N3(), RutaM3N4(), RutaM3N5() };
            return rutas[numero - 1];
        }
        if (mundo == 4)
        {
            IEnumerator[] rutas = new IEnumerator[] { RutaM4N1(), RutaM4N2(), RutaM4N3(), RutaM4N4(), RutaM4N5() };
            return rutas[numero - 1];
        }
        if (mundo == 5) { return RutaMundo5(numero); }
        if (numero == 5) { esperoFinal = true; }
        return RutaMundo6(numero);
    }

    private IEnumerator TodoLoAnterior()
    {
        yield return Nivel("Mundo1_Nivel1", RutaN1());
        yield return Nivel("Mundo1_Nivel2", RutaN2());
        yield return Nivel("Mundo1_Nivel3", RutaN3());
        yield return Nivel("Mundo1_Nivel4", RutaN4());
        yield return Nivel("Mundo1_Nivel5", RutaN5());
        yield return Nivel("Mundo2_Nivel1", RutaM2N1());
        yield return Nivel("Mundo2_Nivel2", RutaM2N2());
        yield return Nivel("Mundo2_Nivel3", RutaM2N3());
        yield return Nivel("Mundo2_Nivel4", RutaM2N4());
        yield return Nivel("Mundo2_Nivel5", RutaM2N5());
        yield return Nivel("Mundo3_Nivel1", RutaM3N1());
        yield return Nivel("Mundo3_Nivel2", RutaM3N2());
        yield return Nivel("Mundo3_Nivel3", RutaM3N3());
        yield return Nivel("Mundo3_Nivel4", RutaM3N4());
        yield return Nivel("Mundo3_Nivel5", RutaM3N5());
        yield return Nivel("Mundo4_Nivel1", RutaM4N1());
        yield return Nivel("Mundo4_Nivel2", RutaM4N2());
        yield return Nivel("Mundo4_Nivel3", RutaM4N3());
        yield return Nivel("Mundo4_Nivel4", RutaM4N4());
        yield return Nivel("Mundo4_Nivel5", RutaM4N5());
    }

    private IEnumerator FotosMundo56()
    {
        string[] escenas = new string[] { "Mundo5_Nivel1", "Mundo5_Nivel2", "Mundo5_Nivel3", "Mundo5_Nivel4", "Mundo5_Nivel5", "Mundo6_Nivel1", "Mundo6_Nivel2", "Mundo6_Nivel3", "Mundo6_Nivel4", "Mundo6_Nivel5" };
        foreach (string escena in escenas)
        {
            yield return Cargar(escena);
            yield return new WaitForSeconds(1f);
            SacarFoto(escena);
        }
        SceneManager.LoadScene("Intro");
        yield return new WaitForSeconds(1.2f);
        SacarFoto("Intro");
    }

    private IEnumerator Tecla(Key tecla)
    {
        teclas.Add(tecla);
        yield return null;
        yield return null;
        teclas.Remove(tecla);
        yield return null;
        yield return null;
    }

    private IEnumerator ProbarSistema()
    {
        Log("===== SISTEMA =====");
        yield return Cargar("Mundo1_Nivel1");
        yield return new WaitForSeconds(0.3f);
        yield return Tecla(Key.Escape);
        Verificar("Esc pausa el juego", MenuPausa.EstaPausado && Time.timeScale == 0f);
        yield return Tecla(Key.Escape);
        Verificar("Esc otra vez continúa", MenuPausa.EstaPausado == false && Time.timeScale == 1f);
        yield return Tecla(Key.H);
        Verificar("H muestra la pista", ((string)Dato("avisoTemporal")).Length > 10);

        SceneManager.LoadScene("Intro");
        yield return new WaitForSeconds(0.5f);
        for (int indice = 0; indice < 12 && SceneManager.GetActiveScene().name == "Intro"; indice++)
        {
            yield return Tecla(Key.Enter);
            yield return new WaitForSeconds(0.1f);
        }
        Verificar("La Intro lleva a elegir mundo", SceneManager.GetActiveScene().name == "SeleccionMundo");

        // Cuerdas y melodía
        yield return Cargar("Mundo5_Nivel2");
        yield return new WaitForSeconds(0.2f);
        PuertaMelodica puerta = FindFirstObjectByType<PuertaMelodica>();
        yield return Salto(inti, -11.5f);
        yield return RebotarHacia(inti, -10f, -11.5f, suelo);
        Verificar("La cuerda 1 suma una nota", puerta.NotasAcertadas == 1);
        yield return Salto(inti, -8.5f);
        yield return RebotarHacia(inti, -7f, -8.5f, suelo);
        Verificar("Una nota equivocada borra la melodía", puerta.NotasAcertadas == 0);

        // Eclipse automático
        yield return Cargar("Mundo6_Nivel1");
        CicloEclipse ciclo = FindFirstObjectByType<CicloEclipse>();
        TipoElemento antes = inti.control.Elemento;
        yield return new WaitForSeconds(ciclo.SegundosParaElEclipse + 0.3f);
        Verificar("El eclipse automático cambia los poderes", inti.control.Elemento != antes && Vivo(inti));
    }

    // Toca una cuerda: camina hacia ella y, ya en el aire, vuelve a xDespues.
    private IEnumerator TocarCuerda(Personaje p, float xCuerda, float xDespues)
    {
        yield return RebotarHacia(p, xCuerda, xDespues, suelo);
    }

    private IEnumerator RutaMundo5(int numero)
    {
        if (numero == 1) { yield return RutaM5N1(); }
        if (numero == 2) { yield return RutaM5N2(); }
        if (numero == 3) { yield return RutaM5N3(); }
        if (numero == 4) { yield return RutaM5N4(); }
        if (numero == 5) { yield return RutaM5N5(); }
    }

    private IEnumerator RutaMundo6(int numero)
    {
        if (numero == 1) { yield return RutaM6N1(); }
        if (numero == 2) { yield return RutaM6N2(); }
        if (numero == 3) { yield return RutaM6N3(); }
        if (numero == 4) { yield return RutaM6N4(); }
        if (numero == 5) { esperoFinal = true; yield return RutaM6N5(); }
    }

    private IEnumerator RutaM5N1()
    {
        // Los peces del charco cruzan el camino a las cuerdas: se espera cada salto.
        yield return Caminar(inti, -4.2f);
        yield return EsperarCriatura("Pez de la lava");
        // En la loma se aterriza lejos del arbusto que muerde (junto a la pared).
        yield return RebotarHacia(inti, -10f, -12.5f, 3f);
        yield return Caminar(killa, 4.2f);
        yield return EsperarCriatura("Pez del agua");
        yield return RebotarHacia(killa, 10f, 12.5f, 3f);
        Log("Llegaron a las lomas");
        yield return EsperarCriatura("Pez de la lava");
        yield return Salto(inti, -6.8f);
        yield return EsperarCriatura("Pez del agua");
        yield return Salto(killa, 6.8f);
        Coroutine cimaInti = StartCoroutine(RebotarHacia(inti, -5.3f, -4f, 9.5f));
        yield return RebotarHacia(killa, 5.3f, 4f, 9.5f);
        yield return cimaInti;
        Log("Llegaron a la cima");
        yield return CaminarJuntos(-2.5f, 2.5f);
    }

    private IEnumerator RutaM5N2()
    {
        // Melodía: naranja (1), celeste (4), amarillo (2). Se saltan las demás cuerdas.
        yield return Salto(inti, -11.5f);
        yield return TocarCuerda(inti, -10f, -11.5f);
        yield return Salto(inti, -8.5f);
        yield return Salto(inti, -5.5f);
        yield return Salto(inti, -2.5f);
        yield return TocarCuerda(inti, -1f, -2.5f);
        yield return Salto(inti, -5.5f);
        yield return TocarCuerda(inti, -7f, -5.5f);
        Verificar("La reja se abrió", FindFirstObjectByType<PuertaMelodica>().EstaAbierta);

        yield return Salto(killa, -11.5f);
        yield return Salto(killa, -8.5f);
        yield return Salto(killa, -5.5f);
        yield return Salto(inti, -2.5f);
        yield return Salto(killa, -2.5f);
        yield return Salto(inti, 0.5f);
        yield return Salto(killa, 0.5f);
        yield return EsperarPiedraQuieta("Piedra");
        // Las púas del piso se saltan desde antes.
        yield return CaminarJuntos(2.9f, 2.9f);
        yield return Salto(inti, 6f);
        // Espera que las púas que hizo salir Inti se vuelvan a esconder.
        yield return new WaitForSeconds(1.7f);
        yield return Salto(killa, 6.4f);
        // Desde más atrás, para pasar por encima del bloque invisible; al llegar
        // sale el enemigo del suelo y se lo salta enseguida.
        yield return Caminar(inti, 6f);
        yield return Salto(inti, 9.6f);
        yield return Caminar(inti, 12f);
        yield return Caminar(killa, 6f);
        yield return Salto(killa, 9.6f);
        yield return Caminar(killa, 12f);
        yield return CaminarJuntos(12f, 14f);
    }

    private IEnumerator RutaM5N3()
    {
        // Inti se pone a la izquierda de sus cuerdas sin tocarlas; Killa a la derecha del muro.
        yield return Salto(inti, -7.5f);
        yield return Salto(inti, -10.5f);
        yield return Salto(inti, -13.5f);
        yield return Caminar(killa, 3.6f);
        // 0 (Inti), 3 (Killa), 1 (Inti), 4 (Killa), 2 (Inti)
        yield return TocarCuerda(inti, -12f, -13.5f);
        yield return TocarCuerda(killa, 5.3f, 3.6f);
        yield return Salto(inti, -10.5f);
        yield return TocarCuerda(inti, -9f, -10.5f);
        yield return Salto(killa, 7.5f);
        yield return TocarCuerda(killa, 9f, 7.5f);
        yield return Salto(inti, -7.5f);
        yield return RebotarHacia(inti, -5.3f, -2.6f, 3f);
        Verificar("Las barras se abrieron", FindFirstObjectByType<PuertaMelodica>().EstaAbierta);
        yield return Caminar(inti, -1.5f);
        yield return RebotarHacia(killa, 5.3f, 2.6f, 3f);
        yield return Caminar(killa, 1.5f);
    }

    private IEnumerator RutaM5N4()
    {
        yield return Salto(inti, -11.5f);
        yield return Salto(inti, -8.5f);
        // Amarillo (2) de día, verde (3) de noche, rojo (0) y naranja (1) de noche.
        yield return EsperarFase(true);
        yield return TocarCuerda(inti, -7f, -8.5f);
        yield return EsperarCambioA(false);
        yield return Salto(inti, -5.5f);
        yield return TocarCuerda(inti, -4f, -5.5f);
        yield return Salto(inti, -8.5f);
        yield return Salto(inti, -11.5f);
        yield return TocarCuerda(inti, -13f, -11.5f);
        yield return EsperarFase(false);
        yield return TocarCuerda(inti, -10f, -11.5f);
        Verificar("La reja se abrió", FindFirstObjectByType<PuertaMelodica>().EstaAbierta);

        // Cruzan saltando las cuerdas y el botón de traición.
        yield return Salto(killa, -11.5f);
        yield return Salto(inti, -8.5f);
        yield return Salto(killa, -8.5f);
        yield return Salto(inti, -5.5f);
        yield return Salto(killa, -5.5f);
        yield return Salto(inti, -2.5f);
        yield return Salto(killa, -2.5f);
        // Saltan por encima de las púas escondidas del piso.
        yield return Salto(inti, 2f);
        yield return Salto(killa, 2f);
        yield return CaminarJuntos(3f, 2.8f);
        yield return Salto(inti, 6.5f);
        yield return TocarHabilidad(inti);
        Log("Palanca usada; Killa escapa");
        // Killa sale de la marca roja saltando las púas escondidas.
        yield return Salto(killa, -1.2f);
        yield return Caminar(inti, 7.3f);
        yield return Salto(inti, 9.6f);
        yield return Caminar(inti, 12f);
        yield return new WaitForSeconds(1.5f);
        yield return Salto(killa, 2.9f);
        yield return Salto(killa, 6.5f);
        yield return Caminar(killa, 7.3f);
        yield return Salto(killa, 9.8f);
        yield return Caminar(killa, 14.5f);
        yield return Caminar(killa, 13.5f);
    }

    private IEnumerator RutaM5N5()
    {
        // Saltan sus botones de traición y se ubican junto a sus cuerdas.
        // Aterrizan entre el botón de traición y la cuerda (las cuerdas hacen rebotar).
        yield return Salto(inti, -3.7f);
        yield return Salto(killa, 3.7f);
        yield return Salto(inti, -7.5f);
        yield return Salto(inti, -10.5f);
        // Junto a la pared, lejos de las púas escondidas de la orilla.
        yield return Salto(inti, -14.3f);
        // 0, 3 (día), 1 (noche), 4, 2, 3 (día)
        yield return TocarCuerda(inti, -12f, -14.3f);
        yield return EsperarFase(true);
        yield return TocarCuerda(killa, 5.3f, 3.7f);
        yield return Salto(inti, -10.5f);
        yield return EsperarCambioA(false);
        yield return TocarCuerda(inti, -9f, -10.5f);
        yield return Salto(killa, 7.5f);
        yield return TocarCuerda(killa, 9f, 7.5f);
        yield return Salto(inti, -7.5f);
        yield return TocarCuerda(inti, -5.3f, -7.5f);
        yield return EsperarCambioA(true);
        yield return RebotarHacia(killa, 5.3f, 2.6f, 3f);
        Verificar("Las barras se abrieron", FindFirstObjectByType<PuertaMelodica>().EstaAbierta);
        yield return Caminar(killa, 1.5f);
        yield return RebotarHacia(inti, -5.3f, -2.6f, 3f);
        yield return Caminar(inti, -1.5f);
    }

    // Espera a tener ese poder con tiempo de sobra antes del próximo eclipse.
    private IEnumerator EsperarPoder(Personaje p, TipoElemento elemento, float margen)
    {
        CicloEclipse ciclo = FindFirstObjectByType<CicloEclipse>();
        float limite = Time.time + 25f;
        while (p.control.Elemento != elemento || ciclo.SegundosParaElEclipse < margen)
        {
            if (Time.time > limite) { falloRuta = "no llegó el poder esperado"; yield break; }
            yield return null;
        }
    }

    // Espera a que falte poco para el eclipse: así se cae con el poder ya cambiado.
    private IEnumerator EsperarCuentaFinal(float segundosAntes)
    {
        CicloEclipse ciclo = FindFirstObjectByType<CicloEclipse>();
        float limite = Time.time + 12f;
        while (ciclo.SegundosParaElEclipse > segundosAntes && Time.time < limite)
        {
            yield return null;
        }
    }

    // Cada uno va a la puerta de su poder actual, hasta ganar.
    private IEnumerator IrALasPuertasPorPoder(float xFuego, float xAgua)
    {
        yield return IrALasPuertasActuales();
        yield break;
        float limite = Time.time + 25f;
        while ((bool)Dato("nivelTerminado") == false && Time.time < limite)
        {
            float destinoInti = xAgua;
            if (inti.control.Elemento == TipoElemento.Fuego) { destinoInti = xFuego; }
            float destinoKilla = xAgua;
            if (killa.control.Elemento == TipoElemento.Fuego) { destinoKilla = xFuego; }
            yield return CaminarJuntos(destinoInti, destinoKilla);
            yield return new WaitForSeconds(0.2f);
        }
    }

    private IEnumerator CruzarConPoder(Personaje p, TipoElemento elemento, float xDestino)
    {
        yield return EsperarPoder(p, elemento, 2.2f);
        yield return Caminar(p, xDestino);
    }

    private IEnumerator RutaM6N1()
    {
        yield return Salto(inti, -12f);
        yield return Salto(killa, -12f);
        // Piso 1: lava y después agua.
        Coroutine intiPiso1 = StartCoroutine(CruzarPiso1M6(inti));
        yield return CruzarPiso1M6(killa);
        yield return intiPiso1;
        Log("Pasaron el piso 1");
        // Al piso 2 se sube desde fuera de su borde: primero hacia arriba.
        yield return CaminarJuntos(13.9f, 13.9f);
        Coroutine piso2Inti = StartCoroutine(SaltoRodeando(inti, 11.3f, 3.7f));
        yield return SaltoRodeando(killa, 11.6f, 3.7f);
        yield return piso2Inti;
        Coroutine intiPiso2 = StartCoroutine(CruzarPiso2M6(inti));
        yield return CruzarPiso2M6(killa);
        yield return intiPiso2;
        Log("Pasaron el piso 2");
        yield return CaminarJuntos(-13.7f, -13.7f);
        Coroutine salidaInti = StartCoroutine(SaltoRodeando(inti, -11.5f, 6.96f));
        yield return SaltoRodeando(killa, -9f, 6.96f);
        yield return salidaInti;
        yield return IrALasPuertasPorPoder(-11.5f, -9f);
    }

    private IEnumerator CruzarPiso1M6(Personaje p)
    {
        yield return CruzarConPoder(p, TipoElemento.Fuego, 1.5f);
        // El enemigo sorpresa del final del piso: se salta apenas empieza a salir.
        // El enemigo sorpresa del final del piso: se lo hace salir y se vuelve al
        // agua (con el poder del agua no daña) hasta que desaparece.
        yield return CruzarConPoder(p, TipoElemento.Agua, 10.2f);
        yield return Caminar(p, 8.4f);
        yield return new WaitForSeconds(1.1f);
        yield return Caminar(p, 12f);
    }

    private IEnumerator CruzarPiso2M6(Personaje p)
    {
        yield return CruzarConPoder(p, TipoElemento.Agua, 1.5f);
        yield return CruzarConPoder(p, TipoElemento.Fuego, -10f);
    }

    private IEnumerator RutaM6N2()
    {
        yield return CaminarJuntos(-7.4f, 7.4f);
        // Al primer escalón se sube desde fuera de su borde: primero hacia arriba.
        Coroutine subeInti = StartCoroutine(SaltoRodeando(inti, -10f, 0.5f));
        yield return SaltoRodeando(killa, 10f, 0.5f);
        yield return subeInti;
        for (int escalon = 0; escalon < 2; escalon++)
        {
            float x = -6f;
            if (escalon == 1) { x = -10f; }
            if (escalon == 1) { yield return CaminarJuntos(-7.3f, 7.3f); }
            yield return EsperarCuentaFinal(0.45f);
            yield return SaltarJuntos(x, -x);
            Log("Escalón " + (escalon + 2) + ": Inti=" + inti.control.Elemento + " vivo=" + Vivo(inti));
        }
        yield return CaminarJuntos(-8.7f, 8.7f);
        yield return SaltarJuntos(-3f, 3f);
        yield return IrALasPuertasPorPoder(-4f, 4f);
    }

    private IEnumerator RutaM6N3()
    {
        // Saltan desde antes de las púas escondidas del borde.
        yield return CaminarJuntos(-10.1f, -10.2f);
        float[] cristales = new float[] { -6f, -2f, 2f, 6f };
        foreach (float x in cristales)
        {
            yield return SaltarJuntos(x, x);
        }
        yield return SaltarJuntos(10.8f, 10.8f);
        // Suben por fuera de los bordes y saltan la lava y el agua: así no dependen
        // del eclipse. Los pisos que se envenenan se pisan solo un instante.
        yield return CaminarJuntos(10.6f, 10.6f);
        Coroutine piso2Inti = StartCoroutine(SaltoRodeando(inti, 7.5f, 3.7f));
        yield return SaltoRodeando(killa, 7.5f, 3.7f);
        yield return piso2Inti;
        yield return CaminarJuntos(5.6f, 5.6f);
        yield return SaltarJuntos(2f, 2f);
        yield return CaminarJuntos(1.5f, 1.5f);
        Coroutine piso3Inti = StartCoroutine(SaltoRodeando(inti, -1.2f, 6.96f));
        yield return SaltoRodeando(killa, -1.2f, 6.96f);
        yield return piso3Inti;
        yield return CaminarJuntos(-2.6f, -2.6f);
        yield return SaltarJuntos(-5.9f, -5.9f);
        yield return CaminarJuntos(-6.8f, -6.8f);
        yield return SaltarJuntos(-9f, -9f);
        yield return SaltarJuntos(-12.5f, -12.5f);
        yield return IrALasPuertasPorPoder(-14.5f, -12.5f);
    }

    private IEnumerator RutaM6N4()
    {
        // Rebotan cuando las lianas están hacia afuera: al saltar después, ya
        // volvieron hacia el centro y no los agarran en el aire.
        yield return EsperarLianaHaciaElCentro("Liana derecha", 1f);
        Coroutine subeInti = StartCoroutine(RebotarHacia(inti, -5f, -8f, 3f));
        yield return RebotarHacia(killa, 5f, 8f, 3f);
        // Aterrizan junto al piso que se calienta y al enemigo escondido: no hay
        // dónde quedarse, así que saltan enseguida por encima del enemigo.
        yield return SaltarJuntos(-12.6f, 12.6f);
        yield return subeInti;
        Log("Subieron con las hojas");
        // El enemigo sorpresa de cada rama se salta apenas empieza a salir.
        yield return CaminarJuntos(-14.3f, 14.3f);
        yield return Caminar(inti, -13f);
        yield return TocarHabilidad(inti);
        Log("Palanca usada");
        yield return Caminar(killa, 11.4f);
        yield return Caminar(inti, -11.4f);
        Coroutine lianaInti = StartCoroutine(CruzarLianaM4N5(inti, "Liana izquierda", 1f, -1.1f));
        yield return CruzarLianaM4N5(killa, "Liana derecha", -1f, 1.1f);
        yield return lianaInti;
        yield return IrALasPuertasPorPoder(1.1f, -1.1f);
    }

    private IEnumerator RutaM6N5()
    {
        GuardianEclipse guardian = FindFirstObjectByType<GuardianEclipse>();
        yield return CaminarJuntos(-5f, 5f);
        for (int golpe = 0; golpe < 3; golpe++)
        {
            // Cada bola cae donde estaba el jugador al lanzarla. Se sale justo después
            // de un disparo y se avanza siempre en la misma dirección: las bolas caen
            // detrás. En la pared se espera el siguiente disparo y se vuelve enseguida.
            yield return EsperarDisparoDelGuardian(guardian);
            yield return CaminarJuntos(-6.6f, 6.6f);
            // El tramo que se envenena se salta.
            yield return SaltarJuntos(-10f, 10f);
            yield return CaminarJuntos(-12.5f, 12.5f);
            Log("En los botones t=" + Time.time.ToString("F2") + " desdeDisparo=" + guardian.SegundosDesdeDisparo.ToString("F2"));
            yield return CaminarJuntos(-14.2f, 14.2f);
            yield return EsperarDisparoDelGuardian(guardian);
            yield return CaminarJuntos(-10f, 10f);
            yield return SaltarJuntos(-6.6f, 6.6f);
            yield return CaminarJuntos(-5f, 5f);
            Log("Vidas del Guardián: " + guardian.Vidas);
        }

        // El escalón sube junto al altar; desde él se salta a las puertas.
        Transform escalon = GameObject.Find("Escalon del altar").transform;
        float limite = Time.time + 8f;
        while (escalon.position.y < -0.6f && Time.time < limite) { yield return null; }
        // El escalón quedó a la altura del piso 1: se sube desde el suelo, por fuera de su borde.
        yield return CaminarJuntos(-4.4f, -4.4f);
        Coroutine escalonInti = StartCoroutine(SaltoRodeando(inti, -6.6f, 0.5f));
        yield return SaltoRodeando(killa, -6.4f, 0.5f);
        yield return escalonInti;
        // Desde el borde del escalón, y moviéndose apenas los pies pasan el altar.
        yield return CaminarJuntos(-5.25f, -5.2f);
        Coroutine altarInti = StartCoroutine(SaltoRodeando(inti, -2f, 3.2f));
        yield return SaltoRodeando(killa, -1.2f, 3.2f);
        yield return altarInti;
        Log("En el altar: Inti=" + inti.cuerpo.position.ToString("F2") + " Killa=" + killa.cuerpo.position.ToString("F2") + " fuego=" + GameObject.Find("Puerta fuego").transform.position.ToString("F2") + " agua=" + GameObject.Find("Puerta agua").transform.position.ToString("F2"));
        yield return IrALasPuertasActuales();
        Log("Después: Inti=" + inti.cuerpo.position.ToString("F2") + " " + inti.control.Elemento + " Killa=" + killa.cuerpo.position.ToString("F2") + " " + killa.control.Elemento);
        yield return new WaitForSeconds(5f);
    }

    // Va a la puerta de su poder según dónde estén las puertas AHORA (pueden
    // cambiarse de lugar cuando alguien se acerca).
    private IEnumerator EsperarDisparoDelGuardian(GuardianEclipse guardian)
    {
        float limite = Time.time + 6f;
        while (guardian.SegundosDesdeDisparo < 0.5f && Time.time < limite) { yield return null; }
        while (guardian.SegundosDesdeDisparo >= 0.5f && Time.time < limite) { yield return null; }
    }

    private IEnumerator IrALasPuertasActuales()
    {
        float limite = Time.time + 25f;
        while ((bool)Dato("nivelTerminado") == false && Time.time < limite)
        {
            float xFuego = GameObject.Find("Puerta fuego").transform.position.x;
            float xAgua = GameObject.Find("Puerta agua").transform.position.x;
            float destinoInti = xAgua;
            if (inti.control.Elemento == TipoElemento.Fuego) { destinoInti = xFuego; }
            float destinoKilla = xAgua;
            if (killa.control.Elemento == TipoElemento.Fuego) { destinoKilla = xFuego; }
            yield return CaminarJuntos(destinoInti, destinoKilla);
            yield return new WaitForSeconds(0.3f);
        }
    }

    // Foto de la pantalla en la carpeta de capturas (solo para revisar a ojo).
    private void SacarFoto(string nombre)
    {
        string carpeta = System.Environment.GetEnvironmentVariable("CAPTURA_SALIDA");
        if (carpeta == null || carpeta == "") { return; }
        ScreenCapture.CaptureScreenshot(System.IO.Path.Combine(carpeta, nombre + ".png"));
    }
}

// Componente vacío donde corren las rutas del bot (ver BotPruebas.ejecutor).
public class EjecutorDeRutasBot : MonoBehaviour
{
}

#endif
