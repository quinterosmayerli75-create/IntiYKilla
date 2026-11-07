# GDD – Inti y Killa

## 1. Ficha general
- **Género:** plataformas y puzzles cooperativos 2D
- **Jugadores:** 2, local, mismo teclado
- **Motor:** Unity 6 LTS, plantilla Universal 2D (URP), C#
- **Plataforma:** PC (Windows) y WebGL
- **Publicación:** itch.io (gratis)
- **Duración:** 6 mundos con 5 niveles cada uno (30 niveles) de 1 a 3 minutos
- **Plazo:** 28 oct – 15 dic 2026 (proyecto final individual)
- **Referencia:** juegos cooperativos de elementos tipo "agua y fuego" (solo mecánica; personajes, arte e historia son originales)

## 2. Concepto y plus
Inti (Sol) y Killa (Luna) cooperan para recorrer Bolivia y llegar juntos a la salida. Cada uno sobrevive en lo que destruye al otro.

**Identidad: juego engañoso.** Plataformas cooperativo tipo "troll game", con diseño propio: **"no confíes en nada… ni en tu compañero"**. Todos los niveles tienen trampas sorpresa, suaves en el Mundo 1 y cada vez más tramposas. Morir es parte de la gracia: el reinicio es casi instantáneo y cada muerte tiene un mensaje gracioso.

Plus diferenciadores:
- **Trampas engañosas y de traición:** se ven igual que las piezas normales; algunas castigan al compañero.
- **Eclipse:** altares donde, si los dos se paran a la vez, intercambian elementos.
- **Se atraviesan:** Inti y Killa no chocan entre sí: ninguno empuja al otro ni puede pararse sobre su cabeza.
- **Aura de elemento:** cada personaje lleva un resplandor con chispas del color de su poder actual; al eclipsar, cambia de color y crece un momento.
- **Efecto de eclipse:** al intercambiar poderes la pantalla se oscurece y la luna pasa por delante del sol (2,4 s, sin detener el juego).
- **Escenarios reales de Bolivia:** 6 mundos.
- **Mundo musical de Aiquile:** cuerdas de charango trampolín y puertas melódicas.
- **Día y noche:** plataformas que aparecen o desaparecen.
- **Estrellas por pocas muertes:** 1 a 3 estrellas por nivel.
- **Niveles finales muy difíciles** (precisión y coordinación).

## 3. Personajes
| Personaje | Jugador | Pisa | Muere con | Habilidad (S / Flecha abajo) |
|---|---|---|---|---|
| Inti (Sol) | 1 | Fuego, lava | Agua | Ilumina zonas oscuras, derrite hielo |
| Killa (Luna) | 2 | Agua, hielo | Fuego | Congela agua (plataforma temporal) |

- Ambos mueren con veneno verde y al caer al vacío.
- Si uno muere, el nivel se reinicia para los dos.
- Historia: un eclipse rompió el equilibrio entre día y noche; deben reunir las gemas de luz desde Uyuni hasta la Isla del Sol.

## 4. Controles
| Acción | Inti | Killa |
|---|---|---|
| Moverse | A / D | ← / → |
| Saltar | W | ↑ |
| Habilidad / interactuar | S | ↓ |
| Pausa | Esc | Esc |

## 5. Mecánicas (componentes reutilizables)
| Mecánica | Desde (mundo-nivel) |
|---|---|
| Movimiento y salto (coyote time) | 1-1 |
| Charcos: lava (solo Inti), agua (solo Killa), veneno (ninguno) | 1-1 |
| Gemas naranjas (Inti) y azules (Killa) | 1-1 |
| Puertas de salida; ganan cuando ambos están en la suya | 1-1 |
| Botones y palancas (algunos requieren a los dos) | 1-2 |
| Altar de eclipse (intercambio de elementos) | 1-3 |
| Oscuridad con luz 2D; Inti ilumina | 2-1 |
| Vagonetas y plataformas móviles | 2-2 |
| Cajas empujables entre los dos | 2-3 |
| Niebla: cada jugador solo ve cerca de sí mismo | 3-1 |
| Lluvia que apaga a quien tiene el fuego si no tiene techo | 3-2 |
| Charcos que Killa congela e Inti derrite (5 s, parpadean antes) | 3-2 |
| Día y noche: plataformas de día (doradas) y de noche (azules), con 0.6 s de gracia tras el cambio para saltar de una a otra | 3-3 |
| Lianas: se balancean solas; al tocarlas en el aire el jugador se agarra, y se suelta con la tecla de salto | 4-1 |
| Hojas trampolín: lanzan mucho más alto que un salto | 4-2 |
| Monos que lanzan cocos en arco; no le lanzan a quien espera en su puerta | 4-2 |
| Crecida del río: el agua sube sin parar y se lleva a los dos; obliga a escalar | 4-4 |
| Cuerdas de charango: trampolín que toca su nota (5 notas con colores); algunas solo suenan de día o de noche | 5-1 |
| Puerta melódica: las luces muestran la melodía; una nota equivocada la reinicia (repetir la última no cuenta) | 5-2 |
| Eclipse automático cada 9 s, con cuenta regresiva; 0,7 s de gracia en lava y agua al cambiar | 6-1 |

