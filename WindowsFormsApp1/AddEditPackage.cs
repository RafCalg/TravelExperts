using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class AddEditPackage : Form
    {
        public MainForm mainForm; //Declares the Mainform as a public object
        private List<int> prdList = new List<int>(); // Used to store the list of selected products Id's
        private List<int> suplList = new List<int>(); // Used to store the list of selected suppliers Id's
        private List<int> prodSuplList = new List<int>(); // Used to store the list of Product_Suppliers Id's

        public AddEditPackage()
        {
            InitializeComponent();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void addPackageButton_Click(object sender, EventArgs e)
        {
            if (true) //replace true with data validation
            {
                //Create new Package with data from texctboxes
                Package newPackage = new Package
                {
                    PkgName             = pkgNameTextBox.Text,
                    PkgStartDate        = Convert.ToDateTime(pkgStartDateDateTimePicker.Text),
                    PkgEndDate          = Convert.ToDateTime(pkgEndDateDateTimePicker.Text),
                    PkgBasePrice        = Convert.ToDecimal(pkgBasePriceTextBox.Text),
                    PkgAgencyCommission = Convert.ToDecimal(pkgAgencyCommissionTextBox.Text),
                    PkgDesc             = pkgDescTextBox.Text
                };

                //used dbContext from the main form to insert new product
                mainForm.dbContext.Packages.InsertOnSubmit(newPackage);
                mainForm.dbContext.SubmitChanges();



                using (TravelExpertsLINQDataContext dbContext = new TravelExpertsLINQDataContext())
                {
                    int i = 0;
                    foreach (int item in prdList)
                    {
                        //Adds a new Product_Supplier object in the Product_Supplier Table
                        Products_Supplier newProdSuppliers = new Products_Supplier();
                        newProdSuppliers.ProductId = prdList[i];
                        newProdSuppliers.SupplierId = suplList[i];

                        mainForm.dbContext.Products_Suppliers.InsertOnSubmit(newProdSuppliers);
                        mainForm.dbContext.SubmitChanges();

                        //Gets the Product Supplier Id that was just created after the Insertion
                        prodSuplList.Add(newProdSuppliers.ProductSupplierId);

                        //Adds a new Product_Supplier object in the Product_Supplier Table
                        Packages_Products_Supplier newPkgProdSupplier = new Packages_Products_Supplier();
                        newPkgProdSupplier.PackageId = newPackage.PackageId;
                        newPkgProdSupplier.ProductSupplierId = prodSuplList[i];

                        mainForm.dbContext.Packages_Products_Suppliers.InsertOnSubmit(newPkgProdSupplier);
                        mainForm.dbContext.SubmitChanges();
                        i++;
                    }


                    


                }

                DialogResult = DialogResult.OK;

                //Refresh the Grid View

            }
        }

        private void AddEditPackage_Load(object sender, EventArgs e)
        {
            TravelExpertsLINQDataContext dbContext = new TravelExpertsLINQDataContext();
            var listProducts = (from x in dbContext.Products select x).ToList();

            //Populate the Product List ComboBox
            prodNameComboBox.DataSource = listProducts;
            prodNameComboBox.ValueMember = "ProductID";
            prodNameComboBox.DisplayMember = "ProdName";

            //Populate the Supplier List Combo Box
            var listSuppliers = (from x in dbContext.Suppliers select x).ToList();
            supNameComboBox.DataSource = listSuppliers;
            supNameComboBox.ValueMember = "SupplierId";
            supNameComboBox.DisplayMember = "SupName";
        }

        private void prodNameComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {

            int selectedId = prodNameComboBox.SelectedIndex+1;
            TravelExpertsLINQDataContext dbContext = new TravelExpertsLINQDataContext();

            
            var listSuppliers = (from x in dbContext.Suppliers select x).ToList();


            var productSuppliers = (from prod in dbContext.Products
                                   join prodSup in dbContext.Products_Suppliers
                                   on prod.ProductId equals prodSup.ProductId
                                   join sup in dbContext.Suppliers
                                   on prodSup.SupplierId equals sup.SupplierId
                                   where prod.ProductId == selectedId
                                   select new
                                   {
                                       supplierId = sup.SupplierId,
                                       supName = sup.SupName
                                   }).ToList();

            //int dummy = productSuppliers[0].supId;

            supNameComboBox.DataSource = productSuppliers;
            supNameComboBox.DisplayMember = "SupName";
            supNameComboBox.ValueMember = "SupplierId";
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            TravelExpertsLINQDataContext dbContext = new TravelExpertsLINQDataContext();
            int prd = (int)prodNameComboBox.SelectedValue;

            prdList.Add(prd); // Stores the selected product in a list

           var prodNameList = (from prod in dbContext.Products
                                   where prod.ProductId == prd
                                   select new
                                   {
                                       name = prod.ProdName
                                   }).ToList();
            string prodName = prodNameList[0].name;


            int supl = (int)supNameComboBox.SelectedValue;
            suplList.Add(supl);

            var suppNameList = (from sup in dbContext.Suppliers
                                where sup.SupplierId == supl
                                select new
                                {
                                    name = sup.SupName
                                }).ToList();

            string suppName = suppNameList[0].name;

            string item = prodName + " | " + suppName;
            productsSuppliersListBox.Items.Add(item);
        }
    }
}
