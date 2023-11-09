using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;
using System.Xml.Linq;
using MySql.Data.MySqlClient;

namespace TrabalhoLinguagem2
{
    public partial class Form1 : Form
    {
        private MySqlConnection Conexao;
        private string data_source = "datasource=localhost;username=root;password=root;database=DB_AGENDA1";
        private MySqlDataReader reader;

        private int ?id_contato_selecioando = null;

        public Form1()
        {
            InitializeComponent();

            lstContatos.View = View.Details;
            lstContatos.LabelEdit = true;
            lstContatos.AllowColumnReorder = true;
            lstContatos.FullRowSelect = true;
            lstContatos.GridLines = true;

            lstContatos.Columns.Add("ID", 30, HorizontalAlignment.Left);
            lstContatos.Columns.Add("Nome", 150, HorizontalAlignment.Left);
            lstContatos.Columns.Add("Email", 150, HorizontalAlignment.Left);
            lstContatos.Columns.Add("Telefone", 150, HorizontalAlignment.Left);

            CarregarContatos();
        }

        private void btnSalvar_Click(object sender, EventArgs e)
        {
            try
            {
                Conexao = new MySqlConnection(data_source);
                Conexao.Open();
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = Conexao;

                if (id_contato_selecioando == null)
                {
                    //insert
                    cmd.CommandText = "INSERT INTO Contatos (Nome, Email, Telefone)" +
                                  " VALUES " +
                                  "(@Nome, @Email, @Telefone) ";

                    cmd.Parameters.AddWithValue("@Nome", txtNome.Text);
                    cmd.Parameters.AddWithValue("@Email", txtEmail.Text);
                    cmd.Parameters.AddWithValue("@Telefone", txtTelefone.Text);
                    cmd.Prepare();
                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Contato Inserido com Sucesso!",
                                    "Sucesso!",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);

                }
                else
                {
                    //update
                    cmd.CommandText = "UPDATE Contatos SET " +
                                      "Nome=@Nome, Email=@Email, Telefone=@Telefone " +
                                      "WHERE Id=@Id ";

                    cmd.Parameters.AddWithValue("@Nome", txtNome.Text);
                    cmd.Parameters.AddWithValue("@Email", txtEmail.Text);
                    cmd.Parameters.AddWithValue("@Telefone", txtTelefone.Text);
                    cmd.Parameters.AddWithValue("@Id", id_contato_selecioando);
                    cmd.Prepare();
                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Contato Atualizado com Sucesso!",
                                    "Sucesso!",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                }

                zerar_formulario();

                CarregarContatos();

            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Erro: " + ex.Number + " occoreu: " + ex.Message,
                                "Erro",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ocorreu: " + ex.Message,
                                "Erro",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
            finally
            {
                Conexao.Close();
            }
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            try 
            {
                Conexao = new MySqlConnection(data_source);
                Conexao.Open();

                MySqlCommand cmd = new MySqlCommand();

                cmd.Connection = Conexao;
                cmd.CommandText = "SELECT * FROM Contatos WHERE Nome LIKE @q OR Email LIKE @q ";
                cmd.Parameters.AddWithValue("@q", "%" + txtBuscar.Text + "%");
                cmd.Prepare();

                MySqlDataReader reader = cmd.ExecuteReader();

                lstContatos.Items.Clear();

                while(reader.Read())
                {
                    string[] row =
                    {
                        reader.GetString(0),
                        reader.GetString(1),
                        reader.GetString(2),
                        reader.GetString(3),
                    };

                    lstContatos.Items.Add(new ListViewItem(row));
                }

            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Erro: " + ex.Number + " occoreu: " + ex.Message,
                                "Erro",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ocorreu: " + ex.Message,
                                "Erro",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
            finally
            {
                Conexao.Close();
            }
        }

        private void CarregarContatos()
        {
            try
            {
                Conexao = new MySqlConnection(data_source);
                Conexao.Open();

                MySqlCommand cmd = new MySqlCommand();

                cmd.Connection = Conexao;
                cmd.CommandText = "SELECT * FROM Contatos ORDER BY Id DESC";
                cmd.Prepare();

                MySqlDataReader reader = cmd.ExecuteReader();

                lstContatos.Items.Clear();

                while (reader.Read())
                {
                    string[] row =
                    {
                        reader.GetString(0),
                        reader.GetString(1),
                        reader.GetString(2),
                        reader.GetString(3),
                    };

                    lstContatos.Items.Add(new ListViewItem(row));
                }

            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Erro: " + ex.Number + " occoreu: " + ex.Message,
                                "Erro",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ocorreu: " + ex.Message,
                                "Erro",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
            finally
            {
                Conexao.Close();
            }
        }

        private void lstContatos_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            ListView.SelectedListViewItemCollection itens_selecionados = lstContatos.SelectedItems;

            foreach(ListViewItem item in itens_selecionados)
            {
                id_contato_selecioando = Convert.ToInt32(item.SubItems[0].Text);
                txtNome.Text = item.SubItems[1].Text;
                txtEmail.Text = item.SubItems[2].Text;
                txtTelefone.Text = item.SubItems[3].Text;

                btnExcluir.Visible = true;
            }
        }

        private void btnNovo_Click(object sender, EventArgs e)
        {
            zerar_formulario();
        }

        private void zerar_formulario()
        {
            id_contato_selecioando = null;
            txtNome.Text = String.Empty;
            txtEmail.Text = "";
            txtTelefone.Text = "";
            txtNome.Focus();
            btnExcluir.Visible = false;
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            excluir_contato();
        }

        private void btnExcluir_Click(object sender, EventArgs e)
        {
            excluir_contato();
        }

        private void excluir_contato()
        {
            try
            {
                DialogResult conf = MessageBox.Show("Tem certeza que deseja excluir o registro?",
                                                    "Ops, tem erteza?",
                                                    MessageBoxButtons.YesNo,
                                                    MessageBoxIcon.Warning);

                if (conf == DialogResult.Yes)
                {
                    //delete
                    Conexao = new MySqlConnection(data_source);
                    Conexao.Open();
                    MySqlCommand cmd = new MySqlCommand();
                    cmd.Connection = Conexao;

                    cmd.CommandText = "DELETE FROM Contatos WHERE Id=@Id ";

                    cmd.Parameters.AddWithValue("@Id", id_contato_selecioando);
                    cmd.Prepare();
                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Contato Excluído com Sucesso!",
                                    "Sucesso!", MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);

                    CarregarContatos();

                    zerar_formulario();
                }

            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Erro: " + ex.Number + " occoreu: " + ex.Message,
                              "Erro",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Error);

            }
            catch (Exception ex)
            {
                MessageBox.Show("Ocorreu: " + ex.Message,
                              "Erro",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Error);

            }
            finally
            {
                Conexao.Close();
            }
        }
    }
}