Estrellas: 1 por terminar, 1 por todas las gemas y 1 por terminar con pocas muertes (el máximo se configura por nivel en `DatosJuego`).

### Trampas (`01_Scripts/Mecanicas/Trampas/`)
Se ven igual que las piezas normales. En el Inspector tienen una opción de pista visual muy sutil (`mostrarPistaSutil`).

| Trampa | Qué hace |
|---|---|
| SueloFalso | Parece suelo y se cae al pisarlo |
| BloqueInvisible | Aparece de golpe cuando alguien salta debajo |
| PuasOcultas | Salen del suelo cuando alguien pasa encima |
| TechoQueCae | Un bloque cae cuando alguien pasa por debajo |
| GemaTrampa | Parece gema, pero al tocarla activa otra trampa; no cuenta para las estrellas |
| PuertaFalsa | Parece la salida, pero se escapa al acercarse |
| PlataformaQueHuye | Se aleja justo cuando alguien salta cerca |
| BotonEnganoso | Parece abrir algo, pero activa otra trampa |
| EnemigoSorpresa | Sale del suelo y persigue un tramo corto |

### Trampas de traición
Detectan qué jugador las activó y castigan al otro. Hay tres castigos: cae lava, salen púas o se abre el suelo. Siempre aparece un aviso rojo y la víctima tiene tiempo de moverse.

| Trampa | Qué hace |
|---|---|
| BotonTraicion | Quien lo pisa castiga a su compañero |
| PalancaSospechosa | Funciona de verdad, pero la primera vez castiga al compañero |
| PlacaVinculada | Si uno se baja de su placa, el que se queda recibe el castigo |

### Sistema de muertes
- Reinicio casi instantáneo al morir (0.4 s de espera más lo que tarde en cargar la escena).
- Contador de muertes visible en el nivel; se conserva entre reinicios y vuelve a cero al entrar de nuevo.
- Récord de menos muertes por nivel guardado en PlayerPrefs.
- Mensajes graciosos según la causa ("Ese suelo no era de verdad...", "¡Killa traicionó a Inti!", "Te dije que no confiaras en las gemas.").

### Reglas de dificultad
- Mundo 1: 1 o 2 trampas por nivel, fáciles de descubrir. Cada mundo agrega más. El Mundo 6 es el más tramposo.
- Los niveles 4 y 5 de cada mundo usan trampas de traición.
- **Regla de justicia:** cada trampa engaña una vez, pero después de conocerla el nivel siempre debe poder ganarse. Nunca hay trampas imposibles de evitar.

### Enemigos y peligros (`01_Scripts/Enemigos/`)
Comportamientos reutilizables para distintos animales. Mientras no haya arte se usan formas de color, con un campo `Sprite` para poner la imagen después.

| Comportamiento | Qué hace | Ejemplos |
|---|---|---|
| EnemigoPatrulla | Camina de un punto A a un punto B | Serpientes |
| EnemigoVolador | Vuela en zigzag o en círculo | Murciélagos, avispas |
| EnemigoEnPicada | Espera arriba y se lanza al jugador más cercano | Cóndor |
| LanzadorProyectiles | Desde un lugar fijo lanza objetos en arco cada cierto tiempo | Monos con cocos, llamas que escupen |
| EnemigoPerseguidor | Persigue dentro de su zona y vuelve a su lugar si te alejas | Avispas del panal |
| PeligroQueCae | Tiembla un instante, cae muy rápido y reaparece en medio segundo: pasa unos 1,2 s colgada | Rocas de las minas |
| PiedraRodante | Rueda por el suelo o una pendiente | Yungas, Aiquile |
| RemolinoViento | Empuja hacia un lado sin matar | Remolinos de sal |

Quien espera dentro de una puerta está a salvo de monos, llamas y avispas. Saltar encima de los enemigos que caminan o vuelan los aturde 3 segundos; tocarlos de costado mata (y el nivel se reinicia para los dos). Cada muerte tiene su mensaje gracioso ("¡Un mono le tiró un coco a Inti!", "La llama escupió a Killa").

**Regla de justicia:** todo ataque tiene un aviso visible (temblor, sonido o animación) antes de golpear, y ningún enemigo hace imposible un nivel.

