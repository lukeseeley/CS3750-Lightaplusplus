using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using FusionCharts.DataEngine;
using FusionCharts.Visualization;
using Lightaplusplus.BisLogic;
using Lightaplusplus.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Lightaplusplus.Pages.Courses.View
{
    public class IndexModel : PageModel
    {
        private readonly Lightaplusplus.Data.Lightaplusplus_SystemContext _context;

        public IndexModel(Lightaplusplus.Data.Lightaplusplus_SystemContext context)
        {
            _context = context;
        }

        public int SectionId { get; set; }
        public double FinalGrade { get; set; }
        public double MinGrade { get; set; }
        public double MaxGrade { get; set; }
        public double AvgGrade { get; set; }
        public string ChartJson { get; internal set; }
        public string ChartJson2 { get; internal set; }
        public string UserType { get; set; }


        public Sections Section { get; set; }

        public List<Models.Assignments> Assignments { get; set; }

        [BindProperty]
        public Notifications Notifications { get; set; }

        public async Task<IActionResult> OnGetAsync(int sectionId)
        {
            var id = Session.getUserId(HttpContext.Session);
            var userType = Session.getUserType(HttpContext.Session);
            ViewData["UserId"] = id;
            ViewData["UserType"] = userType;
            var path = UserValidator.validateUser(_context, HttpContext.Session, new KeyPairId("Sec", sectionId));
            if (path != "") return RedirectToPage(path);

            SectionId = sectionId;

            Section = await _context.Sections.Include(s => s.Course).FirstOrDefaultAsync(s => s.SectionId == SectionId);

            Assignments = await _context.Assignments.Where(a => a.SectionId == SectionId).ToListAsync();

            UserType = userType;

            if (userType == "S")
            {
                var grades = await _context.Grades.Where(g => g.StudentId == id).ToListAsync();

                int maxPoints = 0;
                int currPoints = 0;

                foreach (var a in Assignments)
                {
                    foreach (var g in grades)
                    {
                        if (a.AssignmentId == g.AssignmentId)
                        {
                            maxPoints += (int)a.AssignmentMaxPoints;
                            currPoints += g.GradeValue;
                        }
                    }
                }

                if(maxPoints > 0)
                {
                    FinalGrade = (currPoints / maxPoints) * 100;
                }
            }

            var students = await _context.SectionStudents.Where(s => s.SectionId == sectionId).ToListAsync();

            List<double> studGrades = new List<double>();

            foreach (var s in students)
            {
                var grade = await _context.Grades.Where(g => g.StudentId == s.StudentId).ToListAsync();

                double maxPoint = 0;
                double currPoint = 0;
                

                foreach (var a in grade)
                {
                    foreach (var g in Assignments)
                    {
                        if (a.AssignmentId == g.AssignmentId)
                        {
                            maxPoint += (int)g.AssignmentMaxPoints;
                            currPoint += a.GradeValue;
                        }
                    }
                }
                if(maxPoint > 0)
                {
                    studGrades.Add((currPoint / maxPoint) * 100);
                }
                
            }

            double total = 0;
            studGrades.Sort();

            foreach (var s in studGrades)
            {
                total += s;
            }

            if(studGrades.Count > 0)
            {
                MinGrade = studGrades.ElementAt(0);
                MaxGrade = studGrades.ElementAt(studGrades.Count - 1);
            }
            

            //if (studGrades.Count % 2 == 0)
            //{
            //    int i = studGrades.Count / 2;
            //    Median = ((studGrades.ElementAt(i) + studGrades.ElementAt(i + 1)) / 2).ToString();

            //    Q1 = studGrades.ElementAt(i / 2).ToString();
            //    Q3 = studGrades.ElementAt(i + (i / 2)).ToString();
            //}
            //else
            //{
            //    int i = studGrades.Count / 2;
            //    Median = studGrades.ElementAt(i).ToString();

            //    Q1 = ((studGrades.ElementAt(studGrades.Count / 4) + studGrades.ElementAt(studGrades.Count / 4 + 1)) / 2).ToString();
            //    Q3 = ((studGrades.ElementAt(studGrades.Count / 4 + i) + studGrades.ElementAt(studGrades.Count / 4 + 1 + i)) / 2).ToString();
            //}

            AvgGrade = total / studGrades.Count();


            // create data table to store data
            DataTable ChartData = new DataTable();
            // Add columns to data table
            ChartData.Columns.Add("Stat", typeof(System.String));
            ChartData.Columns.Add("Grade", typeof(System.Double));
            // Add rows to data table

            ChartData.Rows.Add("Min", MinGrade);
            ChartData.Rows.Add("Average", AvgGrade);
            ChartData.Rows.Add("Max", MaxGrade);
            if (userType == "S")
            {
                ChartData.Rows.Add("Yours", FinalGrade);
            }


            // Create static source with this data table
            StaticSource source = new StaticSource(ChartData);
            // Create instance of DataModel class
            DataModel model = new DataModel();
            // Add DataSource to the DataModel
            model.DataSources.Add(source);
            // Instantiate Column Chart
            Charts.ColumnChart column = new Charts.ColumnChart("first_chart");
            // Set Chart's width and height
            column.Width.Pixel(700);
            column.Height.Pixel(400);
            // Set DataModel instance as the data source of the chart
            column.Data.Source = model;
            // Set Chart Title
            column.Caption.Text = "Class Grades";
            // Set chart sub title
            //column.SubCaption.Text = "2017-2018";
            // hide chart Legend
            column.Legend.Show = false;
            // set XAxis Text
            column.XAxis.Text = "Stats";
            // Set YAxis title
            column.YAxis.Text = "Grade";
            // set chart theme
            column.ThemeName = FusionChartsTheme.ThemeName.FUSION;
            // set chart rendering json
            ChartJson = column.Render();

            if(userType == "I")
            {
                //create a list of each letter grade
                List<double> A = new List<double>();
                List<double> B = new List<double>();
                List<double> C = new List<double>();
                List<double> D = new List<double>();
                List<double> F = new List<double>();

                foreach (var g in studGrades)
                {
                    if (g > 89)
                    {
                        A.Add(g);
                    }
                    else if (g > 79)
                    {
                        B.Add(g);
                    }
                    else if (g > 69)
                    {
                        C.Add(g);
                    }
                    else if (g > 59)
                    {
                        D.Add(g);
                    }
                    else if (g > 0)
                    {
                        F.Add(g);
                    }
                }

                // create and fill datatable for chart
                DataTable ChartData2 = new DataTable();
                ChartData2.Columns.Add("Grade", typeof(System.String));
                ChartData2.Columns.Add("Student", typeof(System.Double));
                ChartData2.Rows.Add("A", A.Count);
                ChartData2.Rows.Add("B", B.Count);
                ChartData2.Rows.Add("C", C.Count);
                ChartData2.Rows.Add("D", D.Count);
                ChartData2.Rows.Add("F", F.Count);

                // Create static source with this data table
                StaticSource source2 = new StaticSource(ChartData2);
                // Create instance of DataModel class
                DataModel model2 = new DataModel();
                // Add DataSource to the DataModel
                model2.DataSources.Add(source2);
                // Instantiate Column Chart
                Charts.ColumnChart column2 = new Charts.ColumnChart("first_chart");
                // Set Chart's width and height
                column2.Width.Pixel(900);
                column2.Height.Pixel(500);
                // Set DataModel instance as the data source of the chart
                column2.Data.Source = model2;
                // Set Chart Title
                column2.Caption.Text = "Grades";
                // Set chart sub title
                //column.SubCaption.Text = "2017-2018";
                // hide chart Legend
                column2.Legend.Show = false;
                // set XAxis Text
                column2.XAxis.Text = "Letter Grade";
                // Set YAxis title
                column2.YAxis.Text = "Number of Students";
                // set chart theme
                column2.ThemeName = FusionChartsTheme.ThemeName.FUSION;

                column2.Export.Enabled = true;
                column2.Export.ExportedFileName = "Grades";
                column2.Export.Action = Exporter.ExportAction.DOWNLOAD;


                // set chart rendering json
                ChartJson2 = column2.Render();
            }

            if ((string)ViewData["UserType"] == "S")
            {
                Notifications = new Notifications(HttpContext.Session, _context);
            }

            return Page();
        }
    }
}
