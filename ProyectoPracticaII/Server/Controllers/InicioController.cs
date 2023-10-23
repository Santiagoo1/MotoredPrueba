﻿using Microsoft.AspNetCore.Mvc;

using ProyectoPracticaII.Client.Models;
using ProyectoPracticaII.Client.Recursos;
using ProyectoPracticaII.Client.Contrato;

using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;


namespace ProyectoPracticaII.Server.Controllers
{
    public class InicioController : Controller
    {
        private readonly IUsuarioService _usuarioService;

        public InicioController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;   
        }


        public IActionResult Registrarse()
        {
            return View();
        }


        [HttpPost]

        public async Task<IActionResult> Registrarse(Usuario modelo)
        {
            modelo.Clave = Utilidades.EncriptarClave(modelo.Clave);

            Usuario usuario_creado = await _usuarioService.SaveUsuario(modelo);

            if (usuario_creado.IdUsuario > 0)
                return RedirectToAction("IniciarSesion", "Inicio");

            ViewData["Mensaje"] = "No se pudo crear el usuario";
            return View();
        }

        public IActionResult IniciarSesion()
        {
            return View();
        }

        [HttpPost]

        public async Task<IActionResult> IniciarSesion(string correo, string clave)
        {

            Usuario usuario_encontrado = await _usuarioService.GetUsuario(correo, Utilidades.EncriptarClave(clave));

            if(usuario_encontrado == null)
            {
                ViewData["Mensaje"] = "No se encontraron coinsidencias";
                return View();
            }

            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, usuario_encontrado.NombreUsuario)
            };

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            AuthenticationProperties properties = new AuthenticationProperties()
            {
                AllowRefresh = true
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                properties
                );

            return RedirectToAction("Index","Home");
        }







    }

}
