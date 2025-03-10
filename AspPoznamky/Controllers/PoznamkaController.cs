﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AspPoznamky.Data;
using AspPoznamky.Models;
using System;
using System.Linq;

namespace AspPoznamky.Controllers
{
    public class PoznamkaController : Controller
    {
        private readonly AspPoznamkyContext _context;

        public PoznamkaController(AspPoznamkyContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Vytvoreni()
        {
            ViewData["Uzivatel"] = HttpContext.Session.GetString("Uzivatel");

            return View();
        }

        [HttpPost]
        [ActionName("Vytvoreni")]
        public IActionResult VytvoreniZpracovani(string autor, string text)
        {
            if (text == null || text.Trim().Length == 0)
                return RedirectToAction("Vytvoreni", "Poznamka");

            Uzivatel uzivatel = _context.Uzivatele
                .Where(u => u.Jmeno == HttpContext.Session.GetString("Uzivatel"))
                .FirstOrDefault();

            if (uzivatel != null)
            {
                DateTime aktualniCas = DateTime.UtcNow;

                _context.Poznamky.Add(new Poznamka {
                    Autor = uzivatel,
                    Text = text,
                    DatumVytvoreni = aktualniCas,
                    DatumPosledniUpravy = aktualniCas
                });
                _context.SaveChanges();
            }

            return RedirectToAction("Profil", "Uzivatel");
        }

        public IActionResult Smazani(int id)
        {
            Poznamka poznamka = _context.Poznamky
                .Where(p => p.Id == id)
                .First();

            Uzivatel autor = _context.Uzivatele
                .Where(u => u == poznamka.Autor)
                .First();

            Uzivatel prihlaseny = _context.Uzivatele
                .Where(u => u.Jmeno == HttpContext.Session.GetString("Uzivatel"))
                .First();

            if (autor == prihlaseny)
            {
                _context.Remove(poznamka);
                _context.SaveChanges();
            }

            return RedirectToAction("Profil", "Uzivatel");
        }

        [HttpGet]
        public IActionResult Uprava(int id)
        {
            Poznamka poznamka = _context.Poznamky
                .Where(p => p.Id == id)
                .First();

            Uzivatel autor = _context.Uzivatele
                .Where(u => u == poznamka.Autor)
                .First();

            Uzivatel prihlaseny = _context.Uzivatele
                .Where(u => u.Jmeno == HttpContext.Session.GetString("Uzivatel"))
                .First();

            if(autor == prihlaseny)
            {
                return View(poznamka);
            }

            return RedirectToAction("Profil", "Uzivatel");
        }

        [HttpPost]
        [ActionName("Uprava")]
        public IActionResult UpravaZpracovani(int id, string text)
        {
            Poznamka poznamka = _context.Poznamky
                .Where(p => p.Id == id)
                .First();

            if (text != null && text.Trim().Length > 0)
            {
                poznamka.Text = text;
                poznamka.DatumPosledniUpravy = DateTime.Now;

                _context.Update(poznamka);
                _context.SaveChanges();
            }

            return RedirectToAction("Profil", "Uzivatel");
        }
    }
}
