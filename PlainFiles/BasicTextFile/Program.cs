using BasicTextFile;
using CVSWithLibary;

var textFile = new SimpleTextFile("data.txt");
var lines = textFile.ReadLines();
string usuarioActual = "";

var helper = new CsvHelperExample();
var readPeople = helper.Read("people.csv").ToList();

using (var logger = new LogWriter("log.txt"))
{
    var opc = "0";
    logger.WriteLog("INFO", "Aplicación iniciada.");

    do
    {
        opc = Menu();
        Console.WriteLine("=======================================");
        switch (opc)
        {
            case "1":
                logger.WriteLog("INFO", usuarioActual + " - Mostrar contenido.");
                if (readPeople.Count == 0)
                {
                    Console.WriteLine("Archivo vacío.");
                    logger.WriteLog("ERROR", usuarioActual + " - Archivo vacío.");
                    break;
                }
                foreach (var person in readPeople)
                {
                    Console.WriteLine(person);
                }
                break;

            case "2":
                logger.WriteLog("INFO", usuarioActual + " - Agregar línea.");

                int id;
                do
                {
                    Console.Write("Ingrese el ID (numérico y único): ");
                } while (!int.TryParse(Console.ReadLine(), out id) || readPeople.Any(p => p.Id == id));

                string firstName;
                do
                {
                    Console.Write("Ingrese el nombre: ");
                    firstName = Console.ReadLine() ?? "";
                } while (string.IsNullOrWhiteSpace(firstName));

                string lastName;
                do
                {
                    Console.Write("Ingrese el apellido: ");
                    lastName = Console.ReadLine() ?? "";
                } while (string.IsNullOrWhiteSpace(lastName));

                string phone;
                do
                {
                    Console.Write("Ingrese el teléfono: ");
                    phone = Console.ReadLine() ?? "";
                } while (!System.Text.RegularExpressions.Regex.IsMatch(phone, @"^\(?\d{2,4}\)?[-.\s]?\d{3,4}[-.\s]?\d{3,4}$"));

                Console.Write("Ingrese la ciudad: ");
                var city = Console.ReadLine() ?? "";

                decimal balance;
                do
                {
                    Console.Write("Ingrese el saldo (número positivo): ");
                } while (!decimal.TryParse(Console.ReadLine(), out balance) || balance < 0);

                var newPerson = new Person
                {
                    Id = id,
                    FirstName = firstName,
                    LastName = lastName,
                    Phone = phone,
                    City = city,
                    Balance = balance
                };

                readPeople.Add(newPerson);
                break;

            case "3":
                logger.WriteLog("INFO", usuarioActual + " - Editar persona.");

                Console.Write("Ingrese el ID de la persona a editar: ");
                string inputId = Console.ReadLine() ?? "";
                if (!int.TryParse(inputId, out int editId))
                {
                    Console.WriteLine("ID no válido.");
                    break;
                }

                var personToEdit = readPeople.FirstOrDefault(p => p.Id == editId);
                if (personToEdit == null)
                {
                    Console.WriteLine("Persona no encontrada.");
                    break;
                }

                Console.WriteLine($"Editando a: {personToEdit.FirstName} {personToEdit.LastName}");

                Console.Write($"Nombre ({personToEdit.FirstName}): ");
                var newFirstName = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(newFirstName))
                    personToEdit.FirstName = newFirstName;

                Console.Write($"Apellido ({personToEdit.LastName}): ");
                var newLastName = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(newLastName))
                    personToEdit.LastName = newLastName;

                string newPhone;
                do
                {
                    Console.Write($"Teléfono ({personToEdit.Phone}): ");
                    newPhone = Console.ReadLine() ?? "";
                    if (string.IsNullOrWhiteSpace(newPhone))
                        break;
                } while (!System.Text.RegularExpressions.Regex.IsMatch(newPhone, @"^\(?\d{2,4}\)?[-.\s]?\d{3,4}[-.\s]?\d{3,4}$"));
                if (!string.IsNullOrWhiteSpace(newPhone))
                    personToEdit.Phone = newPhone;

                Console.Write($"Ciudad ({personToEdit.City}): ");
                var newCity = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(newCity))
                    personToEdit.City = newCity;

                string newBalanceStr;
                decimal tempBalance = 0;
                do
                {
                    Console.Write($"Saldo ({personToEdit.Balance}): ");
                    newBalanceStr = Console.ReadLine() ?? "";
                    if (string.IsNullOrWhiteSpace(newBalanceStr))
                        break;
                } while (!decimal.TryParse(newBalanceStr, out tempBalance) || tempBalance < 0);

                if (!string.IsNullOrWhiteSpace(newBalanceStr))
                    personToEdit.Balance = tempBalance;

                Console.WriteLine("Persona actualizada exitosamente.");
                break;

            case "4":
                logger.WriteLog("INFO", usuarioActual + " - Eliminar persona.");

                Console.Write("Ingrese el ID de la persona a eliminar: ");
                string deleteIdInput = Console.ReadLine() ?? "";
                if (!int.TryParse(deleteIdInput, out int deleteId))
                {
                    Console.WriteLine("ID no válido.");
                    break;
                }

                var personToDelete = readPeople.FirstOrDefault(p => p.Id == deleteId);
                if (personToDelete == null)
                {
                    Console.WriteLine("Persona no encontrada.");
                    break;
                }

                Console.WriteLine("Datos de la persona:");
                Console.WriteLine($"ID: {personToDelete.Id}");
                Console.WriteLine($"Nombre: {personToDelete.FirstName} {personToDelete.LastName}");
                Console.WriteLine($"Teléfono: {personToDelete.Phone}");
                Console.WriteLine($"Ciudad: {personToDelete.City}");
                Console.WriteLine($"Saldo: {personToDelete.Balance}");

                Console.Write("¿Está seguro que desea eliminar esta persona? (s/n): ");
                var confirm = Console.ReadLine()?.Trim().ToLower();

                if (confirm == "s")
                {
                    readPeople.Remove(personToDelete);
                    Console.WriteLine("Persona eliminada exitosamente.");
                    logger.WriteLog("INFO", usuarioActual + $" - Persona con ID {deleteId} eliminada.");
                }
                else
                {
                    Console.WriteLine("Eliminación cancelada.");
                    logger.WriteLog("INFO", usuarioActual + $" - Canceló la eliminación de ID {deleteId}.");
                }
                break;

            case "5":
                logger.WriteLog("INFO", usuarioActual + $" - Guardado el archivo.");
                using (var writer = new StreamWriter("people.csv"))
                using (var csv = new CsvHelper.CsvWriter(writer, System.Globalization.CultureInfo.InvariantCulture))
                {
                    csv.WriteHeader<Person>();
                    csv.NextRecord();
                    foreach (var person in readPeople)
                    {
                        csv.WriteRecord(person);
                        csv.NextRecord();
                    }
                }
                Console.WriteLine("Personas guardadas en el archivo CSV.");
                logger.WriteLog("INFO", "Archivo guardado.");
                break;

            case "0":
                Console.WriteLine("Saliendo...");
                break;

            default:
                Console.WriteLine("Opción no válida. Intente nuevamente.");
                break;
        }
    } while (opc != "0");
    logger.WriteLog("INFO", "Aplicación finalizada.");
    GuardarCambios();
}

void GuardarCambios()
{
    Console.WriteLine("Guardando cambios...");
    textFile.WriteLines(lines);
    Console.WriteLine("Cambios guardados.");
}

string Menu()
{
    string[] lineasUsuarios = File.ReadAllLines("C:\\Estructure de datos\\Archivos planos\\PlainFiles\\CVSWithLibary\\bin\\Debug\\net8.0\\Users.txt");
    int intentos = 0;
    bool autenticado = false;

    while (!autenticado && intentos < 3)
    {
        Console.WriteLine("\n=======================================");
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
                    Console.WriteLine("");
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

    Console.WriteLine("");
    Console.WriteLine("=======================================");
    Console.WriteLine("1. Mostrar contenido");
    Console.WriteLine("2. Agregar persona");
    Console.WriteLine("3. Editar Persona");
    Console.WriteLine("4. Borrar Persona");
    Console.WriteLine("5. Guardar cambios");
    Console.WriteLine("0. Salir");
    Console.Write("Seleccione una opción: ");
    return Console.ReadLine() ?? "0";
}
