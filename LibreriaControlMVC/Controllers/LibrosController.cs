﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LibreriaControlMVC.Context;
using LibreriaControlMVC.Models;

namespace LibreriaControlMVC.Controllers
{
    public class LibrosController : Controller
    {
        private readonly AppDbContext _context;

        public LibrosController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Libros
        public async Task<IActionResult> Index()
        {
            var libro = _context.Libros.Include(l => l.Editorial).Include(l => l.Genero);
            return View(await libro.ToListAsync());
        }

        // GET: Libros/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var libro = await _context.Libros
                .Include(l => l.Editorial)
                .Include(l => l.Genero)
                .Include(l => l.Autores).ThenInclude(a => a.Autor)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);
            if (libro == null)
            {
                return NotFound();
            }

            return View(libro);
        }

        // GET: Libros/Create
        public IActionResult Create()
        {
            ViewData["Autores"] = new SelectList(_context.Autores, "Id", "NombreCompleto");
            ViewData["EditorialId"] = new SelectList(_context.Editoriales, "Id", "Nombre");
            ViewData["GeneroId"] = new SelectList(_context.Generos, "Id", "Nombre");
            return View();
        }

        // POST: Libros/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Titulo,Publicacion,Stock,EditorialId,GeneroId")] Libro libro, List<int> autores)
        {
            if (ModelState.IsValid)
            {
                _context.Add(libro);
                foreach(var autor in autores)
                {
                    _context.Add(new LibroAutor() { AutorId = autor, Libro = libro });
                }
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Autores"] = new SelectList(_context.Autores, "Id", "NombreCompleto", autores);
            ViewData["EditorialId"] = new SelectList(_context.Editoriales, "Id", "Nombre", libro.EditorialId);
            ViewData["GeneroId"] = new SelectList(_context.Generos, "Id", "Nombre", libro.GeneroId);
            return View(libro);
        }

        // GET: Libros/Edit/5
        public  IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var libro = _context.Libros
                .Include(l => l.Autores)
                .AsNoTracking()
                .FirstOrDefault(li => li.Id == id);

            

            if (libro == null) 
            {
                return NotFound();
            }
            ViewData["Autores"] = new SelectList(_context.Autores, "Id", "NombreCompleto", libro.Autores.Select(autores => autores.AutorId));
            ViewData["EditorialId"] = new SelectList(_context.Editoriales, "Id", "Nombre", libro.EditorialId);
            ViewData["GeneroId"] = new SelectList(_context.Generos, "Id", "Nombre", libro.GeneroId);
            return View(libro);
        }

        // POST: Libros/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Titulo,Publicacion,Stock,EditorialId,GeneroId")] Libro libro, List<int> autores)
        {
            if (id != libro.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(libro);

                    var dbLibro = _context.Libros
                        .Include(lib => lib.Autores)
                        .FirstOrDefault(lib => lib.Id == id);

                    _context.LibrosAutores.RemoveRange(dbLibro.Autores);

                    foreach (var autor in autores)
                    {
                        _context.Add(new LibroAutor() { AutorId = autor, LibroId = id });
                    }

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LibroExists(libro.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["Autores"] = new SelectList(_context.Autores, "Id", "NombreApellido", autores);
            ViewData["EditorialId"] = new SelectList(_context.Editoriales, "Id", "Nombre", libro.EditorialId);
            ViewData["GeneroId"] = new SelectList(_context.Generos, "Id", "Nombre", libro.GeneroId);
            return View(libro);
        }

        // GET: Libros/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var libro = await _context.Libros
                .Include(l => l.Editorial)
                .Include(l => l.Genero)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);
            if (libro == null)
            {
                return NotFound();
            }

            return View(libro);
        }

        // POST: Libros/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var libro = await _context.Libros.FindAsync(id);
            _context.Libros.Remove(libro);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LibroExists(int id)
        {
            return _context.Libros.Any(e => e.Id == id);
        }
    }
}