| Mundo | Enemigos y peligros |
|---|---|
| 1. Salar de Uyuni | Solo remolinos de sal, en los niveles 4 y 5 |
| 2. Minas de Potosí | Rocas que caen y murciélagos, desde el nivel 2 |
| 3. Yungas | Cóndor desde el nivel 2 y piedras rodantes desde el nivel 3 |
| 4. Trópico | Monos que lanzan cocos y serpientes, en casi todos los niveles |
| 5. Aiquile | Avispas y piedras rodantes, combinadas con las trampas |
| 6. Isla del Sol | Llamas que escupen en todos los niveles; el nivel 5 es el jefe final |

### Jefe final: GuardianEclipse (nivel 6-5)
Se mueve de lado a lado y lanza proyectiles de fuego y de agua; cada uno solo daña al elemento contrario. Antes de cada disparo brilla del color de la bola. Tiene 3 vidas y pierde una cuando los DOS jugadores pisan a la vez dos botones del escenario; después de cada golpe parpadea 2,5 s y se vuelve más rápido. Al vencerlo sube un escalón hacia las puertas, y al ganar el nivel llega la escena Final.

### Plantilla de cada mundo
- **Nivel 1:** enseña la mecánica nueva, con trampas suaves.
- **Nivel 2:** combina la mecánica con cooperación (botones, palancas).
- **Nivel 3:** usa el eclipse y más trampas sorpresa.
- **Nivel 4:** aparecen las trampas de traición.
- **Nivel 5:** el más tramposo y difícil del mundo.
- Todos: de 1 a 3 minutos, caben en una pantalla, con una pista al inicio (se vuelve a leer con H) y de 2 a 4 gemas por elemento.

## 6. Mundos y niveles
Son 6 mundos con 5 niveles cada uno. Cada nivel es una escena llamada `MundoX_NivelY` (por ejemplo, `Mundo1_Nivel2`).

| Mundo | Mecánica nueva |
|---|---|
| 1. Salar de Uyuni (tutorial) | Movimiento, botones, eclipse |
| 2. Minas de Potosí | Luz, vagonetas, cajas |
| 3. Yungas | Niebla, lluvia, día y noche |
| 4. Trópico (Chapare) | Lianas, hojas trampolín, monos |
| 5. Aiquile, tierra del charango | Cuerdas trampolín, puerta melódica |
| 6. Isla del Sol | Eclipse automático, todo mezclado |

| Nivel | Título | Objetivo | Dif. |
|---|---|---|---|
| 1-1 | Primeros pasos | Moverse, saltar, gemas | 1 |
| 1-2 | Botones y compuertas | Botones y puertas de dos | 1 |
| 1-3 | El primer eclipse | Primer eclipse | 2 |
| 1-4 | Espejo de sal | Repaso: botones y eclipse juntos | 2 |
| 1-5 | Travesía del salar | Examen del mundo: todo lo aprendido | 2 |
| 2-1 | La luz de Inti | Inti ilumina para Killa | 2 |
| 2-2 | Vagonetas | Vagonetas y plataformas | 2 |
| 2-3 | Cajas pesadas | Cajas entre los dos | 3 |
| 2-4 | Galerías oscuras | Luz y vagonetas en la oscuridad | 3 |
| 2-5 | El corazón del Cerro Rico | Examen del mundo: luz, vagonetas y cajas | 3 |
| 3-1 | Camino en la niebla | Caminos angostos con niebla | 2 |
| 3-2 | Lluvia en la montaña | Lluvia y refugios | 3 |
| 3-3 | Día y noche | Día y noche | 3 |
| 3-4 | Puente entre nubes | Niebla con plataformas de día y de noche | 3 |
| 3-5 | El camino de la muerte | Examen del mundo: niebla, lluvia, día y noche | 4 |
| 4-1 | Ríos y lianas | Ríos y lianas | 3 |
| 4-2 | Hojas trampolín | Hojas trampolín y monos | 3 |
| 4-3 | Eclipse en la selva | Eclipse en la selva | 3 |
| 4-4 | Persecución en el río | Escapar de la crecida escalando en zigzag | 4 |
| 4-5 | El corazón del Chapare | Examen del mundo: lianas, monos y eclipse | 4 |
| 5-1 | Cuerdas de charango | Cuerdas de charango | 3 |
| 5-2 | La puerta melódica | Primera puerta melódica | 3 |
| 5-3 | Melodía a dúo | Melodía coordinada | 4 |
| 5-4 | Serenata nocturna | Melodía + día y noche | 4 |
| 5-5 | El gran concierto | Examen del mundo: melodía larga a dúo | 4 |
| 6-1 | Eclipse eterno | Eclipse automático | 4 |
| 6-2 | Las escalinatas de Yumani | Subida vertical con eclipse automático | 4 |
| 6-3 | El templo del Sol | Eclipse automático con luz y oscuridad | 5 |
| 6-4 | Todo a la vez | Todo mezclado | 5 |
| 6-5 | El gran final | Gran final | 5 |

