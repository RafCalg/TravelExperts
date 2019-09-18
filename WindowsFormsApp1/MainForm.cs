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
    public partial class MainForm : Form
    {
        public TravelExpertsLINQDataContext dbContext = new TravelExpertsLINQDataContext();
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            packageDataGridView.DataSource = dbContext.Packages;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            AddEditPackage addEditForm = new AddEditPackage();
            addEditForm.mainForm = this;

            //Shows the Add form
            DialogResult result = addEditForm.ShowDialog();


            if (result == DialogResult.OK) // Save
            {
                //Refresh the Grid View
                //dbContext.Refresh(System.Data.Linq.RefreshMode.OverwriteCurrentValues,
                //                    dbContext.Products);
                dbContext = new TravelExpertsLINQDataContext();
                
               packageDataGridView.DataSource = dbContext.Packages;

            }
        }
    }
}
