using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MongoDB.Bson;
using MongoDB.Driver;

namespace IoTeus
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public IMongoDatabase Database { get; private set; }
        public bool Conectado { get; set; }

        private async void btnConectar_Click(object sender, EventArgs e)
        {
            if (!Conectado)

                try
                {
                    var client = new MongoClient(txtStringConexao.Text);
                    if(client != null)
                    {
                        lblStatusConexao.Text = "Conectado";
                        lblStatusConexao.BackColor = Color.Green;

                        Database = client.GetDatabase("IoTeus");
                        Conectado = true;

                        btnRefresh_Click(btnRefresh, e);

                        groupBoxMain.Enabled = true;
                        btnConectar.Enabled = false;
                    }
                    else
                    {
                        groupBoxMain.Enabled = false;
                        MessageBox.Show("Não foi possivel conectar. Tente novamente");
                    }

                }
                catch (Exception ex)
                {

                    groupBoxMain.Enabled = false;
                    MessageBox.Show("Não foi possivel conectar. Tente novamente. Codigo do erro: " + ex.Message);
                }
            

            
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            if (!Conectado) return;

            listBoxMaquinas.Items.Clear();
            var collection = Database.GetCollection<Maquina>("Maquina");

            listBoxMaquinas.Items.AddRange(
                collection
                    .Find(_ => true)
                    .ToList()
                    .ToArray()
                    );

        }

        private void listBoxMaquinas_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListBox lb = (ListBox) sender;

            Maquina maquina = (Maquina)lb.SelectedItem;

            if (maquina == null) return;

            txtNome.DataBindings.Clear();
            txtNome.DataBindings.Add("Text", maquina, nameof(maquina.Name));

            txtDiasOperacao.DataBindings.Clear();
            txtDiasOperacao.DataBindings.Add("Text", maquina, nameof(maquina.DiasOperacao));

            txtTipo.DataBindings.Clear();
            txtTipo.DataBindings.Add("Text", maquina, nameof(maquina.Tipo));

            txtIndex.DataBindings.Clear();
            txtIndex.DataBindings.Add("Text", maquina, nameof(maquina.Id));

            txtLocalizacao.DataBindings.Clear();
            txtLocalizacao.DataBindings.Add("Text", maquina, nameof(maquina.Localizacao));

            btnSalvar.Enabled = false;

        }

        private void btnAutoRefresh_Click(object sender, EventArgs e)
        {
            if (!btnAutoRefresh.Checked)
            {
                timerAutoRefresh.Enabled = true;
                btnAutoRefresh.Checked = true;
                btnRefresh_Click(btnRefresh, e);
            }
            else
            {
                timerAutoRefresh.Enabled = false;
                btnAutoRefresh.Checked = false;
            }

        }

        private void timerAutoRefresh_Tick(object sender, EventArgs e)
        {
            btnRefresh_Click(btnRefresh, e);
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            listBoxMaquinas.Enabled = false;
            toolStrip1.Enabled = false;
            txtTipo.DataBindings.Clear();
            txtNome.DataBindings.Clear();
            txtLocalizacao.DataBindings.Clear();
            txtIndex.DataBindings.Clear();
            txtDiasOperacao.DataBindings.Clear();

            txtTipo.Clear();
            txtNome.Clear();
            txtLocalizacao.Clear();
            txtIndex.Clear();
            txtDiasOperacao.Clear();

            btnSalvar.Enabled = true;
            btnCancelar.Enabled = true;
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (!Conectado) return;

            Maquina maquina = (Maquina)listBoxMaquinas.SelectedItem;

            var collection = Database.GetCollection<Maquina>("Maquina");

            collection.DeleteOne(a => a.Id == maquina.Id);

            txtTipo.Clear();
            txtNome.Clear();
            txtLocalizacao.Clear();
            txtIndex.Clear();
            txtDiasOperacao.Clear();

            txtTipo.DataBindings.Clear();
            txtNome.DataBindings.Clear();
            txtLocalizacao.DataBindings.Clear();
            txtIndex.DataBindings.Clear();
            txtDiasOperacao.DataBindings.Clear();
            btnSalvar.Enabled = false;
            listBoxMaquinas.Items.Remove(maquina);
            listBoxMaquinas.Refresh();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (!Conectado) return;

            Maquina maquina = (Maquina)listBoxMaquinas.SelectedItem;

            var collection = Database.GetCollection<Maquina>("Maquina");

            var filter = Builders<Maquina>.Filter.Eq(s => s.Id, maquina.Id);

            collection.ReplaceOne(filter, maquina);
        }

        private void btnSalvar_Click(object sender, EventArgs e)
        {
            if (!Conectado) return;

            var collection = Database.GetCollection<Maquina>("Maquina");

            var maquina = new Maquina
            {
                Name = txtNome.Text,
                DiasOperacao = txtDiasOperacao.Text,
                Localizacao = txtLocalizacao.Text,
                Tipo = txtTipo.Text,
                Id = ObjectId.GenerateNewId()
            };

            collection.InsertOne(maquina);
            listBoxMaquinas.Items.Add(maquina);
            listBoxMaquinas.Refresh();
            toolStrip1.Enabled = true;
            btnSalvar.Enabled = false;
            btnCancelar.Enabled = false;
            listBoxMaquinas.Enabled = true;
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            toolStrip1.Enabled = true;
            btnSalvar.Enabled = false;
            btnCancelar.Enabled = false;
            listBoxMaquinas.Enabled = true;

            listBoxMaquinas_SelectedIndexChanged(listBoxMaquinas, e);
        }
    }
}
