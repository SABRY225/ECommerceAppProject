using System;
using System.Collections.Generic;
using System.Text;
using ECommerceApp.Application.DTOs.CategoryDtos;
using ECommerceApp.Application.Interfaces.Rebositories;
using ECommerceApp.Application.Interfaces.Services;
using ECommerceApp.Domain.Entities;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace ECommerceApp.Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IGenericRebository<Category> _genericRebository;

        public CategoryService(IGenericRebository<Category> genericRebository)
        {
            _genericRebository = genericRebository;
        }

        public CategoryDashboardDto GetDashboard()
        {
            var categories =_genericRebository.GetAll().Include(c=>c.Products).Where(c=>c.IsDeleted==false).ToList();
            var totalCategories =categories.Count();
            

            var activeProducts=categories.SelectMany(c=>c.Products).Count();

            var topCategory=categories.OrderByDescending(c=>c.Products.Count()).FirstOrDefault();

            var lastUpdated = categories.Max(c => c.UpdatedAt ?? c.CreatedAt);
            var categoryDashDto = new CategoryDashboardDto
            {
                TotalCategories = totalCategories,
                ActiveProducts = activeProducts,
                TopPerforming = topCategory?.CategoryName,
                LastUpdated = lastUpdated




            };









            return categoryDashDto;

       

        }


        public IQueryable<GetCategoryDto> GetAll()
        {
            var categories = _genericRebository.GetAll();
            var categoriesDto = categories.Adapt<IQueryable<GetCategoryDto>>();
            return categoriesDto;


        }

        public GetCategoryDto GetById(int id)
        {
            var category = _genericRebository.GetAll().FirstOrDefault(c => c.Id == id && !c.IsDeleted);

            if (category == null)
            {
                Console.WriteLine("Category not found");
                return null;

            }
            var categoryDto = category.Adapt<GetCategoryDto>();
            return categoryDto;
        }
        public void Add(AddCategoryDto newCategoryDto)
        {
            var newCategory = newCategoryDto.Adapt<Category>();
            _genericRebository.Add(newCategory);
        }

        public void Delete(int id)
        {
            var category = _genericRebository.GetAll().FirstOrDefault(c => c.Id == id && !c.IsDeleted);

            if (category == null)
            {
                Console.WriteLine("Category not found");
                return;

            }
            _genericRebository.Delete(category);
        }


        public void Update(UpdateCategoryDto categoryDto)
        {
            var category = _genericRebository.GetAll().FirstOrDefault(c => c.Id == categoryDto.Id);
            if (category == null)
            {
                Console.WriteLine("Category not found");
                return;

            }

            _genericRebository.Update(category);


        }
    }
}
