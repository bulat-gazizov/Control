using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace QubitControl
{
    public static class Settings
    {

        public static string btURL
        {
            get
            {
                return Properties.Settings.Default.btURL;
            }
            set
            {
                Properties.Settings.Default.btURL = value;
                Properties.Settings.Default.Save();
            }
        }
        public static int btPort
        {
            get
            {
                return Properties.Settings.Default.btPort;
            }
            set
            {
                Properties.Settings.Default.btPort = value;
                Properties.Settings.Default.Save();
            }
        }
        public static Uri btCompleteURL
        {
            get
            {
                Uri uri = new Uri(btURL);
                if(uri.Port == btPort) return uri;
                string FullURL;
                if (uri.Scheme == "")
                    FullURL = "http://";
                else
                    FullURL = uri.Scheme + "://";

                FullURL = FullURL + uri.Host;
                FullURL = FullURL + ":" + btPort.ToString();
                FullURL = FullURL + uri.AbsolutePath;
                uri = new Uri(FullURL);

                return uri;
            }
        }


        public static bool btRequireAuth
        {
            get
            {
                return Properties.Settings.Default.btRequireAuth;
            }
            set
            {
                Properties.Settings.Default.btRequireAuth = value;
                Properties.Settings.Default.Save();
            }
        }
        public static string btUserName
        {
            get
            {
                return Properties.Settings.Default.btUserName;
            }
            set
            {
                Properties.Settings.Default.btUserName = value;
                Properties.Settings.Default.Save();
            }
        }
        public static string btPassword
        {
            get
            {
                return Decrypt(Properties.Settings.Default.btPassword, EncryptionKey());
            }
            set
            {
                Properties.Settings.Default.btPassword = Encrypt(value, EncryptionKey());
                Properties.Settings.Default.Save();
            }
        }

        private static string DecryptPassword(string password)
        {
            return Decrypt(password, EncryptionKey());
        }
        private static string EncryptionKey()
        {
            string key = "";
            Guid? g = System.DirectoryServices.AccountManagement.UserPrincipal.Current.Guid;
            if (g != null)
                key = g.ToString();

            key = key + System.DirectoryServices.AccountManagement.UserPrincipal.Current.Sid.ToString();
            return key.Replace("-","") ;
        }

        private static string Encrypt(string strToEncrypt, string strKey)
        {
            try
            {
                TripleDESCryptoServiceProvider objDESCrypto =
                    new TripleDESCryptoServiceProvider();
                MD5CryptoServiceProvider objHashMD5 = new MD5CryptoServiceProvider();
                byte[] byteHash, byteBuff;
                string strTempKey = strKey;
                byteHash = objHashMD5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(strTempKey));
                objHashMD5 = null;
                objDESCrypto.Key = byteHash;
                objDESCrypto.Mode = CipherMode.ECB; //CBC, CFB
                byteBuff = ASCIIEncoding.ASCII.GetBytes(strToEncrypt);
                return Convert.ToBase64String(objDESCrypto.CreateEncryptor().
                    TransformFinalBlock(byteBuff, 0, byteBuff.Length));
            }
            catch (Exception ex)
            {
                return "Wrong Input. " + ex.Message;
            }
        }

        /// <summary>
        /// Decrypt the given string using the specified key.
        /// </summary>
        /// <param name="strEncrypted">The string to be decrypted.</param>
        /// <param name="strKey">The decryption key.</param>
        /// <returns>The decrypted string.</returns>
        private static string Decrypt(string strEncrypted, string strKey)
        {
            try
            {
                TripleDESCryptoServiceProvider objDESCrypto =
                    new TripleDESCryptoServiceProvider();
                MD5CryptoServiceProvider objHashMD5 = new MD5CryptoServiceProvider();
                byte[] byteHash, byteBuff;
                string strTempKey = strKey;
                byteHash = objHashMD5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(strTempKey));
                objHashMD5 = null;
                objDESCrypto.Key = byteHash;
                objDESCrypto.Mode = CipherMode.ECB; //CBC, CFB
                byteBuff = Convert.FromBase64String(strEncrypted);
                string strDecrypted = ASCIIEncoding.ASCII.GetString
                (objDESCrypto.CreateDecryptor().TransformFinalBlock
                (byteBuff, 0, byteBuff.Length));
                objDESCrypto = null;
                return strDecrypted;
            }
            catch (Exception ex)
            {
                return "Wrong Input. " + ex.Message;
            }
        }


    }
}
