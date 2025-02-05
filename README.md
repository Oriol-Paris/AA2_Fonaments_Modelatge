# AA2_Fonaments_Modelatge

## Scripts

### Gradient

He replanteado el código de clase para que pueda funcionar con los brazos de Doc Ock. A parte de esto lo he optimizado y limpiado, sobretodo con el uso de los joints, que lo hago a través de una lista y no guardo los joints en variables distintas, esto me ha permitido eliminar mucho código repetido y que sea más legible en general.

### Arm Controller

Básicamente hace de máquina de estados para los distintos brazos de Doc Ock dependiendo de Spiderman, los 3 estados posibles son "TOO_FAR" cuando está demasiado lejos de Spiderman, "CAN_MOVE" cuando está lo suficientemente cerca como para mover los brazos y "GRABBED" cuando está agarrando a Spiderman.

### Doc Ock

Es el Character Controller de Doc Ock, junto con los controles para cerrar el programa. Los controles son los siguientes:

WASD ->  Movimiento
Q -> Agarrar a Spiderman
Esc -> Cerrar el programa

### Claw Controller

En cuanto se llama a la función StartAnim() desde el script anterior, se empieza a ejecutar una animación que cierra la garra del brazo de Doc Ock y setea a Spiderman como hijo de Doc Ock, agarrándolo.

### Spiderman Movement

Dado un recorrido en el mapa, este script se dedica a que Spiderman lo recorra.
