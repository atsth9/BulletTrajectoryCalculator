using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BulletTrajectoryCalculator.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Message = "Modify this template to jump-start your ASP.NET MVC application.";

            return View();
        }

       
        [HttpPost]
        public ActionResult Calculate(string InputSlantRange, string InputElevationAngle)
        {
            
           
            ViewBag.Title = "Your angle of correction is: " + GetAngleOfCorrection(InputSlantRange, InputElevationAngle) + "'";
            return View();
        }



        public double GetAngleOfCorrection(string InputSlantRange, string InputElevationAngle)
        {
             //declare variables
            double  angle = 0;
            double SlantRange = 0;
            double ElevationAngle = 0;
            double BulletDrop = 0;
            int MaxRangeIndex = 0;
            double HighRange = 0;
            double LowRange = 0;
            double HighElevation = 0;
            double LowElevation = 0;

            //declare and initialize bulletdrop table
            double[,] BulletDropTable = new double[,] { { 0, 100, 200, 300, 400, 500 }, { 1.5, 0, -2.9, -11, -25.2, -46.4 } };

            //parse the strings into numbers (and validate the data)
            bool check1 = Double.TryParse(InputSlantRange, out SlantRange);
            bool check2 = Double.TryParse(InputElevationAngle, out ElevationAngle);
            ViewBag.SlantRange = "Slant Range: " + SlantRange;
            ViewBag.ElevationAngle = "Elevation Angle: " + ElevationAngle;

            //calculate equivalent horizontal range 
            double HorizontalRange = SlantRange * Math.Cos(ElevationAngle*.0174532925);
            //TODO check if horizontal range is > 500

            //calculate bullet drop using linear interpolation
            //loop through the bulletdrop table to find which two values it lies between
            for (int i = 0; i < 6; i++)
            {
                if (BulletDropTable[0,i] > HorizontalRange)
                {
                    MaxRangeIndex = i;
                    break;
                }
            }
            //pull the correct values from BulletDropTable and store them into easy to read variables
            HighRange = BulletDropTable[0,MaxRangeIndex];
            LowRange  = BulletDropTable[0,MaxRangeIndex - 1];
            HighElevation  = BulletDropTable[1,MaxRangeIndex];
            LowElevation = BulletDropTable[1,MaxRangeIndex - 1];
            //use equation to calculate bullet drop correction in Minutes
            BulletDrop = (HighElevation*(HorizontalRange - LowRange) + LowElevation*(HighRange - HorizontalRange))/(HighRange-LowRange);
            angle = Math.Round(((-((BulletDrop * .01) / HorizontalRange) * (180 / Math.PI)) * 60), 1);
            return angle;
        }
       
    }
}
