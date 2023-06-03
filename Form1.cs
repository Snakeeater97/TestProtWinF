using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using Newtonsoft.Json;
using System.Globalization;

namespace TestProtWinF
{
    public partial class Form1 : Form
    {
        private List<Producto> presupuesto;

        public Form1()
        {
            InitializeComponent();
            presupuesto = new List<Producto>();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Paso 1: Mostrar el menú de ventas
            AgregarEtiqueta("Menú de Ventas", 10, 10);
            AgregarEtiqueta("[1] Consultar disponibilidad de vuelos", 10, 40);
            AgregarEtiqueta("[2] Consultar inventario de hotelería", 10, 60);
            AgregarEtiqueta("[3] Consultar solicitudes de paquetes de clientes", 10, 80);
            AgregarEtiqueta("[4] Consultar reservas", 10, 100);
            AgregarEtiqueta("[5] Salir", 10, 120);

            menuListBox = new ListBox();
            menuListBox.Location = new Point(10, 150);
            menuListBox.Size = new Size(200, 100);
            menuListBox.SelectedIndexChanged += MenuListBox_SelectedIndexChanged;
            Controls.Add(menuListBox);

            // Agregar opciones al ListBox del menú
            menuListBox.Items.Add("[1] Consultar disponibilidad de vuelos");
            menuListBox.Items.Add("[2] Consultar inventario de hotelería");
            menuListBox.Items.Add("[3] Consultar solicitudes de paquetes de clientes");
            menuListBox.Items.Add("[4] Consultar reservas");
            menuListBox.Items.Add("[5] Ver mi presupuesto actual");
            menuListBox.Items.Add("[6] Salir");
        }

        private void MenuListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            int opcionSeleccionada = menuListBox.SelectedIndex + 1;

            switch (opcionSeleccionada)
            {
                case 1:
                    ConsultarDisponibilidadVuelos();
                    break;
                case 2:
                    ConsultarInventarioHoteleria();
                    break;
                case 5:
                    VerPresupuestoActual();
                    break;
                case 6:
                    Application.Exit();
                    break;
                default:
                    // Opción inválida seleccionada
                    MessageBox.Show("Opción inválida");
                    menuListBox.SelectedIndex = -1;
                    break;
            }
        }
        private ListBox vuelosListBox;
        

        private List<string> presupuestoVuelos = new List<string>();

