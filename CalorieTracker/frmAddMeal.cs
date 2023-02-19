﻿using BLL.Services;
using DAL;
using Entities.Concrete;
using Entities.Dtos.FoodDtos;
using Entities.Dtos.MealDetailsDtos;
using Entities.Dtos.MealDtos;
using Entities.Dtos.UserDtos;
using Entities.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UI
{
    public partial class frmAddMeal : Form
    {
        private Form activeForm;
        Context context = new Context();
        frmReports statisticsForm;
        frmUser userForm;
        frmAddMeal addMealForm;
        frmMain mainForm;

        User currentUser;
        Meal currentMeal;

        private Button currentButton;
        private int tempIndex;

        public frmAddMeal(User user)
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.ControlBox = false;
            this.currentUser = user;
            CenterToScreen();
        }

        private void frmAddMeal_Load(object sender, EventArgs e)
        {
            FillFoods();
            FillMealTypes();
            //Get daily meals of user




















        }

        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();
        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hWnd, int wMsg, int wParam, int lParam);
        private void ActivateButton(object btnSender)
        {
            if (btnSender != null)
            {
                if (currentButton != (Button)btnSender)
                {
                    DisableButton();
                    Color color = Color.YellowGreen;
                    currentButton = (Button)btnSender;
                    currentButton.BackColor = color;
                    currentButton.ForeColor = Color.White;
                }
            }
        }

        private void DisableButton()
        {
            //List<Button> menuButtons = new List<Button>() { btnHome, btnMeal, btnSettings, btnUser, btnReports};
            foreach (Control previousBtn in panelMenu.Controls)
            {
                if (previousBtn.GetType() == typeof(Button))
                {
                    previousBtn.BackColor = Color.OliveDrab;
                }
            }
        }

        private void btnHome_Click(object sender, EventArgs e)
        {
            ActivateButton(sender);


            this.Hide();
            mainForm = new frmMain(currentUser);
            mainForm.Show();

        }

        private void btnReports_Click(object sender, EventArgs e)
        {
            ActivateButton(sender);

            this.Hide();
            statisticsForm = new frmReports(currentUser);
            statisticsForm.Show();

        }

        private void btnMeal_Click(object sender, EventArgs e)
        {
            ActivateButton(sender);

        }

        private void btnUser_Click(object sender, EventArgs e)
        {
            ActivateButton(sender);


            this.Hide();
            userForm = new frmUser(currentUser);
            userForm.Show();
        }


        private void btnMinimizeApp_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void btnCloseApp_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void panelTitleBar_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }
        private void FillFoods()
        {
            FoodService foodService = new FoodService(context);
            List<FoodViewModel> foodList = new List<FoodViewModel>();           
            foodList = foodService.GetAllFoods();
            foreach (var food in foodList)
            {
                cmbFoodList.Items.Add(food);
            }
        }
        private void FillMealTypes()
        {
            MealTypeService mealTypeService = new MealTypeService(context);
            List<MealTypeViewModel> mealTypeList = new List<MealTypeViewModel>();
            mealTypeList = mealTypeService.GetAllMealTypes();
            foreach (var mealType in mealTypeList)
            {
                dgvMealTypes.Rows.Add(mealType.Id,mealType.Name);               
            }
            
        }
        private void FillMealDetails()
        {
            MealDetailsService mealDetailsService = new MealDetailsService(context);
            MealService mealService= new MealService(context);
            DataGridViewRow selectedRow = dgvMealTypes.SelectedRows[0];
            List<Meal> mealList = mealService.GetMealsByDate(DateTime.Now, currentUser, Convert.ToInt32(selectedRow.Cells["clmId"].Value.ToString()));
            Meal meal=mealList.FirstOrDefault(s => s.Id == currentMeal.Id);
            MealViewModel mealViewModel = new MealViewModel()
            {
                 Id= currentMeal.Id,
                //devam edilcek
            };
            mealViewModel.MealDetailsViewModel = mealDetailsService.GetFoodsByMealType(DateTime.Now, currentUser, Convert.ToInt32(selectedRow.Cells["clmId"].Value.ToString()));
            foreach (var mealDetails in mealViewModel.MealDetailsViewModel)
            {
                dgvMealTypes.Rows.Add(mealDetails.Food, mealDetails.Gram, mealDetails.Calorie, mealDetails.Image);
            }
        }
        private void btnAddFood_Click(object sender, EventArgs e)
        {
            if (nudGram.Value==0)
            {
                MessageBox.Show("You must enter gram value.");
                return;
            }
            try
            {
                MealDetailsService mealDetailService = new MealDetailsService(context);
                MealService mealService= new MealService(context);
                DataGridViewRow selectedRow = dgvMealTypes.SelectedRows[0];
                Food selectedFood= cmbFoodList.SelectedItem as Food;               
                MealType selectedMealType = dgvMealTypes.SelectedRows[0].Tag as MealType;                               
                MealCreateDTO mealCreateDTO = new MealCreateDTO
                {
                    MealTypeId = selectedMealType.Id,
                    UserId = currentUser.Id,

                };
                mealService.AddMeal(mealCreateDTO);
             
                MealDetailsCreateDTO mealDetail = new MealDetailsCreateDTO
                {
                    Gram = Convert.ToDouble(nudGram.Value),
                    FoodId = selectedFood.Id,
                    MealId=currentMeal.Id,
                    
                };
                mealDetailService.AddMealDetail(mealDetail);
                FillMealDetails();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
