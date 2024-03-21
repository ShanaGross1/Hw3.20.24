using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageShareLikes.Data
{
    public class ImageRepository
    {
        private readonly string _connectionString;

        public ImageRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<Image> GetAll()
        {
            using var context = new ImageDataContext(_connectionString);
            return context.Images.ToList();
        }

        public Image GetById(int id)
        {
            using var context = new ImageDataContext(_connectionString);
            return context.Images.FirstOrDefault(i => i.Id == id);
        }

        public void AddImage(Image image)
        {
            using var context = new ImageDataContext(_connectionString);
            context.Images.Add(image);
            context.SaveChanges();
        }

        public void IncrementLikes(int id)
        {
            using var context = new ImageDataContext(_connectionString);
            var image = GetById(id);
            if (image != null)
            {
                image.Likes++;
                context.Entry(image).State = EntityState.Modified;
                context.SaveChanges();
            }
        }

        public int GetLikesForImage(int id)
        {
            using var context = new ImageDataContext(_connectionString);
            var image = GetById(id);
            return image != null ? image.Likes : 0;
        }
    }
}
