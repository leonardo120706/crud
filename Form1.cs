using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using MySql.Data.MySqlClient;


namespace cadastrodeclientes
{
    public partial class frmCadastroDeClientes : Form
    {

        //Conexão com o banco de dados MySQL
        MySqlConnection Conexao;
        string data_source = "datasource=localhost; username=root; password=; database=db_cadastro";

        private int ?codigo_cliente = null;

        public frmCadastroDeClientes()
        {
            InitializeComponent();

            //Configurando inicial do ListView par exebição dos dados dos clientes 
            lstCliente.View = View.Details;        //Define e vizualização de como "Detalhes"
            lstCliente.LabelEdit = true;           //Permite editar os títulos das colunas 
            lstCliente.AllowColumnReorder = true;  //Permite reodernar as colunas 
            lstCliente.FullRowSelect = true;       //Seleciona a linha inteira ao clicar 
            lstCliente.GridLines = true;           //Exibe as linhas de grande no ListView 


            //Definindo as colunas do ListView
            lstCliente.Columns.Add("Codigo", 100, HorizontalAlignment.Left); //Coluna de código
            lstCliente.Columns.Add("Nome Completo", 200, HorizontalAlignment.Left); //Coluna de Nome Completo
            lstCliente.Columns.Add("Nome Social", 200, HorizontalAlignment.Left); //Coluna de Nome Social
            lstCliente.Columns.Add("E-mail", 200, HorizontalAlignment.Left); //Coluna de E-mail
            lstCliente.Columns.Add("CPF", 200, HorizontalAlignment.Left); //Coluna de CPF

            //carrega os dados dos clientes na iterface
            carregar_clientes();
        }
        private void carregar_cliente_com_query(string query)
        {
            try
            {
                //cria a conexão com o banco de dados
                Conexao = new MySqlConnection(data_source);
                Conexao.Open();

                //Executaa consulta SQl fornecida
                MySqlCommand cmd = new MySqlCommand(query, Conexao);

                // Se a Consulta contém o parâmetro @q, adiciona o valor da caixa de pesquisa
                if (query.Contains("@q"))
                {
                    cmd.Parameters.AddWithValue("@q", "%" + txtBuscar.Text + "%");
                }

                //Executa o comando e obtém os resultados 
                MySqlDataReader reader = cmd.ExecuteReader();

                //Limpa os itens existentes no ListView antes de adicionar novos
                lstCliente.Items.Clear();


                //Preenche o listView com os dados dos clientes
                while (reader.Read())
                {
                    //Cria uma linha para cada cliente com os dados retornados da consulta
                    string[] row =
                    {
                        Convert.ToString(reader.GetInt32(0)), //codigo
                        reader.GetString(1),
                        reader.GetString(2),
                        reader.GetString(3),
                        reader.GetString(4)
                    };

                    //adicione a linha ao ListView
                    lstCliente.Items.Add(new ListViewItem(row));
                }

                
            }
            catch (MySqlException ex)
            {
                //Trata erros relacionados ao MySQL
                MessageBox.Show("Erro" + ex.Number + "ocorreu: " + ex.Message,
                    "Erro",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {

                //Trata outros tipos de erros
                MessageBox.Show("Ocorreu: " + ex.Message,
                    "Erro",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                //Garante que a conexão com o banco será fechada, mesmo se ocorrre erro
                if (Conexao != null && Conexao.State == ConnectionState.Open)
                {
                    Conexao.Close();
                    //MessageBox.Show("Conexão fechada com sucesso");teste de arbetura de banco                                                                
                }
            }

        }
        //método para carregar todos os clientes no ListView (usando uma consulta sem parâmetros)
        private void carregar_clientes()
        {
            string query = "SELECT * FROM dadosdecliente ORDER BY codigo DESC";
            carregar_cliente_com_query(query);
        }
        
        private bool isValidEmail(string email)
        {
            // Validaçao Regex
            string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            Regex regex = new Regex(pattern);
            return regex.IsMatch(email);
        }
        

        private bool isValidCPFlegth(string cpf)
        {
            //remover quaisquer caracteres não númericos (como pontos e traços)
            cpf = cpf.Replace(".", "").Replace("-", "");
            
            //Verificar se o CPF tem exatamente 11 caracteres numéricos
            if (cpf.Length !=11 || !cpf.All(char.IsDigit))
            {
                return false;
            }
            return true;
        }


        private void btnSalvar_Click(object sender, EventArgs e)
        {
            try
            {
                //Validação de campos obrigatorios
                if (string.IsNullOrEmpty(txtNomeCompleto.Text.Trim() )||
                    string.IsNullOrEmpty(txtEmail.Text.Trim()) ||
                    string.IsNullOrEmpty(txtCPF.Text.Trim()))
                {
                    MessageBox.Show("todos os campos deveria ser preenchido.",
                                     "Validação",
                                     MessageBoxButtons.OK,
                                     MessageBoxIcon.Warning);
                    return; //Impede o prosseguimento se algum campo estiver vazio
                }

                // Validacão do e-mail
                string email = txtEmail.Text.Trim();
                if (!isValidEmail(email))
                {
                    MessageBox.Show("E-mail inválido. Certfique-se de que o e-mail está no formato correto.",
                        "validação",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return; // Impede o prosseguimento se o e-mail for inválido
                }




                //Validação do CPF
                string cpf = txtCPF.Text.Trim();
                if (!isValidCPFlegth(cpf)){
                    MessageBox.Show("CPF invalido. Certifique-se de que o CPF tenha 11 dígitos númericos",
                                     "Validação",
                                      MessageBoxButtons.OK,
                                      MessageBoxIcon.Warning);
                    return;
                };

                //Cria a conexão com o banco de dados
                Conexao = new MySqlConnection(data_source);
                Conexao.Open();

                //MessageBox.Show("Conexão aberta com sucesso"); teste de arbetura de banco
                

                //comeando SQL para insirir um novo cliente no banco de dados

                MySqlCommand cmd = new MySqlCommand
                {
                    Connection = Conexao
                };

                cmd.Prepare();
                if(codigo_cliente == null)
                {
                    //insert CREATE
                    cmd.CommandText = "INSERT INTO dadosdecliente(nomecompleto, nomesocial, email, cpf)" +
                   "VAlUES (@nomecompleto, @nomesocial, @email, @cpf )";


                    //adiciona parâmetros com os dados do formulário

                    cmd.Parameters.AddWithValue("@nomecompleto", txtNomeCompleto.Text.Trim());
                    cmd.Parameters.AddWithValue("@nomesocial", txtNomeSocial.Text.Trim());
                    cmd.Parameters.AddWithValue("@email", email);
                    cmd.Parameters.AddWithValue("@cpf", cpf);

                    //Executa um comando de Inserção no banco
                    cmd.ExecuteNonQuery();

                    //Menssagem de sucesso
                    MessageBox.Show("Contato Inserido com Sucesso: ",
                        "Sucesso",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
                else
                {
                    //UPDATE
                    cmd.CommandText = $"UPDATE `dadosdecliente` SET " +
                        $"nomecompleto = @nomecompleto, " +
                        $"nomesocial = @nomesocial, " +
                        $"email = @email, " +
                        $"cpf =  @cpf " +
                        $"WHERE codigo = @codigo"; //WHERE(onde) o codigo for ihual ao cliente no banco de dados

                    cmd.Parameters.AddWithValue("@nomecompleto", txtNomeCompleto.Text.Trim());
                    cmd.Parameters.AddWithValue("@nomesocial", txtNomeSocial.Text.Trim());
                    cmd.Parameters.AddWithValue("@email", email);
                    cmd.Parameters.AddWithValue("@cpf", cpf);
                    cmd.Parameters.AddWithValue("@codigo", codigo_cliente);

                    //Execute o comando de alteração no Banco
                    cmd.ExecuteNonQuery();

                    //Mensagem de sucesso para dados de atualização
                    MessageBox.Show($"Os dados com o código {codigo_cliente} foram alterados com Sucesso!",
                        "Sucesso", 
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                }

                codigo_cliente = null;

                //Limpa os campos após os sucesso 
                txtNomeCompleto.Text = string.Empty; //mesma coisa de escrever string . '''';
                txtNomeSocial.Text = "";
                txtEmail.Text = "";
                txtCPF.Text = "";

                //Recarregar os cliente na ListView
                carregar_clientes(); 

                //Muda para a aba de pesquisa
                tabControl1.SelectedIndex = 1;
            }
            catch(MySqlException ex)
            {
                //Trata erros relacionados ao MySQL
                MessageBox.Show("Erro" + ex.Number + "ocorreu: " + ex.Message,
                    "Erro",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {

                //Trata outros tipos de erros
                MessageBox.Show("Ocorreu: " + ex.Message,
                    "Erro",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                //Garante que a conexão com o banco será fechada, mesmo se ocorrre erro
                if(Conexao != null && Conexao.State == ConnectionState.Open)
                {
                    Conexao.Close();
                    //MessageBox.Show("Conexão fechada com sucesso");teste de arbetura de banco                                                                
                }
            }
        }

        private void btnPesquisar_Click(object sender, EventArgs e)
        {
            string query = "SELECT * FROM dadosdecliente WHERE nomecompleto LIKE @q OR nomesocial LIKE @q ORDER BY codigo DESC";
            carregar_cliente_com_query(query);
        }

        private void lstCliente_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            ListView.SelectedListViewItemCollection clientedaselecao = lstCliente.SelectedItems;

            foreach(ListViewItem item in clientedaselecao)
            {
                codigo_cliente = Convert.ToInt32(item.SubItems[0].Text);

                //Exibe uma MessageBox código do cliente
                MessageBox.Show("Código do cliente: " + codigo_cliente.ToString(),
                    "Código Selecionado",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                txtNomeCompleto.Text = item.SubItems[1].Text;
                txtNomeSocial.Text = item.SubItems[2].Text;
                txtEmail.Text = item.SubItems[3].Text;
                txtCPF.Text = item.SubItems[4].Text;

            }
            // Muda para a aba de dados do cliente
            tabControl1.SelectedIndex = 0; 
        }

        private void btnNovoCliente_Click(object sender, EventArgs e)
        {
            codigo_cliente = null;

            txtNomeCompleto.Text = string.Empty; //mesma coisa de escrever string . "";
            txtNomeSocial.Text = "";
            txtEmail.Text = "";
            txtCPF.Text = "";

            txtNomeCompleto.Focus();
        }
    }
}
