# Propuesta: Inti y Killa, versión engañosa y desafiante

Fecha: 11 de noviembre de 2026. Estado: **aprobada**. Se empieza por las trampas; el arte (rocas flotantes, decoración) viene después, con imágenes hechas en Leonardo AI.

## 1. Objetivo

Que los 30 niveles sean mucho más difíciles al estilo de los juegos "troll" de plataformas: el escenario parece sencillo, pero el juego intenta engañar al jugador todo el tiempo ("¡no puede ser, me volvió a engañar!").

- La dificultad sale del **engaño**, no de poner más pinchos o más enemigos.
- Se conservan los personajes, el arte, la historia y los sistemas propios del juego: solo se toma la idea de diseño.
- **Regla de justicia** (GDD): toda trampa tiene un aviso visible y, después de conocerla, el nivel siempre se puede ganar.

## 2. Punto de partida (pruebas del 11 de noviembre)

| Prueba | Resultado |
|---|---|
| Los 30 niveles cargan y corren sin errores | 30 de 30 |
| Un bot juega los 30 niveles con teclas simuladas (sin teletransportarse) | 30 ganados, 0 muertes |
| Sistema: pausa, pista, eclipse, cuerdas, melodía, Intro y Final después del jefe | 0 fallos |
| Piezas del desafío probadas una por una | 0 fallos |
| Reinicio: ninguna trampa queda activada (la escena se recarga entera) | Revisado en el código |

Lo que todavía no se cumple:

- **La dificultad no sube de forma progresiva.** Trampas por nivel: Mundo 1 = 4,8 · Mundo 2 = 4,8 · Mundo 3 = 3,4 · Mundo 4 = 4,6 · Mundo 5 = 6 · Mundo 6 = 5,4.
- Casi no hay **trampas combinadas** ni "momentos memorables".
- La plataforma que huye y la flecha engañosa están programadas, pero no están en ningún nivel.
- **El bot de pruebas no está guardado en el proyecto**, así que la prueba de "30 de 30 ganados" no se puede repetir.
- En 9 niveles el bot no recoge todas las gemas (3-1, 4-1, 4-4, 4-5, 5-2, 5-3, 5-4, 5-5, 6-3). Hay que confirmar que se pueden alcanzar.

## 3. Trampas que ya existen y se reutilizan

| Tipo | Trampas |
|---|---|
| Objetos | GemaTrampa (activa púas, techo u otras trampas), GemaQueHuye, PocionEnganosa |
| Escenario | SueloFalso, SueloQueDesapareceAlSaltar, TechoQueCae, ParedAplastadora, PisoQueSeCalienta, BloqueInvisible, BloqueQueCierraElRegreso, PlataformaQueHuye, PlataformaQueSeVoltea, SueloEnContra, PuasOcultas |
| Psicológicas | PuertaFalsa, PuertasQueSeCambian, BotonEnganoso, ZonaControlesInvertidos, flecha engañosa |
| Traición | BotonTraicion, PalancaSospechosa, PlacaVinculada |
| Sorpresa | EnemigoSorpresa (sale del suelo), EnemigoQueCaeDelTecho, LluviaDeRocas (con sombras), SensorTrampa (al entrar, al saltar, los dos juntos) |

## 4. Piezas nuevas (etapa 1)

Cada pieza es un componente reutilizable con su aviso visible, y se prueba por separado antes de ponerla en un nivel.

| Pieza | Qué hace | Aviso justo |
|---|---|---|
| GemaExplosiva | Parece una gema; al tocarla tiembla y explota en un radio pequeño | Brilla en rojo 0,6 s antes de explotar |
| GemaQuePersigue | Le salen ojos y dientes y persigue unos segundos; pisarla la aturde | Abre los ojos antes de moverse |
| PlataformaFantasma | Se ve sólida, pero se atraviesa | Parpadeo muy sutil |
| PalancaEnganosa | Promete "apagar el peligro" y lo enciende, o enciende otro | Pista sutil en el cartel |
| RocaPorDetras | Al pasar un punto, una roca aparece atrás y rueda hacia el jugador | Temblor y polvo antes |
| NubeTramposa | Una nube del fondo suelta un rayo o una lluvia sobre el jugador | Sombra en el piso |
| SalidaQueSeAleja | La puerta real se corre una vez a un lugar más difícil (pero alcanzable) | Se ve cómo se mueve |
| Sensor "al llegar a la salida" | Cuando uno entra a su puerta, al compañero le pasa algo | Aviso rojo, como las traiciones |

## 5. Plan por mundo (etapas 2 a 4)

Se trabaja de a dos mundos por etapa. Cada nivel 5 tiene una trampa que **parece imposible la primera vez** pero tiene un truco.

| Mundo | Dificultad | Trampas por nivel | Momentos memorables propuestos |
|---|---|---|---|
| 1 Salar de Uyuni (espejismos, sal, gemas) | Difícil | 6 | 1-1 gema-espejismo que huye y flecha que apunta al suelo falso · 1-3 una gema desata lluvia de rocas de sal mientras esperan el eclipse · 1-4 una de las dos plataformas "espejo" es fantasma · 1-5 gema sobre un bloque que se rompe + bicho que cae + el piso de abajo se vuelve veneno |
| 2 Minas de Potosí (derrumbes, vagonetas, túneles) | Muy difícil | 7 | 2-2 la vagoneta arranca sola justo al aterrizar · 2-3 gema explosiva junto a la caja · 2-4 roca por detrás en el túnel oscuro · 2-5 cuando uno llega a la puerta, se derrumba el techo sobre el otro |
| 3 Yungas (niebla, lluvia, cóndor, día y noche) | Muy difícil | 8 | 3-1 nube tramposa en la niebla · 3-2 el techo-refugio de la lluvia se cae · 3-4 el bloque de noche huye al aterrizar · 3-5 roca por detrás + nube + salida que se aleja |
| 4 Trópico (lianas, hojas, monos, crecida) | Extrema | 9 | 4-1 gema que persigue en la orilla · 4-2 una hoja lanza directo al mono · 4-4 plataforma flotante que huye con la crecida subiendo · 4-5 liana + gema que desata lluvia de cocos + salida que se aleja |
| 5 Aiquile (mecanismos, cuerdas, traición) | Extrema | 10 | 5-2 la palanca que "abre" la reja la cierra · 5-3 trampa cuando los dos están juntos en la salida · 5-4 cuando uno entra a su puerta, al otro le sale una pared · 5-5 cadena larga de trampas |
| 6 Isla del Sol (eclipse y todo mezclado) | Desafío final | 11-12 | Combinaciones de todo, sincronizadas con el eclipse automático · 6-5: trampas en la arena del jefe y salida que se aleja después de ganar |

