using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObject;

namespace DataAccess.Repository
{
    public class CategoryRepository : ICategoryRepository
    {
        public IEnumerable<Category> GetCategorys() => CategoryDAO.Instance.GetCategoryList();
        public Category GetCategoryByID(int CategoryID) => CategoryDAO.Instance.GetCategoryByID(CategoryID);
        public void InsertCategory(Category Category) => CategoryDAO.Instance.AddNew(Category);
        public void DeleteCategory(int CategoryID) => CategoryDAO.Instance.Remove(CategoryID);
        public void UpdateCategory(Category Category) => CategoryDAO.Instance.Update(Category);


    }
}
