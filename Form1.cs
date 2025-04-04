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

namespace cadastrodeclientes
{
    public partial class frmCadastroDeClientes : Form
    {
        public frmCadastroDeClientes()
        {
            InitializeComponent();
        }
        // Validaçao Regex
        private bool isValidEmail(string email)
        {
            string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z{2,}$";
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
                };
                    

            }
            catch (Exception ex)
            {

                //Trata outros tipos de erros
                MessageBox.Show("Ocorreu: " + ex.Message,
                    "Erro",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
    }
}
