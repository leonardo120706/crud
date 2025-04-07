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

        public frmCadastroDeClientes()
        {
            InitializeComponent();
        }
        // Validaçao Regex
        private bool isValidEmail(string email)
        {
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

                cmd.CommandText = "INSERT INTO dadosdecliente(nomecompleto, nomesocial, email, cpf)" +
                    "VAlUES (@nomecompleto, @nomesocial, @email, @cpf )";




                //adiciona parâmetroscom os dados do formulário

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
    }
}
