using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Xml.XPath;

namespace XMLParser
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            clearResults();
        }

        private void clearResults()
        {
            lblCustomer.Text = String.Empty;
            lblDate.Text = String.Empty;
            lblId.Text = String.Empty;
            lblPriceAverage.Text = String.Empty;
            lblTotal.Text = String.Empty;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            fileDialog.ShowDialog();
            txtFileName.Text = fileDialog.FileName;            
        }

        private void btnProcess_Click(object sender, EventArgs e)
        {
            if (!validateInput())
            {
                MessageBox.Show("Please select a valid XML file!");
                return;
            }
            try
            {
                var doc = XDocument.Load(txtFileName.Text);
                WebOrder webOrder = getWebOrder(doc);
                webOrder.Items = getWebOrderItems(doc);
                populateResults(webOrder);
            } catch (Exception ex)
            {
                MessageBox.Show("Error processing data!");
                Console.WriteLine(ex);
            }            
        }

        private bool validateInput()
        {
            return txtFileName.Text.EndsWith(".xml");
        }

        private WebOrder getWebOrder(XDocument doc)
        {
            WebOrder webOrder = new WebOrder();
            webOrder.Id = Convert.ToInt32(doc.XPathSelectElement("/WebOrder").Attribute("id").Value);
            webOrder.Customer = doc.XPathSelectElement("/WebOrder/Customer").Value;
            string dateStr = doc.XPathSelectElement("/WebOrder/Date").Value;
            int year = Convert.ToInt32(dateStr.Substring(0, 4));
            int month = Convert.ToInt32(dateStr.Substring(4, 2));
            int day = Convert.ToInt32(dateStr.Substring(6, 2));
            webOrder.Date = new DateTime(year, month, day);

            return webOrder;
        }

        private List<WebOrderItem> getWebOrderItems(XDocument doc)
        {
            List<WebOrderItem> webOrderItems = new List<WebOrderItem>();
            var items = doc.XPathSelectElements("/WebOrder/Items/Item");
            foreach (var item in items)
            {
                WebOrderItem webOrderItem = new WebOrderItem();
                webOrderItem.Id = item.Attribute("id").Value;
                webOrderItem.Description = item.Attribute("description").Value;
                webOrderItem.Quantity = Convert.ToInt32(item.Element("Quantity").Value);
                webOrderItem.Price = decimal.Parse(item.Element("Price").Value,
                                new NumberFormatInfo() { NumberDecimalSeparator = "." });
                webOrderItems.Add(webOrderItem);
            }
            return webOrderItems;
        }

        private void populateResults(WebOrder webOrder)
        {
            lblId.Text = webOrder.Id.ToString();
            lblCustomer.Text = webOrder.Customer;
            lblDate.Text = webOrder.Date.ToString("dd.MMMM.yyyy");

            NumberFormatInfo numberFormatInfo = new NumberFormatInfo() {
                NumberDecimalSeparator = ",",
                NumberGroupSeparator = ".",
                NumberDecimalDigits = 3 };

            decimal total = webOrder.Items.Sum(wi => wi.Price * wi.Quantity);
            lblTotal.Text = total.ToString("n", numberFormatInfo);

            decimal average = webOrder.Items.Average(wi => wi.Price);
            lblPriceAverage.Text = average.ToString("n", numberFormatInfo);
        }
    }
}