Los 30 niveles están armados (Mundos 1 a 6).

Desbloqueo (durante el desarrollo todo está abierto con `modoPruebaTodoAbierto` en GestorProgreso; se apaga antes de publicar): al inicio solo está abierto el nivel 1-1. Ganar un nivel abre el siguiente de su mundo. Un mundo nuevo se abre solo al ganar el nivel 5 del anterior.

Reglas: cada nivel dura 1–3 min, introduce una sola idea nueva, y la dificultad final viene de la precisión, no de la longitud.

## 7. Escenas e interfaz
Escenas: `Intro` → `MenuPrincipal` → `SeleccionMundo` → `SeleccionNivel` → `Mundo1_Nivel1`…`Mundo6_Nivel5` → `Final`.

- `SeleccionMundo`: tarjetas altas que se pasan hacia los lados (arrastrando, con flechas o con el teclado), con número, dibujo y nombre del mundo, estrellas (por ejemplo, 9/15) y candado si está bloqueado. El fondo muestra el paisaje del mundo central.
- `SeleccionNivel`: una sola escena para todos los mundos. Cuadros grandes con número y 3 estrellas, título debajo, candado si está bloqueado y "Pronto" si el nivel aún no existe. Flecha redonda para volver.
- F1 en estas dos pantallas desbloquea todo (modo prueba y defensa).
- Al ganar el nivel 5 de un mundo aparece "¡Mundo completado!" y se vuelve a `SeleccionMundo`.

Ventanas encima del nivel (no escenas): HUD (gemas, muertes, récord), Pausa con Esc (continuar, reiniciar, opciones, salir al mapa), Opciones (volumen general y pantalla completa; música y efectos separados llegarán con el Audio Mixer, e idioma con la traducción), Victoria (estrellas, gemas, muertes y tiempo), Derrota (reinicio automático con mensaje gracioso).

`Intro` y `Final` cuentan la historia en páginas (Enter sigue, Esc salta). El Final muestra el total de estrellas.

Guardado: PlayerPrefs (mundos desbloqueados, niveles desbloqueados por mundo, estrellas por nivel, récord de muertes por nivel, opciones).

## 8. Arte y sonido
- 2D dibujado a mano, colores planos, luces 2D.
- Personajes por partes en Photopea → PSD → 2D Animation (idle, correr, saltar, caer, morir, celebrar).
- Un tileset base con Rule Tiles recoloreado por mundo + decoración propia.
- Parallax de 2–3 capas por mundo.
- Juice: partículas, temblor de cámara, bloom, animaciones de UI con DOTween.
- Música andina/tropical libre de derechos (licencias en créditos); efectos de salto, gema, botón, puerta, muerte, eclipse, victoria y 5 notas de charango; Audio Mixer con música y efectos separados.

## 9. Tecnología
Paquetes: Universal RP (2D Renderer), 2D Tilemap Editor, 2D Tilemap Extras, 2D Sprite, 2D Animation, 2D PSD Importer, Input System, Cinemachine, TextMeshPro (uGUI), Localization, DOTween (Asset Store).

Skills del plugin oficial de Unity para Claude Code: new-unity-project, unity-cli, unity-package-management, tilemap-palette-create, tilemap-ruletile-createfromsegment, sprite-editor, manage-sprite-atlas, urp-postprocessing, ui-ugui, audio-setup-mixers, localization, optimize-web.

Plugin: https://unity.com/blog/unity-plugin-for-claude-code

## 10. Cronograma
| Semana | Fechas | Entregable |
|---|---|---|
| 1 | 28 oct – 3 nov | Proyecto, movimiento 2 jugadores, muerte/reinicio, salida, niveles 1-1 a 1-3 |
| 2 | 4 – 10 nov | Estructura de mundos, Mundo 1 completo, vagonetas, cajas, niebla, lluvia, día y noche |
| 3 | 11 – 17 nov | Mundo 2, lianas, monos, charango, eclipse automático |
| 4 | 18 – 24 nov | Mundos 3 y 4 |
| 5 | 25 nov – 1 dic | Mundos 5 y 6, HUD, pausa, opciones, victoria |
| 6 | 2 – 8 dic | Arte final, audio, juice, intro y final |
| 7 | 9 – 15 dic | Pruebas, build WebGL, publicación en itch.io, defensa |

Plan B: si al cierre de la semana 5 faltan niveles, publicar con 4 mundos completos (20 niveles) pulidos.