        private void ConsultarDisponibilidadVuelos()
        {
            LimpiarFormulario();

            AgregarEtiqueta("Origen:", 10, 10);
            TextBox origenTextBox = AgregarTextBox(10, 30);
            AgregarEtiqueta("Destino:", 10, 60);
            TextBox destinoTextBox = AgregarTextBox(10, 80);
            AgregarEtiqueta("Fecha ida:", 10, 110);
            DateTimePicker fechaIdaPicker = AgregarDateTimePicker(10, 130);
            AgregarEtiqueta("Fecha vuelta (opcional):", 10, 160);
            DateTimePicker fechaVueltaPicker = AgregarDateTimePicker(10, 180);
            AgregarEtiqueta("Cantidad de pasajeros:", 10, 210);
            NumericUpDown pasajerosNumericUpDown = AgregarNumericUpDown(10, 230);
            vuelosListBox = new ListBox();
            vuelosListBox.Location = new Point(10, 290);
            vuelosListBox.Size = new Size(400, 150);
            Controls.Add(vuelosListBox);

            Button buscarButton = AgregarBoton("Buscar", 10, 260);
            buscarButton.Click += (sender, e) =>
            {
                string origen = origenTextBox.Text;
                string destino = destinoTextBox.Text;
                DateTime fechaIda = fechaIdaPicker.Value;
                DateTime? fechaVuelta = fechaVueltaPicker.Checked ? fechaVueltaPicker.Value : null;
                int cantidadPasajeros = (int)pasajerosNumericUpDown.Value;

                // Cargar los vuelos desde el archivo JSON
                string rutaArchivo = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "VuelosDisponibles.json");
                List<Dictionary<string, string>> vuelosDisponibles = new List<Dictionary<string, string>>();

                try
                {
                    string contenidoJson = File.ReadAllText(rutaArchivo);
                    vuelosDisponibles = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(contenidoJson);
                }
                catch (FileNotFoundException ex)
                {
                    MessageBox.Show("No se encontró el archivo 'VuelosDisponibles.json'. Verifica la ubicación y el nombre del archivo.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al leer el archivo 'VuelosDisponibles.json'. Detalles del error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Filtrar los vuelos según las fechas de ida y vuelta, origen y destino
                List<Dictionary<string, string>> vuelosFiltrados = vuelosDisponibles.Where(vuelo =>
                {
                    DateTime fechaIdaVuelo = DateTime.ParseExact(vuelo["Ida"], "d/M/yyyy", CultureInfo.InvariantCulture);
                    DateTime fechaVueltaVuelo = DateTime.ParseExact(vuelo["Vuelta"], "d/M/yyyy", CultureInfo.InvariantCulture);

                    return fechaIda <= fechaIdaVuelo &&
                        (!fechaVuelta.HasValue || fechaVuelta >= fechaVueltaVuelo) &&
                        vuelo["Origen"] == origen &&
                        vuelo["Destino"] == destino;
                }).ToList();


                // Mostrar los resultados en la interfaz de usuario
                vuelosListBox.Items.Clear();

                foreach (var vuelo in vuelosFiltrados)
                {
                    string origenVuelo = vuelo["Origen"];
                    string destinoVuelo = vuelo["Destino"];
                    string fechaIdaVuelo = vuelo["Ida"];
                    string fechaVueltaVuelo = vuelo["Vuelta"];

                    string vueloStr = $"{origenVuelo} - {destinoVuelo} | Ida: {fechaIdaVuelo} | Vuelta: {fechaVueltaVuelo}";
                    vuelosListBox.Items.Add(vueloStr);
                }
            };

            Button agregarPresupuestoButton = AgregarBoton("Agregar al presupuesto", 10, 450);
            agregarPresupuestoButton.Click += (sender, e) =>
            {
                if (vuelosListBox.SelectedItem != null)
                {
                    string vueloSeleccionado = vuelosListBox.SelectedItem.ToString();
                    presupuestoVuelos.Add(vueloSeleccionado);
                    MessageBox.Show($"Vuelo '{vueloSeleccionado}' agregado al presupuesto");
                }
            };

            //Button volverButton = AgregarBoton("Volver al menú principal", 100, 260);
            //volverButton.Click += VolverButton_Click; arreglar este boton choto
        }


        private void ConsultarInventarioHoteleria()
        {
            LimpiarFormulario();

            AgregarEtiqueta("Destino:", 10, 10);
            TextBox destinoTextBox = AgregarTextBox(10, 30);
            AgregarEtiqueta("Fecha check-in:", 10, 60);
            DateTimePicker checkInPicker = AgregarDateTimePicker(10, 80);
            AgregarEtiqueta("Fecha check-out:", 10, 110);
            DateTimePicker checkOutPicker = AgregarDateTimePicker(10, 130);
            AgregarEtiqueta("Cantidad de huéspedes:", 10, 160);
            NumericUpDown huespedesNumericUpDown = AgregarNumericUpDown(10, 180);

            Button buscarButton = AgregarBoton("Buscar", 10, 210);
            buscarButton.Click += (sender, e) =>
            {
                // Lógica de búsqueda de hoteles
                string destino = destinoTextBox.Text;
                DateTime checkIn = checkInPicker.Value;
                DateTime checkOut = checkOutPicker.Value;
                int cantidadHuespedes = (int)huespedesNumericUpDown.Value;

                // Aquí puedes implementar la lógica para buscar hoteles
                // y mostrar los resultados en la interfaz de usuario
            };

            Button volverButton = AgregarBoton("Volver al menú principal", 120, 210);
            volverButton.Click += (sender, e) =>
            {
                menuListBox.SelectedIndex = -1;
            };
        }

        private void VerPresupuestoActual()
        {
            LimpiarFormulario();

            if (presupuesto.Count == 0)
            {
                MessageBox.Show("El presupuesto está vacío");
                menuListBox.SelectedIndex = -1;
                return;
            }

            AgregarEtiqueta("Presupuesto actual:", 10, 10);

            ListBox presupuestoListBox = new ListBox();
            presupuestoListBox.Location = new Point(10, 30);
            presupuestoListBox.Size = new Size(200, 150);
            presupuestoListBox.DisplayMember = "Nombre";
            presupuestoListBox.DataSource = presupuesto;
            Controls.Add(presupuestoListBox);

            Button eliminarButton = AgregarBoton("Eliminar producto", 10, 190);
            eliminarButton.Click += (sender, e) =>
            {
                if (presupuestoListBox.SelectedIndex >= 0)
                {
                    presupuesto.RemoveAt(presupuestoListBox.SelectedIndex);
                    presupuestoListBox.DataSource = null;
                    presupuestoListBox.DataSource = presupuesto;
                }
            };

            Button confirmarButton = AgregarBoton("Confirmar pre-reserva", 120, 190);
            confirmarButton.Click += (sender, e) =>
            {
                // Lógica para confirmar pre-reserva
                MessageBox.Show("Pre-reserva confirmada");
            };

            Button volverButton = AgregarBoton("Volver al menú principal", 10, 220);
            volverButton.Click += (sender, e) =>
            {
                menuListBox.SelectedIndex = -1;
            };
        }

        private void AgregarEtiqueta(string texto, int x, int y)
        {
            Label etiqueta = new Label();
            etiqueta.Text = texto;
            etiqueta.Location = new Point(x, y);
            Controls.Add(etiqueta);
        }

        private TextBox AgregarTextBox(int x, int y)
        {
            TextBox textBox = new TextBox();
            textBox.Location = new Point(x, y);
            textBox.Size = new Size(150, 20);
            Controls.Add(textBox);
            return textBox;
        }

        private DateTimePicker AgregarDateTimePicker(int x, int y)
        {
            DateTimePicker dateTimePicker = new DateTimePicker();
            dateTimePicker.Location = new Point(x, y);
            Controls.Add(dateTimePicker);
            return dateTimePicker;
        }

        private NumericUpDown AgregarNumericUpDown(int x, int y)
        {
            NumericUpDown numericUpDown = new NumericUpDown();
            numericUpDown.Location = new Point(x, y);
            numericUpDown.Size = new Size(50, 20);
            Controls.Add(numericUpDown);
            return numericUpDown;
        }

        private Button AgregarBoton(string texto, int x, int y)
        {
            Button boton = new Button();
            boton.Text = texto;
            boton.Location = new Point(x, y);
            Controls.Add(boton);
            return boton;
        }

        private void LimpiarFormulario()
        {
            foreach (Control control in Controls)
            {
                if (control is TextBox textBox)
                {
                    textBox.Text = string.Empty;
                }
                else if (control is ListBox listBox)
                {
                    listBox.Items.Clear();
                }
                else if (control is DateTimePicker dateTimePicker)
                {
                    dateTimePicker.Value = DateTime.Today;
                }
                else if (control is NumericUpDown numericUpDown)
                {
                    numericUpDown.Value = 0;
                }
            }
        }

        private void Form1_Load_1(object sender, EventArgs e)
        {

        }
    }

    public class Producto
    {
        public string Nombre { get; set; }
        public decimal Precio { get; set; }
    }
}
