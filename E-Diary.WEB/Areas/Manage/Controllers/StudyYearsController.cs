﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using E_Diary.WEB.Data;
using E_Diary.WEB.Data.Entities;
using System.Diagnostics.Eventing.Reader;
using E_Diary.WEB.Areas.Manage.Models;

namespace E_Diary.WEB.Areas.Manage.Controllers
{
    /// <summary>
    /// Контроллер "Аттестационные периоды"
    /// </summary>
    [Area("Manage")]
    public class StudyYearsController : Controller
    {
        private readonly ASPIdentityDBContext _context;

        public StudyYearsController(ASPIdentityDBContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            return View(await _context.StudyYears.ToListAsync());
        }
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var studyYear = await _context.StudyYears
                .FirstOrDefaultAsync(m => m.Id == id);
            if (studyYear == null)
            {
                return NotFound();
            }

            return View(studyYear);
        }
        /// <summary>
        /// Переход на страницу создания нового учебного года
        /// </summary>
        public IActionResult Create()
        {
            return View();
        }
        /// <summary>
        /// Отправка формы на странице создания нового учебного года
        /// </summary>
        /// <param name="studyYear">Учебный год</param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Start,End")] StudyYear studyYear)
        {
            if (ModelState.IsValid)
            {
                //Проверяем валидность учебного года
                if (IsYearValid(studyYear))
                {
                    //Проверяем, нет ли уже учебного года в указанном диапазоне
                    if (!ContextHaveOtherYearInRange(studyYear))
                    {
                        _context.Add(studyYear);
                        await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(Index));
                    }
                    else ModelState.AddModelError(string.Empty, "Диапазон учебного года уже входит в диапазон другого учебного года");
                }
                else ModelState.AddModelError(string.Empty, "Учебный год не может заканчиваться раньше, чем начинается");
            }
            return View(studyYear);
        }
        /// <summary>
        /// Переход на страницу редактирования учебного года
        /// </summary>
        /// <param name="id">ID учебного года</param>
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var studyYear = await _context.StudyYears.FindAsync(id);
            if (studyYear == null)
            {
                return NotFound();
            }
            return View(studyYear);
        }
        /// <summary>
        /// Отправка формы на странице редактирования учебного года
        /// </summary>
        /// <param name="id"></param>
        /// <param name="studyYear"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Start,End")] StudyYear studyYear)
        {
            if (id != studyYear.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (IsYearValid(studyYear))
                    {
                        if (_context.CertificationPeriods.Any(p => (p.Start < studyYear.Start || p.End > studyYear.Start) && p.StudyYear.Id == studyYear.Id))
                            {
                            if (!ContextHaveOtherYearInRange(studyYear))
                            {
                                _context.Update(studyYear);
                                await _context.SaveChangesAsync();
                                return RedirectToAction(nameof(Index));
                            }
                            else ModelState.AddModelError(string.Empty, "Диапазон учебного года уже входит в диапазон другого учебного года");
                        }
                            else ModelState.AddModelError(string.Empty, "Одна из четвертей выходит за пределы нового диапазона");
                    }
                    else ModelState.AddModelError(string.Empty, "Учебный год не может заканчиваться раньше, чем начинается");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudyYearExists(studyYear.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return View(studyYear);
        }
        /// <summary>
        /// Удалить учебный год
        /// </summary>
        /// <param name="id">ID учебного года</param>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var studyYear = await _context.StudyYears.FindAsync(id);
            if (studyYear != null)
            {
                _context.StudyYears.Remove(studyYear);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StudyYearExists(int id)
        {
            return _context.StudyYears.Any(e => e.Id == id);
        }
        private bool IsYearValid(StudyYear year) => year.Start < year.End;
        private bool ContextHaveOtherYearInRange(StudyYear year)
        {
            if (year == null) return false;
            try
            {
                return _context.StudyYears.Any(y => y.Start < year.End && year.Start < y.End);
            }
            catch (Exception ex)
            {
                return false;
            }
        }




        public IActionResult CreateCertificationPeriod(int yearId)
        {
            return View(new CreateCertificationPeriodViewModel() { StudyYearId = yearId});
        }
        /// <summary>
        /// Создать аттестационный период (четверть, триместр, семестр)
        /// </summary>
        /// <param name="model">Модель представления для создания</param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCertificationPeriod(CreateCertificationPeriodViewModel model)
        {
            if (ModelState.IsValid)
            {
                StudyYear? year = await _context.StudyYears.FindAsync(model.StudyYearId);
                if (year == null) return BadRequest();
                CertificationPeriod period = new()
                {
                    Start = model.Start,
                    End = model.End,
                    StudyYear = year,
                    Name = model.Name
                };
                if (IsPeriodValid(period))
                {
                    if (!YearHaveOtherPeriodsInRange(period))
                    {
                        _context.Add(period);
                        await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(Index));
                    }
                    else ModelState.AddModelError(string.Empty, "Учебный год уже содержит диапазон, имеющий даты в данном промежутке");
                }
                else ModelState.AddModelError(string.Empty, "Диапазаон недействителен");

            }
            return View(model);
        }

        /// <summary>
        /// Редактирование аттестационного периода
        /// </summary>
        /// <param name="id">ID аттестационного периода</param>
        public async Task<IActionResult> EditCertificationPeriod(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var certificationPeriod = await _context.CertificationPeriods.FindAsync(id);
            if (certificationPeriod == null)
            {
                return NotFound();
            }
            EditCertificationPeriodViewModel model = new()
            {
                Id = certificationPeriod.Id,
                StudyYearId = certificationPeriod.StudyYear.Id,
                Name = certificationPeriod.Name,
                Start = certificationPeriod.Start,
                End = certificationPeriod.End
            };
            return View(model);
        }

        /// <summary>
        /// Отправка формы на странице редактирования аттестационного периода
        /// </summary>
        /// <param name="model">Модель представления для редактирования</param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCertificationPeriod(EditCertificationPeriodViewModel model)
        {
            CertificationPeriod? certificationPeriod = await _context.CertificationPeriods.FindAsync(model.Id);
            if (certificationPeriod == null) return NotFound();
            if (ModelState.IsValid)
            {
                try
                {
                    certificationPeriod.Name = model.Name;
                    certificationPeriod.Start = model.Start;
                    certificationPeriod.End = model.End;
                    if (IsPeriodValid(certificationPeriod))
                    {
                        if (!YearHaveOtherPeriodsInRange(certificationPeriod))
                        {
                            _context.Update(certificationPeriod);
                            await _context.SaveChangesAsync();
                            return RedirectToAction(nameof(Index));
                        }
                        else ModelState.AddModelError(string.Empty, "Учебный год уже содержит диапазон, имеющий даты в данном промежутке");
                    }
                    else ModelState.AddModelError(string.Empty, "Диапазаон недействителен");

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CertificationPeriodExists(certificationPeriod.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                
            }
            return View(model);
        }
        /// <summary>
        /// Удалить аттестационный период
        /// </summary>
        /// <param name="id">ID аттестационного периода</param>
        /// <returns></returns>
        [HttpPost, ActionName("DeleteCertificationPeriod")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCertificationPeriod(int id)
        {
            var certificationPeriod = await _context.CertificationPeriods.FindAsync(id);
            if (certificationPeriod != null)
            {
                _context.CertificationPeriods.Remove(certificationPeriod);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CertificationPeriodExists(int id)
        {
            return _context.CertificationPeriods.Any(e => e.Id == id);
        }

        private bool IsPeriodValid(CertificationPeriod period) => (period.Start < period.End) && (period.Start >= period.StudyYear.Start && period.End <= period.StudyYear.End);
        private bool YearHaveOtherPeriodsInRange(CertificationPeriod period)
        {
            if (period == null) return false;
            try
            {
                return _context.CertificationPeriods.Any(p => (p.Start < period.End && period.Start < p.End) && p.StudyYear.Id == period.StudyYear.Id && p.Id != period.Id);
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
