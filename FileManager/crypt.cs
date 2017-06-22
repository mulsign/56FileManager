using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace FileManager
{
    class Crypt
    {
        private static string srcPath = Environment.CurrentDirectory + "\\Crypt";
        private string userName = Environment.UserName;
        private string computerName = Environment.MachineName;
        private string desktopDir = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

        private byte[] AES_Encrypt(byte[] bytesToBeEncryted, byte[] passwordBytes)
        {
            byte[] encryptedBytes = null;
            byte[] saltBytes = Encoding.UTF8.GetBytes(computerName);
            using (MemoryStream ms = new MemoryStream())
            {
                using (RijndaelManaged AES = new RijndaelManaged())
                {
                    AES.KeySize = 256;
                    AES.BlockSize = 128;

                    var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
                    AES.Key = key.GetBytes(AES.KeySize / 8);
                    AES.IV = key.GetBytes(AES.BlockSize / 8);

                    AES.Mode = CipherMode.CBC;

                    using (var cs = new CryptoStream(ms, AES.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytesToBeEncryted, 0, bytesToBeEncryted.Length);
                        cs.Close();
                    }
                    encryptedBytes = ms.ToArray();
                }
            }
            return encryptedBytes;
        }

        public string CreatePassword(int length)
        {
            const string valid = "qwertyuiopasdfghjklzxcvbnmQWERTYUIOPASDFGHJKLZXCVBNM7894561230*=!";
            StringBuilder res = new StringBuilder();
            var seed = Guid.NewGuid().GetHashCode();
            Random rnd = new Random(seed);
            while (0 < length--)
            {
                res.Append(valid[rnd.Next(valid.Length)]);
            }
            return res.ToString();
        }


        public void EncryptFile(string file)
        {
            string password = CreatePassword(20);
            byte[] bytesToBeEncrypted = File.ReadAllBytes(file);

            SQLiteHelper insertkey = new SQLiteHelper();

            insertkey.ConnectToDatabase(1);
            insertkey.InsertKeys(file, password);

            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

            passwordBytes = SHA256.Create().ComputeHash(passwordBytes);

            byte[] bytesEncrypted = AES_Encrypt(bytesToBeEncrypted, passwordBytes);

            File.WriteAllBytes(file, bytesEncrypted);
            File.Move(file, file + ".encrypted");
        }

        public void EncryptDirectory(string location)
        {

            string[] files = Directory.GetFiles(location);
            string[] childDirectory = Directory.GetDirectories(location);
            for (int i = 0; i < files.Length; i++)
            {
                EncryptFile(files[i]);
            }
            for (int i = 0; i < childDirectory.Length; i++)
            {
                EncryptDirectory(childDirectory[i]);
            }
        }

        private byte[] AES_Decrypt(byte[] bytesToBeDecrypted, byte[] passwordBytes)
        {
            byte[] decryptedBytes = null;
            byte[] saltBytes = Encoding.UTF8.GetBytes(computerName);

            using (MemoryStream ms = new MemoryStream())
            {
                using (RijndaelManaged AES = new RijndaelManaged())
                {

                    AES.KeySize = 256;
                    AES.BlockSize = 128;

                    var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
                    AES.Key = key.GetBytes(AES.KeySize / 8);
                    AES.IV = key.GetBytes(AES.BlockSize / 8);

                    AES.Mode = CipherMode.CBC;

                    using (var cs = new CryptoStream(ms, AES.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytesToBeDecrypted, 0, bytesToBeDecrypted.Length);
                        cs.Close();
                    }
                    decryptedBytes = ms.ToArray();

                }
            }
            return decryptedBytes;
        }

        public void DecryptFile(string file)
        {
            SQLiteHelper selectKey = new SQLiteHelper();
            string password = selectKey.SelectKeys(file);

            byte[] bytesToBeDecrypted = File.ReadAllBytes(file);
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            passwordBytes = SHA256.Create().ComputeHash(passwordBytes);

            byte[] bytesDecrypted = AES_Decrypt(bytesToBeDecrypted, passwordBytes);

            File.WriteAllBytes(file, bytesDecrypted);
            string extension = Path.GetExtension(file);
            string result = file.Substring(0, file.Length - extension.Length);
            File.Move(file, result);

        }

        public void DecryptDirectory(string location)
        {

            string[] files = Directory.GetFiles(location);
            string[] childDirectories = Directory.GetDirectories(location);
            for (int i = 0; i < files.Length; i++)
            {
                string extension = Path.GetExtension(files[i]);
                if (extension == ".encrypted")
                {
                    DecryptFile(files[i]);
                }
            }
            for (int i = 0; i < childDirectories.Length; i++)
            {
                DecryptDirectory(childDirectories[i]);
            }

        }
    }
}
