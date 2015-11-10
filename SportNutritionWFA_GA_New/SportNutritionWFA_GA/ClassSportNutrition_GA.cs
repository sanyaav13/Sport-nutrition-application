using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace SportNutritionWFA_GA
{     
    struct Record
    {
         public string product;
         public string brand;
         public int price;
         
         //public Record(string pr, string br, int prc)
         //{
         //       product = pr;
         //       brand = br;
         //       price = prc;
         // }
        }
    public class ClassSportNutrition_GA
    {
        public  Encoding encodingDfault = Encoding.Default;  // кодовая страница по умолчанию

        public string primaryTXTfile = @"прайс розничный!!!!!.txt";                 //@"новый прайс######.txt";      // первичный файл txt, полученный из Exel и содержащий символы \t
        public string cleaningTXTfile = @"очищеный прайс######.txt";  // очищенный от \t  и отформатированный  txt-файл
        public string BINARYfile = @"price binare file.dat";          //двоичный файл

        public string CurrentDirectory = Directory.GetCurrentDirectory() + @"\";

        public string product_format = "{0,-60}"; // продукт
        public string brand_format = "{0,-40}";   // производитель
        public string price_format = "{0,-10}";   // цена

        //**********************************************************
        // очистка текстового файла от знаков табуляции
        public void CleaningTextFileFromTabs(string primary_full_file_name, string cleaning_full_file_name) 
        {
            StreamReader reader = new StreamReader(primary_full_file_name, encodingDfault);
            StreamWriter writer = new StreamWriter(cleaningTXTfile, false, encodingDfault);

            string line;
            string line_new;

            while ((line = reader.ReadLine()) != null)
            {
                string[] words = line.Split(new char[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);// не учитывать пустые строки
                string[] words_new = new string[words.Length];

                for (int i = 0; i < words.Length; i++)
                {
                    switch (i)
                    {
                        case 0: words_new[i] = String.Format(product_format, words[i]); // продукт
                                break;
                        case 1: words_new[i] = String.Format(brand_format, words[i]);   // производитель
                                break;
                        case 2: words_new[i] = String.Format(price_format, words[i]);   // цена
                                break;
                        default: words_new[i] = String.Format("{0,-80}", words[i]);
                                break;
                    }
                }

                line_new = String.Join("", words_new);

                writer.WriteLine(line_new);
            }
            reader.Close();
            writer.Close();
        }

        //********************************************
        // формирование бинарного файла
        public void CreateBinaryFile(string cleaning_full_file_name, string binary_full_file_name )
        {
            Record rec;
            StreamReader reader = new StreamReader(cleaning_full_file_name, encodingDfault);
            BinaryWriter writer = new BinaryWriter(File.Open(binary_full_file_name, FileMode.OpenOrCreate));

            string line;
            while ((line = reader.ReadLine()) != null)
            {
                string pr;
                rec.product = line.Substring(0, 60);
                rec.brand = line.Substring(60, 40);
                pr = line.Substring(100).Trim();
                rec.price = Convert.ToInt32(pr);

                writer.Write(rec.product);
                writer.Write(rec.brand);
                writer.Write(rec.price);
            }
            reader.Close();
            writer.Close();
        }

        //*********************************************
        // нумерация строк в DataGridview
        public void LineNumberingDatagridView( DataGridView  dataGridView1)
        {
             int rowNumber = 1;
                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        if (row.IsNewRow) continue;
                        row.HeaderCell.Value = rowNumber.ToString().Trim();
                        rowNumber = rowNumber + 1;
                    }
                    dataGridView1.AutoResizeRowHeadersWidth(
                        DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders);
        }
        // контроль символа ' (апостроф)
        public string ControlApostrophe(string str)
        {
            if (str.Contains("'"))
            {
                int pos = str.IndexOf('\'');
                str = str.Insert(pos, "'");
            }
            return str;
        }

    }
    

}
