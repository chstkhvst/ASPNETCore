using ASPNETCore.Application.DTO;
using ASPNETCore.Domain.Entities;
using ASPNETCore.Domain.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace ASPNETCore.Application.Services
{
    public class REObjectServices
    {
        private readonly IREObjectRepository _reObjectRepository;
        private readonly IImageRepository _imageRepository;

        public REObjectServices(IREObjectRepository reObjectRepository, IImageRepository imageRepository)
        {
            _reObjectRepository = reObjectRepository;
            _imageRepository = imageRepository;
        }

        //Получение всех объектов недвижимости
        public async Task<IEnumerable<REObjectDTO>> GetAllAsync(bool isAdmin = false)
        {
            // Для не-админов сразу запрашиваем только активные объекты (statusId = 1)
            // Для админов получаем все объекты без фильтрации по статусу
            var objects = isAdmin
                ? await _reObjectRepository.GetAllAsync()
                : await _reObjectRepository.GetFilteredAsync(null, null, 1);

            return objects.Select(o => new REObjectDTO
            {
                Id = o.Id,
                Rooms = o.Rooms,
                Floors = o.Floors,
                Building = o.Building,
                Roomnum = o.Roomnum,
                Square = o.Square,
                Street = o.Street,
                DealTypeId = o.DealTypeId,
                Price = o.Price,
                TypeId = o.TypeId,
                StatusId = o.StatusId,
                Status = o.Status != null ? new Status { Id = o.Status.Id, StatusName = o.Status.StatusName } : null,
                ObjectType = o.ObjectType != null ? new ObjectType { Id = o.ObjectType.Id, TypeName = o.ObjectType.TypeName } : null,
                DealType = o.DealType != null ? new DealType { Id = o.DealType.Id, DealName = o.DealType.DealName } : null,
                ObjectImages = o.ObjectImages?.Select(i => new ObjectImagesDTO
                {
                    Id = i.Id,
                    ImagePath = i.ImagePath,
                    ObjectId = i.ObjectId
                }).ToList() ?? new List<ObjectImagesDTO>()
            });
        }
        // Получение объекта по ID
        public async Task<REObjectDTO> GetByIdAsync(int id)
        {
            var obj = await _reObjectRepository.GetByIdAsync(id);
            if (obj == null)
            {
                Console.WriteLine($"Объект с ID {id} не найден в БД!");
                return null;
            }

            return new REObjectDTO
            {
                Id = obj.Id,
                Rooms = obj.Rooms,
                Floors = obj.Floors,
                Building = obj.Building,
                Roomnum = obj.Roomnum,
                Square = obj.Square,
                Street = obj.Street,
                DealTypeId = obj.DealTypeId,
                Price = obj.Price,
                TypeId = obj.TypeId,
                StatusId = obj.StatusId,
                Status = obj.Status != null ? new Status { Id = obj.Status.Id, StatusName = obj.Status.StatusName } : null,
                ObjectType = obj.ObjectType != null ? new ObjectType { Id = obj.ObjectType.Id, TypeName = obj.ObjectType.TypeName } : null,
                DealType = obj.DealType != null ? new DealType { Id = obj.DealType.Id, DealName = obj.DealType.DealName } : null,
                ObjectImages = obj.ObjectImages?.Select(i => new ObjectImagesDTO
                {
                    Id = i.Id,
                    ImagePath = i.ImagePath,
                    ObjectId = i.ObjectId
                }).ToList() ?? new List<ObjectImagesDTO>()

            };
        }

        // Поиск объектов по названию
        public async Task<IEnumerable<REObjectDTO>> SearchByNameAsync(string street)
        {
            var objects = await _reObjectRepository.SearchByNameAsync(street);
            return objects.Select(o => new REObjectDTO
            {
                Id = o.Id,
                Rooms = o.Rooms,
                Floors = o.Floors,
                Building = o.Building,
                Roomnum = o.Roomnum,
                Square = o.Square,
                Street = o.Street,
                DealTypeId = o.DealTypeId,
                Price = o.Price,
                TypeId = o.TypeId,
                StatusId = o.StatusId
            });
        }

        // Добавление нового объекта
        //public async Task AddAsync(CreateREObjectDTO o)
        //{
        //    var obj = new REObject
        //    {
        //        Id = o.Id,
        //        Rooms = o.Rooms,
        //        Floors = o.Floors,
        //        Building = o.Building,
        //        Roomnum = o.Roomnum,
        //        Square = o.Square,
        //        Street = o.Street,
        //        DealTypeId = o.DealTypeId,
        //        Price = o.Price,
        //        TypeId = o.TypeId,
        //        StatusId = o.StatusId,
        //        ObjectImages = o.ObjectImages?.Select(img => new ObjectImages
        //        {
        //            ImagePath = img.ImagePath,
        //            ObjectId = o.Id
        //        }).ToList()
        //    };

        //    await _reObjectRepository.AddAsync(obj);
        //}
        private async Task<string> SaveImage(IFormFile file, IWebHostEnvironment env)
        {
            var uploadsFolder = Path.Combine(env.WebRootPath, "assets");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return $"/assets/{uniqueFileName}";
        }
        public async Task AddAsync(CreateREObjectDTO o, IFormFileCollection? files = null, IWebHostEnvironment? env = null)
        {
            var obj = new REObject
            {
                Id = o.Id,
                Rooms = o.Rooms,
                Floors = o.Floors,
                Building = o.Building,
                Roomnum = o.Roomnum,
                Square = o.Square,
                Street = o.Street,
                DealTypeId = o.DealTypeId,
                Price = o.Price,
                TypeId = o.TypeId,
                StatusId = o.StatusId,
                ObjectImages = o.ObjectImages?.Select(img => new ObjectImages
                {
                    ImagePath = img.ImagePath,
                    ObjectId = o.Id
                }).ToList() ?? new List<ObjectImages>()
            };

            // Добавляем файлы, если они есть
            await _reObjectRepository.AddAsync(obj);
            var savedId = obj.Id;

            if (files != null && env != null)
            {
                foreach (var file in files)
                {
                    var imagePath = await SaveImage(file, env);
                    await _imageRepository.AddAsync(new ObjectImages
                    {
                        ImagePath = imagePath,
                        ObjectId = savedId // Используем подтвержденный ID
                    });
                }
            }
        }
        public async Task UpdateAsync(
            CreateREObjectDTO o,
            IFormFileCollection? files = null,
            IWebHostEnvironment? env = null,
            IEnumerable<int>? imagesToDelete = null)
        {
            var obj = await _reObjectRepository.GetByIdAsync(o.Id);
            if (obj == null)
                return;

            // Обновляем основные свойства
            obj.Rooms = o.Rooms;
            obj.Floors = o.Floors;
            obj.Building = o.Building;
            obj.Roomnum = o.Roomnum;
            obj.Square = o.Square;
            obj.Street = o.Street;
            obj.DealTypeId = o.DealTypeId;
            obj.Price = o.Price;
            obj.TypeId = o.TypeId;
            obj.StatusId = o.StatusId;

            // Удаляем изображения, помеченные для удаления
            if (imagesToDelete != null && imagesToDelete.Any())
            {
                foreach (int id in imagesToDelete)
                    await _imageRepository.DeleteAsync(id);
            }

            // Добавляем новые файлы
            if (files != null && files.Count > 0 && env != null)
            {
                foreach (var file in files)
                {
                    var imagePath = await SaveImage(file, env);
                    await _imageRepository.AddAsync(new ObjectImages
                    {
                        ImagePath = imagePath,
                        ObjectId = o.Id
                    });
                }
            }

            await _reObjectRepository.UpdateAsync(obj);
        }

        // Удаление объекта
        public async Task DeleteAsync(int id)
        {
            try
            {
                await _reObjectRepository.DeleteAsync(id);
            }
            catch (InvalidOperationException ex)
            {
                throw new ApplicationException($"Ошибка при удалении объекта: {ex.Message}", ex);
            }
        }
        public async Task<IEnumerable<REObjectDTO>> GetFilteredAsync(
            int? typeId,
            int? dealTypeId,
            int? statusId,
            bool isAdmin = false)
        {
            // Для не-админов 1 по умолчанию
            var effectiveStatusId = isAdmin ? statusId : 1;

            var objects = await _reObjectRepository.GetFilteredAsync(
                typeId,
                dealTypeId,
                effectiveStatusId);

            return objects.Select(o => new REObjectDTO
            {
                Id = o.Id,
                Rooms = o.Rooms,
                Floors = o.Floors,
                Building = o.Building,
                Roomnum = o.Roomnum,
                Square = o.Square,
                Street = o.Street,
                DealTypeId = o.DealTypeId,
                Price = o.Price,
                TypeId = o.TypeId,
                StatusId = o.StatusId,
                Status = o.Status != null ? new Status { Id = o.Status.Id, StatusName = o.Status.StatusName } : null,
                ObjectType = o.ObjectType != null ? new ObjectType { Id = o.ObjectType.Id, TypeName = o.ObjectType.TypeName } : null,
                DealType = o.DealType != null ? new DealType { Id = o.DealType.Id, DealName = o.DealType.DealName } : null,
                ObjectImages = o.ObjectImages?.Select(i => new ObjectImagesDTO
                {
                    Id = i.Id,
                    ImagePath = i.ImagePath,
                    ObjectId = i.ObjectId
                }).ToList() ?? new List<ObjectImagesDTO>()
            });
        }
    }
}
