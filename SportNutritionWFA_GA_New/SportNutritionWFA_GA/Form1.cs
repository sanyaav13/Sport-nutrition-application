using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;


namespace SportNutritionWFA_GA
{
 
    

    public partial class Form1 : Form
    {
        private string fn = string.Empty;

        static string filename_txt = "прайс розничный_20.03.15 финальный.txt";
        static string filename_bin = "price.dat";
        string path_txt = @"C:\Users\User\Documents\Visual Studio 2012\Projects\GalinaAN\SportNutritionWFA_GA\" + filename_txt;
        string path_bin = @"C:\Users\User\Documents\Visual Studio 2012\Projects\GalinaAN\SportNutritionWFA_GA\" + filename_bin;
        //string selectedItemcomboBoxBrand;
        DataTable table;
        ClassSportNutrition_GA cl = new ClassSportNutrition_GA();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //ClassSportNutrition_GA cl = new ClassSportNutrition_GA();

            string primary_full_file_name = cl.CurrentDirectory + cl.primaryTXTfile;
            string cleaning_full_file_name = cl.CurrentDirectory + cl.cleaningTXTfile;
            string binary_full_file_name = cl.CurrentDirectory + cl.BINARYfile;

           // cl.CleaningTextFileFromTabs(primary_full_file_name, cleaning_full_file_name);
           // cl.CreateBinaryFile(cleaning_full_file_name, binary_full_file_name);

            saveFileDialog1.Filter = "Текстовые файлы (*.txt)|*.txt|All files (*.*)|*.*";


            Record rec;
            BinaryReader reader = new BinaryReader(File.Open( binary_full_file_name, FileMode.Open ));
            table = new DataTable();
            // Заполнение "шапки" таблицы
            table.Columns.Add("Продукт");
           // table.Columns.Add("Вкус");
            table.Columns.Add("Производитель");
           // table.Columns.Add("Кол-во");
            table.Columns.Add("Цена");


            List<string> brand_list = new List<string>();

            while (reader.PeekChar() > -1)
            {

                rec.product = reader.ReadString(); // продукт//String.Format(cl.product_format, reader.ReadString());
                rec.brand = String.Format(cl.brand_format, reader.ReadString().Trim());//reader.ReadString();
                rec.price = reader.ReadInt32();
                table.Rows.Add(rec.product, rec.brand.Trim(), rec.price.ToString());
                //table.Rows.Add(rec.product, taste, rec.brand.Trim(), quantity, rec.price.ToString());

                if ( ! brand_list.Contains(rec.brand) )
                    brand_list.Add(rec.brand);
            }
            reader.Close();

            // Для сетки данных указываем источник данных 
            
            dataGridView1.DataSource = table;         

            dataGridView1.Columns[0].ReadOnly = true;
            dataGridView1.Columns[1].ReadOnly = true;
            dataGridView1.Columns[2].ReadOnly = true;
            dataGridView1.AllowUserToAddRows = true; // запрет добавления новой строки


            dataGridView1.Columns[0].FillWeight = 70;
            dataGridView1.Columns[1].FillWeight = 35;
            dataGridView1.Columns[2].FillWeight = 20;

            brand_list.Sort();
            brand_list.Insert(0, "Не выбран");
            comboBoxBrand.DataSource = brand_list;

          

        }

        private void comboBoxBrand_SelectedValueChanged(object sender, EventArgs e)
        {
                //Size r = dataGridView1.PreferredSize;
                //dataGridView1.Width = 1;
                string filter;
                string comboBoxBrandSelectedValue = comboBoxBrand.SelectedValue.ToString();
                // контроль правильности написания символа ' это  \'
                comboBoxBrandSelectedValue = cl.ControlApostrophe(comboBoxBrandSelectedValue);
               

                if (comboBoxBrandSelectedValue.Contains("Не выбран"))
                    filter = "[Производитель] LIKE '*'";
                else
                {
                    filter = "[Производитель] LIKE '";       
                    filter = String.Concat(filter, comboBoxBrandSelectedValue);
                    filter = String.Concat(filter, "'");      
                }

                    DataTable gridTable = (DataTable)dataGridView1.DataSource;

                    gridTable.DefaultView.RowFilter = filter;

                    
                    // нумерация строк ***********
                    cl.LineNumberingDatagridView(dataGridView1);

        }

