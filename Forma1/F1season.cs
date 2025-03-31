using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace Forma1
{
    public partial class F1season : Form
    {
        private string connectionString = "server=localhost;database=f1_pilotak;uid=root;pwd=;";

        public F1season()
        {
            InitializeComponent();
            LoadPilotData();
            LoadTeamPoints();
        }

        private void LoadPilotData()
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT nev, csapatnev, pontszam FROM pilotak";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataReader reader = cmd.ExecuteReader();
                listBoxPilota.Items.Clear();
                while (reader.Read())
                {
                    listBoxPilota.Items.Add(reader["nev"].ToString() + ": " +reader["csapatnev"].ToString() + ": " + reader["pontszam"].ToString());
                }
            }
        }
        private void LoadTeamPoints()
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT csapatnev, SUM(pontszam) as osszpont FROM pilotak GROUP BY csapatnev";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataReader reader = cmd.ExecuteReader();
                listBoxCsapat.Items.Clear();
                while (reader.Read())
                {
                    listBoxCsapat.Items.Add(reader["csapatnev"].ToString() + ": " + reader["osszpont"].ToString());
                }
            }
        }    

        private void btnAdd_Click(object sender, EventArgs e)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "INSERT INTO pilotak (nev, csapatnev, pontszam) VALUES (@nev, @csapatnev, @pontszam)";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@nev", textBoxName.Text);
                cmd.Parameters.AddWithValue("@csapatnev", textBoxTeam.Text);
                cmd.Parameters.AddWithValue("@pontszam", int.Parse(textBoxPoint.Text));
                cmd.ExecuteNonQuery();
            }
            LoadPilotData();
            LoadTeamPoints();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "UPDATE pilotak SET pontszam = pontszam + @pontszam WHERE nev = @nev";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@nev", textBoxName.Text);
                cmd.Parameters.AddWithValue("@pontszam", int.Parse(textBoxPoint.Text));
                cmd.ExecuteNonQuery();
            }
            LoadPilotData();
            LoadTeamPoints();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBoxName.Text))
            {
                MessageBox.Show("Kérlek, adj meg egy pilóta nevet a törléshez!");
                return;
            }

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "DELETE FROM pilotak WHERE nev = @nev";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@nev", textBoxName.Text);
                cmd.ExecuteNonQuery();
            }

            // Csak akkor próbáljuk meg törölni, ha van kiválasztott elem
            if (listBoxPilota.SelectedItem != null)
            {
                listBoxPilota.Items.Remove(listBoxPilota.SelectedItem);
            }

            if (listBoxCsapat.SelectedItem != null)
            {
                listBoxCsapat.Items.Remove(listBoxCsapat.SelectedItem);
            }

            LoadPilotData();
            LoadTeamPoints();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            textBoxName.Text = "";
            textBoxTeam.Text = "";
            textBoxPoint.Text = "";            
            textBoxName.Focus();
        }
    }
}
