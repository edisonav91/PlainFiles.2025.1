using BasicTextFile;

var textFile = new SimpleTextFile("data.txt");
var lines = textFile.ReadLines();
string usuarioActual = "";

using (var logger = new LogWriter("log.txt"))
{
    var opc = "0";
    logger.WriteLog("INFO", "Application started.");

    do
    {
        opc = Menu();
        Console.WriteLine("=======================================");
        switch (opc)
        {
            case "1":
                logger.WriteLog("INFO", usuarioActual + " - Show content.");
                if (lines.Length == 0)
                {
                    Console.WriteLine("Empty file.");
                    logger.WriteLog("ERROR", usuarioActual + " - Empty file.");
                    break;
                }
                foreach (var line in lines)
                {
                    Console.WriteLine(line);
                }
                break;

            case "2":
                logger.WriteLog("INFO", usuarioActual + " - Add line.");
                Console.Write("Enter the line to add: ");
                var newLine = Console.ReadLine();
                if (!string.IsNullOrEmpty(newLine))
                {
                    lines = lines.Append(newLine).ToArray();
                }
                break;

            case "3":
                logger.WriteLog("INFO", usuarioActual + " - Update line.");
                Console.Write("Enter the line to update: ");
                var lineToUpdate = Console.ReadLine();
                Console.Write("Enter the new value: ");
                var newValue = Console.ReadLine();
                for (int i = 0; i < lines.Length; i++)
                {
                    if (lines[i] == lineToUpdate)
                    {
                        lines[i] = newValue!;
                        break;
                    }
                }
                break;

            case "4":
                logger.WriteLog("INFO", usuarioActual + " - Remove line.");
                Console.Write("Enter the line to remove: ");
                var lineToRemove = Console.ReadLine();
                if (!string.IsNullOrEmpty(lineToRemove))
                {
                    lines = lines.Where(line => line != lineToRemove).ToArray();
                }
                break;

            case "5":
                SaveChanges();
                logger.WriteLog("INFO", "Save file.");
                break;

            case "0":
                Console.WriteLine("Exiting...");
                break;

            default:
                Console.WriteLine("Invalid option. Please try again.");
                break;
        }
    } while (opc != "0");
    logger.WriteLog("INFO", "Application ended.");
    SaveChanges();
}

void SaveChanges()
{
    Console.WriteLine("Saving changes...");
    textFile.WriteLines(lines);
    Console.WriteLine("Changes saved.");
}

string Menu()
{
    string[] lineasUsuarios = File.ReadAllLines("C:\\Estructure de datos\\Archivos planos\\PlainFiles\\CVSWithLibary\\bin\\Debug\\net8.0\\Users.txt");
    int intentos = 0;
    bool autenticado = false;

    while (!autenticado && intentos < 3)
    {
        Console.WriteLine("=======================================");
        Console.Write("Ingrese el nombre de usuario: ");
        string? usuarioIngresado = Console.ReadLine();

        Console.Write("Ingrese la contraseña: ");
        string? contrasenaIngresada = Console.ReadLine();

        bool usuarioExiste = false;

        for (int i = 0; i < lineasUsuarios.Length; i++)
        {
            string[] partes = lineasUsuarios[i].Split(',');

            if (partes.Length != 3)
                continue;

            string usuarioArchivo = partes[0];
            string contrasenaArchivo = partes[1];
            bool activo = bool.Parse(partes[2]);

            if (usuarioArchivo == usuarioIngresado)
            {
                usuarioExiste = true;

                if (!activo)
                {
                    Console.WriteLine("El usuario está bloqueado. Comuníquese con el administrador.");
                    return "0";
                }

                if (contrasenaArchivo == contrasenaIngresada)
                {
                    autenticado = true;
                    usuarioActual = usuarioArchivo;
                    Console.WriteLine($"Bienvenido, {usuarioActual}.");
                    break;
                }
                else
                {
                    Console.WriteLine("Contraseña incorrecta.");
                    intentos++;
                    break;
                }
            }
        }

        if (!usuarioExiste)
        {
            Console.WriteLine("El usuario no existe.");
            intentos++;
        }

        if (intentos == 3)
        {
            for (int i = 0; i < lineasUsuarios.Length; i++)
            {
                var partes = lineasUsuarios[i].Split(',');

                if (partes[0] == usuarioIngresado)
                {
                    lineasUsuarios[i] = $"{partes[0]},{partes[1]},false";
                    File.WriteAllLines("Users.txt", lineasUsuarios);
                    Console.WriteLine("Usuario bloqueado por exceder el número de intentos.");
                    break;
                }
            }

            return "0";
        }
    }

    Console.WriteLine("=======================================");
    Console.WriteLine("1. Mostrar contenido");
    Console.WriteLine("2. Agregar persona");
    Console.WriteLine("3. Guardar cambios");
    Console.WriteLine("0. Salir");
    Console.Write("Seleccione una opción: ");
    return Console.ReadLine() ?? "0";
}
