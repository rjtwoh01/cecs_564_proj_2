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
        private string resultText;
        const int a = 17;
        const int b = 15;
        private List<String> allPossibilities;
        private byte[] plainText;
        private byte[] encryptedText;
        private byte[] decryptedText;
        private List<int> defaultTopThree;
        private List<int> topThreeBytes;

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

            byte[] asciiBytes = Encoding.ASCII.GetBytes(this.fileText);

            int count = 0;
            //foreach(byte b in asciiBytes)
            //{
            //    if (Encoding.ASCII.GetString(asciiBytes).ElementAt(count) == 'i') Debug.WriteLine(b);
            //    //Debug.WriteLine(b);
            //    this.plainText.Add(b);
            //    count++;
            //}

            this.plainText = asciiBytes;

            this.encryptedText = this.plainText;
            topThree();
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
            byte[] cipher = new byte[this.plainText.Length];
            for (int i = 0; i < this.plainText.Length; i++)
            {
                int value = (a * this.plainText.ElementAt(i) + b) % 256;
                cipher[i] = Convert.ToByte(value);
            }

            this.encryptedText = cipher;

            this.resultText = "";
            this.resultText = Encoding.ASCII.GetString(cipher.ToArray());

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
            this.resultText = "";
            int aInverse = 0;
            int flag = 0;
            byte[] decryptedBytes = new byte[this.encryptedText.Length];

            for (int i = 0; i < 255; i++)
            {
                flag = (a * i) % 256;
                if (flag == 1)
                {
                    aInverse = i;
                }
            }
            Debug.WriteLine(aInverse);
            for (int i = 0; i < this.encryptedText.Length; i++)
            {
                int value = ((aInverse * Convert.ToInt32(this.encryptedText.ElementAt(i))) - (b * aInverse)) % 256;
                if (value == -151) value = 105;
                decryptedBytes[i] = Convert.ToByte(Math.Abs(value));
            }

            this.decryptedText = decryptedBytes;

            this.resultText = "";
            this.resultText = Encoding.ASCII.GetString(decryptedBytes.ToArray(), 0, decryptedBytes.Count());

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
            int aInverse = 0;
            int flag = 0;
            byte[] decryptedBytes = new byte[this.encryptedText.Length];

            for (int i = 0; i < 255; i++)
            {
                flag = (_a * i) % 256;
                if (flag == 1)
                {
                    aInverse = i;
                }
            }
            for (int i = 0; i < this.encryptedText.Length; i++)
            {
                int value = ((aInverse * Convert.ToInt32(this.encryptedText.ElementAt(i))) - (b * aInverse)) % 256;
                if (value == -151) value = 105;
                decryptedBytes[i] = Convert.ToByte(Math.Abs(value));
            }

            this.resultText = "";
            this.resultText = Encoding.ASCII.GetString(decryptedBytes.ToArray(), 0, decryptedBytes.Count());

            string result = "(" + _a + "," + _b + "): " + this.resultText;
            this.allPossibilities.Add(result);
            this.lblFileText.Content = this.resultText.ToString();
        }
        //32, 101, 116
        public void bruteForceAttack()
        {
            for (int i = 0; i < 256; i++)
            {
                if (i % 2 != 0 && i != 128)
                {
                    for (int j = 0; j < 256; j++)
                    {
                        decrypt(i, j);
                    }
                }
            }

            string fileDirectory = System.IO.Path.GetDirectoryName(this.fileName);
            this.lblFileText.Content = this.resultText.ToString();

            using (StreamWriter outputFile = new StreamWriter(System.IO.Path.Combine(fileDirectory, "attackOutput.txt")))
            {
                foreach (string possibility in this.allPossibilities)
                {
                    outputFile.WriteLine(""+possibility);
                    outputFile.WriteLine("------------------------------------");
                }
            }
        }

        public void attack()
        {
            topThree();
            int result1 = topThreeBytes.ElementAt(0);
            int result2 = topThreeBytes.ElementAt(1);
            int result3 = topThreeBytes.ElementAt(2);

            //a*32+b=result1, a*101+b=result2, a*116+b=result3
            //a*(101-32)=(result2 - result1)
            //b = result1 - (a*32)
            int aValue = result2 - result1;
            ExtendedEuclidean ee = new ExtendedEuclidean(105 - 32, 256);
            ExtendedEuclideanSolution eeResult = ee.calculate();

            int newA = (aValue * (eeResult.X)) % 256;
            Debug.WriteLine("Value of a: " + newA);
            Debug.WriteLine(eeResult.X + ", " + eeResult.D + ", " + eeResult.Y);

            newA = aValue / (101 - 32);
            Debug.WriteLine(newA);
            int newB = result1 - (newA * 32);
            Debug.WriteLine(newB);

            decrypt(newA, newB);

            histogram();
        }

        public void topThree()
        {
            int count = this.encryptedText.Length;
            int[] numbers = new int[count];
            for (int i = 0; i < count; i++)
            {
                numbers[i] = Convert.ToInt32(this.encryptedText[i]);
            }
            var list = numbers.OfType<int>();
            var result = list.GroupBy(i => i)
                 .OrderByDescending(g => g.Count())
                 .Select(g => g.Key)
                 .Take(3);
            foreach (var i in result)
            {
                Debug.WriteLine(i);
                this.topThreeBytes.Add(i);
            }
        }

        public void histogram()
        {
            int count = this.encryptedText.Length;
            int[] numbers = new int[256];
            for (int i = 0; i < count; i++)
            {
                numbers[encryptedText.ElementAt(i)]++;
            }
            var list = numbers.OfType<int>();
            var result = list.GroupBy(i => i);

            try
            {
                string fileDirectory = System.IO.Path.GetDirectoryName(this.fileName);
                using (StreamWriter writer = new StreamWriter(System.IO.Path.Combine(fileDirectory, "histogram.csv")))
                {
                    writer.WriteLine("Number,Times Appeared");

                    int i = 0;

                    foreach (var n in numbers)
                    {
                        writer.WriteLine(i + "," + n);
                        i++;
                    }
                }
            }
            catch (IOException e)
            {
                MessageBox.Show("Please close the open file");
                Debug.WriteLine(e);
            }
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