Los temas siguen el GDD: Yungas es niebla, lluvia y día/noche, y lianas, monos y crecida son del Trópico.

## 6. Etapas y pruebas

| Etapa | Qué se hace | Cómo se comprueba |
|---|---|---|
| 0 | Guardar el bot como prueba oficial en `01_Scripts/Pruebas`, con prueba de reinicio y de gemas alcanzables | El bot corre desde el proyecto y gana los 30 niveles |
| 1 | Las 8 piezas nuevas | Cada pieza probada sola: se activa, avisa, mata solo a quien corresponde y se reinicia bien |
| 2 | Mundos 1 y 2 | El bot actualizado gana los 30 niveles + tú los juegas |
| 3 | Mundos 3 y 4 | Igual |
| 4 | Mundos 5 y 6 | Igual |
| 5 | GDD y checklist al día | Solo se marca lo que pasó las pruebas |

Al final de cada etapa: commit, bot sobre los 30 niveles y una partida tuya antes de seguir.

## 7. Arte con Leonardo AI (después de las trampas)

Para que las imágenes encajen directo en el juego:

- **Formato:** PNG con fondo transparente (sin fondo blanco), vista lateral de frente, como un juego de plataformas 2D.
- **Plataformas de roca flotante:** una pieza ancha de unos **1024 × 256 px**, con borde superior plano (ahí se camina) y la parte de abajo irregular. Mejor si se puede repetir a lo ancho. Una por mundo: sal blanca (Uyuni), mineral marrón y rojizo (Potosí), tierra con musgo (Yungas), madera y raíces (Trópico), adobe y piedra (Aiquile), piedra andina tallada (Isla del Sol).
- **Decoración:** elementos sueltos de unos **256 × 256 px** (cristales de sal, vagonetas, helechos, flores, charangos, piedras talladas), 4 a 6 por mundo.
- **Estilo:** colores planos y vivos, contorno suave, el mismo estilo que Inti y Killa.
- **Dónde ponerlas:** `ArteOriginal/Mundos/MundoN/`, con nombres simples (`Plataforma.png`, `Decoracion1.png`, …).
- **Licencia:** revisar que el plan de Leonardo AI permita publicar en itch.io y anotarlo en los créditos.

## 8. Versión psicológica (20 de noviembre)

Pedido nuevo, a partir de dos videos de referencia: que pasar los niveles sea más "psicológico" y confuso, con un bucle. Del 1-1 al 1-3 siguen siendo fáciles; desde el 1-4 sube la confusión.

**Sala que se repite (el bucle).** Al llegar los dos a sus puertas, el nivel no termina: la pantalla parpadea, aparece "¿Otra vez?" y la misma sala vuelve a empezar con una regla cambiada. Vuelta 2: las puertas se cambian de lugar al acercarse (en 4-2 y 5-1 la vuelta 2 queda igual, porque cruzar obligaría a pasar por un peligro imposible de esquivar). Vuelta 3: aparece una neblina que invierte los controles cerca de la salida. Morir no hace perder las vueltas ganadas. Niveles: 2-3, 3-2, 4-2, 5-1 y 6-4.

**Trampas nuevas** (componentes `SalaQueSeRepite`, `VariantePorVuelta`, `NubeAsesina`, `GemaIman`, `PocionDePeso`, `ArbustoTrampa`, `CartelQueCae`, `ArenaMovediza`, `PuasDeGoma`, `PuasFijas`, `EnemigoEscalera`, `RocaGiganteQueRueda`; el trampolín catapulta es una `HojaTrampolin` con más fuerza):

| Trampa | Dónde | Aviso justo |
|---|---|---|
| Nube con rayo | 1-5, 3-1 | Se oscurece y tiembla antes del rayo |
| Gema imán | 2-1, 6-1 | Anillo gris que late; las piedras caen donde se tomó |
| Antorcha que dispara | 2-2 | Tiembla y se pone roja antes de disparar |
| Arbusto con dientes | 4-1 | Dos puntitos brillantes (ojos) y se sacude |
| Trampolín catapulta | 4-2 | Vena roja en la hoja; púas visibles en el techo |
| Cartel que cae | 4-4 | Las letras tiemblan; caen detrás de quien pasa |
| Arena movediza | 5-3 | Puntitos de arena en el "agua" |
| Poción de peso | 5-4 | Botella gris sobre un piso agrietado |
| Púas de goma | 6-3 | Se mueven como gelatina |
| Púas que caen del techo | 6-3 | Sombra en el piso antes de cada púa |

Programados pero todavía sin colocar: enemigo escalera y roca gigante que rueda.

**Pruebas:** piezas psicológicas 0 fallos, piezas del desafío 0 fallos y el bot gana los 30 niveles (las salas que se repiten, en sus 3 vueltas).
