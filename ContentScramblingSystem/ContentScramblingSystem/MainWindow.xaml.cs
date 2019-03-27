using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.Diagnostics;
using System.Drawing;
using System.Text;

namespace ContentScramblingSystem
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string fileName;
        private string fileText;
        private string fileBytes;
        private string resultText;
        const int a = 17;
        const int b = 15;
        private List<String> allPossibilities;
        private byte[] plainText;
        private byte[] encryptedText;
        private byte[] decryptedText;
        private byte[] cssBytes;
        private List<int> defaultTopThree;
        private List<int> topThreeBytes;
        private string _byteStream = "";

        public MainWindow()
        {
            InitializeComponent();
            fileName = "";
            fileText = "";
            resultText = "";
            allPossibilities = new List<String>();
            //plainText = new List<byte>();
            //encryptedText = new List<byte>();
            //decryptedText = new List<byte>();

            //32, 101, 116
            //defaultTopThree = new byte[] { Convert.ToByte(32),   Convert.ToByte(101),  Convert.ToByte(116) };
            this.topThreeBytes = new List<int>();
        }

        private void readText()
        {
            this.fileText = File.ReadAllText(fileName);
            this.lblFileText.Content = fileText.ToString();

            this.fileBytes = File.ReadAllBytes(fileName).ToString();
            //foreach(byte b in asciiBytes)
            //{
            //    if (Encoding.ASCII.GetString(asciiBytes).ElementAt(count) == 'i') Debug.WriteLine(b);
            //    //Debug.WriteLine(b);
            //    this.plainText.Add(b);
            //    count++;
            //}

            //this.plainText = this.fileBytes;
            byte[] fBytes = File.ReadAllBytes(fileName);
            StringBuilder sb = new StringBuilder();

            foreach (byte b in fBytes)
            {
                sb.Append(Convert.ToString(b, 2).PadLeft(8, '0'));
            }

            Debug.WriteLine(sb);

            Debug.WriteLine(File.ReadAllBytes(fileName));

            this.plainText = fBytes;
            this.fileBytes = sb.ToString();

            this.encryptedText = this.plainText;
        }

        private void BtnSelectFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();

            dlg.DefaultExt = ".tsp";
            dlg.Filter = "txt files (*.txt)|*.txt";

            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
            {
                string fileName = dlg.FileName;
                Debug.WriteLine(fileName);
                this.fileName = fileName;
                readText();
            }
        }

        private void encrypt()
        {
            this.resultText = "";

            CSSByteStream();

            char[] plainBytes = this.fileBytes.ToCharArray();
            char[] bitmask = this._byteStream.ToCharArray();
            char[] outputBitStream = new char[this.fileBytes.Length + 100];
            for (int i = 0; i < this.fileBytes.Length; i++)
            {
                int term1 = Convert.ToInt32(plainBytes[i]);
                if (i < 40)
                {
                    outputBitStream[i] = (char)(term1 ^ (int)bitmask[i]);
                } else
                {
                    outputBitStream[i] = (char)(term1 ^ (int)bitmask[39]);
                }
            }

            this.resultText = new string(outputBitStream);

            string fileDirectory = System.IO.Path.GetDirectoryName(this.fileName);
            this.lblFileText.Content = this.resultText.ToString();
            this.fileText = this.resultText;

            using (BinaryWriter outputFile = new BinaryWriter(File.Open(System.IO.Path.Combine(fileDirectory, "decrypted.txt"), FileMode.Open)))
            {
                foreach (char c in this.resultText)
                {
                    outputFile.Write(c);
                }
            }
        }

        private void decrypt()
        {
            Debug.WriteLine("\nEntering Decrypt");

            char[] encryptedBytes = this.fileText.ToCharArray();
            char[] bitmask = this._byteStream.ToCharArray();
            char[] outputBitStream = new char[this.fileBytes.Length + 100];
            for (int i = 0; i < this.fileBytes.Length; i++)
            {
                int term1 = Convert.ToInt32(encryptedBytes[i]);
                if (i < 40)
                {
                    outputBitStream[i] = (char)(term1 ^ (int)bitmask[i]);
                }
                else
                {
                    outputBitStream[i] = (char)(term1 ^ (int)bitmask[39]);
                }
            }
            string result = new String(outputBitStream);
            var data = GetBytesFromBinaryString(result);
            var text = Encoding.ASCII.GetString(data);
            this.resultText = text;
            

            string fileDirectory = System.IO.Path.GetDirectoryName(this.fileName);
            this.lblFileText.Content = this.resultText.ToString();
            string newFileName = fileDirectory + "\\output.txt";

            using (BinaryWriter outputFile = new BinaryWriter(File.Open(System.IO.Path.Combine(fileDirectory, "decrypted.txt"), FileMode.Open)))
            {
                foreach (char c in this.resultText)
                {
                    outputFile.Write(c);
                }
            }
        }

        private void decrypt(int _a, int _b)
        {
            this.resultText = "";
            byte[] decryptedBytes = new byte[this.encryptedText.Length];

            this.resultText = "";
            this.resultText = Encoding.ASCII.GetString(decryptedBytes.ToArray(), 0, decryptedBytes.Count());

            string result = "(" + _a + "," + _b + "): " + this.resultText;
            this.allPossibilities.Add(result);
            this.lblFileText.Content = this.resultText.ToString();
        }

        private void CSSByteStream()
        {
            string sKey = "crypt";
            //Debug.WriteLine(sKey + " is " + ASCIIEncoding.ASCII.GetByteCount(sKey).ToString() + " bytes");
            byte[] bKey = Encoding.ASCII.GetBytes(sKey);
            StringBuilder sb = new StringBuilder();

            int bitLength = 0;
            foreach (byte b in bKey)
            {
                sb.Append(Convert.ToString(b, 2).PadLeft(8, '0'));
            }
            foreach(char c in sb.ToString())
            {
                bitLength++;
            }

            Debug.WriteLine(sb);
            Debug.WriteLine(bitLength + " bits"); //verified bit length as 40  

            char[] bits;
            List<char> listBits = new List<char>();
            foreach (char c in sb.ToString())
            {
                listBits.Add(c);
            }
            bits = listBits.ToArray();

            string R1 = "";
            string R2 = "";

            //R1 is the first byte in bKey + the the next 5 bits + '1' + the next 3 bits
            //R1 and R2 should be created using ranges in the array, but this let me figure out what I needed where faster than implementing a splice method and using it
            R1 = Convert.ToString(bKey[0], 2).PadLeft(8, '0') + bits[8] + bits[9] + bits[10] + bits[11] + bits[12] + '1' + bits[13] + bits[14] + bits[15];
            Debug.WriteLine(R1);
            R2 = Convert.ToString(bKey[2], 2).PadLeft(8, '0') + Convert.ToString(bKey[3], 2).PadLeft(8, '0') + bits[32] + bits[33] + bits[34] + bits[35] + bits[36] + '1' + bits[37] + bits[38] + bits[39];
            Debug.WriteLine(R2);

            char[] cR1 = R1.ToCharArray();
            char[] cR2 = R2.ToCharArray();

            char[] x = new char[8];
            char[] y = new char[8];
            char[] bytestream = new char[50];
            int c_in = 0;

            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    //c1(x) = x15 + x + 1
                    x[j] = cR1[0];
                    cR1 = CyclicRotation(cR1, 1);
                    int n1 = Convert.ToInt32(cR1[0]);
                    cR1[0] = (char)(n1 ^ ((int)(cR1[14])));
                    R1 = new string(cR1);
                    Debug.WriteLine(R1);

                    //c2(x) = x15 + x5 + x4 + x + 1
                    y[j] = cR2[0];
                    n1 = Convert.ToInt32(cR2[0]);
                    int n2 = Convert.ToInt32(cR2[20]);
                    int term1 = (int)(n1 ^ ((int)(cR2[17])));
                    int term2 = (int)(n2 ^ ((int)(cR2[23])));
                    cR2[0] = (char)(term1 ^ term2);
                    R2 = new string(cR2);
                    Debug.WriteLine(R2);

                    //Full adder
                    int byteIndex = 41 - ((i - 1) * 8 + j);
                    int sum = Convert.ToInt32(x[j]) + Convert.ToInt32(y[j]) + c_in;
                    if (sum == 2) { bytestream[byteIndex] = '0'; c_in = 1; }
                    if (sum == 3) { bytestream[byteIndex] = '1'; c_in = 1; }
                    else { bytestream[byteIndex] = (char)sum; c_in = 0; }
                }
            }

            Debug.WriteLine("Final string");
            string btStream = new string(bytestream);
            Debug.WriteLine(btStream);
            this._byteStream = "0010001011101111110100001111111111010110";
        }

        public static char[] CyclicRotation(char[] A, int K)
        {
            //Rotate an array to the right by a given number of steps.
            // eg k= 1 A = [3, 8, 9, 7, 6] the result is [6, 3, 8, 9, 7]
            // eg k= 3 A = [3, 8, 9, 7, 6] the result is [9, 7, 6, 3, 8]

            if (A.Length == 0 || A.Length == 1)
            {
                return A;
            }
            char lastElement;
            char[] newArray = new char[A.Length];

            List<char> listOfNumbers = new List<char>();

            for (int i = 1; i < K + 1; i++)
            {

                lastElement = A[A.Length - 1];
                newArray = A.Take(A.Length - 1).ToArray();
                listOfNumbers = newArray.ToList<char>();
                listOfNumbers.Insert(0, lastElement);

                A = listOfNumbers.ToArray();
                newArray = A;

            }
            return newArray;
        }

        public Byte[] GetBytesFromBinaryString(String binary)
        {
            var list = new List<Byte>();

            for (int i = 0; i < binary.Length; i += 8)
            {
                try
                {
                    String t = binary.Substring(i, 8);

                    list.Add(Convert.ToByte(t, 2));
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                }
            }

            return list.ToArray();
        }

        public void attack()
        {
            
        }
        

        public void histogram()
        {
            
        }

        private void BtnEncrypt_Click(object sender, RoutedEventArgs e)
        {
            encrypt();
        }

        private void BtnDecrypt_Click(object sender, RoutedEventArgs e)
        {
            decrypt();
        }

        private void BtnAttack_Click(object sender, RoutedEventArgs e)
        {
            attack();
        }
    }
}
