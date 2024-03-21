using ImageShareLikes.Data;
using ImageShareLikes.Data.Migrations;
using ImageShareLikesHw.Web.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Text.Json;
using static System.Net.Mime.MediaTypeNames;

namespace ImageShareLikesHw.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly string _connectionString;
        private IWebHostEnvironment _environment;

        public HomeController(IConfiguration configuration, IWebHostEnvironment environment)
        {
            _connectionString = configuration.GetConnectionString("ConStr");
            _environment = environment;
        }

        public IActionResult Index()
        {
            var repo = new ImageRepository(_connectionString);

            return View(new ImageViewModel
            {
                Images = repo.GetAll()
            });
        }

        public IActionResult Upload()
        {
            return View();
        }


        [HttpPost]
        public IActionResult Upload(IFormFile imagePath, string title)
        {

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(imagePath.FileName)}";
            using var stream = new FileStream(Path.Combine(_environment.WebRootPath, "images", fileName), FileMode.CreateNew);
            imagePath.CopyTo(stream);

            var repo = new ImageRepository(_connectionString);
            repo.AddImage(new() { PostedDate = DateTime.Now, imagePath = fileName, Title = title, Likes = 0 });
            return Redirect("/");
        }


        public IActionResult ViewImage(int id)
        {
            var repo = new ImageRepository(_connectionString);

            return View(new ImageViewModel
            {
                Image = repo.GetById(id)
            });
        }

        public IActionResult DidLikeImage(int id)
        {
            var imagesLiked = HttpContext.Session.Get<List<int>>("ids");
            var didLike = imagesLiked != null && imagesLiked.Contains(id);
            return Json(didLike);
        }

        [HttpPost]
        public void IncrementLikes(int id)
        {
            var imagesLiked = HttpContext.Session.Get<List<int>>("ids") != null ? HttpContext.Session.Get<List<int>>("ids") : new List<int>();
            imagesLiked.Add(id);
            HttpContext.Session.Set("ids", imagesLiked);
            var repo = new ImageRepository(_connectionString);
            repo.IncrementLikes(id);
        }

        public IActionResult GetLikesById(int id)
        {
            var repo = new ImageRepository(_connectionString);
            var likes = repo.GetLikesForImage(id);
            return Json(likes);
        }
    }
    public static class SessionExtensions
    {
        public static void Set<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonSerializer.Serialize(value));
        }

        public static T Get<T>(this ISession session, string key)
        {
            string value = session.GetString(key);

            return value == null ? default(T) :
            JsonSerializer.Deserialize<T>(value);
        }
    }

}