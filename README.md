# TodoAppBack

¡Bienvenido al backend de TodoApp! Este proyecto es la parte del servidor para la aplicación de gestión de tareas.

## Instrucciones

Sigue estos pasos para configurar y ejecutar el backend de la aplicación:

### Descargar el Repositorio

Clona este repositorio en tu máquina local utilizando Git:

git clone https://github.com/DanielBurgosA/TodoAppBack.git

### Configurar la Base de Datos

Crea una base de datos llamada "todo" en tu servidor MySQL.

### Instalar las Dependencias

Navega hasta el directorio del proyecto y ejecuta el siguiente comando para instalar todas las dependencias necesarias:

dotnet restore

### Aplicar Migraciones

Una vez instaladas las dependencias, ejecuta el siguiente comando para aplicar las migraciones y crear las tablas necesarias en la base de datos:

dotnet ef database update

### Ejecutar el Servidor

Finalmente, puedes iniciar el servidor ejecutando el siguiente comando:

dotnet run

## Contribuciones

¡Las contribuciones son bienvenidas! Si deseas contribuir a este proyecto, por favor sigue estos pasos:

1. Realiza un fork del repositorio.
2. Crea una nueva rama (git checkout -b feature/nueva-caracteristica).
3. Realiza tus cambios y commit (git commit -am 'Añadir nueva característica').
4. Sube tus cambios al repositorio (git push origin feature/nueva-caracteristica).
5. Abre un pull request.

## Soporte

Si tienes algún problema o pregunta, por favor abre un issue en este repositorio: https://github.com/DanielBurgosA/TodoAppBack/issues

## Licencia

Este proyecto está licenciado bajo la Licencia MIT: https://opensource.org/licenses/MIT
