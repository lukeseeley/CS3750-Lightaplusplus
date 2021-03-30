using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

namespace Lightaplusplus.Pages.Courses.Grades
{
    public class SubmissionsModel : PageModel
    {
        private readonly Lightaplusplus.Data.Lightaplusplus_SystemContext _context;

        public SubmissionsModel(Lightaplusplus.Data.Lightaplusplus_SystemContext context)
        {
            _context = context;
        }

        [BindProperty]
        public int id { get; set; }

        public string ChartJson { get; internal set; }

        [BindProperty]
        public List<AssignmentSubmissions> SubmissionsList { get; set; }

        [BindProperty]
        public List<StudentSubmission> StudentSubmissions { get; set; }

        [BindProperty]
        public Models.Assignments Assignment { get; set; }

        public async Task<IActionResult> OnGetAsync(int assignmentId, int sectionId)
        {
            var id = Session.getUserId(HttpContext.Session);
            var userType = Session.getUserType(HttpContext.Session);
            ViewData["UserId"] = id;
            ViewData["UserType"] = userType;
            var path = UserValidator.validateUser(_context, HttpContext.Session, 'I');
            if (path != "") return RedirectToPage(path);
            path = UserValidator.validateUser(_context, HttpContext.Session, new KeyPairId("Sec", sectionId));
            if (path != "") return RedirectToPage(path);

            //get the assignment
            var assignment = await _context.Assignments.FirstOrDefaultAsync(a => a.AssignmentId == assignmentId);

            if (assignment == null || assignment.SectionId != sectionId)
            {
                return RedirectToPage("/Courses/View/Index", new { sectionId });
            }

            // get all the grades for this assignment
            var grades = await _context.Grades.Where(g => g.AssignmentId == assignmentId).ToListAsync();
            //create a list of each letter grade
            List<double> gradesPercent = new List<double>();
            List<double> A = new List<double>();
            List<double> B = new List<double>();
            List<double> C = new List<double>();
            List<double> D = new List<double>();
            List<double> F = new List<double>();
            //sort the grades into their grade
            foreach(var g in grades)
            {
                gradesPercent.Add((double)g.GradeValue / (double)assignment.AssignmentMaxPoints);
            }
            foreach(var g in gradesPercent)
            {
                if(g > .89)
                {
                    A.Add(g);
                }
                else if(g > .79)
                {
                    B.Add(g);
                }
                else if (g > .69)
                {
                    C.Add(g);
                }
                else if (g > .59)
                {
                    D.Add(g);
                }
                else
                {
                    F.Add(g);
                }
            }

            // create and fill datatable for chart
            DataTable ChartData = new DataTable();
            ChartData.Columns.Add("Grade", typeof(System.String));
            ChartData.Columns.Add("Student", typeof(System.Double));
            ChartData.Rows.Add("A", A.Count);
            ChartData.Rows.Add("B", B.Count);
            ChartData.Rows.Add("C", C.Count);
            ChartData.Rows.Add("D", D.Count);
            ChartData.Rows.Add("F", F.Count);

            // for line graph

            //StaticSource source = new StaticSource(ChartData);
            //DataModel model = new DataModel();
            //model.DataSources.Add(source);
            //Charts.LineChart line = new Charts.LineChart("scroll_chart_db");
            //line.Scrollable = true;
            //line.Data.Source = model;
            //line.Caption.Text = assignment.AssignmentTitle + " Grades";
            ////line.SubCaption.Text = "2016-2017";
            //line.XAxis.Text = "Letter Grade";
            //line.YAxis.Text = "Number of Students";
            //line.Width.Pixel(900);
            //line.Height.Pixel(500);
            //line.ThemeName = FusionChartsTheme.ThemeName.FUSION;
            //ChartJson = line.Render();

            // for bar graph

            // Create static source with this data table
            StaticSource source = new StaticSource(ChartData);
            // Create instance of DataModel class
            DataModel model = new DataModel();
            // Add DataSource to the DataModel
            model.DataSources.Add(source);
            // Instantiate Column Chart
            Charts.ColumnChart column = new Charts.ColumnChart("first_chart");
            // Set Chart's width and height
            column.Width.Pixel(900);
            column.Height.Pixel(500);
            // Set DataModel instance as the data source of the chart
            column.Data.Source = model;
            // Set Chart Title
            column.Caption.Text = assignment.AssignmentTitle + " Grades";
            // Set chart sub title
            //column.SubCaption.Text = "2017-2018";
            // hide chart Legend
            column.Legend.Show = false;
            // set XAxis Text
            column.XAxis.Text = "Letter Grade";
            // Set YAxis title
            column.YAxis.Text = "Number of Students";
            // set chart theme
            column.ThemeName = FusionChartsTheme.ThemeName.FUSION;

            column.Export.Enabled = true;
            column.Export.ExportedFileName = assignment.AssignmentTitle + "_Grades";
            column.Export.Action = Exporter.ExportAction.DOWNLOAD;


            // set chart rendering json
            ChartJson = column.Render();

            SubmissionsList = await _context.AssignmentSubmissions
                .Include(t => t.Student)
                .Include(a => a.Assignment)
                .Where(e => e.AssignmentId == assignmentId)
                .ToListAsync();

            StudentSubmissions = new List<StudentSubmission>();

            foreach(var studentAssignment in SubmissionsList)
            {
                var submission = await _context.AssignmentSubmissions
                    .Where(s => s.StudentId == studentAssignment.StudentId)
                    .Where(a => a.AssignmentId == assignmentId)
                    .FirstOrDefaultAsync();

                var grade = await _context.Grades
                    .Where(s => s.StudentId == studentAssignment.StudentId)
                    .Where(a => a.AssignmentId == assignmentId)
                    .FirstOrDefaultAsync();

                if(submission == null)
                {
                    break;
                }

                StudentSubmissions.Add(new StudentSubmission(submission, grade));
            }

            Assignment = assignment;

            return Page();
        }
    }
}

