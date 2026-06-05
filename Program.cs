using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace SistemaCitasMedicas
{
    public class Persona
    {
        public string Nombre { get; set; }
        public int Edad { get; set; }
        public string Telefono { get; set; }

        public Persona(string nombre, int edad, string telefono)
        {
            Nombre = nombre;
            Edad = edad;
            Telefono = telefono;
        }

        public virtual void MostrarDatos()
        {
            Console.WriteLine($"Nombre: {Nombre} | Edad: {Edad} | Tel: {Telefono}");
        }
    }

    public class Paciente : Persona
    {
        public string NumeroExpediente { get; set; }
        public string TipoSangre { get; set; }

        public Paciente(string nombre, int edad, string telefono, string numeroExpediente, string tipoSangre)
            : base(nombre, edad, telefono)
        {
            NumeroExpediente = numeroExpediente;
            TipoSangre = tipoSangre;
        }

        public override void MostrarDatos()
        {
            base.MostrarDatos();
            Console.WriteLine($"   -> Expediente: {NumeroExpediente} | Sangre: {TipoSangre}");
        }
    }

    public class Doctor : Persona
    {
        public string Especialidad { get; set; }
        public string CedulaProfesional { get; set; }

        public Doctor(string nombre, int edad, string telefono, string especialidad, string cedulaProfesional)
            : base(nombre, edad, telefono)
        {
            Especialidad = especialidad;
            CedulaProfesional = cedulaProfesional;
        }

        public override void MostrarDatos()
        {
            base.MostrarDatos();
            Console.WriteLine($"   -> Especialidad: {Especialidad} | Cédula: {CedulaProfesional}");
        }
    }


    public class CitaMedica
    {
        public Paciente Paciente { get; private set; }
        public Doctor Doctor { get; set; } // Permitimos cambiar doctor
        public DateTime FechaHora { get; set; } // Permitimos cambiar fecha
        public string Motivo { get; set; } // Permitimos cambiar motivo
        public string Estado { get; private set; }

        public CitaMedica(Paciente paciente, Doctor doctor, DateTime fechaHora, string motivo)
        {
            Paciente = paciente;
            Doctor = doctor;
            FechaHora = fechaHora;
            Motivo = motivo;
            Estado = "Pendiente";
        }

        public void ConfirmarCita() => Estado = "Confirmada";
        public void CancelarCita() => Estado = "Cancelada";

        public void MostrarCita(int indice)
        {
            Console.WriteLine($"[{indice}] CITA: {FechaHora:dd/MM/yyyy HH:mm} | ESTADO: {Estado.ToUpper()}");
            Console.WriteLine($"    Paciente: {Paciente.Nombre} | Doctor: {Doctor.Nombre} ({Doctor.Especialidad})");
            Console.WriteLine($"    Motivo: {Motivo}");
            Console.WriteLine(new string('-', 60));
        }
    }


    class Program
    {
        static List<Paciente> pacientes = new List<Paciente>();
        static List<Doctor> doctores = new List<Doctor>();
        static List<CitaMedica> citas = new List<CitaMedica>();

        static void Main(string[] args)
        {
            CargarDatosIniciales();
            bool salir = false;

            while (!salir)
            {
                try
                {
                    Console.Clear();
                    Console.WriteLine("========================================");
                    Console.WriteLine("   SISTEMA DE GESTIÓN MÉDICA INTEGRAL");
                    Console.WriteLine("========================================");
                    Console.WriteLine("1. Ver Reporte de Citas");
                    Console.WriteLine("2. Agendar Nueva Cita (Permite nuevo paciente)");
                    Console.WriteLine("3. Modificar Cita Existente");
                    Console.WriteLine("4. Confirmar Cita");
                    Console.WriteLine("5. Cancelar Cita");
                    Console.WriteLine("6. Salir");
                    Console.Write("\nSeleccione una opción: ");

                    string opcion = Console.ReadLine();

                    switch (opcion)
                    {
                        case "1": MostrarReporte(); break;
                        case "2": MenuCrearCita(); break;
                        case "3": MenuModificarCita(); break;
                        case "4": ModificarEstadoCita(true); break;
                        case "5": ModificarEstadoCita(false); break;
                        case "6": salir = true; break;
                        default: Error("Opción no válida."); break;
                    }
                }
                catch (Exception ex)
                {
                    Error($"Ocurrió un problema: {ex.Message}");
                }

                if (!salir)
                {
                    Console.WriteLine("\nPresione cualquier tecla para continuar...");
                    Console.ReadKey();
                }
            }
        }

        static void CargarDatosIniciales()
        {
            pacientes.Add(new Paciente("Juan Pérez", 35, "555-0101", "EXP-1001", "O+"));
            pacientes.Add(new Paciente("María García", 28, "555-0202", "EXP-1002", "A-"));
            doctores.Add(new Doctor("Dr. Roberto Sosa", 50, "555-9000", "Cardiología", "CP-8877"));
            doctores.Add(new Doctor("Dra. Elena Ruiz", 42, "555-9001", "Pediatría", "CP-5544"));
            citas.Add(new CitaMedica(pacientes[0], doctores[0], DateTime.Now.AddDays(1), "Chequeo General"));
            citas.Add(new CitaMedica(pacientes[1], doctores[1], DateTime.Now.AddDays(2), "Consulta Pediátrica"));
        }

        static void MenuCrearCita()
        {
            Console.WriteLine("\n--- AGENDAR CITA ---");
            
            Paciente pacienteSeleccionado = null;
            Console.WriteLine("0. [REGISTRAR NUEVO PACIENTE]");
            for (int i = 0; i < pacientes.Count; i++) Console.WriteLine($"{i + 1}. {pacientes[i].Nombre}");
            
            Console.Write("Seleccione una opción: ");
            string sel = Console.ReadLine();

            if (sel == "0")
            {
                pacienteSeleccionado = RegistrarNuevoPaciente();
            }
            else if (int.TryParse(sel, out int index) && index > 0 && index <= pacientes.Count)
            {
                pacienteSeleccionado = pacientes[index - 1];
            }
            else
            {
                Error("Selección inválida.");
                return;
            }

            Console.WriteLine("\nSeleccione un Doctor:");
            for (int i = 0; i < doctores.Count; i++) Console.WriteLine($"{i}. {doctores[i].Nombre} ({doctores[i].Especialidad})");
            if (!int.TryParse(Console.ReadLine(), out int iD) || iD < 0 || iD >= doctores.Count) { Error("Doctor inválido."); return; }

            // Motivo y Fecha
            Console.Write("Motivo: ");
            string motivo = Console.ReadLine();
            DateTime fecha = PedirFecha();

            citas.Add(new CitaMedica(pacienteSeleccionado, doctores[iD], fecha, motivo));
            Success("\nCita agendada correctamente.");
        }

        static void MenuModificarCita()
        {
            if (citas.Count == 0) { Console.WriteLine("No hay citas."); return; }
            MostrarReporte();
            Console.Write("\nÍndice de la cita a MODIFICAR: ");
            if (int.TryParse(Console.ReadLine(), out int idx) && idx >= 0 && idx < citas.Count)
            {
                var cita = citas[idx];
                Console.WriteLine("\n¿Qué desea modificar?");
                Console.WriteLine("1. Doctor");
                Console.WriteLine("2. Fecha y Hora");
                Console.WriteLine("3. Motivo");
                string op = Console.ReadLine();

                switch (op)
                {
                    case "1":
                        Console.WriteLine("Seleccione nuevo Doctor:");
                        for (int i = 0; i < doctores.Count; i++) Console.WriteLine($"{i}. {doctores[i].Nombre}");
                        if (int.TryParse(Console.ReadLine(), out int nD) && nD >= 0 && nD < doctores.Count)
                            cita.Doctor = doctores[nD];
                        break;
                    case "2":
                        cita.FechaHora = PedirFecha();
                        break;
                    case "3":
                        Console.Write("Nuevo motivo: ");
                        cita.Motivo = Console.ReadLine();
                        break;
                }
                Success("Cita actualizada.");
            }
        }



        static Paciente RegistrarNuevoPaciente()
        {
            Console.WriteLine("\n--- DATOS DEL NUEVO PACIENTE ---");
            Console.Write("Nombre completo: "); string nom = Console.ReadLine();
            Console.Write("Edad: "); int edad = int.Parse(Console.ReadLine());
            Console.Write("Teléfono: "); string tel = Console.ReadLine();
            Console.Write("Número de Expediente: "); string exp = Console.ReadLine();
            Console.Write("Tipo de Sangre: "); string sang = Console.ReadLine();

            Paciente nuevo = new Paciente(nom, edad, tel, exp, sang);
            pacientes.Add(nuevo);
            return nuevo;
        }

        static DateTime PedirFecha()
        {
            while (true)
            {
                Console.Write("Ingrese Fecha (DD/MM/AAAA HH:MM): ");
                if (DateTime.TryParseExact(Console.ReadLine(), "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime f))
                    return f;
                Error("Formato incorrecto. Use DD/MM/AAAA HH:MM");
            }
        }

        static void MostrarReporte()
        {
            Console.WriteLine("\n--- LISTADO DE CITAS ---");
            for (int i = 0; i < citas.Count; i++) citas[i].MostrarCita(i);
        }

        static void ModificarEstadoCita(bool confirmar)
        {
            MostrarReporte();
            Console.Write($"Índice para {(confirmar ? "CONFIRMAR" : "CANCELAR")}: ");
            if (int.TryParse(Console.ReadLine(), out int i) && i >= 0 && i < citas.Count)
            {
                if (confirmar) citas[i].ConfirmarCita(); else citas[i].CancelarCita();
                Success("Estado actualizado.");
            }
        }

        static void Error(string m) { Console.ForegroundColor = ConsoleColor.Red; Console.WriteLine($"\n[ERROR] {m}"); Console.ResetColor(); }
        static void Success(string m) { Console.ForegroundColor = ConsoleColor.Green; Console.WriteLine($"{m}"); Console.ResetColor(); }
    }
}
