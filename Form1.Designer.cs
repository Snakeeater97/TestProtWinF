namespace TestProtWinF
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;
        private ListBox menuListBox; // Agrega esta línea para declarar el control menuListBox

        private void InitializeComponent()
        {
            menuListBox = new ListBox();
            SuspendLayout();
            // 
            // menuListBox
            // 
            menuListBox.ItemHeight = 15;
            menuListBox.Items.AddRange(new object[] { "Consultar disponibilidad de vuelos", "Consultar inventario de hotelería", "Consultar solicitudes de paquetes de clientes", "Consultar reservas", "Ver mi presupuesto actual", "Salir" });
            menuListBox.Location = new Point(250, 142);
            menuListBox.Name = "menuListBox";
            menuListBox.Size = new Size(259, 109);
            menuListBox.TabIndex = 0;
            menuListBox.SelectedIndexChanged += MenuListBox_SelectedIndexChanged;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(menuListBox);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load_1;
            ResumeLayout(false);
        }
    }
}
