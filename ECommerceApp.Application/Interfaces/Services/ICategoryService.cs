using System;
using System.Collections.Generic;
using System.Text;
using ECommerceApp.Application.DTOs.CategoryDtos;

namespace ECommerceApp.Application.Interfaces.Services
{
   public interface ICategoryService
    {
        public CategoryDashboardDto GetDashboard();

        public Task<List<GetCategoryDto>> GetAll();
        public GetCategoryDto GetById(int id);
        public void Add(AddCategoryDto dto);
        public  Task Update(UpdateCategoryDto dto);
        public void Delete(int id);


    }
}
