﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OJT.App.Views.Menu;
using OJT.App.Views.Enrollment;
using OJT.Services;
using OJT.Services.Enrollment;
using Microsoft.Office.Interop.Excel;

namespace OJT.App.Views.Student
{
    public partial class UCStudentList : UserControl
    {
        private StudentService studentService = new StudentService();
        private EnrollmentService enrollmentService = new EnrollmentService(); 
        public UCStudentList()
        {
            InitializeComponent();
        }

        private void UCStudentList_Load(object sender, EventArgs e)
        {
            BindGrid();
        }
        private void BindGrid()
        {
            System.Data.DataTable dt = studentService.GetAll();
            dgvStudentList.DataSource = dt;

            DataGridViewButtonColumn btn2 = new DataGridViewButtonColumn();
            btn2.HeaderText = "Enrollment";
            btn2.Text = "Enroll";
            btn2.Name = "btn2";
            btn2.UseColumnTextForButtonValue = true;
            dgvStudentList.Columns.Add(btn2);

            DataGridViewButtonColumn btn = new DataGridViewButtonColumn();
            btn.HeaderText = "Action";
            btn.Text = "Delete";
            btn.Name = "btn";
            btn.UseColumnTextForButtonValue = true;
            dgvStudentList.Columns.Add(btn);

            
        }

        private void btn_addnew_Click(object sender, EventArgs e)
        {
            UCStudent uCStudent = new UCStudent();
            uCStudent.ID = null;
            FrmMenu main = System.Windows.Forms.Application.OpenForms["FrmMenu"] != null ? (FrmMenu)System.Windows.Forms.Application.OpenForms["FrmMenu"] : null;
            if(main == null)
            {
                main = new FrmMenu();
            }
            main.pnUC.Controls.Clear();
            main.pnUC.Controls.Add(uCStudent);
        }

        private void dgvStudentList_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1)
            {
                int studentId = Convert.ToInt32(dgvStudentList.Rows[e.RowIndex].Cells["gc_studentid"].Value);
                if (e.ColumnIndex == dgvStudentList.Columns["gc_studentid"].Index)
                {
                    UCStudent ucStudent = new UCStudent();
                    ucStudent.ID = studentId.ToString();
                    this.Controls.Clear();
                    this.Controls.Add(ucStudent);

                }
            }
            if(dgvStudentList.CurrentCell.ColumnIndex.Equals(10)) {
                int studentId = Convert.ToInt32(dgvStudentList.Rows[e.RowIndex].Cells["gc_studentid"].Value);
                System.Data.DataTable dtb = enrollmentService.Get(studentId);
                int e_id = 0;
                if(dtb.Rows.Count > 0)
                {
                     e_id = Convert.ToInt32(dtb.Rows[0]["enrollment_id"]);
                }
                
                bool success = false;
                if (studentService.Delete(studentId))
                {
                    enrollmentService.Delete(e_id);
                    success = true;
                }
                  
                if (success)
                {
                    MessageBox.Show("Delete Success.", "Success", MessageBoxButtons.OK);
                }
                this.Controls.Clear();
                UCStudentList ucStudentList = new UCStudentList();
                this.Controls.Add(ucStudentList);
            }
            if (dgvStudentList.CurrentCell.ColumnIndex.Equals(9))
            {
                int studentId = Convert.ToInt32(dgvStudentList.Rows[e.RowIndex].Cells["gc_studentid"].Value);
                
                this.Controls.Clear();
                 UCEnrollment uCEnrollment = new UCEnrollment();
                uCEnrollment.student_id = studentId;
                this.Controls.Add(uCEnrollment);
            }
        }

        private void btn_export_Click(object sender, EventArgs e)
        {
            
            Microsoft.Office.Interop.Excel.Application app = new Microsoft.Office.Interop.Excel.Application();
            Microsoft.Office.Interop.Excel.Workbook workbook = app.Workbooks.Add(Type.Missing);
            Microsoft.Office.Interop.Excel.Worksheet worksheet = null;
            worksheet = workbook.Worksheets["Sheet1"];
            worksheet = workbook.ActiveSheet;
            worksheet.Name = "StudentList";

            for(int i = 1; i <= dgvStudentList.Columns.Count-2; i++) 
            {
                worksheet.Cells[1, i] = dgvStudentList.Columns[i - 1].HeaderText;
            }
            for(int i = 0; i < dgvStudentList.Rows.Count; i++)
            {
                for(int j = 0; j < dgvStudentList.Columns.Count-2; j++)
                {
                    worksheet.Cells[i+2, j+1] = dgvStudentList.Rows[i].Cells[j].Value.ToString();
                }
            }

            var saveFileDialoge = new SaveFileDialog();
            saveFileDialoge.FileName = "output";
            saveFileDialoge.DefaultExt = ".xlsx";
            if(saveFileDialoge.ShowDialog() == DialogResult.OK)
            {
                workbook.SaveAs(saveFileDialoge.FileName, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlExclusive, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);

            }
            app.Quit();
        }
    }
}
