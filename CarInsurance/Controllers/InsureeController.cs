using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CarInsurance.Models;

namespace CarInsurance.Controllers
{
    public class InsureeController : Controller
    {
        private InsuranceEntities db = new InsuranceEntities();
        private List<Insuree> insurees = new List<Insuree>();

        // GET: Insuree
        public ActionResult Index()
        {
            return View(db.Insurees.ToList());
        }

        // GET: Insuree/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Insuree insuree = db.Insurees.Find(id);
            if (insuree == null)
            {
                return HttpNotFound();
            }
            return View(insuree);
        }

        // GET: Insuree/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Insuree/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id_,FirstName,LastName,Address,EmailAddress,DateOfBirth,CarYear,CarMake,CarModel,DUI,SpeedingTickets,CoverageType,Quote")] Insuree insuree)
        {
            if (ModelState.IsValid)
            {
                db.Insurees.Add(insuree);
                db.SaveChanges();
                return RedirectToAction("Index");

                // Calculate quote
                decimal baseQuote = 50;

                // Age calculation
                int age = DateTime.Now.Year - insuree.DateOfBirth.Year;
                if (insuree.DateOfBirth > DateTime.Now.AddYears(-age)) age--;

                if (age <= 18)
                {
                    baseQuote += 100;
                }
                else if (age >= 19 && age <= 25)
                {
                    baseQuote += 50;
                }
                else
                {
                    baseQuote += 25;
                }

                // Car year calculation
                if (insuree.CarYear < 2000 || insuree.CarYear > 2015)
                {
                    baseQuote += 25;
                }

                // Car make and model calculation
                if (insuree.CarMake.ToLower() == "porsche")
                {
                    baseQuote += 25;

                    if (insuree.CarModel.ToLower() == "911 carrera")
                    {
                        baseQuote += 25;
                    }
                }

                // Speeding tickets calculation
                baseQuote += insuree.SpeedingTickets * 10;

                // DUI calculation
                if (insuree.DUI)
                {
                    baseQuote *= 1.25m; // 25% increase
                }

                // Full coverage calculation
                if (insuree.CoverageType)
                {
                    baseQuote *= 1.5m; // 50% increase
                }

                insuree.Quote = baseQuote;

                return RedirectToAction("Index");
            }

                return View(insuree);
        }

        // GET: Insuree/Admin
        public ActionResult Admin()
        {
            // Display all quotes issued with user's first name, last name, and email
            var adminViewModel = insurees.Select(i => new AdminViewModel
            {
                FirstName = i.FirstName,
                LastName = i.LastName,
                Email = i.Email,
              
            }).ToList();

            return View(adminViewModel);
        }


        // GET: Insuree/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Insuree insuree = db.Insurees.Find(id);
            if (insuree == null)
            {
                return HttpNotFound();
            }
            return View(insuree);
        }

        // POST: Insuree/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id_,FirstName,LastName,EmailAddress,DateOfBirth,CarYear,CarMake,CarModel,DUI,SpeedingTickets,CoverageType,Quote")] Insuree insuree)
        {
            if (ModelState.IsValid)
            {
                db.Entry(insuree).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(insuree);
        }

        // GET: Insuree/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Insuree insuree = db.Insurees.Find(id);
            if (insuree == null)
            {
                return HttpNotFound();
            }
            return View(insuree);
        }

        // POST: Insuree/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Insuree insuree = db.Insurees.Find(id);
            db.Insurees.Remove(insuree);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }







    }
}
