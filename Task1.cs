using System;
using System.IO;
using System.Security.Cryptography;
using System.Windows.Forms;

namespace FileEncryptionApp 
{
  public partial class Form1: Form 
  {
    public Form1() 
    {
      InitializeComponent();
    }

    private void encryptButton_Click(object sender, EventArgs e) 
    {
      string algorithm = algorithmComboBox.SelectedItem.ToString();
      string password = passwordTextBox.Text;

      if (string.IsNullOrWhiteSpace(password)) 
      {
        MessageBox.Show("Пожалуйста, введите пароль");
        return;
      }

      byte[] key = EncryptionHelper.GenerateKey(algorithm, password);
      byte[] iv = EncryptionHelper.GenerateIV(algorithm, password);

      using(SymmetricAlgorithm symmetricAlgorithm = EncryptionHelper.GetSymmetricAlgorithm(algorithm)) 
      {
        using(FileStream inputFileStream = new FileStream(fileTextBox.Text, FileMode.Open, FileAccess.Read)) 
        {
          using(FileStream outputFileStream = new FileStream(fileTextBox.Text + ".encrypted", FileMode.Create,
            FileAccess.Write)) 
            {
            using(ICryptoTransform encryptor = symmetricAlgorithm.CreateEncryptor(key, iv)) 
            {
              using(CryptoStream cryptoStream = new CryptoStream(outputFileStream, encryptor,
                CryptoStreamMode.Write))
              {
                inputFileStream.CopyTo(cryptoStream);
              }
            }
          }
        }
      }

      MessageBox.Show("Шифрование заверешно");

    }

    private void decryptButton_Click(object sender, EventArgs e) 
    {
      string algorithm = algorithmComboBox.SelectedItem.ToString();
      string password = passwordTextBox.Text;

      if (string.IsNullOrWhiteSpace(password)) 
      {
        MessageBox.Show("Пожалуйста, введите пароль");
        return;
      }

      byte[] key = EncryptionHelper.GenerateKey(algorithm, password);
      byte[] iv = EncryptionHelper.GenerateIV(algorithm, password);

      using(SymmetricAlgorithm symmetricAlgorithm = EncryptionHelper.GetSymmetricAlgorithm(algorithm)) 
      {
        using(FileStream inputFileStream = new FileStream(fileTextBox.Text, FileMode.Open, FileAccess.Read)) 
        {
          using(FileStream outputFileStream = new FileStream(fileTextBox.Text + ".decrypted", FileMode.Create, FileAccess.Write)) 
          {
            using(ICryptoTransform decryptor = symmetricAlgorithm.CreateDecryptor(key, iv)) 
            {
              using(CryptoStream cryptoStream = new CryptoStream(inputFileStream, decryptor,
                CryptoStreamMode.Read)) 
              {
                cryptoStream.CopyTo(outputFileStream);
              }
            }
          }
        }
      }

      MessageBox.Show("Расшифровка завершена");
    }

    private void selectFileButton_Click(object sender, EventArgs e) 
    {
      OpenFileDialog openFileDialog = new OpenFileDialog();

      if (openFileDialog.ShowDialog() == DialogResult.OK) 
      {
        fileTextBox.Text = openFileDialog.FileName;
      }
    }
  }

  public static class EncryptionHelper 
  {
    public static byte[] GenerateKey(string algorithm, string password) 
    {
      if (algorithm == "DES") 
      {
        return new PasswordDeriveBytes(password, null).GetBytes(8);
      } 
      else if (algorithm == "AES-128") 
      {
        return new PasswordDeriveBytes(password, null).GetBytes(16);
      } 
      else 
      {
        throw new ArgumentException("Неверный алгоритм");
      }
    }

    public static byte[] GenerateIV(string algorithm, string password) 
    {
      if (algorithm == "DES")
      {
        return new byte[8];
      }

      else if (algorithm == "AES-128") 
      {
        return new byte[16];
      } 
      else 
      {
        throw new ArgumentException("Неверный алгоритм");
      }
    }

    public static SymmetricAlgorithm GetSymmetricAlgorithm(string algorithm) 
    {
      if (algorithm == "DES") 
      {
        return DES.Create();
      } 
      else if (algorithm == "AES-128") 
      {
        return Aes.Create();
      } 
      else 
      {
        throw new ArgumentException("Неверный алгоритм ");
      }
    }
  }
}