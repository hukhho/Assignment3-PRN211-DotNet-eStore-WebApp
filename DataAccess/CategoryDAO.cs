using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObject;

namespace DataAccess.Repository
{
    public class CategoryDAO
    {
        private static CategoryDAO instance = null;

        private static readonly object instanceLock = new object();
        
        public static CategoryDAO Instance
        {
            get
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new CategoryDAO();
                    }
                    return instance;
                }
            }
        }

        public IEnumerable<Category> GetCategoryList()
        {
            var Categories = new List<Category>();
            try
            {
                using var context = new FStoreDBContext();
                Categories = context.Categories.ToList();
            } catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return Categories;
        }


        public Category GetCategoryByID(int CategoryID)
        {
            Category Category = null;
            try
            {
                using var context = new FStoreDBContext();
                Category = context.Categories.SingleOrDefault(m => m.CategoryId == CategoryID);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return Category;
        }


        public void AddNew(Category Category)
        {       
            try
            {
                Category _Category = GetCategoryByID(Category.CategoryId);
                if (_Category == null)
                {
                    using var context = new FStoreDBContext();
                    context.Categories.Add(Category);
                    context.SaveChanges();
                } else
                {
                    throw new Exception("Category is already exist!!!");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }       
        }

        public void Update(Category Category)
        {
            try { 
                Category _Category = GetCategoryByID(Category.CategoryId);
                if (Category != null) {
                    using var context = new FStoreDBContext();
                    context.Categories.Update(Category);
                    context.SaveChanges();
                } else {
                    throw new Exception("Category does not already exists.");
                }
            } catch (Exception ex) {
                throw new Exception(ex.Message);
            }

        }

        public void Remove(int CategoryID)
        {
            try {
                Category Category = GetCategoryByID(CategoryID);
                if (Category != null) {
                    using var context = new FStoreDBContext();
                    context.Categories.Remove(Category);
                    context.SaveChanges();
                } else {
                    throw new Exception("Category does not already exists.");
                }
            } catch (Exception ex) {
                throw new Exception(ex.Message);
            }
        }
    }


}
