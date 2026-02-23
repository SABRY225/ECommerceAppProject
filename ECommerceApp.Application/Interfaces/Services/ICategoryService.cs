using System;
using System.Collections.Generic;
using System.Text;
using ECommerceApp.Application.DTOs.CategoryDtos;

namespace ECommerceApp.Application.Interfaces.Services
{
   public interface ICategoryService
    {
        public CategoryDashboardDto GetDashboard();

        public IQueryable<GetCategoryDto> GetAll();
        public GetCategoryDto GetById(int id);
        public void Add(AddCategoryDto dto);
        public  void Update(UpdateCategoryDto dto);
        public void Delete(int id);


    }
}