        private void dataGridView1_RowHeaderMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            object product = dataGridView1.Rows[e.RowIndex].Cells[0].Value;
            object taste = string.Empty;       //вкус
            object brand = dataGridView1.Rows[e.RowIndex].Cells[1].Value;
            object price = dataGridView1.Rows[e.RowIndex].Cells[2].Value;
            object quantity = 0;               //количество
            object cost = 0; //string.Empty;   //стоимость
            dataGridViewCustomerOrder.Rows.Add(product, taste, brand, price, quantity, cost);
            // нумерация строк ***********
            cl.LineNumberingDatagridView(dataGridViewCustomerOrder);
        }

        private void dataGridViewCustomerOrder_RowHeaderMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            int rowToDelete = e.RowIndex;
            if (rowToDelete > -1)
            {
                dataGridViewCustomerOrder.Rows.RemoveAt(rowToDelete);
                cl.LineNumberingDatagridView(dataGridViewCustomerOrder);
            }


            double sum = 0;
            for (int i = 0; i < dataGridViewCustomerOrder.RowCount; ++i)
            {
                sum += Convert.ToDouble(dataGridViewCustomerOrder.Rows[i].Cells[5].Value.ToString());
            }
            textBox4.Text = Convert.ToString(sum);
            int discount = 0;
            if (sum >= 5000)
                discount = 12;
            else
                if (sum >= 3000)
                    discount = 10;
                else
                    if (sum >= 1000)
                        discount = 7;
                    else
                        if (sum >= 500)
                            discount = 5;
            textBox2.Text = Convert.ToString(discount);
            double sumDis = sum - (sum * (discount * 0.01));
            textBox3.Text = Convert.ToString(Math.Round(sumDis));
            textBox1.Text = Convert.ToString(Math.Round(sumDis + sumDis * 0.006));

        }

      

        private void dataGridViewCustomerOrder_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridViewCustomerOrder.Columns[e.ColumnIndex].Name == "Quantity")
            {
                double price = Convert.ToDouble(dataGridViewCustomerOrder.Rows[e.RowIndex].Cells["Price"].Value);
                double quantity = Convert.ToDouble(dataGridViewCustomerOrder.Rows[e.RowIndex].Cells["Quantity"].Value);

                dataGridViewCustomerOrder.Rows[e.RowIndex].Cells["Cost"].Value = price * quantity;
            }

            double sum = 0;
            for (int i = 0; i < dataGridViewCustomerOrder.RowCount; ++i)
            {
                sum += Convert.ToDouble(dataGridViewCustomerOrder.Rows[i].Cells[5].Value.ToString());
            }
            textBox4.Text = Convert.ToString(sum);
            int discount = 0;
            if (sum >= 5000)
                discount = 12;
            else
                if (sum >= 3000)
                    discount = 10;
                else
                    if (sum >= 1000)
                        discount = 7;
                    else
                        if (sum >= 500)
                            discount = 5;
            textBox2.Text = Convert.ToString(discount);
            double sumDis = sum - (sum * (discount * 0.01));
            textBox3.Text = Convert.ToString(Math.Round(sumDis));
            textBox1.Text = Convert.ToString(Math.Round(sumDis + sumDis * 0.006));


        }

        private void button1_Click(object sender, EventArgs e)
        {
            string[] a;
            string[] b;
            const String NS = "\r\n"; // Новая строка
            StringBuilder order = new StringBuilder();
            order.AppendLine("--------------Заказ-----------------------------");
            order.AppendLine("Город: " + textBoxCity.Text);
            order.AppendLine("Ф. И. О. : " + textBoxFameli.Text);
            order.AppendLine("Телефон : " + textBoxtelephone.Text);
            order.AppendLine("Перевозчик : " + textBoxCarrier.Text);
            order.AppendLine("Номер склада : " + textBoxstorehouseNumber.Text);
            order.AppendLine("Способ оплаты : " + textBoxPaymentMethod.Text);
            order.AppendLine("-------------------------------------------------");
            order.AppendLine("-------------------------------------------------");
            order.AppendLine("-------------------------------------------------");
            order.AppendLine("--------------Выбранные позиции------------------");
            string dash = new string('-', 140);
            order.AppendLine(dash);
            //order.AppendLine("-------------------------------------------------------------------------------------------------------------------");
            string prod = "| Продукт";
            order.AppendFormat("{0,-60}", prod);
            string taste = "| Вкус";
            order.AppendFormat("{0,-15}", taste);
            string maker = "| Производитель";
            order.AppendFormat("{0,-32}", maker);
            string pr = "| Цена";
            order.AppendFormat("{0,-10}", pr);
            string quan = "| Количество";
            order.AppendFormat("{0,-12}", quan);
            string sum = "| Cумма";
            order.AppendFormat("{0,-10}", sum);
            order.AppendLine();
            order.AppendLine(dash);
            //order.AppendLine("|\tПродукт\t\t\t\t\t\t\t\t|   Вкус\t|    Производитель\t\t\t\t| Цена   | Количество |  Cумма  |");
            //order.AppendLine("-------------------------------------------------------------------------------------------------------------------");

            //for (int i = 0; i < dataGridViewCustomerOrder.RowCount; i++)
            //{
            //    for (int j = 0; j < dataGridViewCustomerOrder.ColumnCount; j++)
            //    {
            //        order.Append(dataGridViewCustomerOrder.Rows[i].Cells[j].Value.ToString() + " ");
            //    }
            //    order.AppendLine();
            //}

            for (int i = 0; i < dataGridViewCustomerOrder.RowCount; i++)
            {
                //dataGridViewCustomerOrder.Rows[i].Cells[0].Value.ToString().Trim();
                order.Append(String.Format("{0, -63}", dataGridViewCustomerOrder.Rows[i].Cells[0].Value.ToString().Trim()));
                order.Append(String.Format("{0, -15}", dataGridViewCustomerOrder.Rows[i].Cells[1].Value.ToString()));
                order.Append(String.Format("{0, -32}", dataGridViewCustomerOrder.Rows[i].Cells[2].Value.ToString()));
                order.Append(String.Format("{0, -10}", dataGridViewCustomerOrder.Rows[i].Cells[3].Value.ToString()));
                order.Append(String.Format("{0, -12}", dataGridViewCustomerOrder.Rows[i].Cells[4].Value.ToString()));
                order.Append(String.Format("{0, -10}", dataGridViewCustomerOrder.Rows[i].Cells[5].Value.ToString()));
                order.AppendLine();
            }
            order.AppendLine(dash);
            //order.AppendLine("-------------------------------------------------------------------------------------------------------------------");
            order.AppendLine();
            order.AppendLine();
            order.AppendLine();
            order.AppendLine("Cумма заказа: " + textBox4.Text);
            order.AppendLine("Cкидка (%): " + textBox2.Text);
            order.AppendLine("Cумма со скидкой: " + textBox3.Text);
            order.AppendLine("Cумма с комиссией банка: " + textBox1.Text);

            saveFileDialog1.FileName = textBoxFameli.Text + textBoxCity.Text;
             if (fn == string.Empty)
             {
                 if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                 {

                     var Писатель = new System.IO.StreamWriter(
                                       saveFileDialog1.FileName, false,
                                       System.Text.Encoding.GetEncoding(1251));
                     // - здесь заказ кодовой страницы Win1251 для русских букв
                     Писатель.Write(order.ToString());
                     Писатель.Close(); 
                 }
             }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            string filter = "[Производитель] LIKE '*'";
            DataTable gridTable = (DataTable)dataGridView1.DataSource;
            gridTable.DefaultView.RowFilter = filter;
            comboBoxBrand.SelectedIndex = 0;
            dataGridViewCustomerOrder.Rows.Clear();
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
            textBox1.Text = "";
            textBoxCity.Text = "";
            textBoxFameli.Text = "";
            textBoxtelephone.Text = "";
            textBoxCarrier.Text = "";
            textBoxstorehouseNumber.Text = ""; 
            textBoxPaymentMethod.Text = ""; 

        }

       
       
    }
}
